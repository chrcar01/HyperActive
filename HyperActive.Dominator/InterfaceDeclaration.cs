using System;
using System.Collections.Generic;
using System.CodeDom;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wraps the creation of an interface.
	/// </summary>
	public class InterfaceDeclaration : ICodeDom<CodeTypeDeclaration>
	{
		private string _name;
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
			}
		}
		private List<CodeDomTypeReference> _typeParameters = new List<CodeDomTypeReference>();
		/// <summary>
		/// Gets the type parameters.
		/// </summary>
		public List<CodeDomTypeReference> TypeParameters
		{
			get
			{
				return _typeParameters;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="InterfaceDeclaration"/> class.
		/// </summary>
		/// <param name="name">The name of the interface.</param>
		public InterfaceDeclaration(string name)
		{
			_name = name;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="InterfaceDeclaration"/> class.
		/// </summary>
		/// <param name="name">The name of the interface.</param>
		/// <param name="typeParameters">The type parameters for the interface.</param>
		public InterfaceDeclaration(string name, params CodeDomTypeReference[] typeParameters)
			: this(name)
		{			
			if (typeParameters == null || typeParameters.Length == 0)
				return;

			this.TypeParameters.AddRange(typeParameters);
		}
		/// <summary>
		/// Toes the code type reference.
		/// </summary>
		/// <returns></returns>
		public CodeTypeReference ToCodeTypeReference()
		{
			CodeTypeReference result = new CodeTypeReference(this.Name);
			if (this.TypeParameters.Count > 0)
			{
				foreach (CodeDomTypeReference typeParameter in this.TypeParameters)
				{
					result.TypeArguments.Add(typeParameter.ToString());
				}
			}
			return result;
		}
		/// <summary>
		/// When called on implementing class, a CodeObject representing the class is returned.
		/// </summary>
		/// <returns></returns>
		public CodeTypeDeclaration ToCodeDom()
		{
			CodeTypeDeclaration result = new CodeTypeDeclaration(this.Name);
			result.IsInterface = true;
			result.Attributes = MemberAttributes.Public;
			if (this.TypeParameters.Count > 0)
			{
				foreach (CodeDomTypeReference typeRef in this.TypeParameters)
				{
					CodeTypeParameter ctp = new CodeTypeParameter(typeRef.ToString());
					result.TypeParameters.Add(ctp);
				}
			}
			return result;
		}
	}
}
