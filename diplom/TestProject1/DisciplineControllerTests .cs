using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyProj.Controllers;
using StudyProj.Repositories.Interfaces;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assert = Xunit.Assert;

namespace TestProject1
{
    public class DisciplineControllerTests
    {
        private readonly Mock<IDisciplineService> _mockDisciplineRepository;
        private readonly DisciplineController _controller;

        public DisciplineControllerTests()
        {
            _mockDisciplineRepository = new Mock<IDisciplineService>();
            _controller = new DisciplineController(_mockDisciplineRepository.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult_WithListOfDisciplines()
        {
            var disciplines = new List<Discipline> { new Discipline { Id = 1, Name = "Math" } };
            _mockDisciplineRepository.Setup(service => service.GetAllAsync()).ReturnsAsync(disciplines);

            var result = await _controller.GetAll();
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(disciplines, jsonResult.Value);
        }

        [Fact]
        public async Task GetDiscipline_ReturnsNotFound_WhenDisciplineNotExists()
        {
            _mockDisciplineRepository.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Discipline)null);
            var result = await _controller.GetDiscipline(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetDiscipline_ReturnsJsonResult_WhenDisciplineExists()
        {
            var discipline = new Discipline { Id = 1, Name = "Math" };
            _mockDisciplineRepository.Setup(service => service.GetAsync(1)).ReturnsAsync(discipline);
            var result = await _controller.GetDiscipline(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(discipline, jsonResult.Value);
        }

        [Fact]
        public async Task Post_CreatesDiscipline_ReturnsSuccessMessage()
        {
            var discipline = new Discipline { Id = 1, Name = "Math" };
            _mockDisciplineRepository.Setup(service => service.CreateAsync(It.IsAny<Discipline>())).ReturnsAsync(discipline);
            var result = await _controller.Post(discipline);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Created successfully with ID: {discipline.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessMessage_WhenDisciplineExists()
        {
            var discipline = new Discipline { Id = 1, Name = "Math" };
            _mockDisciplineRepository.Setup(service => service.GetAsync(1)).ReturnsAsync(discipline);
            _mockDisciplineRepository.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);
            var result = await _controller.Delete(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsFailureMessage_WhenDisciplineNotExists()
        {
            _mockDisciplineRepository.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Discipline)null);
            var result = await _controller.Delete(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete was not successful", jsonResult.Value);
        }

        [Fact]
        public async Task Filter_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.Filter(new Discipline());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Filter_ReturnsJsonResult_WithFilteredDisciplines()
        {
            var disciplines = new List<Discipline> { new Discipline { Id = 1, Name = "Math" } };
            _mockDisciplineRepository.Setup(service => service.GetAllAsync(It.IsAny<Discipline>())).ReturnsAsync(disciplines);
            var result = await _controller.Filter(new Discipline { Name = "Math" });
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(disciplines, jsonResult.Value);
        }

        [Fact]
        public async Task Put_UpdatesDiscipline_ReturnsSuccessMessage()
        {
            var discipline = new Discipline { Id = 1, Name = "Updated Math" };
            _mockDisciplineRepository.Setup(service => service.GetAsync(discipline.Id)).ReturnsAsync(discipline);
            _mockDisciplineRepository.Setup(service => service.UpdateAsync(It.IsAny<Discipline>())).ReturnsAsync(discipline);
            var result = await _controller.Put(discipline);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Update successful for Discipline with ID: {discipline.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_ReturnsFailureMessage_WhenDisciplineNotExists()
        {
            var discipline = new Discipline { Id = 1, Name = "Non-existent" };
            _mockDisciplineRepository.Setup(service => service.GetAsync(discipline.Id)).ReturnsAsync((Discipline)null);
            var result = await _controller.Put(discipline);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Update was not successful", jsonResult.Value);
        }
    }
}
