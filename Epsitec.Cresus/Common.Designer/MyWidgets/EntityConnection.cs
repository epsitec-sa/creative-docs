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
			this.points = new List<Point>();
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


		public List<Point> Points
		{
			get
			{
				return this.points;
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
			if (this.points.Count >= 2)
			{
				Point start = this.GetPoint(0);

				graphics.AddFilledCircle(start, EntityConnection.circleRadius);
				graphics.RenderSolid(Color.FromBrightness(1));

				graphics.AddCircle(start, EntityConnection.circleRadius);
				start = Point.Move(start, this.GetPoint(1), EntityConnection.circleRadius);

				for (int i=0; i<this.points.Count-1; i++)
				{
					Point p1 = (i==0) ? start : this.GetPoint(i);
					Point p2 = this.GetPoint(i+1);

					graphics.AddLine(p1, p2);

					if (i == this.points.Count-2)
					{
						this.PaintArrow(graphics, p1, p2);
					}
				}
				graphics.RenderSolid(Color.FromBrightness(0));
			}
		}

		protected Point GetPoint(int rank)
		{
			return this.MapParentToClient(this.points[rank]);
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

		protected List<Point> points;
		protected FieldRelation relation;
	}
}
