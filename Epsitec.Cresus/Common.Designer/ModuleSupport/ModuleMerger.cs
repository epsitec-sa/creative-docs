//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.ModuleSupport
{
	public static class ModuleMerger
	{
		public static bool Merge(ResourceManagerPool pool, string modulePath, string outputPath)
		{
			ResourceModuleInfo info = pool.GetModuleInfo (modulePath);

			if (info == null)
			{
				System.Diagnostics.Debug.WriteLine ("Failed to locate module " + modulePath);
				return false;
			}

			System.IO.Directory.CreateDirectory (outputPath);

			try
			{
				if (string.IsNullOrEmpty (info.ReferenceModulePath))
				{
					ModuleMerger.CopyModule (info.FullId, outputPath);
				}
				else
				{
					ModuleMerger.Merge (info.FullId, outputPath);
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Merge failed : " + ex.Message);
				return false;
			}

			return true;
		}

		
		private static void CopyModule(ResourceModuleId moduleId, string outputPath)
		{
			string prefix = "file";
			
			Module              srcModule  = new Module (null, DesignerMode.Build, prefix, moduleId);
			ResourceModuleInfo  srcInfo    = srcModule.ModuleInfo;
			ResourceManager     srcManager = srcModule.ResourceManager;

			ResourceModuleId    dstModule  = new ResourceModuleId (moduleId.Name, outputPath, moduleId.Id, moduleId.Layer);
			ResourceManagerPool dstPool    = new ResourceManagerPool ();
			ResourceManager     dstManager = new ResourceManager (dstPool, dstModule);
			ResourceModuleInfo  dstInfo    = new ResourceModuleInfo ();

			dstInfo.FullId          = dstModule;
			dstInfo.SourceNamespace = srcInfo.SourceNamespace;
			dstInfo.UpdateVersions (srcInfo.Versions);
			
			ResourceModule.SaveManifest (dstInfo, ModuleMerger.CreateModuleComment ("Copied"));

			//	Comme on va utiliser le mécanisme d'enregistrement standard
			//	des accesseurs, on doit "surcharger" la méthode SetBundle de
			//	du gestionnaire de ressources standard pour utiliser le bon
			//	gestionnaire à la place, avec des bundles adaptés...

			srcManager.ReplaceSetBundle
				(
					delegate (ResourceBundle bundle, ResourceSetMode mode)
					{
						if (mode == ResourceSetMode.Write)
						{
							//	Clone manuellement le bundle en l'ajustant pour
							//	qu'il s'intègre dans le nouveau module :
							ResourceBundle copy = ResourceBundle.Create (dstManager, prefix, dstModule, bundle.Name, bundle.ResourceLevel, bundle.Culture, 0);
							copy.DefineCaption (bundle.Caption);
							copy.DefineRank (bundle.Rank);
							copy.Compile (bundle.CreateXmlAsData ());

							dstManager.SetBundle (copy, ResourceSetMode.Write);
						}

						return false;
					}
				);

			srcModule.Regenerate ();
			srcModule.SaveResources ();
		}

		private static string CreateModuleComment(string action)
		{
			System.DateTime copyTime = System.DateTime.Now.ToUniversalTime ();
			IdentityCard    identity = Settings.Default.IdentityCard;
			
			string author  = identity == null ? "" : string.Concat (" by ", identity.UserName);
			string comment = string.Concat (action, " on ", copyTime.ToShortDateString (), " ", copyTime.ToShortTimeString (), " UTC", author);
			
			return comment;
		}

		private static void Merge(ResourceModuleId moduleId, string outputPath)
		{
			//	Fusionne les ressources du module de patch spécifié avec celles
			//	du module de référence et enregistre le résultat dans le dossier
			//	de sortie.
			string prefix = "file";

			Module             patchModule = new Module (null, DesignerMode.Build, prefix, moduleId);
			ResourceModuleInfo patchInfo   = patchModule.ModuleInfo;
			ResourceManager    refManager  = patchModule.ResourceManager.GetManagerForReferenceModule ();
			ResourceModuleInfo refInfo     = refManager == null ? null : refManager.DefaultModuleInfo;

			System.Diagnostics.Debug.Assert (refInfo != null);
			System.Diagnostics.Debug.Assert (refInfo.FullId.Id == patchInfo.FullId.Id);

			ResourceModuleId    mergedModule  = new ResourceModuleId (moduleId.Name, outputPath, moduleId.Id, moduleId.Layer);
			ResourceManagerPool mergedPool    = new ResourceManagerPool ();
			ResourceManager     mergedManager = new ResourceManager (mergedPool, mergedModule);
			ResourceModuleInfo  mergedInfo    = new ResourceModuleInfo ();

			mergedInfo.FullId          = mergedModule;
			mergedInfo.SourceNamespace = refInfo.SourceNamespace;

			mergedInfo.UpdateVersions (refInfo.Versions);
			mergedInfo.UpdateVersions (patchInfo.Versions);
			
			//	Crée le fichier "module.info" pour le module à générer.
			ResourceModule.SaveManifest (mergedInfo, ModuleMerger.CreateModuleComment ("Merged"));

			//	Comme on va utiliser le mécanisme d'enregistrement standard
			//	des accesseurs, on doit "surcharger" la méthode SetBundle de
			//	du gestionnaire de ressources standard pour utiliser le bon
			//	gestionnaire à la place, avec des bundles adaptés...

			patchModule.ResourceManager.ReplaceSetBundle
				(
					delegate (ResourceBundle bundle, ResourceSetMode mode)
					{
						if (mode == ResourceSetMode.Write)
						{
							//	Clone manuellement le bundle en l'ajustant pour
							//	qu'il s'intègre dans le nouveau module :
							ResourceBundle copy = ResourceBundle.Create (mergedManager, prefix, mergedModule, bundle.Name, bundle.ResourceLevel, bundle.Culture, 0);
							copy.DefineCaption (bundle.Caption);
							copy.DefineRank (bundle.Rank);
							copy.Compile (bundle.CreateXmlAsData ());

							mergedManager.SetBundle (copy, ResourceSetMode.Write);
						}

						return false;
					}
				);

			patchModule.Regenerate ();
			patchModule.SaveResources ();
		}
	}
}
