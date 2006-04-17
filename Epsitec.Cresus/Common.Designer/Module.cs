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
			this.mainWindow = mainWindow;
			this.mode = mode;
			this.name = moduleName;

			this.resourceManager = new ResourceManager();
			this.resourceManager.SetupApplication(this.name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			this.UpdateBundles();

			this.modifier = new Modifier(this);
			this.notifier = new Notifier(this);
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

		public Notifier Notifier
		{
			get
			{
				return this.notifier;
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
			ResourceBundle bundle = ResourceBundle.Create (this.resourceManager, prefix, this.bundles.Name, ResourceLevel.Localized, culture);

			//	Pour l'instant, l'éditeur ne sait gérer que des bundles de type "String",
			//	donc on force ici explicitement ce type :
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
			string[] ids = this.resourceManager.GetBundleIds("*", ResourceLevel.Default);

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));
		}


		protected MainWindow				mainWindow;
		protected DesignerMode				mode;
		protected string					name;
		protected ResourceManager			resourceManager;
		protected ResourceBundleCollection	bundles;
		protected Modifier					modifier;
		protected Notifier					notifier;
	}
}
