//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.States
{
	public abstract class CoreState : System.IDisposable
	{
		protected CoreState(StateManager stateManager)
		{
			this.stateManager = stateManager;
		}

		
		public StateManager						StateManager
		{
			get
			{
				return this.stateManager;
			}
		}

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
		/// Serializes the state by creating child nodes into the &lt;state&gt;
		/// XML element.
		/// </summary>
		/// <param name="element">The &lt;state&gt; XML element.</param>
		/// <param name="context">The serialization context.</param>
		/// <returns>The populated &lt;state&gt; XML element.</returns>
		public abstract XElement Serialize(XElement element, StateSerializationContext context);

		/// <summary>
		/// Deserializes the state based on the specified &lt;state&gt; XML
		/// element.
		/// </summary>
		/// <param name="element">The &lt;state&gt; XML element.</param>
		/// <returns>The deserialized state (usually simply this state).</returns>
		public abstract CoreState Deserialize(XElement element);

		/// <summary>
		/// Notifies the state that all states have been deserialized; this is
		/// the moment to execute any pending fix-ups.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		public virtual void NotifyDeserialized(StateSerializationContext context)
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



		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		
		internal void Bind(Widget container)
		{
			//	Note: the box has already been bound to this state, this is why
			//	'IsBound' should be true in this context :

			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);

			this.AttachState (container);
		}

		internal void Unbind()
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);

			this.DetachState ();
		}

		
		protected abstract void AttachState(Widget container);

		protected abstract void DetachState();
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.stateManager.Pop (this);
				this.BoxId = 0;
			}
		}

		
		protected void StoreCoreState(XElement element)
		{
			element.Add (new XElement ("core",
				new XAttribute ("boxId", this.BoxId),
				new XAttribute ("deck", this.StateDeck.ToString ()),
				new XAttribute ("title", this.Title)));
		}

		protected void RestoreCoreState(XElement element)
		{
			XElement core = element.Element ("core");
			
			this.BoxId     = ((int)    core.Attribute ("boxId"));
			this.StateDeck = ((string) core.Attribute ("deck")).ToEnum<StateDeck> (StateDeck.None);
			this.Title     = ((string) core.Attribute ("title"));
		}

		protected void AddFixup(System.Action action)
		{
			this.AddFixup ((map) => action ());
		}

		protected void AddFixup(System.Action<StateSerializationContext> action)
		{
			if (this.fixups == null)
			{
				this.fixups = new List<System.Action<StateSerializationContext>> ();
			}
			
			this.fixups.Add (action);
		}

		
		private readonly StateManager			stateManager;
		private int								boxId;
		
		private List<System.Action<StateSerializationContext>>	fixups;
	}
}
