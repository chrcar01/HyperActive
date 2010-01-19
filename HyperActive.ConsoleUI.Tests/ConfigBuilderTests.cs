	using System;
using NUnit.Framework;

namespace HyperActive.ConsoleUI.Tests
{

    [TestFixture]
	public class ConfigBuilderTests
	{
		[Test]
        public void CanGenXmlForSection()
        {
        	Section section = new Section
			{
				OutputPath = @"C:\output-code-gen",
				Namespace = "Awish.Security.Data",
				ConnectionString = "user id=sa;password=anja8247;database=AwishSecurity;server=maximus;",
				Namer = "HyperActive.Core.NameProvider, HyperActive",
				Generator = "HyperActive.Core.Generators.BasicActiveRecordGenerator, HyperActive"
			};
			//Console.WriteLine(section.ToString().Replace("\"", "\\\""));
			Assert.AreEqual("<config><add key=\"namespace\" value=\"Awish.Security.Data\"></add><add key=\"namer\" value=\"HyperActive.Core.NameProvider, HyperActive\"></add><add key=\"connectionstring\" value=\"user id=sa;password=anja8247;database=AwishSecurity;server=maximus;\"></add><add key=\"generator\" value=\"HyperActive.Core.Generators.BasicActiveRecordGenerator, HyperActive\"></add><add key=\"outputpath\" value=\"C:\\output-code-gen\"></add></config>", section.ToString());
        }
		[Test]
		public void CanAddSections()
		{
			Config config = new Config();
			config.AddSection(new Section());
			config.AddSection(new Section());
			Assert.AreEqual(2, config.Sections.Count);
			//Console.WriteLine(config.ToString().Replace("\"", "\\\""));
			Assert.AreEqual("<hyperactive><config></config><config></config></hyperactive>", config.ToString());
		}
	}
}
