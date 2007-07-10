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
		}


		public string RootNamespace
		{
			get
			{
				//	TODO: the root namespace should be stored in the module information

				return "Epsitec";
			}
		}

		private string ModuleNamespace
		{
			get
			{
				return this.resourceModuleInfo.FullId.Name;
			}
		}

		
		public void EmitEntity(System.Text.StringBuilder buffer, StructuredType type)
		{
			
		}




		private ResourceManager resourceManager;
		private ResourceManagerPool resourceManagerPool;
		private ResourceModuleInfo resourceModuleInfo;
	}
}
