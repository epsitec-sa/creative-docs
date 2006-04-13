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
		public Module(DesignerMode mode, string resourcePrefix, string moduleName)
		{
			this.mode = mode;
			this.name = moduleName;

			this.resourceManager = new ResourceManager();
			this.resourceManager.SetupApplication(this.name);
			this.resourceManager.ActivePrefix = resourcePrefix;

			string[] ids = this.resourceManager.GetBundleIds("*", ResourceLevel.Default);

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));

			this.modifier = new Modifier(this);
			this.notifier = new Notifier(this);
		}

		public void Dispose()
		{
			this.modifier.Dispose();
		}


		public DesignerMode Mode
		{
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


		public void Save()
		{
			//	Enregistre tout le module.
			ResourceBundleCollection bundles = this.Bundles;
			foreach (ResourceBundle bundle in bundles)
			{
				this.ResourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
			}
			this.modifier.IsDirty = false;
		}


		protected DesignerMode				mode;
		protected string					name;
		protected ResourceManager			resourceManager;
		protected ResourceBundleCollection	bundles;
		protected Modifier					modifier;
		protected Notifier					notifier;
	}
}
