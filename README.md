# Upgrader
Keep your databases upgraded with ease.

## Example usage
This example will intialize Upgader with one upgrade step named "CreateCustomerTable". When PerformUpgrade is invoked, Upgrader will check if the step have been executed previously. If the step have not been executed previously before, it will be executed and its execution is tracked. Tracking of which steps that have been executed is stored in the database, in a table called "ExecutedSteps".

```csharp
// TODO: Insert real connection string here.
var connectionString = "Server=(local); Integrated Security=true; Initial Catalog=Acme";

var database = new SqlServerDatabase(connectionString); 
var upgrade = new Upgrade<SqlServerDatabase>(database);

var steps = new List<Step>();

steps.Add(new Step("CreateCustomerTable", () => 
{
    database.Tables.Add("Customers", new[]
    {
        new Column("CustomerId", "INT", ColumnModifier.AutoIncrementPrimaryKey), 
        new Column("Name", "VARCHAR(50)")
    });                            
}));

// TODO: Add more steps here as you develop your system.

upgrade.PerformUpgrade(steps);
```

## Supported database management systems
- SQL Server
- MySql (probably MariaDb as well)
