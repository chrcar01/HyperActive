using System;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Interface that describes methods that can be used to test Types.
	/// </summary>
	public interface ITypeTester
	{
		/// <summary>
		/// Method implementations would verify that a type inherits the baseTypeName and contains any typeParameters specified.
		/// </summary>
		/// <param name="baseTypeName"></param>
		/// <param name="typeParameters"></param>
		/// <returns></returns>
		bool InheritsFrom(string baseTypeName, params string[] typeParameters);

		/// <summary>
		/// Determines whether the specified type name has attribute.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type name has attribute; otherwise, <c>false</c>.
		/// </returns>
		bool HasAttribute(string typeName);
	}

}
