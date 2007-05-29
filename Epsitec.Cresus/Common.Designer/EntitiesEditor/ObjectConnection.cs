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


		public ObjectBox.Field Field
		{
			get
			{
				return this.field;
			}
			set
			{
				this.field = value;
			}
		}

		public List<Point> Points
		{
			get
			{
				return this.points;
			}
		}


		public override bool IsReadyForDragging
		{
			//	Est-ce que l'objet est dragable ?
			get
			{
				return false;
			}
		}

		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.hilitedElement == ActiveElement.Connection)
			{
				if (this.field.IsExplored)
				{
					this.field.IsExplored = false;
					this.field.DstBox = null;

					ObjectBox dst = this.editor.SearchParent(this.field);
					this.CloseBoxes(dst);

					this.editor.CreateConnections();
					this.editor.UpdateGeometry();
				}
				else
				{
					this.field.IsExplored = true;

					Module module = this.editor.Module.MainWindow.SearchModule(this.field.Destination);
					CultureMap item = module.AccessEntities.Accessor.Collection[this.field.Destination];
					if (item != null)
					{
						StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
						IList<StructuredData> fields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

						ObjectBox box = new ObjectBox(this.editor);
						box.ParentField = this.field;
						box.Title = item.Name;
						box.SetContent(fields);

						this.field.DstBox = box;
						this.editor.AddBox(box);
						this.editor.UpdateGeometry();

						ObjectBox src = this.editor.SearchSource(this.field);
						Rectangle bounds = box.Bounds;
						bounds.Location = new Point(src.Bounds.Right+50, src.Bounds.Top-box.Bounds.Height);
						box.Bounds = bounds;

						this.editor.CreateConnections();
						this.editor.UpdateAfterMoving(box);
					}
				}
			}
		}

		protected void CloseBoxes(ObjectBox box)
		{
			//	Ferme récursivement toutes les boîtes liées.
			foreach (ObjectBox.Field field in box.Fields)
			{
				if (field.Relation != FieldRelation.None)
				{
					if (field.DstBox != null)
					{
						this.CloseBoxes(field.DstBox);
					}
				}
			}

			this.editor.RemoveBox(box);
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || this.points.Count == 0 || !this.field.IsSourceExpanded)
			{
				return false;
			}

			//	Souris dans la pastille ronde du départ de la connection ?
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
				if (this.field.IsSourceExpanded)
				{
					start = Point.Move(start, this.points[1], ObjectConnection.circleRadius);
				}

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
			}

			if (this.points.Count != 0 && this.field.IsSourceExpanded)
			{
				//	Dessine le cercle au point de départ.
				Point start = this.points[0];
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
			//	Dessine une flèche selon le type de la relation.
			if (this.field.Relation == FieldRelation.Inclusion)
			{
				this.PaintArrowBase(graphics, end, start);
			}
		}

		protected void PaintEndingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type de la relation.
			this.PaintArrowBase(graphics, start, end);

			if (this.field.Relation == FieldRelation.Collection)
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

		protected ObjectBox.Field field;
		protected List<Point> points;
	}
}
