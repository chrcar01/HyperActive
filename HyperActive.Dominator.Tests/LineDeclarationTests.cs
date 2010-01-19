using System;
using System.Collections.Generic;
using System.Text;
using HyperActive.Dominator;
using NUnit.Framework;
using System.CodeDom;
using Microsoft.CSharp;
using System.Data;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class LineDeclarationTests
	{
		[Test]
		public void Test()
		{
			ClassDeclaration cdecl = new ClassDeclaration("ResultSet");
			MethodDeclaration mdecl = cdecl.AddMethod("GetData", typeof(DataTable))
				.Public()
				.AddParameter(typeof(string), "name");
			mdecl.Declare("result").New(new CodeDomTypeReference(typeof(DataTable)));

			mdecl.Declare("columns")
				.As(typeof(DataColumnCollection))
				.Assign("result", "Columns");

			mdecl.Call("columns", "Add").With("typeof(string)", "\"name\"");
			new CodeBuilder().GenerateCode(Console.Out, "My.Data", cdecl);
		}

		[Test]
		public void CanCreateDataTableAssignment()
		{
			CodeNamespace nsdecl = new CodeNamespace("My.Data");
			CodeTypeDeclaration cdecl = new CodeTypeDeclaration("ResultSet");
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "GetData";
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			method.ReturnType = new CodeTypeReference("System.Data.DataTable");
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
			method.Statements.Add(new CodeVariableDeclarationStatement(
				typeof(DataTable),
				"result",
				new CodeObjectCreateExpression(typeof(DataTable))));
			cdecl.Members.Add(method);
			method.Statements.Add(
				new CodeVariableDeclarationStatement(
					typeof(DataColumnCollection), 
					"columns", 
					new CodePropertyReferenceExpression(
						new CodeVariableReferenceExpression("result"), 
						"Columns")));
			method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("columns"), "Add", new CodeTypeOfExpression(typeof(string)), new CodeSnippetExpression("name")));

			nsdecl.Types.Add(cdecl);
			CSharpCodeProvider provider = new CSharpCodeProvider();
			provider.GenerateCodeFromNamespace(nsdecl, Console.Out, new System.CodeDom.Compiler.CodeGeneratorOptions());
		}





		[Test]
		public void VerifyCtors()
		{
			CodeNamespace nsdecl = new CodeNamespace("My.Data");
			CodeTypeDeclaration cdecl = new CodeTypeDeclaration("ResultSet");
			CodeConstructor ctor = new CodeConstructor();
			ctor.ReturnType = new CodeTypeReference("ResultSet");
			ctor.Attributes = MemberAttributes.Public;
			cdecl.Members.Add(ctor);
			nsdecl.Types.Add(cdecl);
			CSharpCodeProvider provider = new CSharpCodeProvider();
			provider.GenerateCodeFromNamespace(nsdecl, Console.Out, new System.CodeDom.Compiler.CodeGeneratorOptions());
		}






//		[Test]
//		public void Verify()
//		{
//			CodeVariableDeclarationStatement line1 = new CodeVariableDeclarationStatement(typeof(Person), "person", new CodeObjectCreateExpression(typeof(Person)));
//			Line line2 = new Line(typeof(Person)).Initialize();
//			CodeBuilder builder = new CodeBuilder();
//			builder.GenerateCode(Console.Out, line2.ToCodeDom());
//			Assert.AreEqual("person", line2.FieldName);
//////			CodeAssignStatement line3 = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("person"), "Name"), new CodeSnippetExpression("\"Chris\""));
//////			Line line4 = Line.Assign("person.Name").EqualTo("\"Chris\"");
////		}
//		[Test]
//		public void VerifyBlah()
//		{
//			CodeTypeReference typeref = new CodeTypeReference("Nullable", new CodeTypeReference("System.String"));
//			CodeMemberField field = new CodeMemberField(typeref, "name");
//			CSharpCodeProvider provider = new CSharpCodeProvider();
//			provider.GenerateCodeFromMember(field, Console.Out, new System.CodeDom.Compiler.CodeGeneratorOptions());
//		}
	}

	//public class Line : ICodeDom<CodeStatement>
	//{
	//	private bool _initialize = false;
	//	private string _fieldName = "";
	//	public string FieldName
	//	{
	//		get
	//		{
	//			return _fieldName;
	//		}
	//	}
	//	private Type _type;
	//	public Line(Type type)
	//	{
	//		_type = type;
	//		string typeName = _type.ToString();
	//		_fieldName = typeName.Substring(typeName.LastIndexOf(".")+1).ToLower();
	//	}
	//	public Line Initialize()
	//	{
	//		_initialize = true;
	//		return this;
	//	}
	//	public CodeStatement ToCodeDom()
	//	{
	//		if (_initialize)
	//			return new CodeVariableDeclarationStatement(this._type, this.FieldName, new CodeObjectCreateExpression(this._type));

	//		return null;
	//	}
	//}
}
