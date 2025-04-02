using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyProj.Controllers;
using StudyProj.Repositories.Implementations;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1
{
    public class StudentControllerTests
    {
        private readonly Mock<IStudentService> _mockStudentService;
        private readonly StudentController _controller;

        public StudentControllerTests()
        {
            _mockStudentService = new Mock<IStudentService>();
            _controller = new StudentController(_mockStudentService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult_WithListOfStudents()
        {
            var students = new List<Student> { new Student { Id = 1, Name = "Alice" } };
            _mockStudentService.Setup(service => service.GetAllAsync()).ReturnsAsync(students);

            var result = await _controller.GetAll();

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(students, jsonResult.Value);
        }

        [Fact]
        public async Task GetStudent_ReturnsNotFound_WhenStudentNotExists()
        {
            _mockStudentService.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Student)null);
            var result = await _controller.GetStudent(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetStudent_ReturnsJsonResult_WhenStudentExists()
        {
            var student = new Student { Id = 1, Name = "Alice" };
            _mockStudentService.Setup(service => service.GetAsync(1)).ReturnsAsync(student);
            var result = await _controller.GetStudent(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(student, jsonResult.Value);
        }

        [Fact]
        public async Task Post_CreatesStudent_ReturnsSuccessMessage()
        {
            var student = new Student { Id = 1, Name = "Alice" };
            _mockStudentService.Setup(service => service.CreateAsync(It.IsAny<Student>())).ReturnsAsync(student);

            var result = await _controller.Post(student);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Created successfully with ID: {student.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_UpdatesStudent_ReturnsSuccessMessage()
        {
            var student = new Student { Id = 1, Name = "Updated Name" };
            _mockStudentService.Setup(service => service.GetAsync(student.Id)).ReturnsAsync(student);
            _mockStudentService.Setup(service => service.UpdateAsync(It.IsAny<Student>())).ReturnsAsync(student);

            var result = await _controller.Put(student);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Update successful for Student with ID: {student.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task Put_ReturnsFailureMessage_WhenStudentNotExists()
        {
            var student = new Student { Id = 1, Name = "Non-existent" };
            _mockStudentService.Setup(service => service.GetAsync(student.Id)).ReturnsAsync((Student)null);

            var result = await _controller.Put(student);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Update was not successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessMessage_WhenStudentExists()
        {
            var student = new Student { Id = 1, Name = "Alice" };
            _mockStudentService.Setup(service => service.GetAsync(1)).ReturnsAsync(student);
            _mockStudentService.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete successful", jsonResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsFailureMessage_WhenStudentNotExists()
        {
            _mockStudentService.Setup(service => service.GetAsync(It.IsAny<int>())).ReturnsAsync((Student)null);
            var result = await _controller.Delete(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("Delete was not successful", jsonResult.Value);
        }
        [Fact]
        public async Task Filter_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.Filter(new Student());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Filter_ReturnsJsonResult_WithFilteredFacilities()
        {
            var students = new List<Student> { new Student { Id = 1, Name = "Library" } };
            _mockStudentService.Setup(service => service.GetAllAsync(It.IsAny<Student>())).ReturnsAsync(students);

            var result = await _controller.Filter(new Student { Name = "Library" });
            var jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(students, jsonResult.Value);
        }
    }
}
