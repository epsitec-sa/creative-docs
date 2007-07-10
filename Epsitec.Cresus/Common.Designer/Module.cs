using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Description d'un module de ressources ouvert par l'application Designer.
	/// </summary>
	public class Module
	{
		public Module(DesignerApplication designerApplication, DesignerMode mode, string resourcePrefix, ResourceModuleId moduleId)
		{
			this.UniqueIDCreate();

			this.designerApplication = designerApplication;
			this.mode = mode;
			this.moduleInfo = moduleId;

			this.resourceManager = new ResourceManager(moduleId, this.designerApplication.ResourceManagerPool);
			this.resourceManager.DefineDefaultModuleName(this.moduleInfo.Name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			this.modifier = new Modifier(this);

			this.accessStrings   = new ResourceAccess(ResourceAccess.Type.Strings,   this, this.moduleInfo, this.designerApplication);
			this.accessStrings2  = new ResourceAccess(ResourceAccess.Type.Strings2,  this, this.moduleInfo, this.designerApplication);
			this.accessCaptions  = new ResourceAccess(ResourceAccess.Type.Captions,  this, this.moduleInfo, this.designerApplication);
			this.accessCaptions2 = new ResourceAccess(ResourceAccess.Type.Captions2, this, this.moduleInfo, this.designerApplication);
			this.accessCommands2 = new ResourceAccess(ResourceAccess.Type.Commands2, this, this.moduleInfo, this.designerApplication);
			this.accessPanels    = new ResourceAccess(ResourceAccess.Type.Panels,    this, this.moduleInfo, this.designerApplication);
			this.accessScripts   = new ResourceAccess(ResourceAccess.Type.Scripts,   this, this.moduleInfo, this.designerApplication);
			this.accessEntities  = new ResourceAccess(ResourceAccess.Type.Entities,  this, this.moduleInfo, this.designerApplication);
			this.accessTypes2    = new ResourceAccess(ResourceAccess.Type.Types2,    this, this.moduleInfo, this.designerApplication);
			this.Load();

			//	Attention: il faut avoir fait le this.accessEntities.Load() avant de créer this.accessFields2 !
			this.accessFields2   = new ResourceAccess(ResourceAccess.Type.Fields2,   this, this.moduleInfo, this.designerApplication);
			this.accessFields2.Load();

			foreach (ResourceAccess access in Access)
			{
				access.DirtyChanged += new EventHandler(this.HandleAccessDirtyChanged);
			}
		}

		public void Dispose()
		{
			foreach (ResourceAccess access in Access)
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


		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		public ResourceModuleId ModuleInfo
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

		public ResourceAccess AccessStrings2
		{
			get
			{
				return this.accessStrings2;
			}
		}

		public ResourceAccess AccessCaptions
		{
			get
			{
				return this.accessCaptions;
			}
		}

		public ResourceAccess AccessCaptions2
		{
			get
			{
				return this.accessCaptions2;
			}
		}

		public ResourceAccess AccessCommands2
		{
			get
			{
				return this.accessCommands2;
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

		public ResourceAccess AccessFields2
		{
			get
			{
				return this.accessFields2;
			}
		}

		public ResourceAccess AccessTypes2
		{
			get
			{
				return this.accessTypes2;
			}
		}

		public ResourceAccess GetAccess(ResourceAccess.Type type)
		{
			//	Cherche un accès d'après son type.
			switch (type)
			{
				case ResourceAccess.Type.Strings:
					return this.accessStrings;

				case ResourceAccess.Type.Strings2:
					return this.accessStrings2;

				case ResourceAccess.Type.Captions:
				case ResourceAccess.Type.Fields:
				case ResourceAccess.Type.Commands:
				case ResourceAccess.Type.Types:
				case ResourceAccess.Type.Values:
					return this.accessCaptions;

				case ResourceAccess.Type.Captions2:
					return this.accessCaptions2;

				case ResourceAccess.Type.Commands2:
					return this.accessCommands2;

				case ResourceAccess.Type.Panels:
					return this.accessPanels;

				case ResourceAccess.Type.Scripts:
					return this.accessScripts;

				case ResourceAccess.Type.Entities:
					return this.accessEntities;

				case ResourceAccess.Type.Fields2:
					return this.accessFields2;

				case ResourceAccess.Type.Types2:
					return this.accessTypes2;
			}

			return null;
		}


		public void Load()
		{
			//	Charge toutes les ressources.
			foreach (ResourceAccess access in Access)
			{
				if (access != null)
				{
					access.Load();
				}
			}
		}

		public void Save()
		{
			//	Enregistre toutes les ressources.
			foreach (ResourceAccess access in Access)
			{
				access.Save();
			}
		}

		public string CheckMessage()
		{
			//	Retourne l'éventuel rapport.
			List<ResourceAccess.ShortcutItem> list = new List<ResourceAccess.ShortcutItem>();

			foreach (ResourceAccess access in Access)
			{
				access.AddShortcuts(list);
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			ResourceAccess.CheckShortcuts(builder, list);

			//	On pourrait vérifier ici d'autres choses que les raccourcis...

			return builder.ToString();
		}

		public void Check()
		{
			//	Vérifie toutes les ressources et affiche un rapport.
			string message = CheckMessage();

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
			//	Montre une ressource 'Panel' dans une fenêtre.
			ResourceBundle bundle = this.accessPanels.GetField(index, null, ResourceAccess.FieldType.Panel).Bundle;
			string name = this.accessPanels.GetField(index, null, ResourceAccess.FieldType.Name).String;
			UI.Panel panel = UI.Panel.GetPanel(bundle);

			if (panel != null)
			{
				UserInterface.RunPanel(panel, this.resourceManager, this.designerApplication.Window, name);
			}
		}


		public bool IsGlobalDirty
		{
			get
			{
				foreach (ResourceAccess access in Access)
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
				foreach (ResourceAccess access in Access)
				{
					if (access.IsLocalDirty)
					{
						return true;
					}
				}

				return false;
			}
		}


		private void HandleAccessDirtyChanged(object sender)
		{
			//	Appelé lorsque l'état IsDirty d'un accès a changé.
			this.designerApplication.GetCommandState("Save").Enable = this.IsGlobalDirty;
			this.designerApplication.GetCommandState("EditCancel").Enable = this.IsLocalDirty;
			this.designerApplication.UpdateBookModules();
		}


		protected IEnumerable<ResourceAccess> Access
		{
			//	Enumère tous les accès.
			get
			{
				yield return accessStrings;
				yield return accessStrings2;
				yield return accessCaptions;
				yield return accessCaptions2;
				yield return accessCommands2;
				yield return accessPanels;
				yield return accessScripts;
				yield return accessEntities;
				yield return accessFields2;
				yield return accessTypes2;
			}
		}



		#region UniqueID
		protected void UniqueIDCreate()
		{
			//	Assigne un numéro unique à ce module.
			this.uniqueID = Module.uniqueIDGenerator++;
		}

		public string UniqueName
		{
			//	Retourne un nom unique pour ce module.
			get
			{
				return string.Concat("Module-", this.uniqueID.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		protected static int				uniqueIDGenerator = 0;
		protected int						uniqueID;
		#endregion


		protected DesignerApplication		designerApplication;
		protected DesignerMode				mode;
		protected ResourceModuleId			moduleInfo;
		protected Modifier					modifier;
		protected ResourceManager			resourceManager;
		protected ResourceAccess			accessStrings;
		protected ResourceAccess			accessStrings2;
		protected ResourceAccess			accessCaptions;
		protected ResourceAccess			accessCaptions2;
		protected ResourceAccess			accessCommands2;
		protected ResourceAccess			accessPanels;
		protected ResourceAccess			accessScripts;
		protected ResourceAccess			accessEntities;
		protected ResourceAccess			accessFields2;
		protected ResourceAccess			accessTypes2;
	}
}
