//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGenerators;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	public class CodeGenerator
	{
		public CodeGenerator(CodeFormatter formatter, ResourceManager resourceManager)
		{
			this.formatter = formatter;
			this.resourceManager = resourceManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.resourceModuleInfo = this.resourceManagerPool.GetModuleInfo (this.resourceManager.DefaultModulePath);
			this.sourceNamespace = this.resourceModuleInfo.SourceNamespace;
		}


		public CodeFormatter Formatter
		{
			get
			{
				return this.formatter;
			}
		}
		
		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		public ResourceManagerPool ResourceManagerPool
		{
			get
			{
				return this.resourceManagerPool;
			}
		}

		public ResourceModuleInfo ResourceModuleInfo
		{
			get
			{
				return this.resourceModuleInfo;
			}
		}
		
		public string SourceNamespace
		{
			get
			{
				return this.sourceNamespace;
			}
		}




		
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
			
			this.formatter.WriteBeginNamespace (CodeGenerator.CreateEntityNamespace (this.SourceNamespace));

			switch (typeClass)
			{
				case StructuredTypeClass.Entity:
					emitter = new EntityEmitter (this, type);
					
					this.formatter.WriteBeginClass (CodeGenerator.EntityClassAttributes, CodeGenerator.CreateEntityIdentifier (name), specifiers);
					type.ForEachField (emitter.EmitLocalProperty);
					this.formatter.WriteCodeLine ();
					type.ForEachField (emitter.EmitLocalPropertyHandlers);
					this.formatter.WriteCodeLine ();
					this.EmitMethods (type);
					this.formatter.WriteEndClass ();
					break;

				case StructuredTypeClass.Interface:
					emitter = new InterfaceEmitter (this, type);
					
					this.formatter.WriteBeginInterface (CodeGenerator.InterfaceAttributes, CodeGenerator.CreateInterfaceIdentifier (name), specifiers);
					type.ForEachField (emitter.EmitLocalProperty);
					this.formatter.WriteEndInterface ();

					this.formatter.WriteBeginClass (CodeGenerator.StaticClassAttributes, CodeGenerator.CreateInterfaceImplementationIdentifier (name), specifiers);
					this.formatter.WriteEndClass ();
					break;
			}

			this.formatter.WriteEndNamespace ();
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

		private string CreateEntityFullName(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);
			StructuredType type = TypeRosetta.GetTypeObject (caption) as StructuredType;

			IList<ResourceModuleInfo> infos = this.resourceManagerPool.FindModuleInfos (id.Module);
			
			System.Diagnostics.Debug.Assert (infos.Count > 0);
			System.Diagnostics.Debug.Assert (caption != null);
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (infos[0].SourceNamespace));
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

			return string.Concat (Keywords.Global, "::", CodeGenerator.CreateEntityNamespace (infos[0].SourceNamespace), ".", identifier);
		}

		private string CreatePropertyName(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);
			return CodeGenerator.CreatePropertyIdentifier (caption.Name);
		}

		private string CreateTypeFullName(Druid typeId)
		{
			Caption caption = this.resourceManager.GetCaption (typeId);
			AbstractType type = TypeRosetta.GetTypeObject (caption);
			System.Type sysType = type.SystemType;

			if ((sysType == typeof (StructuredType)) ||
				(sysType == null))
			{
				return this.CreateEntityFullName (typeId);
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


		private abstract class Emitter
		{
			public Emitter(CodeGenerator generator, StructuredType type)
			{
				this.generator = generator;
				this.type = type;
			}

			public void EmitLocalProperty(StructuredTypeField field)
			{
				if (field.Membership == FieldMembership.Local)
				{
					string typeName = this.generator.CreateTypeFullName (field.TypeId);
					string propName = this.generator.CreatePropertyName (field.CaptionId);
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

					switch (field.Source)
					{
						case FieldSource.Value:
							this.EmitPropertyGetter (field, typeName, propName);
							this.EmitPropertySetter (field, typeName, propName);
							break;
						
						case FieldSource.Expression:

							//	TODO: properly handle expressions

							this.EmitPropertyGetter (field, typeName, propName);
							break;

						default:
							throw new System.ArgumentException (string.Format ("FieldSource.{0} not valid in this context", field.Source));
					}

					this.generator.formatter.WriteEndProperty ();
				}
			}

			public void EmitLocalPropertyHandlers(StructuredTypeField field)
			{
				if (field.Membership == FieldMembership.Local)
				{
					string typeName = this.generator.CreateTypeFullName (field.TypeId);
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

			protected CodeGenerator generator;
			protected StructuredType type;
		}

		private sealed class EntityEmitter : Emitter
		{
			public EntityEmitter(CodeGenerator generator, StructuredType type)
				: base (generator, type)
			{
			}

			protected override void EmitPropertyGetter(StructuredTypeField field, string typeName, string propName)
			{
				this.generator.formatter.WriteBeginGetter (new CodeAttributes (CodeVisibility.Public));

				if (field.Relation == FieldRelation.Collection)
				{
					this.generator.formatter.WriteCodeLine (Keywords.Return, " ", Keywords.This, ".", Keywords.GetFieldCollectionMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote,");");
				}
				else
				{
					this.generator.formatter.WriteCodeLine (Keywords.Return, " ", Keywords.This, ".", Keywords.GetFieldMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ");");
				}
				
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
					this.generator.formatter.WriteCodeLine (typeName, " ", Keywords.OldValueVariable, " = ", Keywords.This, ".", propName, ";");
					this.generator.formatter.WriteCodeLine (Keywords.If, " (", Keywords.OldValueVariable, " != ", Keywords.ValueVariable, ")");
					this.generator.formatter.WriteBeginBlock ();
					this.generator.formatter.WriteCodeLine (Keywords.This, ".", Keywords.OnPrefix, propName, Keywords.ChangingSuffix, " (", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
					this.generator.formatter.WriteCodeLine (Keywords.This, ".", Keywords.SetFieldMethod, "<", typeName, "> (", Keywords.Quote, field.Id, Keywords.Quote, ", ", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
					this.generator.formatter.WriteCodeLine (Keywords.This, ".", Keywords.OnPrefix, propName, Keywords.ChangedSuffix, " (", Keywords.OldValueVariable, ", ", Keywords.ValueVariable, ");");
					this.generator.formatter.WriteEndBlock ();
					this.generator.formatter.WriteEndSetter ();
				}
			}

			protected override void EmitPropertyOnChanged(StructuredTypeField field, string typeName, string propName)
			{
				string code = string.Concat (Keywords.Void, " ", Keywords.OnPrefix, propName, Keywords.ChangedSuffix, "(",
					/**/					 typeName, " ", Keywords.OldValueVariable, ", ",
					/**/				     typeName, " ", Keywords.NewValueVariable, ")");
				this.generator.formatter.WriteBeginMethod (CodeGenerator.PartialMethodAttributes, code);
				this.generator.formatter.WriteEndMethod ();
			}

			protected override void EmitPropertyOnChanging(StructuredTypeField field, string typeName, string propName)
			{
				string code = string.Concat (Keywords.Void, " ", Keywords.OnPrefix, propName, Keywords.ChangingSuffix, "(",
					/**/					 typeName, " ", Keywords.OldValueVariable, ", ",
					/**/				     typeName, " ", Keywords.NewValueVariable, ")");
				this.generator.formatter.WriteBeginMethod (CodeGenerator.PartialMethodAttributes, code);
				this.generator.formatter.WriteEndMethod ();
			}
		}

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


		private static readonly CodeAttributes EntityClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute);
		private static readonly CodeAttributes StaticClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		private static readonly CodeAttributes InterfaceAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default);
		private static readonly CodeAttributes PropertyAttributes = new CodeAttributes (CodeVisibility.Public);
		private static readonly CodeAttributes PartialMethodAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Default, CodeAttributes.PartialDefinitionAttribute);

		private static class Keywords
		{
			public const string Global = "global";
			public const string Return = "return";
			public const string New    = "new";
			public const string This   = "this";
			public const string If     = "if";
			public const string Quote  = "\"";
			public const string Override = "override";

			public const string Void   = "void";
			public const string String = "string";
			public const string Druid  = "global::Epsitec.Common.Support.Druid";

			public const string SimpleComment = "//";

			public const string ValueVariable = "value";
			public const string OldValueVariable = "oldValue";
			public const string NewValueVariable = "newValue";
			public const string FieldNameVariable = "fieldName";

			public const string GenericIList = "System.Collections.Generic.IList";
			public const string Entities = "Entities";
			public const string EntitySuffix = "Entity";
			public const string InterfaceImplementationSuffix = "InterfaceImplementation";
			public const string AbstractEntity = "Epsitec.Common.Support.EntityEngine.AbstractEntity";

			public const string SetFieldMethod = "SetField";
			public const string GetFieldMethod = "GetField";
			public const string GetFieldCollectionMethod = "GetFieldCollection";
			public const string GetStructuredTypeIdMethod = "GetStructuredTypeId";
			public const string OnPrefix = "On";
			public const string ChangedSuffix = "Changed";
			public const string ChangingSuffix = "Changing";
		}

		private CodeFormatter formatter;
		private ResourceManager resourceManager;
		private ResourceManagerPool resourceManagerPool;
		private ResourceModuleInfo resourceModuleInfo;
		private string sourceNamespace;
	}
}
