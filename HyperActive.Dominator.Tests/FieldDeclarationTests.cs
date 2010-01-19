using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using HyperActive.Dominator;
using System.CodeDom;


namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class FieldDeclarationTests
	{
		[Test]
		public void Verify()
		{
			FieldDeclaration field = new FieldDeclaration("_customer", new CodeDomTypeReference(typeof(int)));
			Assert.AreEqual("_customer", field.Name);
			field.Initialize = true;
			CodeMemberField codefield = field.ToCodeDom();
			Assert.AreEqual("_customer", codefield.Name);
			Assert.AreEqual("System.Int32", codefield.Type.BaseType);

		}
	}
}
