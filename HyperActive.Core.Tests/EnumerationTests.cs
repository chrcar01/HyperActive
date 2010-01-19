using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.CodeDom;
using HyperActive.Dominator;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class EnumerationTests
	{
		[Test]
		public void Can_gen_from_scratch()
		{
			var type = new CodeTypeDeclaration("AccountTypes");
			type.BaseTypes.Add(new CodeTypeReference(typeof(Enumeration)));

			CodeTypeReference fieldType = new CodeTypeReference(typeof(EnumerationField));

			var f1 = new CodeMemberField(fieldType, "_AccountType1");
			f1.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			f1.InitExpression = new CodeObjectCreateExpression(fieldType, new CodeSnippetExpression("1"), new CodeSnippetExpression("\"AccountType1\""));
			type.Members.Add(f1);

			var p1 = new CodeMemberProperty();
			p1.Type = fieldType;
			p1.Name = "AccountType1";
			p1.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			p1.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, "_AccountType1")));
			type.Members.Add(p1);

			var builder = new CodeBuilder();
			builder.GenerateCode(Console.Out, type);
		}
		[Test]
		public void Test()
		{
			Assert.IsTrue(1 == AccountTypes.AccountType1);	
		}
	}

	public class AccountTypes : HyperActive.Core.Enumeration
	{

		private static HyperActive.Core.EnumerationField _AccountType1 = new HyperActive.Core.EnumerationField(1, "AccountType1");

		public static HyperActive.Core.EnumerationField AccountType1
		{
			get
			{
				return _AccountType1;
			}
		}
	}
}
