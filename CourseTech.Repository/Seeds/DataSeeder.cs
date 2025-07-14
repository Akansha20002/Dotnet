using CourseTech.Core.Models;
using CourseTech.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Repository.Seeds
{
    public class DataSeeder(AppDbContext _context, UserManager<AppUser> _userManager, RoleManager<AppRole> _roleManager)
    {
        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedCategoriesAsync();
            await SeedCoursesAsync();
        }
        private async Task SeedRolesAsync()
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new AppRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }
        private async Task SeedUsersAsync()
        {
            if (!await _userManager.Users.AnyAsync())
            {
                var adminUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    UserName = "admin@admin.com",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true
                };

                await _userManager.CreateAsync(adminUser, "Admin123*");
                await _userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());

                var instructorUser1 = new AppUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Fatih",
                    LastName = "Cakiroglu",
                    Email = "fatihcakiroglu@instructor.com",
                    EmailConfirmed = true,
                    UserName = "fatihcakiroglu@instructor.com"
                };

                var instructorUser2 = new AppUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ahmet",
                    LastName = "Kaya",
                    Email = "ahmetkaya@instructor.com",
                    EmailConfirmed = true,
                    UserName = "ahmet@instructor.com"
                };

                await _userManager.CreateAsync(instructorUser1, "Fatih123*");
                await _userManager.CreateAsync(instructorUser2, "Ahmet123*");

                await _userManager.AddToRoleAsync(instructorUser1, Roles.Instructor.ToString());
                await _userManager.AddToRoleAsync(instructorUser2, Roles.Instructor.ToString());
            }
        }
        private async Task SeedCategoriesAsync()
        {
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Name = "Programming" },
                    new Category { Id = Guid.NewGuid(), Name = "Finance" },
                    new Category { Id = Guid.NewGuid(), Name = "Marketing" }
                };

                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }
        }
        private async Task SeedCoursesAsync()
        {
            if (!await _context.Courses.AnyAsync())
            {
                var instructor1 = await _userManager.FindByEmailAsync("fatihcakiroglu@instructor.com");
                var instructor2 = await _userManager.FindByEmailAsync("ahmetkaya@instructor.com");
                var categories = await _context.Categories.ToListAsync();

                var programmingCategory = categories.FirstOrDefault(c => c.Name == "Programming");
                var financeCategory = categories.FirstOrDefault(c => c.Name == "Finance");
                var marketingCategory = categories.FirstOrDefault(c => c.Name == "Marketing");

                if (programmingCategory == null || financeCategory == null || marketingCategory == null)
                {
                    throw new Exception("Required categories not found in the database.");
                }

                var courses = new List<Course>
                {
                    new Course(
                        title: "C# Fundamentals",
                        description: "Learn the basics of C# programming language",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=0pZVjJ7v4w8",
                        level: CourseLevel.Beginner,
                        language: CourseLanguage.English,
                        price: 50,
                        duration: TimeSpan.FromHours(5),
                        instructorId: instructor1!.Id,
                        categoryId: programmingCategory.Id
                    ),
                    new Course(
                        title: "Advanced Stock Trading",
                        description: "Master the art of stock market trading",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example2",
                        level: CourseLevel.Advanced,
                        language: CourseLanguage.English,
                        price: 199,
                        duration: TimeSpan.FromHours(12),
                        instructorId: instructor1.Id,
                        categoryId: financeCategory.Id
                    ),
                    new Course(
                        title: "Digital Marketing Essentials",
                        description: "Learn the fundamentals of digital marketing",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example3",
                        level: CourseLevel.Beginner,
                        language: CourseLanguage.English,
                        price: 79,
                        duration: TimeSpan.FromHours(6),
                        instructorId: instructor1.Id,
                        categoryId: marketingCategory.Id
                    ),
                    new Course(
                        title: "Python for Data Analysis",
                        description: "Learn Python programming for financial analysis",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example4",
                        level: CourseLevel.Intermediate,
                        language: CourseLanguage.English,
                        price: 149,
                        duration: TimeSpan.FromHours(8),
                        instructorId: instructor2!.Id,
                        categoryId: programmingCategory.Id
                    ),
                    new Course(
                        title: "Investment Banking Fundamentals",
                        description: "Introduction to investment banking concepts",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example5",
                        level: CourseLevel.Beginner,
                        language: CourseLanguage.English,
                        price: 129,
                        duration: TimeSpan.FromHours(10),
                        instructorId: instructor2.Id,
                        categoryId: financeCategory.Id
                    ),
                    new Course(
                        title: "Social Media Marketing",
                        description: "Master social media marketing strategies",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example6",
                        level: CourseLevel.Intermediate,
                        language: CourseLanguage.English,
                        price: 89,
                        duration: TimeSpan.FromHours(7),
                        instructorId: instructor2.Id,
                        categoryId: marketingCategory.Id
                    ),
                    new Course(
                        title: "JavaScript Advanced",
                        description: "Advanced concepts in JavaScript development",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example7",
                        level: CourseLevel.Advanced,
                        language: CourseLanguage.English,
                        price: 179,
                        duration: TimeSpan.FromHours(15),
                        instructorId: instructor2.Id,
                        categoryId: programmingCategory.Id
                    ),
                    new Course(
                        title: "Cryptocurrency Trading",
                        description: "Learn to trade cryptocurrencies effectively",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example8",
                        level: CourseLevel.Intermediate,
                        language: CourseLanguage.English,
                        price: 159,
                        duration: TimeSpan.FromHours(9),
                        instructorId: instructor1.Id,
                        categoryId: financeCategory.Id
                    ),
                    new Course(
                        title: "Email Marketing Mastery",
                        description: "Master email marketing campaigns",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example9",
                        level: CourseLevel.Advanced,
                        language: CourseLanguage.English,
                        price: 139,
                        duration: TimeSpan.FromHours(8),
                        instructorId: instructor2.Id,
                        categoryId: marketingCategory.Id
                    ),
                    new Course(
                        title: "React.js Fundamentals",
                        description: "Learn the basics of React.js framework",
                        imageUrl: "https://via.placeholder.com/150",
                        videoUrl: "https://www.youtube.com/watch?v=example10",
                        level: CourseLevel.Beginner,
                        language: CourseLanguage.English,
                        price: 99,
                        duration: TimeSpan.FromHours(6),
                        instructorId: instructor2.Id,
                        categoryId: programmingCategory.Id
                    )
                };

                foreach (var course in courses)
                {
                    course.Publish();
                }
                
                await _context.Courses.AddRangeAsync(courses);
                await _context.SaveChangesAsync();
            }
        }
    }
}
