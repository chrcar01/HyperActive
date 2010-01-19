using System;
using NUnit.Framework;
using System.IO;

namespace HyperActive.ConsoleUI.Tests
{
	[TestFixture]
	public class DriverTests
	{
		[Test]
		public void TestRealConfig()
		{
			string configFilePath = @"C:\svn\Phantasm\Phantasm.Data\hyperactive.config";
			string contentsOfConfigFile = File.ReadAllText(configFilePath);
			Driver driver = new Driver(@"C:\svn\Phantasm\libs", contentsOfConfigFile, configFilePath);
			driver.Run();
		}
	}
}