using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Configuration;
using System.Data;

namespace HyperActive.SchemaProber.Tests
{
	[TestFixture]
	public class DbHelperTests
	{
		private readonly string _connectionString = ConfigurationManager.ConnectionStrings["schemaprobertests"].ConnectionString;

		[Test]
		public void can_get_connection_string()
		{
			var dbhelper = new DbHelper(_connectionString);
			Assert.AreEqual(_connectionString, dbhelper.ConnectionString);
		}

		[Test]
		public void can_get_open_connection()
		{
			using (var cn = new DbHelper(_connectionString).GetConnection(InitialConnectionStates.Open))
			{
				Assert.AreEqual(ConnectionState.Open, cn.State);
			}
		}

		[Test]
		public void can_get_closed_connection()
		{
			using (var cn = new DbHelper(_connectionString).GetConnection(InitialConnectionStates.Closed))
			{
				Assert.AreEqual(ConnectionState.Closed, cn.State);
			}
		}

		[Test]
		public void can_get_reader_from_sql_text()
		{
			var dbhelper = new DbHelper(_connectionString);
			int count = 0;
			using (var reader = dbhelper.ExecuteReader("select id, label from addresstype"))
			{
				while (reader.Read())
				{
					count++;
				}
			}
			Assert.AreEqual(2, count);
		}

	}
}
