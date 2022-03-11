using Ada.FirstCatering.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ada.FirstCatering.API;

public class FirstCateringContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Card> Cards { get; set; }

    protected FirstCateringContext()
    {
    }

    public FirstCateringContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var (cards, employees) = CreateDefaultData();
        modelBuilder.Entity<Card>().HasData(cards);
        modelBuilder.Entity<Employee>().HasData(employees);
        
        base.OnModelCreating(modelBuilder);
    }

    private static (Card[], Employee[]) CreateDefaultData()
    {
        const string defaultCardId = "r7jTG7dqBy5wGO4L";

        var employees = new List<Employee>()
        {
            new()
            {
                Id = 1,
                CardId = defaultCardId,
                Email = "Alex.Redding@ada.ac.uk",
                FirstName = "Alex",
                LastName = "Redding",
                PhoneNumber = "+447474286310"
            }
        };
        var cards = new List<Card>
        {
            new() // Default Card linked to Alex
            {
                Id = defaultCardId,
                Pin = 1234,
                EmployeeId = 1
            }
        };

        var cardsEmployeeId = 2;
        for (var i = 0; i < 20; i++)
        {
            var shouldHaveOwner = Random.Shared.Next(1, 3) == 1;
            cards.Add(new Card
            {
                Id = RandomHelper.GenerateRandomString(16),
                Pin = Random.Shared.Next(1000, 9999),
                EmployeeId = shouldHaveOwner ? cardsEmployeeId++ : null
            });
        }

        var cardNum = 1;
        foreach (var card in cards.Where(x => x.EmployeeId >= 2))
        {
            employees.Add(new Employee()
            {
                Id = card.EmployeeId!.Value,
                Email = $"Test-Employee-{cardNum}@FirstCatering.com",
                FirstName = "Test",
                LastName = $"Employee {cardNum}",
                PhoneNumber = RandomHelper.GenerateRandomPhoneNumber(),
                CardId = card.Id
            });

            cardNum++;
        }

        return (cards.ToArray(), employees.ToArray());
    }
}