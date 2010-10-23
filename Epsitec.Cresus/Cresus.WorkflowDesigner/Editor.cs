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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
			MoveOrEdit,
			HorizontalMove,
			VerticalMove,
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

			this.nodes    = new List<ObjectNode> ();
			this.edges    = new List<ObjectEdge> ();
			this.balloons = new List<BalloonObject> ();

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
			if (!this.RestoreDesign ())
			{
				this.initialNodePos = new Point (0, 100);
				this.initialEdgePos = new Point (150, 100);

				var list = Entity.DeepSearch (this.workflowDefinitionEntity);
				bool isRoot = true;

				foreach (var entity in list)
				{
					if (entity is WorkflowEdgeEntity)
					{
						var edge = new ObjectEdge (this, entity as WorkflowEdgeEntity);
						this.AddEdge (edge);

						edge.Bounds = new Rectangle (this.initialEdgePos, edge.Bounds.Size);
						this.initialEdgePos.Y += 80;
					}

					if (entity is WorkflowNodeEntity)
					{
						var node = new ObjectNode (this, entity as WorkflowNodeEntity);
						node.IsRoot = isRoot;
						this.AddNode (node);

						node.Bounds = new Rectangle (this.initialNodePos, node.Bounds.Size);
						this.initialNodePos.Y += 80;

						isRoot = false;
					}
				}
			}

			foreach (var obj in this.LinkableObjects)
			{
				obj.CreateInitialLinks ();
			}

			this.cartridge = new ObjectCartridge (this, this.workflowDefinitionEntity);

			this.UpdateAfterGeometryChanged (null);
		}

		private bool RestoreDesign()
		{
			string s = this.workflowDefinitionEntity.SerializedDesign.Data;

			if (string.IsNullOrEmpty (s))
			{
				return false;
			}

			XDocument doc = XDocument.Parse (s, LoadOptions.None);
			XElement store = doc.Element ("Store");

			foreach (var element in store.Elements ("Object"))
			{
				string key  = (string) element.Attribute ("Entity");
				string type = (string) element.Attribute ("Type");

				AbstractEntity entity = null;
				AbstractObject obj = null;

				if (key != "null")
				{
					EntityKey? entityKey = EntityKey.Parse (key);
					entity = this.BusinessContext.DataContext.ResolveEntity (entityKey);
				}

				switch (type)
				{
					case "ObjectNode":
						obj = new ObjectNode (this, entity);
						break;

					case "ObjectEdge":
						obj = new ObjectEdge (this, entity);
						break;
				}

				obj.Deserialize (element);

				if (obj is ObjectNode)
				{
					this.AddNode (obj as ObjectNode);
				}

				if (obj is ObjectEdge)
				{
					this.AddEdge (obj as ObjectEdge);
				}
			}

			return true;
		}

		public void SaveDesign()
		{
			System.DateTime now = System.DateTime.Now.ToUniversalTime ();
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			XDocument doc = new XDocument (
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("Saved on " + timeStamp),
				new XElement ("Store", this.ObjectsElements));

			string s = doc.ToString (SaveOptions.DisableFormatting);

			//?this.workflowDefinitionEntity.SerializedDesign.Id = "WorkflowDesigner";
			//?this.workflowDefinitionEntity.SerializedDesign.Data = s;
		}

		private IEnumerable<XElement> ObjectsElements
		{
			get
			{
				foreach (var obj in this.AllObjects)
				{
					var xml = new XElement ("Object");

					string type = obj.GetType ().ToString ();
					string[] types = type.Split ('.');
					xml.Add (new XAttribute ("Type", types.Last ()));

					string entityKey;
					if (obj.AbstractEntity.UnwrapNullEntity () == null)
					{
						entityKey = "null";
					}
					else
					{
						var key = this.BusinessContext.DataContext.GetNormalizedEntityKey (obj.AbstractEntity);
						entityKey = key.ToString ();
					}
					xml.Add (new XAttribute ("Entity", entityKey));

					obj.Serialize (xml);

					yield return xml;
				}
			}
		}


		public int LinkableObjectsCount
		{
			//	Retourne le nombre de boîtes existantes.
			get
			{
				return this.LinkableObjects.Count ();
			}
		}


		public LinkableObject SearchInitialObject(AbstractEntity entity)
		{
			//	Cherche un objet d'après l'entité qu'il représente.
			//	On ne peut chercher ainsi que les objets initiaux, qui ont été sauvés dans la base.
			//	Les entités créées par après n'ont pas de clés !
			if (entity.IsNotNull ())
			{
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
			}

			return null;
		}


		public void OutputDebugInformations()
		{
			System.Diagnostics.Debug.WriteLine ("");
			System.Diagnostics.Debug.WriteLine ("Linkables:");
			foreach (var obj in this.LinkableObjects)
			{
				System.Diagnostics.Debug.WriteLine (obj.DebugInformations);
			}

			System.Diagnostics.Debug.WriteLine ("");
			System.Diagnostics.Debug.WriteLine ("Links:");
			foreach (var link in this.LinkObjects)
			{
				System.Diagnostics.Debug.WriteLine (link.DebugInformations);
			}
		}


		public void AddNode(ObjectNode node)
		{
			//	Ajoute une nouvelle boîte dans l'éditeur.
			//	La position initiale n'a pas d'importance. La première boîte ajoutée (la boîte racine)
			//	est positionnée par RedimArea(). La position des autres est de toute façon recalculée en
			//	fonction de la boîte parent.
			this.nodes.Add (node);
		}

		public void AddEdge(ObjectEdge edge)
		{
			//	Ajoute une nouvelle liaison dans l'éditeur.
			this.edges.Add (edge);
		}

		public void AddBalloon(BalloonObject balloon)
		{
			//	Ajoute un nouveau commentaire dans l'éditeur.
			this.balloons.Add (balloon);
		}

		public void RemoveBalloon(BalloonObject balloon)
		{
			this.balloons.Remove (balloon);
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
			this.balloons.Clear ();
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

		public AbstractObject EditableObject
		{
			get
			{
				return this.editableObject;
			}
			set
			{
				this.editableObject = value;
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
			this.UpdateObjects ();
			this.UpdateLinks ();
			this.RedimArea ();
			this.UpdateLinks ();
		}

		public void UpdateAfterCommentChanged()
		{
			//	Appelé lorsqu'un commentaire ou une information a changé.
			this.UpdateObjects ();
			this.RedimArea ();
			this.UpdateLinks ();
			this.RedimArea();
			this.UpdateLinks ();
		}

		public void UpdateAfterGeometryChanged(AbstractObject node)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateObjects ();
			this.PushLayout (node, PushDirection.Automatic, this.gridStep);
			this.RedimArea ();
			this.UpdateLinks ();
			this.RedimArea ();
			this.UpdateLinks ();
		}

		public void UpdateObjects()
		{
			foreach (var obj in this.LinkableObjects)
			{
				obj.UpdateObject ();
			}
		}

		public void UpdateLinks()
		{
			//	Met à jour la géométrie de toutes les liaisons.
			foreach (var obj in this.LinkObjects)
			{
				obj.UpdateLink ();
			}

			this.Invalidate ();
		}


		public bool IsEmptyArea(Rectangle area, AbstractObject filteredObject)
		{
			//	Retourne true si une zone est entièrement vide (aucune boîte, on ignore les connexions).
			foreach (var obj in this.LinkableObjects)
			{
				if (obj != filteredObject &&  obj.Bounds.IntersectsWith (area))
				{
					return false;
				}
			}

			return true;
		}


		public void CloseObject(LinkableObject obj)
		{
			//	Ferme une boîte et supprme l'entité associés.
			foreach (var link in this.LinkObjects.ToArray ())
			{
				if (link.DstObject == obj)
				{
					link.DstObject = null;
					link.SrcObject.RemoveEntityLink (obj);
					link.SetStumpAngle (link.GetAngle ());
				}
			}

			if (obj is ObjectNode)
			{
				this.nodes.Remove (obj as ObjectNode);
			}

			if (obj is ObjectEdge)
			{
				this.edges.Remove (obj as ObjectEdge);
			}

			//	Supprime l'entité dans la base.
			bool isPublic = false;

			if (obj is ObjectNode)
			{
				var node = obj as ObjectNode;
				isPublic = node.Entity.IsPublic;
			}

			if (!isPublic)
			{
				this.businessContext.DataContext.DeleteEntity (obj.AbstractEntity);
			}

			this.SetLocalDirty ();
		}


		public LinkableObject DetectLinkableObject(Point pos, System.Type filteredType)
		{
			foreach (var obj in this.LinkableObjects)
			{
				if (obj.GetType () != filteredType && obj.Bounds.Contains (pos))
				{
					return obj;
				}
			}

			return null;
		}


		private void PushLayout(AbstractObject exclude, PushDirection direction, double margin)
		{
			//	Pousse les boîtes pour éviter tout chevauchement.
			//	Une boîte peut être poussée hors de la surface de dessin.
			for (int max=0; max<100; max++)
			{
				bool push = false;

				foreach (var obj in this.PushableObjects)
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

		private AbstractObject PushSearch(AbstractObject node, AbstractObject exclude, double margin)
		{
			//	Cherche une boîte qui chevauche 'node'.
			Rectangle rect = node.Bounds;
			rect.Inflate (margin);

			foreach (var obj in this.PushableObjects)
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

		private void PushAction(AbstractObject node, AbstractObject inter, PushDirection direction, double margin)
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

			inter.Bounds = rect;
		}


		private void RedimArea()
		{
			//	Recalcule les dimensions de la surface de travail, en fonction du contenu.
			Rectangle rect = this.ComputeObjectsBounds();

#if false
			bool iGrid = this.grid;
			this.grid = true;
			rect = this.AreaGridAlign (rect);
			this.grid = iGrid;
#endif

			this.MoveObjects(-rect.Left, -rect.Bottom);
			this.UpdateObjectButtonsGeometry ();

			this.AreaSize = rect.Size;
			this.OnAreaSizeChanged();
		}

		private Rectangle ComputeObjectsBounds()
		{
			//	Retourne le rectangle englobant tous les objets.
			Rectangle bounds = Rectangle.Empty;

			foreach (var obj in this.AllObjects)
			{
				Rectangle b = obj.ExtendedBounds;
				b.Inflate (obj.RedimMargin);

				bounds = Rectangle.Union (bounds, b);
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

			foreach (var obj in this.AllObjects)
			{
				obj.Move (dx, dy);
			}
		}

		private void UpdateObjectButtonsGeometry()
		{
			foreach (var obj in this.AllObjects)
			{
				obj.UpdateButtonsGeometry ();
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
					if (!message.IsControlPressed && !this.IsEditing && this.editableObject != null)
					{
						this.editableObject.StartEdition ();
					}

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
				this.MouseMoveUpdateObjects (pos);

				MouseCursorType type = MouseCursorType.Unknown;

				if (this.hilitedObject != null)
				{
					this.hilitedObject.MouseMove (message, pos);
				}

				if (this.hilitedObject == null)
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
					if (this.hilitedObject.HilitedElement == ActiveElement.EdgeHeader ||
						this.hilitedObject.HilitedElement == ActiveElement.NodeHeader ||
						this.hilitedObject.HilitedElement == ActiveElement.CartridgeEditName ||
						this.hilitedObject.HilitedElement == ActiveElement.CartridgeEditDescription)
					{
						if (this.LinkableObjectsCount > 1)
						{
							type = MouseCursorType.MoveOrEdit;
						}
						else
						{
							type = MouseCursorType.Arrow;
						}
					}
					else if (this.hilitedObject.HilitedElement == ActiveElement.None ||
							 this.hilitedObject.HilitedElement == ActiveElement.NodeInside ||
							 this.hilitedObject.HilitedElement == ActiveElement.EdgeInside ||
							 this.hilitedObject.HilitedElement == ActiveElement.EdgeHilited)
					{
						type = MouseCursorType.Arrow;
					}
					else if (this.hilitedObject.HilitedElement == ActiveElement.EdgeEditDescription)
					{
						type = MouseCursorType.IBeam;
					}
					else if (this.hilitedObject.HilitedElement == ActiveElement.CommentEdit)
					{
						type = MouseCursorType.IBeam;
					}
					else if (this.hilitedObject.HilitedElement == ActiveElement.CommentMove ||
							 this.hilitedObject.HilitedElement == ActiveElement.InfoMove ||
							 this.hilitedObject.HilitedElement == ActiveElement.CartridgeMove)
					{
						type = MouseCursorType.Move;
					}
					else if (this.hilitedObject.HilitedElement >= ActiveElement.InfoLine1 &&
							 this.hilitedObject.HilitedElement <= ActiveElement.InfoLine1+ObjectInfo.maxLines)
					{
						type = MouseCursorType.VerticalMove;
					}
					else
					{
						type = MouseCursorType.Finger;
					}
				}

				this.ChangeMouseCursor(type);
			}
		}

		private void MouseMoveUpdateObjects(Point pos)
		{
			//	Met à jour tous les objets suite à un déplacement de la souris.
			//	On parcourt les objets dans le même ordre que le dessin.
			//	Il faut parcourir tous les objets, pour les mettre à jour.
			this.hilitedObject = null;
			ActiveElement hilitedElement = ActiveElement.None;

			foreach (var obj in this.AllObjects)
			{
				ActiveElement element = obj.MouseDetectBackground (pos);
				if (element != ActiveElement.None)
				{
					this.hilitedObject = obj;
					hilitedElement = element;
				}
			}

			foreach (var obj in this.AllObjects)
			{
				ActiveElement element = obj.MouseDetectForeground (pos);
				if (element != ActiveElement.None)
				{
					this.hilitedObject = obj;
					hilitedElement = element;
				}
			}

			//	Lorsque la souris survole un objet, tous les autres deviennent estompés.
			//	Seuls restent normalement affichés l'objet survolé et ses 'amis'.
			bool hasDimmned = (hilitedElement != ActiveElement.None);

			List<AbstractObject> friendObjects = null;
			if (this.hilitedObject != null && hasDimmned)
			{
				friendObjects = this.hilitedObject.FriendObjects;
			}

			foreach (var obj in this.AllObjects)
			{
				if (obj == this.hilitedObject)
				{
					obj.HilitedElement = hilitedElement;
					obj.IsDimmed = false;
				}
				else
				{
					obj.HilitedElement = ActiveElement.None;
					obj.IsDimmed = hasDimmned  && (friendObjects == null || !friendObjects.Contains (obj));
				}
			}

			this.Invalidate ();
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

				if (this.hilitedObject != null)
				{
					this.hilitedObject.MouseDown (message, pos);
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
				this.editableObject = this.hilitedObject;  // avant MouseUp !

				if (this.hilitedObject != null)
				{
					this.hilitedObject.MouseUp (message, pos);
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


		public void ClearHilited()
		{
			foreach (var obj in this.AllObjects)
			{
				obj.HilitedElement = ActiveElement.None;
				obj.IsDimmed = false;
			}

			this.Invalidate ();
		}

		public void CompactAll()
		{
			foreach (var obj in this.LinkableObjects)
			{
				obj.IsExtended = false;
			}
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
			foreach (var obj in this.AllObjects)
			{
				obj.DrawBackground (graphics);
			}

			//	Dessine l'avant plan tous les objets.
			foreach (var obj in this.AllObjects)
			{
				obj.DrawForeground (graphics);
			}
		}


		#region Enumerators
		private IEnumerable<AbstractObject> AllObjects
		{
			//	Cet énumérateur détermine, entre autres, l'ordre dans lequel sont dessinés les objets.
			get
			{
				if (this.cartridge != null)
				{
					yield return this.cartridge;
				}

				LinkableObject top = null;

				foreach (var obj in this.nodes)
				{
					if (obj.IsExtended)
					{
						top = obj;
					}
					else
					{
						yield return obj;
					}
				}

				foreach (var obj in this.edges)
				{
					if (obj.IsExtended)
					{
						top = obj;
					}
					else
					{
						yield return obj;
					}
				}

				foreach (var obj in this.LinkObjects)
				{
					yield return obj;
				}

				foreach (var obj in this.balloons)
				{
					yield return obj;
				}

				if (top != null)
				{
					yield return top;
				}
			}
		}

		public IEnumerable<AbstractObject> PushableObjects
		{
			get
			{
				if (this.cartridge != null)
				{
					yield return this.cartridge;
				}

				foreach (var obj in this.LinkableObjects)
				{
					yield return obj;
				}
			}
		}

		public IEnumerable<ObjectLink> LinkObjects
		{
			get
			{
				foreach (var obj in this.LinkableObjects)
				{
					foreach (var link in obj.ObjectLinks)
					{
						yield return link;
					}
				}
			}
		}

		private IEnumerable<LinkableObject> LinkableObjects
		{
			get
			{
				LinkableObject top = null;

				foreach (var obj in this.nodes)
				{
					if (obj.IsExtended)
					{
						top = obj;
					}
					else
					{
						yield return obj;
					}
				}

				foreach (var obj in this.edges)
				{
					if (obj.IsExtended)
					{
						top = obj;
					}
					else
					{
						yield return obj;
					}
				}

				if (top != null)
				{
					yield return top;
				}
			}
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
					this.SetMouseCursorImage (ref this.mouseCursorMove, Misc.Icon ("CursorMove"));
					break;

				case MouseCursorType.MoveOrEdit:
					this.SetMouseCursorImage (ref this.mouseCursorMoveOrEdit, Misc.Icon ("CursorMoveOrEdit"));
					break;

				case MouseCursorType.HorizontalMove:
					this.SetMouseCursorImage (ref this.mouseCursorHorizontalMove, Misc.Icon ("CursorHorizontalMove"));
					break;

				case MouseCursorType.VerticalMove:
					this.SetMouseCursorImage (ref this.mouseCursorVerticalMove, Misc.Icon ("CursorVerticalMove"));
					break;

				case MouseCursorType.Hand:
					this.SetMouseCursorImage(ref this.mouseCursorHand, Misc.Icon("CursorHand"));
					break;

				case MouseCursorType.IBeam:
					this.SetMouseCursorImage(ref this.mouseCursorEdit, Misc.Icon("CursorEdit"));
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
		public static readonly double			pushMargin = 10;

		private Core.Business.BusinessContext	businessContext;

		private WorkflowDefinitionEntity		workflowDefinitionEntity;
		private List<ObjectNode>				nodes;
		private List<ObjectEdge>				edges;
		private List<BalloonObject>				balloons;
		private ObjectCartridge					cartridge;

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
		private Image							mouseCursorEdit;
		private Image							mouseCursorMove;
		private Image							mouseCursorMoveOrEdit;
		private Image							mouseCursorHorizontalMove;
		private Image							mouseCursorVerticalMove;
		private Image							mouseCursorGrid;
		private Image							mouseCursorLocate;
		private VScroller						vscroller;
		private AbstractObject					hilitedObject;
		private bool							dirtySerialization;
		private bool							grid;
		private double							gridStep;
		private double							gridSubdiv;
		private AbstractObject					editingObject;
		private AbstractObject					editableObject;
		private Point							initialNodePos;
		private Point							initialEdgePos;
	}
}
