//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Workspaces
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
				return this.StateManager == null ? null : this.StateManager.Application;
			}
		}

		public StateManager StateManager
		{
			get
			{
				return this.state == null ? null : this.state.StateManager;
			}
		}

		public States.CoreState State
		{
			get
			{
				return this.state;
			}
			internal set
			{
				this.DefineState (value);
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

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		
		public void SetEnable(bool enable)
		{
			if (this.enabled != enable)
			{
				if (enable)
				{
					this.EnableWorkspace ();
					this.enabled = true;
				}
				else
				{
					this.DisableWorkspace ();
					this.enabled = false;
				}
			}
		}

		public abstract AbstractGroup CreateUserInterface();

		protected abstract void EnableWorkspace();
		
		protected abstract void DisableWorkspace();
		
		private void SetupUserInterface(AbstractGroup container)
		{
			this.container = container;
			this.container.Dock = DockStyle.Fill;
			this.container.Name = this.GetType ().Name;
		}

		private void DefineState(States.CoreState state)
		{
			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (this.state == null);

			this.state = state;
		}



		private States.CoreState state;
		private CoreApplication application;
		private AbstractGroup container;
		private bool enabled;
	}
}
