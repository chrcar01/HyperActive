using HyperActive.Core;
using HyperActive.Core.Generators;
using HyperActive.SchemaProber;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace HyperActive.Core.Generators
{

	/// <summary>
	/// Generator Options
	/// </summary>
	public interface IGeneratorOptions
	{   
		
		
		/// <summary>
		/// Gets or sets the output writer.
		/// </summary>
		/// <value>The output writer.</value>
		Func<string, TextWriter> OutputWriter { get; set; }

		/// <summary>
		/// Gets or sets the output.
		/// </summary>
		/// <value>The output.</value>
		Func<TableSchema, TextWriter> Output { get; set; }

		/// <summary>
		/// Gets or sets the generator.
		/// </summary>
		/// <value>The generator.</value>
		ActiveRecordGenerator Generator { get; set; }
		/// <summary>
		/// Gets or sets the db provider.
		/// </summary>
		/// <value>The db provider.</value>
		IDbProvider DbProvider { get; set; }
		/// <summary>
		/// Gets or sets the namer.
		/// </summary>
		/// <value>The namer.</value>
		NameProvider Namer { get; set; }

		
	}
}
