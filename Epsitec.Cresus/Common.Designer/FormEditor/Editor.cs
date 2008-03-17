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
	/// Widget venant par-dessus le conteneur UI.Panel pour �diter ce dernier.
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


		public void Initialize(Viewers.Forms viewersForms, Module module, PanelsContext context, UI.Panel panel)
		{
			this.viewersForms = viewersForms;
			this.module = module;
			this.context = context;
			this.panel = panel;

			this.objectModifier = new ObjectModifier(this);
		}

		public Viewers.Forms ViewersForms
		{
			get
			{
				return this.viewersForms;
			}
		}

		public Module Module
		{
			//	Module associ�.
			get
			{
				return this.module;
			}
		}

		public FormDescription WorkingForm
		{
			//	Masque de saisie associ�.
			get
			{
				return this.workingForm;
			}
			set
			{
				this.workingForm = value;
			}
		}

		public List<FieldDescription> BaseFields
		{
			get
			{
				return this.baseFields;
			}
			set
			{
				this.baseFields = value;
			}
		}

		public List<FieldDescription> FinalFields
		{
			get
			{
				return this.finalFields;
			}
			set
			{
				this.finalFields = value;
			}
		}

		public PanelsContext Context
		{
			//	Contexte asoci�.
			get
			{
				return this.context;
			}
		}

		public UI.Panel Panel
		{
			//	Panneau associ� qui est le conteneur de tous les widgets.
			//	Editor est fr�re de Panel et vient par-dessus.
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
			//	Retourne la liste des objets s�lectionn�s.
			get
			{
				return this.selectedObjects;
			}
		}


		public List<System.Guid> GetSelectedGuids()
		{
			//	Retourne la liste des Guids de tous les champs s�lectionn�s.
			List<System.Guid> guids = new List<System.Guid>();

			foreach (Widget obj in this.selectedObjects)
			{
				FieldDescription field = this.objectModifier.GetFieldDescription(obj);
				if (field != null)
				{
					guids.Add(field.Guid);
				}
			}

			return guids;
		}

		public void SetSelectedGuids(List<System.Guid> guids)
		{
			//	S�lectionne tous les objets dont on donne les Guids.
			this.selectedObjects.Clear();

			foreach (System.Guid guid in guids)
			{
				Widget obj = this.objectModifier.GetWidget(guid);
				if (obj != null)
				{
					this.selectedObjects.Add(obj);
				}
			}

			this.Invalidate();
		}


		public bool IsEditEnabled
		{
			//	Est-ce que l'�dition est possible ? Pour cela, il faut avoir s�lectionn� un bundle
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
			//	Mise � jour apr�s avoir chang� la g�om�trie d'un ou plusieurs objets.
		}


		public void DoCommand(string name)
		{
			//	Ex�cute une commande.
			switch (name)
			{
				case "PanelDelete":
					//?this.DeleteSelection();
					break;

				case "PanelDuplicate":
					//?this.DuplicateSelection();
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

				case "PanelShowTabIndex":
					this.context.ShowTabIndex = !this.context.ShowTabIndex;
					this.Invalidate();
					this.OnUpdateCommands();
					break;
			}
		}

		public void GetSelectionInfo(out int selected, out int count, out bool isRoot)
		{
			//	Donne des informations sur la s�lection en cours.
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
				message.Captured = true;
				message.Consumer = this;
				return;
			}

			pos = this.ConvEditorToPanel(pos);

			switch (message.MessageType)
			{
				case MessageType.MouseDown:
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
					this.SetHilitedForwardTab(null);
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
			//	La souris a �t� press�e.
			this.SelectDown(pos, isRightButton, isControlPressed, isShiftPressed);
		}

		void ProcessMouseMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� d�plac�e.
			this.SelectMove(pos, isRightButton, isControlPressed, isShiftPressed);
		}

		void ProcessMouseUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� rel�ch�e.
			this.SelectUp(pos, isRightButton, isControlPressed, isShiftPressed);
		}

		void ProcessKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� rel�ch�e.
			this.SelectKeyChanged(isControlPressed, isShiftPressed);
		}
		#endregion

		#region ProcessMouse select
		protected void SelectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris press�e.
			Widget obj;
			
			obj = this.DetectForwardTab(pos);
			if (obj != null)
			{
				this.draggingForwardTab = obj;
				this.posForwardTab = pos;
				this.Invalidate();
				return;
			}

			obj = this.Detect(pos);
			bool selectionChanged = false;

			if (!isControlPressed && this.selectedObjects.Count > 0)  // touche Ctrl rel�ch�e ?
			{
				this.selectedObjects.Clear();
				selectionChanged = true;
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
				selectionChanged = true;
			}

			if (selectionChanged)
			{
				this.UpdateAfterChanging(Viewers.Changing.Selection);
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris d�plac�e.
			Widget obj;

			if (this.draggingForwardTab != null)
			{
				obj = this.Detect(pos);
				if (obj != null)
				{
					FieldDescription dstField = this.objectModifier.GetFieldDescription(obj);
					if (dstField == null || !dstField.IsForwardTab)
					{
						obj = null;
					}
				}
				this.SetHilitedObject(obj);  // met en �vidence l'objet survol� par la souris
				this.posForwardTab = pos;
				this.Invalidate();
				return;
			}

			obj = this.DetectForwardTab(pos);
			this.SetHilitedForwardTab(obj);  // met en �vidence l'objet survol� par la souris

			obj = this.Detect(pos);
			this.SetHilitedObject(obj);  // met en �vidence l'objet survol� par la souris
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris rel�ch�e.
			if (this.draggingForwardTab != null)
			{
				FieldDescription field = this.objectModifier.GetFieldDescription(this.draggingForwardTab);
				System.Guid guid = System.Guid.Empty;

				Widget obj = this.Detect(pos);
				if (obj != null && obj != this.draggingForwardTab)
				{
					FieldDescription dstField = this.objectModifier.GetFieldDescription(obj);
					if (dstField != null && dstField.IsForwardTab)
					{
						guid = dstField.Guid;
					}
				}

				//?this.objectModifier.ChangeForwardTab(field.Guid, guid);
				this.viewersForms.ChangeForwardTab(field.Guid, guid);

				this.draggingForwardTab = null;
				this.Invalidate();
			}
		}

		protected void SelectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, touche press�e ou rel�ch�e.
			if (isControlPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Arrow);
			}
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
			//	D�but du drag pour d�placer les objets s�lectionn�s.
		}

		protected void DraggingMove(Point pos)
		{
			//	Mouvement du drag pour d�placer les objets s�lectionn�s.
		}

		protected void DraggingEnd(Point pos)
		{
			//	Fin du drag pour d�placer les objets s�lectionn�s.
		}
		#endregion


		#region Selection
		protected Widget DetectForwardTab(Point pos)
		{
			//	D�tecte la poign�e (� l'extr�mit� de la fl�che 'ForwardTab' de l'objet s�lectionn�) vis�e par la souris.
			foreach (Widget obj in this.selectedObjects)
			{
				Rectangle rect = this.GetForwardTabHandle(obj);
				if (!rect.IsEmpty && rect.Contains(pos))
				{
					return obj;
				}
			}

			return null;
		}

		protected Widget Detect(Point pos)
		{
			//	D�tecte l'objet vis� par la souris, avec priorit� au dernier objet dessin� (donc plac� dessus).
			return this.Detect(pos, this.panel);
		}

		protected Widget Detect(Point pos, Widget parent)
		{
			Widget[] children = parent.Children.Widgets;
			for (int i=children.Length-1; i>=0; i--)
			{
				Widget widget = children[i];

				if (!Misc.IsValidGuid(widget.Name))  // objet sans Guid valide ?
				{
					continue;
				}

				Rectangle rect = widget.ActualBounds;
				rect = this.objectModifier.AdjustBounds(widget, rect);
				rect = this.objectModifier.InflateMinimalSize(rect);
				if (rect.Contains(pos))
				{
					Widget deep = this.Detect(widget.MapParentToClient(pos), widget);
					if (deep != null)
					{
						return deep;
					}

					return widget;
				}
			}

			return null;
		}


		public void PrepareForDelete()
		{
			//	Pr�paration en vue de la suppression de l'interface.
			this.DeselectAll();
		}

		public void DeselectAll()
		{
			//	D�s�lectionne tous les objets.
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
			//	S�lectionne tous les objets.
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
			//	Inverse la s�lection.
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
			//	S�lectionne le panneau de base.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(this.panel);

			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectParent()
		{
			//	S�lectionne l'objet parent de l'actuelle s�lection.
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
			//	S�lectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectListObject(List<System.Guid> guids)
		{
			//	S�lectionne une liste d'objets d'apr�s une liste de Guids.
			this.selectedObjects.Clear();

			foreach (System.Guid guid in guids)
			{
				Widget obj = this.objectModifier.GetWidget(guid);
				if (obj != null)
				{
					this.selectedObjects.Add(obj);
				}
			}

			//?this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	S�lectionne tous les objets enti�rement inclus dans un rectangle.
			//	Tous les objets s�lectionn�s doivent avoir le m�me parent.
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
			//	R�g�n�re le masque de saisie s'il y a eu un changement.
			this.UpdateAfterChanging(Viewers.Changing.Regenerate);
		}

		public void UpdateAfterSelectionGridChanged()
		{
			//	Mise � jour apr�s un changement de s�lection dans un tableau.
			this.OnChildrenSelected();  // met � jour les panneaux des proxies � droite
		}

		protected void UpdateAfterChanging(Viewers.Changing oper)
		{
			//	Mise � jour apr�s un changement de s�lection, ou apr�s un changement dans
			//	l'arbre des objets (cr�ation, changement de parent�, etc.).
			this.module.DesignerApplication.UpdateViewer(oper);
		}

		protected void SetHilitedObject(Widget obj)
		{
			//	D�termine l'objet � mettre en �vidence lors d'un survol.
			if (this.hilitedObject != obj)
			{
				this.hilitedObject = obj;
				this.Invalidate();
			}
		}

		protected void SetHilitedForwardTab(Widget obj)
		{
			//	D�termine l'objet � mettre en �vidence lors d'un survol.
			if (this.hilitedForwardTab != obj)
			{
				this.hilitedForwardTab = obj;
				this.Invalidate();
			}
		}
		#endregion


		#region Paint
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le panneau.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			//	Dessine les surfaces inutilis�es.
			Rectangle box = this.Client.Bounds;
			Rectangle bounds = this.ConvPanelToEditor(this.RealBounds);

			if (this.isEditEnabled)
			{
				if (bounds.Top < box.Top)  // bande sup�rieure ?
				{
					Rectangle part = new Rectangle(box.Left, bounds.Top, box.Width, box.Top-bounds.Top);
					graphics.AddFilledRectangle(part);
					graphics.RenderSolid(PanelsContext.ColorOutsurface);
				}

				if (box.Bottom < bounds.Bottom)  // bande inf�rieure ?
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
			}

			Transform it = graphics.Transform;
			Point offset = this.ConvOffset;
			graphics.TranslateTransform(offset.X, offset.Y);

			bounds = this.RealBounds;

			//	Dessine la grille magn�tique
			if (this.isEditEnabled && this.context.ShowGrid)
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

			//	Dessine les num�ros d'index pour la touche Tab.
			if (this.context.ShowTabIndex)
			{
				this.DrawTabIndex(graphics, this.panel);
			}

			if (this.isEditEnabled)
			{
				//	Dessine les objets s�lectionn�s.
				if (this.selectedObjects.Count > 0 && !this.isDragging)
				{
					this.DrawSelectedObjects(graphics);
				}

				//	Dessine l'objet survol�.
				if (this.hilitedObject != null)
				{
					this.DrawHilitedObject(graphics, this.hilitedObject);

					if (this.draggingForwardTab != null)
					{
						Point src = this.objectModifier.GetActualBounds(this.draggingForwardTab).Center;
						this.DrawForwardTabArrow(graphics, false, false, src, this.posForwardTab);
					}
				}

				//	Dessine le rectangle de s�lection.
				if (!this.selectedRectangle.IsEmpty)
				{
					Rectangle sel = this.selectedRectangle;
					sel.Deflate(0.5);
					graphics.AddRectangle(sel);
					graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
				}
			}

			graphics.Transform = it;
		}

		protected void DrawTabIndex(Graphics graphics, Widget parent)
		{
			//	Dessine les num�ros d'index pour la touche Tab et les fl�ches 'ForwardTab'.
			foreach (Widget obj in parent.Children)
			{
				FieldDescription field = this.objectModifier.GetFieldDescription(obj);
				if (field == null)
				{
					continue;
				}

				if (field.IsForwardTab)
				{
					Rectangle rect = this.objectModifier.GetActualBounds(obj);
					Rectangle box = new Rectangle(rect.BottomRight+new Point(-20-1, 1), new Size(20, 10));

					string text = this.GetObjectTabIndex(obj);
					if (!string.IsNullOrEmpty(text))
					{
						graphics.AddFilledRectangle(box);
						graphics.RenderSolid(Color.FromBrightness(1));

						graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
						graphics.RenderSolid(PanelsContext.ColorTabIndex);
					}

					if (field.ForwardTabGuid != System.Guid.Empty && this.draggingForwardTab == null)  // fl�che 'ForwardTab' ?
					{
						Point src, dst;
						if (this.GetForwardTabArrow(obj, false, out src, out dst))
						{
							this.DrawForwardTabArrow(graphics, false, false, src, dst);
						}
					}
				}

				if (field.Type == FieldDescription.FieldType.BoxBegin || field.Type == FieldDescription.FieldType.SubForm)
				{
					this.DrawTabIndex(graphics, obj);
				}
			}
		}

		protected string GetObjectTabIndex(Widget obj)
		{
			//	Retourne la cha�ne indiquant l'ordre pour la touche Tab.
			if (Editor.IsObjectTabActive(obj))
			{
				return obj.TabIndex.ToString();
			}
			else
			{
				return null;
			}
		}

		protected static bool IsObjectTabActive(Widget obj)
		{
			//	Indique si l'objet � un ordre pour la touche Tab.
			return (obj.TabNavigationMode & TabNavigationMode.ActivateOnTab) != 0;
		}

		protected void DrawSelectedObjects(Graphics graphics)
		{
			//	Dessine tous les objets s�lectionn�s.
			foreach (Widget obj in this.selectedObjects)
			{
				this.DrawSelectedObject(graphics, obj);
			}
		}

		protected void DrawSelectedObject(Graphics graphics, Widget obj)
		{
			//	Dessine un objet s�lectionn�.
			Rectangle bounds = this.objectModifier.GetActualBounds(obj);
			bounds.Deflate(0.5);

			graphics.LineWidth = 3;
			graphics.AddRectangle(bounds);
			graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			graphics.LineWidth = 1;

			if (this.draggingForwardTab == null)
			{
				Point src, dst;
				if (this.GetForwardTabArrow(obj, true, out src, out dst))
				{
					bool hilited = (obj == this.hilitedForwardTab);
					this.DrawForwardTabArrow(graphics, true, hilited, src, dst);
				}
			}
		}

		protected void DrawForwardTabArrow(Graphics graphics, bool selected, bool hilited, Point start, Point end)
		{
			//	Dessine une fl�che de 'start' � l'extr�mit� 'end'.
			//	Si selected = true, on dessine une poign�e � l'extr�mit� 'end'.
			graphics.Align(ref start);
			graphics.Align(ref end);
			start.X += 0.5;
			start.Y += 0.5;
			end.X += 0.5;
			end.Y += 0.5;

			if (start != end)
			{
				Point p = Point.Move(end, start, Editor.forwardTabArrowLength);

				Point e1 = Transform.RotatePointDeg(end,  Editor.forwardTabArrowAngle, p);
				Point e2 = Transform.RotatePointDeg(end, -Editor.forwardTabArrowAngle, p);

				graphics.AddFilledCircle(start, Editor.forwardTabStartRadius);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);

				graphics.AddLine(start, end);
				graphics.AddLine(end, e1);
				graphics.AddLine(end, e2);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			}

			if (selected)  // dessine une poign�e � l'extr�mit� 'end' ?
			{
				Rectangle rect = new Rectangle(end, end);
				rect.Inflate(Editor.forwardTabHalfHandle);
				graphics.Align(ref rect);
				rect.Offset(-0.5, -0.5);

				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(hilited ? PanelsContext.ColorHandleHilited : PanelsContext.ColorHandleNormal);

				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0));
			}
		}

		protected void DrawHilitedObject(Graphics graphics, Widget obj)
		{
			//	Met en �vidence l'objet survol� par la souris.
			Color color = PanelsContext.ColorHiliteSurface;

			//	Si le rectangle est trop petit (par exemple objet Separator), il est engraiss�.
			Rectangle rect = this.objectModifier.GetActualBounds(obj);
			rect = this.objectModifier.InflateMinimalSize(rect);

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(color);
		}

		protected Rectangle GetForwardTabHandle(Widget obj)
		{
			//	Retourne le rectangle de la poign�e 'ForwardTab'.
			Point src, dst;
			if (this.GetForwardTabArrow(obj, true, out src, out dst))
			{
				Rectangle rect = new Rectangle(dst, dst);
				rect.Inflate(Editor.forwardTabHalfHandle+1);
				return rect;
			}

			return Rectangle.Empty;
		}

		protected bool GetForwardTabArrow(Widget obj, bool selected, out Point src, out Point dst)
		{
			//	Retourne les positions pour la fl�che 'ForwardTab'.
			//	Si l'objet est s�lectionn� (selected = true), on retourne une fl�che sur soi-m�me
			//	s'il n'existe aucun objet destination (ForwardTabGuid ind�fini).
			src = Point.Zero;
			dst = Point.Zero;

			int index = this.objectModifier.GetFieldDescriptionIndex(obj);
			if (index == -1)
			{
				return false;
			}

			FieldDescription srcField = this.objectModifier.GetFieldDescription(index);
			if (srcField == null || !srcField.IsForwardTab)
			{
				return false;
			}

			double offset = (index%10)*Editor.forwardTabSpaceX;  // offset horizontal, pour �viter les superpositions

			if (srcField.ForwardTabGuid == System.Guid.Empty)
			{
				if (selected)
				{
					src = this.objectModifier.GetActualBounds(obj).Center;
					src.X += offset;
					dst = src;  // fl�che sur soi-m�me
					return true;
				}
				else
				{
					return false;
				}
			}

			FieldDescription dstField = this.objectModifier.GetFieldDescription(srcField.ForwardTabGuid);
			if (dstField == null)
			{
				return false;
			}

			Widget dstObj = this.objectModifier.GetWidget(dstField.Guid);
			if (dstObj == null)
			{
				return false;
			}

			src = this.objectModifier.GetActualBounds(obj).Center;
			dst = this.objectModifier.GetActualBounds(dstObj).Center;
			src.X += offset;
			dst.X += offset;

			return true;
		}

		protected Rectangle GetDrawBox(Graphics graphics, Point p1, Point p2, double thickness)
		{
			//	Donne le rectangle d'une bo�te horizontale ou verticale.
			if (p1.Y == p2.Y)  // bo�te horizontale ?
			{
				p1.Y -= thickness+1;
				p2.Y += thickness-1;
				p2.X -= 1;
				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);
				return new Rectangle(p1, p2);
			}
			else if (p1.X == p2.X)  // bo�te verticale ?
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
			//	Adaptation apr�s un changement d'outil ou d'objet.
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
			//	Indique si une position est dans la fen�tre.
			return this.Client.Bounds.Contains(pos);
		}

		protected void SetDirty()
		{
			this.module.AccessForms.SetLocalDirty();
		}

		protected Rectangle ConvPanelToEditor(Rectangle rect)
		{
			return new Rectangle(this.ConvPanelToEditor(rect.BottomLeft), rect.Size);
		}

		protected Rectangle ConvEditorToPanel(Rectangle rect)
		{
			return new Rectangle(this.ConvEditorToPanel(rect.BottomLeft), rect.Size);
		}

		protected Point ConvPanelToEditor(Point pos)
		{
			return pos+this.ConvOffset;
		}

		protected Point ConvEditorToPanel(Point pos)
		{
			return pos-this.ConvOffset;
		}

		protected Point ConvOffset
		{
			//	Retourne l'offset � ajouter/soustraire lors d'une conversion Editor <-> Panel.
			//	On suppose que le Form sous-jascent est align� en mode TopLeft (voir Viewers.Form.UpdateMiscPagePanel,
			//	this.panelContainer.HorizontalAlignment et this.panelContainer.VerticalAlignment).
			get
			{
				return new Point(Editor.margin, Editor.margin+this.ActualHeight-(Editor.margin*2+this.panel.ActualHeight));
			}
		}
		#endregion


		#region IPaintFilter Members
		bool IPaintFilter.IsWidgetFullyDiscarded(Widget widget)
		{
			//	Retourne true pour indiquer que le widget en question ne doit
			//	pas �tre peint, ni ses enfants d'ailleurs. Ceci �vite que les
			//	widgets s�lectionn�s ne soient peints.
			return this.isDragging && this.selectedObjects.Contains(widget);
		}

		bool IPaintFilter.IsWidgetPaintDiscarded(Widget widget)
		{
			return false;
		}

		void IPaintFilter.NotifyAboutToProcessChildren(Widget sender, PaintEventArgs e)
		{
		}

		void IPaintFilter.NotifyChildrenProcessed(Widget sender, PaintEventArgs e)
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

		public virtual void OnUpdateCommands()
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
		protected static readonly double	forwardTabArrowLength = 8;
		protected static readonly double	forwardTabArrowAngle = 25;
		protected static readonly double	forwardTabStartRadius = 3;
		protected static readonly double	forwardTabHalfHandle = 3.5;
		protected static readonly double	forwardTabSpaceX = 6;

		protected Viewers.Forms				viewersForms;
		protected Module					module;
		protected FormDescription			workingForm;
		protected List<FieldDescription>	baseFields;
		protected List<FieldDescription>	finalFields;
		protected UI.Panel					panel;
		protected Druid						druid;
		protected PanelsContext				context;
		protected ObjectModifier			objectModifier;
		protected bool						isEditEnabled = false;

		protected List<Widget>				selectedObjects = new List<Widget>();
		protected Rectangle					selectedRectangle = Rectangle.Empty;
		protected Widget					hilitedObject;
		protected Widget					hilitedForwardTab;
		protected Widget					draggingForwardTab;
		protected Point						posForwardTab;
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
