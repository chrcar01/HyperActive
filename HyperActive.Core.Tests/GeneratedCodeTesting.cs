using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using HyperActive.SchemaProber;
using HyperActive.Dominator;
using HyperActive.Core.Generators;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class Tests
	{
		//[Test]
		//public void CanStripTablePrefix()
		//{
		//	MockRepository mocks = new MockRepository();

		//	//define the mock database objects and behaviors
		//	IDbProvider dbprovider = mocks.CreateMock<IDbProvider>();
		//	string tableName = "hyperblog_post";
		//	TableSchema table = new TableSchema(dbprovider, tableName);
		//	PrimaryKeyColumnSchema pk = mocks.CreateMock<PrimaryKeyColumnSchema>();
		//	Expect.Call<string>(pk.Name).Return("post_id").Message("expect name of the pk is post_id").Repeat.Twice();
		//	Expect.Call<Type>(pk.DataType).Return(typeof(int)).Message("expect type of the pk is int");
		//	Expect.Call<bool>(pk.IsIdentity).Return(true).Message("expect isidentity is true");
		//	Expect.Call<PrimaryKeyColumnSchema>(table.PrimaryKey).Return(pk);
		//	Expect.Call<ForeignKeyColumnSchemaList>(dbprovider.GetForeignKeys(table)).Return(null).Message("expect no foreign keys");
		//	Expect.Call<ColumnSchemaList>(dbprovider.GetColumnSchemas(table)).Return(null).Message("expect no column schemas");

		//	//create the active record generator and generate a namespace from the mock table definition
		//	NameProvider nameProvider = new NameProvider();
		//	nameProvider.SkipTablePrefixes.Add("hyperblog_");
		//	nameProvider.SkipTablePrefixes.Add("d_");
		//	BasicActiveRecordGenerator argen = new BasicActiveRecordGenerator();
		//	argen.NameProvider = nameProvider;
		//	//replay everything so we can start doing shit
		//	mocks.ReplayAll();

		//	//generate the code model, verify generated output
		//	NamespaceDeclaration nsdecl = argen.CreateClassDeclaration("HyperBlog.Data", table).Namespace;
		//	using (DomTester dom = new DomTester(nsdecl))
  //          {
		//		Assert.IsTrue(dom.ContainsType("HyperBlog.Data.Post"));
		//		Assert.IsTrue(dom.ContainsProperty("HyperBlog.Data.Post", "Id"));
		//		Assert.IsTrue(dom.Type("HyperBlog.Data.Post").InheritsFrom("ActiveRecordBase`1", "Post"));
  //          }
		//}
	}
}
