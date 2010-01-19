using System;
using System.Collections.Generic;
using System.CodeDom;

namespace HyperActive.Dominator
{
	/// <summary>
	/// Wrapper for the CodeTypeDeclaration.
	/// </summary>
	public class ClassDeclaration : ICodeDom<CodeTypeDeclaration>
	{
		private bool _isAbstract = false;
		private List<FieldDeclaration> _fields = new List<FieldDeclaration>();
		private PropertyDeclarationList _properties = new PropertyDeclarationList();
		/// <summary>
		/// Gets the properties defined in this ClassDeclaration instance.
		/// </summary>
		public PropertyDeclarationList Properties
		{
			get
			{
				return _properties;
			}
		}

		private List<ConstructorDeclaration> _constructors = null;
		/// <summary>
		/// Gets the list of constructors defined for this class declaration.
		/// </summary>
		public List<ConstructorDeclaration> Constructors
		{
			get
			{
				if (_constructors == null)
					_constructors = new List<ConstructorDeclaration>();
				return _constructors;
			}
		}

		/// <summary>
		/// Adds a constrcutor with the specified args.  No args indicates that the constructor 
		/// will be the default constructor.
		/// </summary>
		/// <returns>The constructor that was added.</returns>
		public ConstructorDeclaration AddConstructor(params ConstructorArg[] args)
		{
			string comment = String.Empty;
			return AddConstructor(comment, args);
		}
        /// <summary>
		/// Adds a constrcutor with the specified args.  No args indicates that the constructor 
		/// will be the default constructor.
		/// </summary>
		/// <returns>The constructor that was added.</returns>
		public ConstructorDeclaration AddConstructor(string comment, params ConstructorArg[] args)
		{
			ConstructorDeclaration result = new ConstructorDeclaration(this.Name, args);
			if (!String.IsNullOrEmpty(comment))
				result.Comments.Add(comment);
			this.Constructors.Add(result);
			return result;
		}

		/// <summary>
		/// Adds a constructor to the class declaration.
		/// </summary>
		/// <param name="argType">The type of the argument.</param>
		/// <param name="argName">The name of the argument.</param>
		/// <param name="fieldName">The field name the to which argument maps.</param>
		/// <returns>The constructor that was added to the class declaration.</returns>
		public ConstructorDeclaration AddConstructor(System.Type argType, string argName, string fieldName)
		{
			string comment = String.Empty;
			return AddConstructor(argType, argName, fieldName, comment);
		}
        /// <summary>
		/// Adds a constructor to the class declaration.
		/// </summary>
		/// <param name="argType">The type of the argument.</param>
		/// <param name="argName">The name of the argument.</param>
		/// <param name="fieldName">The field name the to which argument maps.</param>
		/// <param name="comment">The comment for the argument.</param>
		/// <returns>The constructor that was added to the class declaration.</returns>
		public ConstructorDeclaration AddConstructor(System.Type argType, string argName, string fieldName, string comment)
		{
			ConstructorArg arg = new ConstructorArg(argType, argName, fieldName, comment);
			return this.AddConstructor(arg);
		}
		/// <summary>
		/// Adds the constructor.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <param name="argName">Name of the arg.</param>
		/// <returns></returns>
		public ConstructorDeclaration AddConstructor(string typeName, string argName)
		{
			return this.AddConstructor(new ConstructorArg(typeName, argName));
		}

		/// <summary>
		/// Adds a property to the class declaration.  The field will be added from the property to the class declaration.
		/// </summary>
		/// <param name="property">The PropertyDeclaration to add to the class.</param>
		/// <returns>The property that was passed to the method.</returns>
		public PropertyDeclaration AddProperty(PropertyDeclaration property)
		{
			this._properties.Add(property);
			this._fields.Add(property.Field);
			return property;
		}
		/// <summary>
		/// Adds a property to the class declaration.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="fieldName">Name of the field the property encapsulates.</param>
		/// <param name="typeReference">The type of property.</param>
		/// <returns>The property that was added to the current class declarartion.</returns>
		public PropertyDeclaration AddProperty(string propertyName, string fieldName, CodeDomTypeReference typeReference)
		{
			PropertyDeclaration property = _properties.Find(delegate(PropertyDeclaration p) { return p.Name.Equals(propertyName); });
			if (property != null)
				return property;

			FieldDeclaration field = new FieldDeclaration(fieldName, typeReference);
			this._fields.Add(field);
			property = new PropertyDeclaration(propertyName, field, typeReference);
			this._properties.Add(property);
			return property;
		}

		/// <summary>
		/// Adds a property to the class declaration.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="fieldName">Name of the field the property encapsulates.</param>
		/// <param name="typeName">Name of the property type.</param>
		/// <param name="typeParameters">Any type parameters the type needs.</param>
		/// <returns>The property that was added to the current class declaration.</returns>
		public PropertyDeclaration AddProperty(string propertyName, string fieldName, string typeName, params string[] typeParameters)
		{
			return this.AddProperty(propertyName, fieldName, new CodeDomTypeReference(typeName, typeParameters));
		}

		/// <summary>
		/// Adds a property to the class declaration.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="fieldName">Name of the field the property encapsulates.</param>
		/// <param name="type">The property type.</param>
		/// <returns>The property declaration that was added.</returns>
		public PropertyDeclaration AddProperty(string propertyName, string fieldName, Type type)
		{
			return this.AddProperty(propertyName, fieldName, new CodeDomTypeReference(type));
		}

		/// <summary>
		/// Adds a property to the class declaration.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="fieldName">Name of the field the property encapsulates.</param>
		/// <param name="type">The property type.</param>
		/// <param name="nullable">Indicates whether the database column is nullable or not.</param>
		/// <returns>The property that was added to the current class declaration.</returns>
		public PropertyDeclaration AddProperty(string propertyName, string fieldName, Type type, bool nullable)
		{
			CodeDomTypeReference typeRef = new CodeDomTypeReference(type);
			if (nullable) typeRef.IsNullable();
			return this.AddProperty(propertyName, fieldName, typeRef);
		}
		private List<MethodDeclaration> _methods = new List<MethodDeclaration>();

		/// <summary>
		/// Adds a method to a class declaration.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <returns>The method that was added to the current class declaration.</returns>
		public MethodDeclaration AddMethod(string methodName)
		{
			CodeDomTypeReference returnType = null;
			return AddMethod(methodName, returnType);
		}
        /// <summary>
		/// Adds a method to a class declaration.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="returnType">The type the method returns.</param>
		/// <returns>The method that was added to the current class declaration.</returns>
		public MethodDeclaration AddMethod(string methodName, CodeDomTypeReference returnType)
		{
			MethodDeclaration result = new MethodDeclaration(methodName, returnType);
			_methods.Add(result);
			return result;
		}

		/// <summary>
		/// Adds a method to a class declaration.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="type">The type the method returns.</param>
		/// <returns>The method that was added to the current class declaration.</returns>
		public MethodDeclaration AddMethod(string methodName, Type type)
		{
			return this.AddMethod(methodName, new CodeDomTypeReference(type));
		}


