//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.States;
using Epsitec.Cresus.Core.Workspaces;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>CoreWorkspaceState</c> class manages the state associated with a
	/// workspace, as implemented by a class derived from <see cref="CoreWorkspace"/>.
	/// </summary>
	public abstract class CoreWorkspaceState : CoreState
	{
		protected CoreWorkspaceState(StateManager manager)
			: base (manager)
		{
		}


		public CoreApplication Application
		{
			get
			{
				return this.StateManager.Application;
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



		public override XElement Serialize(XElement element, StateSerializationContext context)
		{
			XElement workspace = new XElement ("workspace");

			this.StoreCoreState (element, context);
			this.StoreWorkspace (workspace, context);

			element.Add (workspace);

			return element;
		}

		public override CoreState Deserialize(XElement element)
		{
			this.RestoreWorkspace (element.Element ("workspace"));
			this.RestoreCoreState (element);

			return this;
		}




		public static IEnumerable<T> FindAll<T>(StateManager manager, System.Predicate<T> filter) where T : CoreWorkspaceState
		{
			foreach (CoreState state in manager.GetAllStates ())
			{
				T workspaceState = state as T;

				if (workspaceState != null)
				{
					if (filter (workspaceState))
					{
						yield return workspaceState;
					}
				}
			}
		}


		protected abstract void StoreWorkspace(XElement workspaceElement, StateSerializationContext context);
		
		protected abstract void RestoreWorkspace(XElement workspaceElement);

		
		protected override void AttachState(Epsitec.Common.Widgets.Widget container)
		{
			this.RootWidget.SetParent (container);
			this.SetEnable (true);
		}

		protected override void DetachState()
		{
			this.SetEnable (false);
			this.RootWidget.SetParent (null);
		}
	}
}
