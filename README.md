# Upgrader
Upgrader keeps your database schema in sync with your source code as your development project evolves. Changes to the database schema are expressed as Upgrader Steps. Each Step that successfully have been deployed is tracked in a table. Typically you use Upgrader on application startup or in an installer.

Features
- Easy-to use object model exposing common DDL operations as methods and properties
- Database schema can be queried enabling expressions that are akward to express in SQL to be expressed as regular control-flow statements in .NET
- Regular SQL can used for data changes, complicated or vendor specific DDL operations
- Steps can be expressed in-line or in separate classes
- Constraints are automaticaly named by convention
- Steps that fail can be rolled back automatically, speeding up development cycle
- Easy to debug

## Install via NuGet
To install Upgrader, run the following command in the Package Manager Console:

```cmd
PM> Install-Package Upgrader
```

You can also view the package page on [Nuget](https://www.nuget.org/packages/Upgrader/).

## Example usage
This example will intialize Upgader with one upgrade step named "CreateCustomerTable". When PerformUpgrade is invoked, Upgrader will check if the step have been executed previously. If the step have not been executed previously before, it will be executed and its execution is tracked. Tracking of which steps that have been executed is stored in the database, in a table called "ExecutedSteps". 

```c#
// TODO: Insert real connection string here.
var connectionString = "Server=(local); Integrated Security=true; Initial Catalog=Acme";

var upgrade = new Upgrade<SqlServerDatabase>(connectionString);

var steps = new StepCollection();

steps.Add("CreateCustomerTable", db =>
{
	db.Tables.Add(
		"Customers", 
		new Column<int>("CustomerId", ColumnModifier.AutoIncrementPrimaryKey),
		new Column<string>("Name", 50)
	);
});

// TODO: Add more steps here as you develop your system.

upgrade.PerformUpgrade(steps);
```

## Table manipulation examples

```c#
// Check if a table named "Customer" exists.
if (database.Tables.Exists("Customer"))
{
	...
}

// Enumerate all table names.
database.Tables.ToList().ForEach(table => Console.WriteLine(table.TableName));

// Create a table named "Customer" with two columns using type mappings.
database.Tables.Add("Customer",
	new Column<int>("CustomerId", ColumnModifier.AutoIncrementPrimaryKey),
	new Column<string>("Name", 50));

// Create a table named "Customer" with two columns using SQL data types.
database.Tables.Add("Customer",
	new Column("CustomerId", "int", ColumnModifier.AutoIncrementPrimaryKey),
	new Column("Name", "varchar(50)"));
	
// Rename "Customer" table to "Customers".
database.Tables.Rename("Customer", "Customers");

// Remove table named "Customers".
database.Tables.Remove("Customers");
```

## Column manipulation examples
```c#
// Check if column "Name" exists in table "Customer".
if (database.Tables["Customer"].Columns.Exists("Name"))
{
	...
}

// Enumerate all columns in table "Customer".
database.Tables["Customer"].Columns.ToList().ForEach(column => Console.WriteLine(column.ColumnName));

// Add a nullable column named "Profit" to table "Customer" using type mappings.
database.Tables["Customer"].Columns.Add<decimal?>("Profit");

// Add a nullable column named "Profit" to table "Customer" using SQL data types.
database.Tables["Customer"].Columns.Add("Profit", "decimal", true);

// Add a non-nullable column named "Status" to table "Customer", set value "0" in all existing rows using type mappings.
database.Tables["Customer"].Columns.Add<int>("Status", 0);

// Add a non-nullable column named "Status" to table "Customer", set value "0" in all existing rows using SQL data types.
database.Tables["Customer"].Columns.Add("Status", "int", 0);

// Change column "Name" in table "Customer" to data type "varchar(100)" using type mappings.
database.Tables["Customer"].Columns["Name"].ChangeDataType<string>(100);

// Change column "Name" in table "Customer" to data type "varchar(100)" using SQL data types.
database.Tables["Customer"].Columns["Name"].ChangeDataType("varchar(100)");

// Rename column "Name" in table "Customer" to "CustomerName".
database.Tables["Customer"].Columns.Rename("Name", "CustomerName");

// Remove column "Profit" in table "Customer".
database.Tables["Customer"].Columns.Remove("Profit");
```

## Primary key manipulation examples
```c#
// Check if primary key exists for table "Customer".
if (database.Tables["Customer"].GetPrimaryKey() != null)
{
	...
}

// Get primary key information for "Customer" table.
database.Tables["Customer"].GetPrimaryKey().GetColumnNames().ToList().ForEach(columnName => Console.WriteLine(columnName));

// Add a primary key for table "Customer" on column "CustomerId".
database.Tables["Customer"].AddPrimaryKey("CustomerId");

// Remove primare key for table "Customer".
database.Tables["Customer"].RemovePrimaryKey();
```

## Foreign key manipulation examples
```c#
// Check if foreign key "FK_Customer_Address" exists for table "Customer".
if (database.Tables["Customer"].ForeignKeys.Exists("FK_Customer_CustomerId_Address"))
{
	...
}

// Enumerate foreign keys for table "Customer".
database.Tables["Customer"].ForeignKeys.ToList().ForEach(foreignKey => Console.WriteLine(foreignKey.GetForeignTable()));

// Add a foreign key for table "Customer" on column "CustomerId" to column "CustomerId" in foreign table "Address".
database.Tables["Customer"].ForeignKeys.Add("CustomerId", "Address");

// Remove primare key for table "Customer".
database.Tables["Customer"].ForeignKeys.Remove("FK_Customer_CustomerId_Address");
```

## Index manipulation examples
```c#
// Check if index named "IX_Customer_Profit" exists for table "Customer".
if (database.Tables["Customer"].Indexes.Exists("IX_Customer_Profit"))
{
	...
}

// Enumerate indexes for table "Customer".
database.Tables["Customer"].Indexes.ToList().ForEach(index => Console.WriteLine(index.IndexName));

// Add non-unique index on column "Profit" in table "Customer".
database.Tables["Customer"].Indexes.Add("Profit");

// Remove index "IX_Customer_Profit".
database.Tables["Customer"].Indexes.Remove("IX_Customer_Profit");
```

## Row manupilation examples
```c#
// IEnumerable<dynamic> representing all rows in the table "Customer".
var rows = database.Tables["Customer"].Rows.Query();

// Add a new row to table "Customer".
datbase.Tables["Customer"].Rows.Add(new { CustomerName = "Acme" });

// Update a row in table "Customer", primary key column is resolved by querying the database schema. 
// (UPDATE Customer SET CustomerName = 'Acme Inc.' WHERE CustomerId = 1)
datbase.Tables["Customer"].Rows.Update(new { CustomerName = "Acme Inc.", CustomerId = 1 });

// Delete a row in table "Customer", primary key column is resolved by querying the database schema. 
// (DELETE FROM Customer WHERE CustomerId = 1)
datbase.Tables["Customer"].Rows.Remove(new { CustomerId = 1 });
```

## Configration options
```c#
// Change table used for tracking executed steps to "Executed" (default is "ExecutedSteps").
upgrade.ExecutedStepsTable = "Executed";

// Change transaction mode for upgrade to "OneTransactionPerStep" (default is "None.")
upgrade.TransactionMode = TransactionMode.OneTransactionPerStep;
```

## Supported database management systems
- SQL Server
- MySql (probably MariaDb as well)
- PostgreSql
- SQLite (not all features supported because of DDL limitations in SQLite)

## Supported operations
Item | Create | Delete | Modify | Rename | Reflection
---- | ------ | ------ | ------ | ------ | ----------
Database | :white_check_mark: | :white_check_mark: | :white_check_mark: | :x: | :white_check_mark:
Table | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark:
Column | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark:
Primary key | :white_check_mark: | :white_check_mark: | :x: | :x: | :white_check_mark:
Foreign key | :white_check_mark: | :white_check_mark: | :x: | :x: | :white_check_mark:
Index | :white_check_mark: | :white_check_mark: | :x: | :x: | :white_check_mark:
