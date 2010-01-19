using System;
using System.Collections.Generic;
using System.Text;

namespace HyperActive.Core
{
	/// <summary>
	/// A generic exception that occurs within HyperActive.
	/// </summary>
	public class HyperActiveException : System.ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the HyperActiveException class.
		/// </summary>
		/// <param name="message">The message of the exception.</param>
		/// <param name="args">Any format args for the message.</param>
		public HyperActiveException(string message, params object[] args)
			: base(String.Format(message, args))
		{
		}
        /// <summary>
        /// Initializes a new instance of the HyperActiveException class.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <param name="args">Any format args for the message.</param>
        public HyperActiveException(System.Exception innerException, string message, params object[] args)
            : base(String.Format(message, args), innerException)
        {
        }
	}
}
