using System;
using System.Collections.Generic;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Represents a view of a database.
	/// </summary>
	public class DatabaseSchema
	{
		private IDbProvider _dbprovider = null;

        /// <summary>
		/// Initializes a new instance of a DatabaseSchema.
		/// </summary>
		/// <param name="dbprovider"></param>
		public DatabaseSchema(IDbProvider dbprovider)
		{
			if (dbprovider == null)
				throw new ArgumentNullException("dbprovider");

			_dbprovider = dbprovider;
		}

		private TableSchemaList _tables = null;
		/// <summary>
		/// Gets the list of tables in the database.
		/// </summary>
		public virtual TableSchemaList Tables
		{
			get
			{
				if (_tables == null)
					_tables = _dbprovider.GetTableSchemas();
				return _tables;
			}
		}
		/// <summary>
		/// Retrieves all of the tables whose names begin with the specified prefix.
		/// </summary>
		/// <param name="prefix">Prefix of the tables to find.</param>
		/// <returns>List of tables whose names begin with the specified prefix.</returns>
		public TableSchemaList GetTables(string prefix)
		{
			return _dbprovider.GetTableSchemas(prefix);
		}
	}
}
