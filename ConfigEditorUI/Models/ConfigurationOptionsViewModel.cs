using HyperActive.Core.Config;
using HyperActive.Core.Generators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ConfigEditorUI.Models
{
	public class ConfigurationOptionsViewModel : INotifyPropertyChanged
	{
		private string _generatorTypeName;
		public string GeneratorTypeName
		{
			get
			{
				return _generatorTypeName;
			}
			set
			{
				_generatorTypeName = value;
				RaisePropertyChanged("GeneratorTypeName");
			}
		}
        
		private bool _iocVerboseLogging;
		/// <summary>
		/// Gets or sets a value indicating whether [ioc verbose logging].
		/// </summary>
		/// <value>
		/// <c>true</c> if [ioc verbose logging]; otherwise, <c>false</c>.</value>
		public bool IocVerboseLogging
		{
			get
			{
				return _iocVerboseLogging;
			}
			set
			{
				_iocVerboseLogging = value;
				RaisePropertyChanged("IocVerboseLogging");
			}
		}

		private string _staticPrimaryKeyName;
		/// <summary>
		/// Gets or sets the name of the static primary key name.  So if you want every primary key column
		/// name to be "ID", set this in the config to "ID", otherwise the name is inferred from the
		/// actual column name in the database.
		/// </summary>
		/// <value>The name of the static primary key.</value>
		public string StaticPrimaryKeyName
		{
			get
			{
				return _staticPrimaryKeyName;
			}
			set
			{
				_staticPrimaryKeyName = value;
				RaisePropertyChanged("StaticPrimaryKeyName");
			}
		}

        private List<string> _skipTables;
        /// <summary>
        /// Gets the list of tables to skip.
        /// </summary>
        /// <value>The list of tables that ar classes will not be generated.</value>
        public List<string> SkipTables
        {
            get
            {
                if (_skipTables == null)
                {
                    _skipTables = new List<string>();
                }
                return _skipTables;
            }
			set
            {
            	_skipTables = value;
				RaisePropertyChanged("SkipTables");
            }
        }
        
		private bool _generateColumnList = true;
		/// <summary>
		/// Gets or sets a value indicating whether [generate column list].
		/// </summary>
		/// <value><c>true</c> if [generate column list]; otherwise, <c>false</c>.</value>
		public bool GenerateColumnList
		{
			get
			{
				return _generateColumnList;
			}
			set
			{
				_generateColumnList = value;
				RaisePropertyChanged("GenerateColumnList");
			}
		}

		private ObservableCollection<EnumDescriptor> _enums;
		/// <summary>
		/// Gets or sets the enums.
		/// </summary>
		/// <value>The enums.</value>
		public ObservableCollection<EnumDescriptor> Enums
		{
			get
			{
				if (_enums == null)
				{
					_enums = new ObservableCollection<EnumDescriptor>();
				}
				return _enums;
			}
		}

		private ObservableCollection<ComponentDescriptor> _components;
		/// <summary>
		/// Gets or sets the components.
		/// </summary>
		/// <value>The components.</value>
		public ObservableCollection<ComponentDescriptor> Components
		{
			get
			{
				if (_components == null)
				{
					_components = new ObservableCollection<ComponentDescriptor>();
				}
				return _components;
			}
		}

		private bool _generateComments = true;
		/// <summary>
		/// Gets or sets a value indicating whether to [generate comments].  The default is true.
		/// </summary>
		/// <value>
		/// <c>true</c> if [generate comments]; otherwise, <c>false</c>.</value>
		public bool GenerateComments
		{
			get
			{
				return _generateComments;
			}
			set
			{
				_generateComments = value;
				RaisePropertyChanged("GenerateComments");
			}
		}

		private string _connectionString;
		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
				RaisePropertyChanged("ConnectionString");
			}
		}

		private string _dataNamespace;
		/// <summary>
		/// Gets or sets the namespace that would contain active record 
		/// classes.  This is used when creating non-activerecord model classes
		/// that need to map property values back and forth.
		/// </summary>
		/// <value>The data namespace.</value>
		public string DataNamespace
		{
			get
			{
				return _dataNamespace;
			}
			set
			{
				_dataNamespace = value;
				RaisePropertyChanged("DataNamespace");
			}
		}

		private string _abstractBaseName;
		/// <summary>
		/// Gets or sets the name of the abstract base which in turn inherits from BaseTypeName.  This is
		/// used when generating ActiveRecord classes that use a config section other than the default
		/// configuration(ie. when using multiple databases from the same project).
		/// </summary>
		/// <value>The name of the abstract base.</value>
		public string AbstractBaseName
		{
			get
			{
				return _abstractBaseName;
			}
			set
			{
				_abstractBaseName = value;
				RaisePropertyChanged("AbstractBaseName");
			}
		}

		private string _baseTypeName;
		/// <summary>
		/// Gets or sets the name of the base type.
		/// </summary>
		/// <value>The name of the base type.</value>
		public string BaseTypeName
		{
			get
			{
				if (String.IsNullOrEmpty(_baseTypeName))
				{
					_baseTypeName = "Castle.ActiveRecord.ActiveRecordBase";
				}
				return _baseTypeName;
			}
			set
			{
				_baseTypeName = value;
				RaisePropertyChanged("BaseTypeName");
			}
		}

		private List<string> _onlyTablesWithPrefix;

		/// <summary>
		/// Gets the prefixes of the the only tables that will be generated.
		/// </summary>
		/// <value>The prefixes of the the only tables that will be generated..</value>
		public List<string> OnlyTablesWithPrefix
		{
			get
			{
				if (_onlyTablesWithPrefix == null)
				{
					_onlyTablesWithPrefix = new List<string>();
				}
				return _onlyTablesWithPrefix;
			}
			set
            {
            	_onlyTablesWithPrefix = value;
				RaisePropertyChanged("OnlyTablesWithPrefix");
            }
		}

	
		private List<string> _skipTablesWithPrefix;
		/// <summary>
		/// Gets the prefixes of tables that will be skipped.
		/// </summary>
		/// <value>The prefixes of tables that will be skipped.</value>
		public List<string> SkipTablesWithPrefix
		{
			get
			{
				if (_skipTablesWithPrefix == null)
				{
					_skipTablesWithPrefix = new List<string>();
				}
				return _skipTablesWithPrefix;
			}
			set
            {
            	_skipTablesWithPrefix = value;
            }
		}


		private string _namespace;
		/// <summary>
		/// Gets or sets the namespace.
		/// </summary>
		/// <value>The namespace.</value>
		public string Namespace
		{
			get
			{
				if (String.IsNullOrEmpty(_namespace))
				{
					_namespace = "This.Is.The.Default.Namespace";
				}
				return _namespace;
			}
			set
			{
				_namespace = value;
				RaisePropertyChanged("Namespace");
			}
		}

		private string _outputPath;
		/// <summary>
		/// Gets or sets the output path.
		/// </summary>
		/// <value>The output path.</value>
		public string OutputPath
		{
			get
			{
				return _outputPath;
			}
			set
			{
				_outputPath = value;
				RaisePropertyChanged("OutputPath");
			}
		}

		private bool _useMicrosoftsHeader = false;
		/// <summary>
		/// Gets a value indicating whether [use microsofts header].
		/// </summary>
		/// <value>
		/// <c>true</c> if [use microsofts header]; otherwise, <c>false</c>.</value>
		public bool UseMicrosoftsHeader
		{
			get
			{
				return _useMicrosoftsHeader;
			}
			set
			{
				_useMicrosoftsHeader = value;
				RaisePropertyChanged("UseMicrosoftsHeader");
			}
		}

		private string _enumOutputPath;
		/// <summary>
		/// Gets or sets the enum output path.
		/// </summary>
		/// <value>The enum output path.</value>
		public string EnumOutputPath
		{
			get
			{
				return _enumOutputPath;
			}
			set
			{
				_enumOutputPath = value;
				RaisePropertyChanged("EnumOutputPath");
			}
		}

		private string _enumNamespace;
		/// <summary>
		/// Gets or sets the enum namespace.
		/// </summary>
		/// <value>The enum namespace.</value>
		public string EnumNamespace
		{
			get
			{
				return _enumNamespace;
			}
			set
			{
				_enumNamespace = value;
				RaisePropertyChanged("EnumNamespace");
			}
		}


		private Dictionary<string, string> _enumReplacements = null;
		/// <summary>
		/// Gets the a list of values that should be swapped out in the Nameprovider.
		/// </summary>
		/// <value>The enum replacements.</value>
		public Dictionary<string, string> EnumReplacements
		{
			get
			{
				if (_enumReplacements == null)
				{
					_enumReplacements = new Dictionary<string, string>();
				}
				return _enumReplacements;
			}
			set
            {
            	_enumReplacements = value;
				RaisePropertyChanged("EnumReplacements");
            }
		}




		private string _assemblyDirectory;
		/// <summary>
		/// Gets or sets the directory that the IOC should search for component types.
		/// </summary>
		/// <value>The assembly directory.</value>
		public string AssemblyDirectory
		{
			get
			{
				return _assemblyDirectory;
			}
			set
			{
				_assemblyDirectory = value;
				RaisePropertyChanged("AssemblyDirectory");
			}
		}



		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			Console.WriteLine("{0} changed.", propertyName);
		}

		#endregion
	}
	
}
