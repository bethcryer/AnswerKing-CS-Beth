using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Api.Services;

public interface IProductService
{
    Task<Product> CreateProduct(RequestModels.Product createProduct);

    Task<Product?> GetProduct(long productId);

    Task<IEnumerable<Product>> GetProducts();

    Task<IEnumerable<Product>> GetProducts(IEnumerable<long> productIds);

    Task<Product?> RetireProduct(long productId);

    Task<Product?> UpdateProduct(long productId, RequestModels.Product updateProduct);
}
