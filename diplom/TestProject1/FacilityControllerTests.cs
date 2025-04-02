using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyProj.Controllers;
using StudyProj.Repositories.Implementations;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1
{
    public class FacilityControllerTests
    {
        private readonly Mock<IFacilityService> _mockFacilityService;
        private readonly FacilityController _controller;

        public FacilityControllerTests()
        {
            _mockFacilityService = new Mock<IFacilityService>();
            _controller = new FacilityController(_mockFacilityService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult_WithListOfFacilities()
        {
            var facilities = new List<Facility> { new Facility { Id = 1, Name = "Library" } };
            _mockFacilityService.Setup(service => service.GetAllAsync()).ReturnsAsync(facilities);

            var result = await _controller.GetAll();
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(facilities, jsonResult.Value);
        }

        [Fact]
        public async Task GetFacility_ReturnsNotFound_WhenFacilityNotExists()
        {
            _mockFacilityService.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Facility)null);
            var result = await _controller.GetFacility(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetFacility_ReturnsJsonResult_WhenFacilityExists()
        {
            var facility = new Facility { Id = 1, Name = "Library" };
            _mockFacilityService.Setup(service => service.GetAsync(1)).ReturnsAsync(facility);

            var result = await _controller.GetFacility(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(facility, jsonResult.Value);
        }

        [Fact]
        public async Task Post_CreatesFacility_ReturnsSuccessMessage()
        {
            var facility = new Facility { Id = 1, Name = "Library" };
            _mockFacilityService.Setup(service => service.CreateAsync(It.IsAny<Facility>())).ReturnsAsync(facility);

            var result = await _controller.Post(facility);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Created successfully with ID: {facility.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_UpdatesFacility_ReturnsSuccessMessage()
        {
            var facility = new Facility { Id = 1, Name = "Updated Library" };
            _mockFacilityService.Setup(service => service.GetAsync(facility.Id)).ReturnsAsync(facility);
            _mockFacilityService.Setup(service => service.UpdateAsync(It.IsAny<Facility>())).ReturnsAsync(facility);

            var result = await _controller.Put(facility);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Update successful for facility with ID: {facility.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_ReturnsFailureMessage_WhenFacilityNotExists()
        {
            var facility = new Facility { Id = 1, Name = "Non-existent" };
            _mockFacilityService.Setup(service => service.GetAsync(facility.Id)).ReturnsAsync((Facility)null);

            var result = await _controller.Put(facility);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Update was not successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessMessage_WhenFacilityExists()
        {
            var facility = new Facility { Id = 1, Name = "Library" };
            _mockFacilityService.Setup(service => service.GetAsync(1)).ReturnsAsync(facility);
            _mockFacilityService.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsFailureMessage_WhenFacilityNotExists()
        {
            _mockFacilityService.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Facility)null);
            var result = await _controller.Delete(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete was not successful", jsonResult.Value);
        }

        [Fact]
        public async Task Filter_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.Filter(new Facility());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Filter_ReturnsJsonResult_WithFilteredFacilities()
        {
            var facilities = new List<Facility> { new Facility { Id = 1, Name = "Library" } };
            _mockFacilityService.Setup(service => service.GetAllAsync(It.IsAny<Facility>())).ReturnsAsync(facilities);

            var result = await _controller.Filter(new Facility { Name = "Library" });
            var jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(facilities, jsonResult.Value);
        }
    }
}
