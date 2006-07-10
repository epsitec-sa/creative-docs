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
			this.name = moduleInfo.Name;
			this.id   = moduleInfo.Id;

			this.resourceManager = new ResourceManager();
			this.resourceManager.DefineDefaultModuleName(this.name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			this.UpdateBundles();
			this.UpdateCaptions();

			this.modifier = new Modifier(this);
		}

		public void Dispose()
		{
			this.modifier.Dispose();
		}


		public MainWindow MainWindow
		{
			get
			{
				return this.mainWindow;
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


		public string Name
		{
			//	Retourne le nom du module.
			get
			{
				return this.name;
			}
		}

		public int Id
		{
			//	Retourne l'identificateur du module.
			get
			{
				return this.id;
			}
		}

		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		public ResourceBundleCollection Bundles
		{
			get
			{
				return this.bundles;
			}
		}

		public ResourceBundle GetCulture(string name)
		{
			//	Cherche le bundle d'une culture.
			for (int b=0; b<this.bundles.Count; b++)
			{
				ResourceBundle bundle = this.bundles[b];
				if (Misc.CultureName(bundle.Culture) == name)  return bundle;
			}
			return null;
		}

		public bool IsExistingCulture(string name)
		{
			//	Indique si une culture donnée existe.
			for (int b=0; b<this.bundles.Count; b++)
			{
				ResourceBundle bundle = this.bundles[b];
				if ( name == bundle.Culture.Name )  return true;
			}
			return false;
		}

		public ResourceBundle NewCulture(string name)
		{
			//	Crée un nouveau bundle pour une culture donnée.
			string prefix = this.resourceManager.ActivePrefix;
			System.Globalization.CultureInfo culture = Resources.FindSpecificCultureInfo(name);
			ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, this.bundles.Name, ResourceLevel.Localized, culture);

			bundle.DefineType("String");
			this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);
			
			this.UpdateBundles();

			return bundle;
		}

		public void DeleteCulture(ResourceBundle bundle)
		{
			//	Supprime une culture.
			// TODO: la suppression ne fonctionne pas !
			this.resourceManager.RemoveBundle("Strings", ResourceLevel.Localized, bundle.Culture);
			this.UpdateBundles();
		}

		protected void UpdateBundles()
		{
			string[] ids = this.resourceManager.GetBundleIds("*", "String", ResourceLevel.Default);
			if (ids.Length == 0)
			{
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, "Strings", ResourceLevel.Default, culture);
				bundle.DefineType("String");
				this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);
			}

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));
		}

#if false
		public void CreateIds()
		{
			//	Crée les 'druid' pour toutes les cultures du module.
			ResourceBundle defaultBundle = this.bundles[ResourceLevel.Default];

			int module = defaultBundle.Module.Id;
			int developer = 0;
			int local = 0;
			foreach (ResourceBundle.Field field in defaultBundle.Fields)
			{
				Druid druid = new Druid(module, developer, local++);
				string label = field.Name;

				foreach (ResourceBundle bundle in this.bundles)
				{
					ResourceBundle.Field f = bundle[label];
					if (!f.IsEmpty)
					{
						f.SetDruid(druid);
					}
				}
			}

			foreach (ResourceBundle bundle in this.bundles)
			{
				this.resourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
			}
		}
