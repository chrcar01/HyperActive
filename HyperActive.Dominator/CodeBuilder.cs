using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wraps the operations for generating code.
	/// </summary>
	public class CodeBuilder
	{
		private CodeDomProvider _domProvider = null;

	
		/// <summary>
		/// Gets the CodeDomProvider that will be used when generating code.  The default is the CSharpCodeProvider.
		/// </summary>
		public CodeDomProvider DomProvider
		{
			get
			{
				if (_domProvider == null)
				{
					_domProvider = new CSharpCodeProvider();
				}
				return _domProvider;
			}
		}
		/// <summary>
		/// Initializes a new instance of the CodeBuilder class using the CSharpCodeProvider for code generation.
		/// </summary>
		public CodeBuilder()
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodeBuilder class.
		/// </summary>
		/// <param name="domProvider">The CodeDomProvider to use when generating code.</param>
		public CodeBuilder(CodeDomProvider domProvider)
		{
			_domProvider = domProvider;
		}

		/// <summary>
		/// Creates a CodeCompileUnit from a NamespaceDeclaration.
		/// </summary>
		/// <param name="nsdecl"></param>
		/// <returns></returns>
		public CodeCompileUnit CreateCodeCompileUnit(NamespaceDeclaration nsdecl)
		{
			CodeCompileUnit result = new CodeCompileUnit();
			result.Namespaces.Add(nsdecl.ToCodeDom());
			return result;
		}

		/// <summary>
		/// Generates code from a codeTypeDeclaration.
		/// </summary>
		/// <param name="writer">The textwriter the code is written to.</param>
		/// <param name="namespaceName">The namespace that will contain the class.</param>
		/// <param name="codeTypeDeclaration">The CodeDom definition to generate.</param>
		public void GenerateCode(TextWriter writer, string namespaceName, ICodeDom<System.CodeDom.CodeTypeDeclaration> codeTypeDeclaration)
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration(namespaceName);
			nsdecl.AddClass(codeTypeDeclaration);
			this.GenerateCode(writer, nsdecl);
		}

		/// <summary>
		/// Generates code from a NamespaceDeclaration instance.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="nsdecl"></param>
		public void GenerateCode(TextWriter writer, NamespaceDeclaration nsdecl)
		{
			CodeCompileUnit compunit = new CodeCompileUnit();
			compunit.Namespaces.Add(nsdecl.ToCodeDom());
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			this.DomProvider.GenerateCodeFromCompileUnit(compunit, writer, options);
		}
		/// <summary>
		/// Generates code from a type.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="typeDeclaration"></param>
		public void GenerateCode(TextWriter writer, CodeTypeDeclaration typeDeclaration)
		{
			this.DomProvider.GenerateCodeFromType(typeDeclaration, writer, new CodeGeneratorOptions());
		}
		/// <summary>
		/// Generates code from a CodeStatement.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="statement"></param>
		public void GenerateCode(TextWriter writer, CodeStatement statement) 
		{
			this.DomProvider.GenerateCodeFromStatement(statement, writer, new CodeGeneratorOptions());
		}
	}
}
