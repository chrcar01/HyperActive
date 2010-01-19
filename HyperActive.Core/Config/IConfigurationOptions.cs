using System;
using System.Collections.Generic;
using HyperActive.Core.Generators;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml;

namespace HyperActive.Core.Config
{
	
    public interface IConfigurationOptions
	{

		/// <summary>
		/// Gets or sets the name of the static primary key name.  So if you want every primary key column
		/// name to be "ID", set this in the config to "ID", otherwise the name is inferred from the
		/// actual column name in the database.
		/// </summary>
		/// <value>The name of the static primary key.</value>
		string StaticPrimaryKeyName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [ioc verbose logging].
        /// </summary>
        /// <value><c>true</c> if [ioc verbose logging]; otherwise, <c>false</c>.</value>
        bool IocVerboseLogging { get; set; }
        /// <summary>
        /// Gets or sets the directory that the IOC should search for component types.
        /// </summary>
        /// <value>The assembly directory.</value>
        string AssemblyDirectory { get; set; }
		/// <summary>
		/// Gets or sets the a list of values that should be swapped out in the Nameprovider.
		/// </summary>
		/// <value>The enum replacements.</value>
		Dictionary<string, string> EnumReplacements { get; }
		/// <summary>
		/// Gets or sets the enum output path.
		/// </summary>
		/// <value>The enum output path.</value>
		string EnumOutputPath { get; set; }

		/// <summary>
		/// Gets or sets the enum namespace.
		/// </summary>
		/// <value>The enum namespace.</value>
		string EnumNamespace { get; set; }

		/// <summary>
		/// Gets a value indicating whether [use microsofts header].
		/// </summary>
		/// <value><c>true</c> if [use microsofts header]; otherwise, <c>false</c>.</value>
		bool UseMicrosoftsHeader { get; set;  }

        /// <summary>
		/// Gets or sets a value indicating whether [generate column list].
		/// </summary>
		/// <value><c>true</c> if [generate column list]; otherwise, <c>false</c>.</value>
		bool GenerateColumnList { get; set; }

		/// <summary>
		/// Gets or sets the enums.
		/// </summary>
		/// <value>The enums.</value>
		IEnumerable<EnumDescriptor> Enums { get; set; }

		/// <summary>
		/// Gets or sets the components.
		/// </summary>
		/// <value>The components.</value>
		IEnumerable<ComponentDescriptor> Components { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to [generate comments].
		/// </summary>
		/// <value><c>true</c> if [generate comments]; otherwise, <c>false</c>.</value>
		bool GenerateComments { get; set; }

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		string ConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the namespace that would contain active record 
		/// classes.  This is used when creating non-activerecord model classes
		/// that need to map property values back and forth
		/// </summary>
		/// <value>The data namespace.</value>
		string DataNamespace { get; set; }

		/// <summary>
		/// Gets or sets the name of the abstract base.
		/// </summary>
		/// <value>The name of the abstract base.</value>
		string AbstractBaseName { get; set; }

		/// <summary>
		/// Gets or sets the name of the base type.
		/// </summary>
		/// <value>The name of the base type.</value>
		string BaseTypeName { get; set; }

        /// <summary>
        /// Gets the list of tables to skip.
        /// </summary>
        /// <value>The list of tables that ar classes will not be generated.</value>
        List<string> SkipTables { get; }

		/// <summary>
		/// Gets the prefixes of tables that will be skipped.
		/// </summary>
		/// <value>The prefixes of tables that will be skipped.</value>
		List<string> SkipTablesWithPrefix { get; }


		/// <summary>
		/// Gets the prefixes of the the only tables that will be generated.
		/// </summary>
		/// <value>The prefixes of the the only tables that will be generated..</value>
		List<string> OnlyTablesWithPrefix { get; }

		/// <summary>
		/// Gets or sets the namespace.
		/// </summary>
		/// <value>The namespace.</value>
		string Namespace { get; set; }

		/// <summary>
		/// Gets or sets the directory where the generated files are written.
		/// </summary>
		/// <value>the directory where the generated files are written.</value>
		string OutputPath { get; set; }
	}
}
