//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreApplication</c> class implements the central application
	/// logic.
	/// </summary>
	public class CoreApplication : Application
	{
		public CoreApplication()
		{
			this.data = new CoreData ();
			this.exceptionManager = new CoreLibrary.ExceptionManager ();
		}


		public CoreData Data
		{
			get
			{
				return this.data;
			}
		}

		public IExceptionManager ExceptionManager
		{
			get
			{
				return this.exceptionManager;
			}
		}
		
		public override string ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName;
			}
		}

		
		public void SetupInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;
			window.ClientSize = new Epsitec.Common.Drawing.Size (600, 400);

			this.Window = window;

			this.CreateWorkspaces ();
			this.formWorkspace.SetEnable (true);
		}

		public void SetupData()
		{
			this.data.SetupDatabase ();
		}

		private void CreateWorkspaces()
		{
			this.formWorkspace = new Workspaces.FormWorkspace ()
			{
				Application = this,
				FormId      = Epsitec.Cresus.Mai2008.FormIds.Facture,
				EntityId    = Epsitec.Cresus.Mai2008.Entities.FactureEntity.EntityStructuredTypeId
			};
#if false
			Druid formId   = Epsitec.Cresus.AddressBook.FormIds.AdressePersonne;
			Druid entityId = Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId;
#endif

			this.Window.Root.Children.Add (this.formWorkspace.Container);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.data != null)
				{
					this.data.Dispose ();
					this.data = null;
				}
				if (this.exceptionManager != null)
				{
					this.exceptionManager.Dispose ();
					this.exceptionManager = null;
				}
			}

			base.Dispose (disposing);
		}


		Workspaces.FormWorkspace				formWorkspace;
		CoreData								data;
		CoreLibrary.ExceptionManager			exceptionManager;
	}
}
