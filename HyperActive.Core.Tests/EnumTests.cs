using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.CSharp;
using System.CodeDom;
using HyperActive.Dominator;

namespace HyperActive.Core.Tests
{

	[TestFixture]
	public class EnumTests
	{

		[Test]
		public void can_gen_enum_from_scratch()
		{
			var @enum = new CodeTypeDeclaration();
			@enum.IsEnum = true;
			@enum.Name = "TransactionTypes";
			var field = new CodeMemberField();
			field.InitExpression = new CodeSnippetExpression("1");
			field.Name = "Rent";
			
			@enum.Members.Add(field);
			var builder = new CodeBuilder();
			builder.GenerateCode(Console.Out, @enum);
		}
	}
}
