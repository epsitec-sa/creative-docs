//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

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
				return this.application;
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

		public AbstractGroup RootWidget
		{
			get
			{
				if (this.rootWidget == null)
				{
					System.Diagnostics.Debug.Assert (this.Application != null);
					System.Diagnostics.Debug.Assert (this.Application.Window != null);

					this.SetupUserInterface (this.CreateUserInterface ());
				}

				return this.rootWidget;
			}
		}


		/// <summary>
		/// Gets the path of the focused field.
		/// </summary>
		/// <value>The path of the focused field or <c>null</c>.</value>
		public string FocusPath
		{
			get
			{
				return this.focusFieldPath;
			}
			set
			{
				if (this.focusFieldPath != value)
				{
					this.focusFieldPath = value;

					//	TODO: generate event...
				}
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
			this.rootWidget = container;
			this.rootWidget.Dock = DockStyle.Fill;
			this.rootWidget.Name = this.GetType ().Name;
		}

		private void DefineState(States.CoreState state)
		{
			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (this.state == null);

			this.state = state;
			this.application = state.StateManager.Application;
		}



		private States.CoreState				state;
		private CoreApplication					application;
		private AbstractGroup					rootWidget;
		private bool							enabled;
		private string							focusFieldPath;
	}
}
