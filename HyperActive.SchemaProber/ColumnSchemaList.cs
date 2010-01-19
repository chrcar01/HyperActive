using System;
using System.Collections.Generic;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// List of ColumnSchemas.
	/// </summary>
	public class ColumnSchemaList : List<IColumnSchema>
	{
		/// <summary>
		/// Gets a ColumnSchema by name from the list of ColumnSchemas.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public IColumnSchema this[string columnName]
		{
			get
			{
				return this.Find(delegate(IColumnSchema col) { return col.Name == columnName; });
			}
		}
		/// <summary>
		/// Initializes a new instance of the ColumnSchemaList
		/// </summary>
		public ColumnSchemaList()
		{
		}
		/// <summary>
		/// Initializes a new instance of the ColumnSchemaList
		/// </summary>
		/// <param name="collection"></param>
		public ColumnSchemaList(IEnumerable<IColumnSchema> collection)
			: base(collection)
		{
			
		}
	}
}
