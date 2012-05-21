//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Designer
{
	public sealed class Designer
	{
		public Designer(Widgets.Application hostApplication)
			: this (hostApplication, hostApplication.ResourceManagerPool)
		{
		}

		public Designer(Widgets.Application hostApplication, Support.ResourceManagerPool pool)
		{
			this.main = new DesignerApplication (pool);
			this.hostApplication = hostApplication;
		}

		public Support.ResourceManagerPool ResourceManagerPool
		{
			get
			{
				return this.main.ResourceManagerPool;
			}
		}

		public DesignerMode DesignerMode
		{
			get
			{
				return this.main.Mode;
			}
			set
			{
				this.main.Mode = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				Widgets.Window window = this.main.Window;
				
				if (window == null)
				{
					return false;
				}

				return window.IsVisible;
			}
		}

		public void Show()
		{
			this.main.Show (this.hostApplication.Window);
		}

		public void Hide()
		{
			this.main.Hide ();
		}

		private DesignerApplication main;
		private Widgets.Application hostApplication;
	}
}
