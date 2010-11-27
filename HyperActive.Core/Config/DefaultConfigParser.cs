using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private XmlElement CreateAddNode(XmlDocument doc, string key, string value)
		{
			XmlElement result = doc.CreateElement("add");
			XmlAttribute nameAttribute = doc.CreateAttribute("key");
			nameAttribute.Value = key;
			result.Attributes.Append(nameAttribute);
			XmlAttribute valueAttribute = doc.CreateAttribute("value");
			valueAttribute.Value = value;
			result.Attributes.Append(valueAttribute);
			return result;
		}
		public void WriteXml(string configFilePath, IEnumerable<IConfigurationOptions> configs)
		{
			if (String.IsNullOrEmpty(configFilePath))
				throw new ArgumentException("configFilePath is null or empty.", "configFilePath");

			if (configs == null || configs.Count() == 0)
				throw new ArgumentNullException("configs", "configs is null.");

			XmlDocument doc = new XmlDocument();
			XmlElement hyperactiveNode = doc.CreateElement("hyperactive");
			foreach (var cfg in configs)
			{
				XmlElement config = doc.CreateElement("config");
				config.AppendChild(CreateAddNode(doc, "abstractbasename", cfg.AbstractBaseName));
				config.AppendChild(CreateAddNode(doc, "assemblydirectory", cfg.AssemblyDirectory));
				config.AppendChild(CreateAddNode(doc, "basetypename", cfg.BaseTypeName));
				config.AppendChild(CreateAddNode(doc, "connectionstring", cfg.ConnectionString));
				config.AppendChild(CreateAddNode(doc, "datanamespace", cfg.DataNamespace));
				config.AppendChild(CreateAddNode(doc, "iocverboselogging", cfg.IocVerboseLogging.ToString().ToLower()));
				config.AppendChild(CreateAddNode(doc, "generatecolumnlist", cfg.GenerateColumnList.ToString().ToLower()));
				config.AppendChild(CreateAddNode(doc, "generatecomments", cfg.GenerateComments.ToString().ToLower()));
				config.AppendChild(CreateAddNode(doc, "usemicrosoftsheader", cfg.UseMicrosoftsHeader.ToString().ToLower()));
				config.AppendChild(CreateAddNode(doc, "namespace", cfg.Namespace));
				config.AppendChild(CreateAddNode(doc, "outputpath", cfg.OutputPath));
				config.AppendChild(CreateAddNode(doc, "enumoutputpath", cfg.EnumOutputPath));
				config.AppendChild(CreateAddNode(doc, "enumnamespace", cfg.EnumNamespace));
				config.AppendChild(CreateAddNode(doc, "staticprimarykeyname", cfg.StaticPrimaryKeyName));
				config.AppendChild(CreateAddNode(doc, "onlytableswithprefix", String.Join(",", cfg.OnlyTablesWithPrefix.ToArray())));
				config.AppendChild(CreateAddNode(doc, "skiptableswithprefix", String.Join(",", cfg.SkipTablesWithPrefix.ToArray())));
				config.AppendChild(CreateAddNode(doc, "skiptables", String.Join(",", cfg.SkipTables.ToArray())));
				var components = doc.CreateElement("components");
				foreach (var c in cfg.Components)
				{
					var component = doc.CreateElement("component");
					var serviceAttribute = doc.CreateAttribute("service");
					serviceAttribute.Value = c.ServiceTypeName;
					component.Attributes.Append(serviceAttribute);
					var serviceImplAttribute = doc.CreateAttribute("serviceimpl");
					serviceImplAttribute.Value = c.ServiceImplementationTypeName;
					component.Attributes.Append(serviceImplAttribute);
					if (c.Parameters.Count > 0)
					{
						foreach (var p in c.Parameters)
						{
							var paramNode = doc.CreateElement("param");
							var nameAttr = doc.CreateAttribute("name");
							nameAttr.Value = p.Name;
							paramNode.Attributes.Append(nameAttr);
							var valueAttr = doc.CreateAttribute("value");
							valueAttr.Value = p.Value;
							paramNode.Attributes.Append(valueAttr);
							var typeAttr = doc.CreateAttribute("type");
							typeAttr.Value = p.Type;
							paramNode.Attributes.Append(typeAttr);
							component.AppendChild(paramNode);
						}
					}
					components.AppendChild(component);
				}
				config.AppendChild(components);
				var enumsNode = doc.CreateElement("enums");
				foreach (var e in cfg.Enums)
				{
					var enumNode = doc.CreateElement("add");
					var tableAttr = doc.CreateAttribute("table");
					tableAttr.Value = e.TableName;
					enumNode.Attributes.Append(tableAttr);
					var nameFieldAttr = doc.CreateAttribute("nameField");
					nameFieldAttr.Value = e.NameField;
					enumNode.Attributes.Append(nameFieldAttr);
					var valueFieldAttr = doc.CreateAttribute("valueField");
					valueFieldAttr.Value = e.ValueField;
					enumNode.Attributes.Append(valueFieldAttr);
					enumsNode.AppendChild(enumNode);
				}
				foreach (var r in cfg.EnumReplacements)
				{
					var replacementNode = doc.CreateElement("replacement");
					var lookforAttr = doc.CreateAttribute("lookfor");
					lookforAttr.Value = r.Key;
					replacementNode.Attributes.Append(lookforAttr);
					var replacewithAttr = doc.CreateAttribute("replacewith");
					replacewithAttr.Value = r.Value;
					replacementNode.Attributes.Append(replacewithAttr);
					enumsNode.AppendChild(replacementNode);
				}
				hyperactiveNode.AppendChild(enumsNode);
				config.AppendChild(enumsNode);

				hyperactiveNode.AppendChild(config);
			}
			doc.AppendChild(hyperactiveNode);
			doc.Save(configFilePath);
		}

		public IEnumerable<IConfigurationOptions> ParseXml(string contentsOfConfigFile)
		{
			if (String.IsNullOrEmpty(contentsOfConfigFile))
				throw new ArgumentException("contentsOfConfigFile is null or empty.", "contentsOfConfigFile");
			
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
