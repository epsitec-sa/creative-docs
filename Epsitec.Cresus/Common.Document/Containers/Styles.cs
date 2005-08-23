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
			int tabIndex = 0;

			// Toolbar principale.
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = tabIndex++;
			this.toolBar.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;

			this.buttonNewAggregate = new IconButton(Misc.Icon("AggregateNew3"));
			this.buttonNewAggregate.Clicked += new MessageEventHandler(this.HandleButtonNewAggregate);
			this.toolBar.Items.Add(this.buttonNewAggregate);
			ToolTip.Default.SetToolTip(this.buttonNewAggregate, Res.Strings.Action.AggregateNew3);

			this.buttonDuplicateAggregate = new IconButton(Misc.Icon("AggregateDuplicate"));
			this.buttonDuplicateAggregate.Clicked += new MessageEventHandler(this.HandleButtonDuplicateAggregate);
			this.toolBar.Items.Add(this.buttonDuplicateAggregate);
			ToolTip.Default.SetToolTip(this.buttonDuplicateAggregate, Res.Strings.Action.AggregateDuplicate);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUpAggregate = new IconButton(Misc.Icon("AggregateUp"));
			this.buttonUpAggregate.Clicked += new MessageEventHandler(this.HandleButtonUpAggregate);
			this.toolBar.Items.Add(this.buttonUpAggregate);
			ToolTip.Default.SetToolTip(this.buttonUpAggregate, Res.Strings.Action.AggregateUp);

			this.buttonDownAggregate = new IconButton(Misc.Icon("AggregateDown"));
			this.buttonDownAggregate.Clicked += new MessageEventHandler(this.HandleButtonDownAggregate);
			this.toolBar.Items.Add(this.buttonDownAggregate);
			ToolTip.Default.SetToolTip(this.buttonDownAggregate, Res.Strings.Action.AggregateDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDeleteAggregate = new IconButton(Misc.Icon("AggregateDelete"));
			this.buttonDeleteAggregate.Clicked += new MessageEventHandler(this.HandleButtonDeleteAggregate);
			this.toolBar.Items.Add(this.buttonDeleteAggregate);
			ToolTip.Default.SetToolTip(this.buttonDeleteAggregate, Res.Strings.Action.AggregateDelete);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonNewStyle = new IconButton(Misc.Icon("AggregateStyleNew"));
			this.buttonNewStyle.Clicked += new MessageEventHandler(this.HandleButtonNewStyle);
			this.toolBar.Items.Add(this.buttonNewStyle);
			ToolTip.Default.SetToolTip(this.buttonNewStyle, Res.Strings.Action.AggregateStyleNew);

			this.buttonDeleteStyle = new IconButton(Misc.Icon("AggregateStyleDelete"));
			this.buttonDeleteStyle.Clicked += new MessageEventHandler(this.HandleButtonDeleteStyle);
			this.toolBar.Items.Add(this.buttonDeleteStyle);
			ToolTip.Default.SetToolTip(this.buttonDeleteStyle, Res.Strings.Action.AggregateStyleDelete);

			// Table des agrégats.
			this.list = new Widgets.AggregateList();
			this.list.Document = this.document;
			this.list.HScroller = true;
			this.list.VScroller = true;
			this.list.Parent = this;
			this.list.Dock = DockStyle.Fill;
			this.list.DockMargins = new Margins(0, 0, 0, 0);
			this.list.FinalSelectionChanged += new EventHandler(this.HandleAggregatesTableSelectionChanged);
			this.list.FlyOverChanged += new EventHandler(this.HandleAggregatesTableFlyOverChanged);
			this.list.DoubleClicked += new MessageEventHandler(this.HandleAggregatesTableDoubleClicked);
			this.list.TabIndex = tabIndex++;
			this.list.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;

			// Roue des couleurs.
			this.colorSelector = new ColorSelector();
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.HasCloseButton = true;
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += new EventHandler(this.HandleColorSelectorClosed);
			this.colorSelector.Parent = this;
			this.colorSelector.Dock = DockStyle.Bottom;
			this.colorSelector.DockMargins = new Margins(0, 0, 5, 0);
			this.colorSelector.TabIndex = tabIndex++;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.colorSelector.SetVisible(false);

			// Conteneur du panneau.
			this.panelContainer = new Widget(this);
			this.panelContainer.Height = 0.0;
			this.panelContainer.Dock = DockStyle.Bottom;
			this.panelContainer.DockMargins = new Margins(0, 0, 5, 0);
			this.panelContainer.TabIndex = tabIndex++;
			this.panelContainer.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;

			// Parent de l'agrégat.
			this.toolBarParent = new HToolBar(this);
			this.toolBarParent.Dock = DockStyle.Bottom;
			this.toolBarParent.DockMargins = new Margins(0, 0, 0, 0);
			this.toolBarParent.TabIndex = tabIndex++;
			this.toolBarParent.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;

			StaticText st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.AggregateParent.Label.Name;
			this.toolBarParent.Items.Add(st);

			this.parentCombo = new Widgets.AggregateCombo();
			this.parentCombo.Document = this.document;
			this.parentCombo.IsReadOnly = true;
			this.parentCombo.Width = 140;
			this.parentCombo.DockMargins = new Margins(0, 0, 1, 1);
			this.parentCombo.OpeningCombo += new CancelEventHandler(this.HandleParentOpeningCombo);
			this.parentCombo.ClosedCombo += new EventHandler(this.HandleParentClosedCombo);
			this.toolBarParent.Items.Add(this.parentCombo);
			ToolTip.Default.SetToolTip(this.parentCombo, Res.Strings.Panel.AggregateParent.Tooltip.Name);

			// Nom de l'agrégat.
			this.toolBarName = new HToolBar(this);
			this.toolBarName.Dock = DockStyle.Bottom;
			this.toolBarName.DockMargins = new Margins(0, 0, 0, 0);
			this.toolBarName.TabIndex = tabIndex++;
			this.toolBarName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;

			st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.AggregateName.Label.Name;
			this.toolBarName.Items.Add(st);

			this.name = new TextField();
			this.name.Width = 140;
			this.name.DockMargins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += new EventHandler(this.HandleNameTextChanged);
			this.toolBarName.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.AggregateName.Tooltip.Name);
		}
		

		// Met en évidence l'objet survolé par la souris.
		public override void Hilite(Objects.Abstract hiliteObject)
		{
			if ( !this.IsVisible )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.list.HiliteColor = context.HiliteSurfaceColor;

			for ( int i=0 ; i<this.document.Aggregates.Count ; i++ )
			{
				Properties.Aggregate agg = this.document.Aggregates[i] as Properties.Aggregate;
				bool hilite = (hiliteObject != null && hiliteObject.Aggregate == agg);
				this.list.HiliteRow(i, hilite);
			}
		}

		
		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			this.list.UpdateContent();
			this.UpdateAggregateName();
			this.UpdateAggregateParent();
			this.UpdateToolBar();
			this.UpdatePanel();
			this.ListShowSelection();
		}

		// Effectue la mise à jour des agrégats.
		protected override void DoUpdateAggregates(System.Collections.ArrayList aggregateList)
		{
			foreach ( Properties.Aggregate agg in aggregateList )
			{
				int row = this.document.Aggregates.IndexOf(agg);
				if ( row != -1 )
				{
					this.list.UpdateRow(row);
				}
			}
		}

		// Effectue la mise à jour des propriétés.
		protected override void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			if ( this.panel != null )
			{
				if ( propertyList.Contains(panel.Property) )
				{
					this.panel.UpdateValues();
				}
			}
		}

		// Met à jour les boutons de la toolbar.
		protected void UpdateToolBar()
		{
			int total = this.list.Rows;
			int sel = this.document.Aggregates.Selected;

			this.buttonUpAggregate.SetEnabled(sel != -1 && sel > 0);
			this.buttonDuplicateAggregate.SetEnabled(sel != -1);
			this.buttonDownAggregate.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDeleteAggregate.SetEnabled(sel != -1);

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
			this.buttonNewStyle.SetEnabled(sel != -1);
			this.buttonDeleteStyle.SetEnabled(enableDelete);
		}


		// Met à jour le panneau pour éditer le nom de l'agrégat sélectionné.
		protected void UpdateAggregateName()
		{
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

		// Met à jour le panneau pour éditer le parent de l'agrégat sélectionné.
		protected void UpdateAggregateParent()
		{
			Properties.Aggregate agg = this.GetAggregate();

			string name = "";
			if ( agg != null && agg.Parent != null )
			{
				name = agg.Parent.AggregateName;
			}

			this.ignoreChanged = true;
			this.parentCombo.Text = name;
			this.ignoreChanged = false;
		}

		// Met à jour le panneau pour éditer la propriété sélectionnée.
		protected void UpdatePanel()
		{
			this.colorSelector.SetVisible(false);
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
			this.panel.Parent = this.panelContainer;
			this.panel.Dock = DockStyle.Fill;
			this.panelContainer.Height = this.panel.DefaultHeight;
			this.panelContainer.ForceLayout();
		}

		// Montre la ligne sélectionnée dans la liste des agrégats.
		protected void ListShowSelection()
		{
			Properties.Aggregate agg = this.GetAggregate();
			if ( agg != null )
			{
				int row, column;
				this.list.GetSelectedRowColumn(out row, out column);
				this.list.ShowCell(row, column);
			}
		}


		// Crée un nouvel agrégat.
		private void HandleButtonNewAggregate(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateNew3(sel, "", true);
		}

		// Duplique un agrégat.
		private void HandleButtonDuplicateAggregate(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateDuplicate(sel);
		}

		// Monte d'une ligne l'agrégat sélectionné.
		private void HandleButtonUpAggregate(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateSwap(sel, sel-1);
		}

		// Descend d'une ligne l'agrégat sélectionné.
		private void HandleButtonDownAggregate(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateSwap(sel, sel+1);
		}

		// Supprime l'agrégat sélectionné.
		private void HandleButtonDeleteAggregate(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateDelete(sel);
		}

		// Sélection changée dans la liste.
		private void HandleAggregatesTableSelectionChanged(object sender)
		{
			this.list.SelectCell(1, this.list.SelectedRow, true);
			this.list.SelectCell(2, this.list.SelectedRow, true);

			if ( this.document.Aggregates.Selected != this.list.SelectedPropertyRow )
			{
				this.document.Modifier.OpletQueueEnable = false;
				this.document.Aggregates.Selected = this.list.SelectedPropertyRow;
				this.document.Modifier.OpletQueueEnable = true;
			}

			Properties.Aggregate agg = this.GetAggregate();
			Properties.Type type = this.list.SelectedProperty;
			Properties.Abstract property = agg.Property(type);
			this.document.Modifier.OpletQueueEnable = false;
			agg.Styles.Selected = agg.Styles.IndexOf(property);
			this.document.Modifier.OpletQueueEnable = true;

			this.UpdateToolBar();
			this.UpdatePanel();
			this.UpdateAggregateName();
			this.UpdateAggregateParent();
			this.ListShowSelection();
		}

		// Liste double-cliquée.
		private void HandleAggregatesTableDoubleClicked(object sender, MessageEventArgs e)
		{
			this.name.SelectAll();
			this.name.Focus();
		}

		// La cellule survolée a changé.
		private void HandleAggregatesTableFlyOverChanged(object sender)
		{
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
				obj.IsHilite = (agg != null && obj.Aggregate == agg);
			}

			this.list.HiliteColor = context.HiliteSurfaceColor;
			int total = this.document.Aggregates.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				this.list.HiliteRow(i, i==rank);
			}
		}


		// Crée une nouvelle propriété.
		private void HandleButtonNewStyle(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenu(pos);
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

		// Supprime la propriété sélectionnée.
		private void HandleButtonDeleteStyle(object sender, MessageEventArgs e)
		{
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateStyleDelete(agg);
			this.UpdatePanel();
		}

		// Le nom de l'agrégat a changé.
		private void HandleNameTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  return;

			Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChange, "ChangeAggregateName", sel);
			agg.AggregateName = this.name.Text;
			this.document.Modifier.OpletQueueValidateAction();
			this.document.IsDirtySerialize = true;

			this.document.Notifier.NotifyAggregateChanged(agg);

			foreach ( Properties.Aggregate a in this.document.Aggregates )
			{
				if ( a.Parent == agg )
				{
					this.document.Notifier.NotifyAggregateChanged(a);
				}
			}
		}

		// Le combo des agrégats sera ouvert.
		private void HandleParentOpeningCombo(object sender, CancelEventArgs e)
		{
			this.parentCombo.IsNoneLine = true;
			this.parentCombo.ExcludeRank = this.document.Aggregates.Selected;
		}

		// Combo des agrégats fermé.
		private void HandleParentClosedCombo(object sender)
		{
			if ( this.ignoreChanged )  return;
			int sel = this.parentCombo.SelectedIndex;
			if ( sel == -1 )  return;

			Properties.Aggregate parent = null;
			if ( sel != -2 )  // pas ligne <aucun> ?
			{
				parent = this.document.Aggregates[sel] as Properties.Aggregate;
			}

			sel = this.document.Aggregates.Selected;
			Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;

			this.document.Modifier.AggregateParent(agg, parent);
		}

		// Le contenu du panneau a changé.
		private void HandlePanelChanged(object sender)
		{
			int sel = this.list.SelectedPropertyRow;
			if ( sel != -1 )
			{
				this.list.UpdateRow(sel);
			}
		}

		// Le widget qui détermine la couleur d'origine a changé.
		private void HandleOriginColorChanged(object sender)
		{
			this.colorSelector.SetVisible(true);
			this.ignoreChanged = true;
			this.colorSelector.Color = this.panel.OriginColorGet();
			this.ignoreChanged = false;
			this.panel.OriginColorSelect(this.panel.OriginColorRank());
		}

		// Couleur changée dans la roue.
		private void HandleColorSelectorChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.panel.OriginColorChange(this.colorSelector.Color);
		}

		// Fermer la roue.
		private void HandleColorSelectorClosed(object sender)
		{
			this.panel.OriginColorDeselect();

			this.colorSelector.SetVisible(false);
			this.colorSelector.BackColor = Color.Empty;
		}


		#region CreateMenu
		// Construit le menu pour choisir le style.
		protected VMenu CreateMenu(Point pos)
		{
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
				item.SetEnabled(!this.MenuExist(agg.Styles, type));
				item.Pressed += new MessageEventHandler(this.HandleMenuPressed);
				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}

		protected bool MenuExist(UndoableList styles, Properties.Type type)
		{
			foreach ( Properties.Abstract property in styles )
			{
				if ( property.Type == type )  return true;
			}
			return false;
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			Properties.Aggregate agg = this.GetAggregate();
			Properties.Type type = Properties.Abstract.TypeName(item.Name);
			this.document.Modifier.AggregateStyleNew(agg, type);
		}
		#endregion

		
		// Donne l'agrégat sélectionné.
		protected Properties.Aggregate GetAggregate()
		{
			int sel = this.document.Aggregates.Selected;

			if ( sel == -1 )  return null;
			if ( sel >= this.document.Aggregates.Count )  return null;

			return this.document.Aggregates[sel] as Properties.Aggregate;
		}


		protected HToolBar					toolBar;
		protected IconButton				buttonNewAggregate;
		protected IconButton				buttonDuplicateAggregate;
		protected IconButton				buttonUpAggregate;
		protected IconButton				buttonDownAggregate;
		protected IconButton				buttonDeleteAggregate;
		protected IconButton				buttonNewStyle;
		protected IconButton				buttonDeleteStyle;
		protected Widgets.AggregateList		list;
		protected HToolBar					toolBarName;
		protected TextField					name;
		protected HToolBar					toolBarParent;
		protected Widgets.AggregateCombo	parentCombo;
		protected Widget					panelContainer;
		protected Panels.Abstract			panel;
		protected ColorSelector				colorSelector;

		protected bool						isExtended = false;
		protected bool						ignoreChanged = false;

		protected bool						aggregateTypesDirty = true;
		protected Properties.Type[]			aggregateTypes;
		protected int						aggregateTypesTotal = 0;
	}
}
