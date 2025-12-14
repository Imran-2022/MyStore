using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using MyStore.Stocks;

namespace MyStore.Sales
{
    public class SaleAppService : ApplicationService, ISaleAppService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly SaleManager _saleManager;

        public SaleAppService(ISaleRepository saleRepository, SaleManager saleManager)
        {
            _saleRepository = saleRepository;
            _saleManager = saleManager;
        }

        public async Task<List<SaleDto>> GetListAsync()
        {
            var sales = await _saleRepository.GetListWithProductsAsync();
            return ObjectMapper.Map<List<Sale>, List<SaleDto>>(sales);
        }

        public async Task<SaleDto> GetAsync(Guid id)
        {
            var sale = await _saleRepository.GetWithProductsAsync(id)
                ?? throw new UserFriendlyException("Sale not found");

            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        public async Task<SaleDto> CreateAsync(CreateUpdateSaleDto input)
        {
            var products = input.Products.Select(p =>
                new SaleProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)
            ).ToList();

            var sale = await _saleManager.CreateSaleAsync(input.Customer, input.DateTime, products);
            await _saleRepository.InsertAsync(sale, autoSave: true);

            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        public async Task<SaleDto> UpdateAsync(Guid id, CreateUpdateSaleDto input)
        {
            var sale = await _saleRepository.GetWithProductsAsync(id)
                ?? throw new UserFriendlyException("Sale not found");

            var products = input.Products.Select(p =>
                new SaleProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)
            ).ToList();

            await _saleManager.UpdateSaleAsync(sale, products, input.Customer, input.DateTime);
            await _saleRepository.UpdateAsync(sale, autoSave: true);

            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        public async Task DeleteAsync(Guid id)
        {
            var sale = await _saleRepository.GetWithProductsAsync(id)
                ?? throw new UserFriendlyException("Sale not found");

            await _saleManager.DeleteSaleAsync(sale);
            await _saleRepository.DeleteAsync(sale, autoSave: true);
        }
    }
}
