using System;
using System.Linq;
using ConfigEditorUI.Models;
using HyperActive.Core.Config;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ConfigEditorUI.Services
{
	public class ConfigService
	{
		private IEnumerable<IConfigurationOptions> Convert(ObservableCollection<ConfigurationOptionsViewModel> models)
		{
        	var result = new List<IConfigurationOptions>();
			foreach(var model in models)
			{
				var option = new DefaultConfigurationOptions();
				option.AbstractBaseName = model.AbstractBaseName;
				option.AssemblyDirectory = model.AssemblyDirectory;
				option.BaseTypeName = model.BaseTypeName;
				option.ConnectionString = model.ConnectionString;
				option.DataNamespace = model.DataNamespace;
				option.EnumNamespace = model.EnumNamespace;
				option.EnumOutputPath = model.EnumOutputPath;
				option.GenerateColumnList = model.GenerateColumnList;
				option.GenerateComments = model.GenerateComments;
				option.IocVerboseLogging = model.IocVerboseLogging;
				option.Namespace = model.Namespace;				
				option.OnlyTablesWithPrefix.AddRange(model.OnlyTablesWithPrefix);
				option.OutputPath = model.OutputPath;
				option.SkipTables.AddRange(model.SkipTables);
				option.SkipTablesWithPrefix.AddRange(model.SkipTablesWithPrefix);
				option.StaticPrimaryKeyName = model.StaticPrimaryKeyName;
				option.UseMicrosoftsHeader = model.UseMicrosoftsHeader;
				option.Components = model.Components;
				option.EnumReplacements = model.EnumReplacements;
				option.Enums = model.Enums;
				result.Add(option);
            }
			return result;
        }
		private ObservableCollection<ConfigurationOptionsViewModel> Convert(IEnumerable<IConfigurationOptions> options)
		{
        	var result = new ObservableCollection<ConfigurationOptionsViewModel>();
            foreach(var option in options)
			{
            	var model = new ConfigurationOptionsViewModel();
				model.AbstractBaseName = option.AbstractBaseName;
				model.AssemblyDirectory = option.AssemblyDirectory;
				model.BaseTypeName = option.BaseTypeName;				
				model.ConnectionString = option.ConnectionString;
				model.DataNamespace = option.DataNamespace;
				model.EnumNamespace = option.EnumNamespace;
				model.EnumOutputPath = option.EnumOutputPath;
				model.GenerateColumnList = option.GenerateColumnList;
				model.GenerateComments = option.GenerateComments;
				model.IocVerboseLogging = option.IocVerboseLogging;
				model.Namespace = option.Namespace;
				model.OnlyTablesWithPrefix.AddRange(option.OnlyTablesWithPrefix);
				model.OutputPath = option.OutputPath;
				model.SkipTables.AddRange(option.SkipTables);
				model.SkipTablesWithPrefix.AddRange(option.SkipTablesWithPrefix);
				model.StaticPrimaryKeyName = option.StaticPrimaryKeyName;
				model.UseMicrosoftsHeader = option.UseMicrosoftsHeader;
				foreach(var comp in option.Components)
				{
                	model.Components.Add(comp);
                }
				foreach(var item in option.EnumReplacements)
				{
                	model.EnumReplacements.Add(item.Key, item.Value);
                }
				foreach(var item in option.Enums)
				{
                	model.Enums.Add(item);
                }
				result.Add(model);
            }
            return result;
        }
		public EditConfigsViewModel EditConfigs(ProjectWindowViewModel project)
		{
			if (project.IsExistingProject)
				return EditConfigs(project.ConfigFilePath);

			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
			builder.DataSource = project.Server;
			builder.InitialCatalog = project.Database;
			builder.IntegratedSecurity = !project.IsSqlServerSecurity;
			if (project.IsSqlServerSecurity)
			{
				builder.UserID = project.Username;
				builder.Password = project.Password;
			}

			EditConfigsViewModel result = CreateConfig();
			result.Configs[0].ConnectionString = builder.ConnectionString;
			return result;
		}
		public EditConfigsViewModel EditConfigs(string configFilePath)
		{
			var result = new EditConfigsViewModel();
			var configParser = new DefaultConfigParser();
			var options = configParser.ParseXml(File.ReadAllText(configFilePath));
			var models = Convert(options);
			foreach (var model in models)
			{
				var generator = model.Components.FirstOrDefault(x => { return x.ServiceTypeName.Equals("HyperActive.Core.Generators.ActiveRecordGenerator, HyperActive"); });
				if (generator != null && !String.IsNullOrEmpty(generator.ServiceImplementationTypeName))
				{
					model.GeneratorTypeName = generator.ServiceImplementationTypeName;
				}
				result.Configs.Add(model);
			}

			//ensure that each config section has at least one component node which defines the AR generator			
			return result;
		}

		public void Save(EditConfigsViewModel model)
		{
			var configParser = new DefaultConfigParser();
			foreach (var config in model.Configs)
			{
				//find the generator node, if GeneratorTypeName is diff than the service, add  it as a component
				var generator = config.Components.FirstOrDefault(x => { return x.ServiceTypeName.Equals("HyperActive.Core.Generators.ActiveRecordGenerator, HyperActive"); });
				if (generator != null && !String.IsNullOrEmpty(config.GeneratorTypeName))
				{
					generator.ServiceImplementationTypeName = config.GeneratorTypeName;
				}
			}
			var options = Convert(model.Configs);			
			configParser.WriteXml(model.ConfigFilePath, options);
		}

		public EditConfigsViewModel CreateConfig()
		{
			var result = new EditConfigsViewModel();
			result.Configs.Add(new ConfigurationOptionsViewModel());
			return result;
		}


	}
}
