using System;
using System.Collections.Generic;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Common interface for extracting information from a database.
	/// </summary>
	public interface IDbProvider
	{
		/// <summary>
		/// Gets the connectionstring the implementing instance is using.
		/// </summary>
		string ConnectionString { get; }
		/// <summary>
		/// Gets or sets the ID of an instance of a class implementing IDbProvider.
		/// </summary>
		string ID { get;set; }
		/// <summary>
		/// Gets a list of tables in a database.
		/// </summary>
		/// <returns>A list of tables from a database.</returns>
		TableSchemaList GetTableSchemas();


		/// <summary>
		/// Gets the associations.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <returns></returns>
		TableSchemaList GetAssociations(TableSchema table);

		/// <summary>
		/// Gets a TableSchema instance from the database by name.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>An instance of a TableSchema representing the database table.</returns>
		TableSchema GetTableSchema(string tableName);
		/// <summary>
		/// Gets a list of columns for a table.
		/// </summary>
		/// <param name="table">The TableSchema instance to retrieve columns.</param>
		/// <returns>A list of columns in the TableSchema instance.</returns>
		ColumnSchemaList GetColumnSchemas(TableSchema table);
		/// <summary>
		/// Gets a list of foreign keys contained in a table.
		/// </summary>
		/// <param name="table">A TableSchema instance representing a database table.</param>
		/// <returns>A list of foreign key columns contained within a database table.</returns>
		ForeignKeyColumnSchemaList GetForeignKeys(TableSchema table);
		/// <summary>
		/// Gets the tables that reference a primary key.
		/// </summary>
		/// <param name="primaryKey">A primary key.</param>
		/// <returns>A list of foreign key columns that reference the primary key.</returns>
		ForeignKeyColumnSchemaList GetForeignKeyReferences(PrimaryKeyColumnSchema primaryKey);
		/// <summary>
		/// Gets the primary key for a table.
		/// </summary>
		/// <param name="table">A TableSchema instance that represents a database table.</param>
		/// <returns>The primary key column of a table.</returns>
		PrimaryKeyColumnSchema GetPrimaryKey(TableSchema table);
		/// <summary>
		/// Gets a list of tables whose names begin with the specified prefix.
		/// </summary>
		/// <param name="prefix">The prefix of the tables to find.</param>
		/// <returns>List of tables whose names begin with the specified prefix.</returns>
		TableSchemaList GetTableSchemas(string prefix);
		/// <summary>
		/// Gets the table enum data.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="nameField">The field in the table to use for the field names in an enum.</param>
		/// <param name="valueField">The field in the table to use for the field value in an enum.</param>
		/// <returns></returns>
		IDictionary<string, int> GetTableEnumData(string tableName, string nameField, string valueField);
	}
}