#endif


		#region Captions
		public ResourceBundleCollection Captions
		{
			get
			{
				return this.captions;
			}
		}

		protected void UpdateCaptions()
		{
			string[] ids = this.resourceManager.GetBundleIds("*", "Caption", ResourceLevel.Default);
			if (ids.Length == 0)
			{
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, "Captions", ResourceLevel.Default, culture);
				bundle.DefineType("Caption");
				this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);
			}
			else
			{
				this.captions = new ResourceBundleCollection(this.resourceManager);
				this.captions.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));
			}
		}
		#endregion


		#region Panels
		public void PanelsRead()
		{
			//	Charge toutes les ressources de type 'Panel'.
			if (this.panelsList != null)  return;

			this.panelsList = new List<ResourceBundle>();
			this.panelsToCreate = new List<ResourceBundle>();
			this.panelsToDelete = new List<ResourceBundle>();

			string[] names = this.resourceManager.GetBundleIds("*", "Panel", ResourceLevel.Default);
			if (names.Length == 0)
			{
				//	S'il n'existe aucun panneau, crée un premier panneau vide.
				//	Ceci est nécessaire, car il n'existe pas de commande pour créer un panneau à partir
				//	de rien, mais seulement une commande pour dupliquer un panneau existant.
				Druid druid = this.PanelCreateUniqueDruid();
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, druid.ToBundleId(), ResourceLevel.Default, culture);

				bundle.DefineType("Panel");
				bundle.DefineCaption(Res.Strings.Viewers.Panels.New);
				bundle.DefineRank(0);

				this.panelsList.Add(bundle);
				this.panelsToCreate.Add(bundle);
			}
			else
			{
				foreach (string name in names)
				{
					ResourceBundle bundle = this.resourceManager.GetBundle(name, ResourceLevel.Default);
					this.panelsList.Add(bundle);

					ResourceBundle.Field field = bundle["Panel"];
					
					if (field.IsValid)
					{
						UI.Panel panel = UserInterface.DeserializePanel(field.AsString, this.resourceManager);
						panel.DrawDesignerFrame = true;
						Viewers.Panels.SetPanel(bundle, panel);
					}
				}

				this.panelsList.Sort(new Comparers.BundleRank());  // trie selon les rangs
			}
		}

		public void PanelsWrite()
		{
			//	Enregistre toutes les modifications effectuées dans les ressources de type 'Panel'.
			if (this.panelsList == null)  return;

			for (int i=0; i<this.panelsList.Count; i++)
			{
				ResourceBundle bundle = this.panelsList[i];
				bundle.DefineRank(i);
				UI.Panel panel = Viewers.Panels.GetPanel (bundle);

				if (panel != null)
				{
					if (!bundle.Contains ("Panel"))
					{
						ResourceBundle.Field field = bundle.CreateField (ResourceFieldType.Data);
						field.SetName ("Panel");
						bundle.Add (field);
					}

					bundle["Panel"].SetXmlValue (UserInterface.SerializePanel (panel));
				}

				if (this.panelsToCreate.Contains(bundle))
				{
					this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);
				}
				else
				{
					this.resourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
				}
			}
			this.panelsToCreate.Clear();

			//	Supprime tous les panneaux mis dans la liste 'à supprimer'.
			foreach (ResourceBundle bundle in this.panelsToDelete)
			{
				this.resourceManager.RemoveBundle(bundle.Druid.ToBundleId(), ResourceLevel.Default, bundle.Culture);
			}
			this.panelsToDelete.Clear();
		}

		public void RunPanel(int index)
		{
			ResourceBundle bundle = this.panelsList[index];
			UI.Panel panel = Viewers.Panels.GetPanel(bundle);
			
			if (panel != null)
			{
				UserInterface.RunPanel(panel, this.resourceManager, this.PanelName(index));
			}
		}

		public int PanelsCount
		{
			//	Retourne le nombre total de ressources de type 'Panel'.
			get
			{
				return this.panelsList.Count;
			}
		}

		public int PanelIndex(Druid druid)
		{
			//	Donne l'index d'une ressource de type 'Panel'.
			for (int i=0; i<this.panelsList.Count; i++)
			{
				if (this.panelsList[i].Druid == druid)
				{
					return i;
				}
			}
			return -1;
		}

		public Druid PanelDruid(int index)
		{
			//	Donne le druid d'une ressource de type 'Panel'.
			return this.panelsList[index].Druid;
		}

		public string PanelName(int index)
		{
			//	Donne le nom d'une ressource de type 'Panel'.
			return this.panelsList[index].Caption;
		}

		public ResourceBundle PanelBundle(int index)
		{
			//	Donne une ressource de type 'Panel'.
			return this.panelsList[index];
		}

		public void PanelMove(Druid druid, int newIndex)
		{
			//	Déplace une ressource de type 'Panel'.
			int actualIndex = this.PanelIndex(druid);
			System.Diagnostics.Debug.Assert(actualIndex != -1);

			ResourceBundle bundle = this.panelsList[actualIndex];
			this.panelsList.RemoveAt(actualIndex);
			this.panelsList.Insert(newIndex, bundle);
		}

		public void PanelRename(Druid druid, string newName)
		{
			//	Renomme une ressource de type 'Panel'.
			int index = this.PanelIndex(druid);
			System.Diagnostics.Debug.Assert(index != -1);

			ResourceBundle bundle = this.panelsList[index];
			bundle.DefineCaption(newName);
		}

		public Druid PanelCreate(string name, int index)
		{
			//	Crée une nouvelle ressource de type 'Panel'.
			Druid druid = this.PanelCreateUniqueDruid();
			string prefix = this.resourceManager.ActivePrefix;
			System.Globalization.CultureInfo culture = this.BaseCulture;
			ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, druid.ToBundleId(), ResourceLevel.Default, culture);

			bundle.DefineType("Panel");
			bundle.DefineCaption(name);

			this.panelsList.Insert(index, bundle);
			this.panelsToCreate.Add(bundle);

			return druid;
		}

		public void PanelDelete(Druid druid)
		{
			//	Supprime une ressource de type 'Panel'.
			int index = this.PanelIndex(druid);
			System.Diagnostics.Debug.Assert(index != -1);
			ResourceBundle bundle = this.panelsList[index];

			this.panelsList.Remove(bundle);

			//	S'il ne s'agit pas d'une nouvelle ressource, il faut l'ajouter dans la liste
			//	des ressources à détruire (lors du PanelsWrite).
			if (this.panelsToCreate.Contains(bundle))
			{
				this.panelsToCreate.Remove(bundle);
			}
			else
			{
				this.panelsToDelete.Add(bundle);
			}
		}

		protected Druid PanelCreateUniqueDruid()
		{
			//	Crée un nouveau druid unique pour une ressource de type 'Panel'.
			int moduleId = this.id;
			int developerId = 0;  // [PA] provisoire
			int localId = 0;

			foreach (ResourceBundle bundle in this.panelsList)
			{
				Druid druid = bundle.Druid;

				if (druid.IsValid && druid.Developer == developerId && druid.Local >= localId)
				{
					localId = druid.Local+1;
				}
			}

			return new Druid(moduleId, developerId, localId);
		}
		#endregion


		protected System.Globalization.CultureInfo BaseCulture
		{
			//	Retourne la culture de base, définie par les ressources "Strings".
			get
			{
				ResourceBundle res = this.bundles[ResourceLevel.Default];
				return res.Culture;
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


		protected static readonly string	PanelPrefix = "P.";

		protected MainWindow				mainWindow;
		protected DesignerMode				mode;
		protected string					name;
		protected int						id;
		protected ResourceManager			resourceManager;
		protected ResourceBundleCollection	bundles;
		protected ResourceBundleCollection	captions;
		protected List<ResourceBundle>		panelsList;
		protected List<ResourceBundle>		panelsToCreate;
		protected List<ResourceBundle>		panelsToDelete;
		protected Modifier					modifier;
	}
}
