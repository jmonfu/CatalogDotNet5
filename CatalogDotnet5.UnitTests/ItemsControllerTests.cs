using System;
using CatalogDotnet5.API.Repositories;
using CatalogDotnet5.API.Entities;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using CatalogDotnet5.API.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CatalogDotnet5.API.Dtos;
using FluentAssertions;
using System.Collections.Generic;

namespace CatalogDotnet5.UnitTests
{
    public class ItemsControllerTests
    {
        // naming convention - UnitOfWork_StateUnderTest_ExpectedBehaviour

        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new();
        private readonly Random rand = new();

        [Fact]
        public async Task GetItemAsync_NonExistentItem_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);
            
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object); 

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithExistentItem_ReturnsExpectedItem()
        {
            // Arrange
            Item expectedItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object); 

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            // here we say, do not compare the Item to the ItemDto, but focus on the properties of the Item 
            // and the properties of the ItemDto and as long as they are the same, then compare their values
            // result.Value.Should().BeEquivalentTo(
            //     expectedItem,
            //     options => options.ComparingByMembers<Item>());

            //since now we are not comparing classes to recordtypes anymore, we can remove the ComparingByMembers
            result.Value.Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public async Task GetItemsAsync_WithExistentItem_ReturnsAllItems()
        {
            // Arrange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object); 

            // Act
            var actualItems = await controller.GetItemsAsync();

            // Assert
            // actualItems.Should().BeEquivalentTo(
            //     expectedItems,
            //     options => options.ComparingByMembers<Item>());

            //since now we are not comparing classes to recordtypes anymore, we can remove the ComparingByMembers
            actualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
        {
            // Arrange
            var allItems = new[] 
            { 
                new Item() {Name = "Potion"},
                new Item() {Name = "Antidote"},
                new Item() {Name = "Hi-Potion"}
            };
            var namesToMatch = "Potion";

            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(allItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object); 

            // Act
            IEnumerable<ItemDto> foundItems = await controller.GetItemsAsync(namesToMatch);

            // Assert
            foundItems.Should().OnlyContain(
                item => item.Name == allItems[0].Name || item.Name == allItems[2].Name
            );
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arrange
            // here we are going to ignore what happens in the repository to minimize the impact if the 
            // repository.CreateItemAsync(item) eventually changes, which will be out of the scope of this test

            var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), rand.Next(100));
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object); 

            // Act
            var result = await controller.CreateItemAsync(itemToCreate);

            // Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);

        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arrange
            // here we are going to ignore what happens in the repository to minimize the impact if the 
            // repository.UpdateItemAsync(item) eventually changes, which will be out of the scope of this test

            Item existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3); 

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object); 

            // Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            // Assert
            result.Should().BeOfType<NoContentResult>();

        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arrange
            // here we are going to ignore what happens in the repository to minimize the impact if the 
            // repository.UpdateItemAsync(item) eventually changes, which will be out of the scope of this test

            Item existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object); 

            // Act
            var result = await controller.DeleteItemAsync(itemId);

            // Assert
            result.Should().BeOfType<NoContentResult>();

        }


        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
