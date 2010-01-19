using System;
using System.Collections.Generic;
using System.CodeDom;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wrapper for the CodeConstructor
	/// </summary>
	public class ConstructorDeclaration : ICodeDom<CodeConstructor>
	{
		private string _containingTypeName;
		private List<ConstructorArg> _args = null;
		private List<string> _comments;
		/// <summary>
		/// Gets the comments for the constructor.
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
		
		/// <summary>
		/// Gets the list of arguments for this constructor.
		/// </summary>
		public List<ConstructorArg> Args
		{
			get
			{
				if (_args == null)
					_args = new List<ConstructorArg>();
				return _args;
			}
		}
		/// <summary>
		/// Initializes a new instance of the ConstructorDeclaration class.
		/// </summary>
		/// <param name="containingTypeName">The name of the type containing this constructor.  Passing in zero args creates a default constructor.</param>
		/// <param name="args">The constructor arguments.  Zero args results in a default constructor.</param>
		public ConstructorDeclaration(string containingTypeName, params ConstructorArg[] args)
			: this(containingTypeName, String.Empty, args)
		{
		}

        /// <summary>
		/// Initializes a new instance of the ConstructorDeclaration class.
		/// </summary>
		/// <param name="containingTypeName">The name of the type containing this constructor.  Passing in zero args creates a default constructor.</param>
		/// <param name="comment">Comment for the constructor.</param>
		/// <param name="args">The constructor arguments.  Zero args results in a default constructor.</param>
		public ConstructorDeclaration(string containingTypeName, string comment, params ConstructorArg[] args)
		{
			if (String.IsNullOrEmpty(containingTypeName))
				throw new ArgumentException("containingTypeName is null or empty.", "containingTypeName");

			_containingTypeName = containingTypeName;
			if (!String.IsNullOrEmpty(comment))
				this.Comments.Add(comment);
			if (args != null && args.Length > 0)
				this.Args.AddRange(new List<ConstructorArg>(args));
			else
				this.Args.Add(new ConstructorArg());
		}

        /// <summary>
		/// Initializes a new instance of the ConstructorDeclaration class.
		/// </summary>
		/// <param name="containingTypeName">The name of the type containing this constructor.</param>
		/// <param name="argType">The type of arg.</param>
		/// <param name="argName">The name of the arg.</param>
		/// <param name="fieldName">The name of the field tha arg to which the arg is mapped.</param>
		public ConstructorDeclaration(string containingTypeName, System.Type argType, string argName, string fieldName)
			: this(containingTypeName, new ConstructorArg(argType, argName, fieldName))
		{
		}

		/// <summary>
		/// Adds an argument to the constructor.
		/// </summary>
		/// <param name="type">The type of arg.</param>
		/// <param name="argName">The name of the arg.</param>
		/// <param name="fieldName">The name of the field tha arg to which the arg is mapped.</param>
		/// <returns></returns>
		public ConstructorDeclaration AddArgument(Type type, string argName, string fieldName)
		{
			string comment = String.Empty;
			return AddArgument(type, argName, fieldName, comment);
		}
        /// <summary>
		/// Adds an argument to the constructor.
		/// </summary>
		/// <param name="type">The type of arg.</param>
		/// <param name="argName">The name of the arg.</param>
		/// <param name="fieldName">The name of the field tha arg to which the arg is mapped.</param>
		/// <param name="comment">The comment for the doc summary for this arg.</param>
		/// <returns></returns>
		public ConstructorDeclaration AddArgument(Type type, string argName, string fieldName, string comment)
		{
			ConstructorArg arg = new ConstructorArg(type, argName, fieldName, comment);
			this.Args.Add(arg);
			return this;
		}

		private List<CodeStatement> _statements;

		/// <summary>
		/// Gets the statements.  Use this to add the body of the constructor.
		/// </summary>
		/// <value>The statements that will appear in the body.</value>
		public List<CodeStatement> Statements
		{
			get
			{
				if (_statements == null)
				{
					_statements = new List<CodeStatement>();
				}
				return _statements;
			}
		}

		/// <summary>
		/// Creates the CodeDom representation of this ConstructorDeclaration.
		/// </summary>
		/// <returns>CodeConstructor representing this instance of ConstructorDeclaration.</returns>
		public CodeConstructor ToCodeDom()
		{
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;
			ctor.ReturnType = new CodeTypeReference(_containingTypeName);

			if (this.Statements.Count > 0)
			{
				ctor.Statements.AddRange(this.Statements.ToArray());
			}

			foreach (ConstructorArg arg in this.Args)
			{
				if (String.IsNullOrEmpty(arg.ArgName))
					continue;

				ctor.Parameters.Add(arg.ToCodeDom());
				if (this.Statements.Count == 0)
				{
					ctor.Statements.Add(
						new CodeAssignStatement(
							new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), arg.FieldName),
							new CodeArgumentReferenceExpression(arg.ArgName)));
				}
			}	
			
			if (this.Comments.Count > 0)
			{
				ctor.Comments.Add(new CodeCommentStatement("<summary>", true));
				foreach (string comment in this.Comments)
				{
					ctor.Comments.Add(new CodeCommentStatement(comment, true));
				}
				ctor.Comments.Add(new CodeCommentStatement("</summary>", true));
				foreach (ConstructorArg arg in this.Args)
				{
					if (arg.Comments.Count == 0)
						continue;
					string s = String.Format("<param name=\"{0}\">{1}</param>", arg.ArgName, String.Join(" ", arg.Comments.ToArray()));
					ctor.Comments.Add(new CodeCommentStatement(s, true));
				}
			}
			return ctor;
		}
	}
}
