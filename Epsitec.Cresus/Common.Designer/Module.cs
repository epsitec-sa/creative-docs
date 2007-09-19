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
			this.designerApplication = designerApplication;
			this.mode = mode;
			this.moduleInfo = moduleId;

			this.resourceManager = new ResourceManager(this.designerApplication.ResourceManagerPool, moduleId);
			this.resourceManager.DefineDefaultModuleName(this.moduleInfo.Name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			this.modifier = new Modifier(this);

			this.accessStrings  = new ResourceAccess(ResourceAccess.Type.Strings,  this, this.moduleInfo, this.designerApplication);
			this.accessCaptions = new ResourceAccess(ResourceAccess.Type.Captions, this, this.moduleInfo, this.designerApplication);
			this.accessCommands = new ResourceAccess(ResourceAccess.Type.Commands, this, this.moduleInfo, this.designerApplication);
			this.accessPanels   = new ResourceAccess(ResourceAccess.Type.Panels,   this, this.moduleInfo, this.designerApplication);
			this.accessEntities = new ResourceAccess(ResourceAccess.Type.Entities, this, this.moduleInfo, this.designerApplication);
			this.accessTypes    = new ResourceAccess(ResourceAccess.Type.Types,    this, this.moduleInfo, this.designerApplication);
			this.Load();

			//	Attention: il faut avoir fait le this.accessEntities.Load() avant de cr�er this.accessFields !
			this.accessFields   = new ResourceAccess(ResourceAccess.Type.Fields,   this, this.moduleInfo, this.designerApplication);
			this.accessFields.Load();

			//	Attention: il faut avoir fait le this.accessTypes.Load() avant de cr�er this.accessValues !
			this.accessValues   = new ResourceAccess(ResourceAccess.Type.Values,   this, this.moduleInfo, this.designerApplication);
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
			ResourceBundleBatchSaver saver = new ResourceBundleBatchSaver ();
			//	Enregistre toutes les ressources.
			foreach (ResourceAccess access in this.Accesses)
			{
				access.Save(saver);
			}
			saver.Execute ();
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
			//	Montre une ressource 'Panel' dans une fen�tre.
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


		private void HandleAccessDirtyChanged(object sender)
		{
			//	Appel� lorsque l'�tat IsDirty d'un acc�s a chang�.
			this.designerApplication.GetCommandState("Save").Enable = this.IsGlobalDirty;
			this.designerApplication.GetCommandState("EditOk").Enable = this.IsLocalDirty;
			this.designerApplication.GetCommandState("EditCancel").Enable = this.IsLocalDirty;
			this.designerApplication.UpdateBookModules();
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


		protected DesignerApplication		designerApplication;
		protected DesignerMode				mode;
		protected ResourceModuleId			moduleInfo;
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
