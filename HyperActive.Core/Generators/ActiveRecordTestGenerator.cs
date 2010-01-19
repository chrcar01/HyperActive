using System;
using System.Collections.Generic;
using System.Text;
using HyperActive.Dominator;
using HyperActive.SchemaProber;
using System.IO;

namespace HyperActive.Core.Generators
{
	/// <summary>
	/// Generates an NUnit TestFixture that tests an ActiveRecord class.
	/// </summary>
	public class ActiveRecordTestGenerator
	{
		private NameProvider _nameProvider = null;
		/// <summary>
		/// Gets or sets the NameProvider to use when naming code elements.
		/// </summary>
		public NameProvider NameProvider
		{
			get
			{
				return _nameProvider;
			}
			protected set
			{
				_nameProvider = value;
			}
		}
		private IValueProvider _valueProvider = null;
		/// <summary>
		/// Gets or sets the ValueProvider used when generating test values.
		/// </summary>
		public IValueProvider ValueProvider
		{
			get
			{
				return _valueProvider;
			}
			protected set
			{
				_valueProvider = value;
			}
		}
		/// <summary>
		/// Initializes a new instance ofthe ActiveRecordTestGenerator class.
		/// </summary>
		public ActiveRecordTestGenerator()
			: this(new NameProvider(), new ValueProviderImpl())
		{
		}
		/// <summary>
		/// Initializes a new instance of the ActiveRecordTestGenerator class.
		/// </summary>
		/// <param name="nameProvider">The name provider to use when generating code.</param>
        public ActiveRecordTestGenerator(NameProvider nameProvider)
			: this(nameProvider, new ValueProviderImpl())
		{			
		}
		/// <summary>
		/// Initializes a new instance of the ActiveRecordTestGenerator class.
		/// </summary>
		/// <param name="nameProvider">The name provider to use when generating code.</param>
		/// <param name="valueProvider">The value provider to use when generating test values.</param>
		public ActiveRecordTestGenerator(NameProvider nameProvider, IValueProvider valueProvider)
		{
			_nameProvider = nameProvider;
			_valueProvider = valueProvider;
		}
		/// <summary>
		/// Generates the test class for a table.
		/// </summary>
		/// <param name="path">The path where the code will be written.</param>
		/// <param name="namespaceName">The namespace the class will belong.</param>
		/// <param name="table">The table to generate test code for.</param>
		public void Generate(string path, string namespaceName, TableSchema table)
		{
			using (StreamWriter fileWriter = new StreamWriter(path, false))
			using (StringWriter codeWriter = new StringWriter())
            {
				this.Generate(codeWriter, namespaceName, table);
				fileWriter.Write(codeWriter.ToString());
            }
		}
		/// <summary>
		/// Generates the test class for a table.
		/// </summary>
		/// <param name="writer">The writer to which the code is written.</param>
		/// <param name="namespaceName">The namespace containing the test class.</param>
		/// <param name="table">The table being tested.</param>
		public void Generate(TextWriter writer, string namespaceName, TableSchema table)
		{
			if (table == null)
				throw new ArgumentNullException("table");

			NamespaceDeclaration nstest = new NamespaceDeclaration(namespaceName + ".Tests");

			//declare the test class
			string typeName = this.NameProvider.GetClassName(table.Name);
			ClassDeclaration testclass = nstest.AddClass(typeName + "Tests", true);
			string targetTypeName = namespaceName + "." + typeName;
			testclass.InheritsFrom(new ClassDeclaration("ActiveRecordBaseTest", new CodeDomTypeReference(targetTypeName)));

			//create the test method
			MethodDeclaration testmeth = testclass.AddMethod(String.Format("Verify{0}", typeName)).Public();
			testmeth.AddAttribute("NUnit.Framework.TestAttribute");
			string fullName = namespaceName + "." + typeName;

			
			string fieldName = this.NameProvider.GetClassName(table.Name).Substring(0, 1).ToLower() + this.NameProvider.GetClassName(table.Name).Substring(1);
			testmeth.Declare(fieldName).New(new CodeDomTypeReference(fullName));//generates: Customer customer = new Customer();
			foreach (ColumnSchema column in table.NonKeyColumns)
			{
				string propertyName = this.NameProvider.GetPropertyName(column);
				string propertyValue = this.ValueProvider.GetValue(column);
				testmeth.Assign(fieldName).Property(propertyName).To(propertyValue);
			}
			testmeth.Call(fieldName, "Save");
			nstest.Imports("System");
			CodeBuilder builder = new CodeBuilder();
			builder.GenerateCode(writer, nstest);
		}

	}
}
