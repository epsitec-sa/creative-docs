using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Bo�te pour repr�senter un lien entre des entit�s.
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



		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est press�.
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est rel�ch�.
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || this.points.Count < 2)
			{
				return false;
			}

			//	Souris dans la pastille ronde du d�part de la connection ?
			double d = Point.Distance(pos, this.points[0]);
			if (d <= ObjectConnection.circleRadius+6)
			{
				element = ActiveElement.Connection;
				return true;
			}

			return false;
		}


		public override void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

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

				//	Dessine le cercle au point de d�part.
				start = this.points[0];
				if (this.hilitedElement == ActiveElement.Connection)
				{
					Color c = adorner.ColorCaption;
					c.A = 0.2;

					graphics.AddFilledCircle(start, ObjectConnection.circleRadius+4);
					graphics.RenderSolid(Color.FromBrightness(1));

					graphics.AddFilledCircle(start, ObjectConnection.circleRadius+4);
					graphics.RenderSolid(c);

					graphics.AddCircle(start, ObjectConnection.circleRadius+4);
					graphics.RenderSolid(Color.FromBrightness(0));
				}
				else
				{
					graphics.AddFilledCircle(start, ObjectConnection.circleRadius);
					graphics.RenderSolid(Color.FromBrightness(1));

					graphics.AddCircle(start, ObjectConnection.circleRadius);
					graphics.RenderSolid(Color.FromBrightness(0));
				}
			}
		}

		protected void PaintStartingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une fl�che selon le type de la relation.
			if (this.relation == FieldRelation.Inclusion)
			{
				this.PaintArrowBase(graphics, end, start);
			}
		}

		protected void PaintEndingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une fl�che selon le type de la relation.
			this.PaintArrowBase(graphics, start, end);

			if (this.relation == FieldRelation.Collection)
			{
				end = Point.Move(end, start, ObjectConnection.arrowLength*0.75);
				this.PaintArrowBase(graphics, start, end);
			}
		}

		protected void PaintArrowBase(Graphics graphics, Point start, Point end)
		{
			//	Dessine une fl�che � l'extr�mit� 'end'.
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
