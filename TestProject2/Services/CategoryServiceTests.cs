using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Core.Models;
using CourseTech.Core.DTOs.Category;
using CourseTech.Service.Services;
using CourseTech.Core.Repositories;
using CourseTech.Shared;

namespace TestProject2.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _unitOfWorkMock.Setup(x => x.Category).Returns(_categoryRepoMock.Object);
            _categoryService = new CategoryService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Test Category" };
            var categoryDto = new CategoryDTO(categoryId, "Test Category");

            _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
            _mapperMock.Setup(x => x.Map<CategoryDTO>(category)).Returns(categoryDto);

            var result = await _categoryService.GetByIdAsync(categoryId);

            Assert.True(result.IsSuccess);
            Assert.Equal(categoryDto, result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFail_WhenCategoryDoesNotExist()
        {
            var categoryId = Guid.NewGuid();
            _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId))
                             .ReturnsAsync((Category)null!); // Fix: Simulate not found

            var result = await _categoryService.GetByIdAsync(categoryId);

            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category 1" },
                new Category { Id = Guid.NewGuid(), Name = "Category 2" }
            };

            var categoryDtos = new List<CategoryDTO>
            {
                new CategoryDTO(categories[0].Id, "Category 1"),
                new CategoryDTO(categories[1].Id, "Category 2")
            };

            _categoryRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoryDTO>>(categories)).Returns(categoryDtos);

            var result = await _categoryService.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(categoryDtos, result.Data);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewCategory()
        {
            var createDto = new CategoryCreateDTO("New Category");
            var category = new Category { Id = Guid.NewGuid(), Name = "New Category" };
            var categoryDto = new CategoryDTO(category.Id, "New Category");

            _mapperMock.Setup(x => x.Map<Category>(createDto)).Returns(category);
            _categoryRepoMock.Setup(x => x.InsertAsync(category)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<CategoryDTO>(category)).Returns(categoryDto);

            var result = await _categoryService.CreateAsync(createDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(categoryDto, result.Data);
            _categoryRepoMock.Verify(x => x.InsertAsync(category), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingCategory()
        {
            var categoryId = Guid.NewGuid();
            var updateDto = new CategoryUpdateDTO(categoryId, "Updated Category");
            var existingCategory = new Category { Id = categoryId, Name = "Original Category" };
            var updatedCategoryDto = new CategoryDTO(categoryId, "Updated Category");

            _categoryRepoMock.Setup(x => x.GetByIdAsync(updateDto.Id)).ReturnsAsync(existingCategory);
            _mapperMock.Setup(x => x.Map<CategoryDTO>(existingCategory)).Returns(updatedCategoryDto);

            var result = await _categoryService.UpdateAsync(updateDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(updatedCategoryDto, result.Data);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldDeleteCategory()
        {
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Category to Delete" };

            _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);

            var result = await _categoryService.SoftDeleteAsync(categoryId);

            Assert.True(result.IsSuccess);
            _categoryRepoMock.Verify(x => x.Update(category), Times.Once); // ? Corrected
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
