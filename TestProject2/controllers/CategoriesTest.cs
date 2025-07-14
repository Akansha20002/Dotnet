using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.Category;
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
    public class CategoriesTests
    {
        private readonly Mock<ICategoryService> _serviceMock;
        private readonly CategoriesController _controller;

        public CategoriesTests()
        {
            _serviceMock = new Mock<ICategoryService>();
            _controller = new CategoriesController(_serviceMock.Object);
        }

        private static CategoryDTO CreateSampleCategory() => new(Guid.NewGuid(), "Programming");

        [Fact]
        public async Task GetById_ReturnsOk_WhenCategoryExists()
        {
            var id = Guid.NewGuid();
            var result = ServiceResult<CategoryDTO>.Success(CreateSampleCategory());
            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(result);

            var response = await _controller.GetById(id) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenNotFound()
        {
            var id = Guid.NewGuid();
            var result = ServiceResult<CategoryDTO>.Fail("Not found");
            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(result);

            var response = await _controller.GetById(id) as ObjectResult;

            Assert.Equal(400, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithCategories()
        {
            var result = ServiceResult<IEnumerable<CategoryDTO>>.Success(new[] { CreateSampleCategory() });
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(result);

            var response = await _controller.GetAll() as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }

        [Fact]
        public async Task GetCategoriesWithCourses_ReturnsOk()
        {
            var result = ServiceResult<IEnumerable<CategoryDTO>>.Success(new[] { CreateSampleCategory() });
            _serviceMock.Setup(s => s.GetCategoriesWithCoursesAsync()).ReturnsAsync(result);

            var response = await _controller.GetCategoriesWithCourses() as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task GetCategoryWithCourses_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var result = ServiceResult<IEnumerable<CategoryDTO>>.Success(new[] { CreateSampleCategory() });
            _serviceMock.Setup(s => s.GetCategoryWithCoursesAsync(id)).ReturnsAsync(result);

            var response = await _controller.GetCategoryWithCourses(id) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenSuccessful()
        {
            var createDto = new CategoryCreateDTO("Design");
            var result = ServiceResult<CategoryDTO>.Success(new CategoryDTO(Guid.NewGuid(), createDto.Name));

            _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(result);

            var response = await _controller.Create(createDto) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccessful()
        {
            var updateDto = new CategoryUpdateDTO(Guid.NewGuid(), "Updated Category");
            var result = ServiceResult<CategoryDTO>.Success(new CategoryDTO(updateDto.Id, updateDto.Name));

            _serviceMock.Setup(s => s.UpdateAsync(updateDto)).ReturnsAsync(result);

            var response = await _controller.Update(updateDto) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var result = ServiceResult.Success();

            _serviceMock.Setup(s => s.SoftDeleteAsync(id)).ReturnsAsync(result);

            var response = await _controller.Delete(id) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }
    }
}
