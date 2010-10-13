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

			return base.GetToolTipText(element);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	La souris est bougée.
			if (this.isDraggingDst)
			{
				this.DraggingDstMouseMove (pos);
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
				this.DraggingDstMouseDown (pos);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingDst)
			{
				this.DraggingDstMouseUp ();
			}

#if false
			if (this.hilitedElement == ActiveElement.LinkChangeDst)
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

			if (this.hilitedElement == ActiveElement.LinkClose)
			{
				this.srcObject.ObjectLinks.Remove (this);

				if (!this.IsUnlinked)
				{
					this.srcObject.RemoveEntityLink (this.dstObject);
				}

				this.editor.Invalidate ();
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
			if (this.DetectRoundButton (pos, this.PositionLinkChangeDst))
			{
				return ActiveElement.LinkChangeDst;
			}

			//	Souris dans le bouton pour créer le noeud destination ?
			if (this.DetectRoundButton (pos, this.PositionLinkCreateDst))
			{
				return ActiveElement.LinkCreateDst;
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


		public bool IsUnlinked
		{
			get
			{
				return this.dstObject == null;
			}
		}

		public void UpdateLink()
		{
			this.startVector = Vector.Zero;
			this.endVector   = Vector.Zero;

			if (this.isDraggingDst)
			{
				this.DraggingDstUpdateLink ();
				return;
			}

			if (this.DstObject == null)  // moignon de liaison o---> ?
			{
				Point p1 = new Point (this.srcObject.Bounds.Right,                            this.srcObject.Bounds.Center.Y);
				Point p2 = new Point (this.srcObject.Bounds.Right+AbstractObject.lengthClose, this.srcObject.Bounds.Center.Y);

				this.startVector = new Vector (p1, new Size ( 1, 0));
				this.endVector   = new Vector (p2, new Size (-1, 0));

				this.SetPathDirty ();
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

		private void DraggingDstUpdateLink()
		{
			this.endVector = new Vector (this.draggingDstPos, Size.Zero);

			double min = double.MaxValue;
			foreach (LinkAnchor anchor in System.Enum.GetValues (typeof (LinkAnchor)))
			{
				Vector vector = this.SrcObject.GetLinkVector (anchor, this.draggingDstPos, false);

				if (vector.IsValid)
				{
					double d = Point.Distance (this.draggingDstPos, vector.Origin);

					if (min > d)
					{
						min = d;
						this.startVector = vector;
					}
				}
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
			graphics.Rasterizer.AddOutline (this.Path, 6);
			graphics.RenderSolid (Color.FromBrightness (1));

			graphics.Rasterizer.AddOutline (this.Path, 2);
			Color color = (this.hilitedElement == ActiveElement.LinkHilited) ? this.GetColorMain () : this.GetColor (0);
			graphics.RenderSolid (color);

			{
				graphics.LineWidth = 2;

				if (this.startVector.IsValid && this.startVector.HasDirection)
				{
					AbstractObject.DrawStartingArrow (graphics, this.startVector.Origin, this.startVector.End);
				}

				if (this.endVector.IsValid && this.endVector.HasDirection)
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
			p = this.PositionLinkChangeDst;
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

			//	Dessine le bouton pour créer le noeud destination.
			p = this.PositionLinkCreateDst;
			if (!p.IsZero)
			{
				if (this.hilitedElement == ActiveElement.LinkCreateDst)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.Plus, true, false);
				}
				if (this.hilitedElement == ActiveElement.LinkHilited)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.Plus, false, false);
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

		private Point PositionLinkChangeDst
		{
			//	Retourne la position du bouton pour modifier la destination.
			get
			{
				if (this.endVector.IsValid)
				{
					return this.endVector.Origin;
				}
				else
				{
					return Point.Zero;
				}
			}
		}

		private Point PositionLinkCreateDst
		{
			//	Retourne la position du bouton pour créer la destination.
			get
			{
				if (this.endVector.IsValid && this.IsUnlinked)
				{
					return this.endVector.Origin + new Point (AbstractObject.buttonRadius*2+2, 0);
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
				return !this.IsUnlinked && (this.comment == null || !this.comment.IsVisible);
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


		#region Draggind destination
		private void DraggingDstMouseDown(Point pos)
		{
			this.isDraggingDst = true;
			this.editor.LockObject (this);

			this.DraggingDstMouseMove(pos);
		}

		private void DraggingDstMouseMove(Point pos)
		{
			this.draggingDstPos = pos;
			this.UpdateLink ();

			var obj = this.editor.DetectLinkableObject (pos, this.srcObject.GetType ());

			if (this.hilitedDstObject != obj)
			{
				if (this.hilitedDstObject != null)
				{
					this.hilitedDstObject.IsHilitedForLinkChanging = false;
				}

				this.hilitedDstObject = obj;

				if (this.hilitedDstObject != null)
				{
					this.hilitedDstObject.IsHilitedForLinkChanging = true;
				}
			}

			this.editor.Invalidate ();
		}

		private void DraggingDstMouseUp()
		{
			this.isDraggingDst = false;

			if (this.hilitedDstObject == null)
			{
				this.UpdateLink ();
			}
			else
			{
				if (this.dstObject != null)
				{
					this.srcObject.RemoveEntityLink (this.dstObject);
				}

				this.dstObject = this.hilitedDstObject;
				
				this.srcObject.AddEntityLink (this.dstObject);

				this.hilitedDstObject.IsHilitedForLinkChanging = false;
				this.hilitedDstObject = null;
			}

			this.editor.UpdateGeometry ();
			this.editor.LockObject (null);
			this.editor.SetLocalDirty ();
		}
		#endregion

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

				if (this.endVector.HasDirection)
				{
					path.CurveTo (this.startVector.GetPoint (d), this.endVector.GetPoint (d), this.endVector.Origin);
				}
				else
				{
					path.CurveTo (this.startVector.GetPoint (d), this.endVector.Origin);
				}
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
		private Point							draggingDstPos;
		private ObjectComment					comment;
		private LinkableObject					hilitedDstObject;
	}
}
