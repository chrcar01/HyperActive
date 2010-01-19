using HyperActive.Core;
using HyperActive.Core.Config;
using HyperActive.Core.Container;
using HyperActive.Core.Generators;
using HyperActive.SchemaProber;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
namespace HyperActive.ConsoleUI
{
	
	/// <summary>
	/// Main console application.
	/// </summary>
	public class Driver
	{
		private static void WriteHeader()
		{
			Console.WriteLine("HyperActive {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}
		private static bool GenerateConfig(Arguments arguments)
		{
			if (!arguments.Parameters.ContainsKey("gc"))
				return false;

			string filePath = Path.Combine(Environment.CurrentDirectory, "hyperactive.config");
			if (File.Exists(filePath))
				File.Delete(filePath);
			using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.CreateNew)))
			using (StreamReader reader = new StreamReader(Assembly.GetAssembly(typeof(ActiveRecordGenerator)).GetManifestResourceStream("HyperActive.Core.hyperactive.config")))
			{
				writer.Write(reader.ReadToEnd());
			}
			return true;
		}
		private static string GetConfigFilePath(Arguments arguments)
		{
			string configFilePath = arguments.Parameters["config"];
			if (String.IsNullOrEmpty(configFilePath))
			{
				string message = "";
				using (StringWriter writer = new StringWriter())
				{
					writer.WriteLine("Path to the hyperactive config is required.");
					message = writer.ToString();
				}
				throw new ArgumentNullException("configFilePath", message);
			}

			if (!File.Exists(configFilePath))
				throw new FileNotFoundException("config file was not found", configFilePath);
			return configFilePath;
		}
		/// <summary>
		/// Main entrypoint for the application.
		/// </summary>
		/// <param name="args">CommandLine arguments.</param>
		public static void Main(string[] args)
		{
			WriteHeader();
			try
			{
				Arguments arguments = new Arguments(args);
				if (!GenerateConfig(arguments))
				{
					string configFilePath = GetConfigFilePath(arguments);
					string assemblyDirectory = arguments.Parameters["assemblyDirectory"] ?? arguments.Parameters["ad"] ?? "";
					Driver driver = new Driver(assemblyDirectory, File.ReadAllText(configFilePath), configFilePath);
					driver.Log = new ConsoleLogger();
					driver.Run();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			Console.WriteLine("Done...");
		}

		private ILogger _log;
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
        private string _assemblyDirectory = "";
		private string _contentsOfConfigFile = "";
		private string _configDirectory;

		/// <summary>
		/// Initializes a new instance of the <see cref="Driver"></see> class.
		/// </summary>
		/// <param name="xmlconfig">The contents of the hyperactive config file.</param>
		public Driver(string xmlconfig)
			: this(String.Empty, xmlconfig, String.Empty)
		{
		}
        /// <summary>
		/// Initializes a new instance of the <see cref="Driver"/> class.
		/// </summary>
		/// <param name="assemblyDirectory">Directory that the IOC should search for component types.</param>
		/// <param name="contentsOfConfigFile">The contents of the hyperactive config file.</param>
		/// <param name="configFilePath">Path to the hyperactive config file.</param>
		public Driver(string assemblyDirectory, string contentsOfConfigFile, string configFilePath)
		{
			_assemblyDirectory = assemblyDirectory;
			_contentsOfConfigFile = contentsOfConfigFile;
			_configDirectory = new FileInfo(configFilePath).Directory.FullName;
		}


		private void EnsureEnumOutputPath(IConfigurationOptions config)
		{
			if (config.Enums.Count() == 0)
				return;

			Log.Write("Ensuring enum output path... ");
			string enumOutputPath = config.EnumOutputPath;
			if (!Path.IsPathRooted(enumOutputPath))
			{
				enumOutputPath = Path.Combine(_configDirectory, enumOutputPath);
			}
			if (!Directory.Exists(enumOutputPath))
			{
				Directory.CreateDirectory(enumOutputPath);
			}
			
			config.EnumOutputPath = enumOutputPath;			
			Log.WriteLine(config.EnumOutputPath);
		}

		private void EnsureOutputPath(IConfigurationOptions config)
		{
			Log.Write("Ensuring output path... ");
			string outputPath = config.OutputPath;
			if (!Path.IsPathRooted(outputPath))
			{
				outputPath = Path.Combine(_configDirectory, outputPath);
			}
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}
			config.OutputPath = outputPath;
			Log.WriteLine(config.OutputPath);
		}

		private NameProvider InitializeNameProvider(IConfigurationOptions config, Ioc ioc)
		{
			Log.WriteLine("Initializing NameProvider");
			var nameProvider = ioc.Get<NameProvider>() ?? new NameProvider();
			nameProvider.TablePrefixes.AddRange(config.OnlyTablesWithPrefix);
			nameProvider.TablePrefixes.AddRange(config.SkipTablesWithPrefix);
			nameProvider.EnumReplacements = config.EnumReplacements;
			return nameProvider;
		}
		/// <summary>
		/// Responsible for executing a <see cref="ICodeRunner"/> implementation for
		/// each of the configuration sections specified in the hyperactive config file.
		/// </summary>
		public void Run()
		{
            
			var configParser = new DefaultConfigParser();
			var configs = configParser.ParseXml(_contentsOfConfigFile);

			foreach (IConfigurationOptions config in configs)
			{
				EnsureOutputPath(config);
				EnsureEnumOutputPath(config);

                var assemblyDirectory = String.IsNullOrEmpty(config.AssemblyDirectory) ? _assemblyDirectory : config.AssemblyDirectory;
                Log.WriteLine("AssemblyDirectory: {0}", assemblyDirectory);

                var ioc = new Ioc(assemblyDirectory, config.Components, true);

				var cfg = new CodeRunnerConfig();
				cfg.DbProvider = ioc.Get<IDbProvider>();
				if (cfg.DbProvider == null)
				{
					if (String.IsNullOrEmpty(config.ConnectionString))
					{
						Log.WriteLine("It seems as though no DbProvider component was specified in the config and no connectionstring was specified.");
						Log.WriteLine("Either define <add key=\"connectionstring\" value=\"your connection string\" /> at the root of the config node");
						Log.WriteLine("or define a DbProvider component in the components section.");
						continue;
					}
					cfg.DbProvider = new SqlServerProvider(new DbHelper(config.ConnectionString));
				}
				Log.WriteLine("DbProvider -> {0}", cfg.DbProvider.GetType().Name);
				cfg.Generator = ioc.Get<ActiveRecordGenerator>() ?? new ConfigurableActiveRecordGenerator(config);
				if (cfg.Generator.ConfigOptions == null)
				{
					cfg.Generator.ConfigOptions = config;
				}
				Log.WriteLine("Generator -> {0}", cfg.Generator.GetType().Name);

				cfg.NameProvider = InitializeNameProvider(config, ioc);
				cfg.Generator.NameProvider = cfg.NameProvider;

				cfg.Options = config;
				cfg.Writer =
					(path) =>
					{
						return new StreamWriter(path, false);
					};
				var codeRunner = new CodeRunnerImpl(cfg);
				codeRunner.VerboseLogging = true;
				codeRunner.Execute();
			}
		}
	}
}