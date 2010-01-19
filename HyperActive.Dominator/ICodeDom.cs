using System;
using System.CodeDom;
namespace HyperActive.Dominator
{
	/// <summary>
	/// Indicates that a Type can convert itself to anything in the CodeDom.
	/// </summary>
	/// <typeparam name="T">The CodeObject that will be created by calling ToCodeDom.</typeparam>
	public interface ICodeDom<T> where T : CodeObject
	{
		/// <summary>
		/// When called on implementing class, a CodeObject representing the class is returned.
		/// </summary>
		/// <returns></returns>
		T ToCodeDom();
	}
}
