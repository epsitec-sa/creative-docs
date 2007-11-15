using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor
{
	/// <summary>
	/// Widget venant par-dessus le conteneur UI.Panel pour éditer ce dernier.
	/// </summary>
	public class Editor : AbstractGroup, IPaintFilter
	{
		protected enum Attachment
		{
			None,
			Left,
			Right,
			Bottom,
			Top,
		}

		protected enum MouseCursorType
		{
			Unknown,
			Arrow,
			ArrowPlus,
			Global,
			Grid,
			GridPlus,
			Hand,
			Edit,
			Pen,
			Zoom,
			Finger,
		}


		public Editor() : base()
		{
			this.AutoFocus = true;
			this.InternalState |= InternalState.Focusable;
		}

		public Editor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static Editor()
		{
			Widgets.Helpers.VisualPropertyMetadata metadata = new Widgets.Helpers.VisualPropertyMetadata(ContentAlignment.TopLeft, Widgets.Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Widgets.Visual.ContentAlignmentProperty.OverrideMetadata(typeof(Editor), metadata);
		}


		public void Initialize(Module module, PanelsContext context, UI.Panel panel)
		{
			this.module = module;
			this.context = context;
			this.panel = panel;

			this.objectModifier = new ObjectModifier(this);
		}

		public Module Module
		{
			//	Module associé.
			get
			{
				return this.module;
			}
		}

		public FormDescription Form
		{
			//	Masque de saisie associé.
			get
			{
				return this.form;
			}
			set
			{
				this.form = value;
			}
		}

		public PanelsContext Context
		{
			//	Contexte asocié.
			get
			{
				return this.context;
			}
		}

		public UI.Panel Panel
		{
			//	Panneau associé qui est le conteneur de tous les widgets.
			//	Editor est frère de Panel et vient par-dessus.
			get
			{
				return this.panel;
			}
			set
			{
				this.selectedObjects.Clear();
				this.UpdateAfterChanging(Viewers.Changing.Show);
				
				this.panel = value;
				this.Invalidate();
			}
		}

		public Druid Druid
		{
			//	Druid du conteneur des widgets.
			get
			{
				return this.druid;
			}
			set
			{
				this.druid = value;
			}
		}

		public ObjectModifier ObjectModifier
		{
			//	Retourne le modificateur d'objets.
			get
			{
				return this.objectModifier;
			}
		}

		public List<Widget> SelectedObjects
		{
			//	Retourne la liste des objets sélectionnés.
			get
			{
				return this.selectedObjects;
			}
		}

		public List<int> GetSelectedUniqueId()
		{
			//	Retourne la liste des identificateurs uniques des objets sélectionnés.
			List<int> uniqueIds = new List<int>();

			foreach (Widget obj in this.selectedObjects)
			{
				if (obj.Index != -1)
				{
					uniqueIds.Add(obj.Index);
				}
			}

			return uniqueIds;
		}

		public void SetSelectedUniqueId(List<int> uniqueIds)
		{
			//	Sélectionne tous les objets dont on donne les identificateurs uniques.
			this.selectedObjects.Clear();

			foreach (int uniqueId in uniqueIds)
			{
				Widget obj = this.objectModifier.GetWidget(uniqueId);
				if (obj != null)
				{
					this.selectedObjects.Add(obj);
				}
			}

			this.Invalidate();
		}

		public bool IsEditEnabled
		{
			//	Est-ce que l'édition est possible ? Pour cela, il faut avoir sélectionné un bundle
			//	dans la liste de gauche.
			get
			{
				return this.isEditEnabled;
			}
			set
			{
				this.isEditEnabled = value;
			}
		}

		public void UpdateGeometry()
		{
			//	Mise à jour après avoir changé la géométrie d'un ou plusieurs objets.
		}


		public void DoCommand(string name)
		{
			//	Exécute une commande.
			switch (name)
			{
				case "PanelDelete":
					this.DeleteSelection();
					break;

				case "PanelDuplicate":
					this.DuplicateSelection();
					break;

				case "PanelDeselectAll":
					this.DeselectAll();
					break;

				case "PanelSelectAll":
					this.SelectAll();
					break;

				case "PanelSelectInvert":
					this.SelectInvert();
					break;

				case "PanelSelectRoot":
					this.SelectRoot();
					break;

				case "PanelSelectParent":
					this.SelectParent();
					break;

				case "PanelShowGrid":
					this.context.ShowGrid = !this.context.ShowGrid;
					this.Invalidate();
					this.OnUpdateCommands();
					break;
			}
		}

		public void GetSelectionInfo(out int selected, out int count, out bool isRoot)
		{
			//	Donne des informations sur la sélection en cours.
			selected = this.selectedObjects.Count;
			count = this.panel.Children.Count;
			isRoot = false;
		}

		public string SelectionInfo
		{
			//	Donne le texte pour les statuts.
			get
			{
				return null;
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (!this.isEditEnabled)
			{
				return;
			}

			pos = this.ConvEditorToPanel(pos);

			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					this.SetDirty();
					this.ProcessMouseDown(pos, message.IsRightButton, message.IsControlPressed, message.IsShiftPressed);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					this.ProcessMouseMove(pos, message.IsRightButton, message.IsControlPressed, message.IsShiftPressed);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.ProcessMouseUp(pos, message.IsRightButton, message.IsControlPressed, message.IsShiftPressed);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseWheel:
					message.Consumer = this;
					break;

				case MessageType.MouseLeave:
					this.SetHilitedObject(null);
					break;

				case MessageType.KeyDown:
					this.ProcessKeyChanged(message.IsControlPressed, message.IsShiftPressed);
					break;

				case MessageType.KeyUp:
					this.ProcessKeyChanged(message.IsControlPressed, message.IsShiftPressed);
					break;

				case MessageType.KeyPress:
					break;
			}
		}

		#region ProcessMouse
		void ProcessMouseDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a été pressée.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.CreateObjectDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessMouseMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a été déplacée.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.CreateObjectMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessMouseUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a été relâchée.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.CreateObjectUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a été relâchée.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.CreateObjectKeyChanged(isControlPressed, isShiftPressed);
			}
		}
		#endregion

		#region ProcessMouse select
		protected void SelectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris pressée.
			Widget obj = this.Detect(pos);

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				this.selectedObjects.Clear();
				this.UpdateAfterChanging(Viewers.Changing.Selection);
			}

			if (obj != null)
			{
				if (this.selectedObjects.Contains(obj))
				{
					this.selectedObjects.Remove(obj);
				}
				else
				{
					this.selectedObjects.Add(obj);
				}
				this.UpdateAfterChanging(Viewers.Changing.Selection);
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris déplacée.
			Widget obj = this.Detect(pos);
			this.SetHilitedObject(obj);  // met en évidence l'objet survolé par la souris
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris relâchée.
		}

		protected void SelectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, touche pressée ou relâchée.
			if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Arrow);
			}
		}
		#endregion

		#region ProcessMouse global
		protected void GlobalDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris pressée.
		}

		protected void GlobalMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris déplacée.
		}

		protected void GlobalUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris relâchée.
		}

		protected void GlobalKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, touche pressée ou relâchée.
			if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Global);
			}
		}
		#endregion


		#region ProcessMouse create object
		protected void CreateObjectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris pressée.
		}

		protected void CreateObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Pen);
		}

		protected void CreateObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris relâchée.
		}

		protected void CreateObjectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, touche pressée ou relâchée.
		}
		#endregion


		protected bool ChangeTextResource(Widget obj)
		{
			//	Choix de la ressource (un Druid) pour l'objet.
			//	Retourne false s'il fallait choisir un Druid et que l'utilisateur ne l'a pas fait.
			return true;
		}


		#region Dragging
		protected void DraggingStart(Point pos)
		{
			//	Début du drag pour déplacer les objets sélectionnés.
		}

		protected void DraggingMove(Point pos)
		{
			//	Mouvement du drag pour déplacer les objets sélectionnés.
		}

		protected void DraggingEnd(Point pos)
		{
			//	Fin du drag pour déplacer les objets sélectionnés.
		}
		#endregion


		#region Selection
		protected Widget Detect(Point pos)
		{
			//	Détecte l'objet visé par la souris, avec priorité au dernier objet
			//	dessiné (donc placé dessus).
			Widget detected = this.Detect(pos, this.panel);
			if (detected == null && this.selectedObjects.Count == 0)
			{
				Rectangle rect = this.panel.Client.Bounds;
				if (rect.Contains(pos))
				{
					rect.Deflate(this.GetDetectPadding(this.panel));
					if (!rect.Contains(pos))
					{
						detected = this.panel;
					}
				}
			}
			return detected;
		}

		protected Widget Detect(Point pos, Widget parent)
		{
			Widget[] children = parent.Children.Widgets;
			for (int i=children.Length-1; i>=0; i--)
			{
				Widget widget = children[i];

				if (widget.Index == -1)  // objet sans UniqueId ?
				{
					continue;
				}

				Rectangle rect = widget.ActualBounds;
				if (rect.Contains(pos))
				{
					Widget deep = this.Detect(widget.MapParentToClient(pos), widget);
					if (deep != null)
					{
						return deep;
					}

					if (this.selectedObjects.Count > 0)
					{
						if (widget.Parent != this.selectedObjects[0].Parent)
						{
							continue;
						}
					}

					return widget;
				}
			}

			return null;
		}

		protected Margins GetDetectPadding(Widget obj)
		{
			//	Retourne les marges intérieures pour la détection du padding.
			Margins padding = obj.Padding;
			padding += obj.GetInternalPadding();

			padding.Left   = System.Math.Max(padding.Left,   this.context.GroupOutline);
			padding.Right  = System.Math.Max(padding.Right,  this.context.GroupOutline);
			padding.Bottom = System.Math.Max(padding.Bottom, this.context.GroupOutline);
			padding.Top    = System.Math.Max(padding.Top,    this.context.GroupOutline);

			return padding;
		}

		public void PrepareForDelete()
		{
			//	Préparation en vue de la suppression de l'interface.
			this.DeselectAll();
		}

		public void DeselectAll()
		{
			//	Désélectionne tous les objets.
			if (this.selectedObjects.Count > 0)
			{
				this.selectedObjects.Clear();
				this.UpdateAfterChanging(Viewers.Changing.Selection);
				this.OnChildrenSelected();
				this.Invalidate();
			}
		}

		protected void SelectAll()
		{
			//	Sélectionne tous les objets.
			this.selectedObjects.Clear();

			foreach (Widget obj in this.panel.Children)
			{
				this.selectedObjects.Add(obj);
			}

			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectInvert()
		{
			//	Inverse la sélection.
			List<Widget> list = new List<Widget>();

			foreach (Widget obj in this.panel.Children)
			{
				if (!this.selectedObjects.Contains(obj))
				{
					list.Add(obj);
				}
			}

			this.selectedObjects = list;

			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectRoot()
		{
			//	Sélectionne le panneau de base.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(this.panel);

			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectParent()
		{
			//	Sélectionne l'objet parent de l'actuelle sélection.
			System.Diagnostics.Debug.Assert(this.selectedObjects.Count != 0);
			Widget parent = this.selectedObjects[0].Parent;
			this.selectedObjects.Clear();
			this.selectedObjects.Add(parent);

			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectOneObject(Widget obj)
		{
			//	Sélectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectListObject(List<Widget> list)
		{
			//	Sélectionne une liste d'objets.
			this.selectedObjects = list;
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	Sélectionne tous les objets entièrement inclus dans un rectangle.
			//	Tous les objets sélectionnés doivent avoir le même parent.
			this.SelectObjectsInRectangle(sel, this.panel);
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel, Widget parent)
		{
		}

		public void RegenerateForm()
		{
			//	Régénère le masque de saisie s'il y a eu un changement.
			this.UpdateAfterChanging(Viewers.Changing.Regenerate);
		}

		public void UpdateAfterSelectionGridChanged()
		{
			//	Mise à jour après un changement de sélection dans un tableau.
			this.OnChildrenSelected();  // met à jour les panneaux des proxies à droite
		}

		protected void UpdateAfterChanging(Viewers.Changing oper)
		{
			//	Mise à jour après un changement de sélection, ou après un changement dans
			//	l'arbre des objets (création, changement de parenté, etc.).
			this.module.DesignerApplication.UpdateViewer(oper);
		}

		protected void SetHilitedObject(Widget obj)
		{
			//	Détermine l'objet à mettre en évidence lors d'un survol.
			if (this.hilitedObject != obj)
			{
				this.hilitedObject = obj;
				this.Invalidate();
			}
		}
		#endregion


		#region Operations
		protected void DeleteSelection()
		{
			//	Supprime tous les objets sélectionnés.
			foreach (Widget obj in this.selectedObjects)
			{
				int index = this.objectModifier.GetFormDescriptionIndex(obj);
				if (index != -1)
				{
					this.form.Fields.RemoveAt(index);
				}
			}

			this.SetDirty();
			this.selectedObjects.Clear();
			this.UpdateAfterChanging(Viewers.Changing.Delete);
			this.OnChildrenSelected();
			this.Invalidate();
			this.SetDirty();
		}

		protected void DuplicateSelection()
		{
			//	Duplique tous les objets sélectionnés.
		}

		protected Rectangle SelectBounds
		{
			//	Retourne le rectangle englobant tous les objets sélectionnés.
			get
			{
				Rectangle bounds = Rectangle.Empty;
				return bounds;
			}
		}
		#endregion


		#region Paint
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le panneau.
			if (!this.isEditEnabled)
			{
				return;
			}

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			//	Dessine les surfaces inutilisées.
			Rectangle box = this.Client.Bounds;
			Rectangle bounds = this.ConvPanelToEditor(this.RealBounds);

			if (bounds.Top < box.Top)  // bande supérieure ?
			{
				Rectangle part = new Rectangle(box.Left, bounds.Top, box.Width, box.Top-bounds.Top);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (box.Bottom < bounds.Bottom)  // bande inférieure ?
			{
				Rectangle part = new Rectangle(box.Left, box.Bottom, box.Width, bounds.Bottom-box.Bottom);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (bounds.Right < box.Right)  // bande droite ?
			{
				Rectangle part = new Rectangle(bounds.Right, bounds.Bottom, box.Right-bounds.Right, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (box.Left < bounds.Left)  // bande gauche ?
			{
				Rectangle part = new Rectangle(box.Left, bounds.Bottom, bounds.Left-box.Left, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			Transform it = graphics.Transform;
			graphics.TranslateTransform(Editor.margin, Editor.margin);

			bounds = this.RealBounds;

			//	Dessine la grille magnétique
			if (this.context.ShowGrid)
			{
				double step = this.context.GridStep;
				int hilite = 0;
				for (double x=step+0.5; x<bounds.Width; x+=step)
				{
					graphics.AddLine(x, bounds.Bottom, x, bounds.Top);
					graphics.RenderSolid(((++hilite)%10 == 0) ? PanelsContext.ColorGrid1 : PanelsContext.ColorGrid2);
				}
				hilite = 0;
				for (double y=step+0.5; y<bounds.Height; y+=step)
				{
					graphics.AddLine(bounds.Left, y, bounds.Right, y);
					graphics.RenderSolid(((++hilite)%10 == 0) ? PanelsContext.ColorGrid1 : PanelsContext.ColorGrid2);
				}
			}

			//	Dessine les objets sélectionnés.
			if (this.selectedObjects.Count > 0 && !this.isDragging)
			{
				this.DrawSelectedObjects(graphics);
			}

			//	Dessine l'objet survolé.
			if (this.hilitedObject != null)
			{
				this.DrawHilitedObject(graphics, this.hilitedObject);
			}

			//	Dessine le rectangle de sélection.
			if (!this.selectedRectangle.IsEmpty)
			{
				Rectangle sel = this.selectedRectangle;
				sel.Deflate(0.5);
				graphics.AddRectangle(sel);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			}

			graphics.Transform = it;
		}

		protected void DrawSelectedObjects(Graphics graphics)
		{
			foreach (Widget obj in this.selectedObjects)
			{
				this.DrawSelectedObject(graphics, obj);
				this.DrawPadding(graphics, obj, 1.0);
			}

			if (this.selectedObjects.Count > 0)
			{
				Widget obj = this.selectedObjects[0];
				if (obj != this.panel)
				{
					this.DrawPadding(graphics, obj.Parent, 0.4);
				}
			}
		}

		protected void DrawSelectedObject(Graphics graphics, Widget obj)
		{
			Rectangle bounds = this.objectModifier.GetActualBounds(obj);
			bounds.Deflate(0.5);

			graphics.LineWidth = 3;
			graphics.AddRectangle(bounds);
			graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			graphics.LineWidth = 1;
		}

		protected void DrawPadding(Graphics graphics, Widget obj, double factor)
		{
			//	Dessine les marges de padding d'un objet, sous forme de hachures.
		}

		protected void DrawHilitedObject(Graphics graphics, Widget obj)
		{
			//	Met en évidence l'objet survolé par la souris.
			Color color = PanelsContext.ColorHiliteSurface;

			//	Si le rectangle est trop petit (par exemple objet Separator), il est engraissé.
			Rectangle rect = this.objectModifier.GetActualBounds(obj);

			double ix = 0;
			if (rect.Width < this.context.MinimalSize)
			{
				ix = this.context.MinimalSize;
			}

			double iy = 0;
			if (rect.Height < this.context.MinimalSize)
			{
				iy = this.context.MinimalSize;
			}

			rect.Inflate(ix, iy);

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(color);
		}

		protected Rectangle GetDrawBox(Graphics graphics, Point p1, Point p2, double thickness)
		{
			//	Donne le rectangle d'une boîte horizontale ou verticale.
			if (p1.Y == p2.Y)  // boîte horizontale ?
			{
				p1.Y -= thickness+1;
				p2.Y += thickness-1;
				p2.X -= 1;
				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);
				return new Rectangle(p1, p2);
			}
			else if (p1.X == p2.X)  // boîte verticale ?
			{
				p1.X -= thickness+1;
				p2.X += thickness-1;
				p2.Y -= 1;
				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);
				return new Rectangle(p1, p2);
			}
			else
			{
				throw new System.Exception("This geometry is not implemented.");
			}
		}
		#endregion


		#region Misc
		public void AdaptAfterToolChanged()
		{
			//	Adaptation après un changement d'outil ou d'objet.
		}

		public Rectangle RealBounds
		{
			//	Retourne le rectangle englobant de tous les objets contenus dans le panneau.
			get
			{
				return new Rectangle(Point.Zero, this.panel.ActualSize);
			}
		}

		protected bool IsInside(Point pos)
		{
			//	Indique si une position est dans la fenêtre.
			return this.Client.Bounds.Contains(pos);
		}

		protected void SetDirty()
		{
			this.module.AccessForms.SetLocalDirty();
		}

		protected Point ConvPanelToEditor(Point pos)
		{
			pos.X += Editor.margin;
			pos.Y += Editor.margin;
			return pos;
		}

		protected Point ConvEditorToPanel(Point pos)
		{
			pos.X -= Editor.margin;
			pos.Y -= Editor.margin;
			return pos;
		}

		protected Rectangle ConvPanelToEditor(Rectangle rect)
		{
			rect.Offset(Editor.margin, Editor.margin);
			return rect;
		}

		protected Rectangle ConvEditorToPanel(Rectangle rect)
		{
			rect.Offset(-Editor.margin, -Editor.margin);
			return rect;
		}
		#endregion


		#region IPaintFilter Members
		bool IPaintFilter.IsWidgetFullyDiscarded(Widget widget)
		{
			//	Retourne true pour indiquer que le widget en question ne doit
			//	pas être peint, ni ses enfants d'ailleurs. Ceci évite que les
			//	widgets sélectionnés ne soient peints.
			return this.isDragging && this.selectedObjects.Contains(widget);
		}

		bool IPaintFilter.IsWidgetPaintDiscarded(Widget widget)
		{
			return false;
		}

		void IPaintFilter.NotifyAboutToProcessChildren()
		{
		}

		void IPaintFilter.NotifyChildrenProcessed()
		{
		}
		#endregion


		#region MouseCursor
		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			//	Change le sprite de la souris.
			if ( cursor == this.lastCursor )  return;
			this.lastCursor = cursor;

			switch (cursor)
			{
				case MouseCursorType.Arrow:
					this.SetMouseCursorImage(ref this.mouseCursorArrow, Misc.Icon("CursorArrow"));
					break;

				case MouseCursorType.ArrowPlus:
					this.SetMouseCursorImage(ref this.mouseCursorArrowPlus, Misc.Icon("CursorArrowPlus"));
					break;

				case MouseCursorType.Global:
					this.SetMouseCursorImage(ref this.mouseCursorGlobal, Misc.Icon("CursorGlobal"));
					break;

				case MouseCursorType.Grid:
					this.SetMouseCursorImage(ref this.mouseCursorGrid, Misc.Icon("CursorGrid"));
					break;

				case MouseCursorType.GridPlus:
					this.SetMouseCursorImage(ref this.mouseCursorGridPlus, Misc.Icon("CursorGridPlus"));
					break;

				case MouseCursorType.Edit:
					this.SetMouseCursorImage(ref this.mouseCursorEdit, Misc.Icon("CursorEdit"));
					break;

				case MouseCursorType.Hand:
					this.SetMouseCursorImage(ref this.mouseCursorHand, Misc.Icon("CursorHand"));
					break;

				case MouseCursorType.Finger:
					this.SetMouseCursorImage(ref this.mouseCursorFinger, Misc.Icon("CursorFinger"));
					break;

				case MouseCursorType.Pen:
					this.SetMouseCursorImage(ref this.mouseCursorPen, Misc.Icon("CursorPen"));
					break;

				case MouseCursorType.Zoom:
					this.SetMouseCursorImage(ref this.mouseCursorZoom, Misc.Icon("CursorZoom"));
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
				image = Support.ImageProvider.Default.GetImage(name, Support.Resources.DefaultManager);
			}

			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion


		#region Events
		protected virtual void OnChildrenAdded()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ChildrenAdded");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ChildrenAdded
		{
			add
			{
				this.AddUserEventHandler("ChildrenAdded", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ChildrenAdded", value);
			}
		}

		protected virtual void OnChildrenSelected()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ChildrenSelected");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ChildrenSelected
		{
			add
			{
				this.AddUserEventHandler("ChildrenSelected", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ChildrenSelected", value);
			}
		}

		protected virtual void OnChildrenGeometryChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ChildrenGeometryChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ChildrenGeometryChanged
		{
			add
			{
				this.AddUserEventHandler("ChildrenGeometryChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ChildrenGeometryChanged", value);
			}
		}

		protected virtual void OnUpdateCommands()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("UpdateCommands");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler UpdateCommands
		{
			add
			{
				this.AddUserEventHandler("UpdateCommands", value);
			}
			remove
			{
				this.RemoveUserEventHandler("UpdateCommands", value);
			}
		}
		#endregion


		public static readonly double		margin = 10;

		protected Module					module;
		protected FormDescription			form;
		protected UI.Panel					panel;
		protected Druid						druid;
		protected PanelsContext				context;
		protected ObjectModifier			objectModifier;
		protected bool						isEditEnabled = false;

		protected List<Widget>				selectedObjects = new List<Widget>();
		protected Rectangle					selectedRectangle = Rectangle.Empty;
		protected Widget					hilitedObject;
		protected bool						isDragging;
		protected MouseCursorType			lastCursor = MouseCursorType.Unknown;

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorGrid = null;
		protected Image						mouseCursorGridPlus = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
		protected Image						mouseCursorFinger = null;
	}
}
