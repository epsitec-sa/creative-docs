//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Xml;
using System.Xml.Serialization;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class LinkableObject : AbstractObject
	{
		public LinkableObject(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.objectLinks = new List<ObjectLink> ();
		}


		public virtual void CreateLinks()
		{
		}

		public virtual void UpdateLinks()
		{
		}


		public List<ObjectLink> ObjectLinks
		{
			get
			{
				return this.objectLinks;
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


		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la bo�te de l'objet.
			Point p1 = this.bounds.TopLeft;
			this.bounds = bounds;
			Point p2 = this.bounds.TopLeft;

			//	S'il existe un commentaire associ�, il doit aussi �tre d�plac�.
			if (this.comment != null)
			{
				Rectangle rect = this.comment.InternalBounds;
				rect.Offset (p2-p1);
				this.comment.SetBounds (rect);
			}
		}


		public virtual Vector GetLinkVector(LinkAnchor anchor, Point dstPos, bool isDst)
		{
			return Vector.Zero;
		}


		public override MainColor BackgroundMainColor
		{
			//	Couleur de fond de la bo�te.
			get
			{
				return this.boxColor;
			}
			set
			{
				if (this.boxColor != value)
				{
					this.boxColor = value;

					//	Change la couleur de toutes les connexions li�es.
					foreach (var obj in this.objectLinks)
					{
						obj.BackgroundMainColor = this.boxColor;
					}

					this.editor.Invalidate ();
					this.editor.SetLocalDirty ();
				}
			}
		}


		public override bool MouseMove(Message message, Point pos)
		{
			foreach (var obj in this.objectLinks)
			{
				if (obj.MouseMove (message, pos))
				{
					return true;
				}
			}

			return base.MouseMove (message, pos);
		}

		public override void MouseDown(Message message, Point pos)
		{
			base.MouseDown (message, pos);

			foreach (var obj in this.objectLinks)
			{
				obj.MouseDown (message, pos);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			base.MouseUp (message, pos);

			foreach (var obj in this.objectLinks)
			{
				obj.MouseUp (message, pos);
			}
		}

		public override bool MouseDetect(Point pos, out ActiveElement element, out int edgeRank)
		{
			foreach (var obj in this.objectLinks)
			{
				ActiveElement e;
				int r;

				if (obj.MouseDetect (pos, out e, out r))
				{
					element = e;
					edgeRank = r;
					return true;
				}
			}

			element = ActiveElement.None;
			edgeRank = -1;
			return false;
		}


		protected void DraggingMouseMove(Point pos)
		{
			var list = new List<Point> ();

			foreach (var obj in this.objectLinks)
			{
				if (obj.Comment != null)
				{
					list.Add (obj.PositionLinkComment);
				}
			}

			Rectangle bounds = this.editor.NodeGridAlign (new Rectangle (pos-this.draggingOffset, this.Bounds.Size));
			this.SetBounds (bounds);
			this.editor.UpdateLinks ();

			int i = 0;
			foreach (var obj in this.objectLinks)
			{
				if (obj.Comment != null)
				{
					Point p1 = list[i++];
					Point p2 = obj.PositionLinkComment;

					obj.Comment.Move (p2.X-p1.X, p2.Y-p1.Y);
				}
			}
		}


		protected void SetEdgesHilited(bool isHilited)
		{
			//	Modifie l'�tat 'hilited' de toutes les connexions qui partent de l'objet.
			//	Avec false, les petits cercles des liaisons ferm�es ne sont affich�s qu'� droite.
			if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				isHilited = false;
			}

			foreach (var obj in this.objectLinks)
			{
				obj.IsSrcHilited = isHilited;
			}
		}

		protected bool IsEdgeReadyForOpen()
		{
			//	Indique si l'une des connexions qui partent de l'objet est en mode EdgeOpen*.
			foreach (var obj in this.objectLinks)
			{
				ActiveElement ae = obj.HilitedElement;
				if (ae == ActiveElement.EdgeOpenLeft ||
					ae == ActiveElement.EdgeOpenRight)
				{
					return true;
				}
			}

			return false;
		}

		protected void DrawLinks(Graphics graphics)
		{
			foreach (var obj in this.objectLinks)
			{
				obj.DrawBackground (graphics);
			}
		}


		protected Rectangle						bounds;
		protected List<ObjectLink>				objectLinks;
		protected ObjectComment					comment;
		protected Point							draggingOffset;
	}
}
