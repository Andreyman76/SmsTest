using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmsTest.Application.Abstractions;
using SmsTest.Application.Entities;

namespace SmsTest.ConsoleApp.DAL;

/// <summary>
/// Репозиторий, инкапсулирующий логику работы с DbContext
/// </summary>
/// <param name="scopeFactory"></param>
internal class DishRepository(
    IServiceScopeFactory scopeFactory) : IDishRepository
{
    /// <summary>
    /// Обновление меню на актуальное
    /// </summary>
    /// <param name="dishes"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task ReplaceDishesAsync(
        IEnumerable<Dish> dishes,
        CancellationToken token = default)
    {
        using var scope = scopeFactory.CreateScope();
        using var context
            = scope.ServiceProvider.GetRequiredService<SmsTestDbContext>();

        await using var transaction
            = await context.Database.BeginTransactionAsync(token);

        await context.Dishes.ExecuteDeleteAsync(token);

        context.Dishes.AddRange(dishes);

        await context.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }

    /// <summary>
    /// Поиск товаров по атикулам
    /// </summary>
    /// <param name="articles"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Dish[]> FindDishesByArticlesAsync(
        IEnumerable<string> articles,
        CancellationToken token = default)
    {
        using var scope = scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<SmsTestDbContext>();

        var dishes = await context.Dishes
            .AsNoTracking()
            .Where(x => articles.Contains(x.Article))
            .ToArrayAsync(token);

        return dishes;
    }
}