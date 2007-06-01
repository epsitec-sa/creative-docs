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


		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.hilitedElement == ActiveElement.ConnectionOpenLeft ||
				this.hilitedElement == ActiveElement.ConnectionOpenRight)
			{
				Module module = this.editor.Module.MainWindow.SearchModule(this.field.Destination);
				CultureMap item = module.AccessEntities.Accessor.Collection[this.field.Destination];
				if (item != null)
				{
					this.field.IsExplored = true;

					ObjectBox box = new ObjectBox(this.editor);
					box.ParentField = this.field;
					box.Title = item.Name;
					box.SetContent(item);

					this.field.DstBox = box;
					this.editor.AddBox(box);
					this.editor.UpdateGeometry();

					ObjectBox src = this.field.SrcBox;
					Rectangle bounds = box.Bounds;
					if (this.hilitedElement == ActiveElement.ConnectionOpenLeft)
					{
						bounds.Location = new Point(src.Bounds.Left-50-box.Bounds.Width, src.Bounds.Top-box.Bounds.Height);
					}
					else
					{
						bounds.Location = new Point(src.Bounds.Right+50, src.Bounds.Top-box.Bounds.Height);
					}
					box.Bounds = bounds;

					this.editor.CreateConnections();
					this.editor.UpdateAfterMoving(box);
				}
			}

			if (this.hilitedElement == ActiveElement.ConnectionClose)
			{
				ObjectBox dst = this.field.DstBox;
				this.field.IsExplored = false;
				this.field.DstBox = null;
				this.CloseBoxes(dst);

				this.editor.CreateConnections();
				this.editor.UpdateGeometry();
			}

			if (this.hilitedElement == ActiveElement.ConnectionChange)
			{
				ObjectBox box = this.field.SrcBox;

				StructuredData data = box.CultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

				StructuredData dataField = dataFields[this.field.Rank];
				FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
				if (rel == FieldRelation.Reference)
				{
					rel = FieldRelation.Collection;
					dataField.SetValue(Support.Res.Fields.Field.Relation, rel);
				}
				else if (rel == FieldRelation.Collection)
				{
					rel = FieldRelation.Reference;
					dataField.SetValue(Support.Res.Fields.Field.Relation, rel);
				}

				this.field.Relation = rel;
				this.editor.Invalidate();
				this.hilitedElement = ActiveElement.None;
			}
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
			if (this.field.IsExplored)
			{
				if (Point.Distance(pos, this.points[0]) <= AbstractObject.buttonRadius)
				{
					element = ActiveElement.ConnectionClose;
					return true;
				}
			}
			else
			{
				if (Point.Distance(pos, this.points[0]) <= AbstractObject.buttonRadius)
				{
					element = ActiveElement.ConnectionOpenLeft;
					return true;
				}

				if (Point.Distance(pos, this.points[1]) <= AbstractObject.buttonRadius)
				{
					element = ActiveElement.ConnectionOpenRight;
					return true;
				}
			}

			//	Souris dans le bouton pour changer la connection ?
			Point p = this.PositionChange;
			if (!p.IsZero && Point.Distance(pos, p) <= AbstractObject.buttonRadius)
			{
				element = ActiveElement.ConnectionChange;
				return true;
			}

			//	Souris le long de la connection ?
			if (DetectOver(pos, 4))
			{
				element = ActiveElement.ConnectionHilited;
				return true;
			}

			return false;
		}

		protected bool DetectOver(Point pos, double margin)
		{
			//	Détecte si la souris est le long de la connection.
			if (this.points.Count >= 2 && this.field.IsExplored)
			{
				for (int i=0; i<this.points.Count-1; i++)
				{
					Point p1 = this.points[i];
					Point p2 = this.points[i+1];

					if (Point.Distance(p1, pos) <= margin ||
						Point.Distance(p2, pos) <= margin)
					{
						return true;
					}

					Point p = Point.Projection(p1, p2, pos);
					if (Point.Distance(p, pos) <= margin && Geometry.IsInside(p1, p2, p))
					{
						return true;
					}
				}
			}

			return false;
		}


		public override void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			if (this.points.Count >= 2 && this.field.IsExplored)
			{
				Point start = this.points[0];
				if (this.field.IsSourceExpanded)
				{
					start = Point.Move(start, this.points[1], AbstractObject.bulletRadius);
				}

				graphics.LineWidth = 2;
				for (int i=0; i<this.points.Count-1; i++)
				{
					Point p1 = (i==0) ? start : this.points[i];
					Point p2 = this.points[i+1];

					if (i == 0)
					{
						this.DrawStartingArrow(graphics, p1, p2);
					}

					graphics.AddLine(p1, p2);

					if (i == this.points.Count-2)
					{
						this.DrawEndingArrow(graphics, p1, p2);
					}
				}
				graphics.LineWidth = 1;

				Color color = Color.FromBrightness(0);
				if (this.hilitedElement == ActiveElement.ConnectionHilited ||
					this.hilitedElement == ActiveElement.ConnectionChange )
				{
					color = this.GetColorCaption();
				}
				graphics.RenderSolid(color);
			}

			if (this.points.Count != 0 && this.field.IsSourceExpanded)
			{
				//	Dessine les cercles aux points de départ.
				for (int i=0; i<this.points.Count; i++)
				{
					Point start = this.points[i];
					GlyphShape shape = GlyphShape.None;

					bool hilite = false;
					if (this.hilitedElement == ActiveElement.ConnectionOpenLeft)
					{
						hilite = (i == 0);
						shape = GlyphShape.ArrowLeft;
					}
					else if (this.hilitedElement == ActiveElement.ConnectionOpenRight)
					{
						hilite = (i == 1);
						shape = GlyphShape.ArrowRight;
					}
					else if (this.hilitedElement == ActiveElement.ConnectionClose)
					{
						if (i != 0)  break;
						hilite = true;
						shape = GlyphShape.Close;
					}
					else
					{
						if (this.field.IsExplored && i != 0)  break;
					}

					if (hilite)
					{
						this.DrawRoundButton(graphics, start, AbstractObject.buttonRadius, shape, true, false);
					}
					else
					{
						this.DrawRoundButton(graphics, start, AbstractObject.bulletRadius, GlyphShape.None, false, false);
					}
				}

				//	Dessine le bouton pour changer la connection.
				if (this.hilitedElement == ActiveElement.ConnectionHilited ||
					this.hilitedElement == ActiveElement.ConnectionChange)
				{
					Point p = this.PositionChange;
					if (!p.IsZero)
					{
						if (this.hilitedElement == ActiveElement.ConnectionChange)
						{
							this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, GlyphShape.Dots, true, false);
						}
						else
						{
							this.DrawRoundButton(graphics, p, AbstractObject.bulletRadius, GlyphShape.None, false, false);
						}
					}
				}
			}
		}

		protected void DrawStartingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type de la relation.
			if (this.field.Relation == FieldRelation.Inclusion)
			{
				this.DrawArrowBase(graphics, end, start);
			}
		}

		protected void DrawEndingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type de la relation.
			this.DrawArrowBase(graphics, start, end);

			if (this.field.Relation == FieldRelation.Collection)
			{
				end = Point.Move(end, start, ObjectConnection.arrowLength*0.75);
				this.DrawArrowBase(graphics, start, end);
			}
		}

		protected void DrawArrowBase(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche à l'extrémité 'end'.
			Point p = Point.Move(end, start, ObjectConnection.arrowLength);

			Point e1 = Transform.RotatePointDeg(end, ObjectConnection.arrowAngle, p);
			Point e2 = Transform.RotatePointDeg(end, -ObjectConnection.arrowAngle, p);

			graphics.AddLine(end, e1);
			graphics.AddLine(end, e2);
		}

		protected Point PositionChange
		{
			//	Retourne la position du bouton pour changer le type de la relation.
			get
			{
				if (this.points.Count >= 2 && this.field.IsExplored)
				{
					return this.points[this.points.Count-1];
				}

				return Point.Zero;
			}
		}



		protected static readonly double arrowLength = 12;
		protected static readonly double arrowAngle = 25;

		protected ObjectBox.Field field;
		protected List<Point> points;
	}
}
