namespace MyBank.Application.ReadModels;

public record AccountReadModel(int Id, string AccountNumber, decimal Balance, string Currency);
    