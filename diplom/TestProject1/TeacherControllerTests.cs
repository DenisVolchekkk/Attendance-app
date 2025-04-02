using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyProj.Controllers;
using StudyProj.Repositories.Implementations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1
{
    public class TeacherControllerTests
    {
        private readonly Mock<ITeacherService> _mockTeacherService;
        private readonly TeacherController _controller;

        public TeacherControllerTests()
        {
            _mockTeacherService = new Mock<ITeacherService>();
            _controller = new TeacherController(_mockTeacherService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult_WithListOfTeachers()
        {
            var teachers = new List<Teacher> { new Teacher { Id = 1, Name = "John Doe" } };
            _mockTeacherService.Setup(service => service.GetAllAsync()).ReturnsAsync(teachers);

            var result = await _controller.GetAll();

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(teachers, jsonResult.Value);
        }

        [Fact]
        public async Task GetTeacher_ReturnsNotFound_WhenTeacherNotExists()
        {
            _mockTeacherService.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Teacher)null);
            var result = await _controller.GetTeacher(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTeacher_ReturnsJsonResult_WhenTeacherExists()
        {
            var teacher = new Teacher { Id = 1, Name = "John Doe" };
            _mockTeacherService.Setup(service => service.GetAsync(1)).ReturnsAsync(teacher);

            var result = await _controller.GetTeacher(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(teacher, jsonResult.Value);
        }

        [Fact]
        public async Task Post_CreatesTeacher_ReturnsSuccessMessage()
        {
            var teacher = new Teacher { Id = 1, Name = "John Doe" };
            _mockTeacherService.Setup(service => service.CreateAsync(It.IsAny<Teacher>())).ReturnsAsync(teacher);

            var result = await _controller.Post(teacher);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Created successfully with ID: {teacher.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_UpdatesTeacher_ReturnsSuccessMessage()
        {
            var teacher = new Teacher { Id = 1, Name = "Updated Name" };
            _mockTeacherService.Setup(service => service.GetAsync(teacher.Id)).ReturnsAsync(teacher);
            _mockTeacherService.Setup(service => service.UpdateAsync(It.IsAny<Teacher>())).ReturnsAsync(teacher);

            var result = await _controller.Put(teacher);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Update successful for Teacher with ID: {teacher.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_ReturnsFailureMessage_WhenTeacherNotExists()
        {
            var teacher = new Teacher { Id = 1, Name = "Non-existent" };
            _mockTeacherService.Setup(service => service.GetAsync(teacher.Id)).ReturnsAsync((Teacher)null);

            var result = await _controller.Put(teacher);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Update was not successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessMessage_WhenTeacherExists()
        {
            var teacher = new Teacher { Id = 1, Name = "John Doe" };
            _mockTeacherService.Setup(service => service.GetAsync(1)).ReturnsAsync(teacher);
            _mockTeacherService.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsFailureMessage_WhenTeacherNotExists()
        {
            _mockTeacherService.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Teacher)null);
            var result = await _controller.Delete(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete was not successful", jsonResult.Value);
        }
        [Fact]
        public async Task Filter_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.Filter(new Teacher());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Filter_ReturnsJsonResult_WithFilteredFacilities()
        {
            var teachers = new List<Teacher> { new Teacher { Id = 1, Name = "Library" } };
            _mockTeacherService.Setup(service => service.GetAllAsync(It.IsAny<Teacher>())).ReturnsAsync(teachers);

            var result = await _controller.Filter(new Teacher { Name = "Library" });
            var jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(teachers, jsonResult.Value);
        }
    }
}
