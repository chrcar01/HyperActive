using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperActive.Core
{

	public interface ILogger
	{
		void Write(string text, params object[] args);
		void WriteLine(string text, params object[] args);
	}
}
