using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wrapper around the CodeAttributeDeclaration
	/// </summary>
	public class AttributeDeclaration
	{
		private CodeDomTypeReference _typeReference;
		/// <summary>
		/// Gets the CodeDomTypeReference for this AttributeDeclaration.
		/// </summary>
		public CodeDomTypeReference TypeReference
		{
			get
			{
				return _typeReference;
			}
		}

		#region Instance Constructors
		/// <summary>
		/// Initializes a new instance of the AttributeDeclaration class.
		/// </summary>
		/// <param name="typeReference"></param>
		public AttributeDeclaration(CodeDomTypeReference typeReference)
		{
			if (typeReference == null)
				throw new ArgumentNullException("typeReference");

			_typeReference = typeReference;
		}
		/// <summary>
		/// Initializes a new instance of the AttributeDeclaration class.
		/// </summary>
		/// <param name="typeName">The name of the attrbute type.</param>
		/// /// <param name="typeParameters">Attrubte type parameters.</param>
		public AttributeDeclaration(string typeName, params string[] typeParameters)
			: this(new CodeDomTypeReference(typeName, typeParameters))
		{

		}
		/// <summary>
		/// Initializes a new instance of the AttributeDeclaration class.
		/// </summary>
		/// <param name="type">The type of attribute.</param>
		public AttributeDeclaration(Type type)
			: this(new CodeDomTypeReference(type))
		{

		}
		#endregion

		/// <summary>
		/// Responsible for converting the current instance of an AttributeDeclaration
		/// to it's CodeDom equivalent, CodeAttrbuteDeclaration.
		/// </summary>
		/// <returns></returns>
		public CodeAttributeDeclaration ToCodeDom()
		{
			CodeAttributeDeclaration result = new CodeAttributeDeclaration(_typeReference.ToCodeDom());
			foreach (object arg in Arguments)
			{
				result.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression(arg.ToString())));
			}
			return result;
		}
		/// <summary>
		/// Adds a quoted agument to the attribute declaration.  For example, if the attribute name was MyAttribute,
		/// calling AddQuotedArgument("Description of something") would result in
		/// an attribute that looks like [MyAttribute("Description of something")].
		/// </summary>
		/// <param name="value">The attribute argument that will be generated in quotes.</param>
		/// <returns>The instance of the AttributeDeclaration with the  added argument.</returns>
		public AttributeDeclaration AddQuotedArgument(string value)
		{
			this.AddArgument(quote(value));
			return this;
		}

		/// <summary>
		/// Adds a quoted agument to the attribute declaration.  For example, if the attribute name was MyAttribute,
		/// calling AddQuotedArgument(ColumnName, "my_column") would result in
		/// an attribute that looks like [MyAttribute(ColumnName="my_column")].
		/// </summary>
		/// <param name="name">Name of the attribute parameter.</param>
		/// <param name="value">Value that will be generated in quotes.</param>
		/// <returns>The instance of the AttributeDeclaration with the  added argument.</returns>
		public AttributeDeclaration AddQuotedArgument(string name, string value)
		{

			this.AddArgument(String.Format("{0}={1}", name, quote(value)));
			return this;
		}

		/// <summary>
		/// Adds an argument to the attribute declaration "as-is".  
		/// If the attribute type was MyAttribute, calling AddArgument("Name=Value") would result in an 
		/// attribute that looks like [MyAttribute(Name=Value)].
		/// </summary>
		/// <param name="value">The attribute argument.</param>
		/// <returns>The instance of the AttributeDeclaration with the  added argument.</returns>
		public AttributeDeclaration AddArgument(string value)
		{
			if (!Arguments.Contains(value))
				Arguments.Add(value);
			return this;
		}


		private StringCollection _arguments = new StringCollection();
		/// <summary>
		/// Gets the list of arguments to pass to the attribute.
		/// </summary>
		public StringCollection Arguments
		{
			get
			{
				return _arguments;
			}
		}

		private string quote(string value)
		{
			return String.Format("\"{0}\"", value);
		}

	}
}
