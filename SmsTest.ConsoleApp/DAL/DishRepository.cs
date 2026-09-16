using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmsTest.ConsoleApp.DAL.Entities;

namespace SmsTest.ConsoleApp.DAL;

internal class DishRepository(IServiceScopeFactory scopeFactory)
{
    public async Task ReplaceMenuAsync(
        IEnumerable<Dish> dishes,
        CancellationToken token = default)
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<SmsTestDbContext>();

        await context.Dishes.ExecuteDeleteAsync(token);
        await context.AddRangeAsync(dishes, token);
        await context.SaveChangesAsync(token);
    }

    public async Task<Dish[]> FindDishesByArticlesAsync(
        IEnumerable<string> articles,
        CancellationToken token = default)
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<SmsTestDbContext>();

        var dishes = await context.Dishes
            .AsNoTracking()
            .Where(x => articles.Contains(x.Article))
            .ToArrayAsync(token);

        return dishes;
    }
}