using System;
using HyperActive.SchemaProber;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace HyperActive.Core
{
	/// <summary>
	/// Used by ActiveRecordGenerators when naming class members.
	/// </summary>
	public class NameProvider
	{
		private Dictionary<string, string> _enumReplacements;
		/// <summary>
		/// Gets or sets the enum replacements.
		/// </summary>
		/// <value>The enum replacements.</value>
		public Dictionary<string, string> EnumReplacements
		{
			get
			{
				if (_enumReplacements == null)
				{
					_enumReplacements = new Dictionary<string, string>();
				}
				return _enumReplacements;
			}
			set
			{
				_enumReplacements = value;
			}
		}
		private List<string> _tablePrefixes;
		/// <summary>
		/// Gets the table prefixes.
		/// </summary>
		/// <value>The table prefixes.</value>
		public List<string> TablePrefixes
		{
			get
			{
				if (_tablePrefixes == null)
				{
					_tablePrefixes = new List<string>();
				}
				return _tablePrefixes;
			}
		}

		private bool _removePrefixes = true;
		/// <summary>
		/// Gets or sets a value indicating whether remove table prefixes from
		/// any method generating table names.  The default value is true.
		/// </summary>
		/// <value><c>true</c> if [true] remove prefixes; otherwise, leave prefixes.</value>
		protected bool RemovePrefixes
		{
			get
			{
				return _removePrefixes;
			}
			set
			{
				_removePrefixes = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NameProvider"></see> class.  
		/// </summary>
		public NameProvider()
			: this(null)
		{
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="NameProvider"></see> class.  A list of table 
		/// prefixes.  If provided, by default then they will be removed any time the name of a table 
		/// needs to be converted to a property or classname.
		/// </summary>
		/// <param name="tablePrefixes">The table prefixes.</param>
		public NameProvider(List<string> tablePrefixes)
			: this(tablePrefixes, true)
		{
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="NameProvider"/> class.
		/// </summary>
		/// <param name="tablePrefixes">The table prefixes.</param>
		/// <param name="removePrefixes">if set to <c>true</c> [remove prefixes].  The default is true.</param>
		public NameProvider(List<string> tablePrefixes, bool removePrefixes)
		{
			_tablePrefixes = tablePrefixes;
			_removePrefixes = removePrefixes;
		}

		/// <summary>
		/// The default implementation will pluralize the name of the table.  So if the name
		/// of the table is AccountType, the name of the Enum will be AccountTypes.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns></returns>
		public virtual string GetEnumName(string tableName)
		{
			string result = String.Empty;
			if (String.IsNullOrEmpty(tableName))
				return result;

			return Inflector.Pluralize(this.GetClassName(tableName));
		}

		private string ScrubPrefix(string tableName)
		{
			string result = tableName;
			if (String.IsNullOrEmpty(result) || !this.RemovePrefixes || this.TablePrefixes.Count == 0)
				return result;

			foreach (string prefix in this.TablePrefixes)
			{
				if (result.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					result = tableName.Substring(prefix.Length);
					break;
				}
			}
			return result;
		}
		/// <summary>
		/// Gets the name of the enum field.
		/// </summary>
		/// <param name="nameOrLabel">The name or label.</param>
		/// <returns></returns>
		public virtual string GetEnumFieldName(string nameOrLabel)
		{
			string result = String.Empty;
			if (String.IsNullOrEmpty(nameOrLabel))
				return result;

			if (this.EnumReplacements.Count > 0)
			{
				foreach (var kvp in this.EnumReplacements)
				{
					nameOrLabel = nameOrLabel.Replace(kvp.Key, kvp.Value);
				}
			}
			//remove non letter punctuation
			string tempNameOrLabel = "";
			foreach (char c in nameOrLabel.ToCharArray())
			{
				if (!Char.IsLetterOrDigit(c))
					continue;
				tempNameOrLabel += c;
			}
			return Inflector.Pascalize(tempNameOrLabel);
		}


		/// <summary>
		/// Gets the name of the table.  If this NameProvider instance was instantiated with tableprefixes,
		/// the default behavior is to remove those prefixes from the name of the table.  This throws an 
		/// ArgumentNullException if table is null.
		/// </summary>
		/// <param name="table">The TableSchema the name of the table is extracted.</param>
		/// <returns></returns>
		public string GetTableName(TableSchema table)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table", "table is null.");
			}

			return this.GetTableName(table.Name);
		}

		/// <summary>
		/// The default implementation will trim every prefix in SkipTablePrefixes from the beginning of a table.
		/// </summary>
		/// <param name="tableName">Name of the table to trim.</param>
		/// <returns>Name of the table with every prefix in SkipTablePrefixes removed from the beginning of the name.</returns>
		public virtual string GetTableName(string tableName)
		{
			string result = tableName;
			if (String.IsNullOrEmpty(tableName))
				return result;
			
			return ScrubPrefix(result);
		}


		/// <summary>
		/// Gets a the name of a class evalutad from expression.  The default implementation passes the results
		/// of GetTableName(expression) to Inflector.Pascalize(string).
		/// </summary>
		/// <param name="expression">Expression to evaluate to the name of a class.</param>
		/// <returns>A string to use as the name of a class.</returns>
		public virtual string GetClassName(string expression)
		{
			string result = this.GetTableName(expression).Replace(" ", "");
			return Inflector.Pascalize(result);
		}

		/// <summary>
		/// Gets a value to be used as the name of a class.  Override GetClassName(string) to customize the behavior.
		/// </summary>
		/// <param name="table">The table to convert to the name of the class.</param>
		/// <returns>The name of the class based on the table.</returns>
		public string GetClassName(TableSchema table)
		{
			return this.GetClassName(table.Name);
		}

		/// <summary>
		/// Gets a property name derived from the expression.  The default implementation passes expression to 
		/// Inflector.Pascalize.  If the remaining value is 2 characters in length, the result will be both characters capitalized.
		/// </summary>
		/// <param name="expression">Expression to convert to property name.</param>
		/// <returns>The expression as a property name.</returns>
		public virtual string GetPropertyName(string expression)
		{
			string result = Inflector.Pascalize(ScrubPrefix(expression.Replace(" ", "")));
			return result.Length == 2 ? result.ToUpper() : result;
		}

		/// <summary>
		/// Gets a property name derived from a column.  Override GetPropertyName(string) to customize.
		/// </summary>
		/// <param name="column">The column the property from which the name of the property is derived.</param>
		/// <returns>The name of the property based on the column.</returns>
		public string GetPropertyName(ColumnSchema column)
		{
			return this.GetPropertyName(column.Name);
		}

		/// <summary>
		/// Gets a foreign key property name.  The default implementation removes _id from the end of the 
		/// foreignkey name and returns the result of passing that value to GetPropertyName.
		/// </summary>
		/// <param name="foreignkey">The foreign key that will supply the database name to format.</param>
		/// <returns>The forign key name formatted as a property name.</returns>
		public virtual string GetPropertyName(ForeignKeyColumnSchema foreignkey)
		{
			return this.GetPropertyName(foreignkey.Name.Replace("_id", ""));
		}

		/// <summary>
		/// Gets a property name derived from a foreign key reference.  The default implementation returns 
		/// the pluralized, pascalized version of the foreign key table name.
		/// </summary>
		/// <param name="foreignkey">The foreign key.</param>
		/// <returns>The property name derived from the foreign key.</returns>
		public virtual string GetListPropertyName(ForeignKeyColumnSchema foreignkey)
		{
			return this.GetPropertyName(Inflector.Pluralize(this.GetClassName(foreignkey.Table.Name.Replace(foreignkey.PrimaryKeyTable.Name, ""))));
		}

		/// <summary>
		/// Gets a field name derived from the expression.  The default implementation will return 
		/// underscore plus the camelized version of expression.
		/// </summary>
		/// <param name="expression">The expression from which the field name is derived.</param>
		/// <returns>The name of the field based on the expression.</returns>
		public virtual string GetFieldName(string expression)
		{
			return "_" + Inflector.Camelize(ScrubPrefix(expression.Replace(" ", "")));
		}

		/// <summary>
		/// Gets a field name derived from the column name.  Override GetFieldName(string) to customize.
		/// </summary>
		/// <param name="column">The column from which field name is derived.</param>
		/// <returns>The name of the field derived from the column.</returns>
		public string GetFieldName(ColumnSchema column)
		{
			return this.GetFieldName(column.Name);
		}

		/// <summary>
		/// Gets a field name derived from the foreign key column name.  This implemenations removes
		/// the _id from the name of a foreign key column and returns underscore and the camelized version 
		/// of the foreignkey name.
		/// </summary>
		/// <param name="foreignkey">The foreign key from which the field name is derived.</param>
		/// <returns>The name of the field derived from the foreign key.</returns>
		public virtual string GetFieldName(ForeignKeyColumnSchema foreignkey)
		{
			return this.GetFieldName(foreignkey.Name.Replace("_id", ""));
		}

		/// <summary>
		/// Gets a field name derived from a foreign key reference.
		/// </summary>
		/// <param name="foreignkey">The foreign key being referenced</param>
		/// <returns>The field name.</returns>
		public virtual string GetListFieldName(ForeignKeyColumnSchema foreignkey)
		{
			return this.GetFieldName(Inflector.Pluralize(ScrubPrefix(foreignkey.Table.Name)));
		}

		/// <summary>
		/// Returns a name formatted in camelcase for use as an argument. For example, changes FirstName to firstName.
		/// </summary>
		/// <param name="column">The column to format.</param>
		/// <returns>The name formatted as an argument.</returns>
		public virtual string GetArgumentName(ColumnSchema column)
		{
			return this.GetArgumentName(column.Name);
		}

		/// <summary>
		/// Returns a name formatted in camelcase for use as an argument. For example, changes FirstName to firstName.
		/// </summary>
		/// <param name="name">The name to format as an argument.</param>
		/// <returns>The name formatted as an argument.</returns>
		public virtual string GetArgumentName(string name)
		{
			return Inflector.Camelize(ScrubPrefix(name));
		}
		/// <summary>
		/// Used to return an escaped value of an expression if it happens to be a sql keyword
		/// </summary>
		/// <param name="expression">The expression to evaluate.</param>
		/// <returns>If the expression is a sql key word it's returned prefixed with a backtick(`), otherwise the original expression is returned.</returns>
		public virtual string Escape(string expression)
		{
			string result = String.Empty;
			if (String.IsNullOrEmpty(expression))
				return result;

			switch(expression.ToLower())
			{
            	case "transaction":
				case "description":
				case "user":
					result = "`" + expression + "`";
					break;
				default:
					result = expression;
					break;
            }
			return result;
		}
	}
}
