using System;
using HyperActive.Dominator;
using System.CodeDom;

namespace HyperActive.Core.Generators
{
	public interface IEnumGenerator
	{
		/// <summary>
		/// Generates the specified writer.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="nameField">The name field.</param>
		/// <param name="valueField">The value field.</param>
		ICodeDom<CodeTypeDeclaration> Generate(string tableName, string nameField, string valueField);
	}
}
