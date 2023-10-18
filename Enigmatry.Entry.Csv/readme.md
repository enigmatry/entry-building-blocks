# Entry Csv Building Block

Building Block for adding a Csv Helper to an Entry based project.

## Examples

Export supplier objects to a byte[], which can be saved as a csv file:

```cs
    var csvHelper = new CsvHelper<Supplier>();
    var fileContent = csvHelper.WriteRecords(listOfSuppliers, CultureInfo.CurrentCulture);
```

Import csv file into an enumerable of objects:

```cs
    var csvHelper = new CsvHelper<Supplier>();
    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
    var records = csv.GetRecords(fs, CultureInfo.CurrentCulture);
```

## Attributes

This building block also offers 2 (optional) attributes. 1 for specifying that a property should be ignored, and 1 for specifying a different name for a property.

```cs
    public class Item
    {
        [Ignore]
        public Guid Id { get; set; }
        [Name("Omschrijving")]
        public string Description { get; set; } = String.Empty;
    }
```