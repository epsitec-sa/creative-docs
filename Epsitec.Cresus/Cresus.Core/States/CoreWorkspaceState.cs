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

		public CoreState						LinkedState
		{
			get;
			set;
		}

		public string							LinkedStateFieldPath
		{
			get;
			set;
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


		protected virtual void StoreWorkspace(XElement element, StateSerializationContext context)
		{
			if (this.workspace != null)
			{
				if (this.LinkedState != null)
				{
					string link = context.GetTag (this.LinkedState);

					if (this.LinkedStateFieldPath != null)
					{
						link = string.Concat (link, " ", this.LinkedStateFieldPath);
					}
					
					element.Add (new XAttribute ("link", link));
				}

//				element.Add (new XAttribute ("entityId", this.workspace.EntityId.ToString ()));
//				element.Add (new XAttribute ("formId", this.workspace.FormId.ToString ()));
//				element.Add (new XAttribute ("mode", this.workspace.Mode.ToString ()));
//				element.Add (new XAttribute ("currentEntityId", currentEntityId ?? ""));
//				element.Add (new XAttribute ("focusPath", this.workspace.FocusPath == null ? "" : this.workspace.FocusPath.ToString ()));
			}
		}

		protected virtual void RestoreWorkspace(XElement workspaceElement)
		{
			System.Diagnostics.Debug.Assert (this.workspace != null);

			if (workspaceElement != null)
			{
				string entityId        = (string) workspaceElement.Attribute ("entityId");
				string formId          = (string) workspaceElement.Attribute ("formId");
				string mode            = (string) workspaceElement.Attribute ("mode");
				string currentEntityId = (string) workspaceElement.Attribute ("currentEntityId");
				string focusPath       = (string) workspaceElement.Attribute ("focusPath");
				string link            = (string) workspaceElement.Attribute ("link");

#if false
				XElement dialogDataXml = workspaceElement.Element ("dialogData");
				
				this.workspace.EntityId = Druid.Parse (entityId);
				this.workspace.FormId   = Druid.Parse (formId);
				this.workspace.Mode     = mode.ToEnum<FormWorkspaceMode> (FormWorkspaceMode.None);

				AbstractEntity item = this.ResolvePersistedEntity (currentEntityId);

				switch (this.workspace.Mode)
				{
					case FormWorkspaceMode.Edition:
						this.workspace.CurrentItem = item;
						break;

					case FormWorkspaceMode.Creation:
						this.workspace.CurrentItem = this.StateManager.Application.Data.DataContext.CreateEntity (this.workspace.EntityId);
						break;

					case FormWorkspaceMode.Search:
						this.RegisterFixup (() => this.workspace.SelectEntity (item));
						break;
				}
				
				if (string.IsNullOrEmpty (focusPath) == false)
				{
					this.workspace.FocusPath = EntityFieldPath.Parse (focusPath);
				}
#endif
				
				if (!string.IsNullOrEmpty (link))
				{
					//	The link is expressed either as "tag" or "tag path", which have
					//	to be restored as LinkedState and LinkedFieldPath.
					
					string[] args = link.Split (' ');

					if (args.Length > 1)
					{
						this.LinkedStateFieldPath = args[1];
					}

					this.RegisterFixup (context => this.LinkedState = context.GetState (args[0]));
				}

#if false
				this.workspace.Initialize ();

				if (dialogDataXml != null)
				{
					FormWorkspaceState.RestoreDialogData (this.workspace.DialogData, dialogDataXml);
				}
#endif
			}
		}

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
