using HyperActive.Core.Container;
using NUnit.Framework;
using System;
using System.Linq;
using HyperActive.Core.Config;

namespace HyperActive.Core.Tests
{
	[TestFixture]
	public class ContainerTests
	{
		[Test]
		public void Test()
		{
			var configs = new DefaultConfigParser().ParseFile(@"C:\svn\AwishWork\Awish.Lars\trunk\Awish.Lars.Data\hyperactive.config");
			Assert.AreEqual(1, configs.Count());
			var configOptions = configs.ElementAt(0);
			Assert.AreEqual(1, configOptions.Components.Count());
			var ioc = new Ioc(@"C:\svn\AwishWork\lib", configOptions.Components);
			var type = ioc.ResolveType("Awish.Common.ActiveRecordExtensions.AwishActiveRecordGenerator, Awish.Common.ActiveRecordExtensions");
			Assert.IsNotNull(type);
		}
	}
}
