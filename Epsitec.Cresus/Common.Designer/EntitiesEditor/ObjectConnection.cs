using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Boîte pour représenter un lien entre des entités.
	/// </summary>
	public class ObjectConnection : AbstractObject
	{
		public ObjectConnection(Editor editor) : base(editor)
		{
			this.points = new List<Point>();
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
				}
			}
		}



		public override void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
			if (this.points.Count >= 2)
			{
				Point start = this.points[0];
				start = Point.Move(start, this.points[1], ObjectConnection.circleRadius);

				graphics.LineWidth = 2;
				for (int i=0; i<this.points.Count-1; i++)
				{
					Point p1 = (i==0) ? start : this.points[i];
					Point p2 = this.points[i+1];

					if (i == 0)
					{
						this.PaintStartingArrow(graphics, p1, p2);
					}

					graphics.AddLine(p1, p2);

					if (i == this.points.Count-2)
					{
						this.PaintEndingArrow(graphics, p1, p2);
					}
				}
				graphics.LineWidth = 1;
				graphics.RenderSolid(Color.FromBrightness(0));

				//	Dessine le cercle au point de départ.
				start = this.points[0];
				graphics.AddFilledCircle(start, ObjectConnection.circleRadius);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddCircle(start, ObjectConnection.circleRadius);
				graphics.RenderSolid(Color.FromBrightness(0));
			}
		}

		protected void PaintStartingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type de la relation.
			if (this.relation == FieldRelation.Inclusion)
			{
				this.PaintArrowBase(graphics, end, start);
			}
		}

		protected void PaintEndingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type de la relation.
			this.PaintArrowBase(graphics, start, end);

			if (this.relation == FieldRelation.Collection)
			{
				end = Point.Move(end, start, ObjectConnection.arrowLength*0.75);
				this.PaintArrowBase(graphics, start, end);
			}
		}

		protected void PaintArrowBase(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche à l'extrémité 'end'.
			Point p = Point.Move(end, start, ObjectConnection.arrowLength);

			Point e1 = Transform.RotatePointDeg(end, ObjectConnection.arrowAngle, p);
			Point e2 = Transform.RotatePointDeg(end, -ObjectConnection.arrowAngle, p);

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
