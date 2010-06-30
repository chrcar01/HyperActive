using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyperActive.Dominator;
using HyperActive.SchemaProber;
using HyperActive.Core.Config;
using System.CodeDom;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// 
	/// </summary>
	public class ConfigurableActiveRecordGenerator : ActiveRecordGenerator
	{
		/// <summary>
		/// Initializes a new instance of the ConfigurableActiveRecordGenerator class.
		/// </summary>
		/// <param name="configOptions"></param>
		public ConfigurableActiveRecordGenerator(IConfigurationOptions configOptions)
		{
			base.ConfigOptions = configOptions;
		}
		

		/// <summary>
		/// Adds a prrimary key property to the ClassDeclaration.
		/// </summary>
		/// <param name="classDeclaration">The ClassDeclaration instance the property is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		/// <returns></returns>
		protected override PropertyDeclaration CreatePrimaryKeyProperty(ClassDeclaration classDeclaration, TableSchema table)
		{
			PrimaryKeyColumnSchema pk = table.PrimaryKey;
			string propertyName = String.IsNullOrEmpty(this.ConfigOptions.StaticPrimaryKeyName) ? this.NameProvider.GetPropertyName(pk) : this.ConfigOptions.StaticPrimaryKeyName;
			string fieldName = String.IsNullOrEmpty(this.ConfigOptions.StaticPrimaryKeyName) ? this.NameProvider.GetFieldName(pk) : this.NameProvider.GetFieldName(this.ConfigOptions.StaticPrimaryKeyName);
			PropertyDeclaration pkprop = classDeclaration.AddProperty(propertyName, fieldName, pk.DataType);
			AttributeDeclaration pkatrrib = pkprop.AddAttribute("Castle.ActiveRecord.PrimaryKeyAttribute");
			
			if (pk.IsIdentity)
				pkatrrib.AddArgument("Castle.ActiveRecord.PrimaryKeyType.Identity");
			else
			{
				if (pk.SqlType == System.Data.SqlDbType.UniqueIdentifier)
					pkatrrib.AddArgument("Castle.ActiveRecord.PrimaryKeyType.GuidComb");
				else
					pkatrrib.AddArgument("Castle.ActiveRecord.PrimaryKeyType.Assigned");
			}
			pkatrrib.AddQuotedArgument((pk.Name));
			if (this.ConfigOptions.GenerateComments)
			{
				pkprop.AddComment("This is the primary key and maps to {0}", table.PrimaryKey.Name);
			}
			return pkprop;
		}

		/// <summary>
		/// Adds an attribute to a ClassDeclaration.
		/// </summary>
		/// <param name="classDeclaration">The ClassDeclaration instance the property is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		protected override void AddClassAttributes(ClassDeclaration classDeclaration, TableSchema table)
		{
			classDeclaration.AddAttribute("Castle.ActiveRecord.ActiveRecordAttribute").AddQuotedArgument(this.NameProvider.Escape(table.Name));
		}

		/// <summary>
		/// Creates a ClassDeclaration.
		/// </summary>
		/// <param name="namespaceDeclaration">The NamespaceDeclaration instance the class is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		/// <returns>
		/// A reference to the ClassDeclaration instance that was created.
		/// </returns>
		protected override ClassDeclaration CreateClass(NamespaceDeclaration namespaceDeclaration, TableSchema table)
		{
			var result = namespaceDeclaration.AddClass(this.NameProvider.GetClassName(table), true);

			string baseTypeName = String.IsNullOrEmpty(ConfigOptions.AbstractBaseName)
				? ConfigOptions.BaseTypeName
				: ConfigOptions.AbstractBaseName;

			var baseType = new ClassDeclaration(baseTypeName, new CodeDomTypeReference(result.FullName));
			result.InheritsFrom(baseType);
			return result;
		}

		/// <summary>
		/// Creates a property that is a foreign key within the current table that points to a primary key in another table.  These typically have BelongsTo attributes.
		/// </summary>
		/// <param name="classDeclaration">The class to which the property is added.</param>
		/// <param name="foreignKey">The foreign key the property represents.</param>
		/// <returns>The foreign key property that was added.</returns>
		protected override PropertyDeclaration CreateForeignKey(ClassDeclaration classDeclaration, ForeignKeyColumnSchema foreignKey)
		{
			string columnName = foreignKey.Name.Replace("ID", "").Replace("_id", "");
			string fkname = this.NameProvider.GetPropertyName(columnName);
			string fkfieldname = this.NameProvider.GetFieldName(columnName);
			string fkdatatype = this.NameProvider.GetClassName(foreignKey.PrimaryKeyTable);
			PropertyDeclaration result = classDeclaration.AddProperty(fkname, fkfieldname, fkdatatype);
			result.AddAttribute("Castle.ActiveRecord.BelongsToAttribute").AddQuotedArgument(foreignKey.Name);
			return result;
		}

		/// <summary>
		/// Creates a property that has many children that point to the primary key contained within this class.  Typically these will have HasMany attributes.
		/// </summary>
		/// <param name="classDecl">The class to which the property is added.</param>
		/// <param name="foreignkey">The foreign key in the table that references the primary key in this class.</param>
		/// <returns>The property that was added.</returns>
		protected override PropertyDeclaration CreateForeignKeyReference(ClassDeclaration classDecl, ForeignKeyColumnSchema foreignkey)
		{
			if (foreignkey.Table.PrimaryKey == null)
				return null;

			string propertyName = this.NameProvider.GetListPropertyName(foreignkey);
			string fieldName = this.NameProvider.GetListFieldName(foreignkey);
			string typeParam = this.NameProvider.GetClassName(foreignkey.Table);
			CodeDomTypeReference genericList = new CodeDomTypeReference("System.Collections.Generic.List").AddTypeParameters(typeParam);
			PropertyDeclaration result = classDecl.AddProperty(propertyName, fieldName, "System.Collections.Generic.IList");
			
			result.AddTypeParameter(typeParam)
				.AddAttribute("Castle.ActiveRecord.HasManyAttribute")
				.AddArgument("typeof(" + this.NameProvider.GetClassName(foreignkey.Table) + ")")
				.AddArgument("Lazy=true");

			return result;
		}

		/// <summary>
		/// Creates a plain ol' property representing any column that is not a foreign key or primary key.
		/// </summary>
		/// <param name="classDeclaration">The class to which the property is added.</param>
		/// <param name="column">The column the property represents.</param>
		protected override void CreateNonKeyProperty(ClassDeclaration classDeclaration, ColumnSchema column)
		{
			string propertyName = this.NameProvider.GetPropertyName(column);
			string fieldName = this.NameProvider.GetFieldName(column);
			PropertyDeclaration prop = classDeclaration.AddProperty(propertyName, fieldName, column.DataType, column.Nullable);

			if (this.ConfigOptions.GenerateComments)
			{
				prop.AddComment("Gets or sets the {0} of the {1}.", this.NameProvider.GetPropertyName(column), classDeclaration.Name);
				if (column.Nullable)
					prop.AddComment("This property is nullable.");
				else
					prop.AddComment("This property cannot be null.");
				if (column.DataType == typeof(string))
					prop.AddComment("The max length of this property is {0}.", column.Length);
			}

			AttributeDeclaration propAttrib = prop.AddAttribute("Castle.ActiveRecord.PropertyAttribute")
					.AddQuotedArgument(column.Name)
					.AddArgument("Length=" + (column.Length < 0 ? 0 : column.Length).ToString())
					.AddArgument("NotNull=" + (column.Nullable == false).ToString().ToLower());

			if (column.SqlType == System.Data.SqlDbType.Image)
				propAttrib.AddArgument("ColumnType=\"BinaryBlob\"");

			if (column.SqlType == System.Data.SqlDbType.Xml)
				propAttrib.AddArgument("ColumnType=\"StringClob\"");
						
		}
		protected override ClassDeclaration CreateConstructors(ClassDeclaration classDecl, TableSchema table)
		{
			base.CreateConstructors(classDecl, table);
			MethodDeclaration method = classDecl.AddMethod("Find", new CodeDomTypeReference(this.NameProvider.GetClassName(table)));
			method.MethodAttributes.Add(MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final);
			method.AddParameter(typeof(int), "id");
			method.Returns("FindByPrimaryKey").With("id", "false");
			return classDecl;
		}
		protected override void CreateAssociationProperties(ClassDeclaration classDeclaration, TableSchema table)
		{
			if (classDeclaration == null || table == null || table.Associations.Count == 0)
				return;

			foreach (var assocTable in table.Associations)
			{
				Console.WriteLine("assocTable.Name=" + assocTable.Name);
				var otherTable = assocTable.ForeignKeys.Single(fk => { return fk.PrimaryKeyTable.Name != table.Name; }).PrimaryKeyTable;

				string propName = Inflector.Pluralize(this.NameProvider.GetPropertyName(otherTable.Name));
				string fieldName = Inflector.Pluralize(this.NameProvider.GetFieldName(otherTable.Name));
				var typeRef = new CodeDomTypeReference(typeof(IList<>).FullName, this.NameProvider.GetClassName(otherTable));
				var prop = classDeclaration.AddProperty(propName, fieldName, typeRef);
				prop.AddAttribute("Castle.ActiveRecord.HasAndBelongsToMany")
					.AddArgument("typeof(" + this.NameProvider.GetClassName(otherTable) + ")")
					.AddQuotedArgument("Table", assocTable.Name)
					.AddQuotedArgument("ColumnKey", assocTable.ForeignKeys.Single(fk => { return fk.PrimaryKeyTable.Name == table.Name; }).Name)
					.AddQuotedArgument("ColumnRef", assocTable.ForeignKeys.Single(fk => { return fk.PrimaryKeyTable.Name != table.Name; }).Name)
					.AddArgument("Lazy = true");


			}
		}
	}
}
