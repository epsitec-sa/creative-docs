//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGenerators
{
	public class EntityCodeGenerator
	{
		public EntityCodeGenerator(CodeFormatter formatter, ResourceManager resourceManager)
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

			//	Define the base type from which the entity class will inherit from;
			//	it is either the defined base type entity or the root AbstractEntity
			//	class :

			if (typeClass == StructuredTypeClass.Entity)
			{
				if (type.BaseTypeId.IsValid)
				{
					specifiers.Add (this.CreateEntityFullName (type.BaseTypeId));
				}
				else
				{
					specifiers.Add (string.Concat (Keywords.Global, "::", Keywords.AbstractEntity));
				}
			}

			//	Include all locally imported interfaces, if any :

			foreach (Druid interfaceId in type.InterfaceIds)
			{
				specifiers.Add (this.CreateEntityFullName (interfaceId));
			}

			Emitter emitter;
			
			this.formatter.WriteBeginNamespace (EntityCodeGenerator.CreateEntityNamespace (this.SourceNamespace));

			switch (typeClass)
			{
				case StructuredTypeClass.Entity:
					emitter = new EntityEmitter (this, type);
					
					this.formatter.WriteBeginClass (EntityCodeGenerator.EntityClassAttributes, EntityCodeGenerator.CreateEntityIdentifier (name), specifiers);
					type.ForEachField (emitter.EmitLocalProperty);
					this.formatter.WriteEndClass ();
					break;

				case StructuredTypeClass.Interface:
					emitter = new InterfaceEmitter (this, type);
					
					this.formatter.WriteBeginInterface (EntityCodeGenerator.InterfaceAttributes, EntityCodeGenerator.CreateInterfaceIdentifier (name), specifiers);
					type.ForEachField (emitter.EmitLocalProperty);
					this.formatter.WriteEndInterface ();

					this.formatter.WriteBeginClass (EntityCodeGenerator.StaticClassAttributes, EntityCodeGenerator.CreateInterfaceImplementationIdentifier (name), specifiers);
					this.formatter.WriteEndClass ();
					break;
			}

			this.formatter.WriteEndNamespace ();
		}

		
		private static string CreateEntityIdentifier(string name)
		{
			return string.Concat (name, Keywords.EntitySuffix);
		}

		private static string CreateInterfaceIdentifier(string name)
		{
			return name;
		}

		private static string CreateInterfaceImplementationIdentifier(string name)
		{
			return string.Concat (name.Substring (1), Keywords.InterfaceImplementationSuffix);
		}

		private static string CreateEntityNamespace(string name)
		{
			return string.Concat (name, ".", Keywords.Entities);
		}

		private string CreateEntityFullName(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);
			IList<ResourceModuleInfo> infos = this.resourceManagerPool.FindModuleInfos (id.Module);
			
			System.Diagnostics.Debug.Assert (infos.Count > 0);
			System.Diagnostics.Debug.Assert (caption != null);
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (infos[0].SourceNamespace));

			return string.Concat (Keywords.Global, "::", EntityCodeGenerator.CreateEntityNamespace (infos[0].SourceNamespace), ".", EntityCodeGenerator.CreateEntityIdentifier (caption.Name));
		}

		private string CreatePropertyName(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);
			return EntityCodeGenerator.CreateEntityIdentifier (caption.Name);
		}

		private string CreateTypeFullName(Druid typeId)
		{
			Caption caption = this.resourceManager.GetCaption (typeId);
			AbstractType type = TypeRosetta.GetTypeObject (caption);
			System.Type sysType = type.SystemType;

			if (sysType == typeof (StructuredType))
			{
				return this.CreateEntityFullName (typeId);
			}
			else
			{
				return EntityCodeGenerator.GetTypeName (sysType);
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
			public Emitter(EntityCodeGenerator generator, StructuredType type)
			{
				this.generator = generator;
				this.type = type;
			}

			public abstract void EmitLocalProperty(StructuredTypeField field);

			protected EntityCodeGenerator generator;
			protected StructuredType type;
		}

		private sealed class EntityEmitter : Emitter
		{
			public EntityEmitter(EntityCodeGenerator generator, StructuredType type)
				: base (generator, type)
			{
			}

			public override void EmitLocalProperty(StructuredTypeField field)
			{
				if (field.Membership == FieldMembership.Local)
				{
					string code = string.Concat (this.generator.CreateTypeFullName (field.TypeId), " ", this.generator.CreatePropertyName (field.CaptionId));
					this.generator.formatter.WriteBeginProperty (EntityCodeGenerator.PropertyAttributes, code);
					this.generator.formatter.WriteEndProperty ();
				}
			}
		}

		private sealed class InterfaceEmitter : Emitter
		{
			public InterfaceEmitter(EntityCodeGenerator generator, StructuredType type)
				: base (generator, type)
			{
			}
			
			public override void EmitLocalProperty(StructuredTypeField field)
			{
				if (field.Membership == FieldMembership.Local)
				{
				}
			}
		}


		private static readonly CodeAttributes EntityClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute);
		private static readonly CodeAttributes StaticClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		private static readonly CodeAttributes InterfaceAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default);
		private static readonly CodeAttributes PropertyAttributes = new CodeAttributes (CodeVisibility.Public);

		private static class Keywords
		{
			public const string Global = "global";
			public const string Entities = "Entities";
			public const string EntitySuffix = "Entity";
			public const string InterfaceImplementationSuffix = "InterfaceImplementation";
			public const string AbstractEntity = "Epsitec.Common.Support.AbstractEntity";
		}

		private CodeFormatter formatter;
		private ResourceManager resourceManager;
		private ResourceManagerPool resourceManagerPool;
		private ResourceModuleInfo resourceModuleInfo;
		private string sourceNamespace;
	}
}
