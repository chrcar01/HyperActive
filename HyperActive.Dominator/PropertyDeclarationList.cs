using System;
using System.Collections.Generic;
using System.Text;

namespace HyperActive.Dominator
{
	/// <summary>
	/// List of PropertyDeclarations.
	/// </summary>
	public class PropertyDeclarationList : List<PropertyDeclaration>
	{
		/// <summary>
		/// Gets a PropertyDeclaration.
		/// </summary>
		/// <param name="propertyName">Name of the PropertyDeclaration to get.</param>
		/// <returns>The property declaration matching propertyName.</returns>
		public PropertyDeclaration this[string propertyName]
		{
			get
			{
				return this.Find(delegate(PropertyDeclaration pd){ return pd.Name.Equals(propertyName); });
			}
		}
	}
}
