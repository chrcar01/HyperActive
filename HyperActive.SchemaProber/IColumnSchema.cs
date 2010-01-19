using System;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Implement this interface to represent a column in a database.
	/// </summary>
	public interface IColumnSchema
	{
		/// <summary>
		/// Gets the length of the column
		/// </summary>
		int Length { get; }
		/// <summary>
		/// Gets a value indicating whether a column can contain nulls.  It's looking for the 'is_nullable'
		/// key in the props.
		/// </summary>
		bool Nullable { get; }
		/// <summary>
		/// Gets a value indicating whether this column is a primary key.  It depends on a boolean value
		/// in the props with the key 'is_primary_key'.
		/// </summary>
		bool IsPrimaryKey { get; }
		/// <summary>
		/// Gets a value indicating whether this column is a foreign key.  It depends on a boolean value
		/// in the props with the key 'is_foreign_key'.
		/// </summary>
		bool IsForeignKey { get; }
		/// <summary>
		/// Gets the .NET datatype for this column.
		/// </summary>
		Type DataType { get; }
		/// <summary>
		/// gets the name of the column.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the TableSchema containing this column.
		/// </summary>
		TableSchema Table { get; }
	}
}
