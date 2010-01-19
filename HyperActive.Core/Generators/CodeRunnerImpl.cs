using HyperActive.Core.Config;
using HyperActive.Core.Generators;
using HyperActive.Dominator;
using HyperActive.SchemaProber;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HyperActive.Core.Generators
{
	public class CodeRunnerImpl : ICodeRunner
	{
		private ILogger _logger;
		public ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					_logger = new ConsoleLogger();
				}
				return _logger;
			}
			set
			{
				_logger = value;
			}
		}
		private bool _verboseLogging;
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
		private void Write(string text, params object[] args)
		{
			if (!this.VerboseLogging)
				return;
			this.Logger.Write(text, args);
		}
		private void WriteLine(string text, params object[] args)
		{
			if (!this.VerboseLogging)
				return;
			this.Logger.WriteLine(text, args);
		}
		private IConfigurationOptions _options;
		private CodeRunnerConfig _codeRunnerConfig;
		public CodeRunnerImpl(CodeRunnerConfig codeRunnerConfig)
		{
			_codeRunnerConfig = codeRunnerConfig;
			_options = _codeRunnerConfig.Options;
		}
		public void Execute()
		{
			_codeRunnerConfig.Generator.DbProvider = _codeRunnerConfig.DbProvider;
			GenerateAbstractBaseType();
			GenerateTables();
			GenerateEnums();
		}
		private void GenerateAbstractBaseType()
		{
			if (String.IsNullOrEmpty(_codeRunnerConfig.Options.AbstractBaseName))
			{
				return;
			}
			Write("Generating abstract base class {0} ... ", _codeRunnerConfig.Options.AbstractBaseName);
			string template = String.Format(@"
namespace {2} 
{{
	public abstract class {0}<T> : {1}<T> where T : class
	{{
	}}
}}", _options.AbstractBaseName, _options.BaseTypeName, _options.Namespace);
			string outputFileName = Path.Combine(_options.OutputPath, _options.AbstractBaseName + ".cs");
			using (TextWriter writer = _codeRunnerConfig.Writer(outputFileName))
			{
				writer.Write(template);
			}
			WriteLine("Done");
		}

		private TableSchemaList GetTableSchemas()
		{
			WriteLine("Retrieving table schemas");
			TableSchemaList tableSchemas = null;
			if (_options.OnlyTablesWithPrefix.Count == 0)
			{
				WriteLine("TablesWithPrefix was empty.");
				tableSchemas = _codeRunnerConfig.DbProvider.GetTableSchemas();
			}
			else
			{
				tableSchemas = new TableSchemaList();
				foreach (string prefix in _options.OnlyTablesWithPrefix)
				{
					WriteLine("Adding tables with prefix '{0}'", prefix);
					tableSchemas.AddRange(_codeRunnerConfig.DbProvider.GetTableSchemas(prefix));
				}
			}
			return tableSchemas;
		}
		private void GenerateTables()
		{
			TableSchemaList tableSchemas = GetTableSchemas();
            foreach (var tableSchema in tableSchemas.OrderBy(tbl => { return tbl.Name; }))
            {
                if (tableSchema.PrimaryKey == null)
                {
                    WriteLine("Skipping table {0} because it does not have a primary key.", tableSchema.Name);
                    continue;
                }
                if (_options.SkipTablesWithPrefix.Count > 0 && _options.SkipTablesWithPrefix.Contains(tableSchema.Name))
                {
                    WriteLine("Skipping table {0} because it's in the skiptableswithprefix list.", tableSchema.Name);
                    continue;
                }
                if (_options.SkipTables.Count > 0 && _options.SkipTables.Contains(tableSchema.Name))
                {
                    WriteLine("Skipping table {0} because it's in the skiptables list.", tableSchema.Name);
                    continue;
                }
                var ns = new NamespaceDeclaration(_options.Namespace);
                var classDecl = _codeRunnerConfig.Generator.Generate(ns, tableSchema);
                string outputFileName = Path.Combine(_options.OutputPath, _codeRunnerConfig.NameProvider.GetClassName(tableSchema) + ".cs");
                using (TextWriter writer = _codeRunnerConfig.Writer(outputFileName))
                {
                    Write("Generating {0} ... ", tableSchema.Name);
                    if (_codeRunnerConfig.Options.UseMicrosoftsHeader)
                    {
                        new CodeBuilder().GenerateCode(writer, ns);
                    }
                    else
                    {
                        writer.Write(UseHyperActiveHeader(ns));

                    }
                    WriteLine("Done");
                }
            }
		}

		private void GenerateEnums()
		{
			if (_options.Enums == null || _options.Enums.Count() == 0)
			{
				return;
			}

			var generator = new EnumGenerator(_codeRunnerConfig.DbProvider, _codeRunnerConfig.Options, _codeRunnerConfig.NameProvider);
			foreach (var enumDescriptor in _options.Enums)
			{
				Write("Generating enum {0} ... ", enumDescriptor.TableName);
				var @enum = generator.Generate(enumDescriptor.TableName, enumDescriptor.NameField, enumDescriptor.ValueField);
				if (@enum == null)
				{
					WriteLine("Failed.  Make sure {0} is spelled correctly and exists in the database.", enumDescriptor.TableName);
					continue;
				}
				var ns = new NamespaceDeclaration(_options.EnumNamespace);
				ns.AddClass(@enum);

				string fileName = Inflector.Pluralize(_codeRunnerConfig.NameProvider.GetClassName(enumDescriptor.TableName)) + ".cs";
				string outputFilePath = Path.Combine(_options.EnumOutputPath, fileName);
				using (TextWriter writer = _codeRunnerConfig.Writer(outputFilePath))
				{
					if (_options.UseMicrosoftsHeader)
					{
						new CodeBuilder().GenerateCode(writer, ns);
					}
					else
					{
						writer.Write(UseHyperActiveHeader(ns));
					}
				}
				WriteLine("Done.");
			}
		}

		private string UseHyperActiveHeader(NamespaceDeclaration ns)
		{
			string source = "";

			// Generate source to a string for post parsing.
			string tempSource = String.Empty;
			using (StringWriter tempWriter = new StringWriter())
			{
				new CodeBuilder().GenerateCode(tempWriter, ns);
				tempSource = tempWriter.ToString();
			}

			// Now while walking through each line of the file, intercept where
			// there is Microsoft header shite and put in HyperActive header shite.
			using (StringReader reader = new StringReader(tempSource))
			using (StringWriter tempWriter = new StringWriter())
			{
				bool insideHeader = false;
				bool hyperActiveMessageWritten = false;
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line == "// </auto-generated>")
					{
						insideHeader = false;
					}
					if (!insideHeader)
					{
						tempWriter.WriteLine(line);
					}
					else if (!hyperActiveMessageWritten)
					{
						tempWriter.WriteLine("// HyperActive {0} generated this file.", Assembly.GetExecutingAssembly().GetName().Version.ToString());						
						hyperActiveMessageWritten = true;
					}
					if (line.Trim() == "// <auto-generated>")
					{
						insideHeader = true;
					}
				}
				source = tempWriter.ToString();
			}
			return source;
		}
		

	}
}
