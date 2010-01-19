using System;
using HyperActive.Dominator;
using HyperActive.SchemaProber;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// LazyActiveRecordGenerator generates some extra attribute args on hasmany attributes that indicate the resulting class will use lazy loading on those properties.
	/// </summary>
	public class LazyActiveRecordGenerator : SimpleActiveRecordGenerator
	{
		/// <summary>
		/// Creates a property that has a HasManyAttribute.
		/// </summary>
		/// <param name="classDecl">The class containing the property.</param>
		/// <param name="foreignkey">The foreign key to use.</param>
		/// <returns>A prpoerty with a hasmany attribute.</returns>
		protected override PropertyDeclaration CreateForeignKeyReference(ClassDeclaration classDecl, ForeignKeyColumnSchema foreignkey)
		{
			PropertyDeclaration fk = base.CreateForeignKeyReference(classDecl, foreignkey);
			if (fk == null)
				return null;

			AttributeDeclaration hasmany = fk.Attributes.Find(delegate(AttributeDeclaration attr) { return attr.TypeReference.IsTypeOf("Castle.ActiveRecord.HasManyAttribute"); });
			if (hasmany != null)
				hasmany.AddArgument("Lazy=true, Inverse=false");
			return fk;
		}
	}
}
