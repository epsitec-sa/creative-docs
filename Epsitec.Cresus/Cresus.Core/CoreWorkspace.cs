//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public abstract class CoreWorkspace
	{
		public CoreWorkspace()
		{
		}

		public CoreApplication Application
		{
			get
			{
				return this.application;
			}
			internal set
			{
				this.DefineCoreApplication (value);
			}
		}

		public AbstractGroup Container
		{
			get
			{
				if (this.container == null)
				{
					System.Diagnostics.Debug.Assert (this.Application != null);
					System.Diagnostics.Debug.Assert (this.Application.Window != null);

					this.SetupUserInterface (this.CreateUserInterface ());
				}

				return this.container;
			}
		}

		private void SetupUserInterface(AbstractGroup container)
		{
			this.container = container;
			this.container.Dock = DockStyle.Fill;
			this.container.Name = this.GetType ().Name;
		}
		
		private void DefineCoreApplication(CoreApplication coreApplication)
		{
			System.Diagnostics.Debug.Assert (coreApplication != null);
			System.Diagnostics.Debug.Assert (this.application == null);

			this.application = coreApplication;
		}


		public abstract AbstractGroup CreateUserInterface();


		private CoreApplication application;
		private AbstractGroup container;
	}
}
