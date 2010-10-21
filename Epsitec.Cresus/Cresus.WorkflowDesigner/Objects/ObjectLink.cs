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
			this.UpdateButtonsGeometry ();
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
			if (this.isDraggingDst)
			{
				return null;  // pas de tooltip
			}

			switch (element)
			{
				case ActiveElement.LinkChangeDst:
					if (this.dstObject == null)
					{
						return (this.srcObject is ObjectNode) ? "Connecte à une transition" : "Connecte à un noeud";
					}
					else
					{
						return (this.srcObject is ObjectNode) ? "Connecte à une autre transition" : "Connecte à un autre noeud";
					}

				case ActiveElement.LinkCreateDst:
					return (this.srcObject is ObjectNode) ? "Crée une nouvelle transition" : "Crée un nouveau noeud";

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
			if (this.isDraggingDst)
			{
				this.DraggingDstMouseMove (pos);
				return true;
			}
			else if (this.isDraggingCustomize)
			{
				this.CustomizeMouseMove (pos);
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

			if (this.hilitedElement == ActiveElement.LinkCustomizeStart ||
				this.hilitedElement == ActiveElement.LinkCustomizeEnd)
			{
				this.CustomizeMouseDown (pos);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingDst)
			{
				this.DraggingDstMouseUp ();
			}

			if (this.isDraggingCustomize)
			{
				this.CustomizeMouseUp (pos);
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


		private void CreateEdge()
		{
			var edgeEntity = this.editor.BusinessContext.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			edgeEntity.Name = "Nouveau";

			var obj = new ObjectEdge (this.editor, edgeEntity);
			obj.ObjectLinks[0].SetStumpAngle (this.GetAngle ());

			this.dstObject = obj;
			this.srcObject.AddEntityLink (obj);

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
			var nodeEntity = this.editor.BusinessContext.DataContext.CreateEntity<WorkflowNodeEntity> ();

			var obj = new ObjectNode (this.editor, nodeEntity);

			this.dstObject = obj;
			this.srcObject.AddEntityLink (obj);

			this.startManual = false;
			this.endManual = false;

			this.editor.EditableObject = obj;
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


		public void SetStumpAngle(double angle)
		{
			this.startAngle = angle;
			this.endAngle = angle+180;

			this.startManual = false;
			this.endManual = false;

			this.SetPathDirty ();
			this.UpdateVectors ();
			this.UpdateDistances ();
			this.UpdateButtonsGeometry ();

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
			this.UpdateButtonsGeometry ();
		}

		private void UpdateAngles()
		{
			Point src = this.srcObject.Bounds.Center;

			Point dst;

			if (this.isDraggingDst && this.hilitedDstObject != null)
			{
				dst = this.hilitedDstObject.Bounds.Center;
			}
			else if (this.isDraggingDst)
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

			if (this.isDraggingDst && this.hilitedDstObject != null)
			{
				this.endVector = this.hilitedDstObject.GetLinkVector (this.endAngle, isDst: true);
			}
			else if (this.isDraggingDst)
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


		private void AddComment()
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

				this.comment.SetBounds(rect);
				this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment(this.comment);
				this.editor.UpdateAfterCommentChanged();
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
			if (this.IsUsablePath)
			{
				graphics.Rasterizer.AddOutline (this.Path, 6);
				graphics.RenderSolid (Color.FromBrightness (1));

				//	Dessine les contraintes utilisateur.
				if (this.dstObject != null && (this.isDraggingCustomize || this.IsHilite))
				{
					Misc.DrawPathDash (graphics, this.CustomizeConstrainPath, 1, 1, 4, true, this.colorFactory.GetColorMain ());
				}

				//	Dessine la connexion et la flèche.
				Color color = (this.IsHilite || this.isDraggingDst) ? this.colorFactory.GetColorMain () : this.colorFactory.GetColor (0);

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

			if (triangle)
			{
				this.DrawTriangle (graphics, this.srcObject.Bounds.Center, this.startVector.Origin, 6);
			}
			else
			{
				this.DrawCircle (graphics, this.startVector.Origin, 4);
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

		private void DrawCircle(Graphics graphics, Point center, double radius)
		{
			//	Dessine un cercle vide.
			graphics.AddFilledCircle (center, radius);
			graphics.RenderSolid (this.colorFactory.GetColor (1));

			graphics.AddCircle (center, radius);
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
			this.DrawButtons (graphics);
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
				return this.dstObject != null && (this.comment == null || !this.comment.IsVisible);
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
			this.isDraggingCustomize = true;
			this.UpdateButtonsState ();
			this.SetPathDirty ();
			this.editor.LockObject (this);
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
			this.UpdateButtonsGeometry ();
			this.editor.Invalidate ();
		}

		private void CustomizeMouseUp(Point pos)
		{
			this.isDraggingCustomize = false;
			this.UpdateButtonsState ();
			this.editor.LockObject (null);
			this.SetPathDirty ();
			this.UpdateButtonsGeometry ();
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
				this.startAngle = Point.ComputeAngleDeg (this.srcObject.Bounds.Center, value);
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
				this.endAngle = Point.ComputeAngleDeg (this.dstObject.Bounds.Center, value);
				this.UpdateVectors ();
				this.endDistance = Point.Distance (this.endVector.Origin, value);
				this.endManual = true;
			}
		}

#if false
		private Point CustomizeRelToAsb(Point pos)
		{
			Vector startVector = this.startVector;
			Vector endVector   = this.endVector;

			if (startVector.IsZero || endVector.IsZero)
			{
				return Point.Zero;
			}

			double x, y;

			if (startVector.Origin.X == endVector.Origin.X)
			{
				x = startVector.Origin.X;
			}
			else
			{
				x = startVector.Origin.X + pos.X*(endVector.Origin.X-startVector.Origin.X);
			}

			if (startVector.Origin.Y == endVector.Origin.Y)
			{
				y = startVector.Origin.Y;
			}
			else
			{
				y = startVector.Origin.Y + pos.Y*(endVector.Origin.Y-startVector.Origin.Y);
			}

			return new Point (x, y);
		}

		private Point CustomizeAbsToRel(Point pos)
		{
			Vector startVector = this.startVector;
			Vector endVector   = this.endVector;

			if (startVector.IsZero || endVector.IsZero)
			{
				return Point.Zero;
			}

			double x, y;

			if (startVector.Origin.X == endVector.Origin.X)
			{
				x = 0.5;
			}
			else
			{
				x = (pos.X-startVector.Origin.X) / (endVector.Origin.X-startVector.Origin.X);
			}

			if (startVector.Origin.Y == endVector.Origin.Y)
			{
				y = 0.5;
			}
			else
			{
				y = (pos.Y-startVector.Origin.Y) / (endVector.Origin.Y-startVector.Origin.Y);
			}

			return new Point (x, y);
		}
#endif
		#endregion

		#region Draggind destination
		private void DraggingDstMouseDown(Point pos)
		{
			this.isDraggingDst = true;
			this.UpdateButtonsState ();
			this.startManual = false;
			this.endManual = false;
			this.editor.LockObject (this);

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
			this.isDraggingDst = false;
			this.UpdateButtonsState ();
			this.draggingStumpPos = Point.Zero;

			if (this.dstObject != null)
			{
				this.srcObject.RemoveEntityLink (this.dstObject);
			}

			this.dstObject = this.hilitedDstObject;

			if (this.hilitedDstObject == null)
			{
				this.startManual = false;
				this.endManual = false;

				this.SetPathDirty ();
				this.UpdateVectors ();
				this.UpdateDistances ();
				this.UpdateButtonsGeometry ();

				this.startManual = true;
				this.endManual = true;
			}
			else
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
			path.CurveTo (this.CustomizeStartPos, this.CustomizeEndPos, this.endVector.Origin);

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
			button.State.Visible = this.IsHilite && !this.IsDragging;
		}

		private void UpdateButtonStateComment(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHilite && this.HasLinkCommentButton && !this.IsDragging && !this.IsTooShortLink;
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
			button.State.Visible = this.IsHilite && !this.IsDragging;
		}

		private void UpdateButtonStateCustomizeStart(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = (this.hilitedElement == ActiveElement.LinkCustomizeStart || this.hilitedElement == ActiveElement.LinkCustomizeEnd) || (this.IsHilite && this.dstObject != null && !this.IsDragging && !this.IsTooShortLink);
			button.State.Detectable = !this.IsTooShortLink;
		}

		private void UpdateButtonStateCustomizeEnd(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = (this.hilitedElement == ActiveElement.LinkCustomizeStart || this.hilitedElement == ActiveElement.LinkCustomizeEnd) || (this.IsHilite && this.dstObject != null && !this.IsDragging && !this.IsTooShortLink);
			button.State.Detectable = !this.IsTooShortLink;
		}

		private bool IsDragging
		{
			get
			{
				return this.isDraggingCustomize || this.isDraggingDst;
			}
		}

		private bool IsTooShortLink
		{
			get
			{
				double d = Point.Distance (this.startVector.Origin, this.endVector.Origin);
				return d < 50;
			}
		}


		private LinkableObject					srcObject;
		private LinkableObject					dstObject;

		private double							startAngle;
		private double							endAngle;
		private double							startDistance;
		private double							endDistance;
		private Vector							startVector;
		private Vector							endVector;
		private bool							startManual;
		private bool							endManual;
		private Path							path;

		private bool							isDraggingCustomize;

		private bool							isSrcHilited;
		private bool							isDraggingDst;
		private LinkableObject					hilitedDstObject;
		private Point							draggingStumpPos;

		private double							commentAttach;
		private ObjectComment					comment;
	}
}
