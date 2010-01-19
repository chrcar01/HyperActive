/*
* Arguments class: application arguments interpreter
*
* Authors:		R. LOPES
* Contributors:	R. LOPES
* Created:		25 October 2002
* Modified:		28 October 2002
*
* Version:		1.0
*/

using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace HyperActive.Core.Config {
	/// <summary>
	/// Arguments class
	/// </summary>
	public class Arguments{
		
		private StringDictionary _parameters;

		/// <summary>
		/// Gets the Parameters parsed from the command line.
		/// </summary>
		public StringDictionary Parameters
		{
			get
			{
				return _parameters;
			}
		}
		/// <summary>
		/// Initializes a new instance of the Arguments class.
		/// </summary>
		/// <param name="args">Command line args</param>
		public Arguments(string[] args)
		{
			_parameters = new StringDictionary();
			Regex Spliter=new Regex(@"^-{1,2}|^/|=|:",RegexOptions.IgnoreCase|RegexOptions.Compiled);
			Regex Remover= new Regex(@"^['""]?(.*?)['""]?$",RegexOptions.IgnoreCase|RegexOptions.Compiled);
			string Parameter=null;
			string[] Parts;

			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
			foreach(string Txt in args){
				// Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
				Parts=Spliter.Split(Txt,3);
				switch(Parts.Length){
					// Found a value (for the last parameter found (space separator))
					case 1:
						if(Parameter!=null){
							if(!_parameters.ContainsKey(Parameter)){
								Parts[0]=Remover.Replace(Parts[0],"$1");
								_parameters.Add(Parameter,Parts[0]);
								}
							Parameter=null;
							}
						// else Error: no parameter waiting for a value (skipped)
						break;
					// Found just a parameter
					case 2:
						// The last parameter is still waiting. With no value, set it to true.
						if(Parameter!=null){
							if(!_parameters.ContainsKey(Parameter)) _parameters.Add(Parameter,"true");
							}
						Parameter=Parts[1];
						break;
					// Parameter with enclosed value
					case 3:
						// The last parameter is still waiting. With no value, set it to true.
						if(Parameter!=null){
							if(!_parameters.ContainsKey(Parameter)) _parameters.Add(Parameter,"true");
							}
						Parameter=Parts[1];
						// Remove possible enclosing characters (",')
						if(!_parameters.ContainsKey(Parameter)){
							Parts[2]=Remover.Replace(Parts[2],"$1");
							_parameters.Add(Parameter,Parts[2]);
							}
						Parameter=null;
						break;
					}
				}
			// In case a parameter is still waiting
			if(Parameter!=null){
				if(!_parameters.ContainsKey(Parameter)) _parameters.Add(Parameter,"true");
				}
			}


			/// <summary>
			/// Gets the <see cref="System.String"/> with the specified param.
			/// </summary>
			/// <value></value>
		public string this [string Param]{
			get{
				return(_parameters[Param]);
				}
			}
		}
	}
