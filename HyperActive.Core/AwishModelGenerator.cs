using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyperActive.Core.Generators;
using HyperActive.Dominator;
using HyperActive.SchemaProber;
using HyperActive.Core;
using System.CodeDom;
using System.Threading;

namespace HyperActive.Core
{
	public class AwishModelGenerator : ActiveRecordGenerator
	{
		protected override ClassDeclaration CreateClass(NamespaceDeclaration namespaceDeclaration, TableSchema table)
		{
			ClassDeclaration result = new ClassDeclaration(this.NameProvider.GetClassName(table.Name));
			result.IsPartial = true;
			namespaceDeclaration.AddClass(result);
			return result;
		}

		protected override void CreateNonKeyProperty(ClassDeclaration classDeclaration, ColumnSchema column)
		{


			string fieldName = this.NameProvider.GetFieldName(column.Name);
			var columnDataType = new CodeTypeReference(column.DataType);
			if (column.Nullable && column.DataType.IsValueType)
			{
				columnDataType = new CodeTypeReference(typeof(System.Nullable<>));
				columnDataType.TypeArguments.Add(new CodeTypeReference(column.DataType));
			}

			var field = new CodeMemberField(columnDataType, fieldName);

			string propertyName = this.NameProvider.GetPropertyName(column.Name);
			var prop = new CodeMemberProperty();
			prop.Name = propertyName;
			prop.Attributes = MemberAttributes.Final | MemberAttributes.Public;
			prop.Type = columnDataType;

			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
			prop.SetStatements.Add(
				new CodeConditionStatement(
					new CodeBinaryOperatorExpression(
						new CodeSnippetExpression("value"),
						CodeBinaryOperatorType.IdentityInequality,
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)),
					new CodeSnippetStatement(String.Format("{1}((EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges)this).OnPropertyChanged(\"{0}\", this.{2}, value);", propertyName, new String('\t', 5), fieldName)),
					new CodeAssignStatement(
						new CodeFieldReferenceExpression(
							new CodeThisReferenceExpression(), fieldName),
							new CodeSnippetExpression("value"))));

			classDeclaration.Members.Add(field);
			classDeclaration.Members.Add(prop);
			//classDeclaration.AddProperty(propertyName, fieldName, column.DataType, column.Nullable);
		}

		protected override PropertyDeclaration CreatePrimaryKeyProperty(ClassDeclaration classDeclaration, TableSchema table)
		{
			string propertyName = "ID";
			string fieldName = "_id";
			PropertyDeclaration result = classDeclaration.AddProperty(propertyName, fieldName, table.PrimaryKey.DataType, false);
			return result;
		}



		protected override void AddClassAttributes(ClassDeclaration classDeclaration, TableSchema table)
		{
			//ignore
		}

		protected override PropertyDeclaration CreateForeignKey(ClassDeclaration classDeclaration, ForeignKeyColumnSchema foreignKey)
		{

			string columnName = foreignKey.PrimaryKeyTable.Name;

			string fkname = this.NameProvider.GetPropertyName(columnName);
			string fkfieldname = this.NameProvider.GetFieldName(columnName);
			string fkdatatype = this.NameProvider.GetClassName(foreignKey.PrimaryKeyTable);

			//add the field
			var field = new CodeMemberField(fkdatatype, fkfieldname);
			classDeclaration.Members.Add(field);

			//add the prop
			var prop = new CodeMemberProperty();
			prop.Name = fkname;
			prop.Type = new CodeTypeReference(fkdatatype);
			prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			prop.GetStatements.Add(
				new CodeMethodReturnStatement(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fkfieldname)));


