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
		public Module(MainWindow mainWindow, DesignerMode mode, string resourcePrefix, ResourceModuleInfo moduleInfo)
		{
			this.UniqueIDCreate();

			this.mainWindow = mainWindow;
			this.mode = mode;
			this.moduleInfo = moduleInfo;

			this.resourceManager = new ResourceManager(this.mainWindow.ResourceManagerPool);
			this.resourceManager.DefineDefaultModuleName(this.moduleInfo.Name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			this.modifier = new Modifier(this);

			this.accessStrings  = new ResourceAccess(ResourceAccess.Type.Strings,  this.resourceManager, this.moduleInfo, this.mainWindow);
			this.accessCaptions = new ResourceAccess(ResourceAccess.Type.Captions, this.resourceManager, this.moduleInfo, this.mainWindow);
			this.accessPanels   = new ResourceAccess(ResourceAccess.Type.Panels,   this.resourceManager, this.moduleInfo, this.mainWindow);
			this.accessScripts  = new ResourceAccess(ResourceAccess.Type.Scripts,  this.resourceManager, this.moduleInfo, this.mainWindow);

			foreach (ResourceAccess access in Access)
			{
				access.DirtyChanged += new EventHandler(this.HandleAccessDirtyChanged);
			}

			this.Load();
		}

		public void Dispose()
		{
			foreach (ResourceAccess access in Access)
			{
				access.DirtyChanged -= new EventHandler(this.HandleAccessDirtyChanged);
			}

			this.modifier.Dispose();
		}


		public MainWindow MainWindow
		{
			get
			{
				return this.mainWindow;
			}
		}

		public MyWidgets.BundleType BundleTypeWidget
		{
			set
			{
				this.bundleTypeWidget = value;
				this.LocatorInit();
			}
			get
			{
				return this.bundleTypeWidget;
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

		public ResourceAccess AccessPanels
		{
			get
			{
				return this.accessPanels;
			}
		}

		public ResourceAccess GetAccess(ResourceAccess.Type type)
		{
			//	Cherche un accès d'après son type.
			switch (type)
			{
				case ResourceAccess.Type.Strings:
					return this.accessStrings;

				case ResourceAccess.Type.Captions:
				case ResourceAccess.Type.Fields:
				case ResourceAccess.Type.Commands:
				case ResourceAccess.Type.Types:
				case ResourceAccess.Type.Values:
					return this.accessCaptions;

				case ResourceAccess.Type.Panels:
					return this.accessPanels;

				case ResourceAccess.Type.Scripts:
					return this.accessScripts;
			}

			return null;
		}


		public void Load()
		{
			//	Charge toutes les ressources.
			foreach (ResourceAccess access in Access)
			{
				access.Load();
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
				this.mainWindow.DialogMessage(Res.Strings.Error.CheckOK);
			}
			else
			{
				this.mainWindow.DialogError(message);
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
				UserInterface.RunPanel(panel, this.resourceManager, this.mainWindow.Window, name);
			}
		}


		public bool IsDirty
		{
			get
			{
				foreach (ResourceAccess access in Access)
				{
					if (access.IsDirty)
					{
						return true;
					}
				}

				return false;
			}
		}


		#region Locator
		protected void LocatorInit()
		{
			//	Initialise la liste des localisations.
			System.Diagnostics.Debug.Assert(this.bundleTypeWidget != null);
			this.locators = new List<Viewers.Locator>();
			this.locatorIndex = -1;
			this.locatorIgnore = false;
		}

		public void LocatorFix()
		{
			//	Fixe une vue que l'on vient d'atteindre.
			System.Diagnostics.Debug.Assert(this.locators != null);
			if (this.locatorIgnore)
			{
				return;
			}

			ResourceAccess.Type viewerType = this.bundleTypeWidget.CurrentType;

			Druid resource = Druid.Empty;
			ResourceAccess access = this.GetAccess(viewerType);
			int sel = access.AccessIndex;
			if (sel != -1)
			{
				resource = access.AccessDruid(sel);
			}

			Viewers.Locator locator = new Viewers.Locator(viewerType, resource);

			if (this.locatorIndex >= 0 && this.locatorIndex < this.locators.Count)
			{
				if (locator == this.locators[this.locatorIndex])  // locateur déjà fixé ?
				{
					return;  // si oui, il ne faut surtout rien fixer
				}
			}

			//	Supprime les localisations après la localisation courante.
			while (this.locators.Count-1 > this.locatorIndex)
			{
				this.locators.RemoveAt(this.locators.Count-1);
			}

			this.locators.Add(locator);
			this.locatorIndex++;

			this.mainWindow.UpdateCommandLocator();
		}

		public bool LocatorPrevIsEnable
		{
			//	Donne l'état de la commande "LocatorPrev".
			get
			{
				System.Diagnostics.Debug.Assert(this.locators != null);
				return this.locatorIndex > 0;
			}
		}

		public bool LocatorNextIsEnable
		{
			//	Donne l'état de la commande "LocatorNext".
			get
			{
				System.Diagnostics.Debug.Assert(this.locators != null);
				return this.locatorIndex < this.locators.Count-1;
			}
		}

		public void LocatorPrev()
		{
			//	Action de la commande "LocatorPrev".
			System.Diagnostics.Debug.Assert(this.LocatorPrevIsEnable);
			Viewers.Locator locator = this.locators[--this.locatorIndex];
			this.LocatorGoto(locator);
		}

		public void LocatorNext()
		{
			//	Action de la commande "LocatorNext".
			System.Diagnostics.Debug.Assert(this.LocatorNextIsEnable);
			Viewers.Locator locator = this.locators[++this.locatorIndex];
			this.LocatorGoto(locator);
		}

		protected void LocatorGoto(Viewers.Locator locator)
		{
			this.locatorIgnore = true;
			this.bundleTypeWidget.CurrentType = locator.ViewerType;
			this.locatorIgnore = false;

			if (!locator.Resource.IsEmpty)
			{
				ResourceAccess access = this.GetAccess(locator.ViewerType);
				int sel = access.AccessIndexOfDruid(locator.Resource);
				if (sel != -1)
				{
					access.AccessIndex = sel;
					Viewers.Abstract viewer = this.mainWindow.CurrentViewer;
					if (viewer != null)
					{
						this.locatorIgnore = true;
						viewer.Update();
						this.locatorIgnore = false;
					}
				}
			}

			this.LocatorFix();
			this.mainWindow.UpdateCommandLocator();
		}
		#endregion


		private void HandleAccessDirtyChanged(object sender)
		{
			//	Appelé lorsque l'état IsDirty d'un accès a changé.
			this.mainWindow.GetCommandState("Save").Enable = this.IsDirty;
			this.mainWindow.UpdateBookModules();
		}


		protected IEnumerable<ResourceAccess> Access
		{
			//	Enumère tous les accès.
			get
			{
				yield return accessStrings;
				yield return accessCaptions;
				yield return accessPanels;
				yield return accessScripts;
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


		protected MainWindow				mainWindow;
		protected MyWidgets.BundleType		bundleTypeWidget;
		protected DesignerMode				mode;
		protected ResourceModuleInfo		moduleInfo;
		protected Modifier					modifier;
		protected ResourceManager			resourceManager;
		protected ResourceAccess			accessStrings;
		protected ResourceAccess			accessCaptions;
		protected ResourceAccess			accessPanels;
		protected ResourceAccess			accessScripts;
		protected List<Viewers.Locator>		locators;
		protected int						locatorIndex;
		protected bool						locatorIgnore;
	}
}
