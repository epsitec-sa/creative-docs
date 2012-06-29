using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Maintenance
{
	internal sealed class MaintenanceApp : CoreApp
	{
		public MaintenanceApp()
		{
			this.coreData = this.GetComponent<CoreData> ();
			this.businessContext = new BusinessContext (this.coreData);

			Services.SetApplication (this);
		}

		public override string ApplicationIdentifier
		{
			get
			{
				return "Maintenance app";
			}
		}

		public override string ShortWindowTitle
		{
			get
			{
				return "Maintenance app";
			}
		}

		public CoreData CoreData
		{
			get
			{
				return this.CoreData;
			}
		}

		public BusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.businessContext.Dispose ();
			}
			
			base.Dispose (disposing);
		}

		private readonly CoreData coreData;
		private readonly BusinessContext businessContext;
	}
}