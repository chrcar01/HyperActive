using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.CodeDom;
using HyperActive;
using HyperActive.Core;
using HyperActive.Dominator;
using HyperActive.SchemaProber;
using NUnit.Framework;
using System.IO;
using HyperActive.Core.Generators;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class ActiveRecordTestGeneratorTests
	{
		[Test]
		public void StartMeUp()
		{
			IDbHelper helper = null; // new DbHelper(MyConnectionStrings.EasementsDev);
			IDbProvider dbprovider = new SqlServerProvider(helper);
			DatabaseSchema db = new DatabaseSchema(dbprovider);			
			ActiveRecordTestGenerator bliss = new ActiveRecordTestGenerator(new NameProvider());
			foreach (TableSchema table in db.Tables)
			{
				Console.WriteLine("Building " + table.Name + "Tests");				
				bliss.Generate(Console.Out, "Crap.Data", table);
			}
		}
	}
}
