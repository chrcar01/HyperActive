using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// SqlServer implementation of the IDbProvider interface.
	/// </summary>
	public class SqlServerProvider : IDbProvider
	{
		private IDbHelper _helper = null;
		private string _id;
		/// <summary>
		/// Gets or sets the ID of this instance of the SqlServerProvider class.
		/// </summary>
		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Gets the connectionstring the implementing instance is using.
		/// </summary>
		/// <value></value>
		public string ConnectionString
		{
			get
			{
				return _helper.ConnectionString;
			}
		}
		/// <summary>
		/// Initializes a new instance of the SqlServerProvider
		/// </summary>
		/// <param name="helper"></param>
		public SqlServerProvider(IDbHelper helper)
		{
			_helper = helper;
		}
		/// <summary>
		/// Gets all of the columns for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public virtual ColumnSchemaList GetColumnSchemas(TableSchema table)
		{
			string tableName = table.Name;
			ColumnSchemaList result = null;
			string cmdtext = @"
select distinct
	case when cc.definition is null then 0 else 1 end as is_computed,
	case when cc.definition is null then 0 else 1 end as formula,
	isnull(columnproperty(object_id(c.table_name), c.column_name, 'IsIdentity'),0) as is_identity,
	case when (select tc.constraint_type from information_schema.table_constraints tc where tc.table_name = kcu.table_name and kcu.constraint_name = tc.constraint_name and tc.constraint_type = 'PRIMARY KEY') is null then 0 else 1 end as 'is_primary_key',
	case when (select tc.constraint_type from information_schema.table_constraints tc where tc.table_name = kcu.table_name and kcu.constraint_name = tc.constraint_name and tc.constraint_type = 'FOREIGN KEY') is null then 0 else 1 end as 'is_foreign_key',
	c.*
from information_schema.columns c
left join information_schema.key_column_usage kcu
	on c.table_name = kcu.table_name
	and c.column_name = kcu.column_name
left join sys.computed_columns cc
	on cc.[object_id] = object_id(@table_name)
	and cc.[name] = c.column_name
where c.table_name = @table_name
";
			try
			{
				using (SqlCommand cmd = new SqlCommand(cmdtext))
				{
					cmd.Parameters.Add("@table_name", SqlDbType.NVarChar, 128).Value = tableName;
					using (IDataReader reader = _helper.ExecuteReader(cmd))
					{
						while (reader.Read())
						{
							if (result == null) result = new ColumnSchemaList();
							SqlDbType sqlType = getSqlDbType(reader["DATA_TYPE"].ToString());
							Type dataType = getDataType(reader["DATA_TYPE"].ToString());
							string name = reader["COLUMN_NAME"].ToString();
							int length = reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? System.Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]) : -1;
							if (length < 0 && dataType == typeof(string))
								length = 8000; //patch for varchar(max), not sure if this works in all situations.

							Dictionary<string, object> props = new Dictionary<string, object>();
							props.Add("is_identity", Convert.ToBoolean(reader["is_identity"]));
							props.Add("is_primary_key", Convert.ToBoolean(reader["is_primary_key"]));
							props.Add("is_foreign_key", Convert.ToBoolean(reader["is_foreign_key"]));
							props.Add("is_nullable", (reader["is_nullable"].ToString().Equals("YES")));
							props.Add("is_computed", Convert.ToBoolean(reader["is_computed"]));
							props.Add("formula", (reader["formula"] ?? "").ToString());
							result.Add(new ColumnSchema(this, table, sqlType, dataType, name, length, props));
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new SchemaProberException(String.Format("An error occurred while processing the table '{0}'", tableName), ex);
			}

			return result;
		}

		private SqlDbType getSqlDbType(string value)
		{
			SqlDbType result;
			if (value.Equals("numeric", StringComparison.OrdinalIgnoreCase))
				result = SqlDbType.Int;
			else
				result = Enum<SqlDbType>.Parse(value);
			return result;
		}
		private Type getDataType(string sqlType)
		{
			switch (sqlType.ToLower())
			{
				case "ntext":
				case "char":
				case "nvarchar":
				case "text":
				case "nchar":
				case "xml":
				case "varchar": return typeof(string);
				case "tinyint":
				case "smallint":
				case "int": return (typeof(int));
				case "numeric": return (typeof(int));
				case "bigint": return (typeof(long));
				case "bit": return (typeof(bool));
				case "timestamp":
				case "smalldatetime":
				case "date":
				case "datetime2":
				case "datetime": return (typeof(DateTime));
				case "money":
				case "smallmoney":
				case "real":
				case "float":
				case "decimal": return (typeof(decimal));
				case "uniqueidentifier": return (typeof(System.Guid));
				case "varbinary":
				case "binary":
				case "image": return typeof(byte[]);
			}
			throw new NotSupportedException(String.Format("Type '{0}' is not currently supported", sqlType));
		}


		/// <summary>
		/// Returns a new instance of a tableschema.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual TableSchema GetTableSchema(string tableName)
		{
			return new TableSchema(this, tableName);
		}
		/// <summary>
		/// Gets all of the tables in a database.
		/// </summary>
		/// <returns></returns>
		public virtual TableSchemaList GetTableSchemas()
		{			
			TableSchemaList result = null;
			string cmdtext = @"select * from information_schema.tables where table_name not like 'sys%'";
			using (IDataReader reader = this._helper.ExecuteReader(cmdtext))
			{
				while (reader.Read())
				{
					if (result == null) result = new TableSchemaList();
					result.Add(new TableSchema(this, reader["TABLE_NAME"].ToString()));
				}
			}
			return result;
		}

		/// <summary>
		/// Gets a list of tables whose names begin with the specified prefix.
		/// </summary>
		/// <param name="prefix">The prefix of the tables to find.</param>
		/// <returns>List of tables whose names begin with the specified prefix.</returns>
		public TableSchemaList GetTableSchemas(string prefix)
		{
			TableSchemaList result = null;
			string cmdtext = String.Format(@"select table_name from information_schema.tables where table_name like '{0}%' ", prefix);
			using (IDataReader reader = this._helper.ExecuteReader(cmdtext))
			{
				while (reader.Read())
				{
					if (result == null) result = new TableSchemaList();
					result.Add(new TableSchema(this, reader["TABLE_NAME"].ToString()));
				}
			}
			return result;
		}

		/// <summary>
		/// Gets the primary key for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public virtual PrimaryKeyColumnSchema GetPrimaryKey(TableSchema table)
		{
			PrimaryKeyColumnSchema result = null;
			if (table == null)
				return result;

			string cmdtext = @"
select 
	columnproperty(object_id(c.table_name), c.column_name, 'IsIdentity') as is_identity,
	c.*
from information_schema.table_constraints tc
inner join information_schema.key_column_usage kcu
	on kcu.constraint_name = tc.constraint_name
	and kcu.table_name = tc.table_name
inner join information_schema.columns c
	on c.table_name = kcu.table_name
	and c.column_name = kcu.column_name
where tc.constraint_type = 'PRIMARY KEY'
and c.table_name = @table_name
";
			using (SqlCommand cmd = new SqlCommand(cmdtext))
			{
				cmd.Parameters.Add("@table_name", SqlDbType.NVarChar, 128).Value = table.Name;
				using (IDataReader reader = this._helper.ExecuteReader(cmd))
				{
					if (reader.Read())
					{
						SqlDbType sqlType = Enum<SqlDbType>.Parse(reader["DATA_TYPE"].ToString());
						Type dataType = getDataType(reader["DATA_TYPE"].ToString());
						string name = reader["COLUMN_NAME"].ToString();
						Dictionary<string, object> props = new Dictionary<string, object>();
						props.Add("is_identity", Convert.ToBoolean(reader["is_identity"]));
						int length = reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? System.Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]) : -1;
						props.Add("is_primary_key", true);
						props.Add("is_foreign_key", false);
						props.Add("is_nullable", (reader["is_nullable"].ToString().Equals("YES")));
						result = new PrimaryKeyColumnSchema(this, table, sqlType, dataType, name, length, props);
					}
				}
				
			}
			return result;
		}

		/// <summary>
		/// Gets all of the objects representing tables that have references to a primary key.
		/// </summary>
		/// <param name="primaryKey"></param>
		/// <returns></returns>
		public virtual ForeignKeyColumnSchemaList GetForeignKeyReferences(PrimaryKeyColumnSchema primaryKey)
		{
			ForeignKeyColumnSchemaList result = new ForeignKeyColumnSchemaList();
			string cmdtext = @"
select distinct
	col_name(fc.parent_object_id, fc.parent_column_id) as foreign_key_column,
	object_name(fc.parent_object_id) as foreign_key_table,
	col_name(fc.referenced_object_id, fc.referenced_column_id) as primary_key_column,
	object_name(fc.referenced_object_id) as primary_key_table,
	c.data_type,
	c.character_maximum_length,
	c.is_nullable,
	c.table_name,
	c.column_name
from sys.foreign_keys as f
inner join sys.foreign_key_columns as fc
	on f.object_id = fc.constraint_object_id
inner join information_schema.columns c
	on c.column_name = col_name(fc.parent_object_id, fc.parent_column_id)
	and c.table_name = object_name(f.parent_object_id)
where object_name (f.referenced_object_id) =  @table_name
";
			using (SqlCommand cmd = new SqlCommand(cmdtext))
			{
				cmd.Parameters.Add("@table_name", SqlDbType.NVarChar, 128).Value = primaryKey.Table.Name;
				using (IDataReader reader = this._helper.ExecuteReader(cmd))
				{
					while (reader.Read())
					{
						string tableName = reader["TABLE_NAME"].ToString();
						SqlDbType sqlType = Enum<SqlDbType>.Parse(reader["DATA_TYPE"].ToString());
						Type dataType = getDataType(reader["DATA_TYPE"].ToString());
						string name = reader["COLUMN_NAME"].ToString();
						Dictionary<string, object> props = new Dictionary<string, object>();
						props.Add("is_identity", false);
						int length = reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? System.Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]) : -1;
						props.Add("is_primary_key", false);
						props.Add("is_foreign_key", true);
						props.Add("primary_key_table", reader["primary_key_table"]);
						props.Add("primary_key_column", reader["primary_key_column"]);
						props.Add("foreign_key_column", reader["foreign_key_column"]);
						props.Add("foreign_key_table", reader["foreign_key_table"]);
						props.Add("is_nullable", (reader["is_nullable"].ToString().Equals("YES")));
						result.Add(new ForeignKeyColumnSchema(this, new TableSchema(this, tableName), sqlType, dataType, name, length, props));
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Gets all of the foreign keys within a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public virtual ForeignKeyColumnSchemaList GetForeignKeys(TableSchema table)
		{
			ForeignKeyColumnSchemaList result = new ForeignKeyColumnSchemaList();

			string cmdtext = @"
select distinct
	col_name(fc.parent_object_id, fc.parent_column_id) as column_name,
	c.data_type,
	object_name (f.referenced_object_id) as primary_key_table,
	c.*
from sys.foreign_keys as f
inner join sys.foreign_key_columns as fc
	on f.object_id = fc.constraint_object_id
inner join information_schema.columns c
	on c.column_name = col_name(fc.parent_object_id, fc.parent_column_id)
	and c.table_name = object_name(f.parent_object_id)
where object_name(f.parent_object_id) = @table_name
";
			using (SqlCommand cmd = new SqlCommand(cmdtext))
			{
				cmd.Parameters.Add("@table_name", SqlDbType.NVarChar, 128).Value = table.Name;
				using (IDataReader reader = this._helper.ExecuteReader(cmd))
				{
					while (reader.Read())
					{
						string columnName = reader["column_name"].ToString();
						SqlDbType sqldatatype = Enum<SqlDbType>.Parse(reader["data_type"].ToString());
						Type datatype = getDataType(reader["data_type"].ToString());
						Dictionary<string, object> props = new Dictionary<string, object>();
						props.Add("primary_key_table", reader["primary_key_table"].ToString());
						int length = reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? System.Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]) : -1;
						ForeignKeyColumnSchema column = new ForeignKeyColumnSchema(this, table, sqldatatype, datatype, columnName, length, props);
						result.Add(column);
					}
				}
			}
			return result;
		}



		/// <summary>
		/// Gets the table enum data.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="nameField">The field in the table to use for the field names in an enum.</param>
		/// <param name="valueField">The field in the table to use for the field value in an enum.</param>
		/// <returns></returns>
		public IDictionary<string, int> GetTableEnumData(string tableName, string nameField, string valueField)
		{
			#region method contract
			if (String.IsNullOrEmpty(tableName))
				throw new ArgumentException("tableName is null or empty.", "tableName");
			if (String.IsNullOrEmpty(nameField))
				throw new ArgumentException("nameField is null or empty.", "nameField");
			if (String.IsNullOrEmpty(valueField))
				throw new ArgumentException("valueField is null or empty.", "valueField");
			#endregion
			
			string sql = String.Format(@"select {0}, {1} from {2}", nameField, valueField, tableName);
			var result = new Dictionary<string, int>();
			using (IDataReader reader = this._helper.ExecuteReader(sql))
			{
				while (reader.Read())
				{
					result.Add(reader.GetString(0), reader.GetInt32(1));
				}
			}
			return result;
		}


		/// <summary>
		/// Gets the associations.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <returns></returns>
		public virtual TableSchemaList GetAssociations(TableSchema table)
		{
			TableSchemaList result = new TableSchemaList();
			
			//find all tables that have 2 foreign keys, only 2 columns, and NO primary key
			var assocTables = from t in this.GetTableSchemas()
							  where t.PrimaryKey == null
							  && t.Columns.Count == 2
							  && t.ForeignKeys.Count == 2
							  select t;

			foreach (var tbl in assocTables)
			{
				if (tbl.ForeignKeys[0].PrimaryKeyTable.Name == table.Name
					|| tbl.ForeignKeys[1].PrimaryKeyTable.Name == table.Name)
				{
					result.Add(tbl);
				}
			}

			return result;
		}
	}
}
