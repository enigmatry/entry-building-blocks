# Randomness Library

A utility library that provides methods for generating random data, useful for testing and data generation scenarios.

## Intended Usage

Use this library when you need to generate random test data, create dummy content, or simulate randomized behavior in your applications.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.Randomness
```

## Usage Example

```csharp
using Enigmatry.Entry.Randomness;

// Generate a random string
string randomString = RandomGenerator.String(10);

// Generate a random integer within a range
int randomInt = RandomGenerator.Integer(1, 100);

// Generate a random date
DateTime randomDate = RandomGenerator.Date();

// Generate a random boolean
bool randomBool = RandomGenerator.Boolean();

// Generate a random item from a collection
var items = new[] { "apple", "banana", "orange" };
string randomItem = RandomGenerator.ItemFrom(items);
```
