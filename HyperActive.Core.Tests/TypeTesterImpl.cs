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
			_type = type;
		}

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
	}
}
