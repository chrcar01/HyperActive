using System;
using System.Data;
using System.Data.SqlServerCe;

namespace HyperActive.SchemaProber.SqlCeExtensions
{
	/// <summary>
	/// Helper methods for sql server ce
	/// </summary>
	public class SqlCeDbHelper : IDbHelper
	{
		private string _connectionString;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeDbHelper"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public SqlCeDbHelper(string connectionString)
		{
			_connectionString = connectionString;
		}

		/// <summary>
		/// Gets a SqlConnection.
		/// </summary>
		/// <param name="initialConnectionState"></param>
		/// <returns></returns>
		public System.Data.IDbConnection GetConnection(InitialConnectionStates initialConnectionState)
		{
			System.Data.IDbConnection result = new SqlCeConnection(this.ConnectionString);
			if (initialConnectionState == InitialConnectionStates.Open)
				result.Open();			
			return result;
		}

		/// <summary>
		/// Gets the ConnectionString that implementors of this interface use for connecting to the database.
		/// </summary>
		/// <value></value>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}

		/// <summary>
		/// Executes the commandText and returns a SqlDataReader
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns></returns>
		public System.Data.IDataReader ExecuteReader(string commandText)
		{
			IDbCommand cmd = new SqlCeCommand(commandText);
			return ExecuteReader(cmd);
		}

		/// <summary>
		/// Executes the SqlCommand and returns a ready to go instance of a SqlDataReader.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public System.Data.IDataReader ExecuteReader(System.Data.IDbCommand command)
		{
			command.Connection = this.GetConnection(InitialConnectionStates.Open);
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}
	}
}
