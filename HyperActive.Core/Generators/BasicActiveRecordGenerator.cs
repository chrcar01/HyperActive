using System;
using System.Collections.Generic;
using System.Text;
using HyperActive.Dominator;
using HyperActive.SchemaProber;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// Generates ActiveRecordClasses without relationships.
	/// </summary>
	public class BasicActiveRecordGenerator : ActiveRecordGenerator
	{
		
		/// <summary>
		/// Creates a ClassDeclaration.
		/// </summary>
		/// <param name="namespaceDeclaration">The NamespaceDeclaration instance the class is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		/// <returns>A reference to the ClassDeclaration instance that was created.</returns>
		protected override ClassDeclaration CreateClass(NamespaceDeclaration namespaceDeclaration, TableSchema table)
		{

			ClassDeclaration result = namespaceDeclaration.AddClass(this.NameProvider.GetClassName(table), true);
			ClassDeclaration baseType = new ClassDeclaration(this.ConfigOptions.BaseTypeName, new CodeDomTypeReference(result.FullName));
			result.InheritsFrom(baseType);
			return result;
		}

		/// <summary>
		/// Adds a prrimary key property to the ClassDeclaration.
		/// </summary>
		/// <param name="classDeclaration">The ClassDeclaration instance the property is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		protected override PropertyDeclaration CreatePrimaryKeyProperty(ClassDeclaration classDeclaration, TableSchema table)
		{
			PrimaryKeyColumnSchema pk = table.PrimaryKey;
			if (pk == null)
				return null;

			string propertyName = "Id"; 

			string fieldName = this.NameProvider.GetFieldName(pk);
			PropertyDeclaration pkprop = classDeclaration.AddProperty(propertyName, fieldName, pk.DataType);
			AttributeDeclaration pkatrrib = pkprop.AddAttribute("Castle.ActiveRecord.PrimaryKeyAttribute");
			// TODO: Remove this shit code
			if (pk.IsIdentity)
				pkatrrib.AddArgument("Castle.ActiveRecord.PrimaryKeyType.Identity");
			else
				pkatrrib.AddArgument("Castle.ActiveRecord.PrimaryKeyType.Assigned");
			pkatrrib.AddQuotedArgument((pk.Name));
			return pkprop;
		}

		/// <summary>
		/// Adds an attribute to a ClassDeclaration.
		/// </summary>
		/// <param name="classDeclaration">The ClassDeclaration instance the property is added to.</param>
		/// <param name="table">The database table the ClassDeclaration represents.</param>
		protected override void AddClassAttributes(ClassDeclaration classDeclaration, TableSchema table)
		{
			classDeclaration.AddAttribute("Castle.ActiveRecord.ActiveRecordAttribute").AddQuotedArgument(table.Name);
		}

		/// <summary>
		/// Creates a property that is a foreign key within the current table that points to a primary key in another table.  These typically have BelongsTo attributes.
		/// </summary>
		/// <param name="classDeclaration">The class to which the property is added.</param>
		/// <param name="foreignKey">The foreign key the property represents.</param>
		/// <returns>The foreign key property that was added.</returns>
		protected override PropertyDeclaration CreateForeignKey(ClassDeclaration classDeclaration, ForeignKeyColumnSchema foreignKey)
		{
			//no foreign keys
			return null;
		}

		/// <summary>
		/// Creates a property that has many children that point to the primary key contained within this class.  Typically these will have HasMany attributes.
		/// </summary>
		/// <param name="classDecl">The class to which the property is added.</param>
		/// <param name="foreignKey">The foreign key in the table that references the primary key in this class.</param>
		/// <returns>The property that was added.</returns>
		protected override PropertyDeclaration CreateForeignKeyReference(ClassDeclaration classDecl, ForeignKeyColumnSchema foreignKey)
		{
			//no foreign key references
			return null;
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
			classDeclaration.AddProperty(propertyName, fieldName, column.DataType, column.Nullable)
					.AddAttribute("Castle.ActiveRecord.PropertyAttribute")
					.AddQuotedArgument(column.Name);
		}

		protected override void CreateAssociationProperties(ClassDeclaration classDeclaration, TableSchema table)
		{
			//throw new NotImplementedException();
		}
	}
}
