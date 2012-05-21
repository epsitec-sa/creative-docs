using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Widget permettant d'éditer graphiquement des entités.
	/// </summary>
	public class Editor : Widget, Widgets.Helpers.IToolTipHost
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
			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;

			this.boxes = new List<ObjectBox>();
			this.connections = new List<ObjectConnection>();
			this.comments = new List<ObjectComment>();
			this.infos = new List<ObjectInfo>();

			this.zoom = 1;
			this.areaOffset = Point.Zero;
			this.gridStep = 20;
			this.gridSubdiv = 5;

			this.enableDimmed = true;
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


		public Viewers.Entities Entities
		{
			get
			{
				return this.entities;
			}
			set
			{
				this.entities = value;
			}
		}

		public Module Module
		{
			get
			{
				return this.module;
			}
			set
			{
				this.module = value;
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


		public bool EditExpression(Druid fieldId)
		{
			//	Edite l'expression d'un champ.
			ObjectBox root = this.RootBox;
			if (root == null)
			{
				return false;
			}

			return root.EditExpression(fieldId);
		}


		public void AddBox(ObjectBox box)
		{
			//	Ajoute une nouvelle boîte dans l'éditeur. Elle est positionnée toujours au même endroit,
			//	avec une hauteur nulle. La hauteur sera de toute façon adaptée par UpdateBoxes().
			//	La position initiale n'a pas d'importance. La première boîte ajoutée (la boîte racine)
			//	est positionnée par RedimArea(). La position des autres est de toute façon recalculée en
			//	fonction de la boîte parent.
			box.SetBounds(new Rectangle(0, 0, Editor.defaultWidth, 0));
			box.IsExtended = true;

			this.boxes.Add(box);
			this.UpdateAfterOpenOrCloseBox();
		}

		public int BoxCount
		{
			//	Retourne le nombre de boîtes existantes.
			get
			{
				return this.boxes.Count;
			}
		}

		public List<ObjectBox> Boxes
		{
			//	Retourne la liste des boîtes.
			get
			{
				return this.boxes;
			}
		}

		public ObjectBox RootBox
		{
			//	Retourne la boîte racine.
			get
			{
				return this.boxes[0];
			}
		}

		public ObjectBox SearchBox(string title)
		{
			//	Cherche une boîte d'après son titre.
			foreach (ObjectBox box in this.boxes)
			{
				if (box.Title == title)
				{
					return box;
				}
			}

			return null;
		}

		public void AddConnection(ObjectConnection connection)
		{
			//	Ajoute une nouvelle liaison dans l'éditeur.
			this.connections.Add(connection);
		}

		public void AddComment(ObjectComment comment)
		{
			//	Ajoute un nouveau commentaire dans l'éditeur.
			this.comments.Add (comment);
		}

		public void RemoveComment(ObjectComment comment)
		{
			//	Supprime un commentaire dans l'éditeur.
			this.comments.Remove (comment);
		}

		public void AddInfo(ObjectInfo info)
		{
			//	Ajoute une nouvelle information dans l'éditeur.
			this.infos.Add(info);
		}

		public void Clear()
		{
			//	Supprime toutes les boîtes et toutes les liaisons de l'éditeur.
			this.boxes.Clear();
			this.connections.Clear();
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
			this.UpdateBoxes();
			this.UpdateConnections();
		}

		public void UpdateAfterCommentChanged()
		{
			//	Appelé lorsqu'un commentaire ou une information a changé.
			this.RedimArea();

			this.UpdateConnections();
			this.RedimArea();

			this.UpdateDimmed();
		}

		public void UpdateAfterGeometryChanged(ObjectBox box)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateBoxes();  // adapte la taille selon compact/étendu
			this.PushLayout (box, PushDirection.Automatic, this.gridStep);
			this.RedimArea();

			this.UpdateConnections();
			this.RedimArea();

			this.UpdateDimmed();
		}

		public void UpdateAfterMoving(ObjectBox box)
		{
			//	Appelé lorsqu'une boîte a été bougée.
			this.PushLayout (box, PushDirection.Automatic, this.gridStep);
			this.RedimArea();

			this.UpdateConnections();
			this.RedimArea();
		}

		public void UpdateAfterAddOrRemoveConnection(ObjectBox box)
		{
			//	Appelé lorsqu'une liaison a été ajoutée ou supprimée.
			this.UpdateBoxes();
			this.PushLayout (box, PushDirection.Automatic, this.gridStep);
			this.RedimArea();

			//	Il ne faut surtout pas faire un RedimArea entre ces deux opérations, car cela déplace
			//	parfois les commentaires liés aux connexions !
			this.CreateConnections();
			this.UpdateConnections();
			this.RedimArea();

			this.UpdateDimmed();
		}

		private void UpdateBoxes()
		{
			//	Met à jour la géométrie de toutes les boîtes.
			foreach (ObjectBox box in this.boxes)
			{
				Rectangle bounds = box.Bounds;
				double top = bounds.Top;
				double h = box.GetBestHeight();
				bounds.Bottom = top-h;
				bounds.Height = h;
				box.SetBounds(bounds);
			}
		}

		public void UpdateConnections()
		{
			//	Met à jour la géométrie de toutes les liaisons.
			this.CommentsMemorize();

			foreach (ObjectBox box in this.boxes)
			{
				for (int i=0; i<box.Fields.Count; i++)
				{
					Field field = box.Fields[i];

					if (field.Relation != FieldRelation.None)
					{
						ObjectConnection connection = field.Connection;
						if (connection != null)
						{
							connection.Points.Clear();

							if (field.IsExplored)
							{
								this.UpdateConnection(connection, box, field.Index, field.DstBox);
							}
							else
							{
								field.RouteClear();

								//	Important: toujours le point droite en premier !
								double posv = box.GetConnectionSrcVerticalPosition(i);
								connection.Points.Add(new Point(box.Bounds.Right-1, posv));
								connection.Points.Add(new Point(box.Bounds.Left+1, posv));
								connection.Field.Route = Field.RouteType.Close;
							}
						}
					}
				}
			}

			//	Réparti astucieusement le point d'arrivé en haut ou en bas d'une boîte de toutes les
			//	connections de type Bt ou Bb, pour éviter que deux connections n'arrivent sur le même point.
			//	Les croisements sont minimisés.
			foreach (ObjectBox box in this.boxes)
			{
				box.ConnectionListBt.Clear();
				box.ConnectionListBb.Clear();
				box.ConnectionListC.Clear();
				box.ConnectionListD.Clear();
			}

			foreach (ObjectConnection connection in this.connections)
			{
				if (connection.Field.DstBox != null && connection.Field.Route == Field.RouteType.Bt)
				{
					connection.Field.DstBox.ConnectionListBt.Add(connection);
				}

				if (connection.Field.DstBox != null && connection.Field.Route == Field.RouteType.Bb)
				{
					connection.Field.DstBox.ConnectionListBb.Add(connection);
				}

				if (connection.Field.DstBox != null && connection.Field.Route == Field.RouteType.C)
				{
					connection.Field.DstBox.ConnectionListC.Add(connection);
				}

				if (connection.Field.DstBox != null && connection.Field.Route == Field.RouteType.D)
				{
					connection.Field.DstBox.ConnectionListD.Add(connection);
				}
			}

			foreach (ObjectBox box in this.boxes)
			{
				this.ShiftConnectionsB(box, box.ConnectionListBt);
				this.ShiftConnectionsB(box, box.ConnectionListBb);
				this.ShiftConnectionsC(box, box.ConnectionListC);
				this.ShiftConnectionsD(box, box.ConnectionListD);
			}

			foreach (ObjectBox box in this.boxes)
			{
				box.ConnectionListBt.Clear();
				box.ConnectionListBb.Clear();
				box.ConnectionListC.Clear();
				box.ConnectionListD.Clear();
			}

			//	Adapte tous les commentaires.
			foreach (ObjectComment comment in this.comments)
			{
				if (comment.AttachObject is ObjectConnection)
				{
					ObjectConnection connection = comment.AttachObject as ObjectConnection;

					Point oldPos = connection.Field.CommentPosition;
					Point newPos = connection.PositionConnectionComment;

					if (!oldPos.IsZero && !newPos.IsZero)
					{
						Rectangle rect = connection.Field.CommentBounds;
						rect.Offset(newPos-oldPos);
						comment.SetBounds(rect);  // déplace le commentaire
					}
				}
			}

			this.Invalidate();
		}

		private void UpdateConnection(ObjectConnection connection, ObjectBox src, int srcRank, ObjectBox dst)
		{
			//	Met à jour la géométrie d'une liaison.
			Rectangle srcBounds = src.Bounds;
			Rectangle dstBounds = dst.Bounds;

			//	Calcul des rectangles plus petits, pour les tests d'intersections.
			Rectangle srcBoundsLittle = srcBounds;
			Rectangle dstBoundsLittle = dstBounds;
			srcBoundsLittle.Deflate(2);
			dstBoundsLittle.Deflate(2);

			connection.Points.Clear();
			connection.Field.RouteClear();

			double v = src.GetConnectionSrcVerticalPosition(srcRank);
			if (src == dst)  // connection à soi-même ?
			{
				Point p = new Point(srcBounds.Right-1, v);
				connection.Points.Add(p);

				p.X += 30;
				connection.Points.Add(p);

				p.Y -= 10;
				connection.Points.Add(p);

				p.X -= 30;
				connection.Points.Add(p);

				connection.Field.Route = Field.RouteType.Himself;
			}
			else if (!srcBounds.IntersectsWith(dstBounds))
			{
				Point p = new Point(0, v);

				if (dstBounds.Center.X > srcBounds.Right+Editor.connectionDetour/3)  // destination à droite ?
				{
					Point start = new Point(srcBounds.Right-1, p.Y);
					connection.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.connectionDetour)  // destination plus basse ?
					{
						Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Top);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
						connection.Field.Route = Field.RouteType.Bb;
					}
					else if (dstBounds.Bottom > start.Y+Editor.connectionDetour)  // destination plus haute ?
					{
						Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Bottom);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
						connection.Field.Route = Field.RouteType.Bt;
					}
					else
					{
						Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Left);
						if (start.Y != end.Y && end.X-start.X > Editor.connectionDetour)
						{
							connection.Points.Add(Point.Zero);  // (*)
							connection.Points.Add(Point.Zero);  // (*)
							connection.Points.Add(end);
							connection.Field.Route = Field.RouteType.C;
						}
						else
						{
							connection.Points.Add(end);
							connection.Field.Route = Field.RouteType.A;
						}
					}
				}
				else if (dstBounds.Center.X < srcBounds.Left-Editor.connectionDetour/3)  // destination à gauche ?
				{
					Point start = new Point(srcBounds.Left+1, p.Y);
					connection.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.connectionDetour)  // destination plus basse ?
					{
						Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Top);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
						connection.Field.Route = Field.RouteType.Bb;
					}
					else if (dstBounds.Bottom > start.Y+Editor.connectionDetour)  // destination plus haute ?
					{
						Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Bottom);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
						connection.Field.Route = Field.RouteType.Bt;
					}
					else
					{
						Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Right);
						if (start.Y != end.Y && start.X-end.X > Editor.connectionDetour)
						{
							connection.Points.Add(Point.Zero);  // (*)
							connection.Points.Add(Point.Zero);  // (*)
							connection.Points.Add(end);
							connection.Field.Route = Field.RouteType.C;
						}
						else
						{
							connection.Points.Add(end);
							connection.Field.Route = Field.RouteType.A;
						}
					}
				}
				else if (connection.Field.IsAttachToRight)  // destination à droite à cheval ?
				{
					Point start = new Point(srcBounds.Right-1, p.Y);
					Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Right);

					connection.Points.Add(start);
					connection.Points.Add(Point.Zero);  // (*)
					connection.Points.Add(Point.Zero);  // (*)
					connection.Points.Add(end);
					connection.Field.Route = Field.RouteType.D;
				}
				else  // destination à gauche à cheval ?
				{
					Point start = new Point(srcBounds.Left+1, p.Y);
					Point end = dst.GetConnectionDstPosition(start.Y, ObjectBox.ConnectionAnchor.Left);

					connection.Points.Add(start);
					connection.Points.Add(Point.Zero);  // (*)
					connection.Points.Add(Point.Zero);  // (*)
					connection.Points.Add(end);
					connection.Field.Route = Field.RouteType.D;
				}
			}
		}

		// (*)	Sera calculé par ObjectConnection.UpdateRoute !

		private void ShiftConnectionsB(ObjectBox box, List<ObjectConnection> connections)
		{
			//	Met à jour une liste de connections de type Bt ou Bb, afin qu'aucune connection
			//	n'arrive au même endroit.
			connections.Sort(new Comparers.Connection());  // tri pour minimiser les croisements

			double space = (box.Bounds.Width/(connections.Count+1.0))*0.75;

			for (int i=0; i<connections.Count; i++)
			{
				ObjectConnection connection = connections[i];

				int count = connection.Points.Count;
				if (count > 2)
				{
					double dx = space * (i-(connections.Count-1.0)/2);
					double px = connection.Points[count-1].X+dx;

					if (connection.IsRightDirection)
					{
						px = System.Math.Max(px, connection.Points[0].X+8);
					}
					else
					{
						px = System.Math.Min(px, connection.Points[0].X-8);
					}

					connection.Points[count-1] = new Point(px, connection.Points[count-1].Y);
					connection.Points[count-2] = new Point(px, connection.Points[count-2].Y);
					connection.UpdateRoute();
				}
			}
		}

		private void ShiftConnectionsC(ObjectBox box, List<ObjectConnection> connections)
		{
			//	Met à jour une liste de connections de type C, afin qu'aucune connection
			//	n'arrive au même endroit.
			connections.Sort(new Comparers.Connection());  // tri pour minimiser les croisements

			double spaceX = 5;
			double spaceY = 12;

			for (int i=0; i<connections.Count; i++)
			{
				ObjectConnection connection = connections[i];

				if (connection.Points.Count == 4)
				{
					double dx = box.IsExtended ? (connection.IsRightDirection ^ connection.Points[0].Y > connection.Points[connection.Points.Count-1].Y ? spaceX*i : -spaceX*i) : 0;
					double dy = box.IsExtended ? spaceY*i : 0;
					connection.Points[1] = new Point(connection.Points[1].X+dx, connection.Points[1].Y   );
					connection.Points[2] = new Point(connection.Points[2].X+dx, connection.Points[2].Y-dy);
					connection.Points[3] = new Point(connection.Points[3].X,    connection.Points[3].Y-dy);
				}
			}
		}

		private void ShiftConnectionsD(ObjectBox box, List<ObjectConnection> connections)
		{
			//	Met à jour une liste de connections de type D, afin qu'aucune connection
			//	n'arrive au même endroit.
			connections.Sort(new Comparers.Connection());  // tri pour minimiser les croisements

			double spaceX = 5;
			double spaceY = 12;

			for (int i=0; i<connections.Count; i++)
			{
				ObjectConnection connection = connections[i];

				if (connection.Points.Count == 4)
				{
					double dx = connection.IsRightDirection ? spaceX*i : -spaceX*i;
					double dy = box.IsExtended ? spaceY*i : 0;
					connection.Points[1] = new Point(connection.Points[1].X+dx, connection.Points[1].Y   );
					connection.Points[2] = new Point(connection.Points[2].X+dx, connection.Points[2].Y-dy);
					connection.Points[3] = new Point(connection.Points[3].X,    connection.Points[3].Y-dy);
				}
			}
		}

		public void CreateConnections()
		{
			//	Crée (ou recrée) toutes les liaisons nécessaires.
			this.CommentsMemorize();

			//	Supprime tous les commentaires liés aux connections.
			int j = 0;
			while (j < this.comments.Count)
			{
				ObjectComment comment = this.comments[j];

				if (comment.AttachObject is ObjectConnection)
				{
					this.comments.RemoveAt(j);
				}
				else
				{
					j++;
				}
			}
			
			this.connections.Clear();  // supprime toutes les connections existantes

			foreach (ObjectBox box in this.boxes)
			{
				for (int i=0; i<box.Fields.Count; i++)
				{
					Field field = box.Fields[i];

					if (field.Relation != FieldRelation.None)
					{
						//	Si la liaison est ouverte sur une boîte qui n'existe plus,
						//	considère la liaison comme fermée !
						if (field.IsExplored)
						{
							if (!this.boxes.Contains(field.DstBox))
							{
								field.IsExplored = false;
							}
						}

						ObjectConnection connection = new ObjectConnection(this);
						connection.Field = field;
						connection.BackgroundMainColor = box.BackgroundMainColor;
						field.Connection = connection;
						this.AddConnection(connection);
					}
				}
			}

			//	Recrée tous les commentaires liés aux connections.
			foreach (ObjectConnection connection in this.connections)
			{
				if (connection.Field.HasComment && connection.Field.IsExplored)
				{
					connection.Comment = new ObjectComment(this);
					connection.Comment.AttachObject = connection;
					connection.Comment.Text = connection.Field.CommentText;
					connection.Comment.BackgroundMainColor = connection.Field.CommentMainColor;
					connection.Comment.SetBounds(connection.Field.CommentBounds);

					this.AddComment(connection.Comment);
				}
			}

			this.Invalidate();
		}

		private void CommentsMemorize()
		{
			//	Mémorise l'état de tous les commentaires liés à des connections.
			foreach (ObjectConnection connection in this.connections)
			{
				connection.Field.HasComment = false;
			}

			foreach (ObjectComment comment in this.comments)
			{
				if (comment.AttachObject is ObjectConnection)
				{
					ObjectConnection connection = comment.AttachObject as ObjectConnection;

					connection.Field.HasComment = true;
					connection.Field.CommentText = comment.Text;
					connection.Field.CommentMainColor = comment.BackgroundMainColor;

					Point pos = connection.PositionConnectionComment;
					if (!pos.IsZero)
					{
						connection.Field.CommentPosition = pos;
					}

					if (!comment.Bounds.IsEmpty)
					{
						connection.Field.CommentBounds = comment.Bounds;
					}
				}
			}
		}

		private void UpdateDimmed()
		{
			//	Met en estompé toutes les connections qui partent ou qui arrivent sur une entité estompée.
			if (this.enableDimmed)
			{
				foreach (ObjectConnection connection in this.connections)
				{
					connection.IsDimmed = false;
				}

				foreach (ObjectConnection connection in this.connections)
				{
					if (connection.Field.IsExplored)
					{
						if (connection.Field.SrcBox != null && connection.Field.SrcBox.IsDimmed)
						{
							connection.IsDimmed =  true;
						}
						else if (connection.Field.DstBox != null && connection.Field.DstBox.IsDimmed)
						{
							connection.IsDimmed =  true;
						}
					}
					else
					{
						Module dstModule = this.module.DesignerApplication.SearchModule (connection.Field.Destination);
						Module currentModule = this.module.DesignerApplication.CurrentModule;

						connection.IsDimmed = (dstModule != currentModule);
					}

					if (connection.Comment != null)
					{
						connection.Comment.IsDimmed = connection.IsDimmed;
					}
				}

				foreach (ObjectBox box in this.boxes)
				{
					foreach (var comment in box.Comments)
					{
						comment.IsDimmed = box.IsDimmed;
					}

					if (box.Info != null)
					{
						box.Info.IsDimmed = box.IsDimmed;
					}
				}
			}
			else
			{
				foreach (var obj in this.boxes)
				{
					obj.IsDimmed = false;
				}

				foreach (var obj in this.connections)
				{
					obj.IsDimmed = false;
				}

				foreach (var obj in this.comments)
				{
					obj.IsDimmed = false;
				}

				foreach (var obj in this.infos)
				{
					obj.IsDimmed = false;
				}
			}
		}


		public bool IsEmptyArea(Rectangle area)
		{
			//	Retourne true si une zone est entièrement vide (aucune boîte, on ignore les connections).
			foreach (ObjectBox box in this.boxes)
			{
				if (box.Bounds.IntersectsWith(area))
				{
					return false;
				}
			}

			return true;
		}

		public void CloseBox(ObjectBox box)
		{
			//	Ferme une boîte et toutes les boîtes liées, en essayant de fermer le moins possible de boîtes.
			//	La stratégie utilisée est la suivante:
			//	1. On ferme la boîte demandée.
			//	2. Parmi toutes les boîtes restantes, on regarde si une boîte est isolée, c'est-à-dire si
			//	   elle n'est plus reliée à la racine. Si oui, on la détruit.
			//	3. Tant qu'on a détruit au moins une boîte, on recommence au point 2.
			if (box != null && box.IsRoot)
			{
				return;  // on ne détruit jamais la boîte racine
			}

			bool dirty = false;

			if (box != null)
			{
				this.CloseOneBox(box);  // supprime la boîte demandée
				this.CloseConnections(box);  // supprime ses connections
				dirty = true;
			}

#if false
			foreach (ObjectBox abox in this.boxes)
			{
				abox.IsConnectedToRoot = false;
				abox.Parents.Clear();
			}

			foreach (ObjectBox abox in this.boxes)
			{
				foreach (Field field in abox.Fields)
				{
					ObjectBox dstBox = field.DstBox;
					if (dstBox != null)
					{
						dstBox.Parents.Add(abox);
					}
				}
			}

			foreach (ObjectBox abox in this.boxes)
			{
				List<ObjectBox> visited = new List<ObjectBox>();
				visited.Add(abox);
				this.ExploreConnectedToRoot(visited, abox);

				bool toRoot = false;
				foreach (ObjectBox vbox in visited)
				{
					if (vbox == this.boxes[0])
					{
						toRoot = true;
						break;
					}
				}

				if (toRoot)
				{
					foreach (ObjectBox vbox in visited)
					{
						vbox.IsConnectedToRoot = true;
					}
				}
			}

			bool removed;
			do
			{
				removed = false;
				int i = 1;  // on saute toujours la boîte racine
				while (i < this.boxes.Count)
				{
					box = this.boxes[i];
					if (box.IsConnectedToRoot)  // boîte liée à la racine ?
					{
						i++;
					}
					else  // boîte isolée ?
					{
						this.CloseOneBox(box);  // supprime la boîte isolée
						this.CloseConnections(box);  // supprime ses connections
						removed = true;
						dirty = true;
					}
				}
			}
			while (removed);  // recommence tant qu'on a détruit quelque chose

			foreach (ObjectBox abox in this.boxes)
			{
				abox.IsConnectedToRoot = false;
				abox.Parents.Clear();
			}
#endif

			this.UpdateAfterOpenOrCloseBox();

			if (dirty)
			{
				this.module.AccessEntities.SetLocalDirty();
			}
		}

		private void CloseOneBox(ObjectBox box)
		{
			foreach (var comment in box.Comments)
			{
				this.comments.Remove (comment);
			}
			box.Comments.Clear ();

			if (box.Info != null)
			{
				this.infos.Remove(box.Info);
				box.Info = null;
			}

			this.boxes.Remove(box);  // supprime la boîte demandée
		}

		private void ExploreConnectedToRoot(List<ObjectBox> visited, ObjectBox root)
		{
			//	Cherche récursivement tous les objets depuis 'root'.
			foreach (Field field in root.Fields)
			{
				ObjectBox dstBox = field.DstBox;
				if (dstBox != null)
				{
					if (!visited.Contains(dstBox))
					{
						visited.Add(dstBox);
						this.ExploreConnectedToRoot(visited, dstBox);
					}
				}
			}

			foreach (ObjectBox srcBox in root.Parents)
			{
				if (!visited.Contains(srcBox))
				{
					visited.Add(srcBox);
					this.ExploreConnectedToRoot(visited, srcBox);
				}
			}
		}

		private void CloseConnections(ObjectBox removedBox)
		{
			//	Parcourt toutes les connections de toutes les boîtes, pour fermer toutes
			//	les connections sur la boîte supprimée.
			foreach (ObjectBox box in this.boxes)
			{
				foreach (Field field in box.Fields)
				{
					if (field.DstBox == removedBox)
					{
						field.DstBox = null;
						field.IsExplored = false;
					}
				}
			}
		}

		private void UpdateAfterOpenOrCloseBox()
		{
			//	Appelé après avoir ajouté ou supprimé une boîte.
			foreach (ObjectBox box in this.boxes)
			{
				box.UpdateAfterOpenOrCloseBox();
			}
		}


		private void PushLayout(ObjectBox exclude, PushDirection direction, double margin)
		{
			//	Pousse les boîtes pour éviter tout chevauchement.
			//	Une boîte peut être poussée hors de la surface de dessin.
			for (int max=0; max<100; max++)
			{
				bool push = false;

				for (int i=0; i<this.boxes.Count; i++)
				{
					ObjectBox box = this.boxes[i];

					ObjectBox inter = this.PushSearch(box, exclude, margin);
					if (inter != null)
					{
						push = true;
						this.PushAction(box, inter, direction, margin);
						this.PushLayout(inter, direction, margin);
					}
				}

				if (!push)
				{
					break;
				}
			}
		}

		private ObjectBox PushSearch(ObjectBox box, ObjectBox exclude, double margin)
		{
			//	Cherche une boîte qui chevauche 'box'.
			Rectangle rect = box.Bounds;
			rect.Inflate(margin);

			for (int i=0; i<this.boxes.Count; i++)
			{
				ObjectBox obj = this.boxes[i];

				if (obj != box && obj != exclude)
				{
					if (obj.Bounds.IntersectsWith(rect))
					{
						return obj;
					}
				}
			}

			return null;
		}

		private void PushAction(ObjectBox box, ObjectBox inter, PushDirection direction, double margin)
		{
			//	Pousse 'inter' pour venir après 'box' selon la direction choisie.
			Rectangle rect = inter.Bounds;

			double dr = box.Bounds.Right - rect.Left + margin;
			double dl = rect.Right - box.Bounds.Left + margin;
			double dt = box.Bounds.Top - rect.Bottom + margin;
			double db = rect.Top - box.Bounds.Bottom + margin;

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


		private void RedimArea()
		{
			//	Recalcule les dimensions de la surface de travail, en fonction du contenu.
			Rectangle rect = this.ComputeObjectsBounds();
			rect.Inflate (Editor.frameMargin);

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

			foreach (ObjectBox box in this.boxes)
			{
				bounds = Rectangle.Union(bounds, box.Bounds);
			}

			foreach (ObjectConnection connection in this.connections)
			{
				bounds = Rectangle.Union(bounds, connection.Bounds);
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

		private void MoveObjects(double dx, double dy)
		{
			//	Déplace tous les objets.
			if (dx == 0 && dy == 0)  // immobile ?
			{
				return;
			}

			foreach (ObjectBox box in this.boxes)
			{
				box.Move(dx, dy);
			}

			foreach (ObjectConnection connection in this.connections)
			{
				connection.Move(dx, dy);
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
						zoom = System.Math.Max(zoom, Viewers.Entities.zoomMin);
						zoom = System.Math.Min(zoom, Viewers.Entities.zoomMax);
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

		private Point ConvEditorToWidget(Point pos)
		{
			//	Conversion d'une coordonnée dans l'espace de l'éditeur vers l'espace normal des widgets.
			pos.Y = this.areaSize.Height-pos.Y;
			pos -= this.areaOffset;
			pos *= this.zoom;
			pos.Y = this.Client.Size.Height-pos.Y;

			return pos;
		}

		private new void MouseMove(Message message, Point pos)
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

				for (int i=this.connections.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.connections[i];
					if (obj.MouseMove(message, pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				for (int i=this.boxes.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.boxes[i];
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
					if (fly.HilitedElement == AbstractObject.ActiveElement.BoxHeader)
					{
						if (this.IsLocateActionHeader(message))
						{
							ObjectBox box = fly as ObjectBox;
							if (box != null && !box.IsRoot)
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
							if (this.BoxCount > 1)
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
							 fly.HilitedElement == AbstractObject.ActiveElement.BoxInside ||
							 fly.HilitedElement == AbstractObject.ActiveElement.ConnectionHilited ||
							 fly.HilitedElement == AbstractObject.ActiveElement.BoxFieldGroup)
					{
						type = MouseCursorType.Arrow;
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.BoxFieldName ||
							 fly.HilitedElement == AbstractObject.ActiveElement.BoxFieldType ||
							 fly.HilitedElement == AbstractObject.ActiveElement.BoxFieldExpression)
					{
						if (this.IsLocateAction(message))
						{
							type = MouseCursorType.Locate;
						}
						else
						{
							if (fly.IsMousePossible(fly.HilitedElement, fly.HilitedFieldRank))
							{
								type = MouseCursorType.Grid;
							}
							else
							{
								type = MouseCursorType.Arrow;
							}
						}
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.BoxFieldTitle)
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

		private void MouseDown(Message message, Point pos)
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

		private void MouseUp(Message message, Point pos)
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


		public Rectangle BoxGridAlign(Rectangle rect)
		{
			//	Aligne un rectangle d'une boîte (ObjectBox) sur son coin supérieur/gauche,
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
			//	Indique si l'action débouche sur une opération de navigation (pour BoxHeader).
			return (message.IsControlPressed || this.CurrentModifyMode == ModifyMode.Locked);
		}

		public ModifyMode CurrentModifyMode
		{
			//	Retourne le mode de travail courant.
			get
			{
				if (this.module.DesignerApplication.IsReadonly)
				{
					if (this.entities.SubView == 3)  // sous-vue T ?
					{
						return ModifyMode.Partial;
					}
					else
					{
						return ModifyMode.Locked;
					}
				}
				else
				{
					return ModifyMode.Unlocked;
				}
			}
		}


		public void LockObject(AbstractObject obj)
		{
			//	Indique l'objet en cours de drag.
			this.lockObject = obj;
		}

		private AbstractObject DetectObject(Point pos)
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

			for (int i=this.connections.Count-1; i>=0; i--)
			{
				ObjectConnection connection = this.connections[i];

				if (connection.IsReadyForAction)
				{
					return connection;
				}
			}

			for (int i=this.boxes.Count-1; i>=0; i--)
			{
				ObjectBox box = this.boxes[i];

				if (box.IsReadyForAction)
				{
					return box;
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

		public bool EnableDimmed
		{
			get
			{
				return this.enableDimmed;
			}
			set
			{
				this.enableDimmed = value;
				this.UpdateDimmed ();
			}
		}

		public void PaintObjects(Graphics graphics)
		{
			//	Dessine l'arrière-plan de tous les objets.
			foreach (AbstractObject obj in this.boxes)
			{
				obj.DrawBackground (graphics);
			}

			foreach (AbstractObject obj in this.connections)
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
			foreach (AbstractObject obj in this.boxes)
			{
				obj.DrawForeground (graphics);
			}

			foreach (AbstractObject obj in this.connections)
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


		#region Cartridge
		private enum CartridgeSamples
		{
			Abstract          = 0x00000001,
			Schema            = 0x00000002,
			Repository        = 0x00000004,
			Display           = 0x00000008,
			Creation          = 0x00000010,
			Volatile          = 0x00000100,
			Stable            = 0x00000200,
			Immutable         = 0x00000400,
			PublicRelation    = 0x00001000,
			PublicCollection  = 0x00002000,
			PrivateRelation   = 0x00004000,
			PrivateCollection = 0x00008000,
			Interface         = 0x00010000,
		}

		public Size CartridgeSize(BitmapParameters bitmapParameters)
		{
			//	Retourne la taille nécessaire pour le cartouche.
			//	Le cartouche est entièrement dynamique; seuls les exemples utilisés dans le dessin sont comptés.
			double width  = 0;
			double height = 0;

			if (bitmapParameters.GenerateUserCartridge || bitmapParameters.GenerateDateCartridge)
			{
				width += bitmapParameters.GenerateUserCartridge ? Editor.CartridgeHeaderUserWidth : 0;
				width += bitmapParameters.GenerateDateCartridge ? Editor.CartridgeHeaderDateWidth : 0;
				width--;
				height = Editor.CartridgeHeaderHeight;
			}

			if (bitmapParameters.GenerateSamplesCartridge)
			{
				var samples = this.GetCartridgeSamplesUsed ();
				var count = 0;

				for (int i = 0; i < 32; i++)
				{
					if (((int) samples & (1<<i)) != 0)
					{
						count++;  // compte les bits dans 'samples'
					}
				}

				if (count != 0)
				{
					double w = Editor.CartridgeSampleMargin*(count+1) + Editor.CartridgeSampleWidth*count;
					double h = Editor.CartridgeSampleMargin*2 + Editor.CartridgeSampleHeight;

					width = System.Math.Max (width, w);
					height += h;
				}
			}

			return new Size (width*Editor.CartridgeZoom, height*Editor.CartridgeZoom);
		}

		public void PaintCartridge(Graphics graphics, BitmapParameters bitmapParameters)
		{
			//	Dessine le cartouche avec les légendes explicatives.
			//	Le cartouche est entièrement dynamique; seuls les exemples utilisés dans le dessin y figurent.
			Transform initialTransform = graphics.Transform;
			graphics.ScaleTransform (Editor.CartridgeZoom, Editor.CartridgeZoom, 0, 0);

			Rectangle bounds, rect;

			var size = this.CartridgeSize (bitmapParameters);
			bounds = new Rectangle (1, 1, size.Width/Editor.CartridgeZoom, size.Height/Editor.CartridgeZoom);
			bounds.Inflate (0.5);
			graphics.AddFilledRectangle (bounds);
			graphics.RenderSolid (Color.FromHexa ("fff6e0"));  // beige pâle
			graphics.AddRectangle (bounds);
			graphics.RenderSolid (Color.FromBrightness (0));

			if ((bitmapParameters.GenerateUserCartridge || bitmapParameters.GenerateDateCartridge) && bitmapParameters.GenerateSamplesCartridge)
			{
				graphics.AddLine (bounds.Left, bounds.Top-Editor.CartridgeHeaderHeight-1, bounds.Right, bounds.Top-Editor.CartridgeHeaderHeight-1);
				graphics.RenderSolid (Color.FromBrightness (0));
			}

			if (bitmapParameters.GenerateUserCartridge)
			{
				rect = new Rectangle (bounds.Left, bounds.Top-Editor.CartridgeHeaderHeight-1, Editor.CartridgeHeaderUserWidth, Editor.CartridgeHeaderHeight+1);

				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromBrightness (0));

				string name = "Inconnu";
				if (this.module.DesignerApplication.Settings.IdentityCard != null)
				{
					name = this.module.DesignerApplication.Settings.IdentityCard.UserName;
				}

				graphics.PaintText (rect.Left, rect.Bottom+1, rect.Width, rect.Height-1, name, Font.DefaultFont, 14, ContentAlignment.MiddleCenter);
			}

			if (bitmapParameters.GenerateDateCartridge)
			{
				var offset = bitmapParameters.GenerateUserCartridge ? Editor.CartridgeHeaderUserWidth : 0;
				rect = new Rectangle (bounds.Left+offset, bounds.Top-Editor.CartridgeHeaderHeight-1, Editor.CartridgeHeaderDateWidth, Editor.CartridgeHeaderHeight+1);

				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromBrightness (0));

				var date = System.DateTime.Now;
				var text = date.ToString ("dd.MM.yyyy");
				graphics.PaintText (rect.Left, rect.Bottom+1, rect.Width, rect.Height-1, text, Font.DefaultFont, 14, ContentAlignment.MiddleCenter);
			}

			if (bitmapParameters.GenerateSamplesCartridge)
			{
				rect = new Rectangle (bounds.Left+Editor.CartridgeSampleMargin+0.5, bounds.Bottom+Editor.CartridgeSampleMargin+0.5, Editor.CartridgeSampleWidth, Editor.CartridgeSampleHeight);
				var samples = this.GetCartridgeSamplesUsed ();

				if ((samples & CartridgeSamples.Abstract) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Abstrait", null, DataLifetimeExpectancy.Unknown, StructuredTypeFlags.AbstractClass);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Schema) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Schéma", null, DataLifetimeExpectancy.Unknown, StructuredTypeFlags.GenerateSchema);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Repository) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Repository", null, DataLifetimeExpectancy.Unknown, StructuredTypeFlags.GenerateRepository);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Display) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Affichage indiv.", null, DataLifetimeExpectancy.Unknown, StructuredTypeFlags.StandaloneDisplay);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Creation) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Création indiv.", null, DataLifetimeExpectancy.Unknown, StructuredTypeFlags.StandaloneCreation);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Volatile) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Volatile", null, DataLifetimeExpectancy.Volatile, StructuredTypeFlags.None);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Stable) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Stable", null, DataLifetimeExpectancy.Stable, StructuredTypeFlags.None);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Immutable) != 0)
				{
					ObjectBox.DrawFrame (graphics, rect, AbstractObject.MainColor.Grey, false, true, false, "Immuable", null, DataLifetimeExpectancy.Immutable, StructuredTypeFlags.None);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.Interface) != 0)
				{
					var r = rect;
					r.Left += 25;
					ObjectBox.DrawFrame (graphics, r, AbstractObject.MainColor.Grey, false, true, true, "Interface", null, DataLifetimeExpectancy.Unknown, StructuredTypeFlags.None);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.PublicRelation) != 0)
				{
					Editor.PaintRelation (graphics, rect, "Relation publique", "de type référence", FieldRelation.Reference, false);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.PublicCollection) != 0)
				{
					Editor.PaintRelation (graphics, rect, "Relation publique", "de type collection", FieldRelation.Collection, false);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.PrivateRelation) != 0)
				{
					Editor.PaintRelation (graphics, rect, "Relation privée", "de type référence", FieldRelation.Reference, true);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}

				if ((samples & CartridgeSamples.PrivateCollection) != 0)
				{
					Editor.PaintRelation (graphics, rect, "Relation privée", "de type collection", FieldRelation.Collection, true);
					rect.Offset (Editor.CartridgeSampleMargin+Editor.CartridgeSampleWidth, 0);
				}
			}

			graphics.Transform = initialTransform;
		}

		private static void PaintRelation(Graphics graphics, Rectangle bounds, string title1, string title2, FieldRelation relation, bool isPrivateRelation)
		{
			//	Dessine la flèche '--->>*' d'une relation dans le cartouche.
			bounds.Deflate (1.5);
			graphics.AddFilledRectangle (bounds);
			graphics.RenderSolid (Color.FromBrightness (0.9));
			graphics.AddRectangle (bounds);
			graphics.RenderSolid (Color.FromBrightness (0));

			double titleHeight = 15;
			var textRect1 = new Rectangle (bounds.Left, bounds.Top-titleHeight, bounds.Width, titleHeight);
			var textRect2 = new Rectangle (bounds.Left, bounds.Top-titleHeight*2, bounds.Width, titleHeight);
			var arrowRect = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-titleHeight*2);
			var start     = new Point (arrowRect.Left+20,  arrowRect.Bottom+arrowRect.Height/2);
			var end       = new Point (arrowRect.Right-20, arrowRect.Bottom+arrowRect.Height/2);

			graphics.PaintText (textRect1.Left, textRect1.Bottom, textRect1.Width, textRect1.Height, title1, Font.DefaultFont, 10.5, ContentAlignment.MiddleCenter);
			graphics.PaintText (textRect2.Left, textRect2.Bottom, textRect2.Width, textRect2.Height, title2, Font.DefaultFont, 10.5, ContentAlignment.MiddleCenter);

			graphics.LineWidth = 2;
			graphics.AddLine (start, end);
			AbstractObject.DrawEndingArrow (graphics, start, end, relation, isPrivateRelation);
			graphics.RenderSolid (Color.FromBrightness (0));
			graphics.LineWidth = 1;
		}

		private CartridgeSamples GetCartridgeSamplesUsed()
		{
			//	Retourne tous les exemples qui ont un sens de voir figurer dans le cartouche,
			//	c'est-à-dire ceux qui figurent dans le dessin.
			CartridgeSamples samples = 0;

			foreach (ObjectBox obj in this.boxes)
			{
				Editor.AddCartridgeSamplesUsed (ref samples, obj);
			}

			return samples;
		}

		private static void AddCartridgeSamplesUsed(ref CartridgeSamples samples, ObjectBox box)
		{
			//	Ajoute tous les exemples utilisés par un objet.
			if (box.IsInterface)
			{
				samples |= CartridgeSamples.Interface;
			}

			if ((box.StructuredTypeFlags & StructuredTypeFlags.AbstractClass) != 0)
			{
				samples |= CartridgeSamples.Abstract;
			}

			if ((box.StructuredTypeFlags & StructuredTypeFlags.GenerateSchema) != 0)
			{
				samples |= CartridgeSamples.Schema;
			}

			if ((box.StructuredTypeFlags & StructuredTypeFlags.GenerateRepository) != 0)
			{
				samples |= CartridgeSamples.Repository;
			}

			if ((box.StructuredTypeFlags & StructuredTypeFlags.StandaloneDisplay) != 0)
			{
				samples |= CartridgeSamples.Display;
			}

			if ((box.StructuredTypeFlags & StructuredTypeFlags.StandaloneCreation) != 0)
			{
				samples |= CartridgeSamples.Creation;
			}

			switch (box.DataLifetimeExpectancy)
            {
				case DataLifetimeExpectancy.Volatile:
					samples |= CartridgeSamples.Volatile;
					break;

				case DataLifetimeExpectancy.Stable:
					samples |= CartridgeSamples.Stable;
					break;

				case DataLifetimeExpectancy.Immutable:
					samples |= CartridgeSamples.Immutable;
					break;
			}

			foreach (var field in box.Fields)
			{
				Editor.AddCartridgeSamplesUsed (ref samples, field);
			}
		}

		private static void AddCartridgeSamplesUsed(ref CartridgeSamples samples, Field field)
		{
			//	Ajoute tous les exemples utilisés par un champ.
			if (field.IsInterfaceLocal)
			{
				samples |= CartridgeSamples.Interface;
			}

			switch (field.Relation)
			{
				case FieldRelation.Reference:
					if (field.IsPrivateRelation)
					{
						samples |= CartridgeSamples.PrivateRelation;
					}
					else
					{
						samples |= CartridgeSamples.PublicRelation;
					}
					break;

				case FieldRelation.Collection:
					if (field.IsPrivateRelation)
					{
						samples |= CartridgeSamples.PrivateCollection;
					}
					else
					{
						samples |= CartridgeSamples.PublicCollection;
					}
					break;
			}
		}

		private static double CartridgeHeaderUserWidth = 200;
		private static double CartridgeHeaderDateWidth = 100;
		private static double CartridgeHeaderHeight    = 24;

		private static double CartridgeSampleMargin    = 10;
		private static double CartridgeSampleWidth     = 100;
		private static double CartridgeSampleHeight    = 80;
		
		private static double CartridgeZoom            = 0.5;
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
			//	Sérialise toutes les boîtes.
			writer.WriteStartDocument();

			writer.WriteStartElement(Xml.Boxes);
			foreach (ObjectBox box in this.boxes)
			{
				box.WriteXml(writer);
			}
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

		private void ReadXml(XmlReader reader)
		{
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
			var handler = this.GetUserEventHandler("AreaSizeChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler AreaSizeChanged
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
			var handler = this.GetUserEventHandler("AreaOffsetChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler AreaOffsetChanged
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
			var handler = this.GetUserEventHandler("ZoomChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler ZoomChanged
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
		public static readonly double connectionDetour = 30;
		public static readonly double pushMargin = 10;
		private static readonly double frameMargin = 40;

		private Module module;
		private Viewers.Entities entities;
		private List<ObjectBox> boxes;
		private List<ObjectConnection> connections;
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
		private bool grid;
		private double gridStep;
		private double gridSubdiv;
		private bool enableDimmed;
	}
}
