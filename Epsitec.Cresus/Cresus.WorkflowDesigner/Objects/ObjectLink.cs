//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

using System.Xml;
using System.Xml.Serialization;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class ObjectLink : AbstractObject
	{
		public ObjectLink(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.CommentAttach = 0.1;
		}


		public LinkableObject SrcObject
		{
			//	Objet source de la connexion
			get
			{
				return this.srcObject;
			}
			set
			{
				this.srcObject = value;
			}
		}

		public LinkableObject DstObject
		{
			//	Objet destination de la connexion (si la connexion débouche sur un noeud).
			get
			{
				return this.dstObject;
			}
			set
			{
				this.dstObject = value;
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

		public double CommentAttach
		{
			//	Position relative le long de la courbe du commentaire lié (0..1).
			get;
			set;
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
				Rectangle bounds = this.Path.ComputeBounds ();
				bounds.Inflate (2);

				if (this.startVector.IsValid)
				{
					bounds = Rectangle.Union (bounds, new Rectangle (this.startVector.Origin, Size.Zero));
				}

				if (this.endVector.IsValid)
				{
					bounds = Rectangle.Union (bounds, new Rectangle (this.endVector.Origin, Size.Zero));
				}

				return bounds;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			if (this.startVector.IsValid)
			{
				this.startVector = new Vector (this.startVector, new Size (dx, dy));
			}

			if (this.endVector.IsValid)
			{
				this.endVector = new Vector (this.endVector, new Size (dx, dy));
			}

			this.SetPathDirty ();
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDraggingDst)
			{
				return null;  // pas de tooltip
			}

#if false
			switch (element)
			{
				case AbstractObject.ActiveElement.ConnexionComment:
					if (this.comment == null)
					{
						return Res.Strings.Entities.Action.ConnexionComment3;
					}
					else if (!this.comment.IsVisible)
					{
						return string.Format(Res.Strings.Entities.Action.ConnexionComment2, this.comment.Text);
					}
					else
					{
						return Res.Strings.Entities.Action.ConnexionComment1;
					}
			}
#endif

			return base.GetToolTipText(element);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	La souris est bougée.
			if (this.isDraggingDst)
			{
				this.MouseMoveDst (pos);
				return true;
			}
			else
			{
				return base.MouseMove (message, pos);
			}
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			if (this.hilitedElement == ActiveElement.LinkChangeDst)
			{
				this.MouseDownDst (pos);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingDst)
			{
				this.MouseUpDst ();
			}

#if false
			if (this.hilitedElement == ActiveElement.EdgeOpenLeft ||
				this.hilitedElement == ActiveElement.EdgeOpenRight)
			{
				this.IsExplored = true;

				ObjectNode2 node = this.editor.SearchNode2 (this.Entity.NextNode);
				if (node == null)
				{
					//	Ouvre la connexion sur une nouvelle boîte.
					node = new ObjectNode2 (this.editor, this.Entity.NextNode);
					node.BackgroundMainColor = this.boxColor;

					this.DstNode = node;
					this.IsAttachToRight = (this.hilitedElement == ActiveElement.EdgeOpenRight);

					this.editor.AddNode (node);
					this.editor.UpdateGeometry ();

					ObjectNode2 src = this.SrcNode;
					//	Essaie de trouver une place libre, pour déplacer le moins possible d'éléments.
					Rectangle bounds;
					double posv = src.Bounds.Center.Y;

					if (this.hilitedElement == ActiveElement.EdgeOpenLeft)
					{
						bounds = new Rectangle (src.Bounds.Left-50-node.Bounds.Width, posv-node.Bounds.Height, node.Bounds.Width, node.Bounds.Height);
						bounds.Inflate (50, Editor.pushMargin);

						for (int i=0; i<1000; i++)
						{
							if (this.editor.IsEmptyArea (bounds))
							{
								break;
							}
							bounds.Offset (-1, 0);
						}

						bounds.Deflate (50, Editor.pushMargin);
					}
					else
					{
						bounds = new Rectangle (src.Bounds.Right+50, posv-node.Bounds.Height, node.Bounds.Width, node.Bounds.Height);
						bounds.Inflate (50, Editor.pushMargin);

						for (int i=0; i<1000; i++)
						{
							if (this.editor.IsEmptyArea (bounds))
							{
								break;
							}
							bounds.Offset (1, 0);
						}

						bounds.Deflate (50, Editor.pushMargin);
					}
					bounds = this.editor.NodeGridAlign (bounds);
					node.SetBounds (bounds);
				}
				else
				{
					//	Ouvre la connexion sur une boîte existante.
					this.DstNode = node;
					this.IsAttachToRight = (this.hilitedElement == ActiveElement.EdgeOpenRight);
				}

				this.editor.UpdateAfterAddOrRemoveEdge2 (node);
				this.editor.SetLocalDirty ();
			}

			if (this.hilitedElement == ActiveElement.EdgeClose)
			{
				ObjectNode2 dst = this.DstNode;
				this.IsExplored = false;
				this.DstNode = null;
				this.editor.CloseNode(null);
				this.editor.UpdateAfterAddOrRemoveEdge2(null);
			}
#endif

			if (this.hilitedElement == ActiveElement.LinkComment)
			{
				this.AddComment();
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (pos.IsZero || this.startVector.IsZero || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			//	Souris le long de la connexion ?
			if (this.DetectOver(pos, 4))
			{
				return ActiveElement.LinkHilited;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (pos.IsZero || this.startVector.IsZero || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			//	Souris dans le bouton pour fermer la connexion.
			if (this.DetectRoundButton (pos, this.PositionLinkClose))
			{
				return ActiveElement.LinkClose;
			}

			//	Souris dans le bouton pour commenter la connexion.
			if (this.IsLinkCommentButton && this.DetectRoundButton (pos, this.PositionLinkComment))
			{
				return ActiveElement.LinkComment;
			}

			//	Souris dans le bouton pour changer le noeud destination ?
			if (this.DetectRoundButton (pos, this.PositionRouteChangeDst))
			{
				return ActiveElement.LinkChangeDst;
			}

			return ActiveElement.None;
		}

		private bool DetectOver(Point pos, double margin)
		{
			//	Détecte si la souris est le long de la connexion.
			if (this.DstObject != null)
			{
				var rect = this.Bounds;
				rect.Inflate (margin);

				if (rect.Contains (pos))
				{
					if (Geometry.DetectOutline (this.Path, margin, pos))
					{
						return true;
					}
				}
			}

			return false;
		}


		public void UpdateLink()
		{
			this.startVector = Vector.Zero;
			this.endVector   = Vector.Zero;

			if (this.DstObject == null)
			{
				// TODO:

				return;
			}

			//	Une connexion est toujours edge -> node ou node -> edge.
			bool edgeToNode = this.SrcObject is ObjectEdge;

			ObjectEdge edge;
			ObjectNode node;

			if (edgeToNode)
			{
				edge = this.SrcObject as ObjectEdge;
				node = this.DstObject as ObjectNode;
			}
			else
			{
				node = this.SrcObject as ObjectNode;
				edge = this.DstObject as ObjectEdge;
			}

			//	S'il ne s'agit pas d'une connexion edge -> node ou node -> edge, ou ne peut rien faire.
			if (edge == null || node == null)
			{
				return;
			}

			//	S'il y a chevauchement ou presque entre les boîtes sources et destination, il est
			//	préférable de ne pas dessiner de liaison.
			Rectangle re = edge.Bounds;
			Rectangle rn = node.Bounds;

			re.Inflate (4);
			rn.Inflate (4);

			if (re.IntersectsWith (rn))
			{
				return;
			}

			//	Cherche la connexion la plus courte parmi toutes les possibilités.
			//	On tient compte du fait que l'objet node a un vecteur qui est toujours dirgé à
			//	partir du centre, contrairement à l'objet edge.
			LinkAnchor nodeAnchor = LinkAnchor.Left;  // sans importance
			LinkAnchor edgeAnchor = LinkAnchor.Left;  // pour que ça compile

			Vector v1 = node.GetLinkVector (nodeAnchor, edge.Bounds.Center, edgeToNode);

			double min = double.MaxValue;
			foreach (LinkAnchor a2 in System.Enum.GetValues (typeof (LinkAnchor)))
			{
				Vector v2 = edge.GetLinkVector (a2, node.Bounds.Center, !edgeToNode);

				if (v2.IsValid)
				{
					double d = Point.Distance (v1.Origin, v2.Origin);

					if (min > d)
					{
						min = d;
						edgeAnchor = a2;
					}
				}
			}

			//	Calcule les vecteurs définitifs.
			Vector edgeVector = edge.GetLinkVector (edgeAnchor, node.Bounds.Center, !edgeToNode);
			Vector nodeVector = node.GetLinkVector (nodeAnchor, edgeVector.Origin,   edgeToNode);

			if (edgeToNode)
			{
				this.startVector = edgeVector;
				this.endVector   = nodeVector;
			}
			else
			{
				this.startVector = nodeVector;
				this.endVector   = edgeVector;
			}

			this.SetPathDirty ();
		}


		private void AddComment()
		{
			//	Ajoute un commentaire à la connexion.
			if (this.comment == null)
			{
				this.comment = new ObjectComment(this.editor, this.entity);
				this.comment.AttachObject = this;
				this.comment.Text = "Coucou";

				Point attach = this.PositionLinkComment;
				Rectangle rect;

				if (attach.X > this.SrcObject.Bounds.Right)  // connexion sur la droite ?
				{
					rect = new Rectangle(attach.X+20, attach.Y+20, Editor.defaultWidth, 50);  // hauteur arbitraire
				}
				else  // connexion sur la gauche ?
				{
					rect = new Rectangle(attach.X-20-Editor.defaultWidth, attach.Y+20, Editor.defaultWidth, 50);  // hauteur arbitraire
				}

				this.comment.SetBounds(rect);
				this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment(this.comment);
				this.editor.UpdateAfterCommentChanged();

				this.comment.EditComment();  // édite tout de suite le texte du commentaire
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}

			this.editor.SetLocalDirty ();
		}

		
		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine l'objet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			if (this.isDraggingDst)
			{
				//	Dessine une connexion courbe qui rejoint le point d'arrivée en train d'être
				//	déplacé par la souris, pour changer le noeud de destination.
#if false
				Point p1 = this.points.First ();
				Point ps = (this.points.Count > 2) ? this.points[1] : Point.Zero;
				Point p2 = this.points.Last ();
				Color color = this.GetColor (0);
				double lineWidth = 4;

				Path path = new Path ();
				path.MoveTo (p1);
				if (ps.IsZero)
				{
					path.LineTo (p2);
				}
				else
				{
					path.CurveTo (ps, p2);
				}
				Misc.DrawPathDash (graphics, path, lineWidth, 15, 10, true, color);

				graphics.LineWidth = lineWidth;
				AbstractObject.DrawEndingArrow (graphics, ps.IsZero ? p1 : ps, p2);
				graphics.RenderSolid (color);
				graphics.LineWidth = 1;

				this.DrawRoundButton (graphics, p1, AbstractObject.bulletRadius+1, false, false, true);
#endif

				return;
			}

			if (this.DstObject != null)
			{
				graphics.Rasterizer.AddOutline (this.Path, 6);
				graphics.RenderSolid (Color.FromBrightness (1));

				graphics.Rasterizer.AddOutline (this.Path, 2);
				Color color = (this.hilitedElement == ActiveElement.LinkHilited) ? this.GetColorMain () : this.GetColor (0);
				graphics.RenderSolid (color);

				{
					graphics.LineWidth = 2;

					if (this.startVector.IsValid)
					{
						AbstractObject.DrawStartingArrow (graphics, this.startVector.Origin, this.startVector.End);
					}

					if (this.endVector.IsValid)
					{
						AbstractObject.DrawEndingArrow (graphics, this.endVector.End, this.endVector.Origin);
					}

					graphics.RenderSolid (color);
					graphics.LineWidth = 1;
				}

				if (this.startVector.IsValid)
				{
					this.DrawRoundButton (graphics, this.startVector.Origin, AbstractObject.bulletRadius, GlyphShape.None, false, false);
				}
			}

			if (this.DstObject == null && this.startVector.IsValid)
			{
				//	Dessine le moignon de liaison.
				Point start = this.startVector.Origin;
				Point end = new Point(start.X+AbstractObject.lengthClose, start.Y);

				graphics.LineWidth = 2;
				graphics.AddLine(start, end);
				AbstractObject.DrawEndingArrow(graphics, start, end);
				graphics.LineWidth = 1;

				Color color = this.GetColor(0);
				if (this.hilitedElement == ActiveElement.LinkHilited)
				{
					color = this.GetColorMain();
				}
				graphics.RenderSolid(color);
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			Point p;

			//	Dessine le bouton pour commenter la connexion.
			p = this.PositionLinkComment;
			if (!p.IsZero && this.IsLinkCommentButton)
			{
				if (this.hilitedElement == ActiveElement.LinkComment)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "C", true, false);
				}
				if (this.hilitedElement == ActiveElement.LinkHilited)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "C", false, false);
				}
			}

			//	Dessine le bouton pour fermer la connexion.
			p = this.PositionLinkClose;
			if (!p.IsZero)
			{
				if (this.hilitedElement == ActiveElement.LinkClose)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.Close, true, false);
				}
				if (this.hilitedElement == ActiveElement.LinkHilited)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.Close, false, false);
				}
			}

			//	Dessine le bouton pour changer de noeud destination.
			p = this.PositionRouteChangeDst;
			if (!p.IsZero)
			{
				if (this.hilitedElement == ActiveElement.LinkChangeDst)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.LinkHilited)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}
			}
		}


		public Point PositionLinkClose
		{
			get
			{
				if (this.startVector.IsValid)
				{
					return this.startVector.Origin;
				}
				else
				{
					return Point.Zero;
				}
			}
		}


		private bool IsLinkCommentButton
		{
			//	Indique s'il faut affiche le bouton pour montrer le commentaire.
			//	Si un commentaire est visible, il ne faut pas montrer le bouton, car il y a déjà
			//	le bouton CommentAttachToEdge pour déplacer le point d'attache.
			get
			{
				return (this.comment == null || !this.comment.IsVisible);
			}
		}

		public Point PositionLinkComment
		{
			//	Retourne la position du bouton pour commenter la connexion, ou pour déplacer
			//	le point d'attache lorsque le commentaire existe.
			get
			{
				return this.AttachToPoint (this.CommentAttach);
			}
		}

		private Point AttachToPoint(double d)
		{
			if (this.DstObject != null && this.startVector.IsValid)
			{
				return Geometry.PointOnPath (this.Path, this.CommentAttach);
			}
			else
			{
				return Point.Zero;
			}
		}

		public double PointToAttach(Point p)
		{
			if (this.DstObject != null && this.startVector.IsValid)
			{
				double offset = Geometry.OffsetOnPath (this.Path, p);

				offset = System.Math.Max (offset, 0.1);
				offset = System.Math.Min (offset, 0.9);

				return offset;
			}
			else
			{
				return 0;
			}
		}


		private Point PositionRouteChangeDst
		{
			//	Retourne la position du bouton pour modifier le noeud destination.
			get
			{
				if (this.endVector.IsZero)
				{
					return Point.Zero;
				}
				else
				{
					return this.endVector.Origin;
				}
			}
		}


		private void MouseDownDst(Point pos)
		{
			this.isDraggingDst = true;
			this.editor.LockObject (this);

			this.MouseMoveDst(pos);
		}

		private void MouseMoveDst(Point pos)
		{
			this.endVector = new Vector (pos, this.endVector.Direction);

			var node = this.DetectNode (pos);

			if (this.hilitedDstNode != node)
			{
				if (this.hilitedDstNode != null)
				{
					this.hilitedDstNode.IsHilitedForEdgeChanging = false;
				}

				this.hilitedDstNode = node;

				if (this.hilitedDstNode != null)
				{
					this.hilitedDstNode.IsHilitedForEdgeChanging = true;
				}
			}

			this.editor.Invalidate ();
		}

		private void MouseUpDst()
		{
			this.isDraggingDst = false;

			if (this.hilitedDstNode != null)
			{
				this.DstObject = this.hilitedDstNode;
				//?this.Entity.NextNode = this.DstNode.Entity;

				this.hilitedDstNode.IsHilitedForEdgeChanging = false;
				this.hilitedDstNode = null;
			}

			this.editor.UpdateGeometry ();
			this.editor.LockObject (null);
			this.editor.SetLocalDirty ();
		}

		private ObjectNode DetectNode(Point pos)
		{
			for (int i=this.editor.Nodes.Count-1; i>=0; i--)
			{
				ObjectNode node = this.editor.Nodes[i];

				if (node.Bounds.Contains (pos))
				{
					return node;
				}
			}

			return null;
		}


		#region Path engine
		private void SetPathDirty()
		{
			this.path = null;
		}

		private Path Path
		{
			get
			{
				if (this.path == null)
				{
					this.path = this.ComputePath ();
				}

				return this.path;
			}
		}

		private Path ComputePath()
		{
			var path = new Path ();

			if (this.startVector.IsValid && this.endVector.IsValid)
			{
				double d = Point.Distance (this.startVector.Origin, this.endVector.Origin) * 0.5;

				path.MoveTo (this.startVector.Origin);
				path.CurveTo (this.startVector.GetPoint (d), this.endVector.GetPoint (d), this.endVector.Origin);
			}

			return path;
		}
		#endregion


		private LinkableObject					srcObject;
		private LinkableObject					dstObject;
		private Vector							startVector;
		private Vector							endVector;
		private Path							path;
		private bool							isSrcHilited;
		private bool							isDraggingDst;
		private ObjectComment					comment;
		private ObjectNode						hilitedDstNode;
	}
}
