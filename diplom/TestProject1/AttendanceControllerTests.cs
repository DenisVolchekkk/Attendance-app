using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudyProj.Controllers;
using StudyProj.Repositories.Implementations;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Assert = Xunit.Assert;
namespace TestProject1
{
    public class AttendanceControllerTests
    {
        private readonly Mock<IAttendanceService> _mockAttendanceService;
        private readonly AttendanceController _controller;

        public AttendanceControllerTests()
        {
            _mockAttendanceService = new Mock<IAttendanceService>();
            _controller = new AttendanceController(_mockAttendanceService.Object);

            // Устанавливаем контекст контроллера (нужно для ModelState)
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task GetAll_ReturnsAttendances()
        {
            // Arrange
            var expectedAttendances = new List<Attendance>
    {
        new Attendance
        {
            Id = 1,
            IsPresent = true,
            AttendanceDate = DateTime.Now,
            Student = new Student { Name = "John Doe" },
            Schedule = new Schedule { StartTime = new TimeSpan(10, 0, 0), Discipline = new Discipline { Name = "Mathematics" } }
        }
    };
            _mockAttendanceService.Setup(service => service.GetAllAsync()).ReturnsAsync(expectedAttendances);

            // Act
            var result = await _controller.GetAll() as JsonResult;

            // Assert
            Assert.NotNull(result); // Убедитесь, что результат не null
            var actualAttendances = Assert.IsType<List<Attendance>>(result.Value); // Проверка, что результат — это список посещаемости
            Assert.Single(actualAttendances); // Убедитесь, что в списке ровно один элемент
            Assert.Equal(1, actualAttendances[0].Id); // Проверка, что ID первого элемента соответствует ожидаемому
            Assert.Equal("John Doe", actualAttendances[0].Student.Name); // Проверка имени студента
            Assert.Equal("Mathematics", actualAttendances[0].Schedule.Discipline.Name); // Проверка дисциплины
        }

        [Fact]
        public async Task GetAttendance_ValidId_ReturnsAttendance()
        {
            // Arrange
            var attendanceId = 1;
            var expectedAttendance = new Attendance { Id = attendanceId, IsPresent = true, AttendanceDate = DateTime.Now };

            _mockAttendanceService
                .Setup(a => a.GetAsync(attendanceId))
                .ReturnsAsync(expectedAttendance);

            // Act
            var result = await _controller.GetAttendance(attendanceId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var actualAttendance = Assert.IsType<Attendance>(result.Value);
            Assert.Equal(attendanceId, actualAttendance.Id);
        }

        [Fact]
        public async Task GetAttendance_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var attendanceId = 1;

            _mockAttendanceService
                .Setup(a => a.GetAsync(attendanceId))
                .ReturnsAsync((Attendance)null);

            // Act
            var result = await _controller.GetAttendance(attendanceId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_ValidAttendance_CreatesAttendance()
        {
            // Arrange
            var newAttendance = new Attendance { IsPresent = true, AttendanceDate = DateTime.Now };
            var createdAttendance = new Attendance { Id = 1, IsPresent = true, AttendanceDate = DateTime.Now };

            _mockAttendanceService
                .Setup(a => a.CreateAsync(newAttendance))
                .ReturnsAsync(createdAttendance);

            // Act
            var result = await _controller.Post(newAttendance) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Contains("Created successfully with ID", message);
        }

        [Fact]
        public async Task Post_InvalidAttendance_ReturnsCreationFailed()
        {
            // Arrange
            var newAttendance = new Attendance { IsPresent = true, AttendanceDate = DateTime.Now };

            _mockAttendanceService
                .Setup(a => a.CreateAsync(newAttendance))
                .ThrowsAsync(new Exception("Creation failed"));

            // Act
            var result = await _controller.Post(newAttendance) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Creation failed", message);
        }

        [Fact]
        public async Task Put_ValidAttendance_UpdatesAttendance()
        {
            // Arrange
            var updatedAttendance = new Attendance { Id = 1, IsPresent = true, AttendanceDate = DateTime.Now };
            var existingAttendance = new Attendance { Id = 1, IsPresent = false, AttendanceDate = DateTime.Now.AddDays(-1) };

            _mockAttendanceService
                .Setup(a => a.GetAsync(updatedAttendance.Id))
                .ReturnsAsync(existingAttendance);

            _mockAttendanceService
                .Setup(a => a.UpdateAsync(updatedAttendance))
                .ReturnsAsync(updatedAttendance);

            // Act
            var result = await _controller.Put(updatedAttendance) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Contains("Update successful for Attendance", message);
        }

        [Fact]
        public async Task Put_InvalidAttendance_ReturnsUpdateNotSuccessful()
        {
            // Arrange
            var updatedAttendance = new Attendance { Id = 1, IsPresent = true, AttendanceDate = DateTime.Now };

            _mockAttendanceService
                .Setup(a => a.GetAsync(updatedAttendance.Id))
                .ReturnsAsync((Attendance)null);

            // Act
            var result = await _controller.Put(updatedAttendance) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Update was not successful", message);
        }

        [Fact]
        public async Task Delete_ValidAttendance_DeletesAttendance()
        {
            // Arrange
            var attendanceId = 1;
            var existingAttendance = new Attendance { Id = attendanceId };

            _mockAttendanceService
                .Setup(a => a.GetAsync(attendanceId))
                .ReturnsAsync(existingAttendance);

            // Act
            var result = await _controller.Delete(attendanceId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Delete successful", message);
        }

        [Fact]
        public async Task Delete_InvalidAttendance_ReturnsDeleteNotSuccessful()
        {
            // Arrange
            var attendanceId = 1;

            _mockAttendanceService
                .Setup(a => a.GetAsync(attendanceId))
                .ReturnsAsync((Attendance)null);

            // Act
            var result = await _controller.Delete(attendanceId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Delete was not successful", message);
        }

        [Fact]
        public async Task Filter_ValidRequest_ReturnsFilteredAttendances()
        {
            var testAttendance = new Attendance
            {
                IsPresent = true,
                AttendanceDate = DateTime.Now,
                Student = new Student { Name = "John Doe" },
                Schedule = new Schedule { StartTime = new TimeSpan(10, 0, 0), Discipline = new Discipline { Name = "Mathematics" } }
            };

            var expectedAttendances = new List<Attendance>
        {
            new Attendance { Id = 1, IsPresent = true, AttendanceDate = DateTime.Now, Student = new Student { Name = "John Doe" }, Schedule = new Schedule { StartTime = new TimeSpan(10, 0, 0), Discipline = new Discipline { Name = "Mathematics" } } }
        };

            _mockAttendanceService
                .Setup(a => a.GetAllAsync(It.IsAny<Attendance>()))
                .ReturnsAsync(expectedAttendances);

            var result = await _controller.Filter(testAttendance) as JsonResult;

            Assert.NotNull(result);
            var actualAttendances = Assert.IsType<List<Attendance>>(result.Value);
            Assert.Single(actualAttendances);
            Assert.Equal(1, actualAttendances[0].Id);
            Assert.Equal("John Doe", actualAttendances[0].Student.Name);
            Assert.Equal("Mathematics", actualAttendances[0].Schedule.Discipline.Name);
        }

        [Fact]
        public async Task Filter_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("IsPresent", "Required");

            var result = await _controller.Filter(new Attendance()) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }
    }
}