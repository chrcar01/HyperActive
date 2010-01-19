using System;
using System.Collections.Generic;
using System.Text;
using HyperActive.Dominator;
using HyperActive.SchemaProber;
using System.IO;

namespace HyperActive.Core
{
	/// <summary>
	/// Implementation of IValueProvider.
	/// </summary>
	public class ValueProviderImpl : IValueProvider
	{
		/// <summary>
		/// Gets a value that can be assigned to the target column.
		/// </summary>
		/// <param name="column">The column for which the value is generated.</param>
		/// <returns>A value that can be assigned to a property representing the column.</returns>
		public string GetValue(ColumnSchema column)
		{
			if (column == null)
				throw new ArgumentNullException("column");

			string result = null;
			switch (column.DataType.FullName)
			{
				case "System.String":
					int length = column.Length > 1000 || column.Length < 0 ? 1000 : column.Length;
					result = String.Format("\"{0}\"", new String('Z', length));
					break;
				case "System.DateTime":
					result = "new DateTime(1970, 6, 18)";
					break;
				case "System.Boolean":
					result = "true";
					break;
				case "System.Decimal":
					result = "666M";
					break;
				case "System.Int32":
				case "System.Int64":
					result = "123";
					break;
				case "System.Byte[]":
					result = "System.Text.ASCIIEncoding.ASCII.GetBytes(\"GodHatesUsAll\")";
					break;
				case "System.Guid":
					result = "System.Guid.NewGuid()";
					break;
				default:
					throw new InvalidOperationException(String.Format("Unable to generate a value for type of '{0}'", column.DataType.FullName));
			}
			return result;
		}
	}
}
