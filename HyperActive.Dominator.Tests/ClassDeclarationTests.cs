using System;
using NUnit.Framework;
using HyperActive.Dominator;
using System.CodeDom;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class ClassDeclarationTests
	{
		[Test]
		public void CanInheritFromInterface()
		{
			ClassDeclaration cdecl = new ClassDeclaration("Fuck").Implements(new InterfaceDeclaration("You"));
			new CodeBuilder().GenerateCode(Console.Out, "Blah", cdecl);
		}
		
		[Test]
		public void AddProperties()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("Test");
			ClassDeclaration cdecl = nsdecl.AddClass("Customer");
			FieldDeclaration lastName = new FieldDeclaration("_lastName", "System.String");
			PropertyDeclaration propdecl = new PropertyDeclaration("LastName", lastName, typeof(string));
			cdecl.AddProperty(propdecl);
			PropertyDeclaration firstName = cdecl.AddProperty("FirstName", "_firstName", typeof(string));
			PropertyDeclaration duplicateFirstName = cdecl.AddProperty("FirstName", "_firstName", typeof(string));
			Assert.AreEqual(firstName, duplicateFirstName);
			cdecl.AddProperty("DateOfBirth", "_dateOfBirth", typeof(DateTime), true);
			cdecl.AddProperty("Age", "_age", new CodeDomTypeReference(typeof(int)));
			cdecl.AddProperty("Items", "_items", "System.Collections.Generic.List", "System.String");
			using (DomTester dom = new DomTester(nsdecl))
            {
				Assert.IsTrue(dom.ContainsType("Test.Customer"));
				Assert.IsTrue(dom.ContainsProperty("Test.Customer", "FirstName"));
				Assert.IsTrue(dom.ContainsProperty("Test.Customer", "LastName"));
				Assert.IsTrue(dom.ContainsProperty("Test.Customer", "DateOfBirth"));
				Assert.IsTrue(dom.ContainsProperty("Test.Customer", "Age"));
				Assert.IsTrue(dom.ContainsProperty("Test.Customer", "Items"));
            }
			new CodeBuilder().GenerateCode(Console.Out, nsdecl);
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddAttribute_Type_DoesNotAllowNulls()
		{
			Type type = null;
			new ClassDeclaration("Customer").AddAttribute(type);
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddAttribute_StringStringArray_DoesNotAllowNulls()
		{
			string typeName = null;
			new ClassDeclaration("Customer").AddAttribute(typeName);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddAttribute_CodeDomTypeReference_DoesNotAllowNulls()
		{
			CodeDomTypeReference type = null;
			new ClassDeclaration("Customer").AddAttribute(type);
		}
		[Test]
		public void VerifyAddClassTypeParameters()
		{
			NamespaceDeclaration nsdecl = 
				new NamespaceDeclaration("Test");

			nsdecl.AddClass("Customer");
			nsdecl.AddClass("CustomerList")
				.InheritsFrom("System.Collections.Generic.List", "Test.Customer");

			using (DomTester dom = new DomTester(nsdecl))
            {
				Assert.IsTrue(dom.Type("CustomerList")
					.InheritsFrom("List`1", "Customer"));
            }
			//new CodeBuilder().GenerateCode(Console.Out, nsdecl);
		}
		[Test]
		public void VerifyInheritsFrom()
		{
			NamespaceDeclaration nsdecl = 
				new NamespaceDeclaration("TestContainer");

			nsdecl.AddClass("Manager")
				.InheritsFrom(nsdecl.AddClass("Employee"));

			using (DomTester dom = new DomTester(nsdecl))
            {
				Assert.IsTrue(dom.Type("Manager").InheritsFrom("Employee"));				
            }

			//new CodeBuilder().GenerateCode(Console.Out, nsdecl);
		}

		[Test]
		public void VerifyAddAttribute()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("Test");
			ClassDeclaration cdecl = nsdecl.AddClass("Customer");
			cdecl.AddAttribute(typeof(SerializableAttribute));

			ClassDeclaration attrib1 = nsdecl.AddClass("SuperAttribute").InheritsFrom("System.Attribute");
			cdecl.AddAttribute(new CodeDomTypeReference("Test.SuperAttribute"));

			ClassDeclaration attrib2 = nsdecl.AddClass("Crattribute").InheritsFrom("System.Attribute");
			cdecl.AddAttribute("Test.Crattribute");

			using (DomTester dom = new DomTester(nsdecl))
            {
				Assert.IsTrue(dom.Type("Customer").HasAttribute("System.SerializableAttribute"));
				Assert.IsTrue(dom.Type("Customer").HasAttribute("Test.SuperAttribute"));
				Assert.IsTrue(dom.Type("Customer").HasAttribute("Test.Crattribute"));
            }
			new CodeBuilder().GenerateCode(Console.Out, nsdecl);
		}
	}
	
}