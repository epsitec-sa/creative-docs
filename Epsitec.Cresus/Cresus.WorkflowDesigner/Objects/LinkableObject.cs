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


		public virtual void CreateInitialLinks()
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

		public bool IsExtended
		{
			//	Etat de la bo�te (compact ou �tendu).
			get
			{
				return this.isExtended;
			}
			set
			{
				if (this.isExtended != value)
				{
					this.isExtended = value;
					this.UpdateButtonsState ();
				}
			}
		}


		public virtual void SetBoundsAtEnd(Point start, Point end)
		{
		}

		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la bo�te de l'objet.
			bounds = new Rectangle(Point.GridAlign (bounds.BottomLeft), Point.GridAlign (bounds.TopRight));
			
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

			this.UpdateButtonsGeometry ();
		}


		public virtual void RemoveEntityLink(LinkableObject dst)
		{
		}

		public virtual void AddEntityLink(LinkableObject dst)
		{
		}


		public virtual Vector GetLinkVector(double angle, bool isDst)
		{
			return Vector.Zero;
		}

		public virtual Point GetLinkStumpPos(double angle)
		{
			return Point.Zero;
		}


		public string DebugInformationsObjectLinks
		{
			get
			{
				var builder = new System.Text.StringBuilder ();
				builder.Append ("( ObjectLinks: ");

				foreach (var obj in this.objectLinks)
				{
					builder.Append (obj.DebugInformationsBase);
					builder.Append (" ");
				}

				builder.Append (") ");
				return builder.ToString ();
			}
		}

	
		public override ColorItem BackgroundColorItem
		{
			//	Couleur de fond de la bo�te.
			get
			{
				return this.colorFactory.ColorItem;
			}
			set
			{
				if (this.colorFactory.ColorItem != value)
				{
					this.colorFactory.ColorItem = value;

					//	Change la couleur de toutes les connexions li�es.
					foreach (var obj in this.objectLinks)
					{
						obj.BackgroundColorItem = this.colorFactory.ColorItem;
					}

					this.editor.Invalidate ();
					this.editor.SetLocalDirty ();
				}
			}
		}


		public override List<AbstractObject> FriendObjects
		{
			//	Les objets amis sont toutes les connexions qui partent ou arrivent de cet objet.
			get
			{
				var list = new List<AbstractObject> ();

				foreach (var obj in this.objectLinks)
				{
					list.Add (obj);
				}

				foreach (var obj in this.ArrivalObjectLinks)
				{
					list.Add (obj);
				}

				return list;
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


		protected double ComputeBestStumpAngle()
		{
			//	Calcul le meilleur angle possible pour une nouvelle connexion. En tenant compte de toutes
			//	les connexions qui partent et qui arrivent, on se place � l'oppos�.
			var angles = new List<double> ();

			if (this.objectLinks != null)
			{
				//	Tient compte des liens qui partent.
				foreach (var link in this.objectLinks)
				{
					double angle = link.GetAngleSrc ();

					if (!double.IsNaN (angle))
					{
						angles.Add (angle);
					}
				}

				//	Tient compte des liens qui arrivent.
				foreach (var link in this.ArrivalObjectLinks)
				{
					double angle = link.GetAngleDst ();

					if (!double.IsNaN (angle))
					{
						angles.Add (angle);
					}
				}
			}

			if (angles.Count == 0)
			{
				return 270;  // de haut en bas
			}
			else
			{
				return Geometry.AngleAvg (angles) - 180;  // � l'oppos�
			}
		}


		protected void DraggingMouseMove(Point pos)
		{
			var list = new List<Point> ();

			//	M�morise la position de tous les liens qui partent.
			foreach (var obj in this.objectLinks)
			{
				if (obj.Comment != null)
				{
					list.Add (obj.PositionLinkComment);
				}
			}

			//	M�morise la position de tous les liens qui arrivent.
			foreach (var obj in this.ArrivalObjectLinks)
			{
				if (obj.Comment != null)
				{
					list.Add (obj.PositionLinkComment);
				}
			}

			//	D�place l'objet.
			Rectangle bounds = this.editor.NodeGridAlign (new Rectangle (pos-this.draggingOffset, this.Bounds.Size));
			this.SetBounds (bounds);
			this.editor.UpdateLinks ();

			int i = 0;

			//	D�place tous les liens qui partent.
			foreach (var obj in this.objectLinks)
			{
				if (obj.Comment != null)
				{
					Point p1 = list[i++];
					Point p2 = obj.PositionLinkComment;

					obj.Comment.Move (p2.X-p1.X, p2.Y-p1.Y);
				}
			}

			//	D�place tous les liens qui arrivent.
			foreach (var obj in this.ArrivalObjectLinks)
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
				if (this.objectLinks != null)
				{
					foreach (var obj in this.objectLinks)
					{
						if (obj.IsNoneDstObject)
						{
							return true;
						}
					}
				}

				return false;
			}
		}


		private IEnumerable<ObjectLink> ArrivalObjectLinks
		{
			//	Retourne la liste de toutes les connexions qui arrivent sur cet objet.
			//	A l'inverse, this.objectLinks donne la liste de toutes les connexions partantes.
			get
			{
				foreach (var link in this.editor.LinkObjects)
				{
					if (link.DstObject == this)
					{
						yield return link;
					}
				}
			}
		}


		protected Rectangle						bounds;
		protected List<ObjectLink>				objectLinks;
		protected ObjectComment					comment;
		protected bool							isExtended;
		protected Point							draggingOffset;
	}
}
