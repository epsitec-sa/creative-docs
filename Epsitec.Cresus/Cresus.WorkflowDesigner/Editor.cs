//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using Epsitec.Cresus.WorkflowDesigner.Objects;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	/// <summary>
	/// Widget permettant d'�diter graphiquement des entit�s.
	/// </summary>
	public class Editor : Widget, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public enum ModifyMode
		{
			Locked,
			Partial,
			Unlocked,
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

			this.verticalMagnetConstrains   = new List<MagnetConstrain> ();
			this.horizontalMagnetConstrains = new List<MagnetConstrain> ();

			this.zoom = 1;
			this.areaOffset = Point.Zero;
			this.nextUniqueId = 1;

			//	Le Widgets.Timer s'ex�cute dans la bouche d'�v�nement, � l'inverse de System.Timer qui
			//	s'ex�cute dans un autre Thread. Il n'y a donc aucun souci d'ex�cution simultan�e du code.
			double delay = 1.0/Editor.dimmedFrequency;

			this.timer = new Timer ();
			this.timer.AutoRepeat = delay;
			this.timer.HigherAccuracy = true;
			this.timer.TimeElapsed += new EventHandler (this.HandleTimerElapsed);
			this.timer.Start ();
		}

		public Editor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.timer.Dispose ();
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
			//	Force the business context to consider the data as dirty, even if the entities
			//	have not been edited (yet):

			this.businessContext.NotifyExternalChanges ();
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
			//	Cr�e le workflow initial, soit en d�s�rialisant le diagramme, soit un injectant toutes
			//	les entit�s point�es � partir de la d�finition.
			if (this.RestoreDesign ())
			{
				this.UpdateUniqueId ();
			}
			else
			{
				//	D�s�rialisation �chou�e.
				this.initialNodePos = new Point (  0, 150);
				this.initialEdgePos = new Point (150, 150);

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

				this.cartridge = new ObjectCartridge (this, this.workflowDefinitionEntity);

				foreach (var obj in this.LinkableObjects)
				{
					obj.CreateInitialLinks ();
				}
			}

			this.UpdateAfterGeometryChanged (null);
		}

		#region Serialization
		private bool RestoreDesign()
		{
			//	Recr�e tout le diagramme � partir des donn�es s�rialis�es.
			string xmlSource = Editor.RestoreData (this.workflowDefinitionEntity);

			if (string.IsNullOrEmpty (xmlSource))
			{
				return false;
			}

			XDocument doc = XDocument.Parse (xmlSource, LoadOptions.None);
			XElement store = doc.Element ("Store");

			foreach (var element in store.Elements ("Object"))
			{
				string key  = (string) element.Attribute ("Entity");
				string type = (string) element.Attribute ("Type");

				AbstractEntity entity = null;
				AbstractObject obj = null;

				if (!string.IsNullOrEmpty (key))
				{
					EntityKey? entityKey = EntityKey.Parse (key);
					entity = this.BusinessContext.DataContext.ResolveEntity (entityKey);
				}

				obj = this.CreateObject (type, entity);

				if (obj == null)
				{
					continue;
				}

				obj.Deserialize (element);

				dynamic dynamicObj = obj;
				this.AddDeserializedObject (dynamicObj);
			}

			return true;
		}

		private AbstractObject CreateObject(string typeName, AbstractEntity entity)
		{
			string fullTypeName = "Epsitec.Cresus.WorkflowDesigner.Objects." + typeName;
			System.Type type = System.Type.GetType (fullTypeName);

			if (type == null)
			{
				return null;
			}

			object[] constructorArguments = new object[] { this, entity };
			return System.Activator.CreateInstance (type, constructorArguments) as AbstractObject;
		}

		private void AddDeserializedObject(ObjectCartridge cartridge)
		{
			this.cartridge = cartridge;
		}

		private void AddDeserializedObject(ObjectNode node)
		{
			this.AddNode (node);
		}

		private void AddDeserializedObject(ObjectEdge edge)
		{
			edge.ObjectLinks.Clear ();

			this.AddEdge (edge);
		}

		private void AddDeserializedObject(ObjectLink link)
		{
			link.SrcObject.ObjectLinks.Add (link);
		}

		private void AddDeserializedObject(ObjectComment balloon)
		{
			this.AddBalloon (balloon);

			if (balloon.AttachObject is LinkableObject)
			{
				var linkable = balloon.AttachObject as LinkableObject;
				linkable.Comment = balloon;
			}

			if (balloon.AttachObject is ObjectLink)
			{
				var link = balloon.AttachObject as ObjectLink;
				link.Comment = balloon;
			}
		}

		private void AddDeserializedObject(ObjectInfo balloon)
		{
			this.AddBalloon (balloon);

			if (balloon.AttachObject is LinkableObject)
			{
				var linkable = balloon.AttachObject as LinkableObject;
				linkable.Info = balloon;
			}
		}

	
		public void SaveDesign()
		{
			//	Sauve tout le diagramme.
			System.DateTime now = System.DateTime.Now.ToUniversalTime ();
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			XDocument doc = new XDocument
			(
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("Saved on " + timeStamp),
				new XElement ("Store", this.GetSaveObjectsElements ())
			);

			string xmlSource = doc.ToString (SaveOptions.DisableFormatting);

			Editor.SaveData (this.workflowDefinitionEntity, xmlSource);
		}

		private IEnumerable<XElement> GetSaveObjectsElements()
		{
			foreach (var obj in this.ObjectsToSave)
			{
				var xml = new XElement ("Object");

				System.Type type = obj.GetType ();
				xml.Add (new XAttribute ("Type", type.Name));

				if (obj.AbstractEntity.IsNotNull ())
				{
					string entityKey = this.GetEntityKey (obj.AbstractEntity);
					xml.Add (new XAttribute ("Entity", entityKey));
				}

				obj.Serialize (xml);

				yield return xml;
			}
		}

		private string GetEntityKey(AbstractEntity entity)
		{
			var key = this.BusinessContext.DataContext.GetNormalizedEntityKey (entity);
			return key.ToString ();
		}


		private static void SaveData(WorkflowDefinitionEntity def, string xmlSource)
		{
			def.SerializedDesign.Id = "WorkflowDesigner";
			def.SerializedDesign.Data = Editor.EncodeString (xmlSource);
		}

		private static string RestoreData(WorkflowDefinitionEntity def)
		{
			byte[] binaryData = def.SerializedDesign.Data;
			return Editor.DecodeString (binaryData);
		}


		private static byte[] EncodeString(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return new byte[0];
			}
			else
			{
				return System.Text.Encoding.UTF8.GetBytes (value);
			}
		}

		private static string DecodeString(byte[] value)
		{
			if ((value == null) ||
				(value.Length == 0))
			{
				return "";
			}
			else
			{
				return System.Text.Encoding.UTF8.GetString (value);
			}
		}
		#endregion


		public int GetNextUniqueId()
		{
			return this.nextUniqueId++;
		}

		public AbstractObject Search(int uniqueId)
		{
			//	Retourne un objet quelconque d'apr�s son identificateur unique.
			if (uniqueId == 0)
			{
				return null;
			}
			else
			{
				return this.AllObjects.Where (x => x.UniqueId == uniqueId).FirstOrDefault ();
			}
		}

		private void UpdateUniqueId()
		{
			//	R�initialise le g�n�rateur d'identificateurs uniques.
			int max = 0;

			foreach (var obj in this.AllObjects)
			{
				max = System.Math.Max (obj.UniqueId, max);
			}

			this.nextUniqueId = max+1;
		}


		public LinkableObject SearchInitialObject(AbstractEntity entity)
		{
			//	Cherche un objet d'apr�s l'entit� qu'il repr�sente.
			//	On ne peut chercher ainsi que les objets initiaux, qui ont �t� sauv�s dans la base.
			//	Les entit�s cr��es par apr�s n'ont pas de cl�s !
			if (entity.IsNotNull ())
			{
				var searchedKey = this.businessContext.DataContext.GetNormalizedEntityKey (entity);

				foreach (var obj in this.LinkableObjects)
				{
					var key = this.businessContext.DataContext.GetNormalizedEntityKey (obj.AbstractEntity);

					if (key == searchedKey)
					{
						return obj;
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
			//	Ajoute une nouvelle bo�te dans l'�diteur.
			//	La position initiale n'a pas d'importance. La premi�re bo�te ajout�e (la bo�te racine)
			//	est positionn�e par RedimArea(). La position des autres est de toute fa�on recalcul�e en
			//	fonction de la bo�te parent.
			this.nodes.Add (node);
		}

		public void AddEdge(ObjectEdge edge)
		{
			//	Ajoute une nouvelle liaison dans l'�diteur.
			this.edges.Add (edge);
		}

		public void AddBalloon(BalloonObject balloon)
		{
			//	Ajoute un nouveau commentaire dans l'�diteur.
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


		public Size AreaSize
		{
			//	Dimensions de la surface pour repr�senter les bo�tes et les liaisons.
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
			//	Zoom pour repr�senter les bo�tes et les liaisons.
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
			//	Offset de la zone visible, d�termin�e par les ascenseurs.
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
			//	Met � jour la g�om�trie de toutes les bo�tes et de toutes les liaisons.
			this.UpdateObjects ();
			this.UpdateLinks ();
			this.RedimArea ();
			this.UpdateLinks ();
		}

		public void UpdateAfterCommentChanged()
		{
			//	Appel� lorsqu'un commentaire ou une information a chang�.
			this.UpdateObjects ();
			this.RedimArea ();
			this.UpdateLinks ();
			this.RedimArea();
			this.UpdateLinks ();
		}

		public void UpdateAfterGeometryChanged(AbstractObject node)
		{
			//	Appel� lorsque la g�om�trie d'une bo�te a chang� (changement compact/�tendu).
			this.UpdateObjects ();
			this.PushLayout (node, PushDirection.Automatic, Editor.pushMargin);
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
			//	Met � jour la g�om�trie de toutes les liaisons.
			foreach (var obj in this.LinkObjects)
			{
				obj.UpdateLink ();
			}

			this.Invalidate ();
		}


		public bool IsEmptyArea(Rectangle area, AbstractObject filteredObject)
		{
			//	Retourne true si une zone est enti�rement vide (aucune bo�te, on ignore les connexions).
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
			//	Ferme une bo�te et supprime l'entit� associ�e.
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

			//	Supprime l'entit� dans la base.
			bool delete = true;

			if (obj is ObjectNode)
			{
				var node = obj as ObjectNode;
				delete = this.IsDeletePossible (node.Entity);
			}

			if (delete)
			{
				this.businessContext.DataContext.DeleteEntity (obj.AbstractEntity);
			}

			this.SetLocalDirty ();
		}

		private bool IsDeletePossible(WorkflowNodeEntity nodeEntity)
		{
			//	Les noeuds publiques ne sont jamais supprim�s dans la base.
			if (nodeEntity.IsPublic)
			{
				return false;
			}

			//	Si le noeud est r�f�renc� par un 'edge' qui ne fait pas partie de ce diagramme,
			//	il ne faut pas le supprimer.
			var list = Entity.DeepSearch (this.workflowDefinitionEntity);

			var edgeEntities = this.FindEdges (nodeEntity);
			foreach (var edgeEntity in edgeEntities)
			{
				if (!list.Contains (edgeEntity))  // edge d'un autre diagramme ?
				{
					return false;
				}
			}

			return true;
		}

		private List<WorkflowEdgeEntity> FindEdges(WorkflowNodeEntity nodeEntity)
		{
			//	Cherche toutes les entit�s 'edge' qui pointent un 'node' donn�.
			WorkflowEdgeEntity example = new WorkflowEdgeEntity ()
			{
				NextNode = nodeEntity,
			};

			Request request = new Request ()
			{
				RootEntity = example,
			};

			return this.businessContext.DataContext.GetByRequest<WorkflowEdgeEntity> (request).ToList ();
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
			//	Pousse les bo�tes pour �viter tout chevauchement.
			//	Une bo�te peut �tre pouss�e hors de la surface de dessin.
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
			//	Cherche une bo�te qui chevauche 'node'.
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
			//	Pousse 'inter' pour venir apr�s 'node' selon la direction choisie.
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

			this.MoveObjects(-rect.Left, -rect.Bottom);
			this.UpdateObjectGeometry ();

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
			//	D�place tous les objets.
			if (dx == 0 && dy == 0)  // immobile ?
			{
				return;
			}

			foreach (var obj in this.AllObjects)
			{
				obj.Move (dx, dy);
			}
		}

		private void UpdateObjectGeometry()
		{
			foreach (var obj in this.AllObjects)
			{
				obj.UpdateGeometry ();
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			this.brutPos = pos;
			pos = this.ConvWidgetToEditor(pos);

			this.lastMessageType = message.MessageType;
			this.lastMessagePos = pos;

			//-System.Diagnostics.Debug.WriteLine(string.Format("Type={0}", message.MessageType));

			this.TryProcessDimmed ();

			switch (message.MessageType)
			{
				case MessageType.KeyDown:
				case MessageType.KeyUp:
					//	Ne consomme l'�v�nement que si on l'a bel et bien reconnu ! Evite
					//	qu'on ne mange les raccourcis clavier g�n�raux (Alt-F4, CTRL-S, ...)
					if (!message.IsControlPressed && !this.IsEditing && this.editableObject != null)
					{
						this.editableObject.StartEdition ();
					}

					if (this.IsEditing)  // �dition en cours ?
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
			//	Conversion d'une coordonn�e dans l'espace normal des widgets vers l'espace de l'�diteur,
			//	qui varie selon les ascenseurs (AreaOffset) et le zoom.
			pos.Y = this.Client.Size.Height-pos.Y;
			pos /= this.zoom;
			pos += this.areaOffset;
			pos.Y = this.areaSize.Height-pos.Y;

			return pos;
		}

		public Point ConvEditorToWidget(Point pos)
		{
			//	Conversion d'une coordonn�e dans l'espace de l'�diteur vers l'espace normal des widgets.
			pos.Y = this.areaSize.Height-pos.Y;
			pos -= this.areaOffset;
			pos *= this.zoom;
			pos.Y = this.Client.Size.Height-pos.Y;

			return pos;
		}

		private void EditorMouseMove(Message message, Point pos)
		{
			//	Met en �vidence tous les widgets selon la position vis�e par la souris.
			//	L'objet � l'avant-plan a la priorit�.
			if (this.IsEditing)  // �dition en cours ?
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
				this.ChangeMouseCursor (this.lockObject.MouseCursor);
				this.lockObject.MouseMove (message, pos);
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
					if (this.hilitedObject.HilitedElement == ActiveElement.None)
					{
						type = MouseCursorType.Arrow;
					}
					else
					{
						type = this.hilitedObject.MouseCursor;
					}
				}

				this.ChangeMouseCursor(type);
			}
		}

		private void MouseMoveUpdateObjects(Point pos)
		{
			//	Met � jour tous les objets suite � un d�placement de la souris.
			//	On parcourt les objets dans le m�me ordre que le dessin.
			//	Il faut parcourir tous les objets, pour les mettre � jour.
			if (pos.IsZero)
			{
				return;
			}

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

			//	Lorsque la souris survole un objet, tous les autres deviennent estomp�s.
			//	Seuls restent normalement affich�s l'objet survol� et ses 'amis'.
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
			//	D�but du d�placement d'une bo�te.
			if (this.lastCursor == MouseCursorType.Hand)
			{
				this.isAreaMoving = true;
				this.areaMovingInitialPos = this.brutPos;
				this.areaMovingInitialOffset = this.areaOffset;
			}
			else
			{
				if (this.IsEditing)  // �dition en cours ?
				{
					this.editingObject.AcceptEdition ();
					this.editingObject = null;

					this.Invalidate ();
				}

				if (this.hilitedObject != null)
				{
					this.lockObject = this.hilitedObject;
					this.hilitedObject.MouseDown (message, pos);
				}
			}
		}

		private void EditorMouseUp(Message message, Point pos)
		{
			//	Fin du d�placement d'une bo�te.
			if (this.isAreaMoving)
			{
				this.isAreaMoving = false;
			}
			else
			{
				this.editableObject = this.hilitedObject;  // avant MouseUp !

				if (this.hilitedObject != null)
				{
					this.lockObject = null;
					this.hilitedObject.MouseUp (message, pos);
				}

				this.MagnetConstrainClear ();
			}
		}


		#region Magnet constrains
		private void MagnetConstrainClear()
		{
			foreach (var obj in this.AllObjects)
			{
				obj.IsVerticalMagneted   = false;
				obj.IsHorizontalMagneted = false;
			}

			foreach (var mc in this.MagnetConstrains (null).ToArray ())
			{
				mc.Active = false;
			}
		}

		public Point MagnetConstrainCenter(Point center, AbstractObject obj)
		{
			//	Contraint la position du centre d'un objet.
			obj.IsVerticalMagneted   = false;
			obj.IsHorizontalMagneted = false;

			if (this.verticalMagnetConstrains.Count != 0)
			{
				center.X = this.verticalMagnetConstrains[0].Position;
				obj.IsVerticalMagneted = true;
			}

			if (this.horizontalMagnetConstrains.Count != 0)
			{
				center.Y = this.horizontalMagnetConstrains[0].Position;
				obj.IsHorizontalMagneted = true;
			}

			return center;
		}
		
		public void DetectMagnetConstrains(Point pos, AbstractObject obj)
		{
			//	Active toutes les contraintes qui doivent l'�tre, selon la position de la souris.
			Point center = obj.GetCenter (pos);

			double vMax = double.MaxValue;
			double hMax = double.MaxValue;
			MagnetConstrain h = null;
			MagnetConstrain v = null;

			foreach (var mc in this.MagnetConstrains (obj))
			{
				if (mc.IsVertical)
				{
					double d = System.Math.Abs (center.X-mc.Position);
					if (vMax > d && d <= Editor.magnetConstrainMargin)
					{
						vMax = d;
						v = mc;
					}
				}
				else
				{
					double d = System.Math.Abs (center.Y-mc.Position);
					if (hMax > d && d <= Editor.magnetConstrainMargin)
					{
						hMax = d;
						h = mc;
					}
				}
			}

			this.verticalMagnetConstrains.Clear ();
			this.horizontalMagnetConstrains.Clear ();

			foreach (var mc in this.MagnetConstrains (obj).ToArray ())
			{
				if (v != null && v.IsVertical == mc.IsVertical && v.Position == mc.Position)
				{
					mc.Active = true;
					this.verticalMagnetConstrains.Add (mc);
				}
				else if (h != null && h.IsVertical == mc.IsVertical && h.Position == mc.Position)
				{
					mc.Active = true;
					this.horizontalMagnetConstrains.Add (mc);
				}
				else
				{
					mc.Active = false;
				}
			}
		}

		private IEnumerable<MagnetConstrain> MagnetConstrains(AbstractObject filteredObject)
		{
			//	Retourne toutes les contraintes de tous les objets, sauf celui qui est filtr�.
			foreach (var obj in this.AllObjects)
			{
				if (obj != filteredObject)
				{
					foreach (var mc in obj.MagnetConstrains)
					{
						yield return mc;
					}
				}
			}
		}
		#endregion


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
				graphics.AddFilledRectangle(rect);  // � droite
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

		public void PaintObjects(Graphics graphics)
		{
			//	Dessine l'arri�re-plan de tous les objets.
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


		#region Timer
		private void HandleTimerElapsed(object sender)
		{
			this.ProcessDimmed ();
		}

		private void TryProcessDimmed()
		{
			//	Comme le timer passe par la boucle d'�v�nements, celui-ci est quasiment stopp� lorsque
			//	la boucle d'�v�nements est surcharg�e, par exemple lorsque la souris bouge rapidement.
			//	Cette ex�cution, faite lors de la r�ception de chaque �v�nement, essaie de palier � ce
			//	probl�me.
			long deltaTicks = System.DateTime.Now.Ticks - this.lastTick;
			double delta = deltaTicks / 10000000.0;  // temps �coul� en secondes
			double period = 1.0/Editor.dimmedFrequency;

			if (delta >= period)  // est-ce que le timer aurait d� s'ex�cuter ?
			{
				this.ProcessDimmed ();
			}
		}

		private void ProcessDimmed()
		{
			//	Fait avancer d'un 'step' tous les objets.
			long ticks = System.DateTime.Now.Ticks;
			long deltaTicks = ticks - this.lastTick;
			double delta = deltaTicks / 10000000.0;  // temps �coul� en secondes
			double period = 1.0/Editor.dimmedFrequency;

			double step = period*2.0;  // dur�e de l'effet = 1/2 s
			step *= delta/period;
			
			bool changing = false;

			foreach (var obj in this.AllObjects)
			{
				changing |= obj.ProcessDimmed (step);
			}

			if (changing)  // y a-t-il eu un changement ?
			{
				Application.Invoke (this.Invalidate);
			}

			this.lastTick = ticks;
		}
		#endregion

		#region Enumerators
		private IEnumerable<AbstractObject> ObjectsToSave
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

				foreach (var obj in this.LinkObjects)
				{
					yield return obj;
				}

				foreach (var obj in this.balloons)
				{
					yield return obj;
				}
			}
		}

		private IEnumerable<AbstractObject> AllObjects
		{
			//	Cet �num�rateur d�termine, entre autres, l'ordre dans lequel sont dessin�s les objets.
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
				return this.hilitedObject.GetToolTipText();
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
			//	G�n�re un �v�nement pour dire que les dimensions ont chang�.
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
			//	G�n�re un �v�nement pour dire que l'offset de la surface de travail a chang�.
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
			//	G�n�re un �v�nement pour dire que l'offset de la surface de travail a chang�.
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
			//	G�n�re un �v�nement pour dire que l'�tat d'�dition a chang�.
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
		private static readonly double			magnetConstrainMargin = 10;
		private static readonly double			dimmedFrequency = 10;

		private Core.Business.BusinessContext	businessContext;

		private WorkflowDefinitionEntity		workflowDefinitionEntity;
		private readonly List<ObjectNode>		nodes;
		private readonly List<ObjectEdge>		edges;
		private readonly List<BalloonObject>	balloons;
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
		private VScroller						vscroller;
		private AbstractObject					hilitedObject;
		private AbstractObject					editingObject;
		private AbstractObject					editableObject;
		private Point							initialNodePos;
		private Point							initialEdgePos;
		private int								nextUniqueId;
		private readonly List<MagnetConstrain>	verticalMagnetConstrains;
		private readonly List<MagnetConstrain>	horizontalMagnetConstrains;
		private readonly Timer					timer;
		private long							lastTick;
	}
}
