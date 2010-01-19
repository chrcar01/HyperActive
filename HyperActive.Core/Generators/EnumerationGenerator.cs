using System;
using System.IO;
using HyperActive.SchemaProber;
using HyperActive.Dominator;
using HyperActive.Core.Config;
using System.CodeDom;

namespace HyperActive.Core.Generators
{
	public class EnumerationGenerator
	{
		protected IDbProvider _dbProvider = null;
		protected IConfigurationOptions _configOptions = null;
		protected NameProvider _nameProvider = null;

		public EnumerationGenerator(IDbProvider dbProvider, IConfigurationOptions configOptions, NameProvider nameProvider)
		{
			_dbProvider = dbProvider;
			_configOptions = configOptions;
			_nameProvider = nameProvider;
		}

		protected string GetValueField(string valueField, TableSchema tableSchema)
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
		protected string GetNameField(string nameField, TableSchema tableSchema)
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
			var @enum = new ClassDeclaration(_nameProvider.GetEnumName(tableName)).InheritsFrom(typeof(Enumeration).FullName);
			var fieldType = new CodeTypeReference(typeof(EnumerationField));
			foreach (var kvp in data)
			{
				var f1 = new CodeMemberField(fieldType, "_" + _nameProvider.GetEnumFieldName(kvp.Key));
				f1.Attributes = MemberAttributes.Private | MemberAttributes.Static;
				f1.InitExpression = 
					new CodeObjectCreateExpression(fieldType, 
						new CodeSnippetExpression(kvp.Value.ToString()),
						new CodeSnippetExpression("\"" + _nameProvider.GetEnumFieldName(kvp.Key) + "\""));
				@enum.Members.Add(f1);

				var p1 = new CodeMemberProperty();
				p1.Type = fieldType;
				p1.Name = _nameProvider.GetEnumFieldName(kvp.Key);
				p1.Attributes = MemberAttributes.Public | MemberAttributes.Static;
				p1.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, "_" + _nameProvider.GetEnumFieldName(kvp.Key))));
				@enum.Members.Add(p1);

			}
			return @enum;
		}
	}
}
