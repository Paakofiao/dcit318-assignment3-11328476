using System;
using System.Collections.Generic;

namespace FinanceManagementDemo
{
    public record Transaction
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public decimal Amount { get; init; }
        public string Category { get; init; } = string.Empty;

        public Transaction(int id, DateTime date, decimal amount, string category)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.", nameof(category));

            Id = id;
            Date = date;
            Amount = amount;
            Category = category;
        }
    }

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            if (initialBalance < 0) throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Account {AccountNumber}: deducted {transaction.Amount:C}. New balance: {Balance:C}.");
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }
            base.ApplyTransaction(transaction);
        }
    }

    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            var savings = new SavingsAccount(accountNumber: "SA-001", initialBalance: 1000m);
            Console.WriteLine($"Created SavingsAccount {savings.AccountNumber} with starting balance {savings.Balance:C}.");

            var t1 = new Transaction(id: 1, date: DateTime.Now, amount: 150.75m, category: "Groceries");
            var t2 = new Transaction(id: 2, date: DateTime.Now, amount: 300.00m, category: "Utilities");
            var t3 = new Transaction(id: 3, date: DateTime.Now, amount: 200.00m, category: "Entertainment");

            ITransactionProcessor mobile = new MobileMoneyProcessor();
            ITransactionProcessor bank = new BankTransferProcessor();
            ITransactionProcessor crypto = new CryptoWalletProcessor();

            mobile.Process(t1);
            bank.Process(t2);
            crypto.Process(t3);

            savings.ApplyTransaction(t1);
            savings.ApplyTransaction(t2);
            savings.ApplyTransaction(t3);

            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("All transactions recorded.");
            Console.WriteLine($"Final balance: {savings.Balance:C}.");
        }

        public static void Main()
        {
            new FinanceApp().Run();
        }
    }
}
