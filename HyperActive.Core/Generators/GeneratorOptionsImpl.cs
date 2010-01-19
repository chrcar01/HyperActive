using System;
using System.IO;
using HyperActive.Core;
using HyperActive.Core.Generators;
using HyperActive.SchemaProber;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// Implementation of IGeneratorOptions
	/// </summary>
	public class GeneratorOptionsImpl : IGeneratorOptions
	{
		/// <summary>
		/// Gets or sets the enums.
		/// </summary>
		/// <value>The enums.</value>
		public IEnumerable<EnumDescriptor> Enums { get; set; }
		/// <summary>
		/// Gets or sets the namespace that would contain active record
		/// classes.  This is used when creating non-activerecord model classes
		/// that need to map property values back and forth
		/// </summary>
		/// <value>The data namespace.</value>
		public string DataNamespace { get; set; }

		/// <summary>
		/// Gets or sets the name of the abstract base.
		/// </summary>
		/// <value>The name of the abstract base.</value>
		public string AbstractBaseName { get; set; }

		/// <summary>
		/// Gets or sets the name of the base type.
		/// </summary>
		/// <value>The name of the base type.</value>
		public string BaseTypeName { get; set; }
		/// <summary>
		/// Gets or sets the output path.
		/// </summary>
		/// <value>The output path.</value>
		public string OutputPath { get; set; }
		private StringCollection _skipTables;
		/// <summary>
		/// Gets or sets the tables that will not be generated
		/// </summary>
		/// <value>The skip tables.</value>
		public StringCollection SkipTables
		{
			get
			{
				if (_skipTables == null)
					_skipTables = new StringCollection();
				return _skipTables;
			}
		}
		/// <summary>
		/// Gets or sets the namespace.
		/// </summary>
		/// <value>The namespace.</value>
		public string Namespace { get; set; }
		/// <summary>
		/// Gets or sets the output.
		/// </summary>
		/// <value>The output.</value>
		public Func<TableSchema, TextWriter> Output { get; set; }

		/// <summary>
		/// Gets or sets the output writer.
		/// </summary>
		/// <value>The output writer.</value>
        public Func<string, TextWriter> OutputWriter { get; set; }
		/// <summary>
		/// Gets or sets the generator.
		/// </summary>
		/// <value>The generator.</value>
		public ActiveRecordGenerator Generator { get; set; }
		/// <summary>
		/// Gets or sets the db provider.
		/// </summary>
		/// <value>The db provider.</value>
		public IDbProvider DbProvider { get; set; }
		/// <summary>
		/// Gets or sets the namer.
		/// </summary>
		/// <value>The namer.</value>
		public NameProvider Namer { get; set; }


		/// <summary>
		/// Gets or sets the name of the marker interface.
		/// </summary>
		/// <value>The name of the marker interface.</value>
		public string MarkerInterfaceName
		{
			get;set;
		}


		private bool _generateComments = true;
		/// <summary>
		/// Gets or sets a value indicating whether to [generate comments].  The default is true.
		/// </summary>
		/// <value><c>true</c> if [generate comments]; otherwise, <c>false</c>.</value>
		public bool GenerateComments
		{
			get
			{
				return _generateComments;
			}
			set
			{
				_generateComments = value;
			}
		}
		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		public string ConnectionString { get; set; }
	}
}
