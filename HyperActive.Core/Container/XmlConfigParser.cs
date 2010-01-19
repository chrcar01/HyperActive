using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Xml.XPath;
using System.Threading;

namespace HyperActive.Core.Container
{
	
	/// <summary>
	/// Responsible for parsing the contents of a hyperactive config file.
	/// </summary>
	public class XmlConfigParser
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigParser"></see> class.
		/// </summary>
		/// <param name="components">The components.</param>
		public XmlConfigParser(XPathNodeIterator components)
			: this(components, String.Empty)
		{
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigParser"/> class.
		/// </summary>
		/// <param name="components">The components.</param>
		/// <param name="assemblyDirectory">The assembly directory.</param>
		public XmlConfigParser(XPathNodeIterator components, string assemblyDirectory)
		{
			this.AssemblyDirectory = assemblyDirectory;
			this.LoadComponents(components);
		}


		/// <summary>
		/// Optional.  Gets or sets the directory where assembly dependecies are located.
		/// </summary>
		/// <value>The assembly directory.</value>
		public string AssemblyDirectory
		{
			get;
			set;
		}

		private Type ResolveType(string typeAndAssembly)
		{
			Type result = null;
			if (String.IsNullOrEmpty(typeAndAssembly))
				return result;

			string[] nameValuePairs = typeAndAssembly.Split(',');
			string typeName = nameValuePairs[0].Trim();
			string assemblyName = nameValuePairs.Length > 1 ? nameValuePairs[1].Trim() : "";

			//this searches for the internal assemblies, the ones loaded
			//when the app starts, and no where else
			foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
			{
				result = ass.GetType(typeName);
				if (result != null)
				{
					break;
				}
			}
			if (result == null && !String.IsNullOrEmpty(assemblyName))
			{
				List<string> paths = new List<string>();
				string assemblyFileName = assemblyName + ".dll";
				if (!String.IsNullOrEmpty(this.AssemblyDirectory) && Directory.Exists(this.AssemblyDirectory))
					paths.Add(Path.Combine(this.AssemblyDirectory, assemblyFileName));
				paths.Add(Path.Combine(Environment.CurrentDirectory, assemblyFileName));
				paths.Add(Path.Combine(Environment.CurrentDirectory, Path.Combine("bin", assemblyFileName)));
				paths.Add(Path.Combine(Environment.CurrentDirectory, Path.Combine(Path.Combine("bin", "debug"), assemblyFileName)));
				paths.Add(Path.Combine(Environment.CurrentDirectory, Path.Combine(Path.Combine("bin", "release"), assemblyFileName)));
				for (int i = 0; i < paths.Count; i++)
				{
					Console.Write("Looking for {0} in {1}...", assemblyFileName, paths[i]);
					if (!File.Exists(paths[i]))
					{
						Console.WriteLine("Fail");
						continue;
					}

					Console.WriteLine("Found it!");
					Assembly ass = Assembly.LoadFile(paths[i]);
					result = ass.GetType(typeName);
					if (result != null)
						break;

				}
			}

			if (result == null)
				throw new TypeLoadException(String.Format("Unable to load type {0}", typeName, assemblyName));

			return result;
		}

		private Type ResolveType(XPathNavigator navigator, string attributeContainingTypeName)
		{
			string typeAndAssembly = navigator.GetAttribute(attributeContainingTypeName, "");
			return ResolveType(typeAndAssembly);
		}
		IDictionary<string, ServiceDetails> _components = null;
		/// <summary>
		/// Gets the components.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, ServiceDetails> GetComponents()
		{
			return _components;
		}
		/// <summary>
		/// Loads the components.
		/// </summary>
		/// <param name="components">The components.</param>
		protected virtual void LoadComponents(XPathNodeIterator components)
		{
			while (components.MoveNext())
			{
				if (_components == null) _components = new Dictionary<string, ServiceDetails>();
				XPathNavigator component = components.Current;
				ServiceDetails details = new ServiceDetails();
				details.Service = this.ResolveType(component, "service");
				details.ServiceImpl = this.ResolveType(component, "serviceimpl") ?? details.Service;
				XPathNodeIterator componentParams = component.Select(".//param");
				while (componentParams.MoveNext())
				{
					if (details.CtorParams == null) details.CtorParams = new List<CtorParameter>();
					XPathNavigator param = componentParams.Current;
					details.CtorParams.Add(new CtorParameter
					{
						Name = param.GetAttribute("name", ""),
						Type = ResolveType(param.GetAttribute("type", "")),
						Value = param.GetAttribute("value", "")
					});
				}
				_components.Add(details.Service.Name, details);
			}
		}
	}
}
