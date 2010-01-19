using System;
using System.Linq;
using HyperActive.Core;

namespace HyperActive.Core
{
	/// <summary>
	/// Utility class to work with objects.
	/// </summary>
	public class Try
	{
		/// <summary>
		/// Returns first non null result.
		/// </summary>
		/// <typeparam name="T">Type to return.</typeparam>
		/// <param name="functions">List of functions to execute.</param>
		/// <returns>First non-null result from the list of functions.</returns>
		public static T These<T>(params Func<T>[] functions)
		{
			T result = default(T);
			foreach (Func<T> function in functions)
			{
				try
				{
					result = function();
					if (result != null)
					{
						return result;
					}
				}
				catch { }
			}
			return result;
		}
	}
}
