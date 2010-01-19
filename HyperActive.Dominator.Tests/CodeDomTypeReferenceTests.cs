using System;
using System.Collections.Generic;
using System.Text;
using HyperActive.Dominator;
using NUnit.Framework;
using System.CodeDom;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class CodeDomTypeReferenceTests
	{
		[Test]
		public void TestDifferentTypes()
		{
			CodeDomTypeReference intref = new CodeDomTypeReference(typeof(int));
			Assert.IsTrue(intref.IsTypeOf("System.Int32"));
			Assert.AreEqual("System.Int32", intref.ToString());
			CodeDomTypeReference personref = new CodeDomTypeReference(typeof(Person));
			Assert.IsTrue(personref.IsTypeOf("HyperActive.Dominator.Tests.Person"));
			CodeDomTypeReference personlistref = new CodeDomTypeReference("System.Collections.Generic.List", "HyperActive.Dominator.Tests.Person");
			Assert.AreEqual("System.Collections.Generic.List`1[HyperActive.Dominator.Tests.Person]", personlistref.ToString());
			Assert.IsTrue(personlistref.ContainsTypeParameter("HyperActive.Dominator.Tests.Person"));
			Assert.IsTrue(personlistref.IsTypeOf("System.Collections.Generic.List`1[HyperActive.Dominator.Tests.Person]"));
			CodeDomTypeReference dictref = new CodeDomTypeReference("System.Collections.Generic.Dictionary").AddTypeParameters("int", "Person");
			Assert.AreEqual("System.Collections.Generic.Dictionary`2[int,Person]", dictref.ToString());
		}


		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullTypeInConstructorShouldThrowArgNullException()
		{
			Type type = null;
			new CodeDomTypeReference(type);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullTypeNameInCtorShouldThrowNullException()
		{
			string typeName = null;
			new CodeDomTypeReference(typeName);
		}
	}
}
