using System;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// Describes data needed for generating enums.
	/// </summary>
	public class EnumDescriptor
	{
		private string _tableName;
		/// <summary>
		/// Gets or sets the name of the table.
		/// </summary>
		/// <value>The name of the table.</value>
		public string TableName
		{
			get
			{
				return _tableName;
			}
			set
			{
				_tableName = value;
			}
		}
		private string _nameField;
		/// <summary>
		/// Gets or sets the name field.
		/// </summary>
		/// <value>The name field.</value>
		public string NameField
		{
			get
			{
				return _nameField;
			}
			set
			{
				_nameField = value;
			}
		}
		private string _valueField;
		/// <summary>
		/// Gets or sets the value field.
		/// </summary>
		/// <value>The value field.</value>
		public string ValueField
		{
			get
			{
				return _valueField;
			}
			set
			{
				_valueField = value;
			}
		}
	}
}
