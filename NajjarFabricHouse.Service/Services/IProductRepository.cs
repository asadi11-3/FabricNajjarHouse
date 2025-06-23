

using NajjarFabricHouse.Data.Models;
using NajjarFabricHouse.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace NajjarFabricHouse.Service.Services
{
    public  interface IProductRepository
    {

        Task<ApiResponse<List<Product>>> GetAllProductsAsync(string? filterBy, string? sortBy, bool isAscending = true);
        Task<ApiResponse<Product>> GetProductByIdAsync(int productId);
        Task<ApiResponse<bool>> CreateProductAsync(Product productDto);
        Task<ApiResponse<bool>> UpdateProductAsync(int productId, Product productDto);
        Task<ApiResponse<bool>> DeleteProductAsync(int productId);

    }
}
