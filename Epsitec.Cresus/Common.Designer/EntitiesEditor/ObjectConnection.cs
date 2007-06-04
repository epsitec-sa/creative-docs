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
			this.middleRelative = 0.5;
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
			//	Retourne la liste des points. Si la connection est fermée, il s'agit des points
			//	droite et gauche. Aurement, il s'agit d'un nombre variable de points.
			get
			{
				return this.points;
			}
		}

		public bool IsSrcHilited
		{
			//	Indique si la boîte source est survolée par la souris.
			get
			{
				return this.isSrcHilited;
			}
			set
			{
				this.isSrcHilited = value;
			}
		}

		public bool IsAttachToLeft
		{
			//	Indique si la connection est attachée à gauche.
			get
			{
				return this.isAttachToLeft;
			}
			set
			{
				this.isAttachToLeft = value;
			}
		}

		public bool IsAttachToRight
		{
			//	Indique si la connection est attachée à droite.
			get
			{
				return this.isAttachToRight;
			}
			set
			{
				this.isAttachToRight = value;
			}
		}

		public bool IsMiddleRelative
		{
			//	Indique si la position intermédiaire est utilisée.
			get
			{
				return this.isMiddleRelative;
			}
			set
			{
				if (this.isMiddleRelative != value)
				{
					this.isMiddleRelative = value;

					this.UpdateMiddleRelative();
				}
			}
		}

		public double MiddleRelative
		{
			//	Position intermédiaire.
			get
			{
				return this.middleRelative;
			}
			set
			{
				value = System.Math.Max(value, 0.1);
				value = System.Math.Min(value, 0.9);

				if (this.middleRelative != value)
				{
					this.middleRelative = value;

					this.UpdateMiddleRelative();
					this.editor.Invalidate();
				}
			}
		}


		public override bool MouseMove(Point pos)
		{
			//	La souris est bougée.
			if (this.isDraggingMiddleRelative)
			{
				if (!pos.IsZero)
				{
					this.MiddleRelative = (pos.X-this.points[0].X)/(this.points[3].X-this.points[0].X);
				}
				return true;
			}
			else
			{
				return base.MouseMove(pos);
			}
		}

		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
			if (this.hilitedElement == ActiveElement.ConnectionMove)
			{
				this.isDraggingMiddleRelative = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingMiddleRelative)
			{
				this.isDraggingMiddleRelative = false;
				this.editor.LockObject(null);
			}

			if (this.hilitedElement == ActiveElement.ConnectionOpenLeft ||
				this.hilitedElement == ActiveElement.ConnectionOpenRight)
			{
				Module module = this.editor.Module.MainWindow.SearchModule(this.field.Destination);
				CultureMap item = module.AccessEntities.Accessor.Collection[this.field.Destination];
				if (item != null)
				{
					this.field.IsExplored = true;

					ObjectBox box = this.editor.SearchBox(item.Name);
					if (box == null)
					{
						//	Ouvre la connection sur une nouvelle boîte.
						box = new ObjectBox(this.editor);
						box.ParentField = this.field;
						box.Title = item.Name;
						box.SetContent(item);

						this.field.DstBox = box;
						this.editor.AddBox(box);
						this.editor.UpdateGeometry();

						ObjectBox src = this.field.SrcBox;
#if false
						Rectangle bounds = box.Bounds;
						double ox = 50+20*src.ConnectionExploredCount;
						if (this.hilitedElement == ActiveElement.ConnectionOpenLeft)
						{
							bounds.Location = new Point(src.Bounds.Left-ox-box.Bounds.Width, src.Bounds.Top-box.Bounds.Height);
						}
						else
						{
							bounds.Location = new Point(src.Bounds.Right+ox, src.Bounds.Top-box.Bounds.Height);
						}
#else
						//	Essaie de trouver une place libre, pour déplacer le moins possible d'éléments.
						Rectangle bounds;
						double posv = src.GetConnectionSrcVerticalPosition(this.field.Rank) + ObjectBox.headerHeight/2;

						if (this.hilitedElement == ActiveElement.ConnectionOpenLeft)
						{
							bounds = new Rectangle(src.Bounds.Left-50-box.Bounds.Width, posv-box.Bounds.Height, box.Bounds.Width, box.Bounds.Height);
							bounds.Inflate(50, Editor.pushMargin);

							for (int i=0; i<1000; i++)
							{
								if (this.editor.IsEmptyArea(bounds))
								{
									break;
								}
								bounds.Offset(-1, 0);
							}

							bounds.Deflate(50, Editor.pushMargin);
						}
						else
						{
							bounds = new Rectangle(src.Bounds.Right+50, posv-box.Bounds.Height, box.Bounds.Width, box.Bounds.Height);
							bounds.Inflate(50, Editor.pushMargin);

							for (int i=0; i<1000; i++)
							{
								if (this.editor.IsEmptyArea(bounds))
								{
									break;
								}
								bounds.Offset(1, 0);
							}

							bounds.Deflate(50, Editor.pushMargin);
						}
#endif
						box.Bounds = bounds;
					}
					else
					{
						//	Ouvre la connection sur une boîte existante.
						this.field.DstBox = box;
					}

					this.editor.CreateConnections();
					this.editor.UpdateAfterMoving(box);
				}
			}

			if (this.hilitedElement == ActiveElement.ConnectionClose)
			{
				ObjectBox dst = this.field.DstBox;
				this.field.IsExplored = false;
				this.field.DstBox = null;
				this.editor.CloseBox(null);

				this.editor.CreateConnections();
				this.editor.UpdateAfterMoving(null);
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
				this.SetDirty();
				this.editor.Invalidate();
				this.hilitedElement = ActiveElement.None;
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || this.points.Count == 0)
			{
				return false;
			}

			//	Souris dans la pastille ronde du départ de la connection ?
			if (this.field.IsSourceExpanded)
			{
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
						element = ActiveElement.ConnectionOpenRight;
						return true;
					}

					if (Point.Distance(pos, this.points[1]) <= AbstractObject.buttonRadius)
					{
						element = ActiveElement.ConnectionOpenLeft;
						return true;
					}
				}
			}

			//	Souris dans le bouton pour déplacer le point milieu ?
			if (this.isMiddleRelative)
			{
				Point m = Point.Scale(this.points[1], this.points[2], 0.5);
				if (Point.Distance(pos, m) <= AbstractObject.buttonRadius)
				{
					element = ActiveElement.ConnectionMove;
					return true;
				}
			}

			//	Souris dans le bouton pour changer la connection ?
			Point p = this.PositionChange;
			if (!p.IsZero && (this.field.IsExplored || this.field.IsSourceExpanded) && Point.Distance(pos, p) <= AbstractObject.buttonRadius)
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


		protected void UpdateMiddleRelative()
		{
			//	Met à jour les points milieu de la connection.
			if (this.isMiddleRelative)
			{
				double px = this.points[0].X + (this.points[3].X-this.points[0].X)*this.middleRelative;
				this.points[1] = new Point(px, this.points[0].Y);
				this.points[2] = new Point(px, this.points[3].Y);
			}
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

			if (this.points.Count == 2 && !this.field.IsExplored && this.field.IsSourceExpanded)
			{
				Point start = this.points[0];
				Point end = new Point(start.X+ObjectConnection.lengthClose, start.Y);

				graphics.LineWidth = 2;
				graphics.AddLine(start, end);
				this.DrawEndingArrow(graphics, start, end);
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
						hilite = (i == 1);
						shape = GlyphShape.ArrowLeft;
					}
					else if (this.hilitedElement == ActiveElement.ConnectionOpenRight)
					{
						hilite = (i == 0);
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
						if (!this.isSrcHilited && i != 0)  break;
					}

					if (hilite)
					{
						this.DrawRoundButton(graphics, start, AbstractObject.buttonRadius, shape, true, false);
					}
					else
					{
						if (this.hilitedElement == ActiveElement.ConnectionHilited)
						{
							this.DrawRoundButton(graphics, start, AbstractObject.buttonRadius, GlyphShape.Close, false, false);
						}
						else
						{
							this.DrawRoundButton(graphics, start, AbstractObject.bulletRadius, GlyphShape.None, false, false);
						}
					}
				}
			}

			//	Dessine le bouton pour déplacer le point milieu.
			if (this.isMiddleRelative)
			{
				Point m = Point.Scale(this.points[1], this.points[2], 0.5);
				if (this.hilitedElement == ActiveElement.ConnectionMove)
				{
					this.DrawRoundButton(graphics, m, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.ConnectionHilited)
				{
					this.DrawRoundButton(graphics, m, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}
			}

			if (this.points.Count != 0)
			{
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
							this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, GlyphShape.Dots, false, false);
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

				if (this.points.Count == 2 && !this.field.IsExplored && this.field.IsSourceExpanded)
				{
					return new Point(this.points[0].X+ObjectConnection.lengthClose, this.points[0].Y);
				}

				return Point.Zero;
			}
		}



		protected static readonly double arrowLength = 12;
		protected static readonly double arrowAngle = 25;
		protected static readonly double lengthClose = 30;
		protected static readonly double pushMargin = 10;

		protected ObjectBox.Field field;
		protected List<Point> points;
		protected bool isSrcHilited;
		protected bool isAttachToLeft;
		protected bool isAttachToRight;
		protected bool isMiddleRelative;
		protected double middleRelative;
		protected bool isDraggingMiddleRelative;
	}
}
