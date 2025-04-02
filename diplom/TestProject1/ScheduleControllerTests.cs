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
    public class ScheduleControllerTests
    {
        private readonly Mock<IScheduleService> _mockScheduleService;
        private readonly ScheduleController _controller;

        public ScheduleControllerTests()
        {
            _mockScheduleService = new Mock<IScheduleService>();
            _controller = new ScheduleController(_mockScheduleService.Object);

            // Устанавливаем контекст контроллера (нужно для ModelState)
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
        [Fact]
        public async Task GetAll_ReturnsSchedules()
        {
            // Arrange
            var expectedSchedules = new List<Schedule>
        {
            new Schedule { Id = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0), Teacher = new Teacher { Name = "Dr. Smith" }, Discipline = new Discipline { Name = "Mathematics" } }
        };

            _mockScheduleService
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(expectedSchedules);

            // Act
            var result = await _controller.GetAll() as JsonResult;

            // Assert
            Assert.NotNull(result);
            var actualSchedules = Assert.IsType<List<Schedule>>(result.Value);
            Assert.Single(actualSchedules);
            Assert.Equal(1, actualSchedules[0].Id);
        }

        [Fact]
        public async Task GetSchedule_ValidId_ReturnsSchedule()
        {
            // Arrange
            var scheduleId = 1;
            var expectedSchedule = new Schedule { Id = scheduleId, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0) };

            _mockScheduleService
                .Setup(s => s.GetAsync(scheduleId))
                .ReturnsAsync(expectedSchedule);

            // Act
            var result = await _controller.GetSchedule(scheduleId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var actualSchedule = Assert.IsType<Schedule>(result.Value);
            Assert.Equal(scheduleId, actualSchedule.Id);
        }

        [Fact]
        public async Task GetSchedule_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var scheduleId = 1;

            _mockScheduleService
                .Setup(s => s.GetAsync(scheduleId))
                .ReturnsAsync((Schedule)null);

            // Act
            var result = await _controller.GetSchedule(scheduleId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_ValidSchedule_CreatesSchedule()
        {
            // Arrange
            var newSchedule = new Schedule { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0) };
            var createdSchedule = new Schedule { Id = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0) };

            _mockScheduleService
                .Setup(s => s.CreateAsync(newSchedule))
                .ReturnsAsync(createdSchedule);

            // Act
            var result = await _controller.Post(newSchedule) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Contains("Created successfully with ID", message);
        }

        [Fact]
        public async Task Post_InvalidSchedule_ReturnsCreationFailed()
        {
            // Arrange
            var newSchedule = new Schedule { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0) };

            _mockScheduleService
                .Setup(s => s.CreateAsync(newSchedule))
                .ThrowsAsync(new Exception("Creation failed"));

            // Act
            var result = await _controller.Post(newSchedule) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Creation failed", message);
        }

        [Fact]
        public async Task Put_ValidSchedule_UpdatesSchedule()
        {
            // Arrange
            var updatedSchedule = new Schedule { Id = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(10, 0, 0) };
            var existingSchedule = new Schedule { Id = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0) };

            _mockScheduleService
                .Setup(s => s.GetAsync(updatedSchedule.Id))
                .ReturnsAsync(existingSchedule);

            _mockScheduleService
                .Setup(s => s.UpdateAsync(updatedSchedule))
                .ReturnsAsync(updatedSchedule);

            // Act
            var result = await _controller.Put(updatedSchedule) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Contains("Update successful for Schedule", message);
        }

        [Fact]
        public async Task Put_InvalidSchedule_ReturnsUpdateNotSuccessful()
        {
            // Arrange
            var updatedSchedule = new Schedule { Id = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(10, 0, 0) };

            _mockScheduleService
                .Setup(s => s.GetAsync(updatedSchedule.Id))
                .ReturnsAsync((Schedule)null);

            // Act
            var result = await _controller.Put(updatedSchedule) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Update was not successful", message);
        }

        [Fact]
        public async Task Delete_ValidSchedule_DeletesSchedule()
        {
            // Arrange
            var scheduleId = 1;
            var existingSchedule = new Schedule { Id = scheduleId };

            _mockScheduleService
                .Setup(s => s.GetAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            // Act
            var result = await _controller.Delete(scheduleId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Delete successful", message);
        }

        [Fact]
        public async Task Delete_InvalidSchedule_ReturnsDeleteNotSuccessful()
        {
            // Arrange
            var scheduleId = 1;

            _mockScheduleService
                .Setup(s => s.GetAsync(scheduleId))
                .ReturnsAsync((Schedule)null);

            // Act
            var result = await _controller.Delete(scheduleId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var message = Assert.IsType<string>(result.Value);
            Assert.Equal("Delete was not successful", message);
        }

        [Fact]
        public async Task Filter_ValidRequest_ReturnsFilteredSchedules()
        {
            // Arrange
            var testSchedule = new Schedule
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(8, 0, 0),
                Teacher = new Teacher { Name = "Dr. Smith" },
                Discipline = new Discipline { Name = "Mathematics" }
            };

            var expectedSchedules = new List<Schedule>
        {
            new Schedule { Id = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0), Teacher = new Teacher { Name = "Dr. Smith" }, Discipline = new Discipline { Name = "Mathematics" } }
        };

            _mockScheduleService
                .Setup(s => s.GetAllAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(expectedSchedules);

            // Act
            var result = await _controller.Filter(testSchedule) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var actualSchedules = Assert.IsType<List<Schedule>>(result.Value);
            Assert.Single(actualSchedules);
            Assert.Equal(DayOfWeek.Monday, actualSchedules[0].DayOfWeek);
            Assert.Equal(new TimeSpan(8, 0, 0), actualSchedules[0].StartTime);
            Assert.Equal("Dr. Smith", actualSchedules[0].Teacher.Name);
            Assert.Equal("Mathematics", actualSchedules[0].Discipline.Name);
        }

        [Fact]
        public async Task Filter_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("DayOfWeek", "Required");

            // Act
            var result = await _controller.Filter(new Schedule()) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode); // HTTP 400 Bad Request
        }
    }
}