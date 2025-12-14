using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using MyStore.Stocks;

namespace MyStore.Purchases
{
    public class PurchaseAppService : ApplicationService, IPurchaseAppService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly PurchaseManager _purchaseManager;

        public PurchaseAppService(
            IPurchaseRepository purchaseRepository,
            PurchaseManager purchaseManager
        )
        {
            _purchaseRepository = purchaseRepository;
            _purchaseManager = purchaseManager;
        }

        public async Task<PurchaseDto> GetAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetWithProductsAsync(id)
                ?? throw new UserFriendlyException("Purchase not found");

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        public async Task<PurchaseDto> GetByCodeAsync(string purchaseCode)
        {
            var purchase = await _purchaseRepository.GetByCodeAsync(purchaseCode)
                ?? throw new UserFriendlyException($"Purchase with code '{purchaseCode}' not found");

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        public async Task<List<PurchaseDto>> GetListAsync()
        {
            var purchases = await _purchaseRepository.GetListAsync(includeDetails: true);
            return ObjectMapper.Map<List<Purchase>, List<PurchaseDto>>(purchases);
        }

        public async Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input)
        {
            var products = input.Products.Select(p =>
                new PurchaseProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)
            ).ToList();

            var purchase = await _purchaseManager.CreatePurchaseAsync(
                input.PurchaseCode,
                input.SupplierName,
                input.DateTime,
                products,
                input.Discount,
                input.PaidAmount
            );

            await _purchaseRepository.InsertAsync(purchase, autoSave: true);

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        public async Task UpdateAsync(Guid id, CreateUpdatePurchaseDto input)
        {
            var purchase = await _purchaseRepository.GetWithProductsAsync(id)
                ?? throw new UserFriendlyException("Purchase not found");

            var products = input.Products.Select(p =>
                new PurchaseProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)
            ).ToList();

            await _purchaseManager.UpdatePurchaseAsync(
                purchase,
                products,
                input.SupplierName,
                input.DateTime,
                input.Discount,
                input.PaidAmount
            );

            await _purchaseRepository.UpdateAsync(purchase, autoSave: true);
        }

        public async Task DeleteAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetWithProductsAsync(id)
                ?? throw new UserFriendlyException("Purchase not found");

            await _purchaseManager.DeletePurchaseAsync(purchase);
            await _purchaseRepository.DeleteAsync(purchase, autoSave: true);
        }
    }
}
