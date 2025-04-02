using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyProj.Controllers;
using StudyProj.Repositories.Implementations;
using Xunit;
using Domain.Models;
using Assert = Xunit.Assert;
namespace TestProject1
{
    public class GroupControllerTests
    {
        private readonly Mock<IGroupService> _groupServiceMock;
        private readonly GroupController _controller;

        public GroupControllerTests()
        {
            _groupServiceMock = new Mock<IGroupService>();
            _controller = new GroupController(_groupServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult_WithGroups()
        {
            // Arrange
            var groups = new List<Group> { new Group { Id = 1, Name = "Test Group" } };
            _groupServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(groups);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(groups, jsonResult.Value);
        }

        [Fact]
        public async Task GetGroup_ReturnsNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            _groupServiceMock.Setup(s => s.GetAsync(1)).ReturnsAsync((Group)null);

            // Act
            var result = await _controller.GetGroup(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGroup_ReturnsJsonResult_WithGroup()
        {
            // Arrange
            var group = new Group { Id = 1, Name = "Test Group" };
            _groupServiceMock.Setup(s => s.GetAsync(1)).ReturnsAsync(group);

            // Act
            var result = await _controller.GetGroup(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(group, jsonResult.Value);
        }

        [Fact]
        public async Task Post_ReturnsJsonResult_OnSuccess()
        {
            // Arrange
            var group = new Group { Id = 1, Name = "New Group" };
            _groupServiceMock.Setup(s => s.CreateAsync(group)).ReturnsAsync(group);

            // Act
            var result = await _controller.Post(group);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Contains("Created successfully", jsonResult.Value.ToString());
        }

        [Fact]
        public async Task Put_ReturnsJsonResult_OnUpdateSuccess()
        {
            // Arrange
            var group = new Group { Id = 1, Name = "Updated Group" };
            _groupServiceMock.Setup(s => s.GetAsync(1)).ReturnsAsync(group);
            _groupServiceMock.Setup(s => s.UpdateAsync(group)).ReturnsAsync(group);

            // Act
            var result = await _controller.Put(group);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Contains("Update successful", jsonResult.Value.ToString());
        }

        [Fact]
        public async Task Delete_ReturnsJsonResult_OnSuccess()
        {
            // Arrange
            var group = new Group { Id = 1, Name = "Test Group" };
            _groupServiceMock.Setup(s => s.GetAsync(1)).ReturnsAsync(group);
            _groupServiceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Contains("Delete successful", jsonResult.Value.ToString());
        }
        [Fact]
        public async Task Filter_ValidRequest_ReturnsFilteredGroups()
        {
            // Arrange
            var testGroup = new Group
            {
                Name = "Math",
                Chief = new Chief { Name = "John" },
                Facility = new Facility { Name = "Science" }
            };

            var expectedGroups = new List<Group>
        {
            new Group { Id = 1, Name = "Math Group", Chief = new Chief { Name = "John" }, Facility = new Facility { Name = "Science" } }
        };

            _groupServiceMock
                .Setup(s => s.GetAllAsync(It.IsAny<Group>()))
                .ReturnsAsync(expectedGroups);

            // Act
            var result = await _controller.Filter(testGroup) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var actualGroups = Assert.IsType<List<Group>>(result.Value);
            Assert.Single(actualGroups);
            Assert.Equal("Math Group", actualGroups[0].Name);
        }

        [Fact]
        public async Task Filter_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Filter(new Group()) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode); // HTTP 400 Bad Request
        }
    }
}