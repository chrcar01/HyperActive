using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Represents the primary key column of a table.
	/// </summary>
	public class PrimaryKeyColumnSchema : ColumnSchema
	{
		private ForeignKeyColumnSchemaList _foreignKeyReferences;
		/// <summary>
		/// Represents a list of tables where this primary key is referenced as a foreign key.
		/// </summary>
		public ForeignKeyColumnSchemaList ForeignKeyReferences
		{
			get
			{
				if (_foreignKeyReferences == null && this._dbprovider != null)
					_foreignKeyReferences = this._dbprovider.GetForeignKeyReferences(this);
				return _foreignKeyReferences;
			}
		}
		///// <summary>
		///// Initializes a new Instance of the PrimaryKeyColumnSchema class.
		///// </summary>
		//public PrimaryKeyColumnSchema()
		//{
		//	
		//}
		/// <summary>
		/// Initializes a new instance of a PrimaryKeyColumnSchema.
		/// </summary>
		/// <param name="dbprovider">The DbProvider used for interrogating the datastore.</param>
		/// <param name="table">The TableSchema that contains the column.</param>
		/// <param name="sqlType">SqlDbType.</param>
		/// <param name="dataType">.NET DataType</param>
		/// <param name="name">Name of the column.</param>
		/// <param name="props">Any extended properties.</param>
		/// <param name="length">Length of the column if supported.</param>
		public PrimaryKeyColumnSchema(IDbProvider dbprovider, TableSchema table, SqlDbType sqlType, Type dataType, string name, int length, Dictionary<string, object> props)
			: base(dbprovider, table, sqlType, dataType, name, length, props)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyColumnSchema"/> class.
		/// </summary>
		public PrimaryKeyColumnSchema()
		{
		}
	}
}
