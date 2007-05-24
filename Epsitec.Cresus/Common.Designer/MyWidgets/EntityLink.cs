using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Boîte pour représenter un lien entre des entités.
	/// </summary>
	public class EntityLink : Widget
	{
		public EntityLink() : base()
		{
			this.source = Point.Zero;
			this.destination = Point.Zero;
		}

		public EntityLink(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public Point Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					this.source = value;
					this.Invalidate();
				}
			}
		}

		public Point Destination
		{
			get
			{
				return this.destination;
			}
			set
			{
				if (this.destination != value)
				{
					this.destination = value;
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (!this.source.IsZero && !this.destination.IsZero)
			{
				Point p1 = this.MapParentToClient(this.source);
				Point p2 = this.MapParentToClient(this.destination);
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(Color.FromBrightness(0));
			}
		}


		protected Point source;
		protected Point destination;
	}
}
