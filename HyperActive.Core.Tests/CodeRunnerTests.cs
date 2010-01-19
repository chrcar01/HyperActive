using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HyperActive.Core.Generators;
using HyperActive.SchemaProber;
using HyperActive.Core.Config;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class CodeRunnerTests
	{
		[Test]
		public void Test()
		{
			var cfg = new CodeRunnerConfig();
			var configOptions = new DefaultConfigurationOptions();
			configOptions.EnumNamespace = "The.Enum.Namespace";
			var enumList = new List<EnumDescriptor>();
			enumList.Add(new EnumDescriptor { TableName = "TransactionLineType", NameField = "Label", ValueField = "ID" });
			configOptions.Enums = enumList;

			configOptions.ConnectionString = "user id=sa;password=anja8247;server=(local);database=Lars";
			configOptions.OutputPath = @"C:\dev";
			configOptions.EnumOutputPath = @"C:\dev";
			cfg.DbProvider = new SqlServerProvider(new DbHelper(configOptions.ConnectionString));
			cfg.Generator = new ConfigurableActiveRecordGenerator(configOptions);
			cfg.NameProvider = new NameProvider();
			cfg.Options = configOptions;
			cfg.Writer = (s) => { return Console.Out; };
			var codeRunner = new CodeRunnerImpl(cfg);
			codeRunner.Execute();
		}
	}
}
