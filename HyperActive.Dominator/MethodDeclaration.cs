using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wraps a CodeMemberMethod.
	/// </summary>
	public class MethodDeclaration : ICodeDom<CodeMemberMethod>
	{
		private string _name;		
		private CodeDomTypeReference _returnType = null;
		/// <summary>
		/// Initializes a new instance of the MethodDeclaration class.
		/// </summary>
		/// <param name="name"></param>
		public MethodDeclaration(string name)
			: this(name, null)
		{
		}
        /// <summary>
		/// Initializes a new instance of the MethodDeclaration class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="returnType"></param>
		public MethodDeclaration(string name, CodeDomTypeReference returnType)
		{
			_name = name;
			_returnType = returnType;
		}
		private List<AttributeDeclaration> _attributes = new List<AttributeDeclaration>();

		/// <summary>
		/// Adds an attribute to a method.
		/// </summary>
		/// <param name="typeName">Name of the type of attribute.</param>
		/// <param name="typeParameters">Parameters used in defining the attribute.</param>
		/// <returns>The AttributeDeclaration insance that was added to the method.</returns>
		public AttributeDeclaration AddAttribute(string typeName, params string[] typeParameters)
		{
			if (String.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");

			return this.AddAttribute(new CodeDomTypeReference(typeName, typeParameters));
		}
		/// <summary>
		/// Adds an attribute to the method.
		/// </summary>
		/// <param name="type">The type of the attribute to add.</param>
		/// <returns>The AttributeDeclaration insance that was added to the method.</returns>
		public AttributeDeclaration AddAttribute(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return this.AddAttribute(new CodeDomTypeReference(type));
		}
		/// <summary>
		/// Adds an attribute to the method.
		/// </summary>
		/// <param name="typeReference">The type reference to the attribute to add.</param>
		/// <returns>The AttributeDeclaration insance that was added to the method.</returns>
		public AttributeDeclaration AddAttribute(CodeDomTypeReference typeReference)
		{
			if (typeReference == null)
				throw new ArgumentNullException("typeReference");

			AttributeDeclaration result = new AttributeDeclaration(typeReference);
			this._attributes.Add(result);
			return result;
		}
		private List<LineDeclaration> _methodLines = new List<LineDeclaration>();
		/// <summary>
		/// Adds a variable declaration line to the method.
		/// </summary>
		/// <param name="variableName">The name of the variable being declared.</param>
		/// <returns>The LineDeclaration that was created.</returns>
		public LineDeclaration Declare(string variableName)
		{
			LineDeclaration result = new LineDeclaration(variableName, LineTypes.Declaration);
			_methodLines.Add(result);
			return result;
		}

		/// <summary>
		/// Adds an assignment statement line to the method.
		/// </summary>
		/// <param name="variableName">The name of the variable being assigned a value.</param>
		/// <returns>The LineDeclaration that was created.</returns>
		public LineDeclaration Assign(string variableName)
		{
			LineDeclaration result = new LineDeclaration(variableName, LineTypes.Assignment);
			this._methodLines.Add(result);
			return result;
		}
		private List<MemberAttributes> _methodAttributes = null;
		/// <summary>
		/// Gets the MethodAttributes that will be applied to the method.  MemberAttributes.Final is specified by default.
		/// </summary>
		public List<MemberAttributes> MethodAttributes
		{
			get
			{
				if (_methodAttributes == null)
				{
					_methodAttributes = new List<MemberAttributes>();
					_methodAttributes.Add(MemberAttributes.Final); //non virtual unless specified
				}
				return _methodAttributes;
			}
		}
		/// <summary>
		/// Indicates that the method will be virtual.
		/// </summary>
		/// <returns>This method with the MemberAttributes.Final attribute removed, indicating that it will be defined as virtual.</returns>
		public MethodDeclaration Virtual()
		{
			if (this.MethodAttributes.Contains(MemberAttributes.Final))
				this.MethodAttributes.Remove(MemberAttributes.Final);
			return this;
		}
		/// <summary>
		/// Indicates that the method will be public.
		/// </summary>
		/// <returns>This method with the MemberAttributes.Public attribute added, indicating the the method will be defined as public.</returns>
		public MethodDeclaration Public()
		{
			if (!this.MethodAttributes.Contains(MemberAttributes.Public)) 
				this.MethodAttributes.Add(MemberAttributes.Public);
			return this;
		}

		/// <summary>
		/// Responsible for creating the CodeMemberMethod based on the values contained within the MethodDeclaration instance.
		/// </summary>
		/// <returns>A CodeMemberMethod represented by this MethodDeclaration.</returns>
		public CodeMemberMethod ToCodeDom()
		{
			CodeMemberMethod result = new CodeMemberMethod();
			result.Name = this._name;
			if (this._returnType != null)
				result.ReturnType = this._returnType.ToCodeDom();
			MemberAttributes? attribs = new Nullable<MemberAttributes>();
			foreach (MemberAttributes attrib in this.MethodAttributes)
			{
				if (attribs == null)
					attribs = attrib;
				else
					attribs = attribs | attrib;
			}
			if (attribs != null)
				result.Attributes = (MemberAttributes)attribs;
			foreach (AttributeDeclaration attrDecl in this._attributes)
			{
				result.CustomAttributes.Add(attrDecl.ToCodeDom());
			}
			foreach (KeyValuePair<string, System.Type> param in this._parameters)
			{
				result.Parameters.Add(new CodeParameterDeclarationExpression(param.Value, param.Key));
			}
			if (this._methodLines != null)
			{
				foreach (LineDeclaration line in this._methodLines)
				{
					result.Statements.Add(line.ToCodeDom());
				}
			}
			if (_returns != null)
			{
				result.Statements.Add(new CodeMethodReturnStatement(_returns.ToCodeExpression()));
			}
			return result;	
		}

		/// <summary>
		/// Adds a LineDeclaration to this MethodDeclaration that calls a method on a field.
		/// </summary>
		/// <param name="fieldName">The field to which the method is called.</param>
		/// <param name="methodName">The method to call.</param>
		/// <returns>The LineDeclaration that was added to this instance of the MethodDeclaration.</returns>
		public LineDeclaration Call(string fieldName, string methodName)
		{
			LineDeclaration result = new LineDeclaration(fieldName, methodName);
			this._methodLines.Add(result);
			return result;
		}
		private LineDeclaration _returns = null;
		/// <summary>
		/// Returns the specified method to call.
		/// </summary>
		/// <param name="methodToCall">The method to call.</param>
		/// <returns></returns>
		public LineDeclaration Returns(string methodToCall)
		{
			_returns = new LineDeclaration("", methodToCall);
			return _returns;
		}

		private Dictionary<string, System.Type> _parameters = new Dictionary<string, Type>();
		/// <summary>
		/// Adds a parameter declaration to the method signature.
		/// </summary>
		/// <param name="type">Type of the parameter.</param>
		/// <param name="name">Name of the parameter.</param>
		public MethodDeclaration AddParameter(Type type, string name)
		{
			this._parameters.Add(name, type);
			return this;
		}
	}
}
