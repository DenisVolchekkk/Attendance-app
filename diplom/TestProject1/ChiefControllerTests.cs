using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyProj;
using StudyProj.Controllers;
using StudyProj.Repositories.Implementations;
using StudyProj.Repositories.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1
{
    public class ChiefControllerTests
    {
        private readonly Mock<IChiefService> _mockChiefRepository;
        private readonly ChiefController _controller;

        public ChiefControllerTests()
        {
            _mockChiefRepository = new Mock<IChiefService>();
            _controller = new ChiefController(_mockChiefRepository.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult_WithListOfChiefs()
        {
            var chiefs = new List<Chief> { new Chief { Id = 1, Name = "John" } };
            _mockChiefRepository.Setup(service => service.GetAllAsync()).ReturnsAsync(chiefs);

            var result = await _controller.GetAll();

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(chiefs, jsonResult.Value);
        }

        [Fact]
        public async Task GetChief_ReturnsNotFound_WhenChiefNotExists()
        {
            _mockChiefRepository.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Chief)null);
            var result = await _controller.GetChief(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetChief_ReturnsJsonResult_WhenChiefExists()
        {
            var chief = new Chief { Id = 1, Name = "John" };
            _mockChiefRepository.Setup(service => service.GetAsync(1)).ReturnsAsync(chief);
            var result = await _controller.GetChief(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(chief, jsonResult.Value);
        }

        [Fact]
        public async Task Post_CreatesChief_ReturnsSuccessMessage()
        {
            var chief = new Chief { Id = 1, Name = "John" };
            _mockChiefRepository.Setup(service => service.CreateAsync(It.IsAny<Chief>())).ReturnsAsync(chief);

            var result = await _controller.Post(chief);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Created successfully with ID: {chief.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessMessage_WhenChiefExists()
        {
            var chief = new Chief { Id = 1, Name = "John" };
            _mockChiefRepository.Setup(service => service.GetAsync(1)).ReturnsAsync(chief);
            _mockChiefRepository.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsFailureMessage_WhenChiefNotExists()
        {
            _mockChiefRepository.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Chief)null);
            var result = await _controller.Delete(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete was not successful", jsonResult.Value);
        }

        [Fact]
        public async Task Filter_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.Filter(new Chief());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Filter_ReturnsJsonResult_WithFilteredChiefs()
        {
            var chiefs = new List<Chief> { new Chief { Id = 1, Name = "John" } };
            _mockChiefRepository.Setup(service => service.GetAllAsync(It.IsAny<Chief>())).ReturnsAsync(chiefs);

            var result = await _controller.Filter(new Chief { Name = "John" });
            var jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(chiefs, jsonResult.Value);
        }

        [Fact]
        public async Task Put_UpdatesChief_ReturnsSuccessMessage()
        {
            var chief = new Chief { Id = 1, Name = "Updated Name" };
            _mockChiefRepository.Setup(service => service.GetAsync(chief.Id)).ReturnsAsync(chief);
            _mockChiefRepository.Setup(service => service.UpdateAsync(It.IsAny<Chief>())).ReturnsAsync(chief);

            var result = await _controller.Put(chief);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Update successful for Chief with ID: {chief.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_ReturnsFailureMessage_WhenChiefNotExists()
        {
            var chief = new Chief { Id = 1, Name = "Non-existent" };
            _mockChiefRepository.Setup(service => service.GetAsync(chief.Id)).ReturnsAsync((Chief)null);

            var result = await _controller.Put(chief);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Update was not successful", jsonResult.Value);
        }
    }
}