using System;
using System.Data;
using System.Collections.Generic;

namespace HyperActive.SchemaProber
{
    /// <summary>
	/// Represents a field in a table.
	/// </summary>
	public class ColumnSchema : IColumnSchema
	{
		private int _length;
		/// <summary>
		/// Gets the length of the column
		/// </summary>
		public int Length
		{
			get { return _length; }
		}

		/// <summary>
		/// Gets a value indicating whether a column can contain nulls.  It's looking for the 'is_nullable'
		/// key in the props.
		/// </summary>
		public virtual bool Nullable
		{
			get
			{
				return _props !=null
					&& _props.ContainsKey("is_nullable")
					&& Convert.ToBoolean(_props["is_nullable"]);
			}
		}
		/// <summary>
		/// Gets a value indicating whether this column is an ident column.  It depends on a boolean value
		/// in the props with the key 'is_identity'.
		/// </summary>
		public virtual bool IsIdentity
		{
			get 
			{ 
				return _props != null
					&& _props.ContainsKey("is_identity")
					&& Convert.ToBoolean(_props["is_identity"]); 
			}
		}
		/// <summary>
		/// Gets a value indicating whether this column is a primary key.  It depends on a boolean value
		/// in the props with the key 'is_primary_key'.
		/// </summary>
		public virtual bool IsPrimaryKey
		{
			get 
			{ 
				return _props != null
					&& _props.ContainsKey("is_primary_key")
					&& Convert.ToBoolean(_props["is_primary_key"]); 
			}
		}
		/// <summary>
		/// Gets a value indicating whether this column is a foreign key.  It depends on a boolean value
		/// in the props with the key 'is_foreign_key'.
		/// </summary>
		public virtual bool IsForeignKey
		{
			get
			{
				return _props != null
					&& _props.ContainsKey("is_foreign_key")
					&& Convert.ToBoolean(_props["is_foreign_key"]);
			}
		}
		private SqlDbType _sqlType;
		/// <summary>
		/// Gets the sql datatype for this column.
		/// </summary>
		public virtual SqlDbType SqlType
		{
			get
			{
				return _sqlType;
			}
		}
		private Type _dataType;
		/// <summary>
		/// Gets the .NET datatype for this column.
		/// </summary>
		public virtual Type DataType
		{
			get
			{
				return _dataType;
			}
		}
		private string _name;
		/// <summary>
		/// gets the name of the column.
		/// </summary>
		public virtual string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Instance of the IDbProvider used to get information from the database.
		/// </summary>
		protected IDbProvider _dbprovider = null;

		/// <summary>
		/// A Dictionary&lt;string,object&gt;
		/// containing extra properties for this column.
		/// </summary>
		protected Dictionary<string, object> _props = null;

		/// <summary>
		/// Initializes a new instance of the ColumnSchema class.
		/// </summary>
		public ColumnSchema()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ColumnSchema
		/// </summary>
		/// <param name="dbprovider">The DbProvider used for interrogating the datastore.</param>
		/// <param name="table">The TableSchema that contains the column.</param>
		/// <param name="sqlType">SqlDbType.</param>
		/// <param name="dataType">.NET DataType</param>
		/// <param name="name">Name of the column.</param>
		/// <param name="props">Any extended properties.</param>
		/// <param name="length">Length of the column if supported.</param>
		public ColumnSchema(IDbProvider dbprovider, TableSchema table, SqlDbType sqlType, Type dataType, string name, int length, Dictionary<string, object> props)
		{
			_table = table;
			_props = props;
			_sqlType = sqlType;
			_dataType = dataType;
			_name = name;
			_length = length;
			_dbprovider = dbprovider;
		}
		private TableSchema _table = null;
		/// <summary>
		/// Gets the TableSchema containing this column.
		/// </summary>
		public virtual TableSchema Table
		{
			get
			{
				return _table;
			}
		}
	}
}
