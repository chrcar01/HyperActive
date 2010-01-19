using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using HyperActive.SchemaProber;
using Rhino.Mocks;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class NameProviderTests
	{

		private static NameProvider CreateProvider()
		{
			return new NameProvider();
		}
		public void VerifyPrefixStrippedFromEnum()
		{
			var provider = CreateProvider();
			provider.TablePrefixes.Add("phantasm_");
			Assert.AreEqual("PostStatuses", provider.GetEnumName("phantasm_post_status"));
		}
		[Test]
		public void VerifyPrefixStrippedFromClassName()
		{
			var provider = CreateProvider();
			provider.TablePrefixes.Add("phantasm_");
			Assert.AreEqual("Address", provider.GetClassName("phantasm_Address"));
			var dbprovider = MockRepository.GenerateStub<IDbProvider>();
			Assert.AreEqual("Customer", provider.GetClassName(new TableSchema(dbprovider, "phantasm_Customer")));
		}
	}
}
