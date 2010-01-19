using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class EnumDeclarationTests
	{
		[Test]
		public void can_create_enum()
		{
			var @enum = new EnumDeclaration("TransactionLineTypes");
			@enum.AddField("Pending", 1);
			var builder = new CodeBuilder();
			builder.GenerateCode(Console.Out, "Awish.Lars.Core.Enums", @enum);
		}
	}
}
