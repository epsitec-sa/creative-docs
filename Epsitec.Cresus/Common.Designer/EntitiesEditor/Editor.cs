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
		protected enum MouseCursorType
		{
			Unknown,
			Arrow,
			Finger,
			Grid,
			Move,
			Hand,
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
			this.boxes = new List<ObjectBox>();
			this.connections = new List<ObjectConnection>();
			this.zoom = 1;
			this.areaOffset = Point.Zero;
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


		public void AddBox(ObjectBox box)
		{
			//	Ajoute une nouvelle boîte dans l'éditeur. Elle est positionnée toujours au même endroit,
			//	avec une hauteur nulle. La hauteur sera de toute façon adaptée par UpdateBoxes().
			//	La position initiale n'a pas d'importance. La première boîte ajoutée (la boîte racine)
			//	est positionnée par RedimArea(). La position des autres est de toute façon recalculée en
			//	fonction de la boîte parent.
			box.Bounds = new Rectangle(0, 0, Editor.defaultWidth, 0);
			//?box.IsExtended = (this.boxes.Count == 0);
			box.IsExtended = true;

			this.boxes.Add(box);
		}

		public void RemoveBox(ObjectBox box)
		{
			//	Supprime une boîte de l'éditeur.
			this.boxes.Remove(box);
		}

		public int BoxCount
		{
			//	Retourne le nombre de boîtes existantes.
			get
			{
				return this.boxes.Count;
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

		public void Clear()
		{
			//	Supprime toutes les boîtes et toutes les liaisons de l'éditeur.
			this.boxes.Clear();
			this.connections.Clear();
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

		public void UpdateAfterGeometryChanged(ObjectBox box)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateBoxes();  // adapte la taille selon compact/étendu
			this.PushLayout(box, PushDirection.Automatic, Editor.pushMargin);
			this.RedimArea();
			this.UpdateConnections();
		}

		public void UpdateAfterMoving(ObjectBox box)
		{
			//	Appelé lorsqu'une boîte a été bougée.
			this.PushLayout(box, PushDirection.Automatic, Editor.pushMargin);
			this.RedimArea();
			this.UpdateConnections();
		}

		public void UpdateAfterAddOrRemoveConnection(ObjectBox box)
		{
			//	Appelé lorsqu'une liaison a été ajoutée ou supprimée.
			this.UpdateBoxes();
			this.PushLayout(box, PushDirection.Automatic, Editor.pushMargin);
			this.RedimArea();
			this.CreateConnections();
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

		public void UpdateConnections()
		{
			//	Met à jour la géométrie de toutes les liaisons.
			foreach (ObjectBox box in this.boxes)
			{
				for (int i=0; i<box.Fields.Count; i++)
				{
					ObjectBox.Field field = box.Fields[i];

					if (field.Relation != FieldRelation.None)
					{
						ObjectConnection connection = field.Connection;
						if (connection != null)
						{
							connection.Points.Clear();

							if (field.IsExplored)
							{
								this.UpdateConnection(connection, box, field.Rank, field.DstBox);
							}
							else
							{
								double posv = box.GetConnectionVerticalPosition(i);

								connection.Points.Add(new Point(box.Bounds.Right-1, posv));
								connection.Points.Add(new Point(box.Bounds.Left+1, posv));
							}
						}
					}
				}
			}

			this.Invalidate();
		}

		protected void UpdateConnection(ObjectConnection connection, ObjectBox src, int srcRank, ObjectBox dst)
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

			double v = src.GetConnectionVerticalPosition(srcRank);
			if (!srcBounds.IntersectsWith(dstBounds))
			{
				Point p = new Point(0, v);

				if (dstBounds.Center.X > srcBounds.Right+Editor.connectionDetour/3)  // destination à droite ?
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
				else if (dstBounds.Center.X < srcBounds.Left-Editor.connectionDetour/3)  // destination à gauche ?
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

		public void CreateConnections()
		{
			//	Crée (ou recrée) toutes les liaisons nécessaires.
			this.connections.Clear();

			foreach (ObjectBox box in this.boxes)
			{
				for (int i=0; i<box.Fields.Count; i++)
				{
					ObjectBox.Field field = box.Fields[i];

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
						field.Connection = connection;
						connection.Field = field;
						this.AddConnection(connection);
					}
				}
			}

			this.Invalidate();
		}


		protected void PushLayout(ObjectBox exclude, PushDirection direction, double margin)
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

		protected ObjectBox PushSearch(ObjectBox box, ObjectBox exclude, double margin)
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


		protected void RedimArea()
		{
			//	Recalcule les dimensions de la surface de travail, en fonction du contenu.
			Rectangle rect = this.ComputeBoxBounds();
			rect.Inflate(Editor.frameMargin);
			this.MoveBoxes(-rect.Left, -rect.Bottom);

			this.AreaSize = rect.Size;
			this.OnAreaSizeChanged();
		}

		protected Rectangle ComputeBoxBounds()
		{
			//	Retourne le rectangle englobant toutes les boîtes.
			Rectangle bounds = Rectangle.Empty;

			for (int i=0; i<this.boxes.Count; i++)
			{
				ObjectBox box = this.boxes[i];
				bounds = Rectangle.Union(bounds, box.Bounds);
			}

			return bounds;
		}

		protected void MoveBoxes(double dx, double dy)
		{
			//	Déplace toutes les boîtes.
			for (int i=0; i<this.boxes.Count; i++)
			{
				ObjectBox box = this.boxes[i];

				Rectangle bounds = box.Bounds;
				bounds.Offset(dx, dy);
				box.Bounds = bounds;
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			this.brutPos = pos;
			pos = this.ConvWidgetToEditor(pos);

			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					this.MouseMove(pos);
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

				case MessageType.MouseLeave:
					this.MouseMove(Point.Zero);
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

		protected void MouseMove(Point pos)
		{
			//	Met en évidence tous les widgets selon la position visée par la souris.
			//	L'objet à l'avant-plan a la priorité.
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
				this.lockObject.MouseMove(pos);
			}
			else
			{
				AbstractObject fly = null;

				for (int i=this.connections.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.connections[i];
					if (obj.MouseMove(pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				for (int i=this.boxes.Count-1; i>=0; i--)
				{
					AbstractObject obj = this.boxes[i];
					if (obj.MouseMove(pos))
					{
						fly = obj;
						pos = Point.Zero;  // si on était dans cet objet -> plus aucun hilite pour les objets placés dessous
					}
				}

				if (fly == null)
				{
					if (this.IsScrollerEnable)
					{
						this.ChangeMouseCursor(MouseCursorType.Hand);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.Arrow);
					}
				}
				else
				{
					if (fly.HilitedElement == AbstractObject.ActiveElement.HeaderDragging && this.BoxCount > 1)
					{
						this.ChangeMouseCursor(MouseCursorType.Move);
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.None ||
							 fly.HilitedElement == AbstractObject.ActiveElement.Inside ||
							 fly.HilitedElement == AbstractObject.ActiveElement.HeaderDragging ||
							 fly.HilitedElement == AbstractObject.ActiveElement.ConnectionHilited)
					{
						this.ChangeMouseCursor(MouseCursorType.Arrow);
					}
					else if (fly.HilitedElement == AbstractObject.ActiveElement.FieldNameSelect ||
							 fly.HilitedElement == AbstractObject.ActiveElement.FieldTypeSelect)
					{
						this.ChangeMouseCursor(MouseCursorType.Grid);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.Finger);
					}
				}
			}
		}

		protected void MouseDown(Point pos)
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
					obj.MouseDown(pos);
				}
			}
		}

		protected void MouseUp(Point pos)
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
					obj.MouseUp(pos);
				}
			}
		}

		public void LockObject(AbstractObject obj)
		{
			this.lockObject = obj;
		}

		protected AbstractObject DetectObject(Point pos)
		{
			//	Détecte l'objet visé par la souris.
			//	L'objet à l'avant-plan a la priorité.
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

			Color colorOver = adorner.ColorBorder;
			colorOver.A = 0.3;
			graphics.RenderSolid(colorOver);

			//	Dessine tous les objets.
			foreach (AbstractObject obj in this.boxes)
			{
				obj.Draw(graphics);
			}

			foreach (AbstractObject obj in this.connections)
			{
				obj.Draw(graphics);
			}

			graphics.Transform = initialTransform;

			//	Dessine le cadre.
			rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}


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

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			this.Window.MouseCursor = this.MouseCursor;
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

		public event Support.EventHandler AreaSizeChanged
		{
			add
			{
				this.AddUserEventHandler ("AreaSizeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("AreaSizeChanged", value);
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

		public event Support.EventHandler AreaOffsetChanged
		{
			add
			{
				this.AddUserEventHandler ("AreaOffsetChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("AreaOffsetChanged", value);
			}
		}
		#endregion


		protected static readonly double defaultWidth = 200;
		protected static readonly double connectionDetour = 30;
		protected static readonly double pushMargin = 10;
		protected static readonly double frameMargin = 40;

		protected Module module;
		protected List<ObjectBox> boxes;
		protected List<ObjectConnection> connections;
		protected Size areaSize;
		protected double zoom;
		protected Point areaOffset;
		protected AbstractObject lockObject;
		protected bool isScrollerEnable;
		protected Point brutPos;
		protected bool isAreaMoving;
		protected Point areaMovingInitialPos;
		protected Point areaMovingInitialOffset;
		protected MouseCursorType lastCursor = MouseCursorType.Unknown;
		protected Image mouseCursorFinger;
		protected Image mouseCursorHand;
		protected Image mouseCursorGrid;
	}
}
