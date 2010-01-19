using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HtmlBuilder;

namespace HyperActive.ConsoleUI.Tests
{
	public class Config : Element
	{
		private List<Section> _sections;
		public Config(params Section[] sections)
			: base("hyperactive")
		{
			_sections = new List<Section>();
			if (sections != null && sections.Length > 0)
			{
				foreach (Section section in sections)
				{
					_sections.Add(section);
				}
			}
		}
		public ReadOnlyCollection<Section> Sections
		{
			get
			{
				return _sections.AsReadOnly();
			}
		}
		protected override void RenderChildren(IWriter writer)
		{
			foreach(Section section in this.Sections)
			{
				section.Render(writer);
			}
		}
		public void AddSection(Section section)
		{
			_sections.Add(section);
		}
	}

	public class Section : Element
	{
		public Section()
			: base("config")
		{
		}
		private void RenderAdd(IWriter writer, string key, string value)
		{
			if (String.IsNullOrEmpty(value))
				return;

			new Element("add")
				.AddAttribute("key", key)
				.AddAttribute("value", value).Render(writer);
		}
		protected override void RenderChildren(IWriter writer)
		{
			this.RenderAdd(writer, "namespace", this.Namespace);
			this.RenderAdd(writer, "namer", this.Namer);
			this.RenderAdd(writer, "connectionstring", this.ConnectionString);
			this.RenderAdd(writer, "generator", this.Generator);
			this.RenderAdd(writer, "outputpath", this.OutputPath);
			this.RenderAdd(writer, "skiptables", this.SkipTables);
			this.RenderAdd(writer, "basetypename", this.BaseTypeName);
		}
		private string _baseTypeName;
		public string BaseTypeName
		{
			get { return _baseTypeName; }
			set
			{
				_baseTypeName = value;
			}
		}
		private string _outputPath;
		public string OutputPath
		{
			get { return _outputPath; }
			set
			{
				_outputPath = value;
			}
		}
		private string _skipTables;
		public string SkipTables
		{
			get { return _skipTables; }
			set
			{
				_skipTables = value;
			}
		}
		private string _namespace;
		public string Namespace
		{
			get { return _namespace; }
			set
			{
				_namespace = value;
			}
		}
		private string _namer;
		public string Namer
		{
			get { return _namer; }
			set
			{
				_namer = value;
			}
		}
		private string _connectionString;
		public string ConnectionString
		{
			get { return _connectionString; }
			set
			{
				_connectionString = value;
			}
		}
		private string _generator;
		public string Generator
		{
			get { return _generator; }
			set
			{
				_generator = value;
			}
		}
	}
}
