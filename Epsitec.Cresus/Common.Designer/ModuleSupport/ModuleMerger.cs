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
			if (string.IsNullOrEmpty (info.ReferenceModulePath))
			{
				System.Diagnostics.Debug.WriteLine ("Not a patch module : "+ modulePath);
				return false;
			}

			System.IO.Directory.CreateDirectory (outputPath);

			try
			{
				ModuleMerger.Merge (info.FullId, outputPath);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Merge failed : " + ex.Message);
				return false;
			}

			return true;
		}
		
		public static void Merge(ResourceModuleId moduleId, string outputPath)
		{
			//	Fusionne les ressources du module de patch spécifié avec celles
			//	du module de référence et enregistre le résultat dans le dossier
			//	de sortie.
			string prefix = "file";

			Module patchModule = new Module (null, DesignerMode.Build, prefix, moduleId);
			ResourceModuleInfo patchInfo   = patchModule.ModuleInfo;
			ResourceManager refManager  = patchModule.ResourceManager.GetManagerForReferenceModule ();
			ResourceModuleInfo refInfo     = refManager.DefaultModuleInfo;

			System.Diagnostics.Debug.Assert (refInfo.FullId.Id == patchInfo.FullId.Id);

			ResourceModuleId mergedModule  = new ResourceModuleId (moduleId.Name, outputPath, moduleId.Id, moduleId.Layer);
			ResourceManagerPool mergedPool    = new ResourceManagerPool ();
			ResourceManager mergedManager = new ResourceManager (mergedPool, mergedModule);
			ResourceModuleInfo mergedInfo    = new ResourceModuleInfo ();

			mergedInfo.FullId          = mergedModule;
			mergedInfo.SourceNamespace = refInfo.SourceNamespace;

			//	Reprend les versions du module de référence et ajoute celles
			//	du module de patch. Le vecteur des versions résultant inclut
			//	toutes les versions uniques les plus récentes.
			foreach (ResourceModuleVersion version in refInfo.Versions)
			{
				mergedInfo.UpdateVersion (version);
			}
			foreach (ResourceModuleVersion version in patchInfo.Versions)
			{
				mergedInfo.UpdateVersion (version);
			}

			//	Crée le fichier "module.info" pour le module à générer.
			System.DateTime mergeTime = System.DateTime.Now.ToUniversalTime ();
			IdentityCard authorIdentityCard = Settings.ActiveDeveloperIdentityCard;
			string author  = authorIdentityCard == null ? "" : string.Concat (" by ", authorIdentityCard.UserName);
			string comment = string.Concat ("Merged on ", mergeTime.ToShortDateString (), " ", mergeTime.ToShortTimeString (), " UTC", author);
			ResourceModule.SaveManifest (mergedInfo, comment);

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
