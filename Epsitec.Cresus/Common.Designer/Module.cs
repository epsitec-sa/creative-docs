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
		public Module(string resourcePrefix, string moduleName)
		{
			this.name = moduleName;

			this.resourceManager = new ResourceManager();
			this.resourceManager.SetupApplication(this.name);
			this.resourceManager.ActivePrefix = resourcePrefix;
			string[] ids = this.resourceManager.GetBundleIds("*", ResourceLevel.Default);

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));

			this.modifier = new Modifier(this);
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
			get
			{
				return this.name;
			}
		}


		protected string					name;
		protected ResourceManager			resourceManager;
		protected ResourceBundleCollection	bundles;
		protected Modifier					modifier;
	}
}
