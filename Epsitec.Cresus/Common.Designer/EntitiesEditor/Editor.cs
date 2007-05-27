using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Widget permettant d'éditer graphiquement des entités.
	/// </summary>
	public class Editor : Widget
	{
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
			this.objects = new List<AbstractObject>();
			this.boxes = new List<ObjectBox>();
			this.connections = new List<ObjectConnection>();
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


		public void AddBox(ObjectBox box)
		{
			box.Bounds = new Rectangle(20+(Editor.defaultWidth+40)*this.boxes.Count, this.ActualHeight-20-30*this.boxes.Count-100, Editor.defaultWidth, 100);
			box.IsExtended = (this.boxes.Count == 0);

			this.objects.Add(box);
			this.boxes.Add(box);
		}

		public void AddConnection(ObjectConnection connection)
		{
			this.objects.Add(connection);
			this.connections.Add(connection);
		}


		public void UpdateGeometry()
		{
			//	Met à jour la géométrie de toutes les boîtes et de toutes les liaisons.
			this.UpdateBoxes();
			this.UpdateConnections();
		}

		public void UpdateAfterGeometryChanged(ObjectBox box)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateBoxes();  // adapte la taille selon compact/étendu
			
			this.PushBoxesInside(Editor.pushMargin);
			this.PushLayout(box, PushDirection.Automatic, Editor.pushMargin);
			this.PushBoxesInside(Editor.pushMargin);
			this.UpdateConnections();
		}

		protected void UpdateBoxes()
		{
			//	Met à jour la géométrie de toutes les boîtes.
			foreach (ObjectBox box in this.boxes)
			{
				Rectangle bounds = box.Bounds;
				double top = bounds.Top;
				double h = box.GetBestHeight();
				bounds.Bottom = top-h;
				bounds.Height = h;
				box.Bounds = bounds;
			}
		}

		protected void UpdateConnections()
		{
			//	Met à jour la géométrie de toutes les liaisons.
			//	TODO: provisoire !
			this.UpdateConnection(this.connections[0], this.boxes[0], 2, this.boxes[1], FieldRelation.Reference);  // lien client
			this.UpdateConnection(this.connections[1], this.boxes[0], 3, this.boxes[2], FieldRelation.Collection);  // lien articles
			this.UpdateConnection(this.connections[2], this.boxes[0], 5, this.boxes[3], FieldRelation.Inclusion);  // lien rabais
			this.Invalidate();
		}

		protected void UpdateConnection(ObjectConnection connection, ObjectBox src, int srcRank, ObjectBox dst, FieldRelation relation)
		{
			//	Met à jour la géométrie d'une liaison.
			connection.Bounds = this.Client.Bounds;
			connection.Relation = relation;

			Rectangle srcBounds = src.Bounds;
			Rectangle dstBounds = dst.Bounds;

			//	Calcul des rectangles plus petits, pour les tests d'intersections.
			Rectangle srcBoundsLittle = srcBounds;
			Rectangle dstBoundsLittle = dstBounds;
			srcBoundsLittle.Deflate(2);
			dstBoundsLittle.Deflate(2);

			connection.Points.Clear();

			double v = src.GetConnectionVerticalPosition(srcRank);
			if (!double.IsNaN(v) && !srcBounds.IntersectsWith(dstBounds))
			{
				Point p = new Point(0, v);

				if (dstBounds.Center.X > srcBounds.Right+Editor.connectionDetour)  // destination à droite ?
				{
					Point start = new Point(srcBounds.Right-1, p.Y);
					connection.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.connectionDetour)  // destination plus basse ?
					{
						Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Top);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
					}
					else if (dstBounds.Bottom > start.Y+Editor.connectionDetour)  // destination plus haute ?
					{
						Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Bottom);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
					}
					else
					{
						Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Left);
						if (start.Y != end.Y && end.X-start.X > Editor.connectionDetour)
						{
							connection.Points.Add(new Point((start.X+end.X)/2, start.Y));
							connection.Points.Add(new Point((start.X+end.X)/2, end.Y));
						}
						connection.Points.Add(end);
					}
				}
				else if (dstBounds.Center.X < srcBounds.Left-Editor.connectionDetour)  // destination à gauche ?
				{
					Point start = new Point(srcBounds.Left+1, p.Y);
					connection.Points.Add(start);

					if (dstBounds.Top < start.Y-Editor.connectionDetour)  // destination plus basse ?
					{
						Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Top);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
					}
					else if (dstBounds.Bottom > start.Y+Editor.connectionDetour)  // destination plus haute ?
					{
						Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Bottom);
						connection.Points.Add(new Point(end.X, start.Y));
						connection.Points.Add(end);
					}
					else
					{
						Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Right);
						if (start.Y != end.Y && start.X-end.X > Editor.connectionDetour)
						{
							connection.Points.Add(new Point((start.X+end.X)/2, start.Y));
							connection.Points.Add(new Point((start.X+end.X)/2, end.Y));
						}
						connection.Points.Add(end);
					}
				}
				else if (dstBounds.Center.X > srcBounds.Center.X)  // destination à droite à cheval ?
				{
					Point start = new Point(srcBounds.Right-1, p.Y);
					connection.Points.Add(start);

					Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Right);
					double posx = System.Math.Max(start.X, end.X)+Editor.connectionDetour;
					connection.Points.Add(new Point(posx, start.Y));
					connection.Points.Add(new Point(posx, end.Y));
					connection.Points.Add(end);
				}
				else  // destination à gauche à cheval ?
				{
					Point start = new Point(srcBounds.Left+1, p.Y);
					connection.Points.Add(start);

					Point end = dst.GetConnectionDestination(start.Y, ObjectBox.ConnectionAnchor.Left);
					double posx = System.Math.Min(start.X, end.X)-Editor.connectionDetour;
					connection.Points.Add(new Point(posx, start.Y));
					connection.Points.Add(new Point(posx, end.Y));
					connection.Points.Add(end);
				}
			}
		}


		protected void PushLayout(ObjectBox exclude, PushDirection direction, double margin)
		{
			//	Pousse les boîtes pour éviter tout chevauchement.
			//	Une boîte peut être poussée hors de la surface de dessin.
			for (int max=0; max<100; max++)
			{
				bool push = false;

				for (int i=0; i<this.objects.Count; i++)
				{
					AbstractObject obj = this.objects[i];

					if (obj is ObjectBox)
					{
						ObjectBox box = obj as ObjectBox;
						ObjectBox inter = this.PushSearch(box, exclude, margin);
						if (inter != null)
						{
							push = true;
							this.PushAction(box, inter, direction, margin);
							this.PushLayout(inter, direction, margin);
						}
					}
				}

				if (!push)
				{
					break;
				}
			}
		}

		protected ObjectBox PushSearch(ObjectBox box, ObjectBox exclude, double margin)
		{
			//	Cherche une boîte qui chevauche 'box'.
			Rectangle rect = box.Bounds;
			rect.Inflate(margin);

			for (int i=0; i<this.objects.Count; i++)
			{
				AbstractObject obj = this.objects[i];

				if (obj is ObjectBox && obj != box && obj != exclude)
				{
					ObjectBox b = obj as ObjectBox;

					if (b.Bounds.IntersectsWith(rect))
					{
						return b;
					}
				}
			}

			return null;
		}

		protected void PushAction(ObjectBox box, ObjectBox inter, PushDirection direction, double margin)
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

			inter.Bounds = rect;
		}


		protected void PushBoxesInside(double margin)
		{
			//	Remet les boîtes dans la surface de dessin.
			for (int i=0; i<this.objects.Count; i++)
			{
				AbstractObject obj = this.objects[i];

				if (obj is ObjectBox)
				{
					ObjectBox box = obj as ObjectBox;

					Rectangle bounds = box.Bounds;

					if (bounds.Left < margin)
					{
						bounds.Offset(margin-bounds.Left, 0);
					}

					if (bounds.Right > this.ActualWidth-margin)
					{
						bounds.Offset(this.ActualWidth-margin-bounds.Right, 0);
					}

					if (bounds.Bottom < margin)
					{
						bounds.Offset(0, margin-bounds.Bottom);
					}

					if (bounds.Top > this.ActualHeight-margin)
					{
						bounds.Offset(0, this.ActualHeight-margin-bounds.Top);
					}

					if (bounds != box.Bounds)
					{
						box.Bounds = bounds;
						this.PushLayout(box, PushDirection.Automatic, Editor.pushMargin);
					}
				}
			}
		}

		protected void RecenterBoxes(double margin)
		{
			//	Si des boîtes dépassent de la surface de dessin, recentre le tout.
			Rectangle bounds = this.ComputeBoxBounds();

			if (bounds.Left < margin)
			{
				this.MoveBoxes(margin-bounds.Left, 0);
			}

			if (bounds.Right > this.ActualWidth-margin)
			{
				this.MoveBoxes(this.ActualWidth-margin-bounds.Right, 0);
			}

			if (bounds.Bottom < margin)
			{
				this.MoveBoxes(0, margin-bounds.Bottom);
			}

			if (bounds.Top > this.ActualHeight-margin)
			{
				this.MoveBoxes(0, this.ActualHeight-margin-bounds.Top);
			}
		}

		protected Rectangle ComputeBoxBounds()
		{
			//	Retourne le rectangle englobant toutes les boîtes.
			Rectangle bounds = Rectangle.Empty;

			for (int i=0; i<this.objects.Count; i++)
			{
				AbstractObject obj = this.objects[i];

				if (obj is ObjectBox)
				{
					ObjectBox box = obj as ObjectBox;
					bounds = Rectangle.Union(bounds, box.Bounds);
				}
			}

			return bounds;
		}

		protected void MoveBoxes(double dx, double dy)
		{
			//	Déplace toutes les boîtes.
			for (int i=0; i<this.objects.Count; i++)
			{
				AbstractObject obj = this.objects[i];

				if (obj is ObjectBox)
				{
					ObjectBox box = obj as ObjectBox;

					Rectangle bounds = box.Bounds;
					bounds.Offset(dx, dy);
					box.Bounds = bounds;
				}
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					if (this.isDragging)
					{
						this.MouseDraggingMove(pos);
					}
					else
					{
						this.MouseHilite(pos);
					}
					message.Consumer = this;
					break;

				case MessageType.MouseDown:
					this.MouseDown(pos);
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.MouseUp(pos);
					message.Consumer = this;
					break;
			}
		}

		protected void MouseHilite(Point pos)
		{
			//	Met en évidence tous les widgets selon la position visée par la souris.
			//	La boîte à l'avant-plan a la priorité.
			for (int i=this.objects.Count-1; i>=0; i--)
			{
				AbstractObject obj = this.objects[i];

				if (obj is ObjectBox)
				{
					ObjectBox box = obj as ObjectBox;
					if (box.MouseHilite(pos))
					{
						pos = Point.Zero;  // si on était dans cette boîte -> plus aucun hilite pour les boîtes placées dessous
					}
				}
			}
		}

		protected void MouseDown(Point pos)
		{
			//	Début du déplacement d'une boîte.
			ObjectBox box = this.DetectBox(pos);

			if (box != null)
			{
				if (box.IsReadyForDragging)
				{
					this.draggingBox = box;
					this.draggingPos = pos;
					this.isDragging = true;
				}
				else
				{
					box.MouseDown(pos);
				}
			}
		}

		protected void MouseDraggingMove(Point pos)
		{
			//	Déplacement d'une boîte.
			Rectangle bounds = this.draggingBox.Bounds;

			bounds.Offset(pos-this.draggingPos);
			this.draggingPos = pos;

			this.draggingBox.Bounds = bounds;
			this.UpdateConnections();
		}

		protected void MouseUp(Point pos)
		{
			//	Fin du déplacement d'une boîte.
			if (this.isDragging)
			{
				this.PushBoxesInside(Editor.pushMargin);
				this.PushLayout(this.draggingBox, PushDirection.Automatic, Editor.pushMargin);
				this.RecenterBoxes(Editor.pushMargin);
				this.PushBoxesInside(Editor.pushMargin);
				this.UpdateConnections();

				this.draggingBox = null;
				this.isDragging = false;
			}
			else
			{
				ObjectBox box = this.DetectBox(pos);
				if (box != null)
				{
					box.MouseUp(pos);
				}
			}
		}

		protected ObjectBox DetectBox(Point pos)
		{
			//	Détecte la boîte visée par la souris.
			//	La boîte à l'avant-plan a la priorité.
			for (int i=this.objects.Count-1; i>=0; i--)
			{
				AbstractObject obj = this.objects[i];

				if (obj is ObjectBox)
				{
					ObjectBox box = obj as ObjectBox;
					if (box.Bounds.Contains(pos))
					{
						return box;
					}
				}
			}

			return null;
		}



		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			foreach (AbstractObject obj in this.objects)
			{
				obj.Draw(graphics);
			}
		}



		protected static readonly double defaultWidth = 180;
		protected static readonly double connectionDetour = 30;
		protected static readonly double pushMargin = 10;

		protected List<AbstractObject> objects;
		protected List<ObjectBox> boxes;
		protected List<ObjectConnection> connections;
		protected bool isDragging;
		protected Point draggingPos;
		protected ObjectBox draggingBox;
	}
}
