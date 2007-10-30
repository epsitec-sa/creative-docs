//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>CodeGenerator</c> class produces a C# source code wrapper for
	/// the entities defined in the resource files.
	/// </summary>
	public class CodeGenerator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeGenerator"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		public CodeGenerator(ResourceManager resourceManager)
			: this (new CodeFormatter (), resourceManager)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeGenerator"/> class.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		/// <param name="resourceManager">The resource manager.</param>
		public CodeGenerator(CodeFormatter formatter, ResourceManager resourceManager)
		{
			this.formatter = formatter;
			this.resourceManager = resourceManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.resourceModuleInfo = this.resourceManagerPool.GetModuleInfo (this.resourceManager.DefaultModulePath);
			this.sourceNamespace = this.resourceModuleInfo.SourceNamespace;
		}


		/// <summary>
		/// Gets the formatter used when generating code.
		/// </summary>
		/// <value>The formatter.</value>
		public CodeFormatter Formatter
		{
			get
			{
				return this.formatter;
			}
		}

		/// <summary>
		/// Gets the resource manager.
		/// </summary>
		/// <value>The resource manager.</value>
		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		/// <summary>
		/// Gets the resource manager pool.
		/// </summary>
		/// <value>The resource manager pool.</value>
		public ResourceManagerPool ResourceManagerPool
		{
			get
			{
				return this.resourceManagerPool;
			}
		}

		/// <summary>
		/// Gets the resource module information.
		/// </summary>
		/// <value>The resource module information.</value>
		public ResourceModuleInfo ResourceModuleInfo
		{
			get
			{
				return this.resourceModuleInfo;
			}
		}

		/// <summary>
		/// Gets the source namespace.
		/// </summary>
		/// <value>The source namespace.</value>
		public string SourceNamespace
		{
			get
			{
				return this.sourceNamespace;
			}
		}


		/// <summary>
		/// Emits the code for all entities and interfaces defined in the
		/// current module.
		/// </summary>
		public void Emit()
		{
			ResourceAccessors.StructuredTypeResourceAccessor accessor = new ResourceAccessors.StructuredTypeResourceAccessor ();
			accessor.Load (this.resourceManager);

			foreach (CultureMap item in accessor.Collection)
			{
				Caption     caption = this.resourceManager.GetCaption (item.Id);
				StructuredType type = TypeRosetta.GetTypeObject (caption) as StructuredType;

				if ((type != null) &&
					((type.Class == StructuredTypeClass.Entity) || (type.Class == StructuredTypeClass.Interface)))
				{
					this.Emit (type);
				}
			}

			this.Formatter.Flush ();
		}

		/// <summary>
		/// Emits the source code for the entity described by the specified type.
		/// </summary>
		/// <param name="type">The structured type.</param>
		public void Emit(StructuredType type)
		{
			StructuredTypeClass typeClass = type.Class;
			
			string       name       = type.Caption.Name;
			List<string> specifiers = new List<string> ();

			System.Diagnostics.Debug.Assert ((typeClass == StructuredTypeClass.Entity) || (typeClass == StructuredTypeClass.Interface));

			//	Define the base type from which the entity class will inherit; it is
			//	either the defined base type entity or the root AbstractEntity class
			//	for entities. For interfaces, this does not apply.

			switch (typeClass)
			{
				case StructuredTypeClass.Entity:
					if (type.BaseTypeId.IsValid)
					{
						specifiers.Add (this.CreateEntityFullName (type.BaseTypeId));
					}
					else
					{
						specifiers.Add (string.Concat (Keywords.Global, "::", Keywords.AbstractEntity));
					}
					break;

				case StructuredTypeClass.Interface:
					System.Diagnostics.Debug.Assert (type.BaseTypeId.IsEmpty);
					break;

				default:
					throw new System.ArgumentException (string.Format ("StructuredTypeClass.{0} not valid in this context", typeClass));
			}

			//	Include all locally imported interfaces, if any :

			foreach (Druid interfaceId in type.InterfaceIds)
			{
				specifiers.Add (this.CreateEntityFullName (interfaceId));
			}

			Emitter emitter;
			InterfaceImplementationEmitter implementationEmitter;

			this.formatter.WriteCodeLine (Keywords.BeginRegion, " ", this.SourceNamespace, ".", name, " ", typeClass.ToString ());
			this.formatter.WriteBeginNamespace (CodeGenerator.CreateEntityNamespace (this.SourceNamespace));

			switch (typeClass)
			{
				case StructuredTypeClass.Entity:
					emitter = new EntityEmitter (this, type);
					
					this.formatter.WriteBeginClass (CodeGenerator.EntityClassAttributes, CodeGenerator.CreateEntityIdentifier (name), specifiers);
					type.ForEachField (emitter.EmitLocalProperty);
					emitter.EmitCloseRegion ();
					this.formatter.WriteCodeLine ();
					type.ForEachField (emitter.EmitLocalPropertyHandlers);
					emitter.EmitCloseRegion ();
					this.formatter.WriteCodeLine ();
					this.EmitMethods (type);
					this.formatter.WriteEndClass ();
					break;

				case StructuredTypeClass.Interface:
					emitter = new InterfaceEmitter (this, type);
					
					this.formatter.WriteBeginInterface (CodeGenerator.InterfaceAttributes, CodeGenerator.CreateInterfaceIdentifier (name), specifiers);
					type.ForEachField (emitter.EmitLocalProperty);
					emitter.EmitCloseRegion ();
					this.formatter.WriteEndInterface ();

					implementationEmitter = new InterfaceImplementationEmitter (this, type);

					this.formatter.WriteBeginClass (CodeGenerator.StaticPartialClassAttributes, CodeGenerator.CreateInterfaceImplementationIdentifier (name), specifiers);
					type.ForEachField (implementationEmitter.EmitLocalPropertyImplementation);
					this.formatter.WriteEndClass ();
					break;
			}

			this.formatter.WriteEndNamespace ();
			this.formatter.WriteCodeLine (Keywords.EndRegion);
			this.formatter.WriteCodeLine ();
		}

		
		private void EmitMethods(StructuredType type)
		{
			Druid id = type.CaptionId;

			string code = string.Concat (Keywords.Druid, " ", Keywords.GetStructuredTypeIdMethod, "()");
			this.formatter.WriteBeginMethod (new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Override), code);
			this.formatter.WriteCodeLine (Keywords.Return, " ", Keywords.New, " ", Keywords.Druid, " (",
				/**/					  id.Module.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
				/**/					  id.Developer.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
				/**/					  id.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), ");",
				/**/					  "\t", Keywords.SimpleComment, " ", id.ToString ());
			this.formatter.WriteEndMethod ();
		}


		private static string CreateEntityIdentifier(string name)
		{
			return string.Concat (name, Keywords.EntitySuffix);
		}

		private static string CreatePropertyIdentifier(string name)
		{
			return name;
		}

		private static string CreateInterfaceIdentifier(string name)
		{
			return name;
		}

		private static string CreateInterfaceImplementationIdentifier(string name)
		{
			return string.Concat (name, Keywords.InterfaceImplementationSuffix);
		}

		private static string CreateEntityNamespace(string name)
		{
			return string.Concat (name, ".", Keywords.Entities);
		}

		private string CreateEntityShortName(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);
			StructuredType type = TypeRosetta.GetTypeObject (caption) as StructuredType;

			System.Diagnostics.Debug.Assert (type != null);

			string identifier;

			switch (type.Class)
			{
				case StructuredTypeClass.Entity:
					identifier = CodeGenerator.CreateEntityIdentifier (caption.Name);
					break;

				case StructuredTypeClass.Interface:
					identifier = CodeGenerator.CreateInterfaceIdentifier (caption.Name);
					break;

				default:
					throw new System.ArgumentException (string.Format ("StructuredTypeClass.{0} not valid in this context", type.Class));
			}

			return identifier;
		}
		
		private string CreateEntityFullName(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);
			StructuredType type = TypeRosetta.GetTypeObject (caption) as StructuredType;

			IList<ResourceModuleInfo> infos = this.resourceManagerPool.FindModuleInfos (id.Module);
			
			System.Diagnostics.Debug.Assert (infos.Count > 0);
			System.Diagnostics.Debug.Assert (caption != null);
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (infos[0].SourceNamespace));
			System.Diagnostics.Debug.Assert (type != null);
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (caption.Name));

			string identifier;

			switch (type.Class)
			{
				case StructuredTypeClass.Entity:
					identifier = CodeGenerator.CreateEntityIdentifier (caption.Name);
					break;

				case StructuredTypeClass.Interface:
					identifier = CodeGenerator.CreateInterfaceIdentifier (caption.Name);
					break;

				default:
					throw new System.ArgumentException (string.Format ("StructuredTypeClass.{0} not valid in this context", type.Class));
			}

			return string.Concat (Keywords.Global, "::", CodeGenerator.CreateEntityNamespace (infos[0].SourceNamespace), ".", identifier);
		}

		private string CreatePropertyName(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);
			return CodeGenerator.CreatePropertyIdentifier (caption.Name);
		}

		private string CreateTypeFullName(Druid typeId)
		{
			return this.CreateTypeFullName (typeId, false);
		}
		
		private string CreateTypeFullName(Druid typeId, bool nullable)
		{
			Caption caption = this.resourceManager.GetCaption (typeId);
			AbstractType type = TypeRosetta.GetTypeObject (caption);
			System.Type sysType = type.SystemType;

			if ((sysType == typeof (StructuredType)) ||
				(sysType == null))
			{
				return this.CreateEntityFullName (typeId);
			}
			else if (nullable && (sysType.IsValueType) && (!TypeRosetta.IsNullable (sysType)))
			{
				return string.Concat (CodeGenerator.GetTypeName (sysType), "?");
			}
			else
			{
				return CodeGenerator.GetTypeName (sysType);
			}
		}

		private static string GetTypeName(System.Type systemType)
		{
			string systemTypeName = systemType.FullName;

			switch (systemTypeName)
			{
				case "System.Boolean":
					return "bool";
				case "System.Int16":
					return "short";
				case "System.Int32":
					return "int";
				case "System.Int64":
					return "long";
				case "System.String":
					return "string";
			}

			return string.Concat (Keywords.Global, "::", systemTypeName);
		}

		#region Emitter Class

		private abstract class Emitter
		{
			public Emitter(CodeGenerator generator, StructuredType type)
			{
				this.generator = generator;
				this.type = type;
			}

			public virtual void EmitLocalProperty(StructuredTypeField field)
			{
				if (field.Membership == FieldMembership.Local)
				{
					this.EmitInterfaceRegion (field);

					string typeName = this.generator.CreateTypeFullName (field.TypeId, TypeRosetta.IsNullable (field));
					string propName = this.generator.CreatePropertyName (field.CaptionId);
					
					this.EmitLocalBeginProperty (field, typeName, propName);

					switch (field.Source)
					{
						case FieldSource.Value:
							this.EmitPropertyGetter (field, typeName, propName);
							this.EmitPropertySetter (field, typeName, propName);
							break;
						
						case FieldSource.Expression:
							this.EmitPropertyGetter (field, typeName, propName);
							this.EmitPropertySetter (field, typeName, propName);
							break;

						default:
							throw new System.ArgumentException (string.Format ("FieldSource.{0} not valid in this context", field.Source));
					}

					this.generator.formatter.WriteEndProperty ();
				}
			}

			public void EmitLocalPropertyHandlers(StructuredTypeField field)
			{
				if ((field.Membership == FieldMembership.Local) &&
					(field.DefiningTypeId.IsEmpty))
				{
					string typeName = this.generator.CreateTypeFullName (field.TypeId, TypeRosetta.IsNullable (field));
					string propName = this.generator.CreatePropertyName (field.CaptionId);

					switch (field.Relation)
					{
						case FieldRelation.None:
						case FieldRelation.Reference:
							this.EmitPropertyOnChanging (field, typeName, propName);
							this.EmitPropertyOnChanged (field, typeName, propName);
							break;

						case FieldRelation.Collection:
							break;

						default:
							throw new System.NotSupportedException (string.Format ("Relation {0} not supported", field.Relation));
					}
				}
			}

			public void EmitCloseRegion()
			{
				this.EmitInterfaceRegion (null);
			}

			protected virtual void EmitPropertyOnChanging(StructuredTypeField field, string typeName, string propName)
			{
			}

			protected virtual void EmitPropertyOnChanged(StructuredTypeField field, string typeName, string propName)
			{
			}

			protected virtual void EmitPropertyGetter(StructuredTypeField field, string typeName, string propName)
			{
			}

			protected virtual void EmitPropertySetter(StructuredTypeField field, string typeName, string propName)
			{
			}

			private void EmitLocalBeginProperty(StructuredTypeField field, string typeName, string propName)
			{
				string code;

				switch (field.Relation)
				{
					case FieldRelation.None:
					case FieldRelation.Reference:
						code = string.Concat (typeName, " ", propName);
						break;

					case FieldRelation.Collection:
						code = string.Concat (Keywords.Global, "::", Keywords.GenericIList, "<", typeName, ">", " ", propName);
						break;

					default:
						throw new System.ArgumentException (string.Format ("FieldRelation.{0} not valid in this context", field.Relation));
				}

				this.generator.formatter.WriteBeginProperty (CodeGenerator.PropertyAttributes, code);
			}

			private void EmitInterfaceRegion(StructuredTypeField field)
			{
				Druid currentInterfaceId = field == null ? Druid.Empty : field.DefiningTypeId;

				if (this.interfaceId == currentInterfaceId)
				{
					return;
				}

				if (this.interfaceId.IsValid)
				{
					this.generator.formatter.WriteCodeLine (Keywords.EndRegion);
				}

				this.interfaceId = currentInterfaceId;

				if (this.interfaceId.IsValid)
				{
					string interfaceName = this.generator.CreateEntityShortName (this.interfaceId);
					this.generator.formatter.WriteCodeLine (Keywords.BeginRegion, " ", interfaceName, " Members");
				}
			}

			protected CodeGenerator generator;
			protected StructuredType type;

			private Druid interfaceId;
		}

		#endregion

		#region EntityEmitter Class

		private sealed class EntityEmitter : Emitter
		{
			public EntityEmitter(CodeGenerator generator, StructuredType type)
				: base (generator, type)
			{
			}

			public override void EmitLocalProperty(StructuredTypeField field)
			{
				base.EmitLocalProperty (field);

				if (field.Source == FieldSource.Expression)
				{
					string typeName = this.generator.CreateTypeFullName (field.TypeId, TypeRosetta.IsNullable (field));
					string propName = this.generator.CreatePropertyName (field.CaptionId);
					
					this.EmitExpression (field, propName, typeName);
				}
			}

			protected override void EmitPropertyGetter(StructuredTypeField field, string typeName, string propName)
			{
				this.generator.formatter.WriteBeginGetter (new CodeAttributes (CodeVisibility.Public));

				if (field.DefiningTypeId.IsEmpty)
				{
					//	The field is defined locally and is not the implementation of any
					//	interface.

					switch (field.Source)
					{
						case FieldSource.Value:
							this.EmitValueGetter (field, typeName);
							break;

						case FieldSource.Expression:
							this.EmitExpressionGetter (field, propName, typeName);
							break;
					}
				}
				else
				{
					//	The field is defined by an interface and this is an implementation
					//	of the getter.

					string interfaceName = this.generator.CreateTypeFullName (field.DefiningTypeId);
					string interfaceImplementationName = CodeGenerator.CreateInterfaceImplementationIdentifier (interfaceName);
					string getterMethodName = string.Concat (Keywords.InterfaceImplementationGetterMethodPrefix, propName);

					this.generator.formatter.WriteCodeLine (Keywords.Return, " ", interfaceImplementationName, ".", getterMethodName, " (", Keywords.This, ");");
				}
				
				this.generator.formatter.WriteEndGetter ();
			}

			private void EmitExpression(StructuredTypeField field, string propName, string typeName)
			{
				EntityExpression expression = EntityExpression.FromEncodedExpression (field.Expression);

				if (expression.Encoding != EntityExpressionEncoding.LambdaCSharpSourceCode)
				{
					throw new System.InvalidOperationException (string.Format ("Invalid expression encoding '{0}' for field {1}.{2}", expression.Encoding, this.type.Name, propName));
				}

				string className = this.generator.CreateEntityFullName (this.type.CaptionId);
				string comment = string.Concat (" ", Keywords.SimpleComment, " λ ", this.type.CaptionId.ToString (), " ", field.Id);

				this.generator.formatter.WriteField (CodeGenerator.StaticInternalReadOnlyFieldAttributes,
					Keywords.Func, "<", className, ", ", typeName, "> ", Keywords.FuncPrefix, propName, " = ",
					expression.SourceCode, ";", comment);
				
				this.generator.formatter.WriteField (CodeGenerator.StaticInternalReadOnlyFieldAttributes,
					Keywords.LinqExpression, "<", Keywords.Func, "<", className, ", ", typeName, ">> ", Keywords.ExprPrefix, propName, " = ",
					expression.SourceCode, ";", comment);
			}

			private void EmitValueGetter(StructuredTypeField field, string typeName)
			{
				if (field.Relation == FieldRelation.Collection)
				{
					this.generator.formatter.WriteCodeLine (Keywords.Return, " ", Keywords.This, ".", Keywords.GetFieldCollectionMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ");");
				}
				else
				{
					this.generator.formatter.WriteCodeLine (Keywords.Return, " ", Keywords.This, ".", Keywords.GetFieldMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ");");
				}
			}

			private void EmitExpressionGetter(StructuredTypeField field, string propName, string typeName)
			{
				string className = this.generator.CreateEntityFullName (this.type.CaptionId);

				this.generator.formatter.WriteCodeLine (
					Keywords.Return, " ",
					Keywords.Global, "::", Keywords.AbstractEntity, ".", Keywords.GetCalculationMethod, "<", className, ", ", typeName, "> (",
					Keywords.This, ", ",
					Keywords.Quote, field.Id, Keywords.Quote, ", ",
					className, ".", Keywords.FuncPrefix, propName, ");");
			}

			protected override void EmitPropertySetter(StructuredTypeField field, string typeName, string propName)
			{
				if (field.Relation == FieldRelation.Collection)
				{
				}
				else
				{
					this.generator.formatter.WriteBeginSetter (new CodeAttributes (CodeVisibility.Public));

					if (field.DefiningTypeId.IsEmpty)
					{
						//	The field is defined locally and is not the implementation of any
						//	interface.

						switch (field.Source)
						{
							case FieldSource.Value:
								this.EmitValueSetter (field, typeName, propName);
								break;

							case FieldSource.Expression:
								this.EmitExpressionSetter (field, typeName, propName);
								break;
						}
					}
					else
					{
						//	The field is defined by an interface and this is an implementation
						//	of the setter.

						string interfaceName = this.generator.CreateTypeFullName (field.DefiningTypeId);
						string interfaceImplementationName = CodeGenerator.CreateInterfaceImplementationIdentifier (interfaceName);
						string setterMethodName = string.Concat (Keywords.InterfaceImplementationSetterMethodPrefix, propName);

						this.generator.formatter.WriteCodeLine (interfaceImplementationName, ".", setterMethodName, " (", Keywords.This, ", ", Keywords.ValueVariable, ");");
					}

					this.generator.formatter.WriteEndSetter ();
				}
			}

			private void EmitValueSetter(StructuredTypeField field, string typeName, string propName)
			{
				this.generator.formatter.WriteCodeLine (typeName, " ", Keywords.OldValueVariable, " = ", Keywords.This, ".", propName, ";");
				this.generator.formatter.WriteCodeLine (Keywords.If, " (", Keywords.OldValueVariable, " != ", Keywords.ValueVariable, ")");
				this.generator.formatter.WriteBeginBlock ();
				this.generator.formatter.WriteCodeLine (Keywords.This, ".", Keywords.OnPrefix, propName, Keywords.ChangingSuffix, " (", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
				this.generator.formatter.WriteCodeLine (Keywords.This, ".", Keywords.SetFieldMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ", ", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
				this.generator.formatter.WriteCodeLine (Keywords.This, ".", Keywords.OnPrefix, propName, Keywords.ChangedSuffix, " (", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
				this.generator.formatter.WriteEndBlock ();
			}
			
			private void EmitExpressionSetter(StructuredTypeField field, string typeName, string propName)
			{
				this.generator.formatter.WriteCodeLine (Keywords.Throw, " ", Keywords.New, " ", Keywords.NotSupportedException, " ();");
			}

			protected override void EmitPropertyOnChanged(StructuredTypeField field, string typeName, string propName)
			{
				if (field.Source == FieldSource.Value)
				{
					string code = string.Concat (Keywords.Void, " ", Keywords.OnPrefix, propName, Keywords.ChangedSuffix, "(",
						/**/					 typeName, " ", Keywords.OldValueVariable, ", ",
						/**/				     typeName, " ", Keywords.NewValueVariable, ")");
					this.generator.formatter.WriteBeginMethod (CodeGenerator.PartialMethodAttributes, code);
					this.generator.formatter.WriteEndMethod ();
				}
			}

			protected override void EmitPropertyOnChanging(StructuredTypeField field, string typeName, string propName)
			{
				if (field.Source == FieldSource.Value)
				{
					string code = string.Concat (Keywords.Void, " ", Keywords.OnPrefix, propName, Keywords.ChangingSuffix, "(",
						/**/					 typeName, " ", Keywords.OldValueVariable, ", ",
						/**/				     typeName, " ", Keywords.NewValueVariable, ")");
					this.generator.formatter.WriteBeginMethod (CodeGenerator.PartialMethodAttributes, code);
					this.generator.formatter.WriteEndMethod ();
				}
			}
		}

		#endregion

		#region InterfaceEmitter Class

		private sealed class InterfaceEmitter : Emitter
		{
			public InterfaceEmitter(CodeGenerator generator, StructuredType type)
				: base (generator, type)
			{
			}
			
			protected override void EmitPropertyGetter(StructuredTypeField field, string typeName, string propName)
			{
				this.generator.formatter.WriteBeginGetter (new CodeAttributes (CodeVisibility.Public));
				this.generator.formatter.WriteEndGetter ();
			}

			protected override void EmitPropertySetter(StructuredTypeField field, string typeName, string propName)
			{
				if (field.Relation == FieldRelation.Collection)
				{
				}
				else
				{
					this.generator.formatter.WriteBeginSetter (new CodeAttributes (CodeVisibility.Public));
					this.generator.formatter.WriteEndSetter ();
				}
			}
		}

		#endregion

		#region InterfaceImplementationEmitter Class

		private sealed class InterfaceImplementationEmitter : Emitter
		{
			public InterfaceImplementationEmitter(CodeGenerator generator, StructuredType type)
				: base (generator, type)
			{
				this.className = CodeGenerator.CreateInterfaceImplementationIdentifier (type.Caption.Name);
				this.interfaceName = this.generator.CreateEntityFullName (type.CaptionId);
			}

			private void EmitExpression(StructuredTypeField field, string propName, string typeName)
			{
				EntityExpression expression = EntityExpression.FromEncodedExpression (field.Expression);

				if (expression.Encoding != EntityExpressionEncoding.LambdaCSharpSourceCode)
				{
					throw new System.InvalidOperationException (string.Format ("Invalid expression encoding '{0}' for field {1}.{2}", expression.Encoding, this.type.Name, propName));
				}

				string className = this.generator.CreateEntityFullName (this.type.CaptionId);
				string comment = string.Concat (" ", Keywords.SimpleComment, " λ ", this.type.CaptionId.ToString (), " ", field.Id);

				this.generator.formatter.WriteField (CodeGenerator.StaticInternalReadOnlyFieldAttributes,
					Keywords.Func, "<", className, ", ", typeName, "> ", Keywords.FuncPrefix, propName, " = ",
					expression.SourceCode, ";", comment);

				this.generator.formatter.WriteField (CodeGenerator.StaticInternalReadOnlyFieldAttributes,
					Keywords.LinqExpression, "<", Keywords.Func, "<", className, ", ", typeName, ">> ", Keywords.ExprPrefix, propName, " = ",
					expression.SourceCode, ";", comment);
			}

			private void EmitValueGetter(StructuredTypeField field, string typeName)
			{
				if (field.Relation == FieldRelation.Collection)
				{
					this.generator.formatter.WriteCodeLine (Keywords.Return, " ", Keywords.EntityVariable, ".", Keywords.GetFieldCollectionMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ");");
				}
				else
				{
					this.generator.formatter.WriteCodeLine (Keywords.Return, " ", Keywords.EntityVariable, ".", Keywords.GetFieldMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ");");
				}
			}

			private void EmitExpressionGetter(StructuredTypeField field, string propName, string typeName)
			{
				string className = this.generator.CreateEntityFullName (this.type.CaptionId);

				this.generator.formatter.WriteCodeLine (
					Keywords.Return, " ",
					Keywords.Global, "::", Keywords.AbstractEntity, ".", Keywords.GetCalculationMethod, "<", className, ", ", typeName, "> (",
					Keywords.EntityVariable, ", ",
					Keywords.Quote, field.Id, Keywords.Quote, ", ",
					className, ".", Keywords.FuncPrefix, propName, ");");
			}

			public void EmitLocalPropertyImplementation(StructuredTypeField field)
			{
				if (field.Membership == FieldMembership.Local)
				{
					string typeName = this.generator.CreateTypeFullName (field.TypeId, TypeRosetta.IsNullable (field));
					string propName = this.generator.CreatePropertyName (field.CaptionId);

					string getterMethodName = string.Concat (Keywords.InterfaceImplementationGetterMethodPrefix, propName);
					string getterCode = string.Concat (typeName, " ", getterMethodName, "(", this.interfaceName, " ", Keywords.ObjVariable, ")");
					string entityType = string.Concat (Keywords.Global, "::", Keywords.AbstractEntity);
					
					this.generator.formatter.WriteBeginMethod (new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static), getterCode);
					this.generator.formatter.WriteCodeLine (entityType, " ", Keywords.EntityVariable, " = ", Keywords.ObjVariable, " ", Keywords.As, " ", entityType, ";");

					switch (field.Source)
					{
						case FieldSource.Value:
							this.EmitValueGetter (field, typeName);
							break;

						case FieldSource.Expression:
							this.EmitExpressionGetter (field, propName, typeName);
							break;
					}
					
					this.generator.formatter.WriteEndMethod ();

					string setterMethodName = string.Concat (Keywords.InterfaceImplementationSetterMethodPrefix, propName);
					string setterCode = string.Concat (Keywords.Void, " ", setterMethodName, "(", this.interfaceName, " ", Keywords.ObjVariable, ", ", typeName, " ", Keywords.ValueVariable, ")");

					if (field.Relation == FieldRelation.Collection)
					{
					}
					else
					{
						this.generator.formatter.WriteBeginMethod (new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static), setterCode);
						this.generator.formatter.WriteCodeLine (entityType, " ", Keywords.EntityVariable, " = ", Keywords.ObjVariable, " ", Keywords.As, " ", entityType, ";");
						this.generator.formatter.WriteCodeLine (typeName, " ", Keywords.OldValueVariable, " = ", Keywords.ObjVariable, ".", propName, ";");
						this.generator.formatter.WriteCodeLine (Keywords.If, " (", Keywords.OldValueVariable, " != ", Keywords.ValueVariable, ")");
						this.generator.formatter.WriteBeginBlock ();
						this.generator.formatter.WriteCodeLine (this.className, ".", Keywords.OnPrefix, propName, Keywords.ChangingSuffix, " (", Keywords.ObjVariable, ", ", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
						this.generator.formatter.WriteCodeLine (Keywords.EntityVariable, ".", Keywords.SetFieldMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ", ", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
						this.generator.formatter.WriteCodeLine (this.className, ".", Keywords.OnPrefix, propName, Keywords.ChangedSuffix, " (", Keywords.ObjVariable, ", ", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
						this.generator.formatter.WriteEndBlock ();
						this.generator.formatter.WriteEndMethod ();
						
						string code;

						code = string.Concat (Keywords.Void, " ", Keywords.OnPrefix, propName, Keywords.ChangedSuffix, "(",
							/**/			  this.interfaceName, " ", Keywords.ObjVariable, ", ",
							/**/			  typeName, " ", Keywords.OldValueVariable, ", ",
							/**/			  typeName, " ", Keywords.NewValueVariable, ")");
						this.generator.formatter.WriteBeginMethod (CodeGenerator.StaticPartialMethodAttributes, code);
						this.generator.formatter.WriteEndMethod ();

						code = string.Concat (Keywords.Void, " ", Keywords.OnPrefix, propName, Keywords.ChangingSuffix, "(",
							/**/			  this.interfaceName, " ", Keywords.ObjVariable, ", ",
							/**/			  typeName, " ", Keywords.OldValueVariable, ", ",
							/**/			  typeName, " ", Keywords.NewValueVariable, ")");
						this.generator.formatter.WriteBeginMethod (CodeGenerator.StaticPartialMethodAttributes, code);
						this.generator.formatter.WriteEndMethod ();
					}

					switch (field.Source)
					{
						case FieldSource.Value:
							break;

						case FieldSource.Expression:

							//	TODO: properly handle expressions

							break;

						default:
							throw new System.ArgumentException (string.Format ("FieldSource.{0} not valid in this context", field.Source));
					}
				}
			}

			private string className;
			private string interfaceName;
		}

		#endregion

		#region CodeAttributes Constants

		private static readonly CodeAttributes EntityClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute);
		private static readonly CodeAttributes StaticClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		private static readonly CodeAttributes StaticPartialClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static, CodeAttributes.PartialAttribute);
		private static readonly CodeAttributes InterfaceAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default);
		private static readonly CodeAttributes PropertyAttributes = new CodeAttributes (CodeVisibility.Public);
		private static readonly CodeAttributes PartialMethodAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Default, CodeAttributes.PartialDefinitionAttribute);
		private static readonly CodeAttributes StaticPartialMethodAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Static, CodeAttributes.PartialDefinitionAttribute);
		private static readonly CodeAttributes StaticInternalReadOnlyFieldAttributes = new CodeAttributes (CodeVisibility.Internal, CodeAccessibility.Static, CodeAttributes.ReadOnlyAttribute);

		#endregion

		#region Keywords Class

		private static class Keywords
		{
			public const string Global = "global";
			public const string Return = "return";
			public const string New    = "new";
			public const string This   = "this";
			public const string Throw  = "throw";
			public const string If     = "if";
			public const string As     = "as";
			public const string Quote  = "\"";
			public const string Override = "override";

			public const string Void   = "void";
			public const string String = "string";
			public const string Druid  = "global::Epsitec.Common.Support.Druid";
			public const string Func   = "global::System.Func";
			public const string LinqExpression = "global::System.Linq.Expressions.Expression";
			public const string NotSupportedException = "global::System.NotSupportedException";

			public const string SimpleComment = "//";

			public const string BeginRegion = "#region";
			public const string EndRegion = "#endregion";

			public const string ValueVariable = "value";
			public const string OldValueVariable = "oldValue";
			public const string NewValueVariable = "newValue";
			public const string FieldNameVariable = "fieldName";
			public const string ObjVariable = "obj";
			public const string EntityVariable = "entity";

			public const string GenericIList = "System.Collections.Generic.IList";
			public const string Entities = "Entities";
			public const string EntitySuffix = "Entity";
			public const string ExprPrefix = "Expr";
			public const string FuncPrefix = "Func";
			public const string InterfaceImplementationSuffix = "InterfaceImplementation";
			public const string InterfaceImplementationGetterMethodPrefix = "Get";
			public const string InterfaceImplementationSetterMethodPrefix = "Set";
			public const string AbstractEntity = "Epsitec.Common.Support.EntityEngine.AbstractEntity";

			public const string SetFieldMethod = "SetField";
			public const string GetFieldMethod = "GetField";
			public const string GetCalculationMethod = "GetCalculation";
			public const string GetFieldCollectionMethod = "GetFieldCollection";
			public const string GetStructuredTypeIdMethod = "GetStructuredTypeId";
			public const string OnPrefix = "On";
			public const string ChangedSuffix = "Changed";
			public const string ChangingSuffix = "Changing";
		}

		#endregion

		private CodeFormatter formatter;
		private ResourceManager resourceManager;
		private ResourceManagerPool resourceManagerPool;
		private ResourceModuleInfo resourceModuleInfo;
		private string sourceNamespace;
	}
}
