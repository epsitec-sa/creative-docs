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
		public ModuleStore(ResourceManagerPool pool)
		{
			this.pool = pool;
		}

		/// <summary>
		/// Creates an empty reference module based on the specified parameters.
		/// </summary>
		/// <param name="rootDirectoryPath">The path of the root directory.</param>
		/// <param name="moduleName">The module name.</param>
		/// <param name="sourceNamespace">The source namespace.</param>
		/// <param name="moduleLayer">The module layer.</param>
		/// <param name="identity">The developer identity.</param>
		/// <returns>The <see cref="ResourceModuleInfo"/> or <c>null</c>.</returns>
		public ResourceModuleInfo CreateReferenceModule(string rootDirectoryPath, string moduleName, string sourceNamespace, ResourceModuleLayer moduleLayer, IdentityCard identity)
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

			this.ConnectToService ();
			
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

				modulePath = this.pool.GetRootAbsolutePath (modulePath);

				if ((System.IO.Directory.Exists (modulePath)) &&
					(System.IO.Directory.GetFiles (modulePath, "*.info").Length > 0))
				{
					this.service.RecycleModuleId (moduleId, identity.UserName);
					return null;
				}

				//	Create the files as specified by the server; usually, this will
				//	just create the module directory with the module.info file, but
				//	this could change...
				
				foreach (ModuleFile file in dir.Files)
				{
					string path = System.IO.Path.Combine (rootDirectoryPath, file.Path);
					
					path = this.pool.GetRootAbsolutePath (path);

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

		/// <summary>
		/// Creates the patch module for the specified reference module.
		/// </summary>
		/// <param name="rootDirectoryPath">The root directory path.</param>
		/// <param name="referenceModule">The reference module.</param>
		/// <param name="identity">The identity.</param>
		/// <returns></returns>
		public ResourceModuleInfo CreatePatchModule(string rootDirectoryPath, ResourceModuleInfo referenceModule, IdentityCard identity)
		{
			if ((identity == null) ||
				(referenceModule == null) ||
				(string.IsNullOrEmpty (identity.UserName)) ||
				(string.IsNullOrEmpty (rootDirectoryPath)) ||
				(string.IsNullOrEmpty (referenceModule.FullId.Path)) ||
				(referenceModule.IsPatchModule))
			{
				return null;
			}

			string              moduleName  = referenceModule.FullId.Name;
			string              modulePath  = System.IO.Path.Combine (rootDirectoryPath, moduleName);
			int                 moduleId    = referenceModule.FullId.Id;
			ResourceModuleLayer moduleLayer = referenceModule.FullId.Layer;
			ResourceModuleInfo  patchModule = new ResourceModuleInfo ()
			{
				FullId                  = new ResourceModuleId (moduleName, this.pool.GetRootAbsolutePath (modulePath), moduleId, moduleLayer),
				PatchDepth              = referenceModule.PatchDepth+1,
				ReferenceModulePath     = this.pool.GetRootRelativePath (referenceModule.FullId.Path),
				SourceNamespaceDefault  = referenceModule.SourceNamespaceDefault,
				SourceNamespaceEntities = referenceModule.SourceNamespaceEntities,
				SourceNamespaceForms    = referenceModule.SourceNamespaceForms,
				SourceNamespaceRes      = referenceModule.SourceNamespaceRes
			};

			if (System.IO.Directory.Exists (modulePath))
			{
				return null;
			}

			patchModule.Freeze ();
			
			System.DateTime now = System.DateTime.UtcNow;
			
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");
			string comment   = string.Concat ("Created by ", identity.UserName, " on ", timeStamp);
			
			ResourceModule.SaveManifest (patchModule, comment);

			return patchModule;
		}

		/// <summary>
		/// Recycles the specified module. Its ID will be free for reuse. Do not
		/// call this method for a patch module, as it shares its ID with the
		/// reference module!
		/// </summary>
		/// <param name="moduleInfo">The module info.</param>
		/// <param name="identity">The developer identity.</param>
		/// <returns><c>true</c> if the module was successfully recycled; otherwise,
		/// <c>false</c>.</returns>
		public bool RecycleModule(ResourceModuleInfo moduleInfo, IdentityCard identity)
		{
			if ((moduleInfo == null) ||
				(moduleInfo.IsPatchModule) ||
				(moduleInfo.PatchDepth > 0))
			{
				return false;
			}
			else
			{
				return this.RecycleModule (moduleInfo.FullId.Id, identity);
			}
		}

		/// <summary>
		/// Recycles the specified module. Its ID will be free for reuse. Do not
		/// call this method for a patch module, as it shares its ID with the
		/// reference module!
		/// </summary>
		/// <param name="moduleId">The module id.</param>
		/// <param name="identity">The identity.</param>
		/// <returns><c>true</c> if the module was successfully recycled; otherwise,
		/// <c>false</c>.</returns>
		public bool RecycleModule(int moduleId, IdentityCard identity)
		{
			if ((identity == null) ||
				(string.IsNullOrEmpty (identity.UserName)) ||
				(moduleId < 1000))
			{
				return false;
			}
			else
			{
				this.ConnectToService ();

				return this.service.RecycleModuleId (moduleId, identity.UserName);
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



		/// <summary>
		/// Connects to the service, if the connection is not yet or no longer
		/// valid.
		/// </summary>
		private void ConnectToService()
		{
			if (this.service == null)
			{
				this.service = ModuleRepositoryClient.GetService ();
			}
			else
			{
				ModuleRepositoryClient.ReopenService (ref this.service);
			}
		}


		private readonly ResourceManagerPool pool;
		private IModuleRepositoryService service;
	}
}
