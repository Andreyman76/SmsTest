using SmsTest.Application.Entities;

namespace SmsTest.Application.Abstractions;

public interface IDishRepository
{
    Task ReplaceDishesAsync(
        IEnumerable<Dish> dishes,
        CancellationToken token = default);

    Task<Dish[]> FindDishesByArticlesAsync(
        IEnumerable<string> articles,
        CancellationToken token = default);
}