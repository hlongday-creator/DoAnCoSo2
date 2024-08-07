﻿using AutoMapper;
using DoAnCoSo2.Data;
using DoAnCoSo2.Models;
using Neo4jClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo2.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IGraphClient _client;
        private readonly IMapper _mapper;

        public CategoryRepository(IGraphClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<string> AddCategoryAsync(CategoryModel model)
        {
            var newCategory = _mapper.Map<Category>(model);

            await _client.Cypher
                .Create("(category:Category $newCategory)")
                .WithParam("newCategory", newCategory)
                .ExecuteWithoutResultsAsync();

            // Assuming Id is autogenerated by Neo4j
            return newCategory.Slug;
        }
        public async Task<bool> IsSlugExists(string slug)
        {
            var exists = await _client.Cypher
                .Match("(category:Category)")
                .Where((Category category) => category.Slug == slug)
                .Return(category => category.As<Category>())
                .ResultsAsync;

            return exists.Any();
        }

        public async Task DeleteCategoryAsync(string slug)
        {
            await _client.Cypher
                .Match("(category:Category)")
                .Where((Category category) => category.Slug == slug)
                .Delete("category")
                .ExecuteWithoutResultsAsync();
        }

        public async Task<List<CategoryModel>> GetAllCategoriesAsync()
        {
            var categories = await _client.Cypher
                .Match("(category:Category)")
                .Return(category => category.As<Category>())
                .ResultsAsync;

            return _mapper.Map<List<CategoryModel>>(categories);
        }

        public async Task<CategoryModel> GetCategoryAsync(string slug)
        {
            var category = await _client.Cypher
                .Match("(category:Category)")
                .Where((Category category) => category.Slug == slug)
                .Return(category => category.As<Category>())
                .ResultsAsync;

            return _mapper.Map<CategoryModel>(category.FirstOrDefault());
        }

        public async Task UpdateCategoryAsync(string slug, CategoryModel model)
        {
            var updatedCategory = _mapper.Map<Category>(model);

            await _client.Cypher
                .Match("(category:Category)")
                .Where((Category category) => category.Slug == slug)
                .Set("category = $updatedCategory")
                .WithParam("updatedCategory", updatedCategory)
                .ExecuteWithoutResultsAsync();
        }
    }
}
