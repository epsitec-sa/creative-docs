//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGenerators
{
	public class EntityCodeGenerator
	{
		public EntityCodeGenerator(ResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.resourceModuleInfo = this.resourceManagerPool.GetModuleInfo (this.resourceManager.DefaultModulePath);
			this.sourceNamespace = this.resourceModuleInfo.SourceNamespace;
		}


		public string SourceNamespace
		{
			get
			{
				return this.sourceNamespace;
			}
		}

		
		public void Emit(CodeFormatter formatter, StructuredType type)
		{
			StructuredTypeClass typeClass = type.Class;
			
			string       entityName = EntityCodeGenerator.CreateEntityIdentifier (type.Caption.Name);
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
			
			formatter.WriteBeginNamespace (EntityCodeGenerator.CreateEntityNamespace (this.SourceNamespace));

			switch (typeClass)
			{
				case StructuredTypeClass.Entity:
					formatter.WriteBeginClass (EntityCodeGenerator.ClassAttributes, entityName, specifiers);
					formatter.WriteEndClass ();
					break;

				case StructuredTypeClass.Interface:
					formatter.WriteBeginInterface (EntityCodeGenerator.InterfaceAttributes, entityName, specifiers);
					formatter.WriteEndInterface ();
					break;
			}
			
			formatter.WriteEndNamespace ();
		}

		private static string CreateEntityIdentifier(string name)
		{
			return string.Concat (name, Keywords.EntitySuffix);
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



		private static readonly CodeAttributes ClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute);
		private static readonly CodeAttributes InterfaceAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default);

		private static class Keywords
		{
			public const string Global = "global";
			public const string Entities = "Entities";
			public const string EntitySuffix = "Entity";
			public const string AbstractEntity = "Epsitec.Common.Support.AbstractEntity";
		}

		private ResourceManager resourceManager;
		private ResourceManagerPool resourceManagerPool;
		private ResourceModuleInfo resourceModuleInfo;
		private string sourceNamespace;
	}
}
