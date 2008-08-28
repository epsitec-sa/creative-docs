//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>CoreState</c> class implements the common code needed by all state
	/// classes. A state class represents the state and the UI logic of an element
	/// of the application's user interface.
	/// </summary>
	public abstract class CoreState : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoreState"/> class.
		/// </summary>
		/// <param name="stateManager">The state manager.</param>
		protected CoreState(StateManager stateManager)
		{
			this.stateManager = stateManager;
		}


		/// <summary>
		/// Gets the state manager associated with this state.
		/// </summary>
		/// <value>The state manager.</value>
		public StateManager						StateManager
		{
			get
			{
				return this.stateManager;
			}
		}

		/// <summary>
		/// Gets the application object associated with this state.
		/// </summary>
		/// <value>The application object.</value>
		public CoreApplication					Application
		{
			get
			{
				return this.StateManager.Application;
			}
		}


		/// <summary>
		/// Gets or sets the state deck to which this state belongs.
		/// </summary>
		/// <value>The state deck.</value>
		public StateDeck						StateDeck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether this state is bound with a box,
		/// i.e. whether it is currently being displayed.
		/// </summary>
		/// <value><c>true</c> if this state is bound; otherwise, <c>false</c>.</value>
		public bool								IsBound
		{
			get
			{
				return this.stateManager.IsBound (this);
			}
		}

		/// <summary>
		/// Gets or sets the box id associated with this state.
		/// </summary>
		/// <value>The box id.</value>
		public int								BoxId
		{
			get
			{
				return this.boxId;
			}
			set
			{
				if (this.boxId != value)
				{
					int oldValue = this.boxId;
					int newValue = value;

					if (this.IsBound)
					{
						throw new System.InvalidOperationException ("Cannot change box id of a bound state");
					}
					
					this.boxId = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the title for this state (used by the state deck to
		/// identify the state).
		/// </summary>
		/// <value>The title.</value>
		public string							Title
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the linked state. The linked state is a logical parent in
		/// an edition sequence: when the user opens a new state to edit or create
		/// a specific field, the new state is linked with the previously active
		/// state.
		/// </summary>
		/// <value>The linked state.</value>
		public CoreState						LinkedState
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the field path of the linked state. See <see cref="LinkedState"/>;
		/// the field path specifies which field was active in the linked state, so
		/// that the system can update it when this state is closed.
		/// </summary>
		/// <value>The linked state field path.</value>
		public string							LinkedStateFieldPath
		{
			get;
			set;
		}


		/// <summary>
		/// Gets the root widget for the user interface, if any. It may or may not
		/// be currently visible (it might have no parent defined).
		/// </summary>
		/// <value>The root widget or <c>null</c>.</value>
		public AbstractGroup					RootWidget
		{
			get
			{
				return this.rootWidget;
			}
		}


		/// <summary>
		/// Gets or sets the path of the focused field.
		/// </summary>
		/// <value>The path of the focused field or <c>null</c>.</value>
		public string							FocusPath
		{
			get
			{
				return this.focusPath;
			}
			set
			{
				if (this.focusPath != value)
				{
					this.focusPath = value;

					//	TODO: generate event...
				}
			}
		}


		/// <summary>
		/// Serializes the state by creating child nodes into the &lt;state&gt;
		/// XML element.
		/// </summary>
		/// <param name="element">The &lt;state&gt; XML element.</param>
		/// <param name="context">The serialization context.</param>
		/// <returns>The populated &lt;state&gt; XML element.</returns>
		public virtual XElement Serialize(XElement element, StateSerializationContext context)
		{
			XElement workspaceElement = new XElement (Strings.XmlWorkspace);
			XElement coreElement      = new XElement (Strings.XmlCore);

			this.StoreCoreState (coreElement, context);
			this.StoreWorkspace (workspaceElement, context);

			element.Add (workspaceElement);

			return element;
		}

		/// <summary>
		/// Deserializes the state based on the specified &lt;state&gt; XML
		/// element.
		/// </summary>
		/// <param name="element">The &lt;state&gt; XML element.</param>
		/// <returns>The deserialized state (usually simply this state).</returns>
		public virtual CoreState Deserialize(XElement element)
		{
			this.RestoreWorkspace (element.Element (Strings.XmlWorkspace));
			this.RestoreCoreState (element.Element (Strings.XmlCore));

			return this;
		}

		/// <summary>
		/// Notifies the state that all states have been deserialized; this is
		/// the moment to execute any pending fix-ups.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		public virtual void NotifyDeserialized(StateSerializationContext context)
		{
			this.ExecuteFixups (context);
		}


		/// <summary>
		/// Paints a miniature representation of the state, to be used in the
		/// state deck, for instance..
		/// </summary>
		/// <param name="graphics">The graphics context.</param>
		/// <param name="frame">The frame.</param>
		public void PaintMiniature(Graphics graphics, Rectangle frame)
		{
			graphics.AddFilledRectangle (frame);
			graphics.RenderSolid (Color.FromRgb (1, 1, 1));

			if (!string.IsNullOrEmpty (this.Title))
			{
				graphics.AddText (5, 60, 90, 20, this.Title, Font.DefaultFont, 30.0, ContentAlignment.MiddleCenter);
			}

			frame.Deflate (0.5);
			
			graphics.LineWidth = 1;
			graphics.AddRectangle (frame);
			graphics.RenderSolid (Color.FromRgb (0, 0, 0.4));
		}


		/// <summary>
		/// Finds all states which match the specified filter.
		/// </summary>
		/// <typeparam name="T">The expected state type.</typeparam>
		/// <param name="manager">The state manager.</param>
		/// <param name="filter">The state filter.</param>
		/// <returns>The collection of all matching states.</returns>
		public static IEnumerable<T> FindAll<T>(StateManager manager, System.Predicate<T> filter) where T : CoreState
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



		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion


		/// <summary>
		/// Binds the state to the user interface. This method is used only by the
		/// <see cref="StateManager"/> when attaching a state.
		/// </summary>
		/// <param name="container">The container.</param>
		internal void Bind(Widget container)
		{
			//	Note: the box has already been bound to this state, this is why
			//	'IsBound' should be true in this context :

			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);

			this.AttachState (container);
		}

		/// <summary>
		/// Unbinds the state from the user interface. This method is used only
		/// by the <see cref="StateManager"/> when detaching a state.
		/// </summary>
		internal void Unbind()
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);

			this.DetachState ();
		}



		/// <summary>
		/// Creates the user interface.
		/// </summary>
		/// <returns>The group which contains the UI for this state.</returns>
		protected abstract AbstractGroup CreateUserInterface();

		/// <summary>
		/// Enables the workspace.
		/// </summary>
		protected abstract void EnableWorkspace();

		/// <summary>
		/// Disables the workspace.
		/// </summary>
		protected abstract void DisableWorkspace();


		/// <summary>
		/// Attaches the state to the user interface.
		/// </summary>
		/// <param name="container">The container.</param>
		protected virtual void AttachState(Widget container)
		{
			this.SetupUserInterface ();
			this.RootWidget.SetParent (container);
			this.SetWorkspaceEnable (true);
		}

		/// <summary>
		/// Detaches the state from the user interface.
		/// </summary>
		protected virtual void DetachState()
		{
			this.SetWorkspaceEnable (false);
			this.RootWidget.SetParent (null);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.stateManager.Pop (this);
				this.BoxId = 0;
			}
		}


		/// <summary>
		/// Stores the core information of the current state.
		/// </summary>
		/// <param name="coreElement">The core element.</param>
		/// <param name="context">The serialization context.</param>
		protected void StoreCoreState(XElement coreElement, StateSerializationContext context)
		{
			coreElement.Add (new XAttribute (Strings.XmlBoxId, this.BoxId));
			coreElement.Add (new XAttribute (Strings.XmlDeck, this.StateDeck.ToString ()));
			coreElement.Add (new XAttribute (Strings.XmlTitle, this.Title));

			if (this.LinkedState != null)
			{
				string link = context.GetTag (this.LinkedState);

				if (this.LinkedStateFieldPath != null)
				{
					link = string.Concat (link, " ", this.LinkedStateFieldPath);
				}

				coreElement.Add (new XAttribute (Strings.XmlLink, link));
			}
		}

		/// <summary>
		/// Restores the core information of the current state.
		/// </summary>
		/// <param name="coreElement">The core element.</param>
		protected void RestoreCoreState(XElement coreElement)
		{
			this.BoxId     = ((int)    coreElement.Attribute (Strings.XmlBoxId));
			this.StateDeck = ((string) coreElement.Attribute (Strings.XmlDeck)).ToEnum<StateDeck> (StateDeck.None);
			this.Title     = ((string) coreElement.Attribute (Strings.XmlTitle));
			string link    = ((string) coreElement.Attribute ("link"));

			if (!string.IsNullOrEmpty (link))
			{
				//	The link is expressed either as "tag" or "tag path", which have
				//	to be restored as LinkedState and LinkedFieldPath.

				int pos = link.IndexOf (' ');

				if (pos >= 0)
				{
					this.LinkedStateFieldPath = link.Substring (pos+1);
					link = link.Substring (0, pos);
				}

				this.RegisterFixup (context => this.LinkedState = context.GetState (link));
			}
		}


		/// <summary>
		/// Serializes the workspace by storing data into the specified
		/// &lt;workspace&gt; XML element.
		/// </summary>
		/// <param name="workspaceElement">The &lt;workspace&gt; XML element.</param>
		/// <param name="context">The serialization context.</param>
		protected abstract void StoreWorkspace(XElement workspaceElement, StateSerializationContext context);

		/// <summary>
		/// Deserializes the workspace based on the specified &lt;workspace&gt;
		/// XML element.
		/// </summary>
		/// <param name="workspaceElement">The &lt;workspace&gt; XML element.</param>
		protected abstract void RestoreWorkspace(XElement workspaceElement);



		/// <summary>
		/// Registers a fixup function which will be executed at the end of the
		/// state deserialization.
		/// </summary>
		/// <param name="action">The action.</param>
		protected void RegisterFixup(System.Action action)
		{
			this.RegisterFixup ((map) => action ());
		}

		/// <summary>
		/// Registers a fixup function which will be executed at the end of the
		/// state deserialization.
		/// </summary>
		/// <param name="action">The action.</param>
		protected void RegisterFixup(System.Action<StateSerializationContext> action)
		{
			if (this.fixups == null)
			{
				this.fixups = new List<System.Action<StateSerializationContext>> ();
			}
			
			this.fixups.Add (action);
		}

		/// <summary>
		/// Executes the fixups.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		private void ExecuteFixups(StateSerializationContext context)
		{
			if (this.fixups != null)
			{
				foreach (var action in this.fixups)
				{
					action (context);
				}

				this.fixups = null;
			}
		}


		/// <summary>
		/// Enable or disable the workspace user interface.
		/// </summary>
		/// <param name="enable">If set to <c>true</c>, enable the workspace user interface.</param>
		private void SetWorkspaceEnable(bool enable)
		{
			if (this.isWorkspaceEnabled != enable)
			{
				if (enable)
				{
					this.EnableWorkspace ();
					this.isWorkspaceEnabled = true;
				}
				else
				{
					this.DisableWorkspace ();
					this.isWorkspaceEnabled = false;
				}
			}
		}

		/// <summary>
		/// Sets the user interface up, if it does not exist yet.
		/// </summary>
		private void SetupUserInterface()
		{
			if (this.rootWidget == null)
			{
				CoreApplication application = this.Application;

				System.Diagnostics.Debug.Assert (application != null);
				System.Diagnostics.Debug.Assert (application.Window != null);

				this.rootWidget = this.CreateUserInterface ();
				this.rootWidget.Dock = DockStyle.Fill;
				this.rootWidget.Name = this.GetType ().Name;
			}
		}


		#region Private Strings

		/// <summary>
		/// The <c>Strings</c> class defines the constants used for XML serialization
		/// of the state.
		/// </summary>
		private static class Strings
		{
			public static readonly string XmlCore = "core";
			public static readonly string XmlBoxId = "boxId";
			public static readonly string XmlDeck = "deck";
			public static readonly string XmlTitle = "title";
			public static readonly string XmlLink = "link";
			public static readonly string XmlWorkspace = "workspace";
		}

		#endregion


		private readonly StateManager			stateManager;
		private int								boxId;
		
		private List<System.Action<StateSerializationContext>>	fixups;

		private bool							isWorkspaceEnabled;
		private AbstractGroup					rootWidget;
		private string							focusPath;
	}
}
