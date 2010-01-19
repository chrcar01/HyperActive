using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.CodeDom;
using Microsoft.CSharp;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class CodeGenTests
	{
		[Test]
		public void Test()
		{
			var @class = new ClassDeclaration("Customer");
			new CodeBuilder().GenerateCode(Console.Out, "My.Models", @class);
		}
	}
}
