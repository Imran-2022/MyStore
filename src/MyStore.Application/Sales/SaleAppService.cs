using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace MyStore.Sales
{
    public class SaleAppService : ApplicationService, ISaleAppService
    {
        private readonly ISaleRepository _saleRepository;

        public SaleAppService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<List<SaleDto>> GetListAsync()
        {
            var sales = await _saleRepository.GetListWithProductsAsync();
            // Using ObjectMapper to map entities -> DTOs
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
            var sale = new Sale(
                GuidGenerator.Create(),
                input.Customer,
                input.DateTime,
                input.Products.Select(p =>
                    new SaleProduct(
                        GuidGenerator.Create(),
                        p.Warehouse,
                        p.Product,
                        p.Quantity,
                        p.Price
                    )
                ).ToList()
            );

            await _saleRepository.InsertAsync(sale, autoSave: true);

            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        public async Task DeleteAsync(Guid id)
        {
            var sale = await _saleRepository.FindAsync(id);
            if (sale == null)
            {
                throw new UserFriendlyException("Sale not found");
            }

            await _saleRepository.DeleteAsync(sale, autoSave: true);
        }


    }
}




// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Volo.Abp;
// using Volo.Abp.Application.Services;

// namespace MyStore.Sales
// {
//     public class SaleAppService : ApplicationService, ISaleAppService
//     {
//         private readonly ISaleRepository _saleRepository;

//         public SaleAppService(ISaleRepository saleRepository)
//         {
//             _saleRepository = saleRepository;
//         }

//         public async Task<List<SaleDto>> GetListAsync()
//         {
//             var sales = await _saleRepository.GetListWithProductsAsync();
//             return sales.Select(s => new SaleDto
//             {
//                 Id = s.Id,
//                 Customer = s.Customer,
//                 DateTime = s.DateTime,
//                 Products = s.Products.Select(p => new SaleProductDto
//                 {
//                     Warehouse = p.Warehouse,
//                     Product = p.Product,
//                     Quantity = p.Quantity,
//                     Price = p.Price
//                 }).ToList()
//             }).ToList();
//         }

//         public async Task<SaleDto> GetAsync(Guid id)
//         {
//             var sale = await _saleRepository.GetWithProductsAsync(id)
//                        ?? throw new UserFriendlyException("Sale not found");

//             return new SaleDto
//             {
//                 Id = sale.Id,
//                 Customer = sale.Customer,
//                 DateTime = sale.DateTime,
//                 Products = sale.Products.Select(p => new SaleProductDto
//                 {
//                     Warehouse = p.Warehouse,
//                     Product = p.Product,
//                     Quantity = p.Quantity,
//                     Price = p.Price
//                 }).ToList()
//             };
//         }

//         public async Task<SaleDto> CreateAsync(CreateUpdateSaleDto input)
//         {
//             var sale = new Sale(Guid.NewGuid(), input.Customer, input.DateTime,
//                 input.Products.Select(p => new SaleProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)).ToList()
//             );

//             await _saleRepository.InsertAsync(sale, autoSave: true);

//             return new SaleDto
//             {
//                 Id = sale.Id,
//                 Customer = sale.Customer,
//                 DateTime = sale.DateTime,
//                 Products = sale.Products.Select(p => new SaleProductDto
//                 {
//                     Warehouse = p.Warehouse,
//                     Product = p.Product,
//                     Quantity = p.Quantity,
//                     Price = p.Price
//                 }).ToList()
//             };
//         }
//     }
// }
