//using Xunit;
//using Moq;
//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using AutoMapper;
//using CourseTech.Core.Services;
//using CourseTech.Core.Models;
//using CourseTech.Core.DTOs.AppUser;
//using CourseTech.Service.Services;
//using Microsoft.AspNetCore.Identity;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using CourseTech.Shared.Enums;

//namespace TestProject2.Services
//{
//    public class UserServiceTests
//    {
//        private readonly Mock<UserManager<AppUser>> _userManagerMock;
//        private readonly Mock<IMapper> _mapperMock = new();
//        private readonly UserService _userService;

//        public UserServiceTests()
//        {
//            var store = new Mock<IUserStore<AppUser>>();
//            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
//            _userService = new UserService(_userManagerMock.Object, _mapperMock.Object);
//        }

//        [Fact]
//        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var user = new AppUser 
//            { 
//                Id = userId,
//                FirstName = "John",
//                LastName = "Doe",
//                Email = "john@example.com",
//                UserName = "johndoe"
//            };
//            var userDto = new AppUserDTO(userId, "John", "Doe", "john@example.com", "johndoe");

//            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
//                .ReturnsAsync(user);
//            _mapperMock.Setup(x => x.Map<AppUserDTO>(user))
//                .Returns(userDto);

//            // Act
//            var result = await _userService.GetByIdAsync(userId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(userDto, result.Data);
//        }

//        [Fact]
//        public async Task GetInstructorsAsync_ShouldReturnInstructors()
//        {
//            // Arrange
//            var instructors = new List<AppUser>
//            {
//                new AppUser { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
//                new AppUser { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
//            };
//            var instructorDtos = instructors.Select(i => 
//                new AppUserWithNamesDTO(i.Id, i.FirstName, i.LastName, i.Email ?? "", i.UserName ?? "")).ToList();

//            _userManagerMock.Setup(x => x.GetUsersInRoleAsync("Instructor"))
//                .ReturnsAsync(instructors);
//            _mapperMock.Setup(x => x.Map<IEnumerable<AppUserWithNamesDTO>>(instructors))
//                .Returns(instructorDtos);

//            // Act
//            var result = await _userService.GetInstructorsAsync();

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(instructorDtos, result.Data);
//        }

//        [Fact]
//        public async Task CreateAsync_ShouldCreateUser()
//        {
//            // Arrange
//            var createUserDto = new AppUserWithPasswordDTO(
//                "John", "Doe", "john@example.com", "johndoe", "Password123!");
//            var user = new AppUser
//            {
//                FirstName = createUserDto.FirstName,
//                LastName = createUserDto.LastName,
//                Email = createUserDto.Email,
//                UserName = createUserDto.UserName
//            };
//            var userDto = new AppUserDTO(Guid.NewGuid(), "John", "Doe", "john@example.com", "johndoe");

//            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), createUserDto.Password))
//                .ReturnsAsync(IdentityResult.Success);
//            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "Student"))
//                .ReturnsAsync(IdentityResult.Success);
//            _mapperMock.Setup(x => x.Map<AppUser>(createUserDto))
//                .Returns(user);
//            _mapperMock.Setup(x => x.Map<AppUserDTO>(user))
//                .Returns(userDto);

//            // Act
//            var result = await _userService.CreateAsync(createUserDto);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(userDto, result.Data);
//        }

//        [Fact]
//        public async Task AssignRoleAsync_ShouldAssignRole()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var user = new AppUser { Id = userId };
//            var roleName = "Instructor";

//            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
//                .ReturnsAsync(user);
//            _userManagerMock.Setup(x => x.AddToRoleAsync(user, roleName))
//                .ReturnsAsync(IdentityResult.Success);

//            // Act
//            var result = await _userService.AssignRoleAsync(userId, roleName);

//            // Assert
//            Assert.True(result.IsSuccess);
//        }

//        [Fact]
//        public async Task ResetPasswordAsync_ShouldResetPassword()
//        {
//            // Arrange
//            var email = "john@example.com";
//            var newPassword = "NewPassword123!";
//            var user = new AppUser { Email = email };

//            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
//                .ReturnsAsync(user);
//            _userManagerMock.Setup(x => x.RemovePasswordAsync(user))
//                .ReturnsAsync(IdentityResult.Success);
//            _userManagerMock.Setup(x => x.AddPasswordAsync(user, newPassword))
//                .ReturnsAsync(IdentityResult.Success);

//            // Act
//            var result = await _userService.ResetPasswordAsync(email, newPassword);

//            // Assert
//            Assert.True(result.IsSuccess);
//        }
//    }
//}
