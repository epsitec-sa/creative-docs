//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.ModuleRepository;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.ModuleSupport
{
	class ModuleStore
	{
		public ModuleStore()
		{
			this.service = ModuleRepositoryClient.GetService ();
		}

		public ResourceModuleInfo CreateModule(string rootDirectoryPath, string moduleName, string sourceNamespace, ResourceModuleLayer moduleLayer, IdentityCard identity)
		{
			if ((identity == null) ||
				(string.IsNullOrEmpty (identity.UserName)) ||
				(string.IsNullOrEmpty (rootDirectoryPath)) ||
				(string.IsNullOrEmpty (moduleName)) ||
				(moduleLayer == ResourceModuleLayer.Undefined))
			{
				return null;
			}

			int moduleId = this.service.GetNewModuleId (moduleName, identity.UserName);

			if (moduleId < 1000)
			{
				return null;
			}

			try
			{
				ModuleDirectory dir = this.service.CreateEmptyModule (moduleId, ResourceModuleId.ConvertLayerToPrefix (moduleLayer), sourceNamespace);

				if (dir == null)
				{
					return null;
				}

				string modulePath = System.IO.Path.Combine (rootDirectoryPath, dir.Name);

				foreach (ModuleFile file in dir.Files)
				{
					string path = System.IO.Path.Combine (rootDirectoryPath, file.Path);

					System.IO.Directory.CreateDirectory (System.IO.Path.GetDirectoryName (path));
					System.IO.File.WriteAllBytes (path, file.Data);
				}

				System.IO.Directory.CreateDirectory (System.IO.Path.Combine (modulePath, "SourceCode"));

				return ResourceModule.LoadManifest (modulePath);
			}
			catch
			{
				this.service.RecycleModuleId (moduleId, identity.UserName);
				throw;
			}
		}

		public bool RecycleModule(ResourceModuleInfo moduleInfo, IdentityCard identity)
		{
			if ((identity == null) ||
				(string.IsNullOrEmpty (identity.UserName)) ||
				(moduleInfo == null) ||
				(moduleInfo.IsPatchModule) ||
				(moduleInfo.FullId.Id < 1000))
			{
				return false;
			}
			else
			{
				return this.service.RecycleModuleId (moduleInfo.FullId.Id, identity.UserName);
			}
		}

		private readonly IModuleRepositoryService service;
	}
}
