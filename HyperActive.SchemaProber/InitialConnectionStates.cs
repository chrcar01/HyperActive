using System;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Initial states of a database connection.
	/// </summary>
	public enum InitialConnectionStates
	{
		/// <summary>
		/// Connection should be opened.
		/// </summary>
		Open,
		/// <summary>
		/// Connection should be closed.
		/// </summary>
		Closed
	}
}
