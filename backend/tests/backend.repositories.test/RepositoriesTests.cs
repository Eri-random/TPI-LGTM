using backend.data.DataContext;
using backend.data.Models;
using backend.repositories.implementations;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.test
{
    [TestFixture]
    public class RepositoriesTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Test]
        public async Task GetAllAsync_WithIncludeProperties_ReturnsEntitiesWithIncludes()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);

            context.Usuarios.Add(new Usuario { Id = 1, Nombre = "User1", Rol = new Rol { RolId = 1, Nombre = "Role1" } });
            context.SaveChanges();

            // Act
            var result = await repository.GetAllAsync(u => u.Rol);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.NotNull(result.First().Rol);
            Assert.That("Role1", Is.EqualTo(result.First().Rol.Nombre));
        }

        [Test]
        public async Task GetByIdAsync_WithIncludeProperties_ReturnsEntityWithIncludes()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);

            context.Usuarios.Add(new Usuario { Id = 1, Nombre = "User1", Rol = new Rol { RolId = 1, Nombre = "Role1" } });
            context.SaveChanges();

            // Act
            var result = await repository.GetByIdAsync(1, u => u.Rol);

            // Assert
            Assert.NotNull(result);
            Assert.That("Role1", Is.EqualTo(result.Rol.Nombre));
        }

        [Test]
        public async Task AddAsync_ValidEntity_AddsEntity()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);
            var user = new Usuario { Id = 1, Nombre = "User1" };

            // Act
            await repository.AddAsync(user);

            // Assert
            Assert.NotNull(context.Usuarios.FirstOrDefault(u => u.Id == 1 && u.Nombre == "User1"));
        }

        [Test]
        public async Task UpdateAsync_ValidEntity_UpdatesEntity()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);
            var user = new Usuario { Id = 1, Nombre = "User1" };
            context.Usuarios.Add(user);
            context.SaveChanges();

            // Act
            user.Nombre = "UpdatedUser";
            await repository.UpdateAsync(user);

            // Assert
            Assert.That(context.Usuarios.First().Nombre, Is.EqualTo("UpdatedUser"));
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesEntity()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);
            var user = new Usuario { Id = 1, Nombre = "User1" };
            context.Usuarios.Add(user);
            context.SaveChanges();

            // Act
            await repository.DeleteAsync(1);

            // Assert
            Assert.IsEmpty(context.Usuarios);
        }

        [Test]
        public async Task UpdateRangeAsync_ValidEntities_UpdatesEntities()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);
            var users = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "User1" },
                new Usuario { Id = 2, Nombre = "User2" }
            };
            context.Usuarios.AddRange(users);
            context.SaveChanges();

            // Act
            users[0].Nombre = "UpdatedUser1";
            users[1].Nombre = "UpdatedUser2";
            await repository.UpdateRangeAsync(users);

            // Assert
            Assert.True(context.Usuarios.First(u => u.Id == 1).Nombre.Equals("UpdatedUser1"));
            Assert.True(context.Usuarios.First(u => u.Id == 2).Nombre.Equals("UpdatedUser2"));
        }

        [Test]
        public async Task AddRangeAsync_ValidEntities_AddsEntities()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);
            var users = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "User1" },
                new Usuario { Id = 2, Nombre = "User2" }
            };

            // Act
            await repository.AddRangeAsync(users);

            // Assert
            Assert.True(context.Usuarios.Count() == 2);
            Assert.True(context.Usuarios.First(u => u.Id == 1).Nombre.Equals("User1"));
            Assert.True(context.Usuarios.First(u => u.Id == 2).Nombre.Equals("User2"));
        }

        [Test]
        public async Task DeleteRangeAsync_ValidEntities_DeletesEntities()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new Repository<Usuario>(context);
            var users = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "User1" },
                new Usuario { Id = 2, Nombre = "User2" }
            };
            context.Usuarios.AddRange(users);
            context.SaveChanges();

            // Act
            await repository.DeleteRangeAsync(users);

            // Assert
            Assert.IsEmpty(context.Usuarios);
        }
    }
}
