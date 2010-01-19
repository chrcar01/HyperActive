using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Collections.Specialized;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Types of LineDeclarations.
	/// </summary>
	public enum LineTypes
	{
		/// <summary>
		/// A LineDeclaration will be generating a line that creates a new variable declaration.
		/// </summary>
		Declaration,
		/// <summary>
		/// The LineDeclaration will be generating some form of variable or property assignment.
		/// </summary>
		Assignment,
		/// <summary>
		/// The LineDeclaration will be generating code that calls a method on a local variable.
		/// </summary>
		Call
	}

	/// <summary>
	/// Represents a line of code.  Ultimately this will becomes CodeStatement.
	/// </summary>
	public class LineDeclaration : ICodeDom<CodeStatement>
	{
		private string _localVariableName;
		private string _localVariableProperty;
        private string _methodName;
		private string _variableName;
		private CodeDomTypeReference _variableTypeReference = null;
		private bool _initializeField = false;
		private LineTypes _lineType;

		
		/// <summary>
		/// Creates a new instance of a LineDeclaration.
		/// </summary>
		/// <param name="variableName">Name of the variable used within this statement.</param>
		/// <param name="lineType">The type of the line being created, for example, a variable declaration or an assignment.</param>
		public LineDeclaration(string variableName, LineTypes lineType)
		{
			_variableName = variableName;
			_lineType = lineType;
		}
		/// <summary>
		/// Initializes a new instance of the LineDeclaration class.
		/// </summary>
		/// <param name="variableName">Name of the variable used in the line.</param>
		/// <param name="methodName">Name of the method to call on the variable.</param>
		public LineDeclaration(string variableName, string methodName)
		{
			_variableName = variableName;
			_methodName = methodName;
			_lineType = LineTypes.Call;
		}
		/// <summary>
		/// Sets the variable used in this LineDeclaration to a new instance of the CodeDomTypeReference passed to the method.
		/// </summary>
		/// <param name="typeRef">The variable used in the current instance of the LineDeclaration will be set to a new instance of this type.</param>
		/// <returns></returns>
		public LineDeclaration New(CodeDomTypeReference typeRef)
		{
			this._variableTypeReference = typeRef;
			this._initializeField = true;
			return this;
		}

		private string _propertyName;

		/// <summary>
		/// Specifies the property off of the variable of this LineDeclaration to be used in the generated statement.
		/// </summary>
		/// <param name="propertyName">Name of the property off of the variable within this LineDeclaration.</param>
		/// <returns></returns>
		public LineDeclaration Property(string propertyName)
		{
			this._propertyName = propertyName;
			return this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="localVariableName"></param>
		/// <param name="localVariableProperty"></param>
		/// <returns></returns>
		public LineDeclaration Assign(string localVariableName, string localVariableProperty)
		{
			this._initializeField = true;
			this._localVariableName = localVariableName;
			this._localVariableProperty = localVariableProperty;
			return this;
		}
		/// <summary>
		/// Get's the name of the variable used in this LineDeclaration.
		/// </summary>
		public string VariableName
		{
			get
			{
				return _variableName;
			}
		}
		private string _propertyValue;
		/// <summary>
		/// Sets the value to use when in an assignment LineDeclaration.
		/// </summary>
		/// <param name="propertyValue">Property value used in assignments.</param>
		/// <returns>The current instance of the LineDeclaration with the property value set.</returns>
		public LineDeclaration To(string propertyValue)
		{
			this._propertyValue = propertyValue;
			return this;
		}
		
		/// <summary>
		/// Sets the type for a variable being declared.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public LineDeclaration As(Type type)
		{
			this._variableTypeReference = new CodeDomTypeReference(type);
			return this;
		}
		/// <summary>
		/// Responsible for transforming this LineDeclaration into it's CodeDom equivalent.
		/// </summary>
		/// <returns>CodeStatement representing this instance of the LineDeclaration.</returns>
		public CodeStatement ToCodeDom()
		{
			if (LineTypes.Declaration == this._lineType)
			{
				CodeVariableDeclarationStatement var = new CodeVariableDeclarationStatement(this._variableTypeReference.ToCodeDom(), this._variableName);
				if (this._initializeField)
				{
					if (this._variableTypeReference != null)
						var.InitExpression = new CodeObjectCreateExpression(this._variableTypeReference.ToCodeDom());
					if (!String.IsNullOrEmpty(this._localVariableName) && !String.IsNullOrEmpty(this._localVariableProperty))
						var.InitExpression = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(this._localVariableName), this._localVariableProperty);
				}
				return var;
			}
			if (LineTypes.Assignment == this._lineType)
			{
				CodeExpression left = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(this._variableName), this._propertyName);
				CodeExpression right = new CodeSnippetExpression(this._propertyValue);
				CodeAssignStatement assign = new CodeAssignStatement(left, right);
				return assign;
			}
			if (LineTypes.Call == this._lineType)
			{
				return new CodeExpressionStatement(CreateInvokeExpression());
			}
			return null;
		}

		private CodeMethodInvokeExpression CreateInvokeExpression()
		{
			CodeExpression target = String.IsNullOrEmpty(this._variableName) ? null : new CodeVariableReferenceExpression(this._variableName);
			CodeMethodInvokeExpression invoker = new CodeMethodInvokeExpression(target, this._methodName);
			foreach (string callArg in this._methodCallArgs)
			{
				invoker.Parameters.Add(new CodeSnippetExpression(callArg));
			}
			return invoker;
		}

		/// <summary>
		/// Toes the code expression.
		/// </summary>
		/// <returns></returns>
		public CodeExpression ToCodeExpression()
		{
			return CreateInvokeExpression();
		}

		private StringCollection _methodCallArgs = new StringCollection();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="methodCallArgs"></param>
		/// <returns></returns>
		public LineDeclaration With(params string[] methodCallArgs)
		{
			_methodCallArgs.AddRange(methodCallArgs);
			return this;
		}
	}
}
