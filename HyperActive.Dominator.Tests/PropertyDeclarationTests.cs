using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using NUnit.Framework;
using HyperActive.Dominator;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class PropertyDeclarationTests
	{
		[Test]
		public void Verify()
		{
			PropertyDeclaration prop = new PropertyDeclaration("Customer", new FieldDeclaration("_customer", typeof(int)), typeof(int));
		}
	}
}
