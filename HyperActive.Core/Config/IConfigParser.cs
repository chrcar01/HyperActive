using System;
using System.Collections.Generic;

namespace HyperActive.Core.Config
{
	public interface IConfigParser
	{
		IEnumerable<IConfigurationOptions> ParseXml(string contentsOfConfigFile);
		IEnumerable<IConfigurationOptions> ParseFile(string pathToConfigFile);
	}
}
