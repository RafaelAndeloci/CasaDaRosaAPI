using CasaDaRosa.Application.Abstractions;
using CasaDaRosa.Domain.Entities.Categories;
using CasaDaRosa.Domain.Entities.Products;
using CasaDaRosa.Domain.Entities.Users;
using CasaDaRosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CasaDaRosa.Infrastructure.Persistence;

public sealed class DatabaseSeeder(
    CasaDaRosaDbContext dbContext,
    ISecurityService securityService)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(cancellationToken);
        var categories = await SeedCategoriesAsync(cancellationToken);
        await SeedProductsAsync(categories, cancellationToken);
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        var existingUsers = await dbContext.Users
            .AsTracking()
            .ToListAsync(cancellationToken);

        var usersToSeed = new[]
        {
            new SeedUserDefinition(
                FullName: "Admin Casa da Rosa",
                Email: "admin@casadarosa.local",
                Password: "Admin@123456!",
                PhoneNumber: "+55 (16) 99999-9999",
                IsAdmin: true,
                ConfirmEmail: true),
            new SeedUserDefinition(
                FullName: "Cliente Ativo",
                Email: "cliente@casadarosa.local",
                Password: "Cliente@123456!",
                PhoneNumber: "+55 (16) 98888-7777",
                IsAdmin: false,
                ConfirmEmail: true),
            new SeedUserDefinition(
                FullName: "Cliente Pendente",
                Email: "pendente@casadarosa.local",
                Password: "Pendente@123456!",
                PhoneNumber: "+55 (16) 97777-6666",
                IsAdmin: false,
                ConfirmEmail: false)
        };

        var hasChanges = false;

        foreach (var seedUser in usersToSeed)
        {
            var exists = existingUsers.Any(user => string.Equals(user.Email.Value, seedUser.Email, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                continue;
            }

            var passwordHash = securityService.HashPassword(seedUser.Password);
            var user = seedUser.IsAdmin
                ? User.CreateAdmin(seedUser.FullName, seedUser.Email, passwordHash, seedUser.PhoneNumber)
                : User.Create(seedUser.FullName, seedUser.Email, passwordHash, seedUser.PhoneNumber);

            if (!seedUser.IsAdmin && seedUser.ConfirmEmail)
            {
                user.ConfirmEmail(user.EmailConfirmationToken);
            }

            dbContext.Users.Add(user);
            existingUsers.Add(user);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<Dictionary<string, Category>> SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        var existingCategories = await dbContext.Categories
            .AsTracking()
            .ToListAsync(cancellationToken);

        var categoriesToSeed = new[]
        {
            new SeedCategoryDefinition("Rosas", "Rosas frescas para presentear e decorar.", true),
            new SeedCategoryDefinition("Buquês", "Buquês especiais para ocasiões marcantes.", true),
            new SeedCategoryDefinition("Presentes", "Opções de presentes artesanais e delicados.", true)
        };

        var hasChanges = false;

        foreach (var seedCategory in categoriesToSeed)
        {
            var exists = existingCategories.Any(category => string.Equals(category.Name.Value, seedCategory.Name, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                continue;
            }

            var category = Category.Create(seedCategory.Name, seedCategory.Description, seedCategory.IsActive);
            dbContext.Categories.Add(category);
            existingCategories.Add(category);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return existingCategories.ToDictionary(category => category.Name.Value, StringComparer.OrdinalIgnoreCase);
    }

    private async Task SeedProductsAsync(IReadOnlyDictionary<string, Category> categories, CancellationToken cancellationToken)
    {
        var existingProducts = await dbContext.Products
            .AsTracking()
            .ToListAsync(cancellationToken);

        var productsToSeed = new[]
        {
            new SeedProductDefinition("Rosas", "Rosa Vermelha Premium", "Unidade de rosa vermelha premium.", 12.90m, 50, true),
            new SeedProductDefinition("Rosas", "Rosa Branca Especial", "Unidade de rosa branca especial.", 11.90m, 40, true),
            new SeedProductDefinition("Buquês", "Buquê Romântico", "Buquê com rosas vermelhas e acabamento especial.", 89.90m, 15, true),
            new SeedProductDefinition("Buquês", "Buquê Encantado", "Buquê colorido para celebrações e presentes.", 99.90m, 10, true),
            new SeedProductDefinition("Presentes", "Cesta Floral", "Cesta com flores e itens delicados para presente.", 149.90m, 8, true),
            new SeedProductDefinition("Presentes", "Box Casa da Rosa", "Box especial com flores e mimos artesanais.", 129.90m, 12, true)
        };

        var hasChanges = false;

        foreach (var seedProduct in productsToSeed)
        {
            if (!categories.TryGetValue(seedProduct.CategoryName, out var category))
            {
                continue;
            }

            var exists = existingProducts.Any(product => string.Equals(product.Name.Value, seedProduct.Name, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                continue;
            }

            var product = Product.Create(
                category.Id,
                seedProduct.Name,
                seedProduct.Description,
                new Money(seedProduct.Price, Currency.Brl),
                seedProduct.StockQuantity);

            if (!seedProduct.IsActive)
            {
                product.Deactivate();
            }

            dbContext.Products.Add(product);
            existingProducts.Add(product);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private sealed record SeedUserDefinition(
        string FullName,
        string Email,
        string Password,
        string? PhoneNumber,
        bool IsAdmin,
        bool ConfirmEmail);

    private sealed record SeedCategoryDefinition(
        string Name,
        string Description,
        bool IsActive);

    private sealed record SeedProductDefinition(
        string CategoryName,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        bool IsActive);
}
