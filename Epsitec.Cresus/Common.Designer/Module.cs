//	Copyright � 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Identity;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Description d'un module de ressources ouvert par l'application Designer.
	/// </summary>
	public class Module
	{
		public Module(DesignerApplication designerApplication, DesignerMode mode, string resourcePrefix, ResourceModuleId moduleId)
		{
			this.designerApplication = designerApplication;
			this.mode = mode;
			this.moduleId = moduleId;
			this.batchSaver = new ResourceBundleBatchSaver ();

			ResourceManagerPool pool = this.designerApplication == null ? new ResourceManagerPool () : this.designerApplication.ResourceManagerPool;

			this.resourceManager = new ResourceManager(pool, moduleId);
			this.resourceManager.DefineDefaultModuleName(this.moduleId.Name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			this.moduleInfo = this.resourceManager.DefaultModuleInfo.Clone ();

			this.modifier = new Modifier(this);

			this.accessStrings  = new ResourceAccess(ResourceAccess.Type.Strings,  this, this.moduleId);
			this.accessCaptions = new ResourceAccess(ResourceAccess.Type.Captions, this, this.moduleId);
			this.accessCommands = new ResourceAccess(ResourceAccess.Type.Commands, this, this.moduleId);
			this.accessPanels   = new ResourceAccess(ResourceAccess.Type.Panels,   this, this.moduleId);
			this.accessEntities = new ResourceAccess(ResourceAccess.Type.Entities, this, this.moduleId);
			this.accessTypes    = new ResourceAccess(ResourceAccess.Type.Types,    this, this.moduleId);
			this.Load();

			//	Attention: il faut avoir fait le this.accessEntities.Load() avant de cr�er this.accessFields !
			this.accessFields   = new ResourceAccess(ResourceAccess.Type.Fields,   this, this.moduleId);
			this.accessFields.Load();

			//	Attention: il faut avoir fait le this.accessTypes.Load() avant de cr�er this.accessValues !
			this.accessValues   = new ResourceAccess(ResourceAccess.Type.Values,   this, this.moduleId);
			this.accessValues.Load();

			foreach (ResourceAccess access in this.Accesses)
			{
				access.DirtyChanged += new EventHandler(this.HandleAccessDirtyChanged);
			}
		}

		public void Dispose()
		{
			foreach (ResourceAccess access in this.Accesses)
			{
				access.DirtyChanged -= new EventHandler(this.HandleAccessDirtyChanged);
			}

			this.modifier.Dispose();
		}


		public DesignerApplication DesignerApplication
		{
			get
			{
				return this.designerApplication;
			}
		}

		public DesignerMode Mode
		{
			//	Retourne le mode de fonctionnement du logiciel.
			get
			{
				return this.mode;
			}
		}

		public Modifier Modifier
		{
			get
			{
				return this.modifier;
			}
		}

		public ResourceBundleBatchSaver BatchSaver
		{
			get
			{
				return this.batchSaver;
			}
		}


		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		public ResourceModuleId ModuleId
		{
			get
			{
				return this.moduleId;
			}
		}

		public ResourceModuleInfo ModuleInfo
		{
			get
			{
				return this.moduleInfo;
			}
		}


		public ResourceAccess AccessStrings
		{
			get
			{
				return this.accessStrings;
			}
		}

		public ResourceAccess AccessCaptions
		{
			get
			{
				return this.accessCaptions;
			}
		}

		public ResourceAccess AccessCommands
		{
			get
			{
				return this.accessCommands;
			}
		}

		public ResourceAccess AccessPanels
		{
			get
			{
				return this.accessPanels;
			}
		}

		public ResourceAccess AccessEntities
		{
			get
			{
				return this.accessEntities;
			}
		}

		public ResourceAccess AccessFields
		{
			get
			{
				return this.accessFields;
			}
		}

		public ResourceAccess AccessValues
		{
			get
			{
				return this.accessValues;
			}
		}

		public ResourceAccess AccessTypes
		{
			get
			{
				return this.accessTypes;
			}
		}

		public ResourceAccess GetAccess(ResourceAccess.Type type)
		{
			//	Cherche un acc�s d'apr�s son type.
			switch (type)
			{
				case ResourceAccess.Type.Strings:
					return this.accessStrings;

				case ResourceAccess.Type.Captions:
					return this.accessCaptions;

				case ResourceAccess.Type.Commands:
					return this.accessCommands;

				case ResourceAccess.Type.Panels:
					return this.accessPanels;

				case ResourceAccess.Type.Entities:
					return this.accessEntities;

				case ResourceAccess.Type.Fields:
					return this.accessFields;

				case ResourceAccess.Type.Values:
					return this.accessValues;

				case ResourceAccess.Type.Types:
					return this.accessTypes;
			}

			return null;
		}


		public bool IsGlobalDirty
		{
			get
			{
				foreach (ResourceAccess access in this.Accesses)
				{
					if (access.IsGlobalDirty)
					{
						return true;
					}
				}

				return false;
			}
		}

		public bool IsLocalDirty
		{
			get
			{
				foreach (ResourceAccess access in this.Accesses)
				{
					if (access.IsLocalDirty)
					{
						return true;
					}
				}

				return false;
			}
		}


		public void Load()
		{
			//	Charge toutes les ressources.
			foreach (ResourceAccess access in this.Accesses)
			{
				if (access != null)
				{
					access.Load();
				}
			}
		}

		public void Save()
		{
			//	Enregistre toutes les ressources et met � jour le fichier module.info.
			this.SaveResources ();
			this.UpdateManifest ();
		}

		public string CheckMessage()
		{
			//	Retourne l'�ventuel rapport.
			List<ResourceAccess.ShortcutItem> list = new List<ResourceAccess.ShortcutItem>();

			foreach (ResourceAccess access in this.Accesses)
			{
				access.AddShortcuts(list);
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			ResourceAccess.CheckShortcuts(builder, list);

			//	On pourrait v�rifier ici d'autres choses que les raccourcis...

			return builder.ToString();
		}

		public void Check()
		{
			//	V�rifie toutes les ressources et affiche un rapport.
			string message = this.CheckMessage();

			if (string.IsNullOrEmpty(message))  // aucune anomalie ?
			{
				this.designerApplication.DialogMessage(Res.Strings.Error.CheckOK);
			}
			else
			{
				this.designerApplication.DialogError(message);
			}
		}

		public void RunPanel(int index)
		{
			//	Montre une ressource 'Panel' dans une fen�tre.
			string name = this.accessPanels.GetField(index, null, ResourceAccess.FieldType.Name).String;
			UI.Panel panel = this.accessPanels.GetPanel(index);

			if (panel != null)
			{
				UserInterface.RunPanel(panel, this.resourceManager, this.designerApplication.Window, name);
			}
		}


		public static void Merge(ResourceModuleId moduleId, string outputPath)
		{
			//	Fusionne les ressources du module de patch sp�cifi� avec celles
			//	du module de r�f�rence et enregistre le r�sultat dans le dossier
			//	de sortie.
			string prefix = "file";
			
			Module             patchModule = new Module (null, DesignerMode.Build, prefix, moduleId);
			ResourceModuleInfo patchInfo   = patchModule.ModuleInfo;
			ResourceManager    refManager  = patchModule.ResourceManager.GetManagerForReferenceModule ();
			ResourceModuleInfo refInfo     = refManager.DefaultModuleInfo;

			System.Diagnostics.Debug.Assert (refInfo.FullId.Id == patchInfo.FullId.Id);
			
			ResourceModuleId    mergedModule  = new ResourceModuleId (moduleId.Name, outputPath, moduleId.Id, moduleId.Layer);
			ResourceManagerPool mergedPool    = new ResourceManagerPool ();
			ResourceManager     mergedManager = new ResourceManager (mergedPool, mergedModule);
			ResourceModuleInfo  mergedInfo    = new ResourceModuleInfo ();

			mergedInfo.FullId          = mergedModule;
			mergedInfo.SourceNamespace = refInfo.SourceNamespace;

			//	Reprend les versions du module de r�f�rence et ajoute celles
			//	du module de patch. Le vecteur des versions r�sultant inclut
			//	toutes les versions uniques les plus r�centes.
			foreach (ResourceModuleVersion version in refInfo.Versions)
			{
				mergedInfo.UpdateVersion (version);
			}
			foreach (ResourceModuleVersion version in patchInfo.Versions)
			{
				mergedInfo.UpdateVersion (version);
			}

			//	Cr�e le fichier "module.info" pour le module � g�n�rer.
			System.DateTime mergeTime = System.DateTime.Now.ToUniversalTime ();
			IdentityCard authorIdentityCard = Settings.ActiveDeveloperIdentityCard;
			string author  = authorIdentityCard == null ? "" : string.Concat (" by ", authorIdentityCard.UserName);
			string comment = string.Concat ("Merged on ", mergeTime.ToShortDateString (), " ", mergeTime.ToShortTimeString (), " UTC", author);
			ResourceModule.SaveManifest (mergedInfo, comment);

			//	Comme on va utiliser le m�canisme d'enregistrement standard
			//	des accesseurs, on doit "surcharger" la m�thode SetBundle de
			//	du gestionnaire de ressources standard pour utiliser le bon
			//	gestionnaire � la place, avec des bundles adapt�s...

			patchModule.ResourceManager.ReplaceSetBundle
				(
					delegate (ResourceBundle bundle, ResourceSetMode mode)
					{
						if (mode == ResourceSetMode.Write)
						{
							//	Clone manuellement le bundle en l'ajustant pour
							//	qu'il s'int�gre dans le nouveau module :
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


		private void Regenerate()
		{
			foreach (ResourceAccess access in this.Accesses)
			{
				Support.ResourceAccessors.AbstractResourceAccessor accessor = access.Accessor as Support.ResourceAccessors.AbstractResourceAccessor;

				if (accessor != null)
				{
					access.RegenerateAllFieldsInBundle ();
					accessor.ForceModuleMerge = true;
					accessor.PersistChanges ();
					accessor.ForceModuleMerge = false;
				}
			}
		}

		private void SaveResources()
		{
			//	Enregistre toutes les ressources.
			foreach (ResourceAccess access in this.Accesses)
			{
				access.Save (this.batchSaver);
			}

			//	Passe en revue les bundles tels que les accesseurs les ont
			//	pr�par�s pour les optimiser :
			foreach (ResourceBundle bundle in this.batchSaver.GetLiveBundles ())
			{
				this.OptimizeBundles (bundle);
			}

			this.batchSaver.Execute ();
		}

		private void UpdateManifest()
		{
			int devId = Settings.ActiveDeveloperId;
			ResourceModuleVersion version = null;

			foreach (ResourceModuleVersion item in this.moduleInfo.Versions)
			{
				if (item.DeveloperId == devId)
				{
					version = item;
					break;
				}
			}

			if (version == null)
			{
				version = new ResourceModuleVersion (devId, 1, System.DateTime.Now.ToUniversalTime ());
			}
			else
			{
				version = new ResourceModuleVersion (devId, version.BuildNumber, System.DateTime.Now.ToUniversalTime ());
			}

			this.moduleInfo.UpdateVersion (version);

			ResourceModule.SaveManifest (this.moduleInfo);
		}

		private void OptimizeBundles(ResourceBundle bundle)
		{
			//	Optimise un bundle; cela va supprimer un certain nombre
			//	d'informations inutiles (par ex. des ressources secondaires
			//	vides).

			if (bundle.BasedOnPatchModule)
			{
				//	N'optimise pas les ressources d'un module de patch, car
				//	l'optimisation ne peut se faire qu'apr�s fusion.

				return;
			}

			System.Diagnostics.Debug.Assert (bundle.ResourceLevel != ResourceLevel.Merged);
			System.Diagnostics.Debug.Assert (bundle.ResourceLevel != ResourceLevel.None);

			if ((bundle.Name == Resources.CaptionsBundleName) ||
				(bundle.Name == Resources.StringsBundleName))
			{
				for (int i=0; i<bundle.FieldCount; i++)
				{
					ResourceBundle.Field field = bundle[i];

					if (field.About == "" || ResourceBundle.Field.IsNullString (field.About))
					{
						//	Si un champ contient un commentaire vide et qu'il
						//	s'agit d'une ressource d'un module de r�f�rence,
						//	alors on peut supprimer compl�tement son contenu.

						field.SetAbout (null);
					}

					if (bundle.ResourceLevel != ResourceLevel.Default)
					{
						System.Diagnostics.Debug.Assert (field.Name == null);

						//	Si une ressource est vide dans un bundle autre que le bundle
						//	par d�faut, il faut la supprimer.
						if ((ResourceBundle.Field.IsNullString (field.AsString)) &&
							(ResourceBundle.Field.IsNullString (field.About)))
						{
							bundle.Remove (i);
							i--;
						}
					}
				}
			}
		}

		private void HandleAccessDirtyChanged(object sender)
		{
			//	Appel� lorsque l'�tat IsDirty d'un acc�s a chang�.
			if (this.designerApplication != null)
			{
				this.designerApplication.GetCommandState ("Save").Enable = this.IsGlobalDirty;
				this.designerApplication.GetCommandState ("EditOk").Enable = this.IsLocalDirty;
				this.designerApplication.GetCommandState ("EditCancel").Enable = this.IsLocalDirty;
				this.designerApplication.UpdateBookModules ();
			}
		}


		protected IEnumerable<ResourceAccess> Accesses
		{
			//	Enum�re tous les acc�s.
			get
			{
				yield return accessStrings;
				yield return accessCaptions;
				yield return accessCommands;
				yield return accessEntities;
				yield return accessFields;
				yield return accessValues;
				yield return accessTypes;
				yield return accessPanels;
			}
		}


		protected ResourceBundleBatchSaver	batchSaver;
		protected DesignerApplication		designerApplication;
		protected DesignerMode				mode;
		protected ResourceModuleId			moduleId;
		protected ResourceModuleInfo		moduleInfo;
		protected Modifier					modifier;
		protected ResourceManager			resourceManager;
		protected ResourceAccess			accessStrings;
		protected ResourceAccess			accessCaptions;
		protected ResourceAccess			accessCommands;
		protected ResourceAccess			accessPanels;
		protected ResourceAccess			accessEntities;
		protected ResourceAccess			accessFields;
		protected ResourceAccess			accessValues;
		protected ResourceAccess			accessTypes;
	}
}
