//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.WorkflowDesigner.Objects;

using System.Xml;
using System.Xml.Serialization;
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

		protected enum MouseCursorType
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

		protected enum PushDirection
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

			this.nodes       = new List<ObjectNode>();
			this.edges = new List<ObjectEdge> ();
			this.comments    = new List<ObjectComment> ();
			this.infos       = new List<ObjectInfo> ();

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


		public void SetWorkflowDefinitionEntity(WorkflowDefinitionEntity entity)
		{
			this.workflowDefinitionEntity = entity;
			this.CreateInitialWorkflow ();
		}

		public void SetLocalDirty()
		{
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


		private void CreateInitialWorkflow()
		{
			var n = this.workflowDefinitionEntity.StartingEdges[0].NextNode;  // TODO: provisoire !

			var node = new ObjectNode(this, n);
			node.IsRoot = true;
			this.AddNode (node);

			this.UpdateAfterGeometryChanged (node);
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

		public ObjectNode RootNode
		{
			//	Retourne la boîte racine.
			get
			{
				return this.nodes[0];
			}
		}


		public void AddNode(ObjectNode node)
		{
			//	Ajoute une nouvelle boîte dans l'éditeur. Elle est positionnée toujours au même endroit,
			//	avec une hauteur nulle. La hauteur sera de toute façon adaptée par UpdateNodes().
			//	La position initiale n'a pas d'importance. La première boîte ajoutée (la boîte racine)
			//	est positionnée par RedimArea(). La position des autres est de toute façon recalculée en
			//	fonction de la boîte parent.
			node.SetBounds (new Rectangle (0, 0, Editor.defaultWidth, 0));
			node.IsExtended = true;

			this.nodes.Add (node);
		}

		public void AddEdge(ObjectEdge edge)
		{
			//	Ajoute une nouvelle liaison dans l'éditeur.
			this.edges.Add(edge);
		}

		public void AddComment(ObjectComment comment)
		{
			//	Ajoute un nouveau commentaire dans l'éditeur.
			this.comments.Add(comment);
		}

		public void AddInfo(ObjectInfo info)
		{
			//	Ajoute une nouvelle information dans l'éditeur.
			this.infos.Add(info);
		}


		public void Clear()
		{
			//	Supprime toutes les boîtes et toutes les liaisons de l'éditeur.
			this.nodes.Clear();
			this.edges.Clear();
			this.comments.Clear();
			this.infos.Clear();
			this.LockObject(null);
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
			this.UpdateEdges();
		}

		public void UpdateAfterCommentChanged()
		{
			//	Appelé lorsqu'un commentaire ou une information a changé.
			this.RedimArea();

			this.UpdateEdges();
			this.RedimArea();

			this.UpdateDimmed();
		}

		public void UpdateAfterGeometryChanged(ObjectNode node)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateNodes();  // adapte la taille selon compact/étendu
			this.PushLayout (node, PushDirection.Automatic, this.gridStep);
			this.RedimArea();

			this.UpdateEdges();
			this.RedimArea();

			this.UpdateDimmed();
		}

		public void UpdateAfterMoving(ObjectNode node)
		{
			//	Appelé lorsqu'une boîte a été bougée.
			this.PushLayout (node, PushDirection.Automatic, this.gridStep);
			this.RedimArea();

			this.UpdateEdges();
			this.RedimArea();
		}

		public void UpdateAfterAddOrRemoveEdge(ObjectNode node)
		{
			//	Appelé lorsqu'une liaison a été ajoutée ou supprimée.
			this.UpdateNodes();
			this.PushLayout (node, PushDirection.Automatic, this.gridStep);
			this.RedimArea();

			this.CreateEdges();
			this.RedimArea();

			this.UpdateEdges();
			this.RedimArea();

			this.UpdateDimmed();
		}

		protected void UpdateNodes()
		{
			//	Met à jour la géométrie de toutes les boîtes.
			foreach (ObjectNode node in this.nodes)
			{
				Rectangle bounds = node.Bounds;
				double top = bounds.Top;
				double h = node.GetBestHeight();
				bounds.Bottom = top-h;
				bounds.Height = h;
				node.SetBounds(bounds);
			}
		}

		public void UpdateEdges()
		{
			//	Met à jour la géométrie de toutes les liaisons.
			this.CommentsMemorize();

			foreach (ObjectNode node in this.nodes)
			{
				for (int i=0; i<node.Edges.Count; i++)
				{
					Edge edge = node.Edges[i];

					if (edge.Relation != FieldRelation.None)
					{
						ObjectEdge objectEdge = edge.ObjectEdge;
						if (objectEdge != null)
						{
							objectEdge.Points.Clear();

							if (edge.IsExplored)
							{
								this.UpdateEdge(objectEdge, node, edge.Index, edge.DstNode);
							}
							else
							{
								edge.RouteClear();

								//	Important: toujours le point droite en premier !
								double posv = node.GetEdgeSrcVerticalPosition(i);
								objectEdge.Points.Add(new Point(node.Bounds.Right-1, posv));
								objectEdge.Points.Add(new Point(node.Bounds.Left+1, posv));
								objectEdge.Edge.Route = Edge.RouteType.Close;
							}
						}
					}
				}
			}

			//	Réparti astucieusement le point d'arrivé en haut ou en bas d'une boîte de toutes les
			//	connections de type Bt ou Bb, pour éviter que deux connections n'arrivent sur le même point.
			//	Les croisements sont minimisés.
			foreach (ObjectNode node in this.nodes)
			{
				node.EdgeListBt.Clear();
				node.EdgeListBb.Clear();
				node.EdgeListC.Clear();
				node.EdgeListD.Clear();
			}

			foreach (ObjectEdge edge in this.edges)
			{
				if (edge.Edge.DstNode != null && edge.Edge.Route == Edge.RouteType.Bt)
				{
					edge.Edge.DstNode.EdgeListBt.Add(edge);
				}

				if (edge.Edge.DstNode != null && edge.Edge.Route == Edge.RouteType.Bb)
				{
					edge.Edge.DstNode.EdgeListBb.Add(edge);
				}

				if (edge.Edge.DstNode != null && edge.Edge.Route == Edge.RouteType.C)
				{
					edge.Edge.DstNode.EdgeListC.Add(edge);
				}

				if (edge.Edge.DstNode != null && edge.Edge.Route == Edge.RouteType.D)
				{
					edge.Edge.DstNode.EdgeListD.Add(edge);
				}
			}

			foreach (ObjectNode node in this.nodes)
			{
				this.ShiftEdgesB(node, node.EdgeListBt);
				this.ShiftEdgesB(node, node.EdgeListBb);
				this.ShiftEdgesC(node, node.EdgeListC);
				this.ShiftEdgesD(node, node.EdgeListD);
			}

			foreach (ObjectNode node in this.nodes)
			{
				node.EdgeListBt.Clear();
				node.EdgeListBb.Clear();
				node.EdgeListC.Clear();
				node.EdgeListD.Clear();
			}

			//	Adapte tous les commentaires.
			foreach (ObjectComment comment in this.comments)
			{
				if (comment.AttachObject is ObjectEdge)
				{
					ObjectEdge edge = comment.AttachObject as ObjectEdge;

					Point oldPos = edge.Edge.CommentPosition;
					Point newPos = edge.PositionEdgeComment;

					if (!oldPos.IsZero && !newPos.IsZero)
					{
						Rectangle rect = edge.Edge.CommentBounds;
						rect.Offset(newPos-oldPos);
						comment.SetBounds(rect);  // déplace le commentaire
					}
				}
			}

			this.Invalidate();
		}

		protected void UpdateEdge(ObjectEdge edge, ObjectNode src, int srcRank, ObjectNode dst)
		{
			//	Met à jour la géométrie d'une liaison.
			Rectangle srcBounds = src.Bounds;
			Rectangle dstBounds = dst.Bounds;

			//	Calcul des rectangles plus petits, pour les tests d'intersections.
			Rectangle srcBoundsLittle = srcBounds;
			Rectangle dstBoundsLittle = dstBounds;
			srcBoundsLittle.Deflate(2);
			dstBoundsLittle.Deflate(2);

			edge.Points.Clear();
			edge.Edge.RouteClear();

			double v = src.GetEdgeSrcVerticalPosition(srcRank);
			if (src == dst)  // connection à soi-même ?
			{
				Point p = new Point(srcBounds.Right-1, v);
				edge.Points.Add(p);

				p.X += 30;
				edge.Points.Add(p);

				p.Y -= 10;
				edge.Points.Add(p);

				p.X -= 30;
				edge.Points.Add(p);

				edge.Edge.Route = Edge.RouteType.Himself;
			}
			else if (!srcBounds.IntersectsWith(dstBounds))
			{
				Point p = new Point(0, v);

				if (dstBounds.Center.X > srcBounds.Right+Editor.edgeDetour/3)  // destination à droite ?
				{
					Point start = new Point(srcBounds.Right-1, p.Y);
					edge.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.edgeDetour)  // destination plus basse ?
					{
						Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Top);
						edge.Points.Add(new Point(end.X, start.Y));
						edge.Points.Add(end);
						edge.Edge.Route = Edge.RouteType.Bb;
					}
					else if (dstBounds.Bottom > start.Y+Editor.edgeDetour)  // destination plus haute ?
					{
						Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Bottom);
						edge.Points.Add(new Point(end.X, start.Y));
						edge.Points.Add(end);
						edge.Edge.Route = Edge.RouteType.Bt;
					}
					else
					{
						Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Left);
						if (start.Y != end.Y && end.X-start.X > Editor.edgeDetour)
						{
							edge.Points.Add(Point.Zero);  // (*)
							edge.Points.Add(Point.Zero);  // (*)
							edge.Points.Add(end);
							edge.Edge.Route = Edge.RouteType.C;
						}
						else
						{
							edge.Points.Add(end);
							edge.Edge.Route = Edge.RouteType.A;
						}
					}
				}
				else if (dstBounds.Center.X < srcBounds.Left-Editor.edgeDetour/3)  // destination à gauche ?
				{
					Point start = new Point(srcBounds.Left+1, p.Y);
					edge.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.edgeDetour)  // destination plus basse ?
					{
						Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Top);
						edge.Points.Add(new Point(end.X, start.Y));
						edge.Points.Add(end);
						edge.Edge.Route = Edge.RouteType.Bb;
					}
					else if (dstBounds.Bottom > start.Y+Editor.edgeDetour)  // destination plus haute ?
					{
						Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Bottom);
						edge.Points.Add(new Point(end.X, start.Y));
						edge.Points.Add(end);
						edge.Edge.Route = Edge.RouteType.Bt;
					}
					else
					{
						Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Right);
						if (start.Y != end.Y && start.X-end.X > Editor.edgeDetour)
						{
							edge.Points.Add(Point.Zero);  // (*)
							edge.Points.Add(Point.Zero);  // (*)
							edge.Points.Add(end);
							edge.Edge.Route = Edge.RouteType.C;
						}
						else
						{
							edge.Points.Add(end);
							edge.Edge.Route = Edge.RouteType.A;
						}
					}
				}
				else if (edge.Edge.IsAttachToRight)  // destination à droite à cheval ?
				{
					Point start = new Point(srcBounds.Right-1, p.Y);
					Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Right);

					edge.Points.Add(start);
					edge.Points.Add(Point.Zero);  // (*)
					edge.Points.Add(Point.Zero);  // (*)
					edge.Points.Add(end);
					edge.Edge.Route = Edge.RouteType.D;
				}
				else  // destination à gauche à cheval ?
				{
					Point start = new Point(srcBounds.Left+1, p.Y);
					Point end = dst.GetEdgeDstPosition(start.Y, ObjectNode.EdgeAnchor.Left);

					edge.Points.Add(start);
					edge.Points.Add(Point.Zero);  // (*)
					edge.Points.Add(Point.Zero);  // (*)
					edge.Points.Add(end);
					edge.Edge.Route = Edge.RouteType.D;
				}
			}
		}

		// (*)	Sera calculé par ObjectEdge.UpdateRoute !

		protected void ShiftEdgesB(ObjectNode node, List<ObjectEdge> edges)
		{
			//	Met à jour une liste de connections de type Bt ou Bb, afin qu'aucune connection
			//	n'arrive au même endroit.
			edges.Sort(new Comparers.EdgeComparer());  // tri pour minimiser les croisements

			double space = (node.Bounds.Width/(edges.Count+1.0))*0.75;

			for (int i=0; i<edges.Count; i++)
			{
				ObjectEdge edge = edges[i];

				int count = edge.Points.Count;
				if (count > 2)
				{
					double dx = space * (i-(edges.Count-1.0)/2);
					double px = edge.Points[count-1].X+dx;

					if (edge.IsRightDirection)
					{
						px = System.Math.Max(px, edge.Points[0].X+8);
					}
					else
					{
						px = System.Math.Min(px, edge.Points[0].X-8);
					}

					edge.Points[count-1] = new Point(px, edge.Points[count-1].Y);
					edge.Points[count-2] = new Point(px, edge.Points[count-2].Y);
					edge.UpdateRoute();
				}
			}
		}

		protected void ShiftEdgesC(ObjectNode node, List<ObjectEdge> edges)
		{
			//	Met à jour une liste de connections de type C, afin qu'aucune connection
			//	n'arrive au même endroit.
			edges.Sort(new Comparers.EdgeComparer());  // tri pour minimiser les croisements

			double spaceX = 5;
			double spaceY = 12;

			for (int i=0; i<edges.Count; i++)
			{
				ObjectEdge edge = edges[i];

				if (edge.Points.Count == 4)
				{
					double dx = node.IsExtended ? (edge.IsRightDirection ^ edge.Points[0].Y > edge.Points[edge.Points.Count-1].Y ? spaceX*i : -spaceX*i) : 0;
					double dy = node.IsExtended ? spaceY*i : 0;
					edge.Points[1] = new Point(edge.Points[1].X+dx, edge.Points[1].Y   );
					edge.Points[2] = new Point(edge.Points[2].X+dx, edge.Points[2].Y-dy);
					edge.Points[3] = new Point(edge.Points[3].X,    edge.Points[3].Y-dy);
				}
			}
		}

		protected void ShiftEdgesD(ObjectNode node, List<ObjectEdge> edges)
		{
			//	Met à jour une liste de connections de type D, afin qu'aucune connection
			//	n'arrive au même endroit.
			edges.Sort(new Comparers.EdgeComparer());  // tri pour minimiser les croisements

			double spaceX = 5;
			double spaceY = 12;

			for (int i=0; i<edges.Count; i++)
			{
				ObjectEdge edge = edges[i];

				if (edge.Points.Count == 4)
				{
					double dx = edge.IsRightDirection ? spaceX*i : -spaceX*i;
					double dy = node.IsExtended ? spaceY*i : 0;
					edge.Points[1] = new Point(edge.Points[1].X+dx, edge.Points[1].Y   );
					edge.Points[2] = new Point(edge.Points[2].X+dx, edge.Points[2].Y-dy);
					edge.Points[3] = new Point(edge.Points[3].X,    edge.Points[3].Y-dy);
				}
			}
		}

		public void CreateEdges()
		{
			//	Crée (ou recrée) toutes les liaisons nécessaires.
			this.CommentsMemorize();

			//	Supprime tous les commentaires liés aux connections.
			int j = 0;
			while (j < this.comments.Count)
			{
				ObjectComment comment = this.comments[j];

				if (comment.AttachObject is ObjectEdge)
				{
					this.comments.RemoveAt(j);
				}
				else
				{
					j++;
				}
			}
			
			this.edges.Clear();  // supprime toutes les connections existantes

			foreach (ObjectNode node in this.nodes)
			{
				for (int i=0; i<node.Edges.Count; i++)
				{
					Edge edge = node.Edges[i];

					if (edge.Relation != FieldRelation.None)
					{
						//	Si la liaison est ouverte sur une boîte qui n'existe plus,
						//	considère la liaison comme fermée !
						if (edge.IsExplored)
						{
							if (!this.nodes.Contains(edge.DstNode))
							{
								edge.IsExplored = false;
							}
						}

						ObjectEdge objectEdge = new ObjectEdge(this, null);
						objectEdge.Edge = edge;
						objectEdge.BackgroundMainColor = node.BackgroundMainColor;
						edge.ObjectEdge = objectEdge;
						this.AddEdge(objectEdge);
					}
				}
			}

			//	Recrée tous les commentaires liés aux connections.
			foreach (ObjectEdge objectEdge in this.edges)
			{
				if (objectEdge.Edge.HasComment && objectEdge.Edge.IsExplored)
				{
					objectEdge.Comment = new ObjectComment (this, null);
					objectEdge.Comment.AttachObject = objectEdge;
					objectEdge.Comment.Text = objectEdge.Edge.CommentText;
					objectEdge.Comment.BackgroundMainColor = objectEdge.Edge.CommentMainColor;
					objectEdge.Comment.SetBounds(objectEdge.Edge.CommentBounds);

					this.AddComment(objectEdge.Comment);
				}
			}

			this.Invalidate();
		}

		protected void CommentsMemorize()
		{
			//	Mémorise l'état de tous les commentaires liés à des connections.
			foreach (ObjectEdge objectEdge in this.edges)
			{
				objectEdge.Edge.HasComment = false;
			}

			foreach (ObjectComment comment in this.comments)
			{
				if (comment.AttachObject is ObjectEdge)
				{
					ObjectEdge objectEdge = comment.AttachObject as ObjectEdge;

					objectEdge.Edge.HasComment = true;
					objectEdge.Edge.CommentText = comment.Text;
					objectEdge.Edge.CommentMainColor = comment.BackgroundMainColor;

					Point pos = objectEdge.PositionEdgeComment;
					if (!pos.IsZero)
					{
						objectEdge.Edge.CommentPosition = pos;
					}

					if (!comment.Bounds.IsEmpty)
					{
						objectEdge.Edge.CommentBounds = comment.Bounds;
					}
				}
			}
		}

		protected void UpdateDimmed()
		{
			//	Met en estompé toutes les connections qui partent ou qui arrivent sur une entité estompée.
			foreach (ObjectEdge objectEdge in this.edges)
			{
				objectEdge.IsDimmed = false;
			}

			foreach (ObjectEdge objectEdge in this.edges)
			{
				if (objectEdge.Edge.IsExplored)
				{
					if (objectEdge.Edge.SrcNode != null && objectEdge.Edge.SrcNode.IsDimmed)
					{
						objectEdge.IsDimmed = true;
					}
					else if (objectEdge.Edge.DstNode != null && objectEdge.Edge.DstNode.IsDimmed)
					{
						objectEdge.IsDimmed = true;
					}
				}
				else
				{
#if false
					Module dstModule = this.module.DesignerApplication.SearchModule(connection.Field.Destination);
					Module currentModule = this.module.DesignerApplication.CurrentModule;

					connection.IsDimmed = (dstModule != currentModule);
#endif
				}

				if (objectEdge.Comment != null)
				{
					objectEdge.Comment.IsDimmed = objectEdge.IsDimmed;
				}
			}

			foreach (ObjectNode node in this.nodes)
			{
				if (node.Comment != null)
				{
					node.Comment.IsDimmed = node.IsDimmed;
				}

				if (node.Info != null)
				{
					node.Info.IsDimmed = node.IsDimmed;
				}
			}
		}


		public bool IsEmptyArea(Rectangle area)
		{
			//	Retourne true si une zone est entièrement vide (aucune boîte, on ignore les connections).
			foreach (ObjectNode node in this.nodes)
			{
				if (node.Bounds.IntersectsWith(area))
				{
					return false;
				}
			}

			return true;
		}

		public void CloseNode(ObjectNode node)
		{
			//	Ferme une boîte et toutes les boîtes liées, en essayant de fermer le moins possible de boîtes.
			//	La stratégie utilisée est la suivante:
			//	1. On ferme la boîte demandée.
			//	2. Parmi toutes les boîtes restantes, on regarde si une boîte est isolée, c'est-à-dire si
			//	   elle n'est plus reliée à la racine. Si oui, on la détruit.
			//	3. Tant qu'on a détruit au moins une boîte, on recommence au point 2.
			if (node != null && node.IsRoot)
			{
				return;  // on ne détruit jamais la boîte racine
			}

			bool dirty = false;

			if (node != null)
			{
				this.CloseOneNode(node);  // supprime la boîte demandée
				this.CloseEdges(node);  // supprime ses connections
				dirty = true;
			}

			foreach (ObjectNode anode in this.nodes)
			{
				anode.IsConnectedToRoot = false;
				anode.Parents.Clear();
			}

			foreach (ObjectNode anode in this.nodes)
			{
				foreach (Edge edge in anode.Edges)
				{
					ObjectNode dstNode = edge.DstNode;
					if (dstNode != null)
					{
						dstNode.Parents.Add(anode);
					}
				}
			}

			foreach (ObjectNode anode in this.nodes)
			{
				List<ObjectNode> visited = new List<ObjectNode>();
				visited.Add(anode);
				this.ExploreConnectedToRoot(visited, anode);

				bool toRoot = false;
				foreach (ObjectNode vnode in visited)
				{
					if (vnode == this.nodes[0])
					{
						toRoot = true;
						break;
					}
				}

				if (toRoot)
				{
					foreach (ObjectNode vnode in visited)
					{
						vnode.IsConnectedToRoot = true;
					}
				}
			}

			bool removed;
			do
			{
				removed = false;
				int i = 1;  // on saute toujours la boîte racine
				while (i < this.nodes.Count)
				{
					node = this.nodes[i];
					if (node.IsConnectedToRoot)  // boîte liée à la racine ?
					{
						i++;
					}
					else  // boîte isolée ?
					{
						this.CloseOneNode(node);  // supprime la boîte isolée
						this.CloseEdges(node);  // supprime ses connections
						removed = true;
						dirty = true;
					}
				}
			}
			while (removed);  // recommence tant qu'on a détruit quelque chose

			foreach (ObjectNode anode in this.nodes)
			{
				anode.IsConnectedToRoot = false;
				anode.Parents.Clear();
			}

			if (dirty)
			{
				this.SetLocalDirty();
			}
		}

		protected void CloseOneNode(ObjectNode node)
		{
			if (node.Comment != null)
			{
				this.comments.Remove(node.Comment);
				node.Comment = null;
			}

			if (node.Info != null)
			{
				this.infos.Remove(node.Info);
				node.Info = null;
			}

			this.nodes.Remove(node);  // supprime la boîte demandée
		}

		protected void ExploreConnectedToRoot(List<ObjectNode> visited, ObjectNode root)
		{
			//	Cherche récursivement tous les objets depuis 'root'.
			foreach (Edge edge in root.Edges)
			{
				ObjectNode dstNode = edge.DstNode;
				if (dstNode != null)
				{
					if (!visited.Contains(dstNode))
					{
						visited.Add(dstNode);
						this.ExploreConnectedToRoot(visited, dstNode);
					}
				}
			}

			foreach (ObjectNode srcNode in root.Parents)
			{
				if (!visited.Contains(srcNode))
				{
					visited.Add(srcNode);
					this.ExploreConnectedToRoot(visited, srcNode);
				}
			}
		}

		protected void CloseEdges(ObjectNode removedNode)
		{
			//	Parcourt toutes les connections de toutes les boîtes, pour fermer toutes
			//	les connections sur la boîte supprimée.
			foreach (ObjectNode node in this.nodes)
			{
				foreach (Edge edge in node.Edges)
				{
					if (edge.DstNode == removedNode)
					{
						edge.DstNode = null;
						edge.IsExplored = false;
					}
				}
			}
		}


		protected void PushLayout(ObjectNode exclude, PushDirection direction, double margin)
		{
			//	Pousse les boîtes pour éviter tout chevauchement.
			//	Une boîte peut être poussée hors de la surface de dessin.
			for (int max=0; max<100; max++)
			{
				bool push = false;

				for (int i=0; i<this.nodes.Count; i++)
				{
					ObjectNode node = this.nodes[i];

					ObjectNode inter = this.PushSearch(node, exclude, margin);
					if (inter != null)
					{
						push = true;
						this.PushAction(node, inter, direction, margin);
						this.PushLayout(inter, direction, margin);
					}
				}

				if (!push)
				{
					break;
				}
			}
		}

		protected ObjectNode PushSearch(ObjectNode node, ObjectNode exclude, double margin)
		{
			//	Cherche une boîte qui chevauche 'node'.
			Rectangle rect = node.Bounds;
			rect.Inflate(margin);

			for (int i=0; i<this.nodes.Count; i++)
			{
				ObjectNode obj = this.nodes[i];

				if (obj != node && obj != exclude)
				{
					if (obj.Bounds.IntersectsWith(rect))
					{
						return obj;
					}
				}
			}

			return null;
		}

		protected void PushAction(ObjectNode node, ObjectNode inter, PushDirection direction, double margin)
		{
			//	Pousse 'inter' pour venir après 'node' selon la direction choisie.
			Rectangle rect = inter.Bounds;

			double dr = node.Bounds.Right - rect.Left + margin;
			double dl = rect.Right - node.Bounds.Left + margin;
			double dt = node.Bounds.Top - rect.Bottom + margin;
			double db = rect.Top - node.Bounds.Bottom + margin;

			if (direction == PushDirection.Automatic)
			{
				double min = System.Math.Min(System.Math.Min(dr, dl), System.Math.Min(dt, db));

					 if (min == dr)  direction = PushDirection.Right;
				else if (min == dl)  direction = PushDirection.Left;
				else if (min == dt)  direction = PushDirection.Top;
				else                 direction = PushDirection.Bottom;
			}

			if (direction == PushDirection.Right)
			{
				rect.Offset(dr, 0);
			}

			if (direction == PushDirection.Left)
			{
				rect.Offset(-dl, 0);
			}

			if (direction == PushDirection.Top)
			{
				rect.Offset(0, dt);
			}

			if (direction == PushDirection.Bottom)
			{
				rect.Offset(0, -db);
			}

			inter.SetBounds(rect);
		}


		protected void RedimArea()
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

		protected Rectangle ComputeObjectsBounds()
		{
			//	Retourne le rectangle englobant tous les objets.
			Rectangle bounds = Rectangle.Empty;

			foreach (ObjectNode node in this.nodes)
			{
				bounds = Rectangle.Union(bounds, node.Bounds);
			}

			foreach (ObjectEdge edge in this.edges)
			{
				bounds = Rectangle.Union(bounds, edge.Bounds);
			}

			foreach (ObjectComment comment in this.comments)
			{
				bounds = Rectangle.Union(bounds, comment.Bounds);
			}

			foreach (ObjectInfo info in this.infos)
			{
				bounds = Rectangle.Union(bounds, info.Bounds);
			}

			return bounds;
		}

		protected void MoveObjects(double dx, double dy)
		{
			//	Déplace tous les objets.
			if (dx == 0 && dy == 0)  // immobile ?
			{
				return;
			}

			foreach (ObjectNode node in this.nodes)
			{
				node.Move(dx, dy);
			}

			foreach (ObjectEdge edge in this.edges)
			{
				edge.Move(dx, dy);
			}

			foreach (ObjectComment comment in this.comments)
			{
				comment.Move(dx, dy);
			}
		
			foreach (ObjectInfo info in this.infos)
			{
				info.Move(dx, dy);
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
					break;

				case MessageType.MouseMove:
					this.MouseMove(message, pos);
					message.Consumer = this;
					break;

				case MessageType.MouseDown:
					this.MouseDown(message, pos);
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.MouseUp(message, pos);
					message.Consumer = this;
					break;

				case MessageType.MouseLeave:
					this.MouseMove(message, Point.Zero);
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

		protected Point ConvWidgetToEditor(Point pos)
		{
			//	Conversion d'une coordonnée dans l'espace normal des widgets vers l'espace de l'éditeur,
			//	qui varie selon les ascenseurs (AreaOffset) et le zoom.
			pos.Y = this.Client.Size.Height-pos.Y;
			pos /= this.zoom;
			pos += this.areaOffset;
			pos.Y = this.areaSize.Height-pos.Y;

			return pos;
		}

		protected Point ConvEditorToWidget(Point pos)
		{
			//	Conversion d'une coordonnée dans l'espace de l'éditeur vers l'espace normal des widgets.
			pos.Y = this.areaSize.Height-pos.Y;
			pos -= this.areaOffset;
			pos *= this.zoom;
			pos.Y = this.Client.Size.Height-pos.Y;

			return pos;
		}

		protected void MouseMove(Message message, Point pos)
		{
			//	Met en évidence tous les widgets selon la position visée par la souris.
			//	L'objet à l'avant-plan a la priorité.
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

				for (int i=this.infos.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.infos[i];
					if (obj.MouseMove(message, pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				for (int i=this.edges.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.edges[i];
					if (obj.MouseMove(message, pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				for (int i=this.nodes.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.nodes[i];
					if (obj.MouseMove(message, pos))
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
					if (fly.HilitedElement == AbstractObject.ActiveElement.NodeHeader)
					{
						if (this.IsLocateActionHeader(message))
						{
							ObjectNode node = fly as ObjectNode;
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
					else if (fly.HilitedElement == AbstractObject.ActiveElement.None ||
							 fly.HilitedElement == AbstractObject.ActiveElement.NodeInside ||
							 fly.HilitedElement == AbstractObject.ActiveElement.EdgeHilited ||
							 fly.HilitedElement == AbstractObject.ActiveElement.NodeEdgeGroup)
					{
						type = MouseCursorType.Arrow;
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.NodeEdgeName ||
							 fly.HilitedElement == AbstractObject.ActiveElement.NodeEdgeType ||
							 fly.HilitedElement == AbstractObject.ActiveElement.NodeEdgeExpression)
					{
						if (this.IsLocateAction(message))
						{
							type = MouseCursorType.Locate;
						}
						else
						{
							if (fly.IsMousePossible(fly.HilitedElement, fly.HilitedEdgeRank))
							{
								type = MouseCursorType.Grid;
							}
							else
							{
								type = MouseCursorType.Arrow;
							}
						}
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.NodeEdgeTitle)
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
					else if (fly.HilitedElement == AbstractObject.ActiveElement.CommentEdit)
					{
						type = MouseCursorType.IBeam;
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.CommentMove ||
							 fly.HilitedElement == AbstractObject.ActiveElement.InfoMove)
					{
						type = MouseCursorType.Move;
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.InfoEdit)
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

		protected void MouseDown(Message message, Point pos)
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
				AbstractObject obj = this.DetectObject(pos);
				if (obj != null)
				{
					obj.MouseDown(message, pos);
				}
			}
		}

		protected void MouseUp(Message message, Point pos)
		{
			//	Fin du déplacement d'une boîte.
			if (this.isAreaMoving)
			{
				this.isAreaMoving = false;
			}
			else
			{
				AbstractObject obj = this.DetectObject(pos);
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

		protected AbstractObject DetectObject(Point pos)
		{
			//	Détecte l'objet visé par la souris.
			//	L'objet à l'avant-plan a la priorité.
			for (int i=this.comments.Count-1; i>=0; i--)
			{
				ObjectComment comment = this.comments[i];

				if (comment.IsReadyForAction)
				{
					return comment;
				}
			}

			for (int i=this.infos.Count-1; i>=0; i--)
			{
				ObjectInfo info = this.infos[i];

				if (info.IsReadyForAction)
				{
					return info;
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

			foreach (AbstractObject obj in this.infos)
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

			foreach (AbstractObject obj in this.infos)
			{
				obj.DrawForeground (graphics);
			}

			foreach (AbstractObject obj in this.comments)
			{
				obj.DrawForeground (graphics);
			}
		}


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

		protected void WriteXml(XmlWriter writer)
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

		protected void ReadXml(XmlReader reader)
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
			this.UpdateAfterAddOrRemoveConnection(null);
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

		protected string GetTooltipEditedText(Point pos)
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
		protected void ChangeMouseCursor(MouseCursorType cursor)
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

		protected void SetMouseCursorImage(ref Image image, string name)
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
		#endregion


		public static readonly double defaultWidth = 200;
		public static readonly double edgeDetour = 30;
		public static readonly double pushMargin = 10;
		private static readonly double frameMargin = 40;

		private WorkflowDefinitionEntity workflowDefinitionEntity;
		private List<ObjectNode> nodes;
		private List<ObjectEdge> edges;
		private List<ObjectComment> comments;
		private List<ObjectInfo> infos;
		private Size areaSize;
		private double zoom;
		private Point areaOffset;
		private AbstractObject lockObject;
		private bool isScrollerEnable;
		private Point brutPos;
		private MessageType lastMessageType;
		private Point lastMessagePos;
		private bool isAreaMoving;
		private Point areaMovingInitialPos;
		private Point areaMovingInitialOffset;
		private MouseCursorType lastCursor = MouseCursorType.Unknown;
		private Image mouseCursorFinger;
		private Image mouseCursorHand;
		private Image mouseCursorGrid;
		private Image mouseCursorLocate;
		private VScroller vscroller;
		private AbstractObject hilitedObject;
		private bool dirtySerialization;
		private bool grid;
		private double gridStep;
		private double gridSubdiv;
	}
}
