using System;
using System.Data.SqlClient;
using System.Data;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Utility interface for executing SqlServer specific methods
	/// </summary>
	public interface IDbHelper
	{
		/// <summary>
		/// Gets a SqlConnection.
		/// </summary>
		/// <param name="initialConnectionState"></param>
		/// <returns></returns>
		IDbConnection GetConnection(InitialConnectionStates initialConnectionState);
		/// <summary>
		/// Gets the ConnectionString that implementors of this interface use for connecting to the database.
		/// </summary>
		string ConnectionString { get; }
		/// <summary>
		/// Executes the commandText and returns a SqlDataReader
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns></returns>
		IDataReader ExecuteReader(string commandText);
		/// <summary>
		/// Executes the SqlCommand and returns a ready to go instance of a SqlDataReader.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		IDataReader ExecuteReader(IDbCommand command);
	}
}
