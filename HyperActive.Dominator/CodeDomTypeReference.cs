using System;
using System.CodeDom;
using System.Collections.Specialized;
using System.Collections.Generic;


namespace HyperActive.Dominator
{
	/// <summary>
	/// Wraps a CodeTypeReference.
	/// </summary>
	public class CodeDomTypeReference : ICodeDom<CodeTypeReference>
	{
		private string _typeName = null;
		private Type _type = null;
		private StringCollection _typeParameters = null;
		private bool _nullable = false;


		/// <summary>
		/// Returns a string representation of the type that this instance of CodeDomTypeReference represents.
		/// </summary>
		public override string ToString()
		{
			string result = "";

			if (this._type != null)
				result = this._type.ToString();
			else
			{
				result = this._typeName;
				if (this._typeParameters != null && this._typeParameters.Count > 0)
				{
					string types = "";
					foreach(string typeParameter in this._typeParameters)
					{
						if (types.Length > 0) types += ",";
						types += typeParameter;
					}
					result += String.Format("`{0}[{1}]", this._typeParameters.Count, types);
				}
			}
			return result;
		}
		/// <summary>
		/// Indicates that the type is nullable.
		/// </summary>
		/// <returns></returns>
		public CodeDomTypeReference IsNullable()
		{
			this._nullable = true;
			return this;
		}
		/// <summary>
		/// Determins if this CodeDomTypeReference matches a typeName.
		/// </summary>
		/// <param name="typeName">The typeName to match.</param>
		/// <returns>True if it matches, false otherwise.</returns>
		public bool IsTypeOf(string typeName)
		{
			return this.ToString().Equals(typeName);
		}


		/// <summary>
		/// Initializes a new instance of the CodeDomTypeReference class.
		/// </summary>
		/// <param name="type">The type this class represents.</param>
		public CodeDomTypeReference(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			_type = type;
		}

		/// <summary>
		/// Initializes a new instance of the CodeDomTypeReference class.
		/// </summary>
		/// <param name="typeName">The name of the type this class represents.</param>
		/// <param name="typeParameters">One or more type parameters for a generic type.</param>
		public CodeDomTypeReference(string typeName, params string[] typeParameters)
		{
			if (String.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");

			_typeName = typeName;
			this.AddTypeParameters(typeParameters);
		}
		/// <summary>
		/// Determines if a type parameter name exists in this CodeDomTypeReference.
		/// </summary>
		/// <param name="typeParameterName">The type parameter to find.</param>
		/// <returns>True if the type parameter name exists, otherwise false.</returns>
		public bool ContainsTypeParameter(string typeParameterName)
		{
			return this._typeParameters != null && this._typeParameters.Contains(typeParameterName);
		}
		/// <summary>
		/// Adds a type parameter to this instance of a CodeDomTypeReference.
		/// </summary>
		/// <param name="typeParameters">The type parameters to add.</param>
		/// <returns>This instance of a CodeDomTypeReference with the added type parameters.</returns>
		public CodeDomTypeReference AddTypeParameters(params string[] typeParameters)
		{
			if (typeParameters == null || typeParameters.Length == 0)
				return this;

			if (this._typeParameters == null)
				this._typeParameters = new StringCollection();

			this._typeParameters.AddRange(typeParameters);
			return this;
		}
		/// <summary>
		/// Converts this CodeDomTypeReference to a CodeTypeReference.
		/// </summary>
		/// <returns>A CodeTypeReference representing this instance of a CodeDomTypeReference.</returns>
		public virtual CodeTypeReference ToCodeDom()
		{
			CodeTypeReference result;
			if (!String.IsNullOrEmpty(this._typeName))
				result = new CodeTypeReference(this._typeName);
			else
				result = new CodeTypeReference(this._type);
			if (this._nullable && isnullable(this._type, this._typeName))
				result = new CodeTypeReference("System.Nullable", result);

			if (this._typeParameters != null && this._typeParameters.Count > 0)
			{
				foreach (string typeParamName in this._typeParameters)
				{
					result.TypeArguments.Add(typeParamName);
				}
			}
			return result;
		}
		private bool isnullable(Type type, string typeName)
		{
			if (type == typeof(string) || type == typeof(byte[]))
				return false;
			if (typeName == "string" || typeName == "byte[]")
				return false;
			return true;
		}
	}
}
