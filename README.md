# Upgrader
Upgrader keeps your database schema in sync with your code as your development project evolves. As new features are added in a development project, changes to the database schema are expressed as Upgrader Steps. Each Step that successfully have been deployed is tracked in a table.

Features
- Easy object model exposing most common DDL operations
- Schema can be reflected
- Dapper can used for complicated or non supported vendor specific DDL operations
- Constraints are automaticaly named by convention
- Easy to debug error schema changes

## Install via NuGet
To install Upgrader, run the following command in the Package Manager Console:

```
PM> Install-Package Upgrader
```

You can also view the package page on [Nuget](https://www.nuget.org/packages/Upgrader/).

## Example usage
This example will intialize Upgader with one upgrade step named "CreateCustomerTable". When PerformUpgrade is invoked, Upgrader will check if the step have been executed previously. If the step have not been executed previously before, it will be executed and its execution is tracked. Tracking of which steps that have been executed is stored in the database, in a table called "ExecutedSteps". Typically you use Upgrader on application startup or in an installer.

```csharp
// TODO: Insert real connection string here.
var connectionString = "Server=(local); Integrated Security=true; Initial Catalog=Acme";

using (var database = new SqlServerDatabase(connectionString))
{ 
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
}
```

## Example with reflection
The library have support for reflecting on the database schema. This can be used for update steps where the target database may have subtle differences from installation to installation. You can also use this to enforce schema conventions.

```csharp
[TestMethod]
public void AllForeignKeysAreNamedWithFK_Prefix()
{
    // TODO: Insert real connection string here.
    var connectionString = "Server=(local); Integrated Security=true; Initial Catalog=Acme";
    
    var database = new SqlServerDatabase(connectionString);

    var allForeignKeys = database.Tables.SelectMany(table => table.ForeignKeys);

    foreach (var foreignKey in allForeignKeys)
    {
        var foreignKeyStartsWithFK_ = foreignKey.ForeignKeyName.StartsWith("FK_");

        Assert.IsTrue(foreignKeyStartsWithFK_, $"Foreign key {foreignKey.ForeignKeyName} on table {foreignKey.TableName} does not start with \"FK_\".");
    }
}
```


## Supported database management systems
- SQL Server
- MySql (probably MariaDb as well)



## Supported operations
Item | Create | Delete | Modify | Rename | Reflection
---- | ------ | ------ | ------ | ------ | ----------
Table | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark:
Column | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark:
Primary key | :white_check_mark: | :white_check_mark: | :x: | :x: | :white_check_mark:
Foreign key | :white_check_mark: | :white_check_mark: | :x: | :x: | :white_check_mark:
Index | :white_check_mark: | :white_check_mark: | :x: | :x: | :white_check_mark:
