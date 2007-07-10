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


		public Field Field
		{
			//	Champ de r�f�rence pour la connection.
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
			//	Commentaire li�.
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
			//	Retourne la liste des points. Si la connection est ferm�e, il s'agit des points
			//	droite et gauche. Aurement, il s'agit d'un nombre variable de points.
			get
			{
				return this.points;
			}
		}

		public bool IsSrcHilited
		{
			//	Indique si la bo�te source est survol�e par la souris.
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
			//	Retourne la bo�te de l'objet.
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

		public override void Move(double dx, double dy)
		{
			//	D�place l'objet.
			for (int i=0; i<this.points.Count; i++)
			{
				this.points[i] = new Point(this.points[i].X+dx, this.points[i].Y+dy);
			}
		}

		public bool IsRightDirection
		{
			//	Retourne la direction effective dans laquelle part la connection.
			//	A ne pas confondre avec Field.IsAttachToRight !
			get
			{
				if (this.points.Count < 2)
				{
					return true;
				}
				else
				{
					return this.points[0].X < this.points[1].X;
				}
			}
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDraggingRoute)
			{
				return null;  // pas de tooltip
			}

			switch (element)
			{
				case AbstractObject.ActiveElement.ConnectionComment:
					if (this.comment == null)
					{
						return "Montre le commentaire associ�";
					}
					else if (!this.comment.IsVisible)
					{
						return string.Format("Montre le commentaire associ�<br/><b>{0}</b>", this.comment.Text);
					}
					else
					{
						return "Cache le commentaire associ�";
					}
			}

			return base.GetToolTipText(element);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	La souris est boug�e.
			if (this.isDraggingRoute)
			{
				this.RouteMove(pos);
				return true;
			}
			else
			{
				return base.MouseMove(message, pos);
			}
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est press�.
			if (this.hilitedElement == ActiveElement.ConnectionMove1 ||
				this.hilitedElement == ActiveElement.ConnectionMove2)
			{
				this.isDraggingRoute = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est rel�ch�.
			if (this.isDraggingRoute)
			{
				this.isDraggingRoute = false;
				this.editor.UpdateAfterGeometryChanged(null);
				this.editor.LockObject(null);
				this.editor.Module.AccessEntities.SetLocalDirty();
			}

			if (this.hilitedElement == ActiveElement.ConnectionOpenLeft ||
				this.hilitedElement == ActiveElement.ConnectionOpenRight)
			{
				Module module = this.editor.Module.DesignerApplication.SearchModule(this.field.Destination);
				CultureMap item = module.AccessEntities.Accessor.Collection[this.field.Destination];
				if (item != null)
				{
					this.field.IsExplored = true;

					ObjectBox box = this.editor.SearchBox(item.Name);
					if (box == null)
					{
						//	Ouvre la connection sur une nouvelle bo�te.
						box = new ObjectBox(this.editor);
						box.BackgroundMainColor = this.boxColor;
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
						//	Essaie de trouver une place libre, pour d�placer le moins possible d'�l�ments.
						Rectangle bounds;
						double posv = src.GetConnectionSrcVerticalPosition(this.field.Rank) - (Editor.connectionDetour+2);

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
						//	Ouvre la connection sur une bo�te existante.
						this.field.DstBox = box;
						this.field.IsAttachToRight = (this.hilitedElement == ActiveElement.ConnectionOpenRight);
					}

					this.editor.UpdateAfterAddOrRemoveConnection(box);
					this.editor.Module.AccessEntities.SetLocalDirty();
				}
			}

			if (this.hilitedElement == ActiveElement.ConnectionClose)
			{
				ObjectBox dst = this.field.DstBox;
				this.field.IsExplored = false;
				this.field.DstBox = null;
				this.editor.CloseBox(null);
				this.editor.UpdateAfterAddOrRemoveConnection(null);
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
				this.editor.Module.AccessEntities.SetGlobalDirty();
				this.editor.Invalidate();
				this.hilitedElement = ActiveElement.None;
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || this.points.Count == 0 || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return false;
			}

			//	Souris dans la pastille ronde du d�part de la connection ?
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
			if (this.IsConnectionCommentButton && this.DetectRoundButton(pos, this.PositionConnectionComment))
			{
				element = ActiveElement.ConnectionComment;
				return true;
			}

			//	Souris dans le bouton pour d�placer le point milieu ?
			if (this.DetectRoundButton(pos, this.PositionRouteMove1))
			{
				element = ActiveElement.ConnectionMove1;
				return true;
			}

			if (this.DetectRoundButton(pos, this.PositionRouteMove2))
			{
				element = ActiveElement.ConnectionMove2;
				return true;
			}

			//	Souris dans le bouton pour changer la connection ?
			if ((this.field.IsExplored || this.field.IsSourceExpanded) && this.DetectRoundButton(pos, this.PositionChangeRelation))
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
			//	D�tecte si la souris est le long de la connection.
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
			//	Ajoute un commentaire � la connection.
			if (this.comment == null)
			{
				this.comment = new ObjectComment(this.editor);
				this.comment.AttachObject = this;
				this.comment.BackgroundMainColor = this.field.CommentMainColor;
				this.comment.Text = this.field.CommentText;

				Point attach = this.PositionConnectionComment;
				Rectangle rect;

				if (attach.X > this.field.SrcBox.Bounds.Right)  // connection sur la droite ?
				{
					rect = new Rectangle(attach.X+20, attach.Y+20, Editor.defaultWidth, 50);  // hauteur arbitraire
				}
				else  // connection sur la gauche ?
				{
					rect = new Rectangle(attach.X-20-Editor.defaultWidth, attach.Y+20, Editor.defaultWidth, 50);  // hauteur arbitraire
				}

				this.comment.SetBounds(rect);
				this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment(this.comment);
				this.editor.UpdateAfterCommentChanged();

				this.comment.EditComment();  // �dite tout de suite le texte du commentaire
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}

			this.editor.Module.AccessEntities.SetLocalDirty();
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

				Color color = this.GetColor(0);
				if (this.hilitedElement == ActiveElement.ConnectionHilited ||
					this.hilitedElement == ActiveElement.ConnectionChangeRelation)
				{
					color = this.GetColorMain();
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

				Color color = this.GetColor(0);
				if (this.hilitedElement == ActiveElement.ConnectionHilited ||
					this.hilitedElement == ActiveElement.ConnectionChangeRelation)
				{
					color = this.GetColorMain();
				}
				graphics.RenderSolid(color);
			}

			if (this.points.Count != 0 && this.field.IsSourceExpanded)
			{
				//	Dessine les cercles aux points de d�part.
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
			if (!p.IsZero && this.IsConnectionCommentButton)
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

			//	Dessine le bouton pour d�placer le point milieu.
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


		protected bool IsConnectionCommentButton
		{
			//	Indique s'il faut affiche le bouton pour montrer le commentaire.
			//	Si un commentaire est visible, il ne faut pas montrer le bouton, car il y a d�j�
			//	le bouton CommentAttachToConnection pour d�placer le point d'attache.
			get
			{
				return (this.comment == null || !this.comment.IsVisible);
			}
		}

		public Point PositionConnectionComment
		{
			//	Retourne la position du bouton pour commenter la connection, ou pour d�placer
			//	le point d'attache lorsque le commentaire existe.
			get
			{
				if (this.field.IsSourceExpanded && this.field.IsExplored && this.points.Count >= 2)
				{
					return this.AttachToPoint(this.field.CommentAttach);
				}

				return Point.Zero;
			}
		}

		protected Point AttachToPoint(double d)
		{
			//	Conversion d'une distance le long de la connection en position.
			//	Une distance positive commence depuis le d�but de la connection.
			//	Une distance n�gative commence depuis la fin de la connection.
			if (this.points.Count < 2)
			{
				return Point.Zero;
			}

			double total = 0;
			for (int i=0; i<this.points.Count-1; i++)
			{
				total += Point.Distance(this.points[i], this.points[i+1]);
			}

			bool fromBegin = true;
			if (d < 0)  // attach� depuis la fin ?
			{
				d = d+total;
				fromBegin = false;
			}

			d = System.Math.Min(d, total-AbstractObject.minAttach);
			d = System.Math.Max(d, AbstractObject.minAttach);

			if (fromBegin)  // attach� depuis le d�but ?
			{
				for (int i=0; i<this.points.Count-1; i++)
				{
					double len = Point.Distance(this.points[i], this.points[i+1]);
					if (d < len)
					{
						return Point.Move(this.points[i], this.points[i+1], d);
					}
					else
					{
						d -= len;
					}
				}
			}
			else  // attach� depuis la fin ?
			{
				d = total-d;
				for (int i=this.points.Count-2; i>=0; i--)
				{
					double len = Point.Distance(this.points[i], this.points[i+1]);
					if (d < len)
					{
						return Point.Move(this.points[i+1], this.points[i], d);
					}
					else
					{
						d -= len;
					}
				}
			}

			return Point.Move(this.points[this.points.Count-1], this.points[this.points.Count-2], AbstractObject.minAttach);
		}

		public double PointToAttach(Point p)
		{
			//	Conversion d'une position le long de la connection en distance depuis le d�but (si positif)
			//	ou depuis la fin (si n�gatif).
			if (this.points.Count < 2)
			{
				return 0;
			}

			double total = 0;
			double min = double.MaxValue;
			int j = -1;
			for (int i=0; i<this.points.Count-1; i++)
			{
				total += Point.Distance(this.points[i], this.points[i+1]);
				Point pi = Point.Projection(this.points[i], this.points[i+1], p);
				if (Geometry.IsInside(this.points[i], this.points[i+1], pi))
				{
					double di = Point.Distance(pi, p);
					if (di < min)
					{
						min = di;
						j = i;
					}
				}
			}

			if (j == -1)
			{
				return 0;
			}

			Point pj = Point.Projection(this.points[j], this.points[j+1], p);
			double dj = Point.Distance(this.points[j], pj);

			for (int i=0; i<j; i++)
			{
				dj += Point.Distance(this.points[i], this.points[i+1]);
			}

			dj = System.Math.Max(dj, AbstractObject.minAttach);
			dj = System.Math.Min(dj, total-AbstractObject.minAttach);

			if (dj > total/2)  // plus proche de la fin ?
			{
				dj = dj-total;  // attach� depuis la fin (valeur n�gative)
			}

			return dj;
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

					if (this.points.Count == 2)
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
					
					if (this.points.Count == 3)
					{
						if (Point.Distance(this.points[0], this.points[1]) >= 50 && Point.Distance(this.points[1], this.points[2]) >= 50)
						{
							return this.points[1];
						}
					}
				}

				if (this.field.Route == Field.RouteType.C)
				{
					if (this.points.Count == 4)
					{
						if (Point.Distance(this.points[0], this.points[3]) >= 75)
						{
							return this.points[1];
						}
					}
				}

				if (this.field.Route == Field.RouteType.D)
				{
					if (this.points.Count == 4)
					{
						return this.points[1];
					}
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
					
					if (this.points.Count == 2)
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

			Point oldPos = this.PositionConnectionComment;  // point d'attache avant re-routage

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

			Point newPos = this.PositionConnectionComment;  // point d'attache apr�s re-routage

			if (this.comment != null)
			{
				Rectangle bounds = this.comment.Bounds;
				bounds.Offset(newPos-oldPos);
				this.comment.SetBounds(bounds);  // d�place le commentaire
			}
		}

		public void UpdateRoute()
		{
			//	Met � jour le routage de la connection, dans les cas ou le routage d�pend des choix de l'utilisateur.
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
				//	Met � jour les points milieu de la connection.
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
