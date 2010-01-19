using System;
using System.Collections.Generic;
using System.Text;
using HyperActive.Core;
using HyperActive.Dominator;
using NUnit.Framework;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using HyperActive.SchemaProber;

namespace HyperActive.Core.Tests
{
	/// <summary>
	/// Interface that describes methods that can be used to test Types.
	/// </summary>
	public interface ITypeTester
	{
		bool InheritsFrom(string baseTypeName, params string[] typeParameters);
	}


	[TestFixture]
	public class DomVerifier
	{
		[Test]
		public void CanCreateActiveRecordFromTable()
		{

			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "Find";
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			method.ReturnType = new CodeTypeReference("UserProfile");
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "id"));
			method.Statements.Add(new CodeVariableDeclarationStatement(typeof(bool), "doNotThrowExceptionIfNotFound", new CodeSnippetExpression("false")));
			method.Statements.Add(
				new CodeMethodReturnStatement(
					new CodeMethodInvokeExpression(null, "FindByPrimaryKey", new CodeExpression[] { new CodeArgumentReferenceExpression("id"), new CodeVariableReferenceExpression("doNotThrowExceptionIfNotFound") })));

			CodeDomProvider provider = new CSharpCodeProvider();
			provider.GenerateCodeFromMember(method, Console.Out, new CodeGeneratorOptions());
		}
		[Test]
		public void CanCreateActiveRecordClass2()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("My.Data").Imports("Castle.ActiveRecord");
			ClassDeclaration cdecl = nsdecl.AddClass("Customer").IsAbstract().InheritsFrom("ActiveRecordBase", "My.Data.Customer");
			using (DomTester tester = new DomTester(nsdecl))
			{
				Assert.IsTrue(tester.IsCompiled, "should have compiled");
				Assert.IsTrue(tester.ContainsType("My.Data.Customer"));
				Assert.IsTrue(tester.Type("My.Data.Customer").InheritsFrom("ActiveRecordBase`1", "Customer"));
			}
		}

		[Test]
		public void CanCreateActiveRecordClass()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("My.Data").Imports("Castle.ActiveRecord");
			ClassDeclaration cdecl = nsdecl.AddClass("Customer").IsAbstract().InheritsFrom("ActiveRecordBase", "My.Data.Customer");

			//compile to assembly
			CodeBuilder builder = new CodeBuilder();
			CodeCompileUnit compunit = builder.CreateCodeCompileUnit(nsdecl);
			CodeDomProvider compiler = new CSharpCodeProvider();
			CompilerParameters options = new CompilerParameters();
			options.GenerateInMemory = true;
			options.WarningLevel = 4;
			options.TreatWarningsAsErrors = true;
			options.IncludeDebugInformation = true;
			options.ReferencedAssemblies.Add(@"C:\lib\Castle\bin\Castle.ActiveRecord.dll");
			CompilerResults results = compiler.CompileAssemblyFromDom(options, compunit);
			
				foreach (CompilerError error in results.Errors)
				{
					Console.WriteLine(error.ErrorText);
				}
			
			Assembly compiled = results.CompiledAssembly;


			//load assembly in it's own app domain
			AppDomain testdomain = AppDomain.CreateDomain("testdomain");
			testdomain.Load("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			testdomain.Load(compiled.GetName());

			//test the assembly
			Type testtype = compiled.GetType("My.Data.Customer");
			Assert.IsNotNull(testtype);
			Assert.AreEqual("ActiveRecordBase`1", testtype.BaseType.Name);
			Assert.AreEqual("Customer", testtype.BaseType.GetGenericArguments()[0].Name);

			AppDomain.Unload(testdomain);
		}

		[Test]
		public void CanCreateClass()
		{
			//create dom
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("My.Data");
			ClassDeclaration cdecl = nsdecl.AddClass("Customer");
			cdecl.AddProperty("ID", "_id", "System.Int32");

			//compile to assembly
			CodeBuilder builder = new CodeBuilder();
			CodeCompileUnit compunit = builder.CreateCodeCompileUnit(nsdecl);
			CodeDomProvider compiler = new CSharpCodeProvider();
			CompilerParameters options = new CompilerParameters();
			options.GenerateInMemory = true;
			options.WarningLevel = 4;
			options.TreatWarningsAsErrors = true;
			options.IncludeDebugInformation = true;
			CompilerResults results = compiler.CompileAssemblyFromDom(options, compunit);
			Assembly compiled = results.CompiledAssembly;

			//load assembly in it's own app domain
			AppDomain testdomain = AppDomain.CreateDomain("testdomain");
			testdomain.Load("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			testdomain.Load(compiled.GetName());

			//test the assembly
			Type testtype = compiled.GetType("My.Data.Customer");
			Assert.IsNotNull(testtype);
			PropertyInfo id = testtype.GetProperty("ID");
			Assert.AreEqual(typeof(int), id.PropertyType);
			AppDomain.Unload(testdomain);
		}
	}
}
