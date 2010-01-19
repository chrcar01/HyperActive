using System;
using System.IO;
using System.Linq;
using HyperActive.Dominator;
using HyperActive.SchemaProber;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using HyperActive.Core.Config;
using System.Collections.Specialized;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// Base class that all ActiveRecordGenerators must inherit.
	/// </summary>
	public abstract class ActiveRecordGenerator
	{
		private IDbProvider _dbProvider;

		/// <summary>
		/// Gets or sets the db provider.
		/// </summary>
		/// <value>The db provider.</value>
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
		private IConfigurationOptions _configOptions = null;
		/// <summary>
		/// Gets or sets the config options.
		/// </summary>
		/// <value>The config options.</value>
		public IConfigurationOptions ConfigOptions
		{
			get
			{
				return _configOptions;
			}
			set
			{
				_configOptions = value;
			}
		}
		private NameProvider _nameProvider = null;
		/// <summary>
		/// Gets or sets the name provider.
		/// </summary>
		/// <value>The name provider.</value>
		public virtual NameProvider NameProvider
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

        /// <summary>
		/// Adds a prrimary key property to the ClassDeclaration.
		/// </summary>
		/// <param name="classDeclaration">The ClassDeclaration instance the property is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
        protected abstract PropertyDeclaration CreatePrimaryKeyProperty(ClassDeclaration classDeclaration, TableSchema table);

		/// <summary>
		/// Adds an attribute to a ClassDeclaration.
		/// </summary>
		/// <param name="classDeclaration">The ClassDeclaration instance the property is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		protected abstract void AddClassAttributes(ClassDeclaration classDeclaration, TableSchema table);

		/// <summary>
		/// Creates a ClassDeclaration.
		/// </summary>
		/// <param name="namespaceDeclaration">The NamespaceDeclaration instance the class is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		/// <returns>A reference to the ClassDeclaration instance that was created.</returns>
		protected abstract ClassDeclaration CreateClass(NamespaceDeclaration namespaceDeclaration, TableSchema table);

		/// <summary>
		/// Creates a property that is a foreign key within the current table that points to a primary key in another table.  These typically have BelongsTo attributes.
		/// </summary>
		/// <param name="classDeclaration">The class to which the property is added.</param>
		/// <param name="foreignKey">The foreign key the property represents.</param>
		/// <returns>The foreign key property that was added.</returns>
		protected abstract PropertyDeclaration CreateForeignKey(ClassDeclaration classDeclaration, ForeignKeyColumnSchema foreignKey);

		/// <summary>
		/// Creates a property that has many children that point to the primary key contained within this class.  Typically these will have HasMany attributes.
		/// </summary>
		/// <param name="classDecl">The class to which the property is added.</param>
		/// <param name="foreignKey">The foreign key in the table that references the primary key in this class.</param>
		/// <returns>The property that was added.</returns>
		protected abstract PropertyDeclaration CreateForeignKeyReference(ClassDeclaration classDecl, ForeignKeyColumnSchema foreignKey);

		/// <summary>
		/// Creates a plain ol' property representing any column that is not a foreign key or primary key.
		/// </summary>
		/// <param name="classDeclaration">The class to which the property is added.</param>
		/// <param name="column">The column the property represents.</param>
		protected abstract void CreateNonKeyProperty(ClassDeclaration classDeclaration, ColumnSchema column);

		/// <summary>
		/// Creates the association properties.
		/// </summary>
		/// <param name="classDeclaration">The class declaration.</param>
		/// <param name="table">The table.</param>
		protected abstract void CreateAssociationProperties(ClassDeclaration classDeclaration, TableSchema table);

		/// <summary>
		/// When overridden in a child class, adds constructors to the class declaration.
		/// </summary>
		/// <param name="classDecl">The class declaration for which we're building constructors.</param>
		/// <param name="table">The table schema the class declaration represents.</param>
		/// <returns>The updated class declaration or null if it's not overridden.</returns>
		protected virtual ClassDeclaration CreateConstructors(ClassDeclaration classDecl, TableSchema table)
		{
			return null;
		}

		/// <summary>
		/// When overriden in a child class, adds methods to the class declaration.
		/// </summary>
		/// <param name="classDecl">The class declaration to which the methods are added.</param>
		/// <param name="table">The table schema the class declaration represents.</param>
		/// <returns>The updated class declaration or null if it's not overriden.</returns>
		protected virtual ClassDeclaration AddMethods(ClassDeclaration classDecl, TableSchema table)
		{
			return null;
		}

		/// <summary>
		/// Generates the specified namespace declaration.
		/// </summary>
		/// <param name="namespaceDeclaration">The namespace declaration.</param>
		/// <param name="table">The table.</param>
		/// <returns></returns>
		public ClassDeclaration Generate(NamespaceDeclaration namespaceDeclaration, TableSchema table)
		{
			ClassDeclaration classDecl = this.CreateClass(namespaceDeclaration, table);
			this.AddClassAttributes(classDecl, table);
			this.CreatePrimaryKeyProperty(classDecl, table);

			foreach (ForeignKeyColumnSchema fk in table.ForeignKeys)
			{
				this.CreateForeignKey(classDecl, fk);
			}
			if (table.PrimaryKey != null && table.PrimaryKey.ForeignKeyReferences != null)
			{
				foreach (ForeignKeyColumnSchema foreignkey in table.PrimaryKey.ForeignKeyReferences)
				{
					this.CreateForeignKeyReference(classDecl, foreignkey);
				}
			}
			if (table.NonKeyColumns != null)
			{
				foreach (ColumnSchema column in table.NonKeyColumns)
				{
					this.CreateNonKeyProperty(classDecl, column);
				}
			}
			if (_configOptions.GenerateColumnList)
			{
				CreateColumnList(table, classDecl);
			}
			this.CreateConstructors(classDecl, table);
			this.CreateAssociationProperties(classDecl, table);
			this.AddMethods(classDecl, table);
			return classDecl;
		}

		/// <summary>
		/// Creates the column list.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="classDecl">The class decl.</param>
		protected virtual void CreateColumnList(TableSchema table, ClassDeclaration classDecl)
		{
			ClassDeclaration cols = new ClassDeclaration(classDecl.Name + "Columns");
			var columns = table.Columns
											.GroupBy(c => { return c.Name; })
											.Select(g => { return g.ElementAt(0); });
			foreach (ColumnSchema column in columns)
			{
				string columnName = column.IsPrimaryKey ? "ID" : column.Name;
				FieldDeclaration field = new FieldDeclaration(columnName, typeof(string))
									.IsPublic()
									.InitializeTo("\"" + columnName + "\"");
				field.AddComment("This column is of type {0}{1}({2} in .NET) and can{3} be null.",
					column.SqlType.ToString().ToLower(),
					column.Length > 0 ? String.Format("({0})", column.Length) : "",
					column.DataType.Name,
					column.Nullable ? "" : "NOT");
				cols.AddField(field);

			}
			classDecl.AddClass(cols);

			FieldDeclaration columnsField = new FieldDeclaration("_columns", new CodeDomTypeReference(cols.Name)).IsStatic();
			columnsField.AddInitializer(new CodeDomTypeReference(cols.Name));
			PropertyDeclaration columnsProperty =
				new PropertyDeclaration("Columns", columnsField, new CodeDomTypeReference(cols.Name))
				.IsStatic().IsReadOnly();
			columnsProperty.AddComment("Gets an instance of the {0} class which contains all of the column names for {1}.", cols.Name, classDecl.Name);
			classDecl.AddProperty(columnsProperty);
		}

	}
}