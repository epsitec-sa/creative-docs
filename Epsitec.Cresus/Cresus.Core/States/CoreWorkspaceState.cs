//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.States;
using Epsitec.Cresus.Core.Workspaces;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>CoreWorkspaceState</c> class manages the state associated with a
	/// workspace, as implemented by a class derived from <see cref="CoreWorkspace"/>.
	/// </summary>
	public abstract class CoreWorkspaceState<T> : CoreState where T : Workspaces.CoreWorkspace, new ()
	{
		protected CoreWorkspaceState(StateManager manager)
			: base (manager)
		{
		}


		public T								Workspace
		{
			get
			{
				return this.workspace;
			}
			internal set
			{
				if (this.workspace != value)
				{
					System.Diagnostics.Debug.Assert (this.workspace == null);
					
					this.workspace = value;
					this.workspace.State = this;
				}
			}
		}


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
			System.Diagnostics.Debug.Assert (this.workspace == null);

			this.workspace = new T ()
			{
				State = this
			};

			this.RestoreWorkspace (element.Element ("workspace"));
			this.RestoreCoreState (element);

			return this;
		}




		public static IEnumerable<CoreWorkspaceState<T>> FindAll(StateManager manager, System.Predicate<CoreWorkspaceState<T>> filter)
		{
			foreach (CoreState state in manager.GetAllStates ())
			{
				CoreWorkspaceState<T> workspaceState = state as CoreWorkspaceState<T>;

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
			if (this.workspace != null)
			{
				this.workspace.RootWidget.SetParent (container);
				this.workspace.SetEnable (true);
			}
		}

		protected override void DetachState()
		{
			if (this.workspace != null)
			{
				this.workspace.SetEnable (false);
				this.workspace.RootWidget.SetParent (null);
			}
		}


		private T								workspace;
	}
}
