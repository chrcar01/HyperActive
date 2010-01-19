using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HtmlBuilder;
using HyperActive.Core.Config;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class ConfigParserTests
	{
		[Test]
		public void can_get_components()
		{
			var contentsOfConfigFile = new Element("hyperactive",
				new Element("config",
					new Element("components",
						new Element("component", "service=TheService;serviceimpl=TheImpl;",
							new Element("param", "name=myParam;type=string;value=$IRock;")))));

			var configParser = new DefaultConfigParser();
			var options = configParser.ParseXml(contentsOfConfigFile).ElementAt(0);
			Assert.AreEqual(1, options.Components.Count());
			var comp = options.Components.ElementAt(0);
			Assert.AreEqual("TheService", comp.ServiceTypeName);
			Assert.AreEqual("TheImpl", comp.ServiceImplementationTypeName);
			Assert.AreEqual(1, comp.Parameters.Count);
			var p = comp.Parameters.ElementAt(0);
			Assert.AreEqual("myParam", p.Name);
			Assert.AreEqual("string", p.Type);
			Assert.AreEqual("$IRock", p.Value);
		}
		[Test]
		public void can_get_enums()
		{
			var contentsOfConfigFile = new Element("hyperactive",
				new Element("config",
					new Element("enums",
						new Element("add", "table=TransactionLineType;nameField=Label;valueField=ID;"))));

			var configParser = new DefaultConfigParser();
			var options = configParser.ParseXml(contentsOfConfigFile).ElementAt(0);
			Assert.AreEqual(1, options.Enums.Count());
			var @enum = options.Enums.ElementAt(0);
			Assert.AreEqual("TransactionLineType", @enum.TableName);
			Assert.AreEqual("Label", @enum.NameField);
			Assert.AreEqual("ID", @enum.ValueField);
			
		}
		[Test]
		public void can_get_key_values()
		{
			var contentsOfConfigFile = new Element("hyperactive",
				new Element("config",
					new Element("add", "key=abstractbasename;value=LarsBase"),
					new Element("add", @"key=outputpath;value=Data\Generated")));
					
					
					
			var configParser = new DefaultConfigParser();
			var configs = configParser.ParseXml(contentsOfConfigFile);
			Assert.AreEqual(1, configs.Count());
			var options = configs.ElementAt(0);
			Assert.AreEqual("LarsBase", options.AbstractBaseName);
			Assert.AreEqual(@"Data\Generated", options.OutputPath);
		}
	}
}
