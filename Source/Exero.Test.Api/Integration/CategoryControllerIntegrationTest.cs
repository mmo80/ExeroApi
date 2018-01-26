using System;
using System.Collections.Generic;
using System.Linq;
using Exero.Api;
using Exero.Api.Controllers;
using Exero.Api.Repositories.Neo4j;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace Exero.Test.Api.Integration
{
    public class CategoryControllerIntegrationTest
    {
        private readonly Guid _userId = Guid.Parse("2ffa1077-5d93-4ccc-a69c-c420f2e84e22");
        private readonly Guid _categoryId = Guid.Parse("f2aa9464-189c-49f9-8b6a-5f5451ed0a24");
        private readonly Guid _categoryIdNotFound = Guid.Parse("ffa2d23a-7a63-4e60-ac69-8fa81e7e8d5f");

        private IGraphRepository GraphRepo()
        {
            return new GraphRepository(Options.Create(new ExeroSettings
            {
                Neo4jSettings = new Neo4jSettings
                {
                    Uri = "bolt://localhost:7687",
                    User = "neo4j",
                    Password = "TM3qYb5AXvNKZ9JpVr5c"
                }
            }));
        }
        

        [Fact]
        public async void CategoryController_GetListOfCategories_NotNullAndListCountMatch()
        {
            var controller = new CategoryController(new CategoryRepository(GraphRepo()));
            
            IActionResult actionResult = await controller.GetCategories(_userId);
            Assert.NotNull(actionResult);

            var result = actionResult as OkObjectResult;
            Assert.NotNull(result);

            var categories = result.Value as IEnumerable<CategoryApi>;
            Assert.NotNull(categories);
            Assert.Equal(2, categories.ToList().Count);
        }

        [Fact]
        public async void CategoryController_GetSingleCategory_NotFound()
        {
            var controller = new CategoryController(new CategoryRepository(GraphRepo()));

            IActionResult actionResult = await controller.GetCategory(_userId, _categoryIdNotFound);
            Assert.NotNull(actionResult);

            var result = actionResult as NotFoundResult;
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async void CategoryController_GetSingleCategory_NotNullAndIdMatch()
        {
            var controller = new CategoryController(new CategoryRepository(GraphRepo()));

            IActionResult actionResult = await controller.GetCategory(_userId, _categoryId);
            Assert.NotNull(actionResult);

            var result = actionResult as OkObjectResult;
            Assert.NotNull(result);

            var category = result.Value as CategoryApi;
            Assert.NotNull(category);
            Assert.Equal(_categoryId, category.Id);
        }
    }
}
