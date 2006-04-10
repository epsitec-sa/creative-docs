using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Description d'un module de ressources ouvert par l'application Designer.
	/// </summary>
	class Module
	{
		public Module(string resourcePrefix, string moduleName)
		{
			this.resourceManager = new ResourceManager();
			this.resourceManager.SetupApplication(moduleName);
			this.resourceManager.ActivePrefix = resourcePrefix;
			string[] ids = this.resourceManager.GetBundleIds("*", ResourceLevel.Default);

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));
		}


		protected ResourceManager			resourceManager;
		protected ResourceBundleCollection	bundles;
	}
}
