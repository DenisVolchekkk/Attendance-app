using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using StudyProj.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1
{
    public class RolesControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<Role>> _mockRoleManager;
        private readonly RolesController _controller;

        public RolesControllerTests()
        {
            // Mock UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Mock RoleManager
            var roleStoreMock = new Mock<IRoleStore<Role>>();
            _mockRoleManager = new Mock<RoleManager<Role>>(
                roleStoreMock.Object, null, null, null, null);

            _controller = new RolesController(_mockRoleManager.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task UserList_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", UserName = "user1" },
                new User { Id = "2", UserName = "user2" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));

            mockDbSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(users.Provider));

            mockDbSet.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(users.GetEnumerator());

            _mockUserManager.Setup(x => x.Users)
                .Returns(mockDbSet.Object);

            // Act
            var result = await _controller.UserList();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<List<User>>(jsonResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task RoleList_ReturnsAllRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { Id = "1", Name = "Dean" },
                new Role { Id = "2", Name = "Chief" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Role>>();
            mockDbSet.As<IAsyncEnumerable<Role>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<Role>(roles.GetEnumerator()));

            mockDbSet.As<IQueryable<Role>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Role>(roles.Provider));

            mockDbSet.As<IQueryable<Role>>()
                .Setup(m => m.Expression)
                .Returns(roles.Expression);
            mockDbSet.As<IQueryable<Role>>()
                .Setup(m => m.ElementType)
                .Returns(roles.ElementType);
            mockDbSet.As<IQueryable<Role>>()
                .Setup(m => m.GetEnumerator())
                .Returns(roles.GetEnumerator());

            _mockRoleManager.Setup(x => x.Roles)
                .Returns(mockDbSet.Object);

            // Act
            var result = await _controller.RoleList();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedRoles = Assert.IsAssignableFrom<List<Role>>(jsonResult.Value);
            Assert.Equal(2, returnedRoles.Count);
        }

        [Fact]
        public async Task Put_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Put("1", new List<string>());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Put_UpdatesUserRolesSuccessfully()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "testuser" };
            var currentRoles = new List<string> { "Chief" };
            var newRoles = new List<string> { "Dean", "Deaputy Dean" };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(currentRoles);
            _mockUserManager.Setup(x => x.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Put("1", newRoles);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal($"Update successful for user with ID: {user.Id}", jsonResult.Value);
        }

        [Fact]
        public async Task GetUserRoles_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserRoles("1");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetUserRoles_ReturnsUserRoles()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "testuser" };
            var roles = new List<string> { "Dean", "Chief" };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            // Act
            var result = await _controller.GetUserRoles("1");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedRoles = Assert.IsAssignableFrom<IList<string>>(jsonResult.Value);
            Assert.Equal(2, returnedRoles.Count);
        }

        [Fact]
        public async Task GetUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUser("1");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetUser_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "testuser" };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser("1");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedUser = Assert.IsType<User>(jsonResult.Value);
            Assert.Equal(user.Id, returnedUser.Id);
        }
    }

    // Helper classes for async testing
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}