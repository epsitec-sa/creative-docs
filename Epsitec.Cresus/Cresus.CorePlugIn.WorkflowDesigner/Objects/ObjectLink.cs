//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public class ObjectLink : AbstractObject
	{
		public ObjectLink(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.commentAttach = 0.1;
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
				value = System.Math.Max (value, 0.1);
				value = System.Math.Min (value, 0.9);

				this.commentAttach = value;
			}
		}


		public bool IsContinuation
		{
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

				bounds = Rectangle.Union (bounds, new Rectangle (this.startVector.Origin, Size.Zero));
				bounds = Rectangle.Union (bounds, new Rectangle (this.endVector.Origin, Size.Zero));

				bounds = Rectangle.Union (bounds, new Rectangle (this.CustomizeStartPos, Size.Zero));
				bounds = Rectangle.Union (bounds, new Rectangle (this.CustomizeEndPos, Size.Zero));

				return bounds;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.SetPathDirty ();
			this.UpdateVectors ();
			this.UpdateGeometry ();
		}


		public override void ContextMenu()
		{
			if (this.IsButtonEnable (ActiveElement.LinkComment))
			{
				this.editor.CreateMenuItem (null, this.comment == null ? "Ajoute un commentaire" : "Ferme le commentaire", "Link.Comment");
			}

			if (this.IsButtonEnable (ActiveElement.LinkCreateDst))
			{
				this.editor.CreateMenuSeparator ();

				if (this.srcObject is ObjectNode)
				{
					this.editor.CreateMenuItem (null, "Crée une nouvelle transition", "Link.CreateEdge");
				}
				else
				{
					this.editor.CreateMenuItem (null, "Crée un nouveau nœud privé", "Link.CreatePrivateNode");
					this.editor.CreateMenuItem (null, "Choisi un nœud public...",   "Link.CreatePublicNode");
				}
			}

			if (this.IsButtonEnable (ActiveElement.LinkClose))
			{
				this.editor.CreateMenuSeparator ();
				this.editor.CreateMenuItem (null, "Supprime le connexion", "Link.Delete");
			}
		}

		public override void MenuAction(string name)
		{
			switch (name)
			{
				case "Link.Comment":
					this.SwapComment ();
					break;

				case "Link.CreateEdge":
					this.CreateEdge ();
					break;

				case "Link.CreatePrivateNode":
					this.CreateNode ();
					break;

				case "Link.CreatePublicNode":
					this.CreatePublicNode ();
					break;

				case "Link.Delete":
					this.LinkClose ();
					break;
			}
		}

	
		public override List<AbstractObject> FriendObjects
		{
			//	Les objets amis sont les node/edge à chaque extrémité de la connexion.
			get
			{
				var list = new List<AbstractObject> ();

				list.Add (this.srcObject);

				if (this.dstObject != null)
				{
					list.Add (this.dstObject);
				}

				if (this.comment != null)
				{
					list.Add (this.comment);
				}

				return list;
			}
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			switch (element)
			{
				case ActiveElement.LinkChangeDst:
					if (this.dstObject == null)
					{
						return (this.srcObject is ObjectNode) ? "Connecte à une transition" : "Connecte à un nœud";
					}
					else
					{
						return (this.srcObject is ObjectNode) ? "Connecte à une autre transition" : "Connecte à un autre nœud";
					}

				case ActiveElement.LinkCreateDst:
					return (this.srcObject is ObjectNode) ? "Crée une nouvelle transition" : "Crée un nouveau nœud<br/>Ctrl+clic choisi un nœud public";

				case ActiveElement.LinkClose:
					return "Supprime la connexion";

				case ActiveElement.LinkComment:
					return "Ajoute un commentaire à la connexion";

				case ActiveElement.LinkCustomizeStart:
				case ActiveElement.LinkCustomizeEnd:
					return "Modifie l'aspect de la connexion";
			}

			return base.GetToolTipText(element);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	La souris est bougée.
			base.MouseMove (message, pos);

			if (this.draggingMode == DraggingMode.MoveLinkDst)
			{
				this.DraggingDstMouseMove (pos);
				return true;
			}
			else if (this.draggingMode == DraggingMode.MoveLinkCustomize)
			{
				this.CustomizeMouseMove (pos);
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);

			if (this.hilitedElement == ActiveElement.LinkChangeDst)
			{
				this.DraggingDstMouseDown (pos);
			}

			if (this.hilitedElement == ActiveElement.LinkCustomizeStart ||
				this.hilitedElement == ActiveElement.LinkCustomizeEnd)
			{
				this.CustomizeMouseDown (pos);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if (this.draggingMode == DraggingMode.MoveLinkDst)
			{
				this.DraggingDstMouseUp ();
			}

			if (this.draggingMode == DraggingMode.MoveLinkCustomize)
			{
				this.CustomizeMouseUp (pos);
			}

			if (this.hilitedElement == ActiveElement.LinkComment)
			{
				this.SwapComment();
			}

			if (this.hilitedElement == ActiveElement.LinkClose)
			{
				this.LinkClose ();
			}

			if (this.hilitedElement == ActiveElement.LinkCreateDst)
			{
				if (this.srcObject is ObjectNode)
				{
					this.CreateEdge ();
				}

				if (this.srcObject is ObjectEdge)
				{
					if (message.IsControlPressed)
					{
						this.CreatePublicNode ();
					}
					else
					{
						this.CreateNode ();
					}
				}
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (pos.IsZero || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
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
			if (pos.IsZero || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				return this.DetectButtons (pos);
			}

			return ActiveElement.None;
		}

		private bool DetectOver(Point pos, double margin)
		{
			//	Détecte si la souris est le long de la connexion.
			var rect = this.Bounds;
			rect.Inflate (margin*2);

			if (rect.Contains (pos) && this.IsUsablePath)
			{
				if (Geometry.DetectOutline (this.Path, margin, pos))
				{
					return true;
				}

				if (Geometry.DetectOutline (this.CustomizeConstrainPath, margin*2, pos))
				{
					return true;
				}
			}

			return false;
		}


		public override MouseCursorType MouseCursor
		{
			get
			{
				return MouseCursorType.Finger;
			}
		}


		private void CreateEdge()
		{
			var edgeEntity = this.editor.CreateEntity<WorkflowEdgeEntity> ();
			edgeEntity.Name = "Nouveau";

			var obj = new ObjectEdge (this.editor, edgeEntity);
			obj.ColorFartory.ColorItem = this.srcObject.ColorFartory.ColorItem;
			obj.ObjectLinks[0].SetStumpAngle (this.GetAngle ());

			this.dstObject = obj;
			this.srcObject.AddEntityLink (obj, this.IsContinuation);

			this.startManual = false;
			this.endManual = false;

			this.editor.EditableObject = obj;
			this.editor.AddEdge (obj);
			obj.SetBoundsAtEnd (this.startVector.Origin, this.endVector.Origin);
			this.editor.UpdateGeometry ();

			this.MoveObjectToFreeArea (obj, this.startVector.Origin, this.endVector.Origin);
		}

		private void CreateNode()
		{
			var nodeEntity = this.editor.CreateEntity<WorkflowNodeEntity> ();

			this.CreateNode (nodeEntity);
		}

		private void CreatePublicNode()
		{
			var dialog = new Dialogs.SelectPublicNodeDialog (this.editor, this.editor);
			dialog.IsModal = true;
			dialog.OpenDialog ();

			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return;
			}

			WorkflowNodeEntity publicEntity = dialog.NodeEntity;
			if (publicEntity == null)
			{
				return;
			}

			var nodeEntity = this.editor.CreateEntity<WorkflowNodeEntity> ();
			nodeEntity.Code = publicEntity.Code;
			nodeEntity.Name = publicEntity.Name;
			nodeEntity.IsForeign = true;

			this.CreateNode (nodeEntity);
		}

		private void CreateNode(WorkflowNodeEntity nodeEntity)
		{
			var obj = new ObjectNode (this.editor, nodeEntity);
			obj.ColorFartory.ColorItem = this.srcObject.ColorFartory.ColorItem;

			this.dstObject = obj;
			this.srcObject.AddEntityLink (obj, this.IsContinuation);

			this.startManual = false;
			this.endManual = false;

			this.editor.EditableObject = obj;
			this.editor.AddNode (obj);
			obj.SetBoundsAtEnd (this.startVector.Origin, this.endVector.Origin);
			this.editor.UpdateGeometry ();

			this.MoveObjectToFreeArea (obj, this.startVector.Origin, this.endVector.Origin);

			if (obj.Entity.IsForeign)
			{
				obj.SwapInfo ();  // montre la bulle des informations
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
			obj.Bounds = bounds;

			this.dstObject = obj;
			this.editor.UpdateAfterGeometryChanged (obj);
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


		public void SetStumpAngle(double angle)
		{
			this.startAngle = angle;
			this.endAngle = angle+180;

			this.startManual = false;
			this.endManual = false;

			this.SetPathDirty ();
			this.UpdateVectors ();
			this.UpdateDistances ();
			this.UpdateGeometry ();

			this.startManual = true;
			this.endManual = true;
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

			this.UpdateAngles ();
			this.UpdateVectors ();
			this.UpdateDistances ();
			this.UpdateGeometry ();
		}

		private void UpdateAngles()
		{
			Point src = this.srcObject.Bounds.Center;
			Point dst;

			if (this.draggingMode == DraggingMode.MoveLinkDst && this.hilitedDstObject != null)
			{
				dst = this.hilitedDstObject.Bounds.Center;
			}
			else if (this.draggingMode == DraggingMode.MoveLinkDst)
			{
				dst = this.draggingStumpPos;
			}
			else if (this.dstObject != null)
			{
				dst = this.dstObject.Bounds.Center;
			}
			else
			{
				dst = new Point (src.X+ObjectLink.lengthStumpLink, src.Y);  // moignon o--->
			}

			double angle = Point.ComputeAngleDeg (src, dst);

			if (!this.startManual)
			{
				this.startAngle = angle;
			}

			if (!this.endManual)
			{
				this.endAngle = angle+180;
			}
		}

		private void UpdateVectors()
		{
			this.startVector = this.srcObject.GetLinkVector (this.startAngle, isDst: false);

			if (this.draggingMode == DraggingMode.MoveLinkDst && this.hilitedDstObject != null)
			{
				this.endVector = this.hilitedDstObject.GetLinkVector (this.endAngle, isDst: true);
			}
			else if (this.draggingMode == DraggingMode.MoveLinkDst)
			{
				this.endVector = new Vector (this.draggingStumpPos, this.endAngle);
			}
			else if (this.dstObject != null)
			{
				this.endVector = this.dstObject.GetLinkVector (this.endAngle, isDst: true);
			}
			else
			{
				Vector s = this.startVector;
				this.endVector = new Vector (s.GetPoint (ObjectLink.lengthStumpLink), s.Origin);
			}

			this.endVectorArrow = this.AdjustEndVectorForArrow ();
		}

		private void UpdateDistances()
		{
			double d = Point.Distance (this.startVector.Origin, this.endVector.Origin) * 0.5;

			if (!this.startManual)
			{
				this.startDistance = d;
			}

			if (!this.endManual)
			{
				this.endDistance = d;
			}
		}

		public double GetAngleSrc()
		{
			//	Retourne l'angle que fait la connexion avec son objet source.
			return this.startAngle;
		}

		public double GetAngleDst()
		{
			//	Retourne l'angle que fait la connexion avec son objet destination.
			return this.endAngle;
		}

		public double GetAngle()
		{
			//	Retourne l'angle de la connexion.
			return Point.ComputeAngleDeg (this.startVector.Origin, this.endVector.Origin);
		}


		private void SwapComment()
		{
			//	Ajoute un commentaire à la connexion.
			if (this.comment == null)
			{
				this.comment = new ObjectComment(this.editor, this.entity);
				this.comment.AttachObject = this;

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

				this.comment.Bounds = rect;
				this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu

				this.editor.AddBalloon(this.comment);
				this.editor.UpdateAfterCommentChanged();
			}
			else
			{
				this.editor.RemoveBalloon (this.comment);
				this.comment = null;
			}

			this.editor.SetLocalDirty ();
		}

		private void LinkClose()
		{
			this.srcObject.ObjectLinks.Remove (this);

			if (!this.IsNoneDstObject)
			{
				this.srcObject.RemoveEntityLink (this.dstObject, this.IsContinuation);
			}

			this.editor.UpdateAfterGeometryChanged (null);
		}
		

		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine l'objet.
			base.DrawBackground (graphics);

			//	Dessine la connexion en blanc.
			if (this.IsUsablePath)
			{
				graphics.Rasterizer.AddOutline (this.Path, 6);
				graphics.RenderSolid (Color.FromBrightness (1));

				//	Dessine les contraintes utilisateur.
				if (this.dstObject != null && (this.draggingMode == DraggingMode.MoveLinkCustomize || this.IsHilite))
				{
					graphics.PaintDashedOutline (this.CustomizeConstrainPath, 1, 1, 4, CapStyle.Round, this.colorFactory.GetColorMain ());
				}

				//	Dessine la connexion et la flèche.
				Color color = (this.IsHilite || this.draggingMode == DraggingMode.MoveLinkDst) ? this.colorFactory.GetColorMain () : this.colorFactory.GetColor (0);

				if (this.draggingMode == DraggingMode.MoveLinkDst && this.hilitedDstObject == null)
				{
					graphics.PaintDashedOutline (this.Path, 2, 1, 4, CapStyle.Round, color);
				}
				else
				{
					if (this.IsContinuation)
					{
						graphics.PaintDashedOutline (this.Path, 2, 0, 4, CapStyle.Round, color);
					}
					else if (this.IsForkDash)
					{
						graphics.PaintDashedOutline (this.Path, 2, 5, 5, CapStyle.Round, color);
					}
					else
					{
						graphics.Rasterizer.AddOutline (this.Path, 2);
						graphics.RenderSolid (color);
					}
				}

				this.DrawArrow (graphics, color);
			}

			//	Dessine la pastille au départ.
			bool triangle = false;
			if (this.srcObject.AbstractEntity is WorkflowNodeEntity)
			{
				var nodeEntity = this.srcObject.AbstractEntity as WorkflowNodeEntity;
				if (nodeEntity.IsAuto)
				{
					triangle = true;
				}
			}

			if (this.IsContinuation)
			{
				this.DrawSquare (graphics, this.startVector.Origin, 4);
			}
			else if (triangle)
			{
				this.DrawTriangle (graphics, this.srcObject.Bounds.Center, this.startVector.Origin, 6);
			}
			else
			{
				this.DrawCircle (graphics, this.startVector.Origin, 4);
			}
		}

		private bool IsForkDash
		{
			get
			{
				//	Retourne true s'il s'agit d'une connexion entrante sur un edge de type fork.
				if (this.dstObject != null && this.dstObject is ObjectEdge)
				{
					var edge = this.dstObject as ObjectEdge;
					return edge.Entity.TransitionType == Core.Business.WorkflowTransitionType.Fork;
				}

				return false;
			}
		}

		private Vector AdjustEndVectorForArrow()
		{
			Point p = Point.Move (this.endVector.Origin, this.endVector.End, ObjectLink.arrowLength);
			return new Vector (p, this.endVector.Direction);
		}

		private void DrawArrow(Graphics graphics, Color color)
		{
			graphics.LineWidth = 2;

			Point e1 = Transform.RotatePointDeg (this.endVector.Origin,  ObjectLink.arrowAngle, this.endVectorArrow.Origin);
			Point e2 = Transform.RotatePointDeg (this.endVector.Origin, -ObjectLink.arrowAngle, this.endVectorArrow.Origin);

			graphics.AddLine (this.endVector.Origin, this.endVectorArrow.Origin);
			graphics.AddLine (this.endVector.Origin, e1);
			graphics.AddLine (this.endVector.Origin, e2);

			graphics.RenderSolid (color);
			graphics.LineWidth = 1;
		}

		private void DrawCircle(Graphics graphics, Point center, double radius)
		{
			//	Dessine un cercle vide.
			graphics.AddFilledCircle (center, radius);
			graphics.RenderSolid (this.colorFactory.GetColor (1));

			graphics.AddCircle (center, radius);
			graphics.RenderSolid (this.colorFactory.GetColor (0));
		}

		private void DrawSquare(Graphics graphics, Point center, double radius)
		{
			//	Dessine un carré vide.
			var rect = new Rectangle (new Point (center.X-radius, center.Y-radius), new Size (radius*2, radius*2));
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.colorFactory.GetColor (1));

			graphics.AddRectangle (rect);
			graphics.RenderSolid (this.colorFactory.GetColor (0));
		}

		private void DrawTriangle(Graphics graphics, Point origin, Point center, double dim)
		{
			//	Dessine un triangle vide, dont la base est dirigée vers l'origine.
			Point p1 = Point.Move (center, origin, -dim);
			Point pp = Point.Move (center, origin,  dim);
			Point v = center-pp;
			Point p2 = new Point (pp.X+v.Y, pp.Y-v.X);
			Point p3 = new Point (pp.X-v.Y, pp.Y+v.X);

			var path = new Path ();
			path.MoveTo (p1);
			path.LineTo (p2);
			path.LineTo (p3);
			path.Close ();

			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid (this.colorFactory.GetColor (1));

			graphics.Rasterizer.AddOutline (path, 1);
			graphics.RenderSolid (this.colorFactory.GetColor (0));
		}


		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine tous les boutons.
			base.DrawForeground (graphics);
		}


		private bool IsHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked || this.draggingMode == DraggingMode.MoveLinkDst)
				{
					return false;
				}

				return (this.hilitedElement == ActiveElement.LinkHilited ||
						this.hilitedElement == ActiveElement.LinkClose ||
						this.hilitedElement == ActiveElement.LinkComment ||
						this.hilitedElement == ActiveElement.LinkCustomizeStart ||
						this.hilitedElement == ActiveElement.LinkCustomizeEnd ||
						this.hilitedElement == ActiveElement.LinkChangeDst ||
						this.hilitedElement == ActiveElement.LinkCreateDst);
			}
		}


		private Point PositionLinkClose
		{
			get
			{
				if (!this.HasSrcWithSingleLink)
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
				return this.endVector.Origin;
			}
		}

		private Point PositionLinkCreateDst
		{
			//	Retourne la position du bouton pour créer la destination.
			get
			{
				if (this.IsNoneDstObject)
				{
					return Point.Move (this.endVector.Origin, this.endVector.End, -ActiveButton.buttonRadius*2+2);
				}
				else
				{
					return Point.Zero;
				}
			}
		}

		private bool HasLinkCommentButton
		{
			//	Indique s'il faut afficher le bouton pour montrer le commentaire.
			//	Si un commentaire est visible, il ne faut pas montrer le bouton, car il y a déjà
			//	le bouton CommentAttachTo pour déplacer le point d'attache.
			get
			{
				return this.dstObject != null && this.comment == null;
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
			return Geometry.PointOnPath (this.Path, d);
		}

		public double PointToAttach(Point p)
		{
			return Geometry.OffsetOnPath (this.Path, p);
		}


		private bool HasSrcWithSingleLink
		{
			get
			{
				return this.srcObject is ObjectEdge;
			}
		}


		#region Customize utilities
		private void CustomizeMouseDown(Point pos)
		{
			this.draggingMode = DraggingMode.MoveLinkCustomize;
			this.UpdateButtonsState ();
			this.SetPathDirty ();
			this.editor.Invalidate ();
		}

		private void CustomizeMouseMove(Point pos)
		{
			if (this.hilitedElement == ActiveElement.LinkCustomizeStart)
			{
				this.CustomizeStartPos = pos;
			}

			if (this.hilitedElement == ActiveElement.LinkCustomizeEnd)
			{
				this.CustomizeEndPos = pos;
			}

			this.SetPathDirty ();
			this.UpdateGeometry ();
			this.editor.Invalidate ();
		}

		private void CustomizeMouseUp(Point pos)
		{
			this.draggingMode = DraggingMode.None;
			this.UpdateButtonsState ();
			this.SetPathDirty ();
			this.UpdateGeometry ();
			this.editor.UpdateGeometry ();
			this.editor.SetLocalDirty ();
			this.editor.Invalidate ();
		}


		private Path CustomizeConstrainPath
		{
			get
			{
				var path = new Path ();

				path.MoveTo (this.startVector.Origin);
				path.LineTo (this.CustomizeStartPos);

				path.MoveTo (this.endVector.Origin);
				path.LineTo (this.CustomizeEndPos);

				return path;
			}
		}


		private Point CustomizeStartPos
		{
			get
			{
				return this.startVector.GetPoint (this.startDistance);
			}
			set
			{
				this.startAngle = this.srcObject.GetLinkAngle (value, isDst: false);
				this.UpdateVectors ();
				this.startDistance = Point.Distance (this.startVector.Origin, value);
				this.startManual = true;
			}
		}

		private Point CustomizeEndPos
		{
			get
			{
				return this.endVector.GetPoint (this.endDistance);
			}
			set
			{
				this.endAngle = this.dstObject.GetLinkAngle (value, isDst: true);
				this.UpdateVectors ();
				this.endDistance = Point.Distance (this.endVector.Origin, value);
				this.endManual = true;
			}
		}
		#endregion

		#region Draggind destination
		private void DraggingDstMouseDown(Point pos)
		{
			this.draggingMode = DraggingMode.MoveLinkDst;
			this.UpdateButtonsState ();
			this.startManual = false;
			this.endManual = false;

			this.DraggingDstMouseMove(pos);
		}

		private void DraggingDstMouseMove(Point pos)
		{
			var obj = this.editor.DetectLinkableObject (pos, filteredType: this.srcObject.GetType ());

			if (obj != this.dstObject && this.DraggingDstAlreadyLinked (obj))  // déjà une connexion sur cet objet ?
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
			this.draggingMode = DraggingMode.None;
			this.UpdateButtonsState ();
			this.draggingStumpPos = Point.Zero;

			if (this.dstObject != null)
			{
				this.srcObject.RemoveEntityLink (this.dstObject, this.IsContinuation);
			}

			this.dstObject = this.hilitedDstObject;

			if (this.hilitedDstObject == null)
			{
				this.startManual = false;
				this.endManual = false;

				this.SetPathDirty ();
				this.UpdateVectors ();
				this.UpdateDistances ();
				this.UpdateGeometry ();

				this.startManual = true;
				this.endManual = true;
			}
			else
			{
				this.srcObject.AddEntityLink (this.dstObject, this.IsContinuation);

				this.hilitedDstObject.IsHilitedForLinkChanging = false;
				this.hilitedDstObject = null;
			}

			this.editor.UpdateGeometry ();
			this.editor.SetLocalDirty ();
		}
		#endregion

		#region Path engine
		private void SetPathDirty()
		{
			this.path = null;
		}

		private bool IsUsablePath
		{
			//	Si les objets source et destination sont très proches ou se chevauchent, le chemin n'est plus utilisable.
			get
			{
				if (this.dstObject == null)
				{
					return true;
				}

				Rectangle rs = this.srcObject.Bounds;
				Rectangle rd = this.dstObject.Bounds;

				rs.Inflate (5);
				rd.Inflate (5);

				return !rs.IntersectsWith (rd);
			}
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

			path.MoveTo (this.startVector.Origin);
			path.CurveTo (this.CustomizeStartPos, this.CustomizeEndPos, this.endVectorArrow.Origin);

			return path;
		}
		#endregion


		protected override void CreateButtons()
		{
			this.buttons.Add (new ActiveButton (ActiveElement.LinkComment,        this.colorFactory, "C",                       this.UpdateButtonGeometryComment,        this.UpdateButtonStateComment));
			this.buttons.Add (new ActiveButton (ActiveElement.LinkClose,          this.colorFactory, GlyphShape.Close,          this.UpdateButtonGeometryClose,          this.UpdateButtonStateClose));
			this.buttons.Add (new ActiveButton (ActiveElement.LinkChangeDst,      this.colorFactory, GlyphShape.HorizontalMove, this.UpdateButtonGeometryChangeDst,      this.UpdateButtonStateChangeDst));
			this.buttons.Add (new ActiveButton (ActiveElement.LinkCreateDst,      this.colorFactory, GlyphShape.Plus,           this.UpdateButtonGeometryCreateDst,      this.UpdateButtonStateCreateDst));
			this.buttons.Add (new ActiveButton (ActiveElement.LinkCustomizeStart, this.colorFactory, "o",                       this.UpdateButtonGeometryCustomizeStart, this.UpdateButtonStateCustomizeStart));
			this.buttons.Add (new ActiveButton (ActiveElement.LinkCustomizeEnd,   this.colorFactory, "o",                       this.UpdateButtonGeometryCustomizeEnd,   this.UpdateButtonStateCustomizeEnd));
		}

		private void UpdateButtonGeometryClose(ActiveButton button)
		{
			button.Center = this.PositionLinkClose;
		}

		private void UpdateButtonGeometryComment(ActiveButton button)
		{
			button.Center = this.PositionLinkComment;
		}

		private void UpdateButtonGeometryChangeDst(ActiveButton button)
		{
			button.Center = this.PositionLinkChangeDst;
		}

		private void UpdateButtonGeometryCreateDst(ActiveButton button)
		{
			button.Center = this.PositionLinkCreateDst;
		}

		private void UpdateButtonGeometryCustomizeStart(ActiveButton button)
		{
			button.Center = this.CustomizeStartPos;
		}

		private void UpdateButtonGeometryCustomizeEnd(ActiveButton button)
		{
			button.Center = this.CustomizeEndPos;
		}

		private void UpdateButtonStateClose(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHilite && !this.IsDragging && !this.HasSrcWithSingleLink;
			button.State.Enable = button.State.Visible;
			button.State.Detectable = button.State.Visible;
		}

		private void UpdateButtonStateComment(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHilite && this.HasLinkCommentButton && !this.IsDragging && !this.IsTooShortLink;
			button.State.Enable = button.State.Visible;
			button.State.Detectable = button.State.Visible;
		}

		private void UpdateButtonStateChangeDst(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = button.State.Hilited || (this.IsHilite && !this.IsDragging);
		}

		private void UpdateButtonStateCreateDst(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHilite && !this.IsDragging && this.IsNoneDstObject;
			button.State.Enable = button.State.Visible;
		}

		private void UpdateButtonStateCustomizeStart(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.dstObject != null && ((this.hilitedElement == ActiveElement.LinkCustomizeStart || this.hilitedElement == ActiveElement.LinkCustomizeEnd) || (this.IsHilite && !this.IsDragging && !this.IsTooShortLink));
			button.State.Detectable = this.dstObject != null && !this.IsTooShortLink;
		}

		private void UpdateButtonStateCustomizeEnd(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.dstObject != null && ((this.hilitedElement == ActiveElement.LinkCustomizeStart || this.hilitedElement == ActiveElement.LinkCustomizeEnd) || (this.IsHilite && !this.IsDragging && !this.IsTooShortLink));
			button.State.Detectable = this.dstObject != null && !this.IsTooShortLink;
		}

		private bool IsTooShortLink
		{
			get
			{
				double d = Point.Distance (this.startVector.Origin, this.endVector.Origin);
				return d < 50;
			}
		}


		#region Serialize
		public override void Serialize(XElement xml)
		{
			base.Serialize (xml);

			xml.Add (new XAttribute ("src", (this.srcObject == null) ? 0 : this.srcObject.UniqueId));
			xml.Add (new XAttribute ("dst", (this.dstObject == null) ? 0 : this.dstObject.UniqueId));

			xml.Add (new XAttribute ("a1", Misc.Truncate (this.startAngle)));
			xml.Add (new XAttribute ("a2", Misc.Truncate (this.endAngle)));
			xml.Add (new XAttribute ("d1", Misc.Truncate (this.startDistance)));
			xml.Add (new XAttribute ("d2", Misc.Truncate (this.endDistance)));
			xml.Add (new XAttribute ("m1", this.startManual));
			xml.Add (new XAttribute ("m2", this.endManual));
			
			xml.Add (new XAttribute ("comment",  Misc.Truncate (this.commentAttach)));
			xml.Add (new XAttribute ("isCont",   this.IsContinuation));
		}

		public override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);

			this.srcObject = this.editor.Search ((int) xml.Attribute ("src")) as LinkableObject;
			this.dstObject = this.editor.Search ((int) xml.Attribute ("dst")) as LinkableObject;

			this.startAngle    = (double) xml.Attribute ("a1");
			this.endAngle      = (double) xml.Attribute ("a2");
			this.startDistance = (double) xml.Attribute ("d1");
			this.endDistance   = (double) xml.Attribute ("d2");
			this.startManual   = (bool)   xml.Attribute ("m1");
			this.endManual     = (bool)   xml.Attribute ("m2");
			this.commentAttach = (double) xml.Attribute ("comment");

			if (xml.Attribute ("isCont") != null)
			{
				this.IsContinuation = (bool) xml.Attribute ("isCont");
			}
		}
		#endregion


		private static readonly double				arrowLength = 12;
		private static readonly double				arrowAngle = 25;

		private LinkableObject						srcObject;
		private LinkableObject						dstObject;

		private double								startAngle;
		private double								endAngle;
		private double								startDistance;
		private double								endDistance;
		private Vector								startVector;
		private Vector								endVector;
		private Vector								endVectorArrow;
		private bool								startManual;
		private bool								endManual;
		private Path								path;

		private bool								isSrcHilited;
		private LinkableObject						hilitedDstObject;
		private Point								draggingStumpPos;

		private double								commentAttach;
		private ObjectComment						comment;
	}
}