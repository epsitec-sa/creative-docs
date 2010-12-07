//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public abstract class LinkableObject : AbstractObject
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

		public ObjectInfo Info
		{
			//	Information liée.
			get
			{
				return this.info;
			}
			set
			{
				this.info = value;
			}
		}

		public bool IsExtended
		{
			//	Etat de la boîte (compact ou étendu).
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


		public override Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
			set
			{
				bounds = new Rectangle (Point.GridAlign (bounds.BottomLeft), Point.GridAlign (bounds.TopRight));

				Point p1 = this.bounds.TopLeft;
				this.bounds = value;
				Point p2 = this.bounds.TopLeft;

				//	S'il existe un commentaire associé, il doit aussi être déplacé.
				if (this.comment != null)
				{
					Rectangle rect = this.comment.InternalBounds;
					rect.Offset (p2-p1);
					this.comment.Bounds = rect;
				}

				//	S'il existe une information associée, elle doit aussi être déplacée.
				if (this.info != null)
				{
					Rectangle rect = this.info.InternalBounds;
					rect.Offset (p2-p1);
					this.info.Bounds = rect;
				}

				this.UpdateGeometry ();
			}
		}


		public virtual void UpdateObject()
		{
		}

		public virtual void SetBoundsAtEnd(Point start, Point end)
		{
		}


		public virtual void RemoveEntityLink(LinkableObject dst, bool isContinuation)
		{
		}

		public virtual void AddEntityLink(LinkableObject dst, bool isContinuation)
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

		public virtual double GetLinkAngle(Point pos, bool isDst)
		{
			return 0;
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
			//	Couleur de fond de la boîte.
			get
			{
				return this.colorFactory.ColorItem;
			}
			set
			{
				if (this.colorFactory.ColorItem != value)
				{
					this.colorFactory.ColorItem = value;

					//	Change la couleur de toutes les connexions liées.
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

				if (this.comment != null)
				{
					list.Add (this.comment);
				}

				if (this.info != null)
				{
					list.Add (this.info);
				}

				return list;
			}
		}


		public override bool MouseMove(Message message, Point pos)
		{
			base.MouseMove (message, pos);

			foreach (var obj in this.objectLinks)
			{
				if (obj.MouseMove (message, pos))
				{
					return true;
				}
			}

			return false;
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
			//	les connexions qui partent et qui arrivent, on se place à l'opposé.
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
				//?return 270;  // de haut en bas
				return 0;  // de gauche à droite
			}
			else
			{
				return Geometry.AngleAvg (angles) - 180;  // à l'opposé
			}
		}


		protected void DraggingMouseMove(Point pos)
		{
			this.editor.DetectMagnetConstrains (pos, this);

			var commentList = new List<Point> ();

			//	Mémorise la position de tous les liens qui partent.
			foreach (var obj in this.objectLinks)
			{
				if (obj.Comment != null)
				{
					commentList.Add (obj.PositionLinkComment);
				}
			}

			//	Mémorise la position de tous les liens qui arrivent.
			foreach (var obj in this.ArrivalObjectLinks)
			{
				if (obj.Comment != null)
				{
					commentList.Add (obj.PositionLinkComment);
				}
			}

			//	Déplace l'objet.
			Point center = Point.GridAlign (this.editor.MagnetConstrainCenter (pos-this.draggingOffset, this));
			this.Bounds = new Rectangle (center.X-this.Bounds.Width/2, center.Y-this.Bounds.Height/2, this.Bounds.Width, this.Bounds.Height);
			this.editor.UpdateLinks ();

			int i = 0;

			//	Déplace tous les liens qui partent.
			foreach (var obj in this.objectLinks)
			{
				if (obj.Comment != null)
				{
					Point p1 = commentList[i++];
					Point p2 = obj.PositionLinkComment;

					obj.Comment.Move (p2.X-p1.X, p2.Y-p1.Y);
				}
			}

			//	Déplace tous les liens qui arrivent.
			foreach (var obj in this.ArrivalObjectLinks)
			{
				if (obj.Comment != null)
				{
					Point p1 = commentList[i++];
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


		#region Serialize
		public override void Serialize(XElement xml)
		{
			base.Serialize (xml);
		}

		public override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);
		}
		#endregion


		protected readonly List<ObjectLink>		objectLinks;
		protected ObjectComment					comment;
		protected ObjectInfo					info;
		protected bool							isExtended;
	}
}
