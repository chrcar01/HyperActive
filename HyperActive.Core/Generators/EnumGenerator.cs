using System;
using System.IO;
using HyperActive.SchemaProber;
using HyperActive.Dominator;
using HyperActive.Core.Config;
using System.CodeDom;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// Responsible for generating enum types.
	/// </summary>
	public class EnumGenerator
	{
		private IDbProvider _dbProvider = null;
		private IConfigurationOptions _configOptions = null;
		private NameProvider _nameProvider = null;

        public EnumGenerator(IDbProvider dbProvider, IConfigurationOptions configOptions, NameProvider nameProvider)
		{
			_dbProvider = dbProvider;
			_configOptions = configOptions;
			_nameProvider = nameProvider;
		}

		private string GetValueField(string valueField, TableSchema tableSchema)
		{
			if (tableSchema == null || tableSchema.Columns == null || tableSchema.Columns.Count == 0)
			{
				return String.Empty;
			}

			string internalValueField = valueField;
			if (String.IsNullOrEmpty(internalValueField)
				|| tableSchema.Columns[internalValueField] == null
				|| tableSchema.Columns[internalValueField].DataType != typeof(int))
			{
				if (tableSchema.Columns["ID"] != null && tableSchema.Columns["ID"].DataType == typeof(int))
				{
					internalValueField = "ID";
				}

			}
			return internalValueField;
		}
		private string GetNameField(string nameField, TableSchema tableSchema)
		{
			if (tableSchema == null || tableSchema.Columns == null || tableSchema.Columns.Count == 0)
			{
				return String.Empty;
			}

			string internalNameField = nameField;
			if (String.IsNullOrEmpty(internalNameField) || tableSchema.Columns[nameField] == null)
			{
				if (tableSchema.Columns["Label"] != null)
				{
					internalNameField = "Label";
				}
				else if (tableSchema.Columns["Name"] != null)
				{
					internalNameField = "Name";
				}
			}
			return internalNameField;
		}

		/// <summary>
		/// Generates the specified writer.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="nameField">The name field.</param>
		/// <param name="valueField">The value field.</param>
		public virtual ICodeDom<CodeTypeDeclaration> Generate(string tableName, string nameField, string valueField)
		{
			if (String.IsNullOrEmpty(tableName))
			{
				throw new ArgumentException("tableName is null or empty.", "tableName");
			}

			var tableSchema = _dbProvider.GetTableSchema(tableName);
			if (tableSchema == null)
			{
				throw new HyperActiveException("Table {0} was not found in the database.  Check your spelling and make sure that's the table you wanted.", tableName);
			}
			string internalNameField = GetNameField(nameField, tableSchema);
			if (String.IsNullOrEmpty(internalNameField))
			{
				return null;
			}

			string internalValueField = GetValueField(valueField, tableSchema);
			if (String.IsNullOrEmpty(internalValueField))
			{
				return null;
			}

			var data = _dbProvider.GetTableEnumData(tableName, internalNameField, internalValueField);
			if (data.Count == 0)
			{
				return null;
			}
			var @enum = new EnumDeclaration(_nameProvider.GetEnumName(tableName));
			foreach (var kvp in data)
			{
				var field = @enum.AddField(_nameProvider.GetEnumFieldName(kvp.Key), kvp.Value);
				field.AddComment("{0} has the value of {1}", field.Name, kvp.Value);
			}
			return @enum;
		}
	}
}
