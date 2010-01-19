using System;
using HyperActive.Dominator;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace HyperActive.Core.Tests
{
	/// <summary>
	/// Wraps the necessary functionality for compiling and loading code for testing.
	/// </summary>
	public class DomTester : IDisposable
	{
		private AppDomain _appDomain;
		private NamespaceDeclaration _nsdecl;
		/// <summary>
		/// Initializes a new instance of the NamespaceDeclaration class.
		/// </summary>
		/// <param name="nsdecl">The namespace being compiled.</param>
		public DomTester(NamespaceDeclaration nsdecl)
		{
			_nsdecl = nsdecl;
			_appDomain = AppDomain.CreateDomain("testdomain");
			this.Compile();
			_appDomain.Load("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			_appDomain.Load(_compiled.GetName());
		}
		/// <summary>
		/// Signals that the dom tester is done and it's AppDomain can be unloaded.
		/// </summary>
		public void Complete()
		{
			AppDomain.Unload(_appDomain);
		}
		private bool _disposed = false;
		/// <summary>
		/// Calls Complete() to unload the AppDomain hosting the compiled code.
		/// </summary>
		public void Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;
			this.Complete();
		}
		/// <summary>
		/// Compiles a type.
		/// </summary>
		public void Compile()
		{
			//compile to assembly
			CodeBuilder builder = new CodeBuilder();
			CodeCompileUnit compunit = builder.CreateCodeCompileUnit(this._nsdecl);
			CodeDomProvider compiler = new CSharpCodeProvider();
			CompilerParameters options = new CompilerParameters();
			options.GenerateInMemory = true;
			options.WarningLevel = 4;
			options.TreatWarningsAsErrors = true;
			options.IncludeDebugInformation = true;
			options.ReferencedAssemblies.Add(@"C:\lib\Castle\bin\Castle.ActiveRecord.dll");
			CompilerResults results = compiler.CompileAssemblyFromDom(options, compunit);
			_isCompiled = results.Errors.Count == 0;
			_compiled = results.CompiledAssembly;
			if (results.Errors.Count > 0)
			{
				foreach(CompilerError error in results.Errors)
				{
					Console.WriteLine(error.ErrorText);
				}
			}
		}
		private Assembly _compiled = null;
		private bool _isCompiled = false;
		/// <summary>
		/// Gets a value indicating whether a namespace was successfully compiled.
		/// </summary>
		public bool IsCompiled
		{
			get
			{
				return _isCompiled;
			}
		}
		/// <summary>
		/// Finds a typeName in the compiled type.
		/// </summary>
		/// <param name="typeName">The name of the type to find.</param>
		/// <returns>True if the typeName is found in the compiled code, otherwise false.</returns>
		public bool ContainsType(string typeName)
		{
			Type type = _compiled.GetType(typeName);
			return type != null;
		}
		/// <summary>
		/// Returns an object that implements the ITypeTester interface for the typeName if it's found in the compiled code.
		/// </summary>
		/// <param name="typeName">The name of the type the ITypeTester interface will test.</param>
		/// <returns>An object that implements the ITypeTester interface that will test typeName.</returns>
		public ITypeTester Type(string typeName)
		{
			Type type = _compiled.GetType(typeName);
			ITypeTester result = new TypeTesterImpl(type);
			return result;
		}

		/// <summary>
		/// Determins if a property exists within a type.
		/// </summary>
		/// <param name="typeName">Name of the type containing the property.</param>
		/// <param name="propertyName">Name of the property to find int the type.</param>
		/// <returns>True if the property is found on the compiled type, otherwise false.</returns>
		public bool ContainsProperty(string typeName, string propertyName)
		{
			Type type = _compiled.GetType(typeName);
			return type.GetProperty(propertyName) != null;
		}
	}
}
