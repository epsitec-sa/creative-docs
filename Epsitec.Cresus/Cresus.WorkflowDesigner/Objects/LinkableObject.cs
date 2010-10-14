//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la boîte de l'objet.
			Point p1 = this.bounds.TopLeft;
			this.bounds = bounds;
			Point p2 = this.bounds.TopLeft;

			//	S'il existe un commentaire associé, il doit aussi être déplacé.
			if (this.comment != null)
			{
				Rectangle rect = this.comment.InternalBounds;
				rect.Offset (p2-p1);
				this.comment.SetBounds (rect);
			}
		}


		public bool IsHilitedForLinkChanging
		{
			//	Indique si cet objet est mis en évidence pendant un changement de destination d'une connexion.
			get
			{
				return this.isHilitedForLinkChanging;
			}
			set
			{
				this.isHilitedForLinkChanging = value;
			}
		}


		public virtual void RemoveEntityLink(LinkableObject dst)
		{
		}

		public virtual void AddEntityLink(LinkableObject dst)
		{
		}


		public virtual Vector GetLinkVector(LinkAnchor anchor, Point dstPos, bool isDst)
		{
			return Vector.Zero;
		}


		public override MainColor BackgroundMainColor
		{
			//	Couleur de fond de la boîte.
			get
			{
				return this.boxColor;
			}
			set
			{
				if (this.boxColor != value)
				{
					this.boxColor = value;

					//	Change la couleur de toutes les connexions liées.
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

			if (this.hilitedElement == ActiveElement.NodeOpenLink)
			{
				//	Crée un moignon de lien o--->
				var link = new ObjectLink (this.editor, this.entity);
				link.SrcObject = this;
				link.UpdateLink ();

				this.objectLinks.Add (link);
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			foreach (var obj in this.objectLinks)
			{
				ActiveElement element = obj.MouseDetectBackground (pos);

				if (element != ActiveElement.None)
				{
					return element;
				}
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			foreach (var obj in this.objectLinks)
			{
				ActiveElement element = obj.MouseDetectForeground (pos);

				if (element != ActiveElement.None)
				{
					return element;
				}
			}

			return ActiveElement.None;
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


		protected bool HasNoneDstObject
		{
			get
			{
				foreach (var obj in this.objectLinks)
				{
					if (obj.IsNoneDstObject)
					{
						return true;
					}
				}

				return false;
			}
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
		protected bool							isHilitedForLinkChanging;
	}
}
