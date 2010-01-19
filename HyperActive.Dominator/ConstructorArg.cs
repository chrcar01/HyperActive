using System;
using System.Collections.Generic;
using System.CodeDom;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Represents a single argument in a ConstrctorDeclaration.
	/// </summary>
	public class ConstructorArg : ICodeDom<CodeParameterDeclarationExpression>
	{
		private string _typeName;
		/// <summary>
		/// Gets the name of the type.
		/// </summary>
		/// <value>The name of the type.</value>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
		}
		private List<string> _comments;
		/// <summary>
		/// Gets the comments for this argument.
		/// </summary>
		public List<string> Comments
		{
			get
			{
				if (_comments == null)
					_comments = new List<string>();
				return _comments;
			}
		}
		private System.Type _argType;
		/// <summary>
		/// Gets the type of the argument.
		/// </summary>
		public System.Type ArgType
		{
			get
			{
				return _argType;
			}
		}
		private string _argName;
		/// <summary>
		/// Gets the name of the argument.
		/// </summary>
		public string ArgName
		{
			get
			{
				return _argName;
			}
		}
		private string _fieldName;
		/// <summary>
		/// Gets the name of the field the argument is mapped.
		/// </summary>
		public string FieldName
		{
			get
			{
				return _fieldName;
			}
		}
		/// <summary>
		/// Initializes a new instance of the ConstructorArg class.
		/// </summary>
		/// <param name="argType">The type of arg.</param>
		/// <param name="argName">The name of the arg.</param>
		/// <param name="fieldName">The name of the field tha arg to which the arg is mapped.</param>
		public ConstructorArg(System.Type argType, string argName, string fieldName)
			: this(argType, argName, fieldName, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ConstructorArg class.  This results in a default constructor.
		/// </summary>
		public ConstructorArg()
		{
		}

        /// <summary>
		/// Initializes a new instance of the ConstructorArg class.
		/// </summary>
		/// <param name="argType">The type of arg.</param>
		/// <param name="argName">The name of the arg.</param>
		/// <param name="fieldName">The name of the field tha arg to which the arg is mapped.</param>
		/// <param name="comment">The comment that will appear in a param comment for the constructor.</param>
		public ConstructorArg(System.Type argType, string argName, string fieldName, string comment)
		{
			if (argType == null)
				throw new ArgumentNullException("argType", "argType is null.");
			if (String.IsNullOrEmpty(argName))
				throw new ArgumentException("argName is null or empty.", "argName");
			if (String.IsNullOrEmpty(fieldName))
				throw new ArgumentException("fieldName is null or empty.", "fieldName");

			_argType = argType;
			_argName = argName;
			_fieldName = fieldName;
			if (!String.IsNullOrEmpty(comment))
				this.Comments.Add(comment);
		}
		/// <summary>
		/// Initializes a new instance of the ConstructorArg class.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="argName"></param>
		public ConstructorArg(string typeName, string argName)
		{
			_typeName = typeName;
			_argName = argName;
		}
		/// <summary>
		/// Transforms this wrapper to it's CodeDom format.
		/// </summary>
		/// <returns>A CodeParameterDeclarationExpression representing this instance.</returns>
		public CodeParameterDeclarationExpression ToCodeDom()
		{
			if (String.IsNullOrEmpty(this.TypeName))
			{
				return new CodeParameterDeclarationExpression(this.ArgType, this.ArgName);
			}
			else
			{
				return new CodeParameterDeclarationExpression(this.TypeName, this.ArgName);
			}
		}
	}
}
