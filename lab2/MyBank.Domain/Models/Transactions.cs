namespace MyBank.Domain.Models;

public class Transaction
{
    public int Id { get; private set; }
    public int FromAccountId { get; private set; }
    public int ToAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(int fromAccountId, int toAccountId, decimal amount) => new Transaction
        {
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };
}