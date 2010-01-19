using System;
using HyperActive.Dominator;

namespace HyperActive.Dominator
{
	/// <summary>
	/// An implementation of ITypeTester.
	/// </summary>
	public class TypeTesterImpl : ITypeTester
	{
		private Type _type;
		/// <summary>
		/// Initializes a new instance of the TypeTesterImpl class.
		/// </summary>
		/// <param name="type"></param>
		public TypeTesterImpl(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type", "type is null.");

			_type = type;
		}

		/// <summary>
		/// Verifies that the contained type inherits the base type and any typeParameters contained in the base type.
		/// </summary>
		/// <param name="baseTypeName"></param>
		/// <param name="typeParameters"></param>
		/// <returns></returns>
		public bool InheritsFrom(string baseTypeName, params string[] typeParameters)
		{
			if (!_type.BaseType.Name.Equals(baseTypeName))
				return false;

			if (typeParameters == null || typeParameters.Length == 0)
				return true;

			Type[] typeArgs = _type.BaseType.GetGenericArguments();
			foreach (string typeParameter in typeParameters)
			{
				if (!Array.Exists<Type>(typeArgs, delegate(Type typeArg) { return typeArg.Name.Equals(typeParameter); }))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the specified attribute type name has attribute.
		/// </summary>
		/// <param name="attributeTypeName">Name of the attribute type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified attribute type name has attribute; otherwise, <c>false</c>.
		/// </returns>
		public bool HasAttribute(string attributeTypeName)
		{
			if (String.IsNullOrEmpty(attributeTypeName))
				throw new ArgumentException("attributeTypeName is null or empty.", "attributeTypeName");

			object[] attribs = this._type.GetCustomAttributes(false);
			if (attribs == null || attribs.Length == 0)
				return false;

			return Array.Exists<object>(attribs, delegate(object attrib) { return attrib.ToString() == attributeTypeName; });
		}
	}
}
