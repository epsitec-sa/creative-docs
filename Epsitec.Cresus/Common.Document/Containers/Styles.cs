using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Styles contient tous les panneaux des styles.
	/// </summary>
	[SuppressBundleSupport]
	public class Styles : Abstract
	{
		public Styles(Document document) : base(document)
		{
			//	Toolbar principale.
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = 1;
			this.toolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			int index = 0;

			this.buttonAggregateNewEmpty = new IconButton(Misc.Icon("AggregateNewEmpty"));
			this.buttonAggregateNewEmpty.Clicked += new MessageEventHandler(this.HandleButtonAggregateNewEmpty);
			this.buttonAggregateNewEmpty.TabIndex = index++;
			this.buttonAggregateNewEmpty.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAggregateNewEmpty);
			ToolTip.Default.SetToolTip(this.buttonAggregateNewEmpty, Res.Strings.Action.AggregateNewEmpty);

			this.buttonAggregateNew3 = new IconButton(Misc.Icon("AggregateNew3"));
			this.buttonAggregateNew3.Clicked += new MessageEventHandler(this.HandleButtonAggregateNew3);
			this.buttonAggregateNew3.TabIndex = index++;
			this.buttonAggregateNew3.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAggregateNew3);
			ToolTip.Default.SetToolTip(this.buttonAggregateNew3, Res.Strings.Action.AggregateNew3);

			this.buttonAggregateNewAll = new IconButton(Misc.Icon("AggregateNewAll"));
			this.buttonAggregateNewAll.Clicked += new MessageEventHandler(this.HandleButtonAggregateNewAll);
			this.buttonAggregateNewAll.TabIndex = index++;
			this.buttonAggregateNewAll.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAggregateNewAll);
			ToolTip.Default.SetToolTip(this.buttonAggregateNewAll, Res.Strings.Action.AggregateNewAll);

			this.buttonAggregateDuplicate = new IconButton(Misc.Icon("AggregateDuplicate"));
			this.buttonAggregateDuplicate.Clicked += new MessageEventHandler(this.HandleButtonAggregateDuplicate);
			this.buttonAggregateDuplicate.TabIndex = index++;
			this.buttonAggregateDuplicate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAggregateDuplicate);
			ToolTip.Default.SetToolTip(this.buttonAggregateDuplicate, Res.Strings.Action.AggregateDuplicate);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonAggregateUp = new IconButton(Misc.Icon("AggregateUp"));
			this.buttonAggregateUp.Clicked += new MessageEventHandler(this.HandleButtonAggregateUp);
			this.buttonAggregateUp.TabIndex = index++;
			this.buttonAggregateUp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAggregateUp);
			ToolTip.Default.SetToolTip(this.buttonAggregateUp, Res.Strings.Action.AggregateUp);

			this.buttonAggregateDown = new IconButton(Misc.Icon("AggregateDown"));
			this.buttonAggregateDown.Clicked += new MessageEventHandler(this.HandleButtonAggregateDown);
			this.buttonAggregateDown.TabIndex = index++;
			this.buttonAggregateDown.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAggregateDown);
			ToolTip.Default.SetToolTip(this.buttonAggregateDown, Res.Strings.Action.AggregateDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonAggregateDelete = new IconButton(Misc.Icon("AggregateDelete"));
			this.buttonAggregateDelete.Clicked += new MessageEventHandler(this.HandleButtonAggregateDelete);
			this.buttonAggregateDelete.TabIndex = index++;
			this.buttonAggregateDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonAggregateDelete);
			ToolTip.Default.SetToolTip(this.buttonAggregateDelete, Res.Strings.Action.AggregateDelete);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonStyleNew = new IconButton(Misc.Icon("AggregateStyleNew"));
			this.buttonStyleNew.Clicked += new MessageEventHandler(this.HandleButtonStyleNew);
			this.buttonStyleNew.TabIndex = index++;
			this.buttonStyleNew.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonStyleNew);
			ToolTip.Default.SetToolTip(this.buttonStyleNew, Res.Strings.Action.AggregateStyleNew);

			this.buttonStyleDelete = new IconButton(Misc.Icon("AggregateStyleDelete"));
			this.buttonStyleDelete.Clicked += new MessageEventHandler(this.HandleButtonStyleDelete);
			this.buttonStyleDelete.TabIndex = index++;
			this.buttonStyleDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonStyleDelete);
			ToolTip.Default.SetToolTip(this.buttonStyleDelete, Res.Strings.Action.AggregateStyleDelete);

			//	Table des agrégats.
			this.list = new Widgets.AggregateList();
			this.list.Document = this.document;
			this.list.List = this.document.Aggregates;
			this.list.HScroller = true;
			this.list.VScroller = true;
			this.list.SetParent(this);
			this.list.MinSize = new Size(10, 87);
			this.list.Dock = DockStyle.Fill;
			this.list.DockMargins = new Margins(0, 0, 0, 0);
			this.list.FinalSelectionChanged += new EventHandler(this.HandleAggregatesTableSelectionChanged);
			this.list.FlyOverChanged += new EventHandler(this.HandleAggregatesTableFlyOverChanged);
			this.list.DoubleClicked += new MessageEventHandler(this.HandleAggregatesTableDoubleClicked);
			this.list.TabIndex = 2;
			this.list.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Roue des couleurs.
			this.colorSelector = new ColorSelector();
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.HasCloseButton = true;
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += new EventHandler(this.HandleColorSelectorClosed);
			this.colorSelector.SetParent(this);
			this.colorSelector.Dock = DockStyle.Bottom;
			this.colorSelector.DockMargins = new Margins(0, 0, 5, 0);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.colorSelector.Visibility = false;

			//	Conteneur du panneau.
			this.panelContainer = new Widget(this);
			this.panelContainer.Height = 0.0;
			this.panelContainer.Dock = DockStyle.Bottom;
			this.panelContainer.DockMargins = new Margins(0, 0, 5, 0);
			this.panelContainer.TabIndex = 99;
			this.panelContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			//	Enfants de l'agrégat.
			this.childrens = new Widgets.AggregateList();
			this.childrens.Document = this.document;
			this.childrens.HScroller = true;
			this.childrens.VScroller = true;
			this.childrens.IsHiliteColumn = false;
			this.childrens.IsOrderColumn = true;
			this.childrens.IsChildrensColumn = false;
			this.childrens.SetParent(this);
			this.childrens.Height = 87;
			this.childrens.Dock = DockStyle.Bottom;
			this.childrens.DockMargins = new Margins(0, 0, 0, 0);
			this.childrens.FinalSelectionChanged += new EventHandler(this.HandleAggregatesChildrensSelectionChanged);
			this.childrens.TabIndex = 98;
			this.childrens.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.toolBarChildrens = new HToolBar(this);
			this.toolBarChildrens.Dock = DockStyle.Bottom;
			this.toolBarChildrens.DockMargins = new Margins(0, 0, 0, 0);
			this.toolBarChildrens.TabIndex = 97;
			this.toolBarChildrens.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.AggregateChildrens.Label.Name;
			this.toolBarChildrens.Items.Add(st);

			index = 0;

			this.buttonChildrensNew = new IconButton(Misc.Icon("AggregateChildrensNew"));
			this.buttonChildrensNew.Clicked += new MessageEventHandler(this.HandleButtonChildrensNew);
			this.buttonChildrensNew.TabIndex = index++;
			this.buttonChildrensNew.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBarChildrens.Items.Add(this.buttonChildrensNew);
			ToolTip.Default.SetToolTip(this.buttonChildrensNew, Res.Strings.Action.AggregateChildrensNew);

			this.toolBarChildrens.Items.Add(new IconSeparator());

			this.buttonChildrensUp = new IconButton(Misc.Icon("Up"));
			this.buttonChildrensUp.Clicked += new MessageEventHandler(this.HandleButtonChildrensUp);
			this.buttonChildrensUp.TabIndex = index++;
			this.buttonChildrensUp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBarChildrens.Items.Add(this.buttonChildrensUp);
			ToolTip.Default.SetToolTip(this.buttonChildrensUp, Res.Strings.Action.AggregateChildrensUp);

			this.buttonChildrensDown = new IconButton(Misc.Icon("Down"));
			this.buttonChildrensDown.Clicked += new MessageEventHandler(this.HandleButtonChildrensDown);
			this.buttonChildrensDown.TabIndex = index++;
			this.buttonChildrensDown.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBarChildrens.Items.Add(this.buttonChildrensDown);
			ToolTip.Default.SetToolTip(this.buttonChildrensDown, Res.Strings.Action.AggregateChildrensDown);

			this.toolBarChildrens.Items.Add(new IconSeparator());

			this.buttonChildrensDelete = new IconButton(Misc.Icon("DeleteItem"));
			this.buttonChildrensDelete.Clicked += new MessageEventHandler(this.HandleButtonChildrensDelete);
			this.buttonChildrensDelete.TabIndex = index++;
			this.buttonChildrensDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBarChildrens.Items.Add(this.buttonChildrensDelete);
			ToolTip.Default.SetToolTip(this.buttonChildrensDelete, Res.Strings.Action.AggregateChildrensDelete);

			//	Nom de l'agrégat.
			this.toolBarName = new HToolBar(this);
			this.toolBarName.Dock = DockStyle.Bottom;
			this.toolBarName.DockMargins = new Margins(0, 0, 0, 0);
			this.toolBarName.TabIndex = 96;
			this.toolBarName.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.AggregateName.Label.Name;
			this.toolBarName.Items.Add(st);

			this.name = new TextField();
			this.name.Width = 135;
			this.name.DockMargins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += new EventHandler(this.HandleNameTextChanged);
			this.name.TabIndex = 1;
			this.name.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBarName.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.AggregateName.Tooltip.Name);

			this.buttonChildrensExtend = new GlyphButton(this.toolBarName);
			this.buttonChildrensExtend.ButtonStyle = ButtonStyle.Icon;
			this.buttonChildrensExtend.GlyphShape = GlyphShape.ArrowUp;
			this.buttonChildrensExtend.Width = 12;
			this.buttonChildrensExtend.Dock = DockStyle.Right;
			this.buttonChildrensExtend.DockMargins = new Margins(0, 0, 5, 5);
			this.buttonChildrensExtend.Clicked += new MessageEventHandler(this.HandleButtonChildrensExtend);
			this.buttonChildrensExtend.TabIndex = 2;
			this.buttonChildrensExtend.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonChildrensExtend, Res.Strings.Panel.Abstract.Extend);

			this.UpdateChildrensExtend();
		}
		

		public override void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en évidence l'objet survolé par la souris.
			if ( !this.IsVisible )  return;

			if ( this.list.Rows != this.document.Aggregates.Count )
			{
				this.SetDirtyContent();
				this.Update();
			}

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.list.HiliteColor = context.HiliteSurfaceColor;

			for ( int i=0 ; i<this.document.Aggregates.Count ; i++ )
			{
				Properties.Aggregate agg = this.document.Aggregates[i] as Properties.Aggregate;
				bool hilite = (hiliteObject != null && hiliteObject.Aggregates.Contains(agg));
				this.list.HiliteRow(i, hilite);
			}
		}

		
		protected override void DoUpdateContent()
		{
			//	Effectue la mise à jour du contenu.
			this.list.List = this.document.Aggregates;
			this.list.UpdateContent();
			this.UpdateAggregateName();
			this.UpdateAggregateChildrens();
			this.UpdateToolBarChildrens();
			this.UpdateToolBar();
			this.UpdatePanel();
			this.ListShowSelection();
		}

		protected override void DoUpdateAggregates(System.Collections.ArrayList aggregateList)
		{
			//	Effectue la mise à jour des agrégats.
			foreach ( Properties.Aggregate agg in aggregateList )
			{
				int row = this.document.Aggregates.IndexOf(agg);
				if ( row != -1 )
				{
					this.list.UpdateRow(row);
				}
			}
		}

		protected override void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			//	Effectue la mise à jour des propriétés.
			if ( this.panel != null )
			{
				if ( propertyList.Contains(panel.Property) )
				{
					this.panel.UpdateValues();
				}
			}
		}

		protected void UpdateToolBar()
		{
			//	Met à jour les boutons de la toolbar.
			int total = this.list.Rows;
			int sel = this.document.Aggregates.Selected;

			this.buttonAggregateNewAll.Enable = (!this.document.Modifier.IsTool || this.document.Modifier.TotalSelected > 0);
			this.buttonAggregateUp.Enable = (sel != -1 && sel > 0);
			this.buttonAggregateDuplicate.Enable = (sel != -1);
			this.buttonAggregateDown.Enable = (sel != -1 && sel < total-1);
			this.buttonAggregateDelete.Enable = (sel != -1);

			Properties.Type type = Properties.Type.None;
			bool enableDelete = false;
			Properties.Aggregate agg = this.GetAggregate();
			if ( agg != null )
			{
				type = this.list.SelectedProperty;
				if ( type != Properties.Type.None )
				{
					if ( agg.Property(type) != null )
					{
						enableDelete = true;
					}
				}
			}
			this.buttonStyleNew.Enable = (sel != -1);
			this.buttonStyleDelete.Enable = (enableDelete);
		}


		protected void UpdateAggregateName()
		{
			//	Met à jour le panneau pour éditer le nom de l'agrégat sélectionné.
			Properties.Aggregate agg = this.GetAggregate();

			string text = "";
			if ( agg != null )
			{
				text = agg.AggregateName;
			}

			this.ignoreChanged = true;
			this.name.Text = text;
			this.ignoreChanged = false;
		}

		protected void UpdateChildrensExtend()
		{
			//	Met à jour les panneaux des enfants selon le mode réduit/étendu.
			this.buttonChildrensExtend.GlyphShape = this.isChildrensExtended ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;
			this.toolBarChildrens.Visibility = (this.isChildrensExtended);
			this.childrens.Visibility = (this.isChildrensExtended);
		}

		protected void UpdateToolBarChildrens()
		{
			//	Met à jour les boutons de la toolbar des enfants.
			int aggSel = this.list.SelectedPropertyRow;
			int total = this.childrens.Rows;
			int sel = this.childrens.SelectedPropertyRow;

			this.buttonChildrensNew.Enable = (aggSel != -1);
			this.buttonChildrensUp.Enable = (sel != -1 && sel > 0);
			this.buttonChildrensDown.Enable = (sel != -1 && sel < total-1);
			this.buttonChildrensDelete.Enable = (sel != -1);
		}

		protected void UpdateAggregateChildrens()
		{
			//	Met à jour le panneau pour éditer les enfants de l'agrégat sélectionné.
			Properties.Aggregate agg = this.GetAggregate();

			if ( agg == null )
			{
				this.childrens.List = null;
			}
			else
			{
				this.childrens.List = agg.Childrens;
				this.childrens.SelectRow(agg.Childrens.Selected, true);
			}

			this.childrens.UpdateContent();
		}

		protected void UpdatePanel()
		{
			//	Met à jour le panneau pour éditer la propriété sélectionnée.
			this.colorSelector.Visibility = false;
			this.colorSelector.BackColor = Color.Empty;

			if ( this.panel != null )
			{
				this.panel.Changed -= new EventHandler(this.HandlePanelChanged);
				this.panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				this.panel.Dispose();
				this.panel = null;
				this.panelContainer.Height = 0.0;
				this.panelContainer.ForceLayout();
			}

			Properties.Aggregate agg = this.GetAggregate();
			if ( agg == null )  return;

			Properties.Type type = this.list.SelectedProperty;
			if ( type == Properties.Type.None )  return;

			Properties.Abstract property = agg.Property(type);
			if ( property == null )  return;

			this.panel = property.CreatePanel(this.document);
			if ( this.panel == null )  return;

			this.panel.Property = property;
			this.panel.IsExtendedSize = true;
			this.panel.IsLayoutDirect = true;
			this.panel.Changed += new EventHandler(this.HandlePanelChanged);
			this.panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);
			this.panel.SetParent(this.panelContainer);
			this.panel.Dock = DockStyle.Fill;
			this.panel.TabIndex = 1;
			this.panel.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.panelContainer.Height = this.panel.DefaultHeight;
			this.panelContainer.ForceLayout();
		}

		protected void ListShowSelection()
		{
			//	Montre la ligne sélectionnée dans la liste des agrégats.
			Properties.Aggregate agg = this.GetAggregate();
			if ( agg != null )
			{
				int row, column;
				this.list.GetSelectedRowColumn(out row, out column);
				this.list.ShowCell(row, column);
			}
		}


		private void HandleButtonAggregateNewEmpty(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel agrégat.
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateNewEmpty(sel, "", true);
		}

		private void HandleButtonAggregateNew3(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel agrégat.
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateNew3(sel, "", true);
		}

		private void HandleButtonAggregateNewAll(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel agrégat.
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateNewAll(sel, "", true);
		}

		private void HandleButtonAggregateDuplicate(object sender, MessageEventArgs e)
		{
			//	Duplique un agrégat.
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateDuplicate(sel);
		}

		private void HandleButtonAggregateUp(object sender, MessageEventArgs e)
		{
			//	Monte d'une ligne l'agrégat sélectionné.
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateSwap(sel, sel-1);
		}

		private void HandleButtonAggregateDown(object sender, MessageEventArgs e)
		{
			//	Descend d'une ligne l'agrégat sélectionné.
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateSwap(sel, sel+1);
		}

		private void HandleButtonAggregateDelete(object sender, MessageEventArgs e)
		{
			//	Supprime l'agrégat sélectionné.
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateDelete(sel);
		}

		private void HandleAggregatesTableSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste.
			this.list.SelectCell(1, this.list.SelectedRow, true);
			this.list.SelectCell(2, this.list.SelectedRow, true);

			if ( this.document.Aggregates.Selected != this.list.SelectedPropertyRow )
			{
				this.document.Modifier.OpletQueueEnable = false;
				this.document.Aggregates.Selected = this.list.SelectedPropertyRow;
				this.document.Modifier.OpletQueueEnable = true;
			}

			Properties.Aggregate agg = this.GetAggregate();
			if ( agg != null )
			{
				Properties.Type type = this.list.SelectedProperty;
				Properties.Abstract property = agg.Property(type);
				this.document.Modifier.OpletQueueEnable = false;
				agg.Styles.Selected = agg.Styles.IndexOf(property);
				this.document.Modifier.OpletQueueEnable = true;
			}

			this.UpdateToolBar();
			this.UpdatePanel();
			this.UpdateAggregateName();
			this.UpdateAggregateChildrens();
			this.UpdateToolBarChildrens();
			this.ListShowSelection();
		}

		private void HandleAggregatesTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Liste double-cliquée.
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleAggregatesTableFlyOverChanged(object sender)
		{
			//	La cellule survolée a changé.
			int rank = this.list.FlyOverRow;

			Properties.Aggregate agg = null;
			if ( rank != -1 )
			{
				agg = this.document.Aggregates[rank] as Properties.Aggregate;
			}

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer) )
			{
				obj.IsHilite = (agg != null && obj.Aggregates.Contains(agg));
			}

			this.list.HiliteColor = context.HiliteSurfaceColor;
			int total = this.document.Aggregates.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				this.list.HiliteRow(i, i==rank);
			}
		}

		private void HandleButtonChildrensNew(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel enfant.
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenuChildrens(pos);
			if ( menu == null )  return;
			menu.Host = this;

			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			if ( pos.Y-menu.Height < area.Bottom )  // dépasse en bas ?
			{
				pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
				pos.Y += menu.Height;  // déroule contre le haut ?
			}

			if ( pos.X+menu.Width > area.Right )  // dépasse à droite ?
			{
				pos.X -= pos.X+menu.Width-area.Right;
			}

			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleButtonChildrensUp(object sender, MessageEventArgs e)
		{
			//	Enfant en haut.
			int sel = this.childrens.SelectedPropertyRow;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateChildrensSwap(agg, sel, sel-1);
		}

		private void HandleButtonChildrensDown(object sender, MessageEventArgs e)
		{
			//	Enfant en bas.
			int sel = this.childrens.SelectedPropertyRow;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateChildrensSwap(agg, sel, sel+1);
		}

		private void HandleButtonChildrensDelete(object sender, MessageEventArgs e)
		{
			//	Supprime l'enfant.
			int sel = this.childrens.SelectedPropertyRow;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.GetAggregate();
			Properties.Aggregate delAgg = agg.Childrens[sel] as Properties.Aggregate;
			this.document.Modifier.AggregateChildrensDelete(agg, delAgg);
		}

		private void HandleButtonChildrensExtend(object sender, MessageEventArgs e)
		{
			//	Etend/réduit le panneau des enfants.
			this.isChildrensExtended = !this.isChildrensExtended;
			this.UpdateChildrensExtend();
			this.ForceLayout();
		}

		private void HandleAggregatesChildrensSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste des enfants.
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.OpletQueueEnable = false;
			agg.Childrens.Selected = this.childrens.SelectedRow;
			this.document.Modifier.OpletQueueEnable = true;

			for ( int i=0 ; i<this.childrens.Columns ; i++ )
			{
				this.childrens.SelectCell(i, this.childrens.SelectedRow, true);
			}

			this.UpdateToolBarChildrens();
		}


		private void HandleButtonStyleNew(object sender, MessageEventArgs e)
		{
			//	Crée une nouvelle propriété.
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenuTypes(pos);
			menu.Host = this;

			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			if ( pos.Y-menu.Height < area.Bottom )  // dépasse en bas ?
			{
				pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
				pos.Y += menu.Height;  // déroule contre le haut ?
			}

			if ( pos.X+menu.Width > area.Right )  // dépasse à droite ?
			{
				pos.X -= pos.X+menu.Width-area.Right;
			}

			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleButtonStyleDelete(object sender, MessageEventArgs e)
		{
			//	Supprime la propriété sélectionnée.
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateStyleDelete(agg);
			this.UpdatePanel();
		}

		private void HandleNameTextChanged(object sender)
		{
			//	Le nom de l'agrégat a changé.
			if ( this.ignoreChanged )  return;

			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  return;

			Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChange, "ChangeAggregateName", sel);
			agg.AggregateName = this.name.Text;
			this.document.Modifier.OpletQueueValidateAction();
			this.document.IsDirtySerialize = true;

			this.document.Notifier.NotifyAggregateChanged(agg);
		}

		private void HandlePanelChanged(object sender)
		{
			//	Le contenu du panneau a changé.
			int sel = this.list.SelectedPropertyRow;
			if ( sel != -1 )
			{
				this.list.UpdateRow(sel);

				double h = this.panel.DefaultHeight;
				if ( h != this.panelContainer.Height )
				{
					this.panel.Height = h;
					this.panelContainer.Height = h;
					this.panelContainer.ForceLayout();
				}
			}
		}

		private void HandleOriginColorChanged(object sender)
		{
			//	Le widget qui détermine la couleur d'origine a changé.
			this.colorSelector.Visibility = true;
			this.ignoreChanged = true;
			this.colorSelector.Color = this.panel.OriginColorGet();
			this.ignoreChanged = false;
			this.panel.OriginColorSelect(this.panel.OriginColorRank());
		}

		private void HandleColorSelectorChanged(object sender)
		{
			//	Couleur changée dans la roue.
			if ( this.ignoreChanged )  return;
			this.panel.OriginColorChange(this.colorSelector.Color);
		}

		private void HandleColorSelectorClosed(object sender)
		{
			//	Fermer la roue.
			this.panel.OriginColorDeselect();

			this.colorSelector.Visibility = false;
			this.colorSelector.BackColor = Color.Empty;
		}


		#region MenuTypes
		protected VMenu CreateMenuTypes(Point pos)
		{
			//	Construit le menu pour choisir le style.
			Properties.Aggregate agg = this.GetAggregate();
			VMenu menu = new VMenu();
			double back = -1;
			for ( int i=0 ; i<100 ; i++ )
			{
				Properties.Type type = Properties.Abstract.SortOrder(i);
				if ( !Properties.Abstract.StyleAbility(type) )  continue;

				if ( back != -1 && back != Properties.Abstract.BackgroundIntensity(type) )
				{
					menu.Items.Add(new MenuSeparator());
				}
				back = Properties.Abstract.BackgroundIntensity(type);

				string icon = Misc.Image(Properties.Abstract.IconText(type));
				string text = Properties.Abstract.Text(type);
				string line = string.Format("{0}   {1}", icon, text);
				MenuItem item = new MenuItem("StyleNew", "", line, "", Properties.Abstract.TypeName(type));
				item.Enable = (!this.MenuTypesExist(agg.Styles, type));
				item.Pressed += new MessageEventHandler(this.HandleMenuTypesPressed);
				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}

		protected bool MenuTypesExist(UndoableList styles, Properties.Type type)
		{
			foreach ( Properties.Abstract property in styles )
			{
				if ( property.Type == type )  return true;
			}
			return false;
		}

		private void HandleMenuTypesPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			Properties.Aggregate agg = this.GetAggregate();
			Properties.Type type = Properties.Abstract.TypeName(item.Name);
			this.document.Modifier.AggregateStyleNew(agg, type);
		}
		#endregion

		
		#region MenuChildrens
		protected VMenu CreateMenuChildrens(Point pos)
		{
			//	Construit le menu pour choisir un enfant à ajouter.
			VMenu menu = new VMenu();
			Properties.Aggregate currentAgg = this.GetAggregate();
			int used = 0;
			for ( int i=0 ; i<this.document.Aggregates.Count ; i++ )
			{
				Properties.Aggregate agg = this.document.Aggregates[i] as Properties.Aggregate;
				if ( agg == currentAgg )  continue;
				if ( currentAgg.Childrens.Contains(agg) )  continue;

				string line = agg.AggregateName;
				MenuItem item = new MenuItem("ChildrensNew", "", line, "", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				item.Pressed += new MessageEventHandler(this.HandleMenuChildrensPressed);
				menu.Items.Add(item);
				used ++;
			}
			if ( used == 0 )  return null;
			menu.AdjustSize();
			return menu;
		}

		protected bool MenuChildrensExist(UndoableList styles, Properties.Type type)
		{
			foreach ( Properties.Abstract property in styles )
			{
				if ( property.Type == type )  return true;
			}
			return false;
		}

		private void HandleMenuChildrensPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			int i = System.Int32.Parse(item.Name, System.Globalization.CultureInfo.InvariantCulture);
			Properties.Aggregate newAgg = this.document.Aggregates[i] as Properties.Aggregate;
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateChildrensNew(agg, newAgg);
		}
		#endregion

		
		protected Properties.Aggregate GetAggregate()
		{
			//	Donne l'agrégat sélectionné.
			int sel = this.document.Aggregates.Selected;

			if ( sel == -1 )  return null;
			if ( sel >= this.document.Aggregates.Count )  return null;

			return this.document.Aggregates[sel] as Properties.Aggregate;
		}


		protected HToolBar					toolBar;
		protected IconButton				buttonAggregateNewEmpty;
		protected IconButton				buttonAggregateNew3;
		protected IconButton				buttonAggregateNewAll;
		protected IconButton				buttonAggregateDuplicate;
		protected IconButton				buttonAggregateUp;
		protected IconButton				buttonAggregateDown;
		protected IconButton				buttonAggregateDelete;
		protected IconButton				buttonStyleNew;
		protected IconButton				buttonStyleDelete;
		protected Widgets.AggregateList		list;
		protected HToolBar					toolBarName;
		protected TextField					name;
		protected HToolBar					toolBarChildrens;
		protected IconButton				buttonChildrensNew;
		protected IconButton				buttonChildrensUp;
		protected IconButton				buttonChildrensDown;
		protected IconButton				buttonChildrensDelete;
		protected GlyphButton				buttonChildrensExtend;
		protected Widgets.AggregateList		childrens;
		protected Widget					panelContainer;
		protected Panels.Abstract			panel;
		protected ColorSelector				colorSelector;

		protected bool						isChildrensExtended = false;
		protected bool						ignoreChanged = false;
	}
}
