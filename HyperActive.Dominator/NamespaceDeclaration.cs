using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Collections.Specialized;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wrapper class around the CodeNamespace.
	/// </summary>
	public class NamespaceDeclaration
	{
		private StringCollection _imports = new StringCollection();
		private List<ICodeDom<CodeTypeDeclaration>> _classes;
		private string _name;
		/// <summary>
		/// Gets the namespace name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		/// <summary>
		/// Adds namespaces to be used as using/imports statements in the generated code.
		/// </summary>
		/// <param name="namespaces"></param>
		public NamespaceDeclaration Imports(params string[] namespaces)
		{
			_imports.AddRange(namespaces);
			return this;
		}

		/// <summary>
		/// Initializes a new instance of a NamespaceDeclaration.
		/// </summary>
		/// <param name="name">The name of the namespace.</param>
		public NamespaceDeclaration(string name)
		{
			_name = name;
			_classes = new List<ICodeDom<CodeTypeDeclaration>>();
		}

		/// <summary>
		/// Adds a class declaration to the namespace.
		/// </summary>
		/// <param name="name">Name of the class.</param>
		/// <param name="isPartialClass">Indicates whether the class is partial or not.</param>
		/// <returns>The class declaration instance that was added to the namespace.</returns>
		public ClassDeclaration AddClass(string name, bool isPartialClass)
		{
			ClassDeclaration result = new ClassDeclaration(this, name);
			result.IsPartial = isPartialClass;
			_classes.Add(result);
			return result;
		}
		/// <summary>
		/// Adds a class declaration to the namespace.
		/// </summary>
		/// <param name="name">Name of the class.</param>
		/// <param name="typeParameters">List of type parameters if the class is generic.</param>
		/// <returns>The class declaration instance that was added the namespace.</returns>
		public ClassDeclaration AddClass(string name, params string[] typeParameters)
		{
			ClassDeclaration result = new ClassDeclaration(this, name, typeParameters);
			result.IsPartial = false;
			_classes.Add(result);
			return result;
		}
		/// <summary>
		/// Adds an object, presumably an ActiveRecord class definition, to the namespace.
		/// </summary>
		/// <param name="classDeclaration">The class declaration.</param>
		public void AddClass(ICodeDom<CodeTypeDeclaration> classDeclaration)
		{
			_classes.Add(classDeclaration);
		}

		/// <summary>
		/// Responsible for generating the NamespaceDeclaration to it's CodeDom equivalent, CodeNamespace.
		/// </summary>
		/// <returns>CodeNamespace representing this NamespaceDeclaration.</returns>
		public CodeNamespace ToCodeDom()
		{
			CodeNamespace cns = new CodeNamespace(this.Name);
			foreach (string import in _imports)
			{
				cns.Imports.Add(new CodeNamespaceImport(import));
			}
			foreach (ICodeDom<CodeTypeDeclaration> cdecl in _classes)
			{
				cns.Types.Add(cdecl.ToCodeDom());
			}
			return cns;
		}
	}
}
