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
			this.commentAttach = 0.5;  // au milieu
			this.StumpAngle = 0;  // moignon o---> par défaut

			this.magnetList = new List<Point> ();
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
			get
			{
				return this.commentAttach;
			}
			set
			{
				value = System.Math.Max (value, 0.2);
				value = System.Math.Min (value, 0.8);

				this.commentAttach = value;
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
			else if (this.isDraggingMagnet)
			{
				this.MagnetMouseMove (pos);
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

			if (this.hilitedElement == ActiveElement.LinkMagnetHome)
			{
				this.MagnetMouseDown (pos, -1);
			}

			if (this.hilitedElement >= ActiveElement.LinkMagnet0 &&
				this.hilitedElement <= ActiveElement.LinkMagnet9)
			{
				this.MagnetMouseDown (pos, this.hilitedElement - ActiveElement.LinkMagnet0);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingDst)
			{
				this.DraggingDstMouseUp ();
			}

			if (this.isDraggingMagnet)
			{
				this.MagnetMouseUp (pos);
			}

			if (this.hilitedElement == ActiveElement.LinkComment)
			{
				this.AddComment();
			}

			if (this.hilitedElement == ActiveElement.LinkClose)
			{
				this.srcObject.ObjectLinks.Remove (this);

				if (!this.IsNoneDstObject)
				{
					this.srcObject.RemoveEntityLink (this.dstObject);
				}

				this.editor.UpdateAfterGeometryChanged (null);
			}

			if (this.hilitedElement == ActiveElement.LinkCreateDst)
			{
				if (this.srcObject is ObjectNode)
				{
					this.CreateEdge ();
				}

				if (this.srcObject is ObjectEdge)
				{
					this.CreateNode ();
				}
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
			if (this.HasLinkCommentButton && this.DetectRoundButton (pos, this.PositionLinkComment))
			{
				return ActiveElement.LinkComment;
			}

			//	Souris dans l'un des boutons magnétiques.
			if (this.DetectRoundButton (pos, this.MagnetHomePos))
			{
				return ActiveElement.LinkMagnetHome;
			}

			for (int i=0; i<this.magnetList.Count; i++)
			{
				if (this.DetectRoundButton (pos, this.GetMagnetAbsolutePos (i)))
				{
					return ActiveElement.LinkMagnet0+i;
				}
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
			var rect = this.Bounds;
			rect.Inflate (margin);

			if (rect.Contains (pos))
			{
				if (Geometry.DetectOutline (this.Path, margin, pos))
				{
					return true;
				}
			}

			return false;
		}


		private void CreateEdge()
		{
			var edgeEntity = this.editor.BusinessContext.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			edgeEntity.Name = "Nouveau";

			var obj = new ObjectEdge (this.editor, edgeEntity);
			obj.ObjectLinks[0].StumpAngle = this.GetAngle ();

			this.dstObject = obj;
			this.srcObject.AddEntityLink (obj);

			this.editor.AddEdge (obj);
			obj.SetBoundsAtEnd (this.startVector.Origin, this.endVector.Origin);
			this.editor.UpdateGeometry ();

			this.MoveObjectToFreeArea (obj, this.startVector.Origin, this.endVector.Origin);
		}

		private void CreateNode()
		{
			var nodeEntity = this.editor.BusinessContext.DataContext.CreateEntity<WorkflowNodeEntity> ();

			var obj = new ObjectNode (this.editor, nodeEntity);

			this.dstObject = obj;
			this.srcObject.AddEntityLink (obj);

			this.editor.AddNode (obj);
			obj.SetBoundsAtEnd (this.startVector.Origin, this.endVector.Origin);
			this.editor.UpdateGeometry ();

			this.MoveObjectToFreeArea (obj, this.startVector.Origin, this.endVector.Origin);
		}


		public override string DebugInformations
		{
			get
			{
				var builder = new System.Text.StringBuilder ();

				builder.Append ("Link: ");

				if (this.srcObject == null)
				{
					builder.Append ("x");
				}
				else
				{
					builder.Append (this.srcObject.DebugInformationsBase);
				}

				builder.Append ("->");

				if (this.dstObject == null)
				{
					builder.Append ("x");
				}
				else
				{
					builder.Append (this.dstObject.DebugInformationsBase);
				}

				return builder.ToString ();
			}
		}

		public override string DebugInformationsBase
		{
			get
			{
				return this.DebugInformations;
			}
		}


		private void MoveObjectToFreeArea(LinkableObject obj, Point start, Point end)
		{
			//	Essaie de trouver une place libre, pour déplacer le moins possible d'éléments.
			Point offset = Point.Move (start, end, 2);

			Rectangle bounds = obj.Bounds;
			bounds.Inflate (Editor.pushMargin);

			for (int i=0; i<1000; i++)
			{
				if (this.editor.IsEmptyArea (bounds, obj))
				{
					break;
				}
				bounds.Offset (offset);
			}

			bounds.Deflate (Editor.pushMargin);

			bounds = this.editor.NodeGridAlign (bounds);
			obj.SetBounds (bounds);

			this.dstObject = obj;
			this.editor.UpdateAfterGeometryChanged (obj);
		}


		public double StumpAngle
		{
			get;
			set;
		}

		public bool IsNoneDstObject
		{
			get
			{
				return this.dstObject == null;
			}
		}

		public void UpdateLink()
		{
			//	Met à jour les deux vecteurs permettant de définir le chemin de la connexion.
			this.SetPathDirty ();

			this.startVector = Vector.Zero;
			this.endVector   = Vector.Zero;

			LinkableObject dstObject = (this.hilitedDstObject == null) ? this.DstObject : this.hilitedDstObject;

			//	Une connexion est toujours edge -> node ou node -> edge.
			bool edgeToNode = this.SrcObject is ObjectEdge;

			LinkableObject edge;
			LinkableObject node;

			if (edgeToNode)
			{
				edge = this.SrcObject as ObjectEdge;
				node =      dstObject as ObjectNode;
			}
			else
			{
				node = this.SrcObject as ObjectNode;
				edge =      dstObject as ObjectEdge;
			}

			if (dstObject == null || (this.isDraggingDst && this.hilitedDstObject == null))
			{
				Point pos = this.draggingStumpPos;

				if (pos.IsZero)
				{
					pos = this.srcObject.GetLinkStumpPos (this.StumpAngle);
				}

				if (edgeToNode)
				{
					node = new ObjectFoo (this.editor, null);
					node.SetBounds (new Rectangle (pos, Size.Zero));
				}
				else
				{
					edge = new ObjectFoo (this.editor, null);
					edge.SetBounds (new Rectangle (pos, Size.Zero));
				}
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

			double angle = this.GetAngleSrc ();
			if (!double.IsNaN (angle))
			{
				this.StumpAngle = angle;
			}
		}

		public double GetAngleSrc()
		{
			//	Retourne l'angle que fait la connexion avec son objet source.
			if (this.srcObject != null && this.startVector.IsValid)
			{
				return Point.ComputeAngleDeg (this.srcObject.Bounds.Center, this.startVector.Origin);
			}
			else
			{
				return double.NaN;
			}
		}

		public double GetAngleDst()
		{
			//	Retourne l'angle que fait la connexion avec son objet destination.
			if (this.dstObject != null && this.endVector.IsValid)
			{
				return Point.ComputeAngleDeg (this.dstObject.Bounds.Center, this.endVector.Origin);
			}
			else
			{
				return double.NaN;
			}
		}

		public double GetAngle()
		{
			//	Retourne l'angle de la connexion.
			if (this.startVector.IsValid && this.endVector.IsValid)
			{
				return Point.ComputeAngleDeg (this.startVector.Origin, this.endVector.Origin);
			}
			else
			{
				return double.NaN;
			}
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
			//	Dessine la connexion en blanc.
			graphics.Rasterizer.AddOutline (this.Path, 6);
			graphics.RenderSolid (Color.FromBrightness (1));

			//	Dessine les contraintes magnétiques.
			if ((this.isDraggingMagnet || this.IsHilite) && this.endVector.HasDirection)
			{
				Misc.DrawPathDash (graphics, this.MagnetConstrainPath, 1, 1, 4, true, this.GetColorMain ());
			}

			//	Dessine la connexion initiale.
			if (this.initialPath != null)
			{
				Color dimmedColor = this.GetColorMain (0.05);

				graphics.Rasterizer.AddOutline (this.initialPath, 2);
				graphics.RenderSolid (dimmedColor);

				this.DrawArrow (graphics, this.initialStartVector, this.initialEndVector, dimmedColor);
			}

			//	Dessine la connexion et la flèche.
			Color color = (this.IsHilite || this.isDraggingDst) ? this.GetColorMain () : this.GetColor (0);

			if (this.isDraggingDst && this.hilitedDstObject == null)
			{
				Misc.DrawPathDash (graphics, this.Path, 2, 1, 4, true, color);
			}
			else
			{
				graphics.Rasterizer.AddOutline (this.Path, 2);
				graphics.RenderSolid (color);
			}

			this.DrawArrow (graphics, this.startVector, this.endVector, color);

			//	Dessine la pastille au départ.
			if (this.startVector.IsValid)
			{
				this.DrawRoundButton (graphics, this.startVector.Origin, AbstractObject.bulletRadius, GlyphShape.None, false, false);
			}
		}

		private void DrawArrow(Graphics graphics, Vector startVector, Vector endVector, Color color)
		{
			graphics.LineWidth = 2;

			if (startVector.IsValid && startVector.HasDirection)
			{
				AbstractObject.DrawStartingArrow (graphics, startVector.Origin, startVector.End);
			}

			if (endVector.IsValid && endVector.HasDirection)
			{
				AbstractObject.DrawEndingArrow (graphics, endVector.End, endVector.Origin);
			}

			graphics.RenderSolid (color);
			graphics.LineWidth = 1;
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine les contraintes magnétiques.
			if ((this.isDraggingMagnet || this.IsHilite) && this.endVector.HasDirection)
			{
				this.DrawRoundButton (graphics, this.MagnetStartPos, 3, GlyphShape.None, false, false);
				this.DrawRoundButton (graphics, this.MagnetEndPos,   3, GlyphShape.None, false, false);
			}

			Point p;

			//	Dessine le bouton pour commenter la connexion.
			p = this.PositionLinkComment;
			if (!p.IsZero && this.HasLinkCommentButton)
			{
				if (this.hilitedElement == ActiveElement.LinkComment)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "C", true, false);
				}
				else if (this.IsHilite)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "C", false, false);
				}
			}

			//	Dessine les boutons magnétiques.
			p = this.MagnetHomePos;
			if (!p.IsZero)
			{
				if (this.hilitedElement == ActiveElement.LinkMagnetHome)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "o", true, false);
				}
				else if (this.IsHilite)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "o", false, false);
				}
			}

			for (int i=0; i<this.magnetList.Count; i++)
			{
				p = this.GetMagnetAbsolutePos (i);
				if (!p.IsZero)
				{
					if (this.hilitedElement == ActiveElement.LinkMagnet0+i)
					{
						this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "o", true, false);
					}
					else if (this.IsHilite)
					{
						this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, "o", false, false);
					}
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
				else if (this.IsHilite)
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
				else if (this.IsHilite)
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
				else if (this.IsHilite)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.Plus, false, false);
				}
			}
		}


		private bool IsHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked || this.isDraggingDst)
				{
					return false;
				}

				return (this.hilitedElement == ActiveElement.LinkHilited ||
						this.hilitedElement == ActiveElement.LinkClose ||
						this.hilitedElement == ActiveElement.LinkComment ||
						this.hilitedElement == ActiveElement.LinkMagnetHome ||
						this.hilitedElement == ActiveElement.LinkMagnet0 ||
						this.hilitedElement == ActiveElement.LinkMagnet1 ||
						this.hilitedElement == ActiveElement.LinkMagnet2 ||
						this.hilitedElement == ActiveElement.LinkMagnet3 ||
						this.hilitedElement == ActiveElement.LinkMagnet4 ||
						this.hilitedElement == ActiveElement.LinkMagnet5 ||
						this.hilitedElement == ActiveElement.LinkMagnet6 ||
						this.hilitedElement == ActiveElement.LinkMagnet7 ||
						this.hilitedElement == ActiveElement.LinkMagnet8 ||
						this.hilitedElement == ActiveElement.LinkMagnet9 ||
						this.hilitedElement == ActiveElement.LinkChangeDst ||
						this.hilitedElement == ActiveElement.LinkCreateDst);
			}
		}


		private Point PositionLinkClose
		{
			get
			{
				if (this.startVector.IsValid && !this.HasSrcWithSingleLink)
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
				if (this.endVector.IsValid && this.IsNoneDstObject)
				{
					if (this.endVector.HasDirection)
					{
						return Point.Move (this.endVector.Origin, this.endVector.End, -AbstractObject.buttonRadius*2+2);
					}
					else
					{
						return this.endVector.Origin + new Point (AbstractObject.buttonRadius*2+2, 0);
					}
				}
				else
				{
					return Point.Zero;
				}
			}
		}

		private bool HasLinkCommentButton
		{
			//	Indique s'il faut affiche le bouton pour montrer le commentaire.
			//	Si un commentaire est visible, il ne faut pas montrer le bouton, car il y a déjà
			//	le bouton CommentAttachTo pour déplacer le point d'attache.
			get
			{
				return this.comment == null || !this.comment.IsVisible;
			}
		}

		public Point PositionLinkComment
		{
			//	Retourne la position du bouton pour commenter la connexion, ou pour déplacer
			//	le point d'attache lorsque le commentaire existe.
			get
			{
				return this.AttachToPoint (this.commentAttach);
			}
		}

		private Point AttachToPoint(double d)
		{
			if (this.startVector.IsValid)
			{
				return Geometry.PointOnPath (this.Path, d);
			}
			else
			{
				return Point.Zero;
			}
		}

		public double PointToAttach(Point p)
		{
			if (this.startVector.IsValid)
			{
				return Geometry.OffsetOnPath (this.Path, p);
			}
			else
			{
				return 0;
			}
		}


		private bool HasSrcWithSingleLink
		{
			get
			{
				return this.srcObject is ObjectEdge;
			}
		}


		#region Magnet utilities
		private void MagnetMouseDown(Point pos, int rank)
		{
			if (rank == -1)
			{
				if (this.magnetList.Count >= ObjectLink.magnetMax)
				{
					return;
				}

				this.magnetList.Add (this.MagnetAbsToRel (this.MagnetHomePos));
				rank = this.magnetList.Count-1;

				this.hilitedElement = ActiveElement.LinkMagnet0+rank;
			}

			this.magnetDraggingRank = rank;
			this.isDraggingMagnet = true;
			this.SetPathDirty ();
			this.editor.LockObject (this);
			this.editor.Invalidate ();
		}

		private void MagnetMouseMove(Point pos)
		{
			if (this.IsNearMagnetHome (pos))
			{
				pos = this.MagnetHomePos;
			}

			this.SetMagnetAbsolutePos (this.magnetDraggingRank, pos);
			this.SetPathDirty ();
			this.editor.Invalidate ();
		}

		private void MagnetMouseUp(Point pos)
		{
			if (this.IsNearMagnetHome (pos))
			{
				this.magnetList.RemoveAt (this.magnetDraggingRank);
			}

			this.isDraggingMagnet = false;
			this.editor.LockObject (null);
			this.SetPathDirty ();
			this.editor.SetLocalDirty ();
			this.editor.Invalidate ();
		}


		private Path MagnetConstrainPath
		{
			get
			{
				var path = new Path ();

				path.MoveTo (this.startVector.Origin);
				path.LineTo (this.MagnetStartPos);

				for (int i = 0; i < this.magnetList.Count; i++)
				{
					path.LineTo (this.GetMagnetAbsolutePos (i));
				}

				path.LineTo (this.MagnetEndPos);
				path.LineTo (this.endVector.Origin);

				return path;
			}
		}


		private bool IsNearMagnetHome(Point pos)
		{
			double d = Point.Distance (this.MagnetHomePos, pos);

			return d < ObjectLink.magnetHomeMargin;
		}

		private Point MagnetHomePos
		{
			get
			{
				return this.AttachToPoint (0.1);
			}
		}

		private Point MagnetStartPos
		{
			get
			{
				Point dst;

				if (this.magnetList.Count == 0)
				{
					dst = this.endVector.Origin;
				}
				else
				{
					dst = this.GetMagnetAbsolutePos (0);
				}

				double d = Point.Distance (this.startVector.Origin, dst) * 0.5;
				return this.startVector.GetPoint (d);
			}
		}

		private Point MagnetEndPos
		{
			get
			{
				Point dst;

				if (this.magnetList.Count == 0)
				{
					dst = this.startVector.Origin;
				}
				else
				{
					dst = this.GetMagnetAbsolutePos (this.magnetList.Count-1);
				}

				double d = Point.Distance (this.endVector.Origin, dst) * 0.5;
				return this.endVector.GetPoint (d);
			}
		}

		private Point GetMagnetAbsolutePos(int rank)
		{
			if (rank < 0 || rank >= ObjectLink.magnetMax)
			{
				return Point.Zero;
			}

			return this.MagnetRelToAsb (this.magnetList[rank]);
		}

		private void SetMagnetAbsolutePos(int rank, Point pos)
		{
			if (rank < 0 || rank >= ObjectLink.magnetMax)
			{
				return;
			}

			this.magnetList[rank] = this.MagnetAbsToRel (pos);
		}

		private Point MagnetRelToAsb(Point pos)
		{
			if (this.startVector.IsZero || this.endVector.IsZero)
			{
				return Point.Zero;
			}

			double x, y;

			if (this.startVector.Origin.X == this.endVector.Origin.X)
			{
				x = this.startVector.Origin.X;
			}
			else
			{
				x = this.startVector.Origin.X + pos.X*(this.endVector.Origin.X-this.startVector.Origin.X);
			}

			if (this.startVector.Origin.Y == this.endVector.Origin.Y)
			{
				y = this.startVector.Origin.Y;
			}
			else
			{
				y = this.startVector.Origin.Y + pos.Y*(this.endVector.Origin.Y-this.startVector.Origin.Y);
			}

			return new Point (x, y);
		}

		private Point MagnetAbsToRel(Point pos)
		{
			if (this.startVector.IsZero || this.endVector.IsZero)
			{
				return Point.Zero;
			}

			double x, y;

			if (this.startVector.Origin.X == this.endVector.Origin.X)
			{
				x = 0.5;
			}
			else
			{
				x = (pos.X-this.startVector.Origin.X) / (this.endVector.Origin.X-this.startVector.Origin.X);
			}

			if (this.startVector.Origin.Y == this.endVector.Origin.Y)
			{
				y = 0.5;
			}
			else
			{
				y = (pos.Y-this.startVector.Origin.Y) / (this.endVector.Origin.Y-this.startVector.Origin.Y);
			}

			return new Point (x, y);
		}
		#endregion

		#region Draggind destination
		private void DraggingDstMouseDown(Point pos)
		{
			this.isDraggingDst = true;
			this.editor.LockObject (this);

			this.initialPath = this.Path;
			this.initialStartVector = this.startVector;
			this.initialEndVector = this.endVector;

			this.DraggingDstMouseMove(pos);
		}

		private void DraggingDstMouseMove(Point pos)
		{
			var obj = this.editor.DetectLinkableObject (pos, this.srcObject.GetType ());

			if (this.DraggingDstAlreadyLinked (obj))  // déjà une connexion sur cet objet ?
			{
				obj = null;  // on n'en veut pas une 2ème !
			}

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

			this.draggingStumpPos = pos;
			this.UpdateLink ();
			this.editor.Invalidate ();
		}

		private bool DraggingDstAlreadyLinked(LinkableObject candidate)
		{
			foreach (var link in this.srcObject.ObjectLinks)
			{
				if (link.dstObject == candidate)
				{
					return true;
				}
			}

			return false;
		}

		private void DraggingDstMouseUp()
		{
			this.isDraggingDst = false;
			this.draggingStumpPos = Point.Zero;
			this.initialPath = null;

			if (this.dstObject != null)
			{
				this.srcObject.RemoveEntityLink (this.dstObject);
			}

			this.dstObject = this.hilitedDstObject;

			if (this.hilitedDstObject != null)
			{
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
				if (this.magnetList.Count == 0 || !this.endVector.HasDirection)
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
				else
				{
					var points = new List<Point> ();

					points.Add (this.MagnetStartPos);

					for (int i = 0; i < this.magnetList.Count; i++)
					{
						points.Add (this.GetMagnetAbsolutePos (i));
					}

					points.Add (this.MagnetEndPos);

					path.MoveTo (this.startVector.Origin);

					for (int i = 0; i < points.Count; i++)
					{
						Point s = points[i];
						Point p;

						if (i < points.Count-1)
						{
							p = Point.Scale (points[i], points[i+1], 0.5);
						}
						else
						{
							p = this.endVector.Origin;
						}

						path.CurveTo (s, p);
					}
				}
			}

			return path;
		}
		#endregion


		private static readonly int				magnetMax = 10;
		private static readonly int				magnetHomeMargin = 10;

		private LinkableObject					srcObject;
		private LinkableObject					dstObject;

		private Vector							startVector;
		private Vector							endVector;
		private Path							path;

		private Vector							initialStartVector;
		private Vector							initialEndVector;
		private Path							initialPath;

		private List<Point>						magnetList;
		private bool							isDraggingMagnet;
		private int								magnetDraggingRank;

		private bool							isSrcHilited;
		private bool							isDraggingDst;
		private LinkableObject					hilitedDstObject;
		private Point							draggingStumpPos;

		private double							commentAttach;
		private ObjectComment					comment;
	}
}
