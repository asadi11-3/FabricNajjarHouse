
using Microsoft.EntityFrameworkCore;
using NajjarFabricHouse.Data.DataBase;
using NajjarFabricHouse.Data.Models;

using NajjarFabricHouse.Service.Models;
using NajjarFabricHouse.Service.Services;


namespace FabricHouse.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<Product>>> GetAllProductsAsync(string? filterBy , string? sortBy , bool isAscending = true)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(filterBy))
                {
                    query = query.Where(p => p.NameProduct.Contains(filterBy) || p.Description.Contains(filterBy));
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    query = sortBy.ToLower() switch
                    {
                        "nameProduct" => isAscending ? query.OrderBy(p => p.NameProduct) : query.OrderByDescending(p => p.NameProduct),
                        "price" => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                        _ => query
                    };
                }

                var products = await query.Select(p => new Product
                {
                    ProductId = p.ProductId,
                    NameProduct = p.NameProduct,
                    Description = p.Description,
                    Price = p.Price,
                    Pattern = p.Pattern,
                    CreatedDate = p.CreatedDate,
                    IsDeleted = p.IsDeleted
                }).ToListAsync();

                return new ApiResponse<List<Product>>
                {
                    IsSuccess = true,
                    Message = "Products retrieved successfully.",
                    StatusCode = 200,
                    Response = products
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Product>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving products: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<Product>> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    return new ApiResponse<Product>
                    {
                        IsSuccess = false,
                        Message = "Product not found.",
                        StatusCode = 404
                    };
                }

                var productDto = new Product
                {
                    ProductId = product.ProductId,
                    NameProduct = product.NameProduct,
                    Description = product.Description,
                    Price = product.Price,
                    Pattern = product.Pattern,
                    CreatedDate = product.CreatedDate,
                    IsDeleted = product.IsDeleted
                };

                return new ApiResponse<Product>
                {
                    IsSuccess = true,
                    Message = "Product retrieved successfully.",
                    StatusCode = 200,
                    Response = productDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Product>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving product: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<bool>> CreateProductAsync(Product productDto)
        {
            try
            {
                var product = new Product
                {
                    NameProduct = productDto.NameProduct,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Pattern = productDto.Pattern,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Message = "Product created successfully.",
                    StatusCode = 201,
                    Response = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = $"Error creating product: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateProductAsync(int productId, Product productDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Product not found.",
                        StatusCode = 404
                    };
                }

                product.NameProduct = productDto.NameProduct;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.Pattern = productDto.Pattern;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Message = "Product updated successfully.",
                    StatusCode = 200,
                    Response = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = $"Error updating product: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Product not found.",
                        StatusCode = 404
                    };
                }

                product.IsDeleted = true;
                
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Message = "Product deleted successfully.",
                    StatusCode = 200,
                    Response = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = $"Error deleting product: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