		private List<AttributeDeclaration> _attributes = new List<AttributeDeclaration>();
		/// <summary>
		/// Adds an attribute to the class.
		/// </summary>
		/// <param name="attributeType">Attribute to add to the class.</param>
		/// <returns>The attribute that was added to the class.</returns>
		public AttributeDeclaration AddAttribute(CodeDomTypeReference attributeType)
		{
			if (attributeType == null)
				throw new ArgumentNullException("attributeType");

			AttributeDeclaration result = new AttributeDeclaration(attributeType);
			_attributes.Add(result);
			return result;
		}
		/// <summary>
		/// Adds an attribute to the class.
		/// </summary>
		/// <param name="typeName">Type of attribute.</param>
		/// <param name="typeParameters">Attribute type parameters.</param>
		/// <returns>The attribute that was added to the class.</returns>
		public AttributeDeclaration AddAttribute(string typeName, params string[] typeParameters)
		{
			if (String.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");

			return this.AddAttribute(new CodeDomTypeReference(typeName, typeParameters));
		}
		/// <summary>
		/// Adds an attribute to the class.
		/// </summary>
		/// <param name="type">The type of attribute.</param>
		/// <returns>The attribute that was added to the class.</returns>
		public AttributeDeclaration AddAttribute(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return this.AddAttribute(new CodeDomTypeReference(type));
		}
		private ClassDeclaration _baseType;
		private NamespaceDeclaration _namespace = null;
		/// <summary>
		/// Gets the NamespaceDeclaration this class belongs.
		/// </summary>
		public NamespaceDeclaration Namespace
		{
			get
			{
				return _namespace;
			}
		}

		/// <summary>
		/// Sets the type that the generated class will be derived from.
		/// </summary>
		/// <returns>The current ClassDeclaration with the addition of the base type.</returns>
		public ClassDeclaration InheritsFrom(ClassDeclaration baseType)
		{
			_baseType = baseType;
			return this;
		}

		/// <summary>
		/// Sets the type that the generated class will be derived from.
		/// </summary>
		/// <param name="baseTypeName">Name of the base type.</param>
		/// <param name="baseTypeParameters">Type parameters for the base type if it is generic.</param>
		/// <returns>The current ClassDeclaration with the addition of the base type.</returns>
		public ClassDeclaration InheritsFrom(string baseTypeName, params string[] baseTypeParameters)
		{
			List<CodeDomTypeReference> typerefs = new List<CodeDomTypeReference>();
			foreach (string baseTypeParameter in baseTypeParameters)
			{
				typerefs.Add(new CodeDomTypeReference(baseTypeParameter));
			}
			_baseType = new ClassDeclaration(baseTypeName, typerefs.ToArray());
			return this;
		}

		private bool _isPartial;
		/// <summary>
		/// Gets or sets a value indicating whether the class is a partial class.
		/// </summary>
		public bool IsPartial
		{
			get { return _isPartial; }
			set
			{
				_isPartial = value;
			}
		}
		private string _name;
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
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
		/// Marks this instance as being an abstract class.
		/// </summary>
		/// <returns>This instance marked as being abstract.</returns>
		public ClassDeclaration IsAbstract()
		{
			_isAbstract = true;
			return this;
		}
		private Type _type = null;
		/// <summary>
		/// Gets the type of this class.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		/// <summary>
		/// Initializes a new instance of the ClassDeclaration class.
		/// </summary>
		/// <param name="nsdecl">The namespace declaration containing this class.</param>
		/// <param name="name">The name of the class.</param>
		/// <param name="typeParameters">Any parameters the type will need to be defined.</param>
		public ClassDeclaration(NamespaceDeclaration nsdecl, string name, params string[] typeParameters)
		{
			_name = name;
			_namespace = nsdecl;
			foreach (string typeParameter in typeParameters)
			{
				_typeParameters.Add(new CodeDomTypeReference(typeParameter));
			}
		}

		/// <summary>
		/// Initializes a new instance of the ClassDeclaration class.
		/// </summary>
		/// <param name="name">Name of the type that will be generated.</param>
		/// <param name="typeParameters">Any parameters the type will need to be defined.</param>
		public ClassDeclaration(string name, params CodeDomTypeReference[] typeParameters)
		{
			_name = name;
			_typeParameters.AddRange(typeParameters);
		}

		/// <summary>
		/// Gets the full name of this class.
		/// </summary>
		public string FullName
		{
			get
			{
				if (_type != null)
					return _type.FullName;
				if (this.Namespace != null)
					return String.Format("{0}.{1}", this.Namespace.Name, this.Name);
				return this.Name;
			}
		}
		/// <summary>
		/// Returns a CodeTypeReference pointing to this class.
		/// </summary>
		/// <returns>CodeTypeReference referencing this class.</returns>
		public CodeTypeReference ToCodeTypeReference()
		{
			CodeTypeReference result = (_type != null ? new CodeTypeReference(_type) : new CodeTypeReference(this.FullName));
			if (this.TypeParameters.Count > 0)
			{
				foreach (CodeDomTypeReference typeRef in this.TypeParameters)
				{
					result.TypeArguments.Add(typeRef.ToString());
				}
			}
			return result;
		}
		/// <summary>
		/// Converts this ClassDeclaration to a CodeTypeDeclaration.
		/// </summary>
		/// <returns>The CodeTypeDeclaration representation of this class.</returns>
		public CodeTypeDeclaration ToCodeDom()
		{
			CodeTypeDeclaration result = new CodeTypeDeclaration(this.Name);
			result.IsPartial = this.IsPartial;
			if (this._isAbstract)
				result.TypeAttributes = System.Reflection.TypeAttributes.Abstract | result.TypeAttributes;
			if (_baseType != null)
			{
				result.BaseTypes.Add(_baseType.ToCodeTypeReference());
			}
			if (this.Structs.Count > 0)
			{
				foreach (StructDeclaration structDeclaration in this.Structs)
				{
					result.Members.Add(structDeclaration.ToCodeDom());
				}
			}
			if (this.Classes.Count > 0)
			{
				foreach (ClassDeclaration klass in this.Classes)
				{
					result.Members.Add(klass.ToCodeDom());
				}
			}
			if (this._interfaces != null && this._interfaces.Count > 0)
			{
				foreach (InterfaceDeclaration iface in this._interfaces)
				{
					result.BaseTypes.Add(iface.ToCodeTypeReference());
				}
			}
			foreach (ConstructorDeclaration ctor in this.Constructors)
			{
				result.Members.Add(ctor.ToCodeDom());
			}
			
			if (this.TypeParameters.Count > 0)
			{
				foreach (CodeDomTypeReference typeRef in this.TypeParameters)
				{
					CodeTypeParameter ctp = new CodeTypeParameter(typeRef.ToString());
					result.TypeParameters.Add(ctp);
				}
			}
			foreach(AttributeDeclaration attr in this._attributes)
			{
				result.CustomAttributes.Add(attr.ToCodeDom());
			}
			foreach (FieldDeclaration fld in this._fields)
			{
				result.Members.Add(fld.ToCodeDom());
			}
			foreach (PropertyDeclaration prop in this._properties)
			{
				result.Members.Add(prop.ToCodeDom());
			}
			foreach (MethodDeclaration method in this._methods)
			{
				result.Members.Add(method.ToCodeDom());
			}
			if (this.Members.Count > 0)
			{
				result.Members.AddRange(this.Members.ToArray());
			}
			return result;
		}


		/// <summary>
		/// Implementses the specified interface declaration.
		/// </summary>
		/// <param name="interfaceDeclaration">The interface declaration.</param>
		/// <returns></returns>
		public ClassDeclaration Implements(InterfaceDeclaration interfaceDeclaration)
		{
			return this.AddInterface(interfaceDeclaration);
		}
		private List<InterfaceDeclaration> _interfaces = new List<InterfaceDeclaration>();
		/// <summary>
		/// Adds the interface.
		/// </summary>
		/// <param name="interfaceDeclaration">The interface declaration.</param>
		/// <returns></returns>
		public ClassDeclaration AddInterface(InterfaceDeclaration interfaceDeclaration)
		{
			this._interfaces.Add(interfaceDeclaration);
			return this;
		}
		private List<StructDeclaration> _structs;

		/// <summary>
		/// Gets the structs contained in this class declaration.
		/// </summary>
		/// <value>The structs.</value>
		public List<StructDeclaration> Structs
		{
			get
			{
				if (_structs == null)
					_structs = new List<StructDeclaration>();
				return _structs;
			}
		}
		/// <summary>
		/// Adds the struct to this class declaration.
		/// </summary>
		/// <param name="structDeclaration">The struct declaration.</param>
		/// <returns></returns>
		public ClassDeclaration AddStruct(StructDeclaration structDeclaration)
		{
			this.Structs.Add(structDeclaration);
			return this;
		}

		private List<ClassDeclaration> _classes = null;

		/// <summary>
		/// Gets the classes.
		/// </summary>
		/// <value>The classes.</value>
		public List<ClassDeclaration> Classes
		{
			get
			{
				if (_classes == null)
					_classes = new List<ClassDeclaration>();
				return _classes;
			}
		}
		/// <summary>
		/// Adds the class.
		/// </summary>
		/// <param name="klass">The klass.</param>
		/// <returns></returns>
		public ClassDeclaration AddClass(ClassDeclaration klass)
		{
			this.Classes.Add(klass);
			return this;
		}

		/// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public ClassDeclaration AddField(FieldDeclaration field)
		{
			this._fields.Add(field);
			return this;
		}

		private List<CodeTypeMember> _members;
		/// <summary>
		/// Gets a list of raw <see cref="CodeTypeMember"/> instances that will
		/// be added to the generated type.
		/// </summary>
		/// <value>The members to add.</value>
		public List<CodeTypeMember> Members
		{
        	get
            {
				if (_members == null)
				{
					_members = new List<CodeTypeMember>();
				}
            	return _members;
            }
        }
		/// <summary>
		/// Adds the member.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <returns></returns>
		public ClassDeclaration AddMember(CodeTypeMember member)
		{
			this.Members.Add(member);
			return this;
		}
	}
}
