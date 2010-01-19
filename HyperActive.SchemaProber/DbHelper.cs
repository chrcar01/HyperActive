using System;
using System.Data;
using System.Data.SqlClient;

namespace HyperActive.SchemaProber
{

	/// <summary>
	/// Helper class that wraps some common ADO.NET activities.  Implements the IDbHelper interface.
	/// </summary>
	public class DbHelper : IDbHelper
	{
		private string _connectionString;
		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}
		/// <summary>
		/// Initializes a new instance of the DbHelper class.
		/// </summary>
		/// <param name="connectionString"></param>
		public DbHelper(string connectionString)
		{
			_connectionString = connectionString;
		}

		/// <summary>
		/// Gets an instance of a SqlConnection
		/// </summary>
		/// <param name="initialConnectionState"></param>
		/// <returns></returns>
		public IDbConnection GetConnection(InitialConnectionStates initialConnectionState)
		{
			SqlConnection result = new SqlConnection(this.ConnectionString);
			if (initialConnectionState == InitialConnectionStates.Open)
				result.Open();
			return result;
		}
		/// <summary>
		/// Executes the commandText and returns a SqlDataReader.
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns></returns>
		public IDataReader ExecuteReader(string commandText)
		{
			SqlCommand cmd = new SqlCommand(commandText);
			return ExecuteReader(cmd);
		}
		/// <summary>
		/// Executes the SqlCommand and returns a ready to go SqlDataReader.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public IDataReader ExecuteReader(IDbCommand command)
		{
			command.Connection = this.GetConnection(InitialConnectionStates.Open);
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}
	}
}
