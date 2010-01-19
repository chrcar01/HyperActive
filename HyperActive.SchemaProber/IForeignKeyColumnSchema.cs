using System;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Implement this interface to represent a foreign key. 
	/// </summary>
	public interface IForeignKeyColumnSchema : IColumnSchema
	{
		/// <summary>
		/// Gets the primary key table that this foreign key originates.
		/// </summary>
		/// <value>The primary key table this foreign key originates.</value>
		TableSchema PrimaryKeyTable { get; }
	}
}
