using System;
using HyperActive.Dominator;
using HyperActive.SchemaProber;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// 
	/// </summary>
	public class SimpleActiveRecordGenerator : ActiveRecordGenerator
	{	
		
		/// <summary>
		/// Creates a ClassDeclaration shell.
		/// </summary>
		/// <param name="nsdecl">NamespaceDeclaration the class is added.</param>
		/// <param name="table">Database table that the generated class will interact with.</param>
		/// <returns>The ClassDeclaration shell that was created.</returns>
		protected override ClassDeclaration CreateClass(NamespaceDeclaration nsdecl, TableSchema table)
		{

			ClassDeclaration result = nsdecl.AddClass(this.NameProvider.GetClassName(table), true);
			ClassDeclaration baseType = null;
			if (!String.IsNullOrEmpty(this.ConfigOptions.AbstractBaseName))
				baseType = new ClassDeclaration(this.ConfigOptions.AbstractBaseName, new CodeDomTypeReference(result.FullName));
			else
				baseType = new ClassDeclaration(this.ConfigOptions.BaseTypeName, new CodeDomTypeReference(result.FullName));
			result.InheritsFrom(baseType);
			return result;
		}
		/// <summary>
		/// Creates a property that maps to a table's primary key.
		/// </summary>
		/// <param name="cdecl">The ClassDeclaration that represents the database table.</param>
		/// <param name="table">The database table that contains the primary key we're mapping as a property.</param>
		protected override PropertyDeclaration CreatePrimaryKeyProperty(ClassDeclaration cdecl, TableSchema table)
		{
			PrimaryKeyColumnSchema pk = table.PrimaryKey;
			string propertyName = "ID";
			string fieldName = "_id"; // this.NameProvider.GetFieldName(pk);
			PropertyDeclaration pkprop = cdecl.AddProperty(propertyName, fieldName, pk.DataType);
			AttributeDeclaration pkatrrib = pkprop.AddAttribute("Castle.ActiveRecord.PrimaryKeyAttribute");
			// TODO: Remove this shit code
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
			return pkprop;
		}
		/// <summary>
		/// Add some class attributes.
		/// </summary>
		/// <param name="classDecl">The ClassDeclaration instance we're adding an attribute to.</param>
		/// <param name="table">The database table that the ClassDeclaration represents.</param>
		protected override void AddClassAttributes(ClassDeclaration classDecl, TableSchema table)
		{
			classDecl.AddAttribute("Castle.ActiveRecord.ActiveRecordAttribute").AddQuotedArgument(this.NameProvider.Escape(table.Name));
		}

		/// <summary>
		/// Creates a property that is a foreign key within the current table that points to a primary key in another table.  These typically have BelongsTo attributes.
		/// </summary>
		/// <param name="classDeclaration">The class to which the property is added.</param>
		/// <param name="foreignKey">The foreign key the property represents.</param>
		/// <returns>The foreign key property that was added.</returns>
		protected override PropertyDeclaration CreateForeignKey(ClassDeclaration classDeclaration, ForeignKeyColumnSchema foreignKey)
		{

			string fkname = this.NameProvider.GetPropertyName(foreignKey);
			string fkfieldname = this.NameProvider.GetFieldName(foreignKey);
			string fkdatatype = this.NameProvider.GetClassName(foreignKey.PrimaryKeyTable);
			PropertyDeclaration result = classDeclaration.AddProperty(fkname, fkfieldname, fkdatatype);
			result.AddAttribute("Castle.ActiveRecord.BelongsToAttribute").AddQuotedArgument(foreignKey.Name);
			return result;
		}

		/// <summary>
		/// Creates a property that has many children that point to the primary key contained within this class.  Typically these will have HasMany attributes.
		/// </summary>
		/// <param name="classDeclaration">The class to which the property is added.</param>
		/// <param name="foreignKey">The foreign key in the table that references the primary key in this class.</param>
		/// <returns>The property that was added.</returns>
		protected override PropertyDeclaration CreateForeignKeyReference(ClassDeclaration classDeclaration, ForeignKeyColumnSchema foreignKey)
		{
			if (foreignKey.Table.PrimaryKey == null)
				return null;

			string propertyName = this.NameProvider.GetListPropertyName(foreignKey);
			string fieldName = this.NameProvider.GetListFieldName(foreignKey);
			string typeParam = this.NameProvider.GetClassName(foreignKey.Table);
			CodeDomTypeReference genericList = new CodeDomTypeReference("System.Collections.Generic.List").AddTypeParameters(typeParam);
			PropertyDeclaration result = classDeclaration.AddProperty(propertyName, fieldName, "System.Collections.Generic.IList");
			result.InitializeField(genericList)
				.AddTypeParameter(typeParam)
				.AddAttribute("Castle.ActiveRecord.HasManyAttribute")
				.AddArgument("typeof(" + this.NameProvider.GetClassName(foreignKey.Table) + ")");
			
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
			AttributeDeclaration attrib = classDeclaration.AddProperty(propertyName, fieldName, column.DataType, column.Nullable)
					.AddAttribute("Castle.ActiveRecord.PropertyAttribute")
					.AddQuotedArgument(column.Name);
			if (column.SqlType == System.Data.SqlDbType.Image)
				attrib.AddArgument("ColumnType=\"BinaryBlob\"");
			if (column.SqlType == System.Data.SqlDbType.Xml)
				attrib.AddArgument("ColumnType=\"StringClob\"");
		}

		protected override void CreateAssociationProperties(ClassDeclaration classDeclaration, TableSchema table)
		{
			//throw new NotImplementedException();
		}
	}
}
