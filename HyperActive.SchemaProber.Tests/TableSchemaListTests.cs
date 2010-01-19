using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Configuration;
using System.Data;
using Moq;

namespace HyperActive.SchemaProber.Tests
{
	[TestFixture]
	public class TableSchemaListTests
	{
		private IDbProvider _dbProvider;
		[TestFixtureSetUp]
		public void InitializeAllTests()
		{
			var helper = new DbHelper("server=(local);database=HyperActive;Integrated Security=SSPI");
			_dbProvider = new SqlServerProvider(helper);
		}
		[Test]
		public void Can_get_associations()
		{
			
		}
	}
}
