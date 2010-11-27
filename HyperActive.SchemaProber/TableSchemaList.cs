using System;
using System.Collections.Generic;

namespace HyperActive.SchemaProber
{

	/// <summary>
	/// List of TableSchemas.
	/// </summary>
	public class TableSchemaList : List<TableSchema>
	{
		/// <summary>
		/// Gets a TableSchema from the list of tables in this instance by name.
		/// </summary>
		/// <param name="tableName">Name of the table to find.</param>
		/// <returns></returns>
		public TableSchema this[string tableName]
		{
			get
			{
				return Find(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
			}
		}
	}
}
