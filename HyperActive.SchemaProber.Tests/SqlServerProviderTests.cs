using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace HyperActive.SchemaProber.Tests
{
	public class SqlServerProviderTests : BaseSchemaProberTest
	{
		[Test]
		public void Can_get_table_schema_by_name()
		{
			var table = this.DbProvider.GetTableSchema("post");
			Assert.IsNotNull(table);
		}
		[Test]
		public void Can_get_foreign_keys_on_association_table()
		{
			var table = this.DbProvider.GetTableSchema("post_tag");
			var keys = this.DbProvider.GetForeignKeys(table);
			
		}

		[Test]
		public void Can_get_associations()
		{
			var table = this.DbProvider.GetTableSchema("post");
			Assert.AreEqual(1, table.Associations.Count);
			Assert.AreEqual("post_tag", table.Associations[0].Name);
			
		}
	}
}
