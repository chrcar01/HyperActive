using HyperActive.Core.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HyperActive.Core.Container
{
	/// <summary>
	/// Represents a component container.
	/// </summary>
	public class Ioc
	{
		private string _assemblyDirectory;
		/// <summary>
		/// Gets or sets the fully qualified path to a directory with assemblies that
		/// contain types needed by HyperActive.
		/// </summary>
		/// <value>The directory containing assemblies needed by HyperActive.</value>
		public string AssemblyDirectory
		{
			get
			{
				return _assemblyDirectory;
			}
			set
            {
            	_assemblyDirectory = value;
            }
		}
		private void Write(string text, params object[] args)
		{
			if (!this.VerboseLogging)
				return;
			this.Log.Write(text, args);
		}
		private void WriteLine(string text, params object[] args)
		{
			if (!this.VerboseLogging)
				return;
			this.Log.WriteLine(text, args);
		}
		private bool _verboseLogging;
		/// <summary>
		/// Gets or sets a value indicating whether logging is turned on.
		/// </summary>
		/// <value><c>true</c> if [verbose logging]; otherwise, <c>false</c>.</value>
		public bool VerboseLogging
		{
			get
			{
				return _verboseLogging;
			}
			set
			{
				_verboseLogging = value;
			}
		}        
		private ILogger _log = null;
		public ILogger Log
		{
			get
			{
				if (_log == null)
				{
					_log = new ConsoleLogger();
				}
				return _log;
			}
			set
			{
				_log = value;
			}
		}
		private IEnumerable<ComponentDescriptor> _componentDescriptors;

		public Ioc()
			: this(String.Empty, null)
		{
		}

		public Ioc(string assemblyDirectory, IEnumerable<ComponentDescriptor> componentDescriptors)
			: this(assemblyDirectory, componentDescriptors, false)
		{
		}
        public Ioc(string assemblyDirectory, IEnumerable<ComponentDescriptor> componentDescriptors, bool verboseLogging)
		{
			_verboseLogging = verboseLogging;
			_assemblyDirectory = assemblyDirectory;

			WriteLine("Assembly directory: {0}", _assemblyDirectory);

			_componentDescriptors = componentDescriptors;
			
			if (_componentDescriptors == null || _componentDescriptors.Count() == 0)
				return;

			this.Initialize();
		}

		private List<CtorParameter> CreateCtorParams(object ctorValues)
		{
			List<CtorParameter> result = new List<CtorParameter>();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(ctorValues))
			{
				result.Add(new CtorParameter(property.Name, property.PropertyType, property.GetValue(ctorValues)));
			}
			return result;
		}

		private Dictionary<string, ServiceDetails> _cache = new Dictionary<string, ServiceDetails>();
		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="serviceImpl">The service impl.</param>
		public void Register(Type service, Type serviceImpl)
		{
			object ctorValues = null;
			Register(service, serviceImpl, ctorValues);
		}
		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="serviceImpl">The service impl.</param>
		/// <param name="ctorValues">The ctor values.</param>
		public void Register(Type service, Type serviceImpl, object ctorValues)
		{
			Register(service, serviceImpl, CreateCtorParams(ctorValues));
		}
		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="serviceImpl">The service impl.</param>
		/// <param name="ctorParams">The ctor params.</param>
		public void Register(Type service, Type serviceImpl, List<CtorParameter> ctorParams)
		{
			string componentName = service.Name;
			_cache.Add(componentName, new ServiceDetails(service, serviceImpl, ctorParams));
		}

		/// <summary>
		/// Registers the specified details.
		/// </summary>
		/// <param name="details">The details.</param>
		public void Register(ServiceDetails details)
		{
			Register(details.Service, details.ServiceImpl, details.CtorParams);
		}

		/// <summary>
		/// Registers the specified components.
		/// </summary>
		/// <param name="components">The components.</param>
		public void Register(IDictionary<string, ServiceDetails> components)
		{
			foreach (KeyValuePair<string, ServiceDetails> item in components)
			{
				_cache.Add(item.Key, item.Value);
			}
		}

		/// <summary>
		/// Gets the specified component.
		/// </summary>
		/// <param name="componentName">Name of the component.</param>
		/// <returns></returns>
        public object Get(string componentName)
        {
            if (String.IsNullOrEmpty(componentName))
            {
                WriteLine("componentName was null");
                return null;
            }

            if (_cache == null || _cache.Count == 0)
            {
                WriteLine("_cache was null or empty");
                return null;
            }

            ServiceDetails details = (from item in _cache
                                      where item.Key.Equals(componentName, StringComparison.OrdinalIgnoreCase)
                                      select item.Value).FirstOrDefault();

            if (details.Service == null && details.ServiceImpl == null)
            {
                return null;
            }

            WriteLine("");
            WriteLine("Details of the service being retrieved");
            WriteLine("Service: {0}", details.Service.Name);
            WriteLine("ServiceImpl: {0}", details.ServiceImpl.Name);
            WriteLine("CtorParams #: {0}", details.CtorParams.Count);
            if (details.CtorParams.Count > 0)
            {
                foreach (var cp in details.CtorParams)
                {
                    WriteLine("ctor name: {0}, value: {1}", cp.Name, cp.Value);
                }
            }


            List<object> args = new List<object>();
            if (details.CtorParams != null && details.CtorParams.Count > 0)
            {
                foreach (CtorParameter p in details.CtorParams)
                {
                    if (p.Type == typeof(string) && (p.Value != null && p.Value.ToString().StartsWith("$")))
                    {
                        string dependencyName = p.Value.ToString().Substring(1);//skip the $
                        object inst = Get(dependencyName);
                        args.Add(inst);
                    }
                    else
                        args.Add(p.Value);
                }
            }
            return Activator.CreateInstance(details.ServiceImpl, args.ToArray());;
        }

		/// <summary>
		/// Gets a component instance.
		/// </summary>
		/// <typeparam name="T">Type of the component to retrieve, this is always the service type.</typeparam>
		/// <returns></returns>
		public T Get<T>()
		{			
			return (T)Get(typeof(T).Name);
		}

		private void Initialize()
		{
			foreach (var comp in _componentDescriptors)
			{				
				ServiceDetails details = new ServiceDetails();
				details.Service = this.ResolveType(comp.ServiceTypeName);
				details.ServiceImpl = this.ResolveType(comp.ServiceImplementationTypeName) ?? details.Service;				
				foreach(var p in comp.Parameters)
				{
					if (details.CtorParams == null) details.CtorParams = new List<CtorParameter>();
					details.CtorParams.Add(new CtorParameter
					{
						Name = p.Name,
						Type = ResolveType(p.Type),
						Value = p.Value
					});
				}
				this.Register(details);
			}
		}

		private string GetTypeName(string typeAndAssembly)
		{
			return typeAndAssembly.Split(',')[0].Trim();
		}

		private string GetAssemblyName(string typeAndAssembly)
		{
			string[] pairs = typeAndAssembly.Split(',');
			if (pairs.Length > 1)
				return pairs[1].Trim();
			return String.Empty;
		}

		private Type ResolveInternal(string typeName)
		{
			WriteLine("Trying to find {0} in the assemblies loaded in the current app domain.", typeName);
			Type result = null;
			foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
			{
				Write("Searching assembly {0} ...", ass.GetName().Name);
				result = ass.GetType(typeName);
				if (result != null)
				{
					WriteLine("Found");
					break;
				}
				WriteLine("Not found");
			}
			return result;
		}

		private List<string> GetSearchPaths(string assemblyFileName)
		{
			var result = new List<string>();
			if (!String.IsNullOrEmpty(this.AssemblyDirectory) && Directory.Exists(this.AssemblyDirectory))
				result.Add(Path.Combine(this.AssemblyDirectory, assemblyFileName));
			result.Add(Path.Combine(Environment.CurrentDirectory, assemblyFileName));
			result.Add(Path.Combine(Environment.CurrentDirectory, Path.Combine("bin", assemblyFileName)));
			result.Add(Path.Combine(Environment.CurrentDirectory, Path.Combine(Path.Combine("bin", "debug"), assemblyFileName)));
			result.Add(Path.Combine(Environment.CurrentDirectory, Path.Combine(Path.Combine("bin", "release"), assemblyFileName)));
			return result;
		}

		/// <summary>
		/// Resolves the type.
		/// </summary>
		/// <param name="typeAndAssembly">The type and name of the assembly in the format of 'Your.Type.Is.Cool, This.Is.The.Assembly'.</param>
		/// <returns></returns>
        public Type ResolveType(string typeAndAssembly)
		{
			Type result = null;
			if (String.IsNullOrEmpty(typeAndAssembly))
				return result;

			WriteLine("Trying to resolve type: {0}", typeAndAssembly);

			string typeName = GetTypeName(typeAndAssembly);
			string assemblyName = GetAssemblyName(typeAndAssembly);

			result = ResolveInternal(typeName);
			if (result != null)
			{
				return result;
			}

			var assemblyFileName = assemblyName + ".dll";
			foreach (string searchPath in GetSearchPaths(assemblyFileName))
			{
				Write("Searching for {0} in {1} ", typeName, searchPath);
				if (!File.Exists(searchPath))
				{
					WriteLine("Not Found");
					continue;
				}
				WriteLine("Found It!");
				var ass = Assembly.LoadFrom(searchPath);
				result = ass.GetType(typeName);
				if (result != null)
					break;
			}
			if (result != null)
			{
				WriteLine("Type {0} was found.", result.Name);
			}
			return result;
		}

	}
}
