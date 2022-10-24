using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleWebApplication.Controllers;
using SimpleWebApplication.Dtos;
using SimpleWebApplication.Interface;
using SimpleWebApplication.Models;
using Xunit;

namespace SimpleWebApplicationUnitTest
{
    public class ItemControllerTest
    {
        private readonly Mock<IItemsRepository> _repoStub = new Mock<IItemsRepository>();
        private readonly Mock<ILogger<ItemController>> _loggerStub = new Mock<ILogger<ItemController>>();
        private readonly Random rand = new Random();

        [Fact]
        public async void GetItemAsync_WithoutExistingItem_ReturnNotFound()
        {
            //Arrange
            _repoStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);
            var controller = new ItemController(_repoStub.Object, _loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void GetItemAsync_WithExistingItem_ReturnExpectedItem()
        {
            //Arrange
            var expectedItem = CreateRandomItem();
            _repoStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(expectedItem);
            var controller = new ItemController(_repoStub.Object, _loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            result.Value.Should().BeEquivalentTo(
                expectedItem,
                options => options.ComparingByMembers<Item>());
        }

        [Fact]
        public async void GetItemsAsync_WithExistingItem_ReturnAllItem()
        {
            //Arrange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
            _repoStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);
            var controller = new ItemController(_repoStub.Object, _loggerStub.Object);

            //Act
            var result = await controller.GetItemsAsync();
            
            //Assert
            var actual = result.Result as ObjectResult;
            actual.Value .Should().BeEquivalentTo(
                expectedItems,
                options => options.ComparingByMembers<Item>());
        }

        [Fact]
        public async void CreateItemAsync_WithItemToCreate_ReturnCreatedItem()
        {
            //Arrange
            var itemToCreate = new ClientItemDto() {
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000)
            };

            var controller = new ItemController(_repoStub.Object, _loggerStub.Object);

            //Act
            var result = await controller.CreateItemAsync(itemToCreate);

            //Assert
            var createdItem = (result.Result as ObjectResult).Value as ItemDto;
            createdItem.Should().BeEquivalentTo(
                itemToCreate,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 1, 0));
        }

        [Fact]
        public async void UpdateItemAsync_WithExistingItem_ReturnOk()
        {
            //Arrange
            var existingItem = CreateRandomItem();
            var itemId = existingItem.Id;
            var itemToUpdate = new ClientItemDto()
            {
                Name = Guid.NewGuid().ToString(),
                Price = existingItem.Price + 1
            };
            
            _repoStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);
            var controller = new ItemController(_repoStub.Object, _loggerStub.Object);

            //Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            //Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async void DeleteItemAsync_WithExistingItem_ReturnOk()
        {
            //Arrange
            var existingItem = CreateRandomItem();
            _repoStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);
            var controller = new ItemController(_repoStub.Object, _loggerStub.Object);

            //Act
            var result = await controller.DeleteItemAsync(existingItem.Id);

            //Assert
            result.Should().BeOfType<OkResult>();
        }

        private Item CreateRandomItem()
        {
            return new Item()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
