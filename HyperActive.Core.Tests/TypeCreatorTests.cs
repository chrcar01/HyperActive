using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using HyperActive.Core;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class TypeCreatorTests
	{
		[Test]
		public void Go()
		{
			Assembly asm = Assembly.LoadFile(@"c:\svn\home\HyperActive\trunk\HyperActive.Core.Tests\bin\Debug\HyperActive.Core.dll");
			Type type = asm.GetType("HyperActive.Core.NameProvider");
			object inst = Activator.CreateInstance(type, null);
			NameProvider nameprovider = inst as NameProvider;
			Assert.IsNotNull(nameprovider);
		}
	}
}
