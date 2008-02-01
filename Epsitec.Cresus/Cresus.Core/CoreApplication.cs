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
		}

		public override string ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName;
			}
		}

		
		public void CreateUserInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;

			this.Window = window;

			this.CreateWorkspaces ();
		}

		private void CreateWorkspaces()
		{
			this.formWorkspace = new FormWorkspace ()
			{
				Application = this
			};

			this.Window.Root.Children.Add (this.formWorkspace.Container);
		}



		FormWorkspace formWorkspace;
	}
}
