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
						throw new System.InvalidOperationException ();
					}
					
					this.boxId = value;

					//	TODO: notify about the box ID change
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

		
		public abstract XElement Serialize(StateManagerSerializationContext context, XElement element);

		public abstract CoreState Deserialize(XElement element);

		public virtual void NotifyDeserialized(StateManagerSerializationContext context)
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

		internal void Unbind()
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);
			
			this.SoftDetachState ();
		}

		internal void Bind(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);

			this.SoftAttachState (container);
		}

		protected abstract void SoftAttachState(Widget container);

		protected abstract void SoftDetachState();
		
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

		protected void AddFixup(System.Action<StateManagerSerializationContext> action)
		{
			if (this.fixups == null)
			{
				this.fixups = new List<System.Action<StateManagerSerializationContext>> ();
			}
			
			this.fixups.Add (action);
		}

		
		private readonly StateManager			stateManager;
		private int								boxId;
		
		private List<System.Action<StateManagerSerializationContext>>	fixups;
	}
}
