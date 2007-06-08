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


		public Field Field
		{
			//	Champ de référence pour la connection.
			get
			{
				return this.field;
			}
			set
			{
				this.field = value;
			}
		}

		public ObjectComment Comment
		{
			//	Commentaire lié.
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
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


		public override Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			get
			{
				Rectangle bounds = Rectangle.Empty;

				if (this.field.IsSourceExpanded)
				{
					foreach (Point p in this.points)
					{
						bounds = Rectangle.Union(bounds, new Rectangle(p, Size.Zero));
					}
				}

				return bounds;
			}
		}


		public override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			switch (element)
			{
				case AbstractObject.ActiveElement.ConnectionComment:
					if (this.comment == null)
					{
						return "Montre le commentaire associé";
					}
					else if (!this.comment.IsVisible)
					{
						return string.Format("Montre le commentaire associé<br/><b>{0}</b>", this.comment.Text);
					}
					else
					{
						return "Cache le commentaire associé";
					}
			}

			return base.GetToolTipText(element);
		}

		public override bool MouseMove(Point pos)
		{
			//	La souris est bougée.
			if (this.isDraggingRoute)
			{
				this.RouteMove(pos);
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
			if (this.hilitedElement == ActiveElement.ConnectionMove1 ||
				this.hilitedElement == ActiveElement.ConnectionMove2)
			{
				this.isDraggingRoute = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingRoute)
			{
				this.isDraggingRoute = false;
				this.editor.UpdateAfterGeometryChanged(null);
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
						this.field.IsAttachToRight = (this.hilitedElement == ActiveElement.ConnectionOpenRight);

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
						double posv = src.GetConnectionSrcVerticalPosition(this.field.Rank) + AbstractObject.headerHeight/2;

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
						box.SetBounds(bounds);
					}
					else
					{
						//	Ouvre la connection sur une boîte existante.
						this.field.DstBox = box;
						this.field.IsAttachToRight = (this.hilitedElement == ActiveElement.ConnectionOpenRight);
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

			if (this.hilitedElement == ActiveElement.ConnectionComment)
			{
				this.AddComment();
			}

			if (this.hilitedElement == ActiveElement.ConnectionChangeRelation)
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
					if (this.DetectRoundButton(pos, this.points[0]))
					{
						element = ActiveElement.ConnectionClose;
						return true;
					}
				}
				else
				{
					if (this.DetectRoundButton(pos, this.points[0]))
					{
						element = ActiveElement.ConnectionOpenRight;
						return true;
					}

					if (this.DetectRoundButton(pos, this.points[1]))
					{
						element = ActiveElement.ConnectionOpenLeft;
						return true;
					}
				}
			}

			//	Souris dans le bouton pour commenter la connection.
			Point p = this.PositionConnectionComment;
			if (!p.IsZero && this.DetectRoundButton(pos, p))
			{
				element = ActiveElement.ConnectionComment;
				return true;
			}

			//	Souris dans le bouton pour déplacer le point milieu ?
			Point m = this.PositionRouteMove1;
			if (!m.IsZero && this.DetectRoundButton(pos, m))
			{
				element = ActiveElement.ConnectionMove1;
				return true;
			}

			m = this.PositionRouteMove2;
			if (!m.IsZero && this.DetectRoundButton(pos, m))
			{
				element = ActiveElement.ConnectionMove2;
				return true;
			}

			//	Souris dans le bouton pour changer la connection ?
			p = this.PositionChangeRelation;
			if (!p.IsZero && (this.field.IsExplored || this.field.IsSourceExpanded) && this.DetectRoundButton(pos, p))
			{
				element = ActiveElement.ConnectionChangeRelation;
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


		protected void AddComment()
		{
			//	Ajoute un commentaire à la connection.
			if (this.comment == null)
			{
				this.comment = new ObjectComment(this.editor);
				this.comment.AttachObject = this;

				Point attach = this.PositionConnectionComment;
				Rectangle rect = new Rectangle(attach.X, attach.Y+20, Editor.defaultWidth, 50);  // hauteur arbitraire
				this.comment.SetBounds(rect);
				this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment(this.comment);
				this.editor.UpdateAfterCommentChanged();
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}
		}

		
		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine l'objet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			if (this.points.Count >= 2 && this.field.IsExplored && (this.field.Route != Field.RouteType.Himself || this.field.IsSourceExpanded))
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
						this.DrawStartingArrow(graphics, p1, p2, this.field.Relation);
					}

					graphics.AddLine(p1, p2);

					if (i == this.points.Count-2)
					{
						this.DrawEndingArrow(graphics, p1, p2, this.field.Relation);
					}
				}
				graphics.LineWidth = 1;

				Color color = Color.FromBrightness(0);
				if (this.hilitedElement == ActiveElement.ConnectionHilited ||
					this.hilitedElement == ActiveElement.ConnectionChangeRelation )
				{
					color = this.GetColorCaption();
				}
				graphics.RenderSolid(color);
			}

			if (this.points.Count == 2 && !this.field.IsExplored && this.field.IsSourceExpanded)
			{
				//	Dessine le moignon de liaison.
				Point start = this.points[0];
				Point end = new Point(start.X+AbstractObject.lengthClose, start.Y);

				graphics.LineWidth = 2;
				graphics.AddLine(start, end);
				this.DrawEndingArrow(graphics, start, end, this.field.Relation);
				graphics.LineWidth = 1;

				Color color = Color.FromBrightness(0);
				if (this.hilitedElement == ActiveElement.ConnectionHilited ||
					this.hilitedElement == ActiveElement.ConnectionChangeRelation )
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

			//	Dessine le bouton pour commenter la connection.
			Point p = this.PositionConnectionComment;
			if (!p.IsZero)
			{
				if (this.hilitedElement == ActiveElement.ConnectionComment)
				{
					this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, "C", true, false);
				}
				if (this.hilitedElement == ActiveElement.ConnectionHilited)
				{
					this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, "C", false, false);
				}
			}

			//	Dessine le bouton pour déplacer le point milieu.
			Point m = this.PositionRouteMove1;
			if (!m.IsZero)
			{
				if (this.hilitedElement == ActiveElement.ConnectionMove1)
				{
					this.DrawRoundButton(graphics, m, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.ConnectionHilited)
				{
					this.DrawRoundButton(graphics, m, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}
			}

			m = this.PositionRouteMove2;
			if (!m.IsZero)
			{
				if (this.hilitedElement == ActiveElement.ConnectionMove2)
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
					this.hilitedElement == ActiveElement.ConnectionChangeRelation)
				{
					p = this.PositionChangeRelation;
					if (!p.IsZero)
					{
						if (this.hilitedElement == ActiveElement.ConnectionChangeRelation)
						{
							this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, "R", true, false);
						}
						else
						{
							this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, "R", false, false);
						}
					}
				}
			}
		}


		public Point PositionConnectionComment
		{
			//	Retourne la position du bouton pour commenter la connection.
			get
			{
				if (this.field.IsSourceExpanded && this.field.IsExplored && this.points.Count >= 2)
				{
					return Point.Move(this.points[0], this.points[1], AbstractObject.buttonRadius*2);
				}

				return Point.Zero;
			}
		}

		protected Point PositionChangeRelation
		{
			//	Retourne la position du bouton pour changer le type de la relation (-> ou ->>).
			get
			{
				if (this.points.Count >= 2 && this.field.IsExplored)
				{
					return this.points[this.points.Count-1];
				}

				if (this.points.Count == 2 && !this.field.IsExplored && this.field.IsSourceExpanded)
				{
					return new Point(this.points[0].X+AbstractObject.lengthClose, this.points[0].Y);
				}

				return Point.Zero;
			}
		}

		protected Point PositionRouteMove1
		{
			//	Retourne la position du bouton pour modifier le routage.
			get
			{
				if (this.field.Route == Field.RouteType.A)
				{
					if (this.points.Count == 6)
					{
						return this.points[2];
					}
					else
					{
						if (Point.Distance(this.points[0], this.points[1]) >= 75)
						{
							return Point.Scale(this.points[0], this.points[1], this.field.RouteRelativeAX1);
						}
					}
				}

				if (this.field.Route == Field.RouteType.Bt || this.field.Route == Field.RouteType.Bb)
				{
					if (this.points.Count == 5)
					{
						return this.points[2];
					}
					else
					{
						if (Point.Distance(this.points[0], this.points[1]) >= 50 && Point.Distance(this.points[1], this.points[2]) >= 50)
						{
							return this.points[1];
						}
					}
				}

				if (this.field.Route == Field.RouteType.C)
				{
					if (Point.Distance(this.points[0], this.points[3]) >= 75)
					{
						return this.points[1];
					}
				}

				if (this.field.Route == Field.RouteType.D)
				{
					return this.points[1];
				}

				return Point.Zero;
			}
		}

		protected Point PositionRouteMove2
		{
			//	Retourne la position du bouton pour modifier le routage.
			get
			{
				if (this.field.Route == Field.RouteType.A)
				{
					if (this.points.Count == 6)
					{
						return this.points[3];
					}
					else
					{
						if (Point.Distance(this.points[0], this.points[1]) >= 75)
						{
							return Point.Scale(this.points[0], this.points[1], this.field.RouteRelativeAX2);
						}
					}
				}

				return Point.Zero;
			}
		}

		protected void RouteMove(Point pos)
		{
			//	Modifie le routage en fonction du choix de l'utilisateur.
			if (pos.IsZero)
			{
				return;
			}

			if (this.field.Route == Field.RouteType.A)
			{
				if (this.hilitedElement == ActiveElement.ConnectionMove1)
				{
					this.field.RouteRelativeAX1 = (pos.X-this.points[0].X)/(this.points[this.points.Count-1].X-this.points[0].X);
				}
				else
				{
					this.field.RouteRelativeAX2 = (pos.X-this.points[0].X)/(this.points[this.points.Count-1].X-this.points[0].X);
				}

				this.field.RouteAbsoluteAY = pos.Y-this.points[0].Y;
			}

			if (this.field.Route == Field.RouteType.Bt || this.field.Route == Field.RouteType.Bb)
			{
				this.field.RouteRelativeBX = (pos.X-this.points[this.points.Count-1].X)/(this.points[0].X-this.points[this.points.Count-1].X);
				this.field.RouteRelativeBY = (pos.Y-this.points[0].Y)/(this.points[this.points.Count-1].Y-this.points[0].Y);
			}

			if (this.field.Route == Field.RouteType.C)
			{
				this.field.RouteRelativeCX = (pos.X-this.points[0].X)/(this.points[3].X-this.points[0].X);
			}

			if (this.field.Route == Field.RouteType.D)
			{
				if (this.field.IsAttachToRight)
				{
					double px = System.Math.Max(this.points[0].X, this.points[3].X) + Editor.connectionDetour;
					this.field.RouteAbsoluteDX = pos.X-px;
				}
				else
				{
					double px = System.Math.Min(this.points[0].X, this.points[3].X) - Editor.connectionDetour;
					this.field.RouteAbsoluteDX = px-pos.X;
				}
			}
		}

		public void UpdateRoute()
		{
			//	Met à jour le routage de la connection, dans les cas ou le routage dépend des choix de l'utilisateur.
			if (this.field.Route == Field.RouteType.A)
			{
				if (this.field.RouteAbsoluteAY == 0)
				{
					if (this.points.Count == 6)
					{
						this.points.RemoveAt(1);
						this.points.RemoveAt(1);
						this.points.RemoveAt(1);
						this.points.RemoveAt(1);
					}
				}
				else
				{
					if (this.points.Count == 2)
					{
						this.points.Insert(1, Point.Zero);
						this.points.Insert(1, Point.Zero);
						this.points.Insert(1, Point.Zero);
						this.points.Insert(1, Point.Zero);
					}

					double px1 = this.points[0].X + (this.points[5].X-this.points[0].X)*this.field.RouteRelativeAX1;
					double px2 = this.points[0].X + (this.points[5].X-this.points[0].X)*this.field.RouteRelativeAX2;
					double py = this.points[0].Y + this.field.RouteAbsoluteAY;
					this.points[1] = new Point(px1, this.points[0].Y);
					this.points[2] = new Point(px1, py);
					this.points[3] = new Point(px2, py);
					this.points[4] = new Point(px2, this.points[0].Y);
				}
			}

			if (this.field.Route == Field.RouteType.Bt || this.field.Route == Field.RouteType.Bb)
			{
				if (this.field.RouteRelativeBX == 0 || this.field.RouteRelativeBY == 0)
				{
					if (this.points.Count == 5)
					{
						this.points.RemoveAt(1);
						this.points.RemoveAt(1);
					}
					this.points[1] = new Point(this.points[2].X, this.points[0].Y);
				}
				else
				{
					if (this.points.Count == 3)
					{
						this.points.Insert(1, Point.Zero);
						this.points.Insert(1, Point.Zero);
					}

					double px = this.points[4].X + (this.points[0].X-this.points[4].X)*this.field.RouteRelativeBX;
					double py = this.points[0].Y + (this.points[4].Y-this.points[0].Y)*this.field.RouteRelativeBY;
					this.points[1] = new Point(px, this.points[0].Y);
					this.points[2] = new Point(px, py);
					this.points[3] = new Point(this.points[4].X, py);
				}
			}

			if (this.field.Route == Field.RouteType.C)
			{
				//	Met à jour les points milieu de la connection.
				double px = this.points[0].X + (this.points[3].X-this.points[0].X)*this.field.RouteRelativeCX;
				this.points[1] = new Point(px, this.points[0].Y);
				this.points[2] = new Point(px, this.points[3].Y);
			}

			if (this.field.Route == Field.RouteType.D)
			{
				double px;
				if (this.field.IsAttachToRight)
				{
					px = System.Math.Max(this.points[0].X, this.points[3].X) + Editor.connectionDetour;
					px += this.field.RouteAbsoluteDX;
				}
				else
				{
					px = System.Math.Min(this.points[0].X, this.points[3].X) - Editor.connectionDetour;
					px -= this.field.RouteAbsoluteDX;
				}
				this.points[1] = new Point(px, this.points[0].Y);
				this.points[2] = new Point(px, this.points[3].Y);
			}
		}


		protected static readonly double pushMargin = 10;

		protected Field field;
		protected List<Point> points;
		protected bool isSrcHilited;
		protected bool isDraggingRoute;
		protected ObjectComment comment;
	}
}
