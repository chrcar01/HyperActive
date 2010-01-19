using System;
using System.CodeDom;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wraps a CodeMemberField.
	/// </summary>
	public class FieldDeclaration : ICodeDom<CodeMemberField>
	{
		private bool _initialize = false;
		private string _initValue = String.Empty;
		private bool _isPublic = false;
		private bool _isStatic = false;
        private string _name;
		private CodeDomTypeReference _typeReference = null;
		/// <summary>
		/// Initializes a new instance of a FieldDeclaration class.
		/// </summary>
		/// <param name="name">Name of the field.</param>
		/// <param name="type">Type of the field.</param>
		public FieldDeclaration(string name, Type type)
			: this(name, new CodeDomTypeReference(type))
		{
		}
		/// <summary>
		/// Initializes a new instance of the FieldDeclaration class.
		/// </summary>
		/// <param name="name">Name of the field.</param>
		/// <param name="typeName">Name of the type for the field.</param>
		/// <param name="typeParameters">Type parameters for the type.</param>
		public FieldDeclaration(string name, string typeName, params string[] typeParameters)
			: this(name, new CodeDomTypeReference(typeName, typeParameters))
		{
		}
		/// <summary>
		/// Initializes a new instance of the FieldDeclaration class.
		/// </summary>
		/// <param name="name">Name of the field.</param>
		/// <param name="typeReference">Type of the field.</param>
		public FieldDeclaration(string name, CodeDomTypeReference typeReference)
		{
			this._name = name;
			this._typeReference = typeReference;
		}
		/// <summary>
		/// Gets or sets a value indicating whether the field is initialized.
		/// </summary>
		public bool Initialize
		{
			get
			{
				return _initialize;
			}
			set
			{
				_initialize = value;
			}
		}
		/// <summary>
		/// Gets the name of the fied.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		private CodeDomTypeReference _initializer = null;
		/// <summary>
		/// Adds an initializer for the field.
		/// </summary>
		/// <param name="typeReference">Initializer for the field.</param>
		/// <returns>This instance of the FieldDeclaration with the initializer added.</returns>
		public FieldDeclaration AddInitializer(CodeDomTypeReference typeReference)
		{
			this.Initialize = true;
			_initializer = typeReference;
			return this;
		}
		/// <summary>
		/// Determines whether this instance is public.
		/// </summary>
		/// <returns></returns>
		public FieldDeclaration IsPublic()
		{
			_isPublic = true;
			return this;
		}

		private List<String> _comments = new List<string>();

		/// <summary>
		/// Adds the comment.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public FieldDeclaration AddComment(string format, params object[] args)
		{
			_comments.Add(String.Format(format, args));
			return this;
		}

		/// <summary>
		/// Converts this instance of FieldDeclaration to a CodeMemberField.
		/// </summary>
		/// <returns>This instance represented as a CodeMemberField.</returns>
		public CodeMemberField ToCodeDom()
		{
			CodeMemberField result = new CodeMemberField(this._typeReference.ToCodeDom(), this.Name);
			if (_isPublic) result.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			if (_isStatic) result.Attributes = result.Attributes | MemberAttributes.Static;
			if (this.Initialize)
			{
				if (!String.IsNullOrEmpty(_initValue))
					result.InitExpression = new CodeSnippetExpression(_initValue);
				else
					result.InitExpression = new CodeObjectCreateExpression((_initializer ?? _typeReference).ToCodeDom());
			}
			if (this._comments != null && this._comments.Count > 0)
			{
				result.Comments.Add(new CodeCommentStatement("<summary>", true));
				foreach (string comment in _comments)
				{
					result.Comments.Add(new CodeCommentStatement(comment, true));
				}
				result.Comments.Add(new CodeCommentStatement("</summary>", true));
			}
			return result;
		}

		/// <summary>
		/// Initializes the field to the initial value.
		/// </summary>
		/// <param name="initialValue">The initial value.</param>
		/// <returns></returns>
		public FieldDeclaration InitializeTo(string initialValue)
		{
			_initialize = true;
			_initValue = initialValue;
			return this;
		}

		/// <summary>
		/// Determines whether this instance is static.
		/// </summary>
		/// <returns></returns>
		public FieldDeclaration IsStatic()
		{
			_isStatic = true;
			return this;
		}
	}
}
