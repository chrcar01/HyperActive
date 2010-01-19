using System;
using HyperActive.SchemaProber;

namespace HyperActive.Core
{
	/// <summary>
	/// Used when generating values for a column.
	/// </summary>
	public interface IValueProvider
	{
		/// <summary>
		/// Gets a value that will "fit" into a column.
		/// </summary>
		/// <param name="column">The colunm to reference when generating values.</param>
		/// <returns>A value that will "fit" into the the column.</returns>
		string GetValue(ColumnSchema column);
	}
}
