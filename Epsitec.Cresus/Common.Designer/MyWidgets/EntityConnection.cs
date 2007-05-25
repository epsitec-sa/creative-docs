using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Boîte pour représenter un lien entre des entités.
	/// </summary>
	public class EntityConnection : Widget
	{
		public EntityConnection() : base()
		{
			this.source = Point.Zero;
			this.destination = Point.Zero;
		}

		public EntityConnection(Widget embedder) : this()
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

		public FieldRelation Relation
		{
			get
			{
				return this.relation;
			}
			set
			{
				if (this.relation != value)
				{
					this.relation = value;
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

				graphics.AddFilledCircle(p1, EntityConnection.circleRadius);
				graphics.RenderSolid(Color.FromBrightness(1));

				graphics.AddCircle(p1, EntityConnection.circleRadius);
				p1 = Point.Move(p1, p2, EntityConnection.circleRadius);

				graphics.AddLine(p1, p2);
				this.PaintArrow(graphics, p1, p2);
				graphics.RenderSolid(Color.FromBrightness(0));
			}
		}

		protected void PaintArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type de la relation.
			this.PaintArrowBase(graphics, start, end);

			if (this.relation == FieldRelation.Collection)
			{
				end = Point.Move(end, start, EntityConnection.arrowLength*0.75);
				this.PaintArrowBase(graphics, start, end);
			}

			if (this.relation == FieldRelation.Inclusion)
			{
				this.PaintArrowBase(graphics, end, start);
			}
		}

		protected void PaintArrowBase(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche à l'extrémité 'end'.
			Point p = Point.Move(end, start, EntityConnection.arrowLength);

			Point e1 = Transform.RotatePointDeg(end, EntityConnection.arrowAngle, p);
			Point e2 = Transform.RotatePointDeg(end, -EntityConnection.arrowAngle, p);

			graphics.AddLine(end, e1);
			graphics.AddLine(end, e2);
		}



		protected static readonly double circleRadius = 4;
		protected static readonly double arrowLength = 12;
		protected static readonly double arrowAngle = 25;

		protected Point source;
		protected Point destination;
		protected FieldRelation relation;
	}
}
