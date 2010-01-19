using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace HyperActive.Dominator.Tests
{

	[TestFixture]
	public class ConstructorTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage="containingTypeName is null or empty.", MatchType=MessageMatch.Contains)]
		public void MissingContainingTypeThrowsArgException()
		{
			new ConstructorDeclaration(null);
		}
		[Test]
		public void VerifyDefaultCtor()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("My.NS");
			nsdecl.AddClass("MyClass").AddConstructor();
			CodeBuilder builder = new CodeBuilder();
			builder.GenerateCode(Console.Out, nsdecl);
		}
		[Test]
		public void VerifySingleArg()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("My.NS");
			nsdecl.AddClass("MyClass")
				.AddConstructor(typeof(int), "id", "_id");
				
			CodeBuilder builder = new CodeBuilder();
			builder.GenerateCode(Console.Out, nsdecl);
		}
		[Test]
		public void VerifyDefaultCtorWithComment()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("My.Stuff");
			nsdecl.AddClass("SomeClass")
				.AddConstructor("Initializes a new instance of SomeClass");
			new CodeBuilder().GenerateCode(Console.Out, nsdecl);
		}
		[Test]
		public void VerifyMultipleArgs()
		{
			NamespaceDeclaration nsdecl = new NamespaceDeclaration("My.Stuff");
			nsdecl.AddClass("SomeClass")
					.AddConstructor("Initializes a new instance of SomeClass",
						new ConstructorArg(typeof(int), "id", "_id", "ID of the person"),
						new ConstructorArg(typeof(string), "name", "_name", "Name of the person")
					);
			new CodeBuilder().GenerateCode(Console.Out, nsdecl);
		}

	}
}
