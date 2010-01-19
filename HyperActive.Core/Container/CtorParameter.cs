using System;

namespace HyperActive.Core.Container
{
	/// <summary>
	/// Represents information about a contstructor parameter.
	/// </summary>
	public struct CtorParameter
	{
		/// <summary>
		/// Name of the the constructor parameter.
		/// </summary>
		public string Name;
		/// <summary>
		/// Type of the constuctor parameter.
		/// </summary>
		public Type Type;
		/// <summary>
		/// Value of the constructor parameter.
		/// </summary>
		public object Value;

		/// <summary>
		/// Initializes a new instance of the <see cref="CtorParameter"/> struct.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		public CtorParameter(string name, Type type, object value)
		{
			Name = name;
			Type = type;
			Value = value;
		}
	}
}
