using System;
using System.Collections.Generic;
using System.CodeDom;

namespace HyperActive.Dominator
{
	/// <summary>
	/// An abstraction over a CodeMemberProperty.
	/// </summary>
	public class PropertyDeclaration  : ICodeDom<CodeMemberProperty>
	{		
		private CodeDomTypeReference _typeReference = null;
		/// <summary>
		/// Gets the CodeDomTypeReference for the property.
		/// </summary>
		public CodeDomTypeReference TypeReference
		{
			get
			{
				return _typeReference;
			}
		}
		private string _name = null;
		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		private FieldDeclaration _field;
		/// <summary>
		/// Gets the FieldDeclaration associated with the property.
		/// </summary>
		public FieldDeclaration Field
		{
			get
			{
				return _field;
			}
		}
		private List<AttributeDeclaration> _attributes = new List<AttributeDeclaration>();
		/// <summary>
		/// Gets a list of attributes that will be used to adorn the property.
		/// </summary>
		public List<AttributeDeclaration> Attributes
		{
			get
			{
				return _attributes;
			}
		}
		/// <summary>
		/// Initializes a new instance of a PropertyDeclaration.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="field">The field the property will encapsulate.</param>
		/// <param name="typeReference">A CodeDomTypeReference the PropertyDeclaration uses to determine it's type.</param>
		public PropertyDeclaration(string name, FieldDeclaration field, CodeDomTypeReference typeReference)
		{
			this._name = name;
			this._field = field;
			this._typeReference = typeReference;
		}
		/// <summary>
		/// Initializes a new instance of a PropertyDeclaration.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="field">The field the property will encapsulate.</param>
		/// <param name="typeName">The name of the type for the property.</param>
		/// /// <param name="typeParameters">A list of types used for constructing Generic type references.</param>
		public PropertyDeclaration(string name, FieldDeclaration field, string typeName, params string[] typeParameters)
			: this(name, field, new CodeDomTypeReference(typeName, typeParameters))
		{
		}
		/// <summary>
		/// Initializes a new instance of a PropertyDeclaration.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="field">The field the property will encapsulate.</param>
		/// <param name="type">The System.Type of the property.</param>
		public PropertyDeclaration(string name, FieldDeclaration field, Type type)
			: this(name, field, new CodeDomTypeReference(type))
		{
		}
		/// <summary>
		/// Adds an attribute to the property.
		/// </summary>
		/// <param name="typeReference">A reference to the type of attribute to add.</param>
		/// <returns>The AttributeDeclaration that was added to the property.  If the attribute has already been added it returns the existing attribute.</returns>
		public AttributeDeclaration AddAttribute(CodeDomTypeReference typeReference)
		{
			AttributeDeclaration result = this.Attributes.Find(delegate(AttributeDeclaration attr) { return attr.TypeReference.Equals(typeReference); });
			if (result != null)
				return result;

			result = new AttributeDeclaration(typeReference);
			this.Attributes.Add(result);
			return result;
		}
		/// <summary>
		/// Adds an attribute to the property.
		/// </summary>
		/// <param name="typeName">Name of the type to add.</param>
		/// <param name="typeParameters">Any type parameters required by the type.</param>
		/// <returns>The AttributeDeclaration that was added to the property.  If the attribute has already been added it returns the existing attribute.</returns>
		public AttributeDeclaration AddAttribute(string typeName, params string[] typeParameters)
		{
			if (String.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");

			return this.AddAttribute(new CodeDomTypeReference(typeName, typeParameters));
		}
		/// <summary>
		/// Adds an attribute to the property.
		/// </summary>
		/// <param name="type">The type of attribute to add.</param>
		/// <returns>The AttributeDeclaration that was added to the property.  If the attribute has already been added it returns the existing attribute.</returns>
		public AttributeDeclaration AddAttribute(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return this.AddAttribute(new CodeDomTypeReference(type));
		}
		/// <summary>
		/// Adds a type parameter to the type reference.
		/// </summary>
		/// <param name="typeParameterName">The type parameter to add to the type of the property.</param>
		/// <returns>This instance of the PropertyDeclaration after the type parameter has been added.</returns>
		public PropertyDeclaration AddTypeParameter(string typeParameterName)
		{
			if (!this._typeReference.ContainsTypeParameter(typeParameterName))
				this._typeReference.AddTypeParameters(typeParameterName);
			return this;
		}

		/// <summary>
		/// Sets the Initialize property on the encapsulated field to true.
		/// </summary>
		/// <returns></returns>
		public PropertyDeclaration InitializeField(CodeDomTypeReference typeReference)
		{
			if (this.Field != null)			
				this.Field.AddInitializer(typeReference);
			
			return this;
		}
		/// <summary>
		/// Responsible for translating all of the Dominator interfaces and classes into plain ol' CodeDom when the actual dom needs to be generated.
		/// </summary>
		/// <returns>An instance of a CodeMemberProperty created from the properties of this PropertyDeclaration.</returns>
		public CodeMemberProperty ToCodeDom()
		{
			CodeMemberProperty prop = new CodeMemberProperty();

			prop.GetStatements.Add(
				new CodeMethodReturnStatement(
						new CodeFieldReferenceExpression(null, this.Field.Name)));
			prop.Attributes = MemberAttributes.Public;
			if (this._isStatic)
				prop.Attributes = (prop.Attributes | MemberAttributes.Static);

			foreach (AttributeDeclaration attr in this.Attributes)
			{
				bool skip = false;
				foreach (CodeAttributeDeclaration customAttrib in prop.CustomAttributes)
				{
					if (customAttrib.Name == attr.TypeReference.ToString())
					{
						skip = true;
						break;
					}
				}
				if (skip) continue;

				prop.CustomAttributes.Add(attr.ToCodeDom());
			}
			prop.Name = this.Name;
			prop.Type = this.TypeReference.ToCodeDom();
			if (!this._isReadOnly)
			{
				prop.SetStatements.Add(
						new CodeAssignStatement(
							new CodeFieldReferenceExpression(null, this.Field.Name),
							new CodePropertySetValueReferenceExpression()));
			}

			if (this._comments != null && this._comments.Count > 0)
			{
				prop.Comments.Add(new CodeCommentStatement("<summary>", true));
				foreach (string comment in _comments)
				{					
					prop.Comments.Add(new CodeCommentStatement(comment, true));
				}
				prop.Comments.Add(new CodeCommentStatement("</summary>", true));
			}

			return prop;
		}
		private List<String> _comments = new List<string>();
		/// <summary>
		/// Adds a new comment line to the property.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An object array used when formatting the comment message.</param>
		/// <returns>The property instance with the added comment.</returns>
		public PropertyDeclaration AddComment(string format, params object[] args)
		{
			_comments.Add(String.Format(format, args));
			return this;
		}
		private bool _isStatic = false;
		/// <summary>
		/// Determines whether this instance is static.
		/// </summary>
		/// <returns></returns>
		public PropertyDeclaration IsStatic()
		{
			_isStatic = true;
			return this;
		}
		private bool _isReadOnly = false;
		/// <summary>
		/// Determines whether [is read only].
		/// </summary>
		/// <returns></returns>
		public PropertyDeclaration IsReadOnly()
		{
			_isReadOnly = true;
			return this;
		}
	}
}
