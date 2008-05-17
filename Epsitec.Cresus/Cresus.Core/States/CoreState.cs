//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
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

		
		public StateManager StateManager
		{
			get
			{
				return this.stateManager;
			}
		}

		public StateDeck StateDeck
		{
			get;
			set;
		}

		public int ZOrder
		{
			get
			{
				return this.zOrder;
			}
			internal set
			{
				this.zOrder = value;
			}
		}

		public int BoxId
		{
			get
			{
				return this.boxId;
			}
			internal set
			{
				if (this.boxId != value)
				{
					if (this.boxId != 0)
					{
						this.DetachState ();
						this.boxId = 0;
					}

					int oldBoxId = this.boxId;
					int newBoxId = value;

					this.boxId = value;

					if (this.boxId != 0)
					{
						this.AttachState ();
					}
				}
			}
		}

		public string Title
		{
			get;
			set;
		}

		protected Widget Container
		{
			get
			{
				return this.container;
			}
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

		internal void SoftDetach()
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			
			this.SoftDetachState ();
		}

		protected virtual void SoftAttachState()
		{
		}

		protected virtual void SoftDetachState()
		{
		}
		
		protected void AttachState()
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			this.container = this.stateManager.Attach (this);
			this.SoftAttachState ();
		}

		protected void DetachState()
		{
			System.Diagnostics.Debug.Assert (this.boxId != 0);
			
			this.SoftDetachState ();
			this.stateManager.Detach (this);
			this.container = null;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.boxId != 0)
				{
					this.DetachState ();
					this.boxId = 0;
				}
			}
		}

		
		protected void StoreCoreState(XElement element)
		{
			element.Add (new XElement ("core",
				new XAttribute ("boxId", this.BoxId)));
		}

		protected void RestoreCoreState(XElement element)
		{
			XElement core = element.Element ("core");
			
			this.BoxId = (int) core.Attribute ("boxId");
		}

		
		private readonly StateManager			stateManager;
		private Widget							container;
		private int								zOrder;
		private int								boxId;
	}
}
