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

namespace TestProject2.Xunit.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly CategoryService _categoryService;

        private const string CategoryNotFoundMessage = "not found";
        private const string DefaultCategoryName = "Test Category";

        public CategoryServiceTests()
        {
            _unitOfWorkMock.Setup(x => x.Category).Returns(_categoryRepoMock.Object);
            _categoryService = new CategoryService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact, Trait("Method", "GetByIdAsync")]
        public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnMappedDto()
        {
            var categoryId = Guid.NewGuid();
            var category = CreateCategory(categoryId);
            var categoryDto = CreateCategoryDTO(categoryId);

            SetupGetById(categoryId, category);
            SetupMapToDTO(category, categoryDto);

            var result = await _categoryService.GetByIdAsync(categoryId);

            Assert.True(result.IsSuccess);
            Assert.Equal(categoryDto, result.Data);
        }

        [Fact, Trait("Method", "GetByIdAsync")]
        public async Task GetByIdAsync_WhenCategoryDoesNotExist_ShouldReturnFailure()
        {
            var categoryId = Guid.NewGuid();
            _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category)null!);

            var result = await _categoryService.GetByIdAsync(categoryId);

            Assert.False(result.IsSuccess);
            Assert.Contains(CategoryNotFoundMessage, result.ErrorMessage![0]);
        }

        [Fact, Trait("Method", "GetAllAsync")]
        public async Task GetAllAsync_WhenCalled_ShouldReturnMappedDtoList()
        {
            var categories = new List<Category>
            {
                CreateCategory(Guid.NewGuid(), "Category 1"),
                CreateCategory(Guid.NewGuid(), "Category 2")
            };

            var categoryDtos = new List<CategoryDTO>
            {
                CreateCategoryDTO(categories[0].Id, "Category 1"),
                CreateCategoryDTO(categories[1].Id, "Category 2")
            };

            _categoryRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoryDTO>>(categories)).Returns(categoryDtos);

            var result = await _categoryService.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(categoryDtos, result.Data);
        }

        [Fact, Trait("Method", "CreateAsync")]
        public async Task CreateAsync_WhenCalled_ShouldInsertCategoryAndReturnDto()
        {
            var createDto = new CategoryCreateDTO("New Category");
            var category = CreateCategory(Guid.NewGuid(), "New Category");
            var categoryDto = CreateCategoryDTO(category.Id, "New Category");

            _mapperMock.Setup(x => x.Map<Category>(createDto)).Returns(category);
            _categoryRepoMock.Setup(x => x.InsertAsync(category)).Returns(Task.CompletedTask);
            SetupMapToDTO(category, categoryDto);

            var result = await _categoryService.CreateAsync(createDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(categoryDto, result.Data);
            _categoryRepoMock.Verify(x => x.InsertAsync(category), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact, Trait("Method", "UpdateAsync")]
        public async Task UpdateAsync_WhenCategoryExists_ShouldUpdateAndReturnDto()
        {
            var categoryId = Guid.NewGuid();
            var updateDto = new CategoryUpdateDTO(categoryId, "Updated Category");
            var category = CreateCategory(categoryId, "Original Category");
            var updatedDto = CreateCategoryDTO(categoryId, "Updated Category");

            SetupGetById(updateDto.Id, category);
            SetupMapToDTO(category, updatedDto);

            var result = await _categoryService.UpdateAsync(updateDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(updatedDto, result.Data);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact, Trait("Method", "SoftDeleteAsync")]
        public async Task SoftDeleteAsync_WhenCategoryExists_ShouldMarkDeletedAndSave()
        {
            var categoryId = Guid.NewGuid();
            var category = CreateCategory(categoryId, "To Delete");

            SetupGetById(categoryId, category);

            var result = await _categoryService.SoftDeleteAsync(categoryId);

            Assert.True(result.IsSuccess);
            _categoryRepoMock.Verify(x => x.Update(category), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        // -------------------- Helper Methods --------------------

        private Category CreateCategory(Guid? id = null, string name = DefaultCategoryName)
            => new Category { Id = id ?? Guid.NewGuid(), Name = name };

        private CategoryDTO CreateCategoryDTO(Guid id, string name = DefaultCategoryName)
            => new CategoryDTO(id, name);

        private void SetupMapToDTO(Category category, CategoryDTO dto)
            => _mapperMock.Setup(x => x.Map<CategoryDTO>(category)).Returns(dto);

        private void SetupGetById(Guid id, Category category)
            => _categoryRepoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(category);
    }
}
