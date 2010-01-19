using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using HyperActive.Core.Generators;

namespace HyperActive.Core.Config
{
	public class DefaultConfigParser : IConfigParser
	{
		private class AttributeHelper
		{
			private XmlNode _node;
			private string _baseXpathQuery;
			public AttributeHelper(string baseXpathQuery, XmlNode node)
			{
				_node = node;
				_baseXpathQuery = baseXpathQuery;
			}
			public bool GetBool(string key, bool @default)
			{
				var node = GetNode(key);
				if (node == null)
					return @default;

				bool result = Boolean.TryParse(node.Value, out result) ? result : @default;
				return result;
			}
			private XmlNode GetNode(string key)
			{
				string xpath = String.Format(_baseXpathQuery, key);
				return _node.SelectSingleNode(xpath);
			}
			public string Get(string key)
			{
				var node = GetNode(key);
				return node != null ? node.Value : String.Empty;

			}
		}
		public IEnumerable<IConfigurationOptions> ParseXml(string contentsOfConfigFile)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(contentsOfConfigFile);
			XmlNodeList configNodes = doc.DocumentElement.SelectNodes("//config");
			var result = new List<IConfigurationOptions>();
			foreach (XmlNode configNode in configNodes)
			{
				IConfigurationOptions options = new DefaultConfigurationOptions();
				var attrib = new AttributeHelper(".//add[@key='{0}']/@value", configNode);
                options.AssemblyDirectory = attrib.Get("assemblydirectory");
				options.AbstractBaseName = attrib.Get("abstractbasename");
				options.BaseTypeName = attrib.Get("basetypename");
				options.ConnectionString = attrib.Get("connectionstring");
				options.DataNamespace = attrib.Get("datanamespace");
                options.IocVerboseLogging = attrib.GetBool("iocverboselogging", false);
				options.GenerateColumnList = attrib.GetBool("generatecolumnlist", true);
				options.GenerateComments = attrib.GetBool("generatecomments", true);
				options.UseMicrosoftsHeader = attrib.GetBool("usemicrosoftsheader", false);
				options.Namespace = attrib.Get("namespace");
				options.OutputPath = attrib.Get("outputpath");
				options.EnumOutputPath = attrib.Get("enumoutputpath");
				options.EnumNamespace = attrib.Get("enumnamespace");
				options.StaticPrimaryKeyName = attrib.Get("staticprimarykeyname");
				options.OnlyTablesWithPrefix.AddRange(attrib.Get("onlytableswithprefix").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
				options.SkipTablesWithPrefix.AddRange(attrib.Get("skiptableswithprefix").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                options.SkipTables.AddRange(attrib.Get("skiptables").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
				options.Enums = ParseEnums(configNode);
				LoadEnumReplacements(configNode, options);
				options.Components = ParseComponents(configNode);
				result.Add(options);
			}
			return result;
		}

		private void LoadEnumReplacements(XmlNode configNode, IConfigurationOptions options)
		{
			var nodeList = configNode.SelectNodes("./enums/replacement");
			foreach (XmlNode node in nodeList)
			{
				string lookfor = node.Attributes["lookfor"] != null ? node.Attributes["lookfor"].Value : String.Empty;
				string replacewith = node.Attributes["replacewith"] != null ? node.Attributes["replacewith"].Value : String.Empty;
				if (String.IsNullOrEmpty(lookfor) || String.IsNullOrEmpty(replacewith))
				{
					continue;
				}

				options.EnumReplacements.Add(lookfor, replacewith);
			}
		}

		private IEnumerable<EnumDescriptor> ParseEnums(XmlNode configNode)
		{
			var result = new List<EnumDescriptor>();
			var enumNodeList = configNode.SelectNodes("./enums//add");
			foreach (XmlNode enumNode in enumNodeList)
			{
				var @enum = new EnumDescriptor();
				@enum.TableName = enumNode.Attributes["table"] != null ? enumNode.Attributes["table"].Value : String.Empty;
				@enum.NameField = enumNode.Attributes["nameField"] != null ? enumNode.Attributes["nameField"].Value : String.Empty;
				@enum.ValueField = enumNode.Attributes["valueField"] != null ? enumNode.Attributes["valueField"].Value : String.Empty;
				result.Add(@enum);
			}
			return result;
		}

		private IEnumerable<ComponentDescriptor> ParseComponents(XmlNode configNode)
		{
			var result = new List<ComponentDescriptor>();
			var componentNodeList = configNode.SelectNodes(".//component");
			foreach (XmlNode componentNode in componentNodeList)
			{
				var componentDescriptor = new ComponentDescriptor();
				componentDescriptor.ServiceTypeName = componentNode.Attributes["service"] != null ? componentNode.Attributes["service"].Value : String.Empty;
                Console.WriteLine("ServiceTypeName: " + componentDescriptor.ServiceTypeName);
				componentDescriptor.ServiceImplementationTypeName = componentNode.Attributes["serviceimpl"] != null ? componentNode.Attributes["serviceimpl"].Value : String.Empty;
                Console.WriteLine("ServiceImplementationTypeName: " + componentDescriptor.ServiceImplementationTypeName);
				var ctorParamNodeList = componentNode.SelectNodes(".//param");
				foreach(XmlNode ctorParamNode in ctorParamNodeList)
				{
					var paramDescriptor = new ConstructorParameterDescriptor();
					paramDescriptor.Name = ctorParamNode.Attributes["name"] != null ? ctorParamNode.Attributes["name"].Value : String.Empty;
					paramDescriptor.Value = ctorParamNode.Attributes["value"] != null ? ctorParamNode.Attributes["value"].Value : String.Empty;
					paramDescriptor.Type = ctorParamNode.Attributes["type"] != null ? ctorParamNode.Attributes["type"].Value : String.Empty;
					componentDescriptor.Parameters.Add(paramDescriptor);
				}
				result.Add(componentDescriptor);
			}
			return result;
		}

		public IEnumerable<IConfigurationOptions> ParseFile(string pathToConfigFile)
		{
			return ParseXml(File.ReadAllText(pathToConfigFile));
		}
	}
}
