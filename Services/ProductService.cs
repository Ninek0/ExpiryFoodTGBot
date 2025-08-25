using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ExpiryFoodTGBot.Models;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;

namespace ExpiryFoodTGBot.Service
{
    public class ProductService
    {
        HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7016/api/Product";
        public ProductService(
            HttpClient httpClient) 
        {
            _httpClient = new HttpClient();
        }
        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{productId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to delete product {ProductId}", productId);
                return false;
            }
        }

        public async Task<ProductModel?> GetProductByIdAsync(int productId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductModel>(
                    $"{_baseUrl}/{productId}");
            }
            catch (HttpRequestException ex)
            {
                //_logger.LogWarning(ex, "Product {ProductId} not found", productId);
                return null;
            }
        }

        public async Task<bool> UpdateProductAsync(ProductModel product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(_baseUrl, product);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to update product {ProductId}", product.Id);
                return false;
            }
        }
        public async Task<bool> CreateProductAsync(ProductModel product) 
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, product);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to update product {ProductId}", product.Id);
                return false;
            }
        }

        public async Task<IReadOnlyList<ProductModel>> GetExpiringProductsAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductModel>>(_baseUrl);
                return products?.AsReadOnly() ?? new List<ProductModel>().AsReadOnly();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to load products");
                return new List<ProductModel>().AsReadOnly();
            }
        }
    }
}
