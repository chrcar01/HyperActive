using System;
using System.Configuration;
using NUnit.Framework;

namespace HyperActive.SchemaProber.Tests
{

	[TestFixture]
	public abstract class BaseSchemaProberTest
	{
		protected string ConnectionString
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["HyperActive"].ConnectionString;
			}
		}

		protected IDbHelper Helper
		{
			get
			{
				return new DbHelper(this.ConnectionString);
			}
		}

		protected IDbProvider DbProvider
		{
			get
			{
				return new SqlServerProvider(this.Helper);
			}
		}
	}
}
