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
		public string[] PanelNames
		{
			//	Retourne la liste des noms des panneaux.
			get
			{
				string[] panelNames = this.resourceManager.GetBundleIds(Module.PanelPrefix+"*", "Panel", ResourceLevel.Default);

				//	S'il n'existe aucun panneau, crée un premier panneau vide.
				//	Ceci est nécessaire, car il n'existe pas de commande pour créer un panneau à partir
				//	de rien, mais seulement une commande pour dupliquer un panneau existant.
				if (panelNames.Length == 0)
				{
					string prefix = this.resourceManager.ActivePrefix;
					System.Globalization.CultureInfo culture = this.BaseCulture;
					ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, Module.PanelPrefix+Res.Strings.Viewers.Panels.New, ResourceLevel.Default, culture);

					bundle.DefineType("Panel");
					bundle.DefineRank(0);
					this.WriteBundle(bundle);

					panelNames = this.resourceManager.GetBundleIds(Module.PanelPrefix+"*", "Panel", ResourceLevel.Default);
					System.Diagnostics.Debug.Assert(panelNames.Length == 1);
				}

				return panelNames;
			}
		}

		public ResourceBundle NewPanel(string name)
		{
			//	Crée une nouvelle ressource de type panneau.
			string prefix = this.resourceManager.ActivePrefix;
			System.Globalization.CultureInfo culture = BaseCulture;
			ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, name, ResourceLevel.Default, culture);
			bundle.DefineType("Panel");

			return bundle;
		}

		public void DeletePanel(string name)
		{
			//	Supprime une ressource de type panneau.
		}

		public ResourceBundle LoadPanelBundle(string label)
		{
			//	Retourne le bundle d'un panneau.
			label = Module.AddPanelPrefix(label);
			return this.resourceManager.GetBundle(label);
		}

		public static string RemovePanelPrefix(string name)
		{
			//	Enlève le préfixe "P." s'il existe.
			if (name.StartsWith(Module.PanelPrefix))
			{
				name = name.Remove(0, Module.PanelPrefix.Length);
			}
			return name;
		}

		public static string AddPanelPrefix(string name)
		{
			//	Ajoute le préfixe "P." s'il n'existe pas.
			if (!name.StartsWith(Module.PanelPrefix))
			{
				name = Module.PanelPrefix + name;
			}
			return name;
		}
		#endregion


		public void WriteBundle(ResourceBundle bundle)
		{
			//	Sérialise un bundle.
			this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);
		}

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
		protected Modifier					modifier;
	}
}
