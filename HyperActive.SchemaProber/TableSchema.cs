using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Represents a database table.
	/// </summary>
	public class TableSchema
	{
		private IDbProvider _dbProvider = null;
		/// <summary>
		/// Gets the IDbProvider instance.
		/// </summary>
		public IDbProvider Provider
		{
			get
			{
				if (_dbProvider == null)
					throw new NullReferenceException("_dbprovider cannot be null");

				return _dbProvider;
			}
		}

		private string _name;
		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		public virtual string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Initializes a new instance of the TableSchema class.
		/// </summary>
		public TableSchema()
		{
		}

        /// <summary>
		/// Initializes a new instance of the TableSchema class.
		/// </summary>
		/// <param name="name">Name of the table.</param>
		/// <param name="dbprovider">Instance of the dbprovider used to obtain information about the table.</param>
		public TableSchema(IDbProvider dbprovider, string name)
		{
			_name = name;
			_dbProvider = dbprovider;
		}
		
		private ColumnSchemaList _columns = null;
		/// <summary>
		/// Gets all of the columns for the table.
		/// </summary>
		public virtual ColumnSchemaList Columns
		{
			get
			{
				if (_columns == null)
					_columns = Provider.GetColumnSchemas(this);
				return _columns;
			}
		}
		private PrimaryKeyColumnSchema _pk;
		/// <summary>
		/// Gets the primary key for the table.
		/// </summary>
		public virtual PrimaryKeyColumnSchema PrimaryKey
		{
			get
			{
				if (_pk == null)
					_pk = this.Provider.GetPrimaryKey(this);
				return _pk;
			}
		}
		private ForeignKeyColumnSchemaList _fkcolumns = null;
		/// <summary>
		/// Gets all of the foreign keys contained in this table.
		/// </summary>
		public virtual ForeignKeyColumnSchemaList ForeignKeys
		{
			get
			{
				if (_fkcolumns == null)
					_fkcolumns = this.Provider.GetForeignKeys(this);
				return _fkcolumns;
			}
		}
		/// <summary>
		/// Gets all colunms that are not foreign keys and not primary keys.
		/// </summary>
		public virtual ColumnSchemaList NonKeyColumns
		{
			get
			{
				if (this.Columns == null || this.Columns.Count == 0)
					return null;

				return new ColumnSchemaList(Columns.FindAll(delegate(IColumnSchema c) { return !(c.IsForeignKey || c.IsPrimaryKey); }));
			}
		}
		/// <summary>
		/// Gets all columns except for the primary key.
		/// </summary>
		public virtual ColumnSchemaList NonPrimaryKeyColumns
		{
			get
			{
				if (this.Columns== null || this.Columns.Count == 0)
					return null;

				return new ColumnSchemaList(Columns.FindAll(delegate(IColumnSchema c){ return !c.IsPrimaryKey; }));
			}
		}


		/// <summary>
		/// Determines whether the specified column is in the table.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>
		/// 	<c>true</c> if the specified column name is found in this table; otherwise, <c>false</c>.
		/// </returns>
		public bool HasColumn(string columnName)
		{
			if (String.IsNullOrEmpty(columnName))
				return false;
			
			if (this.Columns == null || this.Columns.Count == 0)
				return false;

			return this.Columns.Find(c => { return c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase); }) != null;
		}

		private TableSchemaList _associations = null;
		/// <summary>
		/// Gets the associations.
		/// </summary>
		/// <value>The associations.</value>
		public virtual TableSchemaList Associations
		{
			get
			{
				if (_associations == null)
				{
					_associations = _dbProvider.GetAssociations(this);
				}
				return _associations;
			}
		}

		
	}
}
