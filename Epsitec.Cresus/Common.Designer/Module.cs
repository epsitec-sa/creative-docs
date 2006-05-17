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
		public Module(MainWindow mainWindow, DesignerMode mode, string resourcePrefix, string moduleName)
		{
			this.UniqueIDCreate();

			this.mainWindow = mainWindow;
			this.mode = mode;
			this.name = moduleName;

			this.resourceManager = new ResourceManager();
			this.resourceManager.SetupApplication(this.name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			this.UpdateBundles();

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
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(name);
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
			System.Diagnostics.Debug.Assert(ids.Length == 1);

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));
		}


		#region Panels
		public void PanelsRead()
		{
			//	Charge toutes les ressources de type 'Panel'.
			if (this.panelsList != null)  return;

			this.panelsList = new List<LoadedBundle>();
			this.panelsToDelete = new List<LoadedBundle>();

			string[] names = this.resourceManager.GetBundleIds(Module.AddPanelPrefix("*"), "Panel", ResourceLevel.Default);
			if (names.Length == 0)
			{
				//	S'il n'existe aucun panneau, crée un premier panneau vide.
				//	Ceci est nécessaire, car il n'existe pas de commande pour créer un panneau à partir
				//	de rien, mais seulement une commande pour dupliquer un panneau existant.
				string prefix = this.resourceManager.ActivePrefix;
				string name = Module.AddPanelPrefix(Res.Strings.Viewers.Panels.New);
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, name, ResourceLevel.Default, culture);

				bundle.DefineType("Panel");
				bundle.DefineRank(0);

				LoadedBundle loaded = new LoadedBundle(Module.RemovePanelPrefix(name), bundle);
				loaded.Newest = true;
				this.panelsList.Add(loaded);
			}
			else
			{
				foreach (string name in names)
				{
					ResourceBundle bundle = this.resourceManager.GetBundle(name, ResourceLevel.Default);

					LoadedBundle loaded = new LoadedBundle(Module.RemovePanelPrefix(name), bundle);
					this.panelsList.Add(loaded);
				}

				this.panelsList.Sort();  // trie selon les rangs
			}
		}

		public void PanelsWrite()
		{
			//	Enregistre toutes les modifications effectuées dans les ressources de type 'Panel'.
			if (this.panelsList == null)  return;

			for (int i=0; i<this.panelsList.Count; i++)
			{
				LoadedBundle loaded = this.panelsList[i];

				loaded.Bundle.DefineName(Module.AddPanelPrefix(loaded.Name));
				loaded.Bundle.DefineRank(i);

				if (loaded.Newest)
				{
					this.resourceManager.SetBundle(loaded.Bundle, ResourceSetMode.CreateOnly);
					loaded.Newest = false;
				}
				else
				{
					this.resourceManager.SetBundle(loaded.Bundle, ResourceSetMode.UpdateOnly);
				}
			}

			//	Supprime tous les panneaux mis dans la liste 'à supprimer'.
			foreach (LoadedBundle loaded in this.panelsToDelete)
			{
				this.resourceManager.RemoveBundle(Module.AddPanelPrefix(loaded.Name), ResourceLevel.Default, loaded.Bundle.Culture);
			}
			this.panelsToDelete.Clear();
		}

		public int PanelsCount
		{
			//	Retourne le nombre total de ressources de type 'Panel'.
			get
			{
				return this.panelsList.Count;
			}
		}

		public int PanelIndex(string name)
		{
			//	Donne l'index d'une ressource de type 'Panel'.
			for (int i=0; i<this.panelsList.Count; i++)
			{
				if (this.panelsList[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		public string PanelName(int index)
		{
			//	Donne le nom d'une ressource de type 'Panel'.
			return this.panelsList[index].Name;
		}

		public bool PanelNewest(int index)
		{
			//	Indique si une ressource de type 'Panel' est renommable.
			return this.panelsList[index].Newest;
		}

		public ResourceBundle PanelBundle(int index)
		{
			//	Donne une ressource de type 'Panel'.
			return this.panelsList[index].Bundle;
		}

		public void PanelMove(string name, int newIndex)
		{
			//	Déplace une ressource de type 'Panel'.
			int actualIndex = this.PanelIndex(name);
			System.Diagnostics.Debug.Assert(actualIndex != -1);

			LoadedBundle loaded = this.panelsList[actualIndex];
			this.panelsList.RemoveAt(actualIndex);
			this.panelsList.Insert(newIndex, loaded);
		}

		public void PanelRename(string actualName, string newName)
		{
			//	Renomme une ressource de type 'Panel'.
			int index = this.PanelIndex(actualName);
			System.Diagnostics.Debug.Assert(index != -1);

			LoadedBundle loaded = this.panelsList[index];
			System.Diagnostics.Debug.Assert(loaded.Newest);
			loaded.Name = newName;
		}

		public void PanelCreate(string name, int index)
		{
			//	Crée une nouvelle ressource de type 'Panel'.
			name = Module.AddPanelPrefix(name);
			string prefix = this.resourceManager.ActivePrefix;
			System.Globalization.CultureInfo culture = this.BaseCulture;
			ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, name, ResourceLevel.Default, culture);

			bundle.DefineType("Panel");

			LoadedBundle loaded = new LoadedBundle(Module.RemovePanelPrefix(name), bundle);
			loaded.Newest = true;

			this.panelsList.Insert(index, loaded);
		}

		public void PanelDelete(string name)
		{
			//	Supprime une ressource de type 'Panel'.
			int index = this.PanelIndex(name);
			System.Diagnostics.Debug.Assert(index != -1);

			LoadedBundle loaded = this.panelsList[index];

			this.panelsList.RemoveAt(index);

			//	S'il ne s'agit pas d'une nouvelle ressource, il faut l'ajouter dans la liste
			//	des ressources à détruire (lors du PanelsWrite).
			if (!loaded.Newest)
			{
				this.panelsToDelete.Add(loaded);
			}
		}

		protected static string RemovePanelPrefix(string name)
		{
			//	Enlève le préfixe "P." s'il existe.
			if (name.StartsWith(Module.PanelPrefix))
			{
				name = name.Remove(0, Module.PanelPrefix.Length);
			}
			return name;
		}

		protected static string AddPanelPrefix(string name)
		{
			//	Ajoute le préfixe "P." s'il n'existe pas.
			if (!name.StartsWith(Module.PanelPrefix))
			{
				name = Module.PanelPrefix + name;
			}
			return name;
		}
		#endregion


		#region LoadedBundle
		protected class LoadedBundle : System.IComparable
		{
			public LoadedBundle(string name, ResourceBundle bundle)
			{
				this.Name   = name;
				this.Bundle = bundle;
				this.Newest = false;
			}

			public string				Name;
			public ResourceBundle		Bundle;
			public bool					Newest;

			public int CompareTo(object obj)
			{
				LoadedBundle that = obj as LoadedBundle;
				return this.Bundle.Rank.CompareTo(that.Bundle.Rank);
			}
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
		protected ResourceManager			resourceManager;
		protected ResourceBundleCollection	bundles;
		protected List<LoadedBundle>		panelsList;
		protected List<LoadedBundle>		panelsToDelete;
		protected Modifier					modifier;
	}
}
