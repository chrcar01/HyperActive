using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace HyperActive.Dominator
{
	/// <summary>
	/// An enum declaration.
	/// </summary>
	public class EnumDeclaration : ICodeDom<CodeTypeDeclaration>
	{
		private string _name;
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		private List<EnumField> _values;
		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		public List<EnumField> Values
		{
			get
			{
				if (_values == null)
				{
					_values = new List<EnumField>();
				}
				return _values;
			}
		}
		/// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public EnumField AddField(string name)
		{
			int? value = null;
			return AddField(name, value);
		}
        /// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public EnumField AddField(string name, int? value)
		{
			EnumField result = new EnumField(name, value);
			this.Values.Add(result);
			return result;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumDeclaration"/> class.
		/// </summary>
		public EnumDeclaration()
			: this(String.Empty)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumDeclaration"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
        public EnumDeclaration(string name)
			: this(name, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumDeclaration"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="values">The values.</param>
        public EnumDeclaration(string name, List<EnumField> values)
		{
			_name = name;
			_values = values;
		}
		/// <summary>
		/// When called on implementing class, a CodeObject representing the class is returned.
		/// </summary>
		/// <returns></returns>
		public CodeTypeDeclaration ToCodeDom()
		{
			CodeTypeDeclaration result = new CodeTypeDeclaration(this.Name);
			result.IsEnum = true;
			foreach(var value in this.Values)
			{
				result.Members.Add(value.ToCodeDom());
			}
			return result;
		}
	}
}
