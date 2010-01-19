using System;

namespace HyperActive.Core
{
	/// <summary>
	/// Utility class for reading types specified as 'FullTypeName, AssemblyName'.
	/// </summary>
	public class TypeInferer
	{
		private string _typeName;
		/// <summary>
		/// Gets the name of the type.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
		}
		private string _assemblyName;
		/// <summary>
		/// Gets the name of the assembly.
		/// </summary>
		public string AssemblyName
		{
			get
			{
				return _assemblyName;
			}
		}
		/// <summary>
		/// Initializes a new instance of the TypeInferer class.
		/// </summary>
		/// <param name="value">Comma separated full type name and assembly name to infer.</param>
		public TypeInferer(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			string[] values = value.Split(',');
			if (values.Length != 2)
				throw new HyperActiveException("Unable to parse type '{0}'.  The format is 'FullTypeName, AssemblyName'.", value);
			_typeName = values[0];
			_assemblyName = values[1];
		}
	}
}
