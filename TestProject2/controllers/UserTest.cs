using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.AppUser;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Services;
using CourseTech.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestProject2.controllers
{
    public class UserTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        private static readonly Guid TestUserId = Guid.NewGuid();

        public UserTest()
        {
            _userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            _controller = new UsersController(_userServiceMock.Object);
        }

        private static AppUserDTO CreateSampleUser() =>
            new(TestUserId, "Jane", "Doe", "jane.doe@example.com", "1234567890");

        private static AppUserWithPasswordDTO CreateSampleUserWithPassword() =>
            new(TestUserId, "Jane", "Doe", "jane.doe@example.com", "Password123!", "1234567890");

        private static AppUserWithNamesDTO CreateSampleUserWithNames() =>
            new(TestUserId, "Jane Doe");

        private static ResetPasswordDTO CreateResetDto() =>
            new("jane.doe@example.com", "NewPass456!");

        #region GetById

        [Fact]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var expected = ServiceResult<AppUserDTO>.Success(CreateSampleUser());
            _userServiceMock.Setup(s => s.GetByIdAsync(TestUserId)).ReturnsAsync(expected);

            var result = await _controller.GetById(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var expected = ServiceResult<IEnumerable<AppUserDTO>>.Success(new[] { CreateSampleUser() });
            _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(expected);

            var result = await _controller.GetAll();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region GetInstructors

        [Fact]
        public async Task GetInstructors_ReturnsOk()
        {
            var expected = ServiceResult<IEnumerable<AppUserWithNamesDTO>>.Success(new[] { CreateSampleUserWithNames() });
            _userServiceMock.Setup(s => s.GetInstructorsAsync()).ReturnsAsync(expected);

            var result = await _controller.GetInstructorsAsync();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region GetStudents

        [Fact]
        public async Task GetStudents_ReturnsOk()
        {
            var expected = ServiceResult<IEnumerable<AppUserWithNamesDTO>>.Success(new[] { CreateSampleUserWithNames() });
            _userServiceMock.Setup(s => s.GetStudentsAsync()).ReturnsAsync(expected);

            var result = await _controller.GetStudentsAsync();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_ValidDto_ReturnsOk()
        {
            var dto = CreateSampleUserWithPassword();
            var expected = ServiceResult<AppUserDTO>.Success(CreateSampleUser());

            _userServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(expected);

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region ResetPassword

        [Fact]
        public async Task ResetPassword_ValidRequest_ReturnsOk()
        {
            var dto = CreateResetDto();
            var expected = ServiceResult.Success();

            _userServiceMock.Setup(s => s.ResetPasswordAsync(dto.Email, dto.NewPassword)).ReturnsAsync(expected);

            var result = await _controller.ResetPasswordAsync(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ValidDto_ReturnsOk()
        {
            var dto = CreateSampleUser();
            var expected = ServiceResult.Success();

            _userServiceMock.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(expected);

            var result = await _controller.Update(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ValidId_ReturnsOk()
        {
            var expected = ServiceResult.Success();

            _userServiceMock.Setup(s => s.SoftDeleteAsync(TestUserId)).ReturnsAsync(expected);

            var result = await _controller.Delete(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion
    }
}
