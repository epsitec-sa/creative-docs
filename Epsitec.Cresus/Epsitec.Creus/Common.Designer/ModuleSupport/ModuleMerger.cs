//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.ModuleSupport
{
	/// <summary>
	/// The <c>ModuleMerger</c> class is used to produce live modules based on
	/// patch modules and reference modules.
	/// </summary>
	public static class ModuleMerger
	{
		/// <summary>
		/// Merges a module using the specified resource manager pool and
		/// copies the result to the specified output path.
		/// </summary>
		/// <param name="pool">The resource manager pool.</param>
		/// <param name="modulePath">The module path.</param>
		/// <param name="outputPath">The output path.</param>
		/// <returns>
		/// <c>true</c> if the operation was successful, <c>false</c> otherwise.
		/// </returns>
		public static bool Merge(ResourceManagerPool pool, string modulePath, string outputPath)
		{
			ResourceModuleInfo info = pool.GetModuleInfo (modulePath);

			if (info == null)
			{
				System.Diagnostics.Debug.WriteLine ("Failed to locate module " + modulePath);
				return false;
			}

			return ModuleMerger.Merge (pool, info, outputPath);
		}

		public static bool Merge(ResourceManagerPool pool, ResourceModuleInfo module, string outputPath)
		{
			if (!System.IO.Directory.Exists (outputPath))
			{
				System.IO.Directory.CreateDirectory (outputPath);
			}

			if (module.IsPatchModule)
			{
				ModuleMerger.MergeModule (pool, module.FullId, outputPath);
			}
			else
			{
				ModuleMerger.CopyModule (pool, module.FullId, outputPath);
			}

			return true;
		}


		/// <summary>
		/// Copies the module to the specified output path.
		/// </summary>
		/// <param name="moduleId">The id of the source module.</param>
		/// <param name="outputPath">The output path.</param>
		private static void CopyModule(ResourceManagerPool pool, ResourceModuleId moduleId, string outputPath)
		{
			string prefix = pool.DefaultPrefix;
			
			Module              srcModule  = new Module (pool, moduleId);
			ResourceModuleInfo  srcInfo    = srcModule.ModuleInfo;
			ResourceManager     srcManager = srcModule.ResourceManager;

			ResourceModuleId    dstModule  = new ResourceModuleId (moduleId.Name, outputPath, moduleId.Id, moduleId.Layer);
			ResourceManagerPool dstPool    = new ResourceManagerPool ();
			ResourceManager     dstManager = new ResourceManager (dstPool, dstModule);
			ResourceModuleInfo  dstInfo    = new ResourceModuleInfo ();

			dstInfo.FullId                  = dstModule;
			dstInfo.SourceNamespaceDefault  = srcInfo.SourceNamespaceDefault;
			dstInfo.SourceNamespaceEntities = srcInfo.SourceNamespaceEntities;
			dstInfo.SourceNamespaceForms    = srcInfo.SourceNamespaceForms;
			dstInfo.SourceNamespaceRes      = srcInfo.SourceNamespaceRes;
			dstInfo.PatchDepth              = srcInfo.PatchDepth;
			dstInfo.UpdateVersions (srcInfo.Versions);
			
			ResourceModule.SaveManifest (dstInfo, ModuleMerger.CreateModuleComment ("Copied"));

			//	Override the default SetBundle implementation with a special
			//	version which is responsible for saving using the destination
			//	resource manager. This allows us to re-use the standard save
			//	operation implemented in the Designer.Module class.

			srcManager.ReplaceSetBundle (
				delegate (ResourceBundle bundle, ResourceSetMode mode)
				{
					System.Diagnostics.Debug.Assert (mode == ResourceSetMode.Write);

					ResourceBundle copy = ResourceBundle.Create (dstManager, prefix, dstModule, bundle.Name, bundle.ResourceLevel, bundle.Culture, 0);
					
					copy.DefineCaption (bundle.Caption);
					copy.DefineRank (bundle.Rank);
					copy.Compile (bundle.CreateXmlAsData ());

					dstManager.SetBundle (copy, ResourceSetMode.Write);

					//	Don't execute default implementation of SetBundle:
					
					return ResourceManager.SetBundleOperation.Skip;
				});

			srcModule.SetMergeMode (true, dstManager);
			srcModule.Regenerate ();
			srcModule.SaveResources ();
			srcModule.SetMergeMode (false, null);
		}

		/// <summary>
		/// Merges the patch module with its reference module and copies the
		/// result to the output path.
		/// </summary>
		/// <param name="moduleId">The id of the patch module.</param>
		/// <param name="outputPath">The output path.</param>
		private static void MergeModule(ResourceManagerPool pool, ResourceModuleId moduleId, string outputPath)
		{
			string prefix = pool.DefaultPrefix;

			Module             patchModule = new Module (pool, moduleId);
			ResourceModuleInfo patchInfo   = patchModule.ModuleInfo;
			ResourceManager    refManager  = patchModule.ResourceManager.GetManagerForReferenceModule ();
			ResourceModuleInfo refInfo     = refManager == null ? null : refManager.DefaultModuleInfo;

			System.Diagnostics.Debug.Assert (refInfo != null);
			System.Diagnostics.Debug.Assert (refInfo.FullId.Id == patchInfo.FullId.Id);

			ResourceModuleId    mergedModule  = new ResourceModuleId (moduleId.Name, outputPath, moduleId.Id, moduleId.Layer);
			ResourceManagerPool mergedPool    = new ResourceManagerPool ();
			ResourceManager     mergedManager = new ResourceManager (mergedPool, mergedModule);
			ResourceModuleInfo  mergedInfo    = new ResourceModuleInfo ();

			mergedInfo.FullId                  = mergedModule;
			mergedInfo.SourceNamespaceDefault  = refInfo.SourceNamespaceDefault;
			mergedInfo.SourceNamespaceEntities = refInfo.SourceNamespaceEntities;
			mergedInfo.SourceNamespaceForms    = refInfo.SourceNamespaceForms;
			mergedInfo.SourceNamespaceRes      = refInfo.SourceNamespaceRes;
			mergedInfo.PatchDepth              = refInfo.PatchDepth + 1;

			mergedInfo.UpdateVersions (refInfo.Versions);
			mergedInfo.UpdateVersions (patchInfo.Versions);
			
			ResourceModule.SaveManifest (mergedInfo, ModuleMerger.CreateModuleComment ("Merged"));

			//	See description in ModuleMerger.CopyModule
			
			patchModule.ResourceManager.ReplaceSetBundle (
				delegate (ResourceBundle bundle, ResourceSetMode mode)
				{
					System.Diagnostics.Debug.Assert (mode == ResourceSetMode.Write);

					ResourceBundle copy = ResourceBundle.Create (mergedManager, prefix, mergedModule, bundle.Name, bundle.ResourceLevel, bundle.Culture, 0);
					
					copy.DefineCaption (bundle.Caption);
					copy.DefineRank (bundle.Rank);
					copy.Compile (bundle.CreateXmlAsData ());

					mergedManager.SetBundle (copy, ResourceSetMode.Write);

					return ResourceManager.SetBundleOperation.Skip;
				});

			patchModule.SetMergeMode (true, mergedManager);
			patchModule.Regenerate ();
			patchModule.SaveResources ();
			patchModule.SetMergeMode (false, null);
		}

		/// <summary>
		/// Creates the header comment for a module.
		/// </summary>
		/// <param name="action">The action (usually <c>"Copied"</c> or <c>"Merged"</c>).</param>
		/// <returns>The comment with the current date and developer name.</returns>
		private static string CreateModuleComment(string action)
		{
			System.DateTime copyTime = System.DateTime.UtcNow;
			IdentityCard identity = Settings.Default.IdentityCard;

			string author  = identity == null ? "" : string.Concat (" by ", identity.UserName);
			string comment = string.Concat (action, " on ", copyTime.ToShortDateString (), " ", copyTime.ToShortTimeString (), " UTC", author);

			return comment;
		}
	}
}
