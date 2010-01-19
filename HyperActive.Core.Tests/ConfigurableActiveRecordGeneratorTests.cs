using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HyperActive.Core.Generators;
using HyperActive.Core.Config;
using System.Configuration;
using HyperActive.SchemaProber;
using HyperActive.Dominator;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class ConfigurableActiveRecordGeneratorTests
	{
		[Test]
		public void Can_generate_associations()
		{

			string connString = "integrated security=SSPI;server=(local);database=Lars";
			var dbprovider = new SqlServerProvider(new DbHelper(connString));
			var configOptions = new DefaultConfigurationOptions();
			var generator = new AwishModelGenerator(); //new ConfigurableActiveRecordGenerator(configOptions);
			generator.ConfigOptions = configOptions;
			//foreach (var tbl in dbprovider.GetTableSchemas().Where(tbl => { return tbl.PrimaryKey != null; }))
			//{
			var tbl = dbprovider.GetTableSchema("LeasePaymentDescriptorLease");
			//var fkrefs = tbl.PrimaryKey.ForeignKeyReferences;
			//foreach (var fkref in fkrefs)
			//{
			//	
			//}
			var theNs = new NamespaceDeclaration("Awish.Lars.Data");
			var cdecl = generator.Generate(theNs, tbl);
			new CodeBuilder().GenerateCode(Console.Out, "Awish.Lars.Data", cdecl);
			//}
		}
	}
}
