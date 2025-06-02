# Smart Enums

A library that extends the traditional C# enum concept with rich, type-safe enumeration classes that can contain both data and behavior.

## Intended Usage

Use Smart Enums when you need enumerations that contain additional data and behavior beyond simple named constants. Smart Enums provide type-safety, better refactoring support, and the ability to encapsulate logic related to the enumeration.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.SmartEnums
```

## Usage Example

Define a Smart Enum:

```csharp
using Enigmatry.Entry.SmartEnums;

public class PaymentMethod : SmartEnum<PaymentMethod>
{
    public static readonly PaymentMethod CreditCard = new(1, nameof(CreditCard), "Credit Card", 0.02m);
    public static readonly PaymentMethod DebitCard = new(2, nameof(DebitCard), "Debit Card", 0.01m);
    public static readonly PaymentMethod BankTransfer = new(3, nameof(BankTransfer), "Bank Transfer", 0.00m);

    public string DisplayName { get; }
    public decimal TransactionFee { get; }

    private PaymentMethod(int id, string name, string displayName, decimal transactionFee) 
        : base(id, name)
    {
        DisplayName = displayName;
        TransactionFee = transactionFee;
    }

    public decimal CalculateFee(decimal amount) => amount * TransactionFee;
}
```

Using the Smart Enum:

```csharp
// Get by name
var creditCard = PaymentMethod.FromName("CreditCard");

// Get by ID
var bankTransfer = PaymentMethod.FromId(3);

// Use the properties and methods
decimal fee = creditCard.CalculateFee(100.00m);

// List all values
var allMethods = PaymentMethod.List();

// Equality comparison works correctly
bool isSame = (PaymentMethod.CreditCard == creditCard); // true
```
