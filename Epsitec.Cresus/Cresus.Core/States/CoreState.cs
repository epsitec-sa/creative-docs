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

		public string							Title
		{
			get;
			set;
		}

		protected Widget						Container
		{
			get
			{
				return this.container;
			}
		}

		internal string							Tag
		{
			get;
			set;
		}

		public abstract XElement Serialize(XElement element);

		public abstract CoreState Deserialize(XElement element);

		
		public void PaintMiniature(Graphics graphics)
		{
			graphics.AddFilledRectangle (0, 0, 100, 100);
			graphics.RenderSolid (Color.FromAlphaRgb (0.9, 1, 1, 1));

			if (!string.IsNullOrEmpty (this.Title))
			{
				graphics.AddText (5, 60, 90, 20, this.Title, Font.DefaultFont, 30.0, ContentAlignment.MiddleCenter);
			}
			
			graphics.LineWidth = 0.5;
			graphics.AddRectangle (0.25, 0.25, 99.75, 99.75);
			graphics.RenderSolid (Color.FromRgb (0, 0, 0.4));
		}



		#region IDisposable Members

		public void Dispose()
		{
			throw new System.NotImplementedException ();
		}

		#endregion

		internal void Unbind()
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);
			
			this.SoftDetachState ();
			this.container = null;
		}

		internal void Bind(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			System.Diagnostics.Debug.Assert (this.IsBound);

			this.container = container;
			this.SoftAttachState ();
		}

		protected abstract void SoftAttachState();

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

		
		private readonly StateManager			stateManager;
		private Widget							container;
		private int								boxId;
	}
}
