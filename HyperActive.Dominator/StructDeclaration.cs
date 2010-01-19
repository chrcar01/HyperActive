using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Reflection;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Represents a struct
	/// </summary>
	public class StructDeclaration : ICodeDom<CodeTypeDeclaration>
	{
		private string _name;
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		private bool _private = false;
		private List<FieldDeclaration> _fields;
		/// <summary>
		/// Gets the fields.
		/// </summary>
		/// <value>The fields.</value>
		public List<FieldDeclaration> Fields
		{
			get
			{
				if (_fields == null)
					_fields = new List<FieldDeclaration>();
				return _fields;
			}
		}
		/// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public StructDeclaration AddField(FieldDeclaration field)
		{
			this.Fields.Add(field);
			return this;
		}
		/// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="name">The name.</param>
		/// <param name="initialValue">The initial value.</param>
		/// <returns></returns>
		public StructDeclaration AddField(Type dataType, string name, string initialValue)
		{
			this.Fields.Add(new FieldDeclaration(name, dataType).InitializeTo(initialValue));
			return this;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="StructDeclaration"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public StructDeclaration(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Determines whether this instance is private.
		/// </summary>
		/// <returns></returns>
		public StructDeclaration IsPrivate()
		{
			_private = true;
			return this;
		}
		#region ICodeDom<CodeTypeDeclaration> Members

		/// <summary>
		/// When called on implementing class, a CodeObject representing the class is returned.
		/// </summary>
		/// <returns></returns>
		public CodeTypeDeclaration ToCodeDom()
		{
			CodeTypeDeclaration result = new CodeTypeDeclaration(this.Name);
			result.IsStruct = true;
			if (_private) result.TypeAttributes = TypeAttributes.NestedPrivate;
			foreach (FieldDeclaration field in this.Fields)
			{
				result.Members.Add(field.ToCodeDom());
			}
			return result;
		}

		#endregion
	}
}
