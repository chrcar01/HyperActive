using HyperActive.Core.Config;
using HyperActive.SchemaProber;
using System;
using System.IO;

namespace HyperActive.Core.Generators
{
	public class CodeRunnerConfig
	{
		private IConfigurationOptions _options;
		private ActiveRecordGenerator _generator;
		private NameProvider _nameProvider;
		private IDbProvider _dbProvider;
		private Func<string, TextWriter> _writer;
		public CodeRunnerConfig() { }
		public CodeRunnerConfig(IConfigurationOptions options, ActiveRecordGenerator generator, NameProvider nameProvider, IDbProvider dbProvider, Func<string, TextWriter> writer)
		{
			_options = options;
			_generator = generator;
			_nameProvider = nameProvider;
			_dbProvider = dbProvider;
			_writer = writer;
		}
		public IConfigurationOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				_options = value;
			}
		}
		public ActiveRecordGenerator Generator
		{
			get
			{
				return _generator;
			}
			set
			{
				_generator = value;
			}
		}
		public NameProvider NameProvider
		{
			get
			{
				if (_nameProvider == null)
				{
					_nameProvider = new NameProvider();
				}
				return _nameProvider;
			}
			set
			{
				_nameProvider = value;
			}
		}
		public IDbProvider DbProvider
		{
			get
			{
				return _dbProvider;
			}
			set
			{
				_dbProvider = value;
			}
		}
		public Func<string, TextWriter> Writer
		{
			get
			{
				return _writer;
			}
			set
			{
				_writer = value;
			}
		}
	}
}
