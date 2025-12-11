using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace MyStore.Purchases
{
    public class PurchaseAppService : ApplicationService, IPurchaseAppService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly PurchaseManager _purchaseManager;

        public PurchaseAppService(IPurchaseRepository purchaseRepository, PurchaseManager purchaseManager)
        {
            _purchaseRepository = purchaseRepository;
            _purchaseManager = purchaseManager;
        }

        public async Task<PurchaseDto> GetAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetAsync(id);
            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase); // AutoMapper
        }

        public async Task<PurchaseDto> GetByCodeAsync(string purchaseCode)
        {
            var purchase = await _purchaseRepository.GetByCodeAsync(purchaseCode);
            if (purchase == null)
            {
                throw new UserFriendlyException($"Purchase with code '{purchaseCode}' not found.");
            }
            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase); // AutoMapper
        }

        public async Task<List<PurchaseDto>> GetListAsync()
        {
            var purchases = await _purchaseRepository.GetListAsync(includeDetails: true);
            return ObjectMapper.Map<List<Purchase>, List<PurchaseDto>>(purchases); // AutoMapper
        }
        public async Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input)
        {
            if (input.Products == null || input.Products.Count == 0)
                throw new UserFriendlyException("Purchase must contain at least one product.");

            // Map DTO products -> domain products
            var domainProducts = input.Products.ConvertAll(p =>
                new PurchaseProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)
            );

            // Create Purchase aggregate
            var purchase = _purchaseManager.CreatePurchase(
                purchaseCode: input.PurchaseCode,
                supplierName: input.SupplierName,
                dateTime: input.DateTime,
                products: domainProducts,
                discount: input.Discount,
                paidAmount: input.PaidAmount
            );

            // Save to DB
            await _purchaseRepository.InsertAsync(purchase, autoSave: true);

            // Return DTO
            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }
    }
}