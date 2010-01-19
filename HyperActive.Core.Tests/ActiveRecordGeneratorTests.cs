using NUnit.Framework;
using HyperActive.SchemaProber;
using Rhino.Mocks;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using HyperActive.Dominator;
using HyperActive.Core.Generators;
using System;

namespace HyperActive.Core.Tests
{
	//[TestFixture]
	//public class ActiveRecordGeneratorTests
	//{
	//	[Test]
	//	public void Test()
	//	{
	//		IDbHelper helper = new DbHelper("user id=sa;password=anja8247;server=(local);database=pubs;");
	//		IDbProvider provider = new SqlServerProvider(helper);
	//		ActiveRecordGenerator generator = new SimpleActiveRecordGenerator();
	//		generator.NameProvider = new NameProvider();
	//		generator.BaseTypeName = "Castle.ActiveRecord.ActiveRecordValidationnBase";
	//		foreach (TableSchema table in provider.GetTableSchemas())
	//		{
	//			if (table.PrimaryKey == null)
	//				continue;

	//			generator.Generate(Console.Out, "My.Data", table);
	//		}
	//	}
	//	[Test]
	//	public void CanGenTypeInheritingFromAbstractBase()
	//	{
	//		NamespaceDeclaration nsdecl = new NamespaceDeclaration("Awish.Auction.Data");
	//		//create the abstract type first
	//		ClassDeclaration abstractType = new ClassDeclaration(nsdecl, "PlatBookBase", "T")
	//			.InheritsFrom(new ClassDeclaration("Castle.ActiveRecord.ActiveRecordValidationBase", new CodeDomTypeReference("T")))
	//			.IsAbstract();
	//		nsdecl.AddClass(abstractType);

	//		ClassDeclaration landparcel = nsdecl.AddClass("LandParcel", "T")
	//			.InheritsFrom(abstractType);

	//		ActiveRecordGenerator gen = new LazyActiveRecordGenerator();
	//		gen.NameProvider = new NameProvider();
	//		gen.BaseTypeName = "Castle.ActiveRecord.ActiveRecordValidationBase";
	//		gen.AbstractBaseName = abstractType.FullName;

	//		//gen.GenerateAbstractType(Console.Out, "Awish.Auction.Data", "PlatBookBase");

	//		MockRepository mockery = new MockRepository();
	//		TableSchema table = mockery.StrictMock<TableSchema>();
	//		Expect.Call(table.Name).Return("LandParcel").Repeat.Twice();
	//		Expect.Call(table.ForeignKeys).Return(new ForeignKeyColumnSchemaList());
	//		Expect.Call(table.NonKeyColumns).Return(null);

	//		PrimaryKeyColumnSchema pk = mockery.StrictMock<PrimaryKeyColumnSchema>();
	//		Expect.Call(pk.DataType).Return(typeof(int));
	//		Expect.Call(pk.IsIdentity).Return(true);
	//		Expect.Call(pk.Name).Return("ID");			
	//		

	//		Expect.Call(table.PrimaryKey).Return(pk).Repeat.Times(3);
	//		mockery.ReplayAll();
	//		gen.Generate(Console.Out, "Awish.Auction.Data", table);
	//		mockery.VerifyAll();

	//	}
	//	[Test]
	//	public void CanGenIface()
	//	{
	//		ActiveRecordGenerator gen = new BasicMediatorGenerator();
	//		gen.NameProvider = new NameProvider();
	//		gen.GenerateInterface(Console.Out, "You.Suck", "IBlow");
	//	}
	//	[Test]
	//	public void VerifyDefaults()
	//	{
	//		MockRepository mockery = new MockRepository();
	//		ActiveRecordGenerator generator = mockery.StrictMock<ActiveRecordGenerator>();
	//		mockery.ReplayAll();
	//		Assert.AreEqual(typeof(CSharpCodeProvider), generator.DomProvider.GetType(), "Default DomProvider should be CSharpCodeProvider");
	//		generator.DomProvider = new VBCodeProvider();
	//		Assert.AreEqual(typeof(VBCodeProvider), generator.DomProvider.GetType(), "Should be able to change the provider");
	//		Assert.AreEqual("Castle.ActiveRecord.ActiveRecordBase", generator.BaseTypeName, "default base type should be ActiveRecordBase");
	//		generator.BaseTypeName = "Castle.ActiveRecord.ActiveRecordValidationBase";
	//		Assert.AreEqual("Castle.ActiveRecord.ActiveRecordValidationBase", generator.BaseTypeName, "should be able to change the base type");
	//		mockery.VerifyAll();
	//	}
	//	[Test]
	//	public void CreateConstructorsShouldReturnNullIfNotImplemented()
	//	{
	//		MockRepository mockery = new MockRepository();
	//		ActiveRecordGenerator generator = mockery.StrictMock<ActiveRecordGenerator>();
	//		ClassDeclaration cdecl = null;
	//		TableSchema table = null;
	//		ClassDeclaration expectedResult = null;
	//		Expect.Call<ClassDeclaration>(generator.CreateConstructors(cdecl, table)).Return(expectedResult);
	//		mockery.ReplayAll();
	//		Assert.IsNull(generator.CreateConstructors(cdecl, table), "Default implementation of CreateConstructors is to do nothing and return null");
	//		mockery.VerifyAll();
	//	}

	//	
	//}

}

