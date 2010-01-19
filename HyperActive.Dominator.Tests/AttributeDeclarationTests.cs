using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using HyperActive.Dominator;
using System.CodeDom;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class AttributeDeclarationTests
	{
		[Test]
		public void VerifyMostOfFunctionality()
		{			
			CodeDomTypeReference typeReference = new CodeDomTypeReference("Castle.ActiveRecord.HasAndBelongsToManyAttribute");
			AttributeDeclaration attrdecl = new AttributeDeclaration(typeReference);
			Assert.AreEqual("Castle.ActiveRecord.HasAndBelongsToManyAttribute", attrdecl.TypeReference.ToString());
			attrdecl.AddArgument("typeof(Snippet)");
			attrdecl.AddQuotedArgument("column_name");
			attrdecl.AddQuotedArgument("Table", "my_table_name");
			Assert.AreEqual(3, attrdecl.Arguments.Count);
			Assert.AreEqual("typeof(Snippet)", attrdecl.Arguments[0]);
			Assert.AreEqual("\"column_name\"", attrdecl.Arguments[1]);
			Assert.AreEqual("Table=\"my_table_name\"", attrdecl.Arguments[2]);
			CodeAttributeDeclaration codedomattr = attrdecl.ToCodeDom();
			Assert.AreEqual(3, codedomattr.Arguments.Count);
			Assert.AreEqual("Castle.ActiveRecord.HasAndBelongsToManyAttribute", codedomattr.AttributeType.BaseType);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullDomTypeRefInCtorShouldThrowArgNullEx()
		{
			CodeDomTypeReference typeref = null;
			new AttributeDeclaration(typeref);
		}

		[Test]
		public void VerifyAttributeTypeParameters()
		{
			AttributeDeclaration attrdecl = new AttributeDeclaration("MyAttribute", "MyTypeParam");
			Assert.AreEqual("MyAttribute`1[MyTypeParam]", attrdecl.TypeReference.ToString());
		}

		[Test]
		public void VerifyConstructorWithSystemDotType()
		{
			AttributeDeclaration attrdecl = new AttributeDeclaration(typeof(Castle.ActiveRecord.HasAndBelongsToManyAttribute));
			Assert.AreEqual("Castle.ActiveRecord.HasAndBelongsToManyAttribute", attrdecl.TypeReference.ToString());
		}
		
	}
}
