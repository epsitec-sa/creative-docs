//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		}


		public CoreData Data
		{
			get
			{
				return this.data;
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

			this.Window = window;

			this.CreateWorkspaces ();
		}

		public void SetupData()
		{
			this.data.SetupDatabase ();
		}

		private void CreateWorkspaces()
		{
			this.formWorkspace = new FormWorkspace ()
			{
				Application = this
			};

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
			}

			base.Dispose (disposing);
		}


		FormWorkspace formWorkspace;
		CoreData data;
	}
}
