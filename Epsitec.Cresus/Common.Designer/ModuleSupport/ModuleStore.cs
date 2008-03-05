//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.ModuleRepository;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.ModuleSupport
{
	/// <summary>
	/// The <c>ModuleStore</c> class provides the mechanisms needed to create
	/// and recycle modules.
	/// </summary>
	sealed class ModuleStore : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleStore"/> class.
		/// </summary>
		public ModuleStore()
		{
		}

		/// <summary>
		/// Creates an empty module based on the specified parameters.
		/// </summary>
		/// <param name="rootDirectoryPath">The path of the root directory.</param>
		/// <param name="moduleName">The module name.</param>
		/// <param name="sourceNamespace">The source namespace.</param>
		/// <param name="moduleLayer">The module layer.</param>
		/// <param name="identity">The developer identity.</param>
		/// <returns>The <see cref="ResourceModuleInfo"/> or <c>null</c>.</returns>
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

			//	First, contact the server to allocate a new module ID. This might
			//	either fail gracefully (by returning an invalid module ID) or it
			//	could throw an exception.
			
			int moduleId = this.service.GetNewModuleId (moduleName, identity.UserName);

			if (moduleId < 1000)
			{
				return null;
			}

			try
			{
				string moduleLayerPrefix = ResourceModuleId.ConvertLayerToPrefix (moduleLayer);
				
				//	Contact the server and ask it to create an empty module using
				//	the freshly allocated module ID. In the case of a failure, we
				//	make sure that we recycle the module ID so that it does not
				//	get lost forever.
				
				ModuleDirectory dir = this.service.CreateEmptyModule (moduleId, moduleLayerPrefix, sourceNamespace);

				if (dir == null)
				{
					this.service.RecycleModuleId (moduleId, identity.UserName);
					return null;
				}

				string modulePath = System.IO.Path.Combine (rootDirectoryPath, dir.Name);

				//	Create the files as specified by the server; usually, this will
				//	just create the module directory with the module.info file, but
				//	this could change...
				
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


		private void ConnectToService()
		{
			if (this.service == null)
			{
				this.service = ModuleRepositoryClient.GetService ();
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.service != null)
			{
				ModuleRepositoryClient.CloseService (this.service);
				this.service = null;
			}
		}

		#endregion


		private IModuleRepositoryService service;
	}
}
