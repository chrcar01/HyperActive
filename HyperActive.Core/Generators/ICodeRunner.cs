using System;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// Orchestrates the generation of active record classes.
	/// </summary>
	public interface ICodeRunner
	{
		/// <summary>
		/// Executes this instance.
		/// </summary>
		void Execute();
	}

}
