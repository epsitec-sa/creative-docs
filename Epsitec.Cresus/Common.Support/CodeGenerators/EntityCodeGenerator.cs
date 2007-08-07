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

		
		public void EmitEntity(CodeFormatter formatter, StructuredType type)
		{
			string className = EntityCodeGenerator.CreateEntityIdentifier (type.Caption.Name);
			List<string> classSpecifiers = new List<string> ();

			//	Define the base type :

			if (type.BaseTypeId.IsValid)
			{
				classSpecifiers.Add (this.CreateEntityFullName (type.BaseTypeId));
			}
			else
			{
				classSpecifiers.Add (string.Concat (Keywords.Global, "::", Keywords.AbstractEntity));
			}

			foreach (Druid interfaceId in type.InterfaceIds)
			{
				classSpecifiers.Add (this.CreateEntityFullName (interfaceId));
			}
			
			formatter.WriteBeginNamespace (EntityCodeGenerator.CreateEntityNamespace (this.SourceNamespace));
			formatter.WriteBeginClass (EntityCodeGenerator.ClassAttributes, className, classSpecifiers);
			formatter.WriteEndClass ();
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
