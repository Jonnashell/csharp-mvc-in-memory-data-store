using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using exercise.wwwapi.Data;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using System.Net.NetworkInformation;

namespace exercise.wwwapi.Endpoints
{
    public static class ProductEndpoints
    {
        public static void ConfigureProduct(this WebApplication app)
        {
            var products = app.MapGroup("products");

            products.MapGet("/", GetProducts);
            products.MapGet("/{id}", GetProductsById);
            products.MapPost("/", AddProduct);
            products.MapDelete("/{id}", Delete);
            products.MapPut("/{id}", Update);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        public static async Task<IResult> AddProduct(IRepository repository, ProductPost model)
        {
            // Skip for now
            //if (model.Price.GetType() != typeof(int)) return TypedResults.BadRequest(
            //    new { Error = $"Price must be an integer, something else was provided." });

            List<Product> products = await repository.GetAsync();
            if (products.Any(item => item.Name == model.Name)) return TypedResults.BadRequest(
                new { Error = $"Product with provided name already exists." });

            Product product = new Product();
            product.Name = model.Name;
            product.Category = model.Category;
            product.Price = model.Price;
            var results = await repository.AddAsync(product);
            return TypedResults.Ok(results);
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetProducts(IRepository repository, string category = "")
        {
            var results = await repository.GetAsync(category);
            if (results.Count == 0) return TypedResults.NotFound(
                new { Error = $"No products of category '{category}' were found" });
            return TypedResults.Ok(results);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetProductsById(IRepository repository, int id)
        {
            var results = await repository.GetByIdAsync(id);
            if (results == null) return TypedResults.NotFound(
                new { Error = $"No product found with id '{id}'" });
            return TypedResults.Ok(results);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        public static async Task<IResult> Update(IRepository repository, int id, ProductPut model)
        {
            List<Product> products = await repository.GetAsync();
            //products.Where(item => item.Name == model.Name)
            if (products.Any(item => item.Name == model.Name && item.Id != id)) return TypedResults.BadRequest(
                new { Error = $"Product with provided name already exists." });

            var entity = products.FirstOrDefault(item => item.Id == id);

            if (entity is null) return TypedResults.NotFound(
                new { Error = $"No product found with id '{id}'" });

            if (model.Name != null) entity.Name = model.Name;
            if (model.Category != null) entity.Category = model.Category;
            if (model.Price != 0 && model.Price != entity.Price) entity.Price = model.Price;

            await repository.UpdateAsync(id, entity);
            return TypedResults.Created();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> Delete(IRepository repository, int id)
        {
            var results = await repository.DeleteAsync(id);
            if (results == null) return TypedResults.NotFound(new { Error = $"No product found with id '{id}'" });
            return TypedResults.Ok(results);
        }
    }
}
