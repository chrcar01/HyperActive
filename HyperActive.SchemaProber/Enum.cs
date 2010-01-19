using System;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Utility class used for grabbing an enum value from a string
	/// </summary>
	/// <typeparam name="T">The type of enum.</typeparam>
	public static class Enum<T>
	{
		private const bool IGNORE_CASE = true;

		/// <summary>
		/// Extracts an enum value based on a string value.
		/// </summary>
		/// <param name="value">Value to parse.</param>
		/// <returns></returns>
		public static T Parse(string value)
		{
			return (T)Enum.Parse(typeof(T), value, IGNORE_CASE);
		}
	}
}
