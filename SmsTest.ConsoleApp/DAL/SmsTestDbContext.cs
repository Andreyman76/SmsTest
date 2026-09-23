using Microsoft.EntityFrameworkCore;
using SmsTest.Application.Entities;

namespace SmsTest.ConsoleApp.DAL;

internal class SmsTestDbContext(
    DbContextOptions options)
    : DbContext(options)
{
    public DbSet<Dish> Dishes => Set<Dish>();
}