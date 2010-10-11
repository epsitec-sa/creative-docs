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
			this.links = new List<Link> ();

			this.linkListBt = new List<ObjectLink> ();
			this.linkListBb = new List<ObjectLink> ();
			this.linkListC  = new List<ObjectLink> ();
			this.linkListD  = new List<ObjectLink> ();
		}


		public virtual void CreateLinks()
		{
		}

		public virtual void UpdateLinks()
		{
		}

		public void ClearLinks()
		{
			foreach (Link link in this.links)
			{
				link.ObjectLink = null;
			}
		}


		public List<Link> Links
		{
			get
			{
				return this.links;
			}
		}


		public List<ObjectLink> LinkListBt
		{
			get
			{
				return this.linkListBt;
			}
		}

		public List<ObjectLink> LinkListBb
		{
			get
			{
				return this.linkListBb;
			}
		}

		public List<ObjectLink> LinkListC
		{
			get
			{
				return this.linkListC;
			}
		}

		public List<ObjectLink> LinkListD
		{
			get
			{
				return this.linkListD;
			}
		}


		public virtual double GetLinkSrcVerticalPosition(int index)
		{
			return this.Bounds.Center.Y;  // TODO: ...
		}

		public virtual Point GetLinkDstPosition(double posy, ObjectNode.EdgeAnchor anchor)
		{
			return this.Bounds.Center;  // TODO: ...
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
					foreach (Link link in this.links)
					{
						if (link.ObjectLink != null)
						{
							link.ObjectLink.BackgroundMainColor = this.boxColor;
						}
					}

					this.editor.Invalidate ();
					this.editor.SetLocalDirty ();
				}
			}
		}


		protected void SetEdgesHilited(bool isHilited)
		{
			//	Modifie l'état 'hilited' de toutes les connexions qui partent de l'objet.
			//	Avec false, les petits cercles des liaisons fermées ne sont affichés qu'à droite.
			if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				isHilited = false;
			}

			foreach (Link link in this.links)
			{
				if (link.ObjectLink != null)
				{
					link.ObjectLink.IsSrcHilited = isHilited;
				}
			}
		}

		protected bool IsEdgeReadyForOpen()
		{
			//	Indique si l'une des connexions qui partent de l'objet est en mode EdgeOpen*.
			foreach (Link link in this.links)
			{
				if (link.ObjectLink != null)
				{
					ActiveElement ae = link.ObjectLink.HilitedElement;
					if (ae == ActiveElement.EdgeOpenLeft ||
						ae == ActiveElement.EdgeOpenRight)
					{
						return true;
					}
				}
			}

			return false;
		}

		protected void DrawLinks(Graphics graphics)
		{
			foreach (var link in this.links)
			{
				link.ObjectLink.DrawBackground (graphics);
			}
		}


		protected List<Link>					links;
		private List<ObjectLink>				linkListBt;
		private List<ObjectLink>				linkListBb;
		private List<ObjectLink>				linkListC;
		private List<ObjectLink>				linkListD;
	}
}
