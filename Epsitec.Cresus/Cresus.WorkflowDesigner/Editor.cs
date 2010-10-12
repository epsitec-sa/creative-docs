//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WorkflowDesigner.Objects;

using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	/// <summary>
	/// Widget permettant d'éditer graphiquement des entités.
	/// </summary>
	public class Editor : Widget, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public enum ModifyMode
		{
			Locked,
			Partial,
			Unlocked,
		}

		private enum MouseCursorType
		{
			Unknown,
			Arrow,
			Finger,
			Grid,
			Move,
			Hand,
			IBeam,
			Locate,
		}

		private enum PushDirection
		{
			Automatic,
			Left,
			Right,
			Bottom,
			Top,
		}


		public Editor()
		{
			this.AutoEngage = false;
			this.AutoFocus  = true;
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;

			this.nodes    = new List<ObjectNode> ();
			this.edges    = new List<ObjectEdge> ();
			this.comments = new List<ObjectComment> ();

			this.zoom = 1;
			this.areaOffset = Point.Zero;
			this.gridStep = 20;
			this.gridSubdiv = 5;
		}

		public Editor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public void SetBusinessContext(Core.Business.BusinessContext businessContext)
		{
			this.businessContext = businessContext;
		}

		public void SetWorkflowDefinitionEntity(WorkflowDefinitionEntity entity)
		{
			this.workflowDefinitionEntity = entity;
		}

		public void SetLocalDirty()
		{
		}

		public Core.Business.BusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public VScroller VScroller
		{
			get
			{
				return this.vscroller;
			}
			set
			{
				this.vscroller = value;
			}
		}


		public void CreateInitialWorkflow()
		{
			var firstNodeEntity = this.workflowDefinitionEntity as WorkflowNodeEntity;

			List<AbstractEntity> alreadyCreated = new List<AbstractEntity> ();
			this.CreateInitialWorkflow (alreadyCreated, firstNodeEntity, isRoot: true);

			foreach (var obj in this.LinkableObjects)
			{
				obj.CreateLinks ();
			}

			this.CreateLinks ();
			this.UpdateAfterGeometryChanged (null);
		}

		private void CreateInitialWorkflow(List<AbstractEntity> alreadyCreated, WorkflowEdgeEntity edgeEntity)
		{
			if (alreadyCreated.Contains (edgeEntity))
			{
				return;
			}

			alreadyCreated.Add (edgeEntity);

			var edge = new ObjectEdge (this, edgeEntity);
			this.AddEdge (edge);

			if (edgeEntity.NextNode.IsNotNull ())
			{
				this.CreateInitialWorkflow (alreadyCreated, edgeEntity.NextNode);
			}
		}

		private void CreateInitialWorkflow(List<AbstractEntity> alreadyCreated, WorkflowNodeEntity nodeEntity, bool isRoot = false)
		{
			if (alreadyCreated.Contains (nodeEntity))
			{
				return;
			}

			alreadyCreated.Add (nodeEntity);

			var node = new ObjectNode (this, nodeEntity);
			node.IsRoot = isRoot;
			this.AddNode (node);

			foreach (var edgeEntity in nodeEntity.Edges)
			{
				this.CreateInitialWorkflow (alreadyCreated, edgeEntity);
			}
		}


		public int NodeCount
		{
			//	Retourne le nombre de boîtes existantes.
			get
			{
				return this.nodes.Count;
			}
		}

		public List<ObjectNode> Nodes
		{
			//	Retourne la liste des boîtes.
			get
			{
				return this.nodes;
			}
		}


		public LinkableObject SearchObject(AbstractEntity entity)
		{
			//	Cherche un objet d'après l'entité qu'il représente.
			var searchedKey = this.businessContext.DataContext.GetNormalizedEntityKey (entity);

			foreach (var node in this.nodes)
			{
				var key = this.businessContext.DataContext.GetNormalizedEntityKey (node.AbstractEntity);

				if (key == searchedKey)
				{
					return node;
				}
			}

			foreach (var edge in this.edges)
			{
				var key = this.businessContext.DataContext.GetNormalizedEntityKey (edge.AbstractEntity);

				if (key == searchedKey)
				{
					return edge;
				}
			}

			return null;
		}


		public void AddNode(ObjectNode node)
		{
			//	Ajoute une nouvelle boîte dans l'éditeur.
			//	La position initiale n'a pas d'importance. La première boîte ajoutée (la boîte racine)
			//	est positionnée par RedimArea(). La position des autres est de toute façon recalculée en
			//	fonction de la boîte parent.
			node.SetBounds (new Rectangle (0, 0, ObjectNode.frameRadius*2, ObjectNode.frameRadius*2));
			this.nodes.Add (node);
		}

		public void AddEdge(ObjectEdge edge)
		{
			//	Ajoute une nouvelle liaison dans l'éditeur.
			edge.SetBounds (new Rectangle (Point.Zero, ObjectEdge.frameSize));
			this.edges.Add (edge);
		}

		public void AddComment(ObjectComment comment)
		{
			//	Ajoute un nouveau commentaire dans l'éditeur.
			this.comments.Add(comment);
		}


		public int GetNodeTitleNumbrer()
		{
			int number = 0;

			foreach (var node in this.nodes)
			{
				number = System.Math.Max (number, node.TitleNumber);
			}

			return number + 1;
		}


		public void Clear()
		{
			//	Supprime toutes les boîtes et toutes les liaisons de l'éditeur.
			this.nodes.Clear ();
			this.edges.Clear ();
			this.comments.Clear();
			this.LockObject(null);
		}


		public bool IsEditing
		{
			get
			{
				return this.editingObject != null;
			}
		}

		public AbstractObject EditingObject
		{
			get
			{
				return this.editingObject;
			}
			set
			{
				this.editingObject = value;
				this.OnEditingStateChanged ();
			}
		}


		public bool Grid
		{
			get
			{
				return this.grid;
			}
			set
			{
				if (this.grid != value)
				{
					this.grid = value;
					this.Invalidate ();
				}
			}
		}

		public Size AreaSize
		{
			//	Dimensions de la surface pour représenter les boîtes et les liaisons.
			get
			{
				return this.areaSize;
			}
			set
			{
				if (this.areaSize != value)
				{
					this.areaSize = value;
					this.Invalidate();
				}
			}
		}

		public double Zoom
		{
			//	Zoom pour représenter les boîtes et les liaisons.
			get
			{
				return this.zoom;
			}
			set
			{
				if (this.zoom != value)
				{
					this.zoom = value;
					this.Invalidate();
				}
			}
		}

		public Point AreaOffset
		{
			//	Offset de la zone visible, déterminée par les ascenseurs.
			get
			{
				return this.areaOffset;
			}
			set
			{
				if (this.areaOffset != value)
				{
					this.areaOffset = value;
					this.Invalidate();
				}
			}
		}

		public bool IsScrollerEnable
		{
			//	Indique si un (ou deux) ascenseurs sont actifs.
			get
			{
				return this.isScrollerEnable;
			}
			set
			{
				this.isScrollerEnable = value;
			}
		}


		public void UpdateGeometry()
		{
			//	Met à jour la géométrie de toutes les boîtes et de toutes les liaisons.
			this.UpdateNodes();
			this.UpdateLinks();
		}

		public void UpdateAfterCommentChanged()
		{
			//	Appelé lorsqu'un commentaire ou une information a changé.
			this.RedimArea();

			this.UpdateLinks();
			this.RedimArea();

			this.UpdateDimmed();
		}

		public void UpdateAfterGeometryChanged(ObjectNode node)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateNodes ();  // adapte la taille selon compact/étendu
			this.PushLayout (node, PushDirection.Automatic, this.gridStep);
			this.RedimArea ();

			this.UpdateLinks ();
			this.RedimArea ();

			this.UpdateDimmed ();
		}

		public void UpdateAfterMoving(LinkableObject node)
		{
			//	Appelé lorsqu'une boîte a été bougée.
			this.PushLayout (node, PushDirection.Automatic, this.gridStep);
			this.RedimArea ();

			this.UpdateLinks ();
			this.RedimArea ();
		}

		public void UpdateAfterAddOrRemoveEdge(LinkableObject node)
		{
			//	Appelé lorsqu'une liaison a été ajoutée ou supprimée.
			this.UpdateNodes ();
			this.PushLayout (node, PushDirection.Automatic, this.gridStep);
			this.RedimArea ();

			this.CreateLinks ();
			this.RedimArea ();

			this.UpdateLinks ();
			this.RedimArea ();

			this.UpdateDimmed ();
		}

		private void UpdateNodes()
		{
			//	Met à jour la géométrie de toutes les boîtes.
		}

		public void UpdateLinks()
		{
			//	Met à jour la géométrie de toutes les liaisons.
			this.CommentsMemorize();

			foreach (var obj in this.LinkableObjects)
			{
				for (int i=0; i<obj.Links.Count; i++)
				{
					Link link = obj.Links[i];

					ObjectLink objectLink = link.ObjectLink;
					if (objectLink != null)
					{
						objectLink.Points.Clear();

						if (link.DstNode == null)
						{
							link.RouteClear ();

							//	Important: toujours le point droite en premier !
							Point ps = obj.GetLinkSrcVerticalPosition (link.DstPos);
							objectLink.Points.Add (new Point (obj.Bounds.Right-1, ps.Y));
							objectLink.Points.Add (new Point (obj.Bounds.Left+1, ps.Y));
							objectLink.Link.Route = RouteType.Close;
						}
						else
						{
							this.UpdateLink (objectLink, obj, link.Index, link.DstNode);
						}
					}
				}
			}

			//	Réparti astucieusement le point d'arrivé en haut ou en bas d'une boîte de toutes les
			//	connexions de type Bt ou Bb, pour éviter que deux connexions n'arrivent sur le même point.
			//	Les croisements sont minimisés.
			foreach (var obj in this.LinkableObjects)
			{
				obj.LinkListBt.Clear();
				obj.LinkListBb.Clear();
				obj.LinkListC.Clear();
				obj.LinkListD.Clear();
			}

			foreach (Link link in this.Links)
			{
				ObjectLink objectLink = link.ObjectLink;
				if (objectLink != null)
				{
					if (link.DstNode != null && link.Route == RouteType.Bt)
					{
						link.DstNode.LinkListBt.Add (objectLink);
					}

					if (link.DstNode != null && link.Route == RouteType.Bb)
					{
						link.DstNode.LinkListBb.Add (objectLink);
					}

					if (link.DstNode != null && link.Route == RouteType.C)
					{
						link.DstNode.LinkListC.Add (objectLink);
					}

					if (link.DstNode != null && link.Route == RouteType.D)
					{
						link.DstNode.LinkListD.Add (objectLink);
					}
				}
			}

			foreach (var obj in this.LinkableObjects)
			{
				this.ShiftLinksB(obj, obj.LinkListBt);
				this.ShiftLinksB(obj, obj.LinkListBb);
				this.ShiftLinksC(obj, obj.LinkListC);
				this.ShiftLinksD(obj, obj.LinkListD);
			}

			foreach (var obj in this.LinkableObjects)
			{
				obj.LinkListBt.Clear();
				obj.LinkListBb.Clear();
				obj.LinkListC.Clear();
				obj.LinkListD.Clear();
			}

			//	Adapte tous les commentaires.
			foreach (ObjectComment comment in this.comments)
			{
				if (comment.AttachObject is ObjectLink)
				{
					var link = comment.AttachObject as ObjectLink;

					Point oldPos = link.Link.CommentPosition;
					Point newPos = link.PositionLinkComment;

					if (!oldPos.IsZero && !newPos.IsZero)
					{
						Rectangle rect = link.Link.CommentBounds;
						rect.Offset(newPos-oldPos);
						comment.SetBounds(rect);  // déplace le commentaire
					}
				}
			}

			this.Invalidate();
		}

		private void UpdateLink(ObjectLink link, LinkableObject src, int srcRank, LinkableObject dst)
		{
			//	Met à jour la géométrie d'une liaison.
			Rectangle srcBounds = src.Bounds;
			Rectangle dstBounds = dst.Bounds;

			//	Calcul des rectangles plus petits, pour les tests d'intersections.
			Rectangle srcBoundsLittle = srcBounds;
			Rectangle dstBoundsLittle = dstBounds;
			srcBoundsLittle.Deflate(2);
			dstBoundsLittle.Deflate(2);

			link.Points.Clear();
			link.Link.RouteClear();

			Point ps = src.GetLinkSrcVerticalPosition (link.Link.DstPos);
			if (src == dst)  // connexion à soi-même ?
			{
				Point p = new Point(srcBounds.Right-1, ps.Y);
				link.Points.Add(p);

				p.X += 30;
				link.Points.Add(p);

				p.Y -= 10;
				link.Points.Add(p);

				p.X -= 30;
				link.Points.Add(p);

				link.Link.Route = RouteType.Himself;
			}
			else if (!srcBounds.IntersectsWith(dstBounds))
			{
				if (dstBounds.Center.X > srcBounds.Right+Editor.edgeDetour/3)  // destination à droite ?
				{
					Point start = new Point(srcBounds.Right-1, ps.Y);
					link.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.edgeDetour)  // destination plus basse ?
					{
						Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Top);
						link.Points.Add(new Point(end.X, start.Y));
						link.Points.Add(end);
						link.Link.Route = RouteType.Bb;
					}
					else if (dstBounds.Bottom > start.Y+Editor.edgeDetour)  // destination plus haute ?
					{
						Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Bottom);
						link.Points.Add(new Point(end.X, start.Y));
						link.Points.Add(end);
						link.Link.Route = RouteType.Bt;
					}
					else
					{
						Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Left);
						if (start.Y != end.Y && end.X-start.X > Editor.edgeDetour)
						{
							link.Points.Add(Point.Zero);  // (*)
							link.Points.Add(Point.Zero);  // (*)
							link.Points.Add(end);
							link.Link.Route = RouteType.C;
						}
						else
						{
							link.Points.Add(end);
							link.Link.Route = RouteType.A;
						}
					}
				}
				else if (dstBounds.Center.X < srcBounds.Left-Editor.edgeDetour/3)  // destination à gauche ?
				{
					Point start = new Point(srcBounds.Left+1, ps.Y);
					link.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.edgeDetour)  // destination plus basse ?
					{
						Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Top);
						link.Points.Add(new Point(end.X, start.Y));
						link.Points.Add(end);
						link.Link.Route = RouteType.Bb;
					}
					else if (dstBounds.Bottom > start.Y+Editor.edgeDetour)  // destination plus haute ?
					{
						Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Bottom);
						link.Points.Add(new Point(end.X, start.Y));
						link.Points.Add(end);
						link.Link.Route = RouteType.Bt;
					}
					else
					{
						Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Right);
						if (start.Y != end.Y && start.X-end.X > Editor.edgeDetour)
						{
							link.Points.Add(Point.Zero);  // (*)
							link.Points.Add(Point.Zero);  // (*)
							link.Points.Add(end);
							link.Link.Route = RouteType.C;
						}
						else
						{
							link.Points.Add(end);
							link.Link.Route = RouteType.A;
						}
					}
				}
				else if (link.Link.IsAttachToRight)  // destination à droite à cheval ?
				{
					Point start = new Point(srcBounds.Right-1, ps.Y);
					Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Right);

					link.Points.Add(start);
					link.Points.Add(Point.Zero);  // (*)
					link.Points.Add(Point.Zero);  // (*)
					link.Points.Add(end);
					link.Link.Route = RouteType.D;
				}
				else  // destination à gauche à cheval ?
				{
					Point start = new Point(srcBounds.Left+1, ps.Y);
					Point end = dst.GetLinkDstPosition (start.Y, LinkAnchor.Left);

					link.Points.Add(start);
					link.Points.Add(Point.Zero);  // (*)
					link.Points.Add(Point.Zero);  // (*)
					link.Points.Add(end);
					link.Link.Route = RouteType.D;
				}
			}
		}

		// (*)	Sera calculé par ObjectEdge.UpdateRoute !

		private void ShiftLinksB(LinkableObject obj, List<ObjectLink> links)
		{
			//	Met à jour une liste de connexions de type Bt ou Bb, afin qu'aucune connexion
			//	n'arrive au même endroit.
			links.Sort(new Comparers.LinkComparer());  // tri pour minimiser les croisements

			double space = (obj.Bounds.Width/(links.Count+1.0))*0.75;

			for (int i=0; i<links.Count; i++)
			{
				ObjectLink link = links[i];

				int count = link.Points.Count;
				if (count > 2)
				{
					double dx = space * (i-(links.Count-1.0)/2);
					double px = link.Points[count-1].X+dx;

					if (link.IsRightDirection)
					{
						px = System.Math.Max(px, link.Points[0].X+8);
					}
					else
					{
						px = System.Math.Min(px, link.Points[0].X-8);
					}

					link.Points[count-1] = new Point(px, link.Points[count-1].Y);
					link.Points[count-2] = new Point(px, link.Points[count-2].Y);
					link.UpdateRoute();
				}
			}
		}

		private void ShiftLinksC(LinkableObject obj, List<ObjectLink> links)
		{
			//	Met à jour une liste de connexions de type C, afin qu'aucune connexion
			//	n'arrive au même endroit.
			links.Sort(new Comparers.LinkComparer());  // tri pour minimiser les croisements

			double spaceX = 5;
			double spaceY = 12;

			for (int i=0; i<links.Count; i++)
			{
				ObjectLink link = links[i];

				if (link.Points.Count == 4)
				{
					double dx = link.IsRightDirection ^ link.Points[0].Y > link.Points[link.Points.Count-1].Y ? spaceX*i : -spaceX*i;
					double dy = spaceY*i;
					link.Points[1] = new Point(link.Points[1].X+dx, link.Points[1].Y   );
					link.Points[2] = new Point(link.Points[2].X+dx, link.Points[2].Y-dy);
					link.Points[3] = new Point(link.Points[3].X,    link.Points[3].Y-dy);
				}
			}
		}

		private void ShiftLinksD(LinkableObject obj, List<ObjectLink> links)
		{
			//	Met à jour une liste de connexions de type D, afin qu'aucune connexion
			//	n'arrive au même endroit.
			links.Sort(new Comparers.LinkComparer());  // tri pour minimiser les croisements

			double spaceX = 5;
			double spaceY = 12;

			for (int i=0; i<links.Count; i++)
			{
				ObjectLink link = links[i];

				if (link.Points.Count == 4)
				{
					double dx = link.IsRightDirection ? spaceX*i : -spaceX*i;
					double dy = spaceY*i;
					link.Points[1] = new Point(link.Points[1].X+dx, link.Points[1].Y   );
					link.Points[2] = new Point(link.Points[2].X+dx, link.Points[2].Y-dy);
					link.Points[3] = new Point(link.Points[3].X,    link.Points[3].Y-dy);
				}
			}
		}


		public void CreateLinks()
		{
			//	Crée (ou recrée) toutes les liaisons nécessaires.
			this.CommentsMemorize();

			//	Supprime tous les commentaires liés aux connexions.
			int j = 0;
			while (j < this.comments.Count)
			{
				ObjectComment comment = this.comments[j];

				if (comment.AttachObject is LinkableObject)
				{
					this.comments.RemoveAt(j);
				}
				else
				{
					j++;
				}
			}

			foreach (var obj in this.LinkableObjects)
			{
				obj.ClearLinks ();  // supprime toutes les connexions existantes
			}

			foreach (var obj in this.LinkableObjects)
			{
				for (int i=0; i<obj.Links.Count; i++)
				{
					Link link = obj.Links[i];

					//	Si la liaison est ouverte sur une boîte qui n'existe plus,
					//	considère la liaison comme fermée !
					if (link.DstNode != null)
					{
						if (!this.LinkableObjects.Contains (link.DstNode))
						{
							link.DstNode = null;
						}
					}

					ObjectLink objectLink = new ObjectLink(this, obj.AbstractEntity);
					objectLink.Link = link;
					objectLink.BackgroundMainColor = obj.BackgroundMainColor;
					link.ObjectLink = objectLink;
				}
			}

			//	Recrée tous les commentaires liés aux connexions.
			foreach (var link in this.Links)
			{
				var objectLink = link.ObjectLink;

				if (objectLink != null && objectLink.Link.HasComment && objectLink.Link.DstNode != null)
				{
					objectLink.Comment = new ObjectComment (this, objectLink.AbstractEntity);
					objectLink.Comment.AttachObject = objectLink;
					objectLink.Comment.Text = objectLink.Link.CommentText;
					objectLink.Comment.BackgroundMainColor = objectLink.Link.CommentMainColor;
					objectLink.Comment.SetBounds (objectLink.Link.CommentBounds);

					this.AddComment (objectLink.Comment);
				}
			}

			this.Invalidate();
		}

		private void CommentsMemorize()
		{
			//	Mémorise l'état de tous les commentaires liés à des connexions.
			foreach (var link in this.Links)
			{
				link.HasComment = false;
			}

			foreach (ObjectComment comment in this.comments)
			{
				if (comment.AttachObject is ObjectLink)
				{
					ObjectLink objectLink = comment.AttachObject as ObjectLink;

					objectLink.Link.HasComment = true;
					objectLink.Link.CommentText = comment.Text;
					objectLink.Link.CommentMainColor = comment.BackgroundMainColor;

					Point pos = objectLink.PositionLinkComment;
					if (!pos.IsZero)
					{
						objectLink.Link.CommentPosition = pos;
					}

					if (!comment.Bounds.IsEmpty)
					{
						objectLink.Link.CommentBounds = comment.Bounds;
					}
				}
			}
		}

		private void UpdateDimmed()
		{
			//	Met en estompé toutes les connexions qui partent ou qui arrivent sur une entité estompée.
			foreach (var link in this.Links)
			{
				var objectLink = link.ObjectLink;
				if (objectLink != null)
				{
					objectLink.IsDimmed = false;
				}
			}

			foreach (var link in this.Links)
			{
				var objectLink = link.ObjectLink;
				if (objectLink != null)
				{
					if (objectLink.Link.DstNode != null)
					{
						if (objectLink.Link.SrcNode != null && objectLink.Link.SrcNode.IsDimmed)
						{
							objectLink.IsDimmed = true;
						}
						else if (objectLink.Link.DstNode != null && objectLink.Link.DstNode.IsDimmed)
						{
							objectLink.IsDimmed = true;
						}
					}

					if (objectLink.Comment != null)
					{
						objectLink.Comment.IsDimmed = objectLink.IsDimmed;
					}
				}
			}

			foreach (var obj in this.LinkableObjects)
			{
				if (obj.Comment != null)
				{
					obj.Comment.IsDimmed = obj.IsDimmed;
				}
			}
		}


		public bool IsEmptyArea(Rectangle area)
		{
			//	Retourne true si une zone est entièrement vide (aucune boîte, on ignore les connexions).
			foreach (var obj in this.LinkableObjects)
			{
				if (obj.Bounds.IntersectsWith (area))
				{
					return false;
				}
			}

			return true;
		}


		public void CloseObject(LinkableObject obj)
		{
			//	Ferme une boîte.
			if (obj is ObjectNode)
			{
				this.nodes.Remove (obj as ObjectNode);
			}

			if (obj is ObjectEdge)
			{
				this.edges.Remove (obj as ObjectEdge);
			}

			this.SetLocalDirty ();
		}


		private void PushLayout(LinkableObject exclude, PushDirection direction, double margin)
		{
			//	Pousse les boîtes pour éviter tout chevauchement.
			//	Une boîte peut être poussée hors de la surface de dessin.
			for (int max=0; max<100; max++)
			{
				bool push = false;

				foreach (var obj in this.LinkableObjects)
				{
					var inter = this.PushSearch (obj, exclude, margin);
					if (inter != null)
					{
						push = true;
						this.PushAction (obj, inter, direction, margin);
						this.PushLayout (inter, direction, margin);
					}
				}

				if (!push)
				{
					break;
				}
			}
		}

		private LinkableObject PushSearch(LinkableObject node, LinkableObject exclude, double margin)
		{
			//	Cherche une boîte qui chevauche 'node'.
			Rectangle rect = node.Bounds;
			rect.Inflate (margin);

			foreach (var obj in this.LinkableObjects)
			{
				if (obj != node && obj != exclude)
				{
					if (obj.Bounds.IntersectsWith (rect))
					{
						return obj;
					}
				}
			}

			return null;
		}

		private void PushAction(LinkableObject node, LinkableObject inter, PushDirection direction, double margin)
		{
			//	Pousse 'inter' pour venir après 'node' selon la direction choisie.
			Rectangle rect = inter.Bounds;

			double dr = node.Bounds.Right - rect.Left + margin;
			double dl = rect.Right - node.Bounds.Left + margin;
			double dt = node.Bounds.Top - rect.Bottom + margin;
			double db = rect.Top - node.Bounds.Bottom + margin;

			if (direction == PushDirection.Automatic)
			{
				double min = System.Math.Min (System.Math.Min (dr, dl), System.Math.Min (dt, db));

				if (min == dr)
				{
					direction = PushDirection.Right;
				}
				else if (min == dl)
				{
					direction = PushDirection.Left;
				}
				else if (min == dt)
				{
					direction = PushDirection.Top;
				}
				else
				{
					direction = PushDirection.Bottom;
				}
			}

			if (direction == PushDirection.Right)
			{
				rect.Offset (dr, 0);
			}

			if (direction == PushDirection.Left)
			{
				rect.Offset (-dl, 0);
			}

			if (direction == PushDirection.Top)
			{
				rect.Offset (0, dt);
			}

			if (direction == PushDirection.Bottom)
			{
				rect.Offset (0, -db);
			}

			inter.SetBounds (rect);
		}


		private void RedimArea()
		{
			//	Recalcule les dimensions de la surface de travail, en fonction du contenu.
			Rectangle rect = this.ComputeObjectsBounds();
			rect.Inflate(Editor.frameMargin);

			bool iGrid = this.grid;
			this.grid = true;
			rect = this.AreaGridAlign (rect);
			this.grid = iGrid;

			this.MoveObjects(-rect.Left, -rect.Bottom);

			this.AreaSize = rect.Size;
			this.OnAreaSizeChanged();
		}

		private Rectangle ComputeObjectsBounds()
		{
			//	Retourne le rectangle englobant tous les objets.
			Rectangle bounds = Rectangle.Empty;

			foreach (var obj in this.LinkableObjects)
			{
				bounds = Rectangle.Union (bounds, obj.Bounds);
			}

			foreach (var obj in this.LinkObjects)
			{
				bounds = Rectangle.Union (bounds, obj.Bounds);
			}

			foreach (ObjectComment comment in this.comments)
			{
				bounds = Rectangle.Union(bounds, comment.Bounds);
			}

			return bounds;
		}

		private void MoveObjects(double dx, double dy)
		{
			//	Déplace tous les objets.
			if (dx == 0 && dy == 0)  // immobile ?
			{
				return;
			}

			foreach (var obj in this.LinkableObjects)
			{
				obj.Move (dx, dy);
			}

			foreach (var obj in this.LinkObjects)
			{
				obj.Move (dx, dy);
			}

			foreach (ObjectComment comment in this.comments)
			{
				comment.Move(dx, dy);
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			this.brutPos = pos;
			pos = this.ConvWidgetToEditor(pos);

			this.lastMessageType = message.MessageType;
			this.lastMessagePos = pos;

			//-System.Diagnostics.Debug.WriteLine(string.Format("Type={0}", message.MessageType));

			switch (message.MessageType)
			{
				case MessageType.KeyDown:
				case MessageType.KeyUp:
					//	Ne consomme l'événement que si on l'a bel et bien reconnu ! Evite
					//	qu'on ne mange les raccourcis clavier généraux (Alt-F4, CTRL-S, ...)
					if (this.IsEditing)  // édition en cours ?
					{
						if (message.KeyCode == KeyCode.Return)
						{
							this.editingObject.AcceptEdition ();
							message.Consumer = this;
						}
						if (message.KeyCode == KeyCode.Escape)
						{
							this.editingObject.CancelEdition ();
							message.Consumer = this;
						}
					}
					break;

				case MessageType.MouseMove:
					this.EditorMouseMove(message, pos);
					message.Consumer = this;
					break;

				case MessageType.MouseDown:
					this.EditorMouseDown(message, pos);
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.EditorMouseUp(message, pos);
					message.Consumer = this;
					break;

				case MessageType.MouseLeave:
					this.EditorMouseMove(message, Point.Zero);
					break;

				case MessageType.MouseWheel:
					if (message.IsControlPressed)
					{
						double zoom = this.zoom;
						if (message.Wheel < 0)  zoom -= 0.1;
						if (message.Wheel > 0)  zoom += 0.1;
						zoom = System.Math.Max (zoom, MainController.zoomMin);
						zoom = System.Math.Min (zoom, MainController.zoomMax);
						if (this.zoom != zoom)
						{
							this.Zoom = zoom;
							this.OnZoomChanged();
						}
					}
					else
					{
						if (message.Wheel < 0)  this.vscroller.Value += this.vscroller.SmallChange;
						if (message.Wheel > 0)  this.vscroller.Value -= this.vscroller.SmallChange;
					}
					break;
			}
		}

		private Point ConvWidgetToEditor(Point pos)
		{
			//	Conversion d'une coordonnée dans l'espace normal des widgets vers l'espace de l'éditeur,
			//	qui varie selon les ascenseurs (AreaOffset) et le zoom.
			pos.Y = this.Client.Size.Height-pos.Y;
			pos /= this.zoom;
			pos += this.areaOffset;
			pos.Y = this.areaSize.Height-pos.Y;

			return pos;
		}

		public Point ConvEditorToWidget(Point pos)
		{
			//	Conversion d'une coordonnée dans l'espace de l'éditeur vers l'espace normal des widgets.
			pos.Y = this.areaSize.Height-pos.Y;
			pos -= this.areaOffset;
			pos *= this.zoom;
			pos.Y = this.Client.Size.Height-pos.Y;

			return pos;
		}

		private void EditorMouseMove(Message message, Point pos)
		{
			//	Met en évidence tous les widgets selon la position visée par la souris.
			//	L'objet à l'avant-plan a la priorité.
			if (this.IsEditing)  // édition en cours ?
			{
				this.ChangeMouseCursor (MouseCursorType.Arrow);
				return;
			}

			if (message.MessageType == MessageType.MouseMove &&
				Message.CurrentState.Buttons == MouseButtons.None)
			{
				ToolTip.Default.RefreshToolTip(this, this.brutPos);
			}

			if (this.isAreaMoving)
			{
				Point offset = new Point();
				offset.X = this.areaMovingInitialOffset.X-(this.brutPos.X-this.areaMovingInitialPos.X)/this.zoom;
				offset.Y = this.areaMovingInitialOffset.Y+(this.brutPos.Y-this.areaMovingInitialPos.Y)/this.zoom;
				this.AreaOffset = offset;
				this.OnAreaOffsetChanged();
			}
			else if (this.lockObject != null)
			{
				this.lockObject.MouseMove(message, pos);
			}
			else
			{
				AbstractObject fly = null;

				for (int i=this.comments.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.comments[i];
					if (obj.MouseMove(message, pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				for (int i=this.edges.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.edges[i];
					if (obj.MouseMove (message, pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				for (int i=this.nodes.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.nodes[i];
					if (obj.MouseMove (message, pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				MouseCursorType type = MouseCursorType.Unknown;

				if (fly == null)
				{
					if (this.IsScrollerEnable)
					{
						type = MouseCursorType.Hand;
					}
					else
					{
						type = MouseCursorType.Arrow;
					}
				}
				else
				{
					if (fly.HilitedElement == ActiveElement.NodeHeader)
					{
						if (this.IsLocateActionHeader(message))
						{
							var node = fly as ObjectNode;
							if (node != null && !node.IsRoot)
							{
								type = MouseCursorType.Locate;
							}
							else
							{
								type = MouseCursorType.Arrow;
							}
						}
						else
						{
							if (this.NodeCount > 1)
							{
								type = MouseCursorType.Move;
							}
							else
							{
								type = MouseCursorType.Arrow;
							}
						}
					}
					else if (fly.HilitedElement == ActiveElement.None ||
							 fly.HilitedElement == ActiveElement.NodeInside ||
							 fly.HilitedElement == ActiveElement.EdgeHilited)
					{
						type = MouseCursorType.Arrow;
					}
					else if (fly.HilitedElement == ActiveElement.NodeEdgeName ||
							 fly.HilitedElement == ActiveElement.NodeEdgeType ||
							 fly.HilitedElement == ActiveElement.NodeEdgeExpression)
					{
						if (this.IsLocateAction(message))
						{
							type = MouseCursorType.Locate;
						}
						else
						{
							if (fly.IsMousePossible(fly.HilitedElement, fly.HilitedEdgeRank))
							{
								type = MouseCursorType.IBeam;
							}
							else
							{
								type = MouseCursorType.Arrow;
							}
						}
					}
					else if (fly.HilitedElement == ActiveElement.NodeEdgeTitle)
					{
						if (this.IsLocateAction(message))
						{
							type = MouseCursorType.Locate;
						}
						else
						{
							type = MouseCursorType.Arrow;
						}
					}
					else if (fly.HilitedElement == ActiveElement.CommentEdit)
					{
						type = MouseCursorType.IBeam;
					}
					else if (fly.HilitedElement == ActiveElement.CommentMove ||
							 fly.HilitedElement == ActiveElement.InfoMove)
					{
						type = MouseCursorType.Move;
					}
					else if (fly.HilitedElement == ActiveElement.InfoEdit)
					{
						type = MouseCursorType.Arrow;
					}
					else
					{
						type = MouseCursorType.Finger;
					}
				}

				this.ChangeMouseCursor(type);
				this.hilitedObject = fly;
			}
		}

		private void EditorMouseDown(Message message, Point pos)
		{
			//	Début du déplacement d'une boîte.
			if (this.lastCursor == MouseCursorType.Hand)
			{
				this.isAreaMoving = true;
				this.areaMovingInitialPos = this.brutPos;
				this.areaMovingInitialOffset = this.areaOffset;
			}
			else
			{
				if (this.IsEditing)  // édition en cours ?
				{
					this.editingObject.AcceptEdition ();
					this.editingObject = null;

					this.Invalidate ();
				}

				AbstractObject obj = this.GetObjectForAction ();
				if (obj != null)
				{
					obj.MouseDown(message, pos);
				}
			}
		}

		private void EditorMouseUp(Message message, Point pos)
		{
			//	Fin du déplacement d'une boîte.
			if (this.isAreaMoving)
			{
				this.isAreaMoving = false;
			}
			else
			{
				AbstractObject obj = this.GetObjectForAction ();
				if (obj != null)
				{
					obj.MouseUp(message, pos);
				}
			}
		}


		public Rectangle NodeGridAlign(Rectangle rect)
		{
			//	Aligne un rectangle d'une boîte (ObjectNode) sur son coin supérieur/gauche,
			//	en ajustant également sa largeur, mais pas sa hauteur.
			if (this.grid)
			{
				Point topLeft = this.GridAlign (rect.TopLeft);
				double width  = this.GridAlign (rect.Width);

				rect = new Rectangle (topLeft.X, topLeft.Y-rect.Height, width, rect.Height);
			}

			return rect;
		}

		private Rectangle AreaGridAlign(Rectangle rect)
		{
			if (this.grid)
			{
				Point bottomLeft = this.GridAlign (rect.BottomLeft);
				double width     = this.GridAlign (rect.Width);
				double height    = this.GridAlign (rect.Height);

				rect = new Rectangle (bottomLeft.X, bottomLeft.Y, width, height);
			}

			return rect;
		}

		public Point GridAlign(Point pos)
		{
			if (this.grid)
			{
				pos = Point.GridAlign (pos, 0, this.gridStep);
			}

			return pos;
		}

		public double GridAlign(double value)
		{
			if (this.grid)
			{
				value = Point.GridAlign (new Point (value, 0), 0, this.gridStep).X;
			}

			return value;
		}


		public bool IsLocateAction(Message message)
		{
			//	Indique si l'action débouche sur une opération de navigation.
			return (message.IsControlPressed || this.CurrentModifyMode != ModifyMode.Unlocked);
		}

		public bool IsLocateActionHeader(Message message)
		{
			//	Indique si l'action débouche sur une opération de navigation (pour NodeHeader).
			return (message.IsControlPressed || this.CurrentModifyMode == ModifyMode.Locked);
		}

		public ModifyMode CurrentModifyMode
		{
			//	Retourne le mode de travail courant.
			get
			{
				return ModifyMode.Unlocked;
			}
		}


		public void LockObject(AbstractObject obj)
		{
			//	Indique l'objet en cours de drag.
			this.lockObject = obj;
		}

		private AbstractObject GetObjectForAction()
		{
			//	L'objet à l'avant-plan a la priorité.
			for (int i=this.comments.Count-1; i>=0; i--)
			{
				ObjectComment comment = this.comments[i];

				if (comment.IsReadyForAction)
				{
					return comment;
				}
			}

			foreach (var obj in this.LinkableObjects)
			{
				foreach (var link in obj.Links)
				{
					if (link.ObjectLink != null && link.ObjectLink.IsReadyForAction)
					{
						return obj;
					}
				}
			}

			for (int i=this.edges.Count-1; i>=0; i--)
			{
				ObjectEdge edge = this.edges[i];

				if (edge.IsReadyForAction)
				{
					return edge;
				}
			}

			for (int i=this.nodes.Count-1; i>=0; i--)
			{
				ObjectNode node = this.nodes[i];

				if (node.IsReadyForAction)
				{
					return node;
				}
			}

			return null;
		}



		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect;

			Transform initialTransform = graphics.Transform;
			graphics.TranslateTransform(-this.areaOffset.X*this.zoom, this.Client.Bounds.Height-(this.areaSize.Height-this.areaOffset.Y)*this.zoom);
			graphics.ScaleTransform(this.zoom, this.zoom, 0, 0);

			//	Dessine la surface de dessin.
			rect = new Rectangle(0, 0, this.areaSize.Width, this.areaSize.Height);
			graphics.AddFilledRectangle(rect);  // surface de dessin
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine la grille.
			if (this.grid)
			{
				this.PaintGrid (graphics, clipRect);
			}

			//	Dessine les surfaces hors de la zone utile.
			Point bl = this.ConvWidgetToEditor(this.Client.Bounds.BottomLeft);
			Point tr = this.ConvWidgetToEditor(this.Client.Bounds.TopRight);

			rect = new Rectangle(this.areaSize.Width, bl.Y, tr.X-this.areaSize.Width, tr.Y-bl.Y);
			if (!rect.IsSurfaceZero)
			{
				graphics.AddFilledRectangle(rect);  // à droite
			}
			
			rect = new Rectangle(0, bl.Y, this.areaSize.Width, -bl.Y);
			if (!rect.IsSurfaceZero)
			{
				graphics.AddFilledRectangle(rect);  // en bas
			}

			Color colorOver = Color.FromAlphaColor(0.3, adorner.ColorBorder);
			graphics.RenderSolid(colorOver);

			this.PaintObjects (graphics);

			graphics.Transform = initialTransform;

			//	Dessine le cadre.
			rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}

		private void PaintGrid(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine la grille magnétique.
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.zoom;

			double ix = 0.5/this.zoom;
			double iy = 0.5/this.zoom;

			int mul = (int) System.Math.Max (10.0/(this.gridStep*this.zoom), 1.0);

			//	Dessine les traits verticaux.
			double step = this.gridStep*mul;
			int subdiv = (int) this.gridSubdiv;
			int rank = 0;
			for (double pos=0; pos<=this.AreaSize.Width; pos+=step)
			{
				double x = pos;
				double y = 0;
				graphics.Align (ref x, ref y);
				x += ix;
				y += iy;
				graphics.AddLine (x, y, x, this.AreaSize.Height);

				if (rank%subdiv == 0)
				{
					graphics.RenderSolid (Color.FromAlphaRgb (0.3, 0.6, 0.6, 0.6));  // gris
				}
				else
				{
					graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0.6, 0.6, 0.6));  // gris
				}

				rank++;
			}

			//	Dessine les traits horizontaux.
			step = this.gridStep*mul;
			subdiv = (int) this.gridSubdiv;
			rank = 0;
			for (double pos=0; pos<=this.AreaSize.Height; pos+=step)
			{
				double x = 0;
				double y = pos;
				graphics.Align (ref x, ref y);
				x += ix;
				y += iy;
				graphics.AddLine (x, y, this.AreaSize.Width, y);

				if (rank%subdiv == 0)
				{
					graphics.RenderSolid (Color.FromAlphaRgb (0.3, 0.6, 0.6, 0.6));  // gris
				}
				else
				{
					graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0.6, 0.6, 0.6));  // gris
				}

				rank++;
			}

			graphics.LineWidth = initialWidth;
		}

		public void PaintObjects(Graphics graphics)
		{
			//	Dessine l'arrière-plan de tous les objets.
			foreach (AbstractObject obj in this.nodes)
			{
				obj.DrawBackground (graphics);
			}

			foreach (AbstractObject obj in this.edges)
			{
				obj.DrawBackground (graphics);
			}

			foreach (AbstractObject obj in this.comments)
			{
				obj.DrawBackground (graphics);
			}

			//	Dessine l'avant plan tous les objets.
			foreach (AbstractObject obj in this.nodes)
			{
				obj.DrawForeground (graphics);
			}

			foreach (AbstractObject obj in this.edges)
			{
				obj.DrawForeground (graphics);
			}

			foreach (AbstractObject obj in this.comments)
			{
				obj.DrawForeground (graphics);
			}
		}


		#region Enumerators
		private IEnumerable<Link> Links
		{
			get
			{
				foreach (var obj in this.LinkableObjects)
				{
					foreach (var link in obj.Links)
					{
						yield return link;
					}
				}
			}
		}

		private IEnumerable<ObjectLink> LinkObjects
		{
			get
			{
				foreach (var obj in this.LinkableObjects)
				{
					foreach (var link in obj.Links)
					{
						if (link.ObjectLink != null)
						{
							yield return link.ObjectLink;
						}
					}
				}
			}
		}

		private IEnumerable<LinkableObject> LinkableObjects
		{
			get
			{
				foreach (var node in this.nodes)
				{
					yield return node;
				}

				foreach (var edge in this.edges)
				{
					yield return edge;
				}
			}
		}
#endregion

		#region Serialization
		public string Serialize()
		{
			//	Sérialise la vue éditée et retourne le résultat dans un string.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter(buffer);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);
			writer.Formatting = Formatting.None;

			this.WriteXml(writer);

			writer.Flush();
			writer.Close();
			return buffer.ToString();
		}

		public void Deserialize(string data)
		{
			//	Désérialise la vue à partir d'un string de données.
			System.IO.StringReader stringReader = new System.IO.StringReader(data);
			XmlTextReader reader = new XmlTextReader(stringReader);
			
			this.ReadXml(reader);

			reader.Close();
		}

		private void WriteXml(XmlWriter writer)
		{
#if false
			//	Sérialise toutes les boîtes.
			writer.WriteStartDocument();

			writer.WriteStartElement(Xml.Boxes);
			foreach (ObjectBox box in this.boxes)
			{
				box.WriteXml(writer);
			}
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
#endif
		}

		private void ReadXml(XmlReader reader)
		{
#if false
			//	Désérialise toutes les boîtes.
			this.Clear();

			while (reader.ReadToFollowing(Xml.Box))
			{
				ObjectBox box = new ObjectBox (this);
				box.ReadXml (reader);
				
				if (box.CultureMap == null)
				{
					//	Somebody deleted the referenced entity; simply discard the box from
					//	the entity graph and let the user clean up the mess (there might be
					//	visible comments and other entities pointed to by the missing one).

					continue;
				}
				
				this.boxes.Add (box);
			}

			foreach (ObjectBox box in this.boxes)
			{
				box.AdjustAfterRead();
			}

			this.CloseBox(null);  // voir ObjectBox.AdjustAfterRead, commentaire (*)
			this.UpdateAfterAddOrRemoveConnexion(null);
			this.UpdateAfterOpenOrCloseBox();
#endif
		}
		#endregion

		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			return this.GetTooltipEditedText(pos);
		}

		private string GetTooltipEditedText(Point pos)
		{
			//	Donne le texte du tooltip en fonction de la position.
			if (this.hilitedObject == null)
			{
				return null;  // pas de tooltip
			}
			else
			{
				pos = this.ConvWidgetToEditor(pos);
				return this.hilitedObject.GetToolTipText(pos);
			}
		}
		#endregion

		#region MouseCursor
		private void ChangeMouseCursor(MouseCursorType cursor)
		{
			//	Change le sprite de la souris.
			if (cursor == this.lastCursor)
			{
				return;
			}

			this.lastCursor = cursor;

			switch ( cursor )
			{
				case MouseCursorType.Finger:
					this.SetMouseCursorImage(ref this.mouseCursorFinger, Misc.Icon("CursorFinger"));
					break;

				case MouseCursorType.Grid:
					this.SetMouseCursorImage(ref this.mouseCursorGrid, Misc.Icon("CursorGrid"));
					break;

				case MouseCursorType.Move:
					this.MouseCursor = MouseCursor.AsSizeAll;
					break;

				case MouseCursorType.Hand:
					this.SetMouseCursorImage(ref this.mouseCursorHand, Misc.Icon("CursorHand"));
					break;

				case MouseCursorType.IBeam:
					this.MouseCursor = MouseCursor.AsIBeam;
					break;

				case MouseCursorType.Locate:
					this.SetMouseCursorImage(ref this.mouseCursorLocate, Misc.Icon("CursorLocate"));
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			if (this.Window != null)
			{
				this.Window.MouseCursor = this.MouseCursor;
			}
		}

		private void SetMouseCursorImage(ref Image image, string name)
		{
			//	Choix du sprite de la souris.
			if (image == null)
			{
				image = ImageProvider.Default.GetImage(name, Resources.DefaultManager);
			}
			
			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion

		#region Events handler
		protected virtual void OnAreaSizeChanged()
		{
			//	Génère un événement pour dire que les dimensions ont changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("AreaSizeChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Epsitec.Common.Support.EventHandler AreaSizeChanged
		{
			add
			{
				this.AddUserEventHandler("AreaSizeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("AreaSizeChanged", value);
			}
		}

		protected virtual void OnAreaOffsetChanged()
		{
			//	Génère un événement pour dire que l'offset de la surface de travail a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("AreaOffsetChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Epsitec.Common.Support.EventHandler AreaOffsetChanged
		{
			add
			{
				this.AddUserEventHandler("AreaOffsetChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("AreaOffsetChanged", value);
			}
		}

		protected virtual void OnZoomChanged()
		{
			//	Génère un événement pour dire que l'offset de la surface de travail a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ZoomChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Epsitec.Common.Support.EventHandler ZoomChanged
		{
			add
			{
				this.AddUserEventHandler("ZoomChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ZoomChanged", value);
			}
		}

		protected virtual void OnEditingStateChanged()
		{
			//	Génère un événement pour dire que l'état d'édition a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("EditingStateChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		public event Epsitec.Common.Support.EventHandler EditingStateChanged
		{
			add
			{
				this.AddUserEventHandler ("EditingStateChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("EditingStateChanged", value);
			}
		}
		#endregion


		public static readonly double			defaultWidth = 200;
		public static readonly double			edgeDetour = 30;
		public static readonly double			pushMargin = 10;
		private static readonly double			frameMargin = 40;

		private Core.Business.BusinessContext	businessContext;

		private WorkflowDefinitionEntity		workflowDefinitionEntity;
		private List<ObjectNode>				nodes;
		private List<ObjectEdge>				edges;
		private List<ObjectComment>				comments;

		private Size							areaSize;
		private double							zoom;
		private Point							areaOffset;
		private AbstractObject					lockObject;
		private bool							isScrollerEnable;
		private Point							brutPos;
		private MessageType						lastMessageType;
		private Point							lastMessagePos;
		private bool							isAreaMoving;
		private Point							areaMovingInitialPos;
		private Point							areaMovingInitialOffset;
		private MouseCursorType					lastCursor = MouseCursorType.Unknown;
		private Image							mouseCursorFinger;
		private Image							mouseCursorHand;
		private Image							mouseCursorGrid;
		private Image							mouseCursorLocate;
		private VScroller						vscroller;
		private AbstractObject					hilitedObject;
		private bool							dirtySerialization;
		private bool							grid;
		private double							gridStep;
		private double							gridSubdiv;
		private AbstractObject					editingObject;
	}
}