			//if (value == null || this._stateCode == null || (value.ID != this._stateCode.ID))
			var ifValueNull = new CodeBinaryOperatorExpression(new CodeSnippetExpression("value"), CodeBinaryOperatorType.IdentityEquality, new CodeSnippetExpression("null"));
			var ifFieldNull = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fkfieldname), CodeBinaryOperatorType.IdentityEquality, new CodeSnippetExpression("null"));
			var valueIdExpression = new CodePropertyReferenceExpression(new CodeSnippetExpression("value"), "ID");
			var fieldIdExpression = new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fkfieldname), "ID");
			var ifValueIdDoesNotEqualFieldId = new CodeBinaryOperatorExpression(valueIdExpression, CodeBinaryOperatorType.IdentityInequality, fieldIdExpression);


			prop.SetStatements.Add(
				new CodeConditionStatement(
					new CodeBinaryOperatorExpression(
						new CodeBinaryOperatorExpression(ifValueNull, CodeBinaryOperatorType.BooleanOr, ifFieldNull),
						CodeBinaryOperatorType.BooleanOr,
						ifValueIdDoesNotEqualFieldId),
					new CodeSnippetStatement(String.Format("{1}((EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges)this).OnPropertyChanged(\"{0}\", this.{2}, value);", fkname, new String('\t', 5), fkfieldname)),
					new CodeAssignStatement(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fkfieldname),
						new CodeSnippetExpression("value"))));
			classDeclaration.Members.Add(prop);
			return null;
			//PropertyDeclaration result = classDeclaration.AddProperty(fkname, fkfieldname, fkdatatype);
			//return result;
		}

		protected override PropertyDeclaration CreateForeignKeyReference(ClassDeclaration classDeclaration, ForeignKeyColumnSchema foreignKey)
		{
			if (foreignKey.Table.PrimaryKey == null)
				return null;


			string propertyName = this.NameProvider.GetListPropertyName(foreignKey);
			string fieldName = this.NameProvider.GetListFieldName(foreignKey);
			string typeParam = this.NameProvider.GetClassName(foreignKey.Table);
			CodeDomTypeReference genericList = new CodeDomTypeReference("System.Collections.Generic.List").AddTypeParameters(typeParam);

			//create the field
			classDeclaration.Members.Add(new CodeMemberField(genericList.ToCodeDom(), fieldName));

			//create the prop
			var prop = new CodeMemberProperty();
			prop.Name = propertyName;
			prop.Type = genericList.ToCodeDom();
			prop.Attributes = MemberAttributes.Final | MemberAttributes.Public;
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
			prop.SetStatements.Add(
				new CodeConditionStatement(
					new CodeBinaryOperatorExpression(
						new CodeSnippetExpression("value"),
						CodeBinaryOperatorType.IdentityInequality,
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)),
					new CodeSnippetStatement(String.Format("{1}((EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges)this).OnPropertyChanged(\"{0}\", this.{2}, value);", propertyName, new String('\t', 5), fieldName)),
					new CodeAssignStatement(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),
						new CodeSnippetExpression("value"))));

			classDeclaration.Members.Add(prop);
			return null;
		}


		private void AddCopyToMethod(ClassDeclaration classDecl, TableSchema table)
		{
			var method = new CodeMemberMethod();
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			method.Name = "CopyTo";

			string argName = this.NameProvider.GetArgumentName(table.Name);
			var parameter = new CodeParameterDeclarationExpression(this.NameProvider.GetClassName(table), argName);
			method.Parameters.Add(parameter);
			foreach (ColumnSchema column in table.NonKeyColumns)
			{
				var lhs = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression(argName), this.NameProvider.GetPropertyName(column));
				var rhs = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), this.NameProvider.GetPropertyName(column));
				method.Statements.Add(new CodeAssignStatement(lhs, rhs));
			}
			foreach (ColumnSchema column in table.ForeignKeys)
			{
				string propertyName = this.NameProvider.GetPropertyName(column.Name.Replace("ID", "").Replace("_id", ""));
				var lhs = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression(argName), propertyName);
				var rhs = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), propertyName);
				method.Statements.Add(new CodeAssignStatement(lhs, rhs));
			}
			classDecl.AddMember(method);
		}

		private void AddPropertyChangedPlumbing(ClassDeclaration cs)
		{

			var iface = new InterfaceDeclaration("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges", new CodeDomTypeReference(cs.FullName));
			cs.Implements(iface);
			var pairCodeTypeReference = new CodeTypeReference("EightyProofSolutions.BlogEngine.Core.Pair", new CodeTypeReference("System.Object"), new CodeTypeReference("System.Object"));
			var changedPropertiesType = new CodeTypeReference("System.Collections.Generic.Dictionary");
			changedPropertiesType.TypeArguments.Add(typeof(string));
			changedPropertiesType.TypeArguments.Add(pairCodeTypeReference);

			var changedPropertiesField = new CodeMemberField();
			changedPropertiesField.Name = "_changedProperties";
			changedPropertiesField.Type = changedPropertiesType;
			changedPropertiesField.Attributes = MemberAttributes.Private;
			changedPropertiesField.InitExpression = new CodeObjectCreateExpression(changedPropertiesType);
			cs.Members.Add(changedPropertiesField);

			var changedPropertiesProperty = new CodeMemberProperty();
			changedPropertiesProperty.Name = "ChangedProperties";
			changedPropertiesProperty.PrivateImplementationType = new CodeTypeReference("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges");
			changedPropertiesProperty.Type = changedPropertiesType;
			changedPropertiesProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_changedProperties")));
			cs.Members.Add(changedPropertiesProperty);

			var changedMethod = new CodeMemberMethod();
			changedMethod.Name = "Changed";
			changedMethod.PrivateImplementationType = new CodeTypeReference("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges");
			changedMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "propertyName"));
			changedMethod.ReturnType = new CodeTypeReference("System.Boolean");
			changedMethod.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_changedProperties"), "ContainsKey", new CodeArgumentReferenceExpression("propertyName"))));
			cs.Members.Add(changedMethod);

			var trackingChangesField = new CodeMemberField();
			trackingChangesField.Name = "_trackingChanges";
			trackingChangesField.Type = new CodeTypeReference("System.Boolean");
			trackingChangesField.InitExpression = new CodeSnippetExpression("true");
			cs.Members.Add(trackingChangesField);
			var trackingChangesProperty = new CodeMemberProperty();
			trackingChangesProperty.Name = "TrackingChanges";
			trackingChangesProperty.PrivateImplementationType = new CodeTypeReference("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges");
			trackingChangesProperty.Type = new CodeTypeReference("System.Boolean");
			trackingChangesProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_trackingChanges")));
			trackingChangesProperty.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_trackingChanges"), new CodeSnippetExpression("value")));
			cs.Members.Add(trackingChangesProperty);

			var propChangedEvent = new CodeMemberEvent();
			propChangedEvent.Name = "PropertyChanged";
			propChangedEvent.Type = new CodeTypeReference("System.ComponentModel.PropertyChangedEventHandler");
			propChangedEvent.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			cs.Members.Add(propChangedEvent);

			var propChangedMethod = new CodeMemberMethod();
			propChangedMethod.Name = "OnPropertyChanged";
			propChangedMethod.PrivateImplementationType = new CodeTypeReference("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges");
			propChangedMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "propertyName"));
			propChangedMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "oldValue"));
			propChangedMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "newValue"));

			var iftrackchanges = new CodeConditionStatement(
				new CodeBinaryOperatorExpression(
					new CodePropertyReferenceExpression(new CodeCastExpression("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges", new CodeThisReferenceExpression()), "TrackingChanges"),
					CodeBinaryOperatorType.IdentityInequality,
					new CodeSnippetExpression("true")));

			iftrackchanges.TrueStatements.Add(new CodeMethodReturnStatement());
			propChangedMethod.Statements.Add(iftrackchanges);

			var changedPropertyIndexer = new CodeIndexerExpression(
				new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_changedProperties"));
			changedPropertyIndexer.Indices.Add(new CodeArgumentReferenceExpression("propertyName"));
			propChangedMethod.Statements.Add(new CodeAssignStatement(changedPropertyIndexer,
				new CodeObjectCreateExpression(pairCodeTypeReference,
					new CodeArgumentReferenceExpression("oldValue"),
					new CodeArgumentReferenceExpression("newValue"))));

			var ifeventnotnull = new CodeConditionStatement(
				new CodeBinaryOperatorExpression(
					new CodeEventReferenceExpression(new CodeThisReferenceExpression(), "PropertyChanged"),
					CodeBinaryOperatorType.IdentityInequality,
					new CodeSnippetExpression("null")));

			ifeventnotnull.TrueStatements.Add(
				new CodeMethodInvokeExpression(
					new CodeThisReferenceExpression(), "PropertyChanged",
						new CodeThisReferenceExpression(),
						new CodeObjectCreateExpression("System.ComponentModel.PropertyChangedEventArgs",
							new CodeArgumentReferenceExpression("propertyName"))));
			propChangedMethod.Statements.Add(ifeventnotnull);

			cs.Members.Add(propChangedMethod);
		}
		private void AddToDataMethod(ClassDeclaration classDecl, TableSchema table)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "ToData";
			string returnTypeName = this.ConfigOptions.DataNamespace + "." + this.NameProvider.GetClassName(table);
			method.ReturnType = new CodeTypeReference(returnTypeName);
			method.Attributes = MemberAttributes.Public | MemberAttributes.Static;

			string argName = this.NameProvider.GetArgumentName(table.Name);
			var parameter = new CodeParameterDeclarationExpression(this.NameProvider.GetClassName(table), argName);
			method.Parameters.Add(parameter);

			var ifargnull =
				new CodeConditionStatement(
					new CodeSnippetExpression(argName + " == null"),
					new CodeMethodReturnStatement(new CodeSnippetExpression("null")));
			method.Statements.Add(ifargnull);

			var initExpression = new CodeSnippetExpression(returnTypeName + ".Find(" + argName + ".ID) ?? new " + returnTypeName + "()");


			var methodResult = new CodeVariableDeclarationStatement(returnTypeName, "result", initExpression);
			method.Statements.Add(methodResult);

			//create a model based on what we just got from the database
			method.Statements.Add(new CodeCommentStatement("create a model based on what we just got from the database"));
			var internalModelDeclaration = new CodeVariableDeclarationStatement(classDecl.FullName, "modelFilledFromDatabase", new CodeMethodInvokeExpression(null, "FromData", new CodeVariableReferenceExpression("result")));
			method.Statements.Add(internalModelDeclaration);

			//copy the values from the model passed in to the model filled from the database
			method.Statements.Add(new CodeCommentStatement("copy the values from the model passed in to the model filled from the database"));
			var copyToCall = new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(argName), "CopyTo", new CodeVariableReferenceExpression("modelFilledFromDatabase"));
			method.Statements.Add(copyToCall);


			var modelTarget = new CodeVariableReferenceExpression("modelFilledFromDatabase");
			foreach (ColumnSchema column in table.NonKeyColumns)
			{
				string propertyName = this.NameProvider.GetPropertyName(column);
				var right = new CodePropertyReferenceExpression(modelTarget, propertyName);
				var left = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("result"), propertyName);

				var ifpropchanged = new CodeConditionStatement(
						new CodeMethodInvokeExpression(
							new CodeCastExpression("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges", modelTarget),
								"Changed",
								new CodeSnippetExpression("\"" + propertyName + "\"")),
							new CodeAssignStatement(left, right));



				method.Statements.Add(ifpropchanged);
			}

			foreach (ForeignKeyColumnSchema column in table.ForeignKeys)
			{
				//if the fk column name is CompanyID, the name of the property needs to be Company, so remove the ID
				string propertyName = this.NameProvider.GetPropertyName(column).Replace("ID", "");


				string targetTypeName = this.ConfigOptions.DataNamespace + "." + this.NameProvider.GetClassName(column.PrimaryKeyTable.Name);


				var ifpropchanged = new CodeConditionStatement(
						new CodeMethodInvokeExpression(
							new CodeCastExpression("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges", modelTarget),
								"Changed",
								new CodeSnippetExpression("\"" + propertyName + "\"")));

				var ifmodeltargetpropisnull = new CodeConditionStatement(
					new CodeBinaryOperatorExpression(
						new CodePropertyReferenceExpression(modelTarget, propertyName),
						CodeBinaryOperatorType.ValueEquality,
						new CodeSnippetExpression("null")));

				ifmodeltargetpropisnull.TrueStatements.Add(new CodeAssignStatement(
					new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("result"), propertyName),
					new CodeSnippetExpression("null")));

				ifmodeltargetpropisnull.FalseStatements.Add(
					new CodeVariableDeclarationStatement(targetTypeName, Inflector.Camelize(propertyName),
						new CodeObjectCreateExpression(targetTypeName)));

				ifmodeltargetpropisnull.FalseStatements.Add(
					new CodeAssignStatement(
						new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(Inflector.Camelize(propertyName)), "ID"),
						new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(modelTarget, propertyName), "ID")));

				ifmodeltargetpropisnull.FalseStatements.Add(
					new CodeAssignStatement(
						new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("result"), propertyName),
						new CodeVariableReferenceExpression(Inflector.Camelize(propertyName))));

				ifpropchanged.TrueStatements.Add(ifmodeltargetpropisnull);
				method.Statements.Add(ifpropchanged);
			}

			if (table.PrimaryKey != null)
			{
				string propertyName = "ID";
				var right = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression(argName), propertyName);
				var left = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("result"), propertyName);
				method.Statements.Add(new CodeAssignStatement(left, right));
			}



			//((ITrackPropertyChanges)company).ChangedProperties.Clear();
			var clearProps = new CodeMethodInvokeExpression(
				new CodePropertyReferenceExpression(
					new CodeCastExpression("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges", new CodeArgumentReferenceExpression(argName)), "ChangedProperties"), "Clear");


			method.Statements.Add(clearProps);

			method.Statements.Add(new CodeSnippetExpression("foreach (var kvp in ((EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges)modelFilledFromDatabase).ChangedProperties){"));
			method.Statements.Add(new CodeSnippetExpression("\t((EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges)" + argName + ").ChangedProperties.Add(kvp.Key, kvp.Value)"));
			method.Statements.Add(new CodeSnippetExpression("}"));




			//foreach (var kvp in ((ITrackPropertyChanges)modelFilledFromDatabase).ChangedProperties)
			//{
			//	((ITrackPropertyChanges)company).ChangedProperties.Add(kvp.Key, kvp.Value);
			//}

			var resultExpression = new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result"));
			method.Statements.Add(resultExpression);

			classDecl.Members.Add(method);
		}
		private void AddFromDataMethod(ClassDeclaration classDecl, TableSchema table)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "FromData";
			string returnTypeName = this.ConfigOptions.Namespace + "." + this.NameProvider.GetClassName(table);
			method.ReturnType = new CodeTypeReference(returnTypeName);
			method.Attributes = MemberAttributes.Public | MemberAttributes.Static;

			var parameter = new CodeParameterDeclarationExpression(this.ConfigOptions.DataNamespace + "." + this.NameProvider.GetClassName(table), this.NameProvider.GetArgumentName(table.Name));
			method.Parameters.Add(parameter);

			var ifargnull =
				new CodeConditionStatement(
					new CodeSnippetExpression(this.NameProvider.GetArgumentName(table.Name) + " == null"),
					new CodeMethodReturnStatement(new CodeSnippetExpression("null")));
			method.Statements.Add(ifargnull);

			var initExpression = new CodeObjectCreateExpression(returnTypeName);
			var methodResult = new CodeVariableDeclarationStatement(returnTypeName, "result", initExpression);
			method.Statements.Add(methodResult);

			method.Statements.Add(new CodeCommentStatement("Turn off change tracking while we hydrate the model"));

			method.Statements.Add(
				new CodeAssignStatement(
					new CodePropertyReferenceExpression(new CodeCastExpression("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges", new CodeVariableReferenceExpression("result")), "TrackingChanges"),
					new CodeSnippetExpression("false")));

			foreach (ColumnSchema column in table.NonKeyColumns)
			{
				string propertyName = this.NameProvider.GetPropertyName(column.Name.Replace("_id", ""));
				var right = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression(this.NameProvider.GetArgumentName(table.Name)), propertyName);
				var left = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("result"), propertyName);

				method.Statements.Add(new CodeAssignStatement(left, right));
			}

			foreach (ForeignKeyColumnSchema column in table.ForeignKeys)
			{

				string propertyName = this.NameProvider.GetPropertyName(column.Name.Replace("ID", "").Replace("_id", ""));
				var left = new CodePropertyReferenceExpression(
					new CodeArgumentReferenceExpression("result"), propertyName);
				string targetTypeName = this.ConfigOptions.Namespace + "." + this.NameProvider.GetClassName(column.PrimaryKeyTable.Name);
				var right = new CodeMethodInvokeExpression(
					new CodeTypeReferenceExpression(targetTypeName), "FromData",
					new CodePropertyReferenceExpression(
						new CodeArgumentReferenceExpression(this.NameProvider.GetArgumentName(table.Name)),
						this.NameProvider.GetPropertyName(column.Name.Replace("ID", "").Replace("_id", ""))));


				method.Statements.Add(new CodeAssignStatement(left, right));
			}

			if (table.PrimaryKey != null)
			{
				string propertyName = "ID";
				var right = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression(this.NameProvider.GetArgumentName(table.Name)), propertyName);
				var left = new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("result"), propertyName);
				method.Statements.Add(new CodeAssignStatement(left, right));
			}

			method.Statements.Add(new CodeCommentStatement("Turn change tracking back on"));

			method.Statements.Add(
				new CodeAssignStatement(
					new CodePropertyReferenceExpression(new CodeCastExpression("EightyProofSolutions.BlogEngine.Core.Models.ITrackPropertyChanges", new CodeVariableReferenceExpression("result")), "TrackingChanges"),
					new CodeSnippetExpression("true")));

			var resultExpression = new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result"));
			method.Statements.Add(resultExpression);

			classDecl.Members.Add(method);
		}

		protected override ClassDeclaration AddMethods(ClassDeclaration classDecl, TableSchema table)
		{
			AddToDataMethod(classDecl, table);
			AddFromDataMethod(classDecl, table);
			AddPropertyChangedPlumbing(classDecl);
			AddCopyToMethod(classDecl, table);
			return classDecl;
		}

		protected override ClassDeclaration CreateConstructors(ClassDeclaration classDecl, TableSchema table)
		{

			return classDecl;
		}

		protected override void CreateAssociationProperties(ClassDeclaration classDeclaration, TableSchema table)
		{
			return;
		}
	}

}
