using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace HyperActive.SchemaProber
{

	/// <summary>
	/// Represents a foreign key column.
	/// </summary>
	public class ForeignKeyColumnSchema : ColumnSchema, IForeignKeyColumnSchema
	{
		private TableSchema _primaryKeyTable = null;
		/// <summary>
		/// Gets a reference to the PrimaryKeyTable for this foreign key.
		/// </summary>
		public virtual TableSchema PrimaryKeyTable
		{
			get
			{
				if (_primaryKeyTable == null)
				{
					if (!_props.ContainsKey("primary_key_table"))
					{
						throw new KeyNotFoundException("primary_key_table was not found in the properties for this ForeignKeyColumn");
					}
					_primaryKeyTable = this._dbprovider.GetTableSchema(_props["primary_key_table"].ToString());
				}
				return _primaryKeyTable;
			}
		}

		/// <summary>
		/// Initializes a new instance of the ForeignKeyColumnSchema class.
		/// </summary>
		/// <param name="dbprovider">The DbProvider used for interrogating the datastore.</param>
		/// <param name="table">The TableSchema that contains the column.</param>
		/// <param name="sqlType">SqlDbType.</param>
		/// <param name="dataType">.NET DataType</param>
		/// <param name="name">Name of the column.</param>
		/// <param name="props">Any extended properties.</param>
		/// <param name="length">Length of the column if supported.</param>
		public ForeignKeyColumnSchema(IDbProvider dbprovider, TableSchema table, SqlDbType sqlType, Type dataType, string name, int length, Dictionary<string, object> props)
			: base(dbprovider, table, sqlType, dataType, name, length, props)
		{
		}
		///// <summary>
		///// Initializes a new instance of the ForeignKeyColumnSchema class.
		///// </summary>
		//public ForeignKeyColumnSchema()
		//{
		//}
	}
}
