using System;
using System.Runtime.Serialization;

namespace HyperActive.SchemaProber
{
	/// <summary>
	/// Represents an exception that occurs in the SchemaProber
	/// </summary>
	[Serializable]
	public class SchemaProberException : Exception
	{
		// constructors...
		#region SchemaProberException()
		/// <summary>
		/// Constructs a new SchemaProberException.
		/// </summary>
		public SchemaProberException() { }
		#endregion
		#region SchemaProberException(string message)
		/// <summary>
		/// Constructs a new SchemaProberException.
		/// </summary>
		/// <param name="message">The exception message</param>
		public SchemaProberException(string message) : base(message) { }
		#endregion
		#region SchemaProberException(string message, Exception innerException)
		/// <summary>
		/// Constructs a new SchemaProberException.
		/// </summary>
		/// <param name="message">The exception message</param>
		/// <param name="innerException">The inner exception</param>
		public SchemaProberException(string message, Exception innerException) : base(message, innerException) { }
		#endregion
		#region SchemaProberException(SerializationInfo info, StreamingContext context)
		/// <summary>
		/// Serialization constructor.
		/// </summary>
		protected SchemaProberException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}
}
