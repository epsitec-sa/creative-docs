using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.TextStyles contient tous les panneaux des styles.
	/// </summary>
	[SuppressBundleSupport]
	public class TextStyles : Abstract
	{
		public TextStyles(Document document) : base(document)
		{
			// Toolbar principale.
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

			// Table des styles de texte.
			this.list = new Widgets.TextStylesList();
			this.list.Document = this.document;
			this.list.List = this.document.Aggregates;
			this.list.HScroller = true;
			this.list.VScroller = true;
			this.list.SetParent(this);
			this.list.MinSize = new Size(10, 87);
			this.list.Dock = DockStyle.Fill;
			this.list.DockMargins = new Margins(0, 0, 0, 0);
			this.list.FinalSelectionChanged += new EventHandler(this.HandleStylesTableSelectionChanged);
			this.list.FlyOverChanged += new EventHandler(this.HandleStylesTableFlyOverChanged);
			this.list.DoubleClicked += new MessageEventHandler(this.HandleStylesTableDoubleClicked);
			this.list.TabIndex = 2;
			this.list.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			// Roue des couleurs.
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
			this.colorSelector.SetVisible(false);

			// Conteneur du panneau.
			this.panelContainer = new Widget(this);
			this.panelContainer.Height = 0.0;
			this.panelContainer.Dock = DockStyle.Bottom;
			this.panelContainer.DockMargins = new Margins(0, 0, 5, 0);
			this.panelContainer.TabIndex = 99;
			this.panelContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			// Nom de l'agr�gat.
			this.toolBarName = new HToolBar(this);
			this.toolBarName.Dock = DockStyle.Bottom;
			this.toolBarName.DockMargins = new Margins(0, 0, 0, 0);
			this.toolBarName.TabIndex = 96;
			this.toolBarName.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
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
		}
		

		// Effectue la mise � jour du contenu.
		protected override void DoUpdateContent()
		{
			this.list.List = this.document.TextStyles;
			this.list.UpdateContent();
			this.UpdateAggregateName();
			this.UpdateToolBar();
			this.UpdatePanel();
			this.ListShowSelection();
		}

		// Met � jour les boutons de la toolbar.
		protected void UpdateToolBar()
		{
			int total = this.list.Rows;
			int sel = this.list.SelectedPropertyRow;

			this.buttonAggregateNewAll.Enable = (!this.document.Modifier.IsTool || this.document.Modifier.TotalSelected > 0);
			this.buttonAggregateUp.Enable = (sel != -1 && sel > 0);
			this.buttonAggregateDuplicate.Enable = (sel != -1);
			this.buttonAggregateDown.Enable = (sel != -1 && sel < total-1);
			this.buttonAggregateDelete.Enable = (sel != -1);

			Common.Text.Properties.WellKnownType type = Common.Text.Properties.WellKnownType.Other;
			bool enableDelete = false;
			Common.Text.TextStyle style = this.GetStyle();
			if ( style != null )
			{
				type = this.list.SelectedProperty;
				if ( type != Common.Text.Properties.WellKnownType.Other )
				{
					enableDelete = true;
				}
			}
			this.buttonStyleNew.Enable = (sel != -1);
			this.buttonStyleDelete.Enable = (enableDelete);
		}


		// Met � jour le panneau pour �diter le nom de l'agr�gat s�lectionn�.
		protected void UpdateAggregateName()
		{
			Common.Text.TextStyle style = this.GetStyle();

			string text = "";
			if ( style != null )
			{
				text = style.Name;
			}

			this.ignoreChanged = true;
			this.name.Text = text;
			this.ignoreChanged = false;
		}

		// Met � jour le panneau pour �diter la propri�t� s�lectionn�e.
		protected void UpdatePanel()
		{
			this.colorSelector.SetVisible(false);
			this.colorSelector.BackColor = Color.Empty;
		}

		// Montre la ligne s�lectionn�e dans la liste des agr�gats.
		protected void ListShowSelection()
		{
			Common.Text.TextStyle style = this.GetStyle();
			if ( style != null )
			{
				int row, column;
				this.list.GetSelectedRowColumn(out row, out column);
				this.list.ShowCell(row, column);
			}
		}


		// Cr�e un nouvel agr�gat.
		private void HandleButtonAggregateNewEmpty(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateNewEmpty(sel, "", true);
		}

		// Cr�e un nouvel agr�gat.
		private void HandleButtonAggregateNew3(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateNew3(sel, "", true);
		}

		// Cr�e un nouvel agr�gat.
		private void HandleButtonAggregateNewAll(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateNewAll(sel, "", true);
		}

		// Duplique un agr�gat.
		private void HandleButtonAggregateDuplicate(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			if ( sel == -1 )  sel = 10000;
			this.document.Modifier.AggregateDuplicate(sel);
		}

		// Monte d'une ligne l'agr�gat s�lectionn�.
		private void HandleButtonAggregateUp(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateSwap(sel, sel-1);
		}

		// Descend d'une ligne l'agr�gat s�lectionn�.
		private void HandleButtonAggregateDown(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateSwap(sel, sel+1);
		}

		// Supprime l'agr�gat s�lectionn�.
		private void HandleButtonAggregateDelete(object sender, MessageEventArgs e)
		{
			int sel = this.document.Aggregates.Selected;
			this.document.Modifier.AggregateDelete(sel);
		}

		// S�lection chang�e dans la liste.
		private void HandleStylesTableSelectionChanged(object sender)
		{
			this.list.SelectCell(1, this.list.SelectedRow, true);
			this.list.SelectCell(2, this.list.SelectedRow, true);

			this.UpdateToolBar();
			this.UpdatePanel();
			this.UpdateAggregateName();
			this.ListShowSelection();
		}

		// Liste double-cliqu�e.
		private void HandleStylesTableDoubleClicked(object sender, MessageEventArgs e)
		{
			this.name.SelectAll();
			this.name.Focus();
		}

		// La cellule survol�e a chang�.
		private void HandleStylesTableFlyOverChanged(object sender)
		{
		}


		// Cr�e une nouvelle propri�t�.
		private void HandleButtonStyleNew(object sender, MessageEventArgs e)
		{
		}

		// Supprime la propri�t� s�lectionn�e.
		private void HandleButtonStyleDelete(object sender, MessageEventArgs e)
		{
			Common.Text.TextStyle style = this.GetStyle();
			// TODO: � faire
			this.UpdatePanel();
		}

		// Le nom de l'agr�gat a chang�.
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
		}

		// Le contenu du panneau a chang�.
		private void HandlePanelChanged(object sender)
		{
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

		// Le widget qui d�termine la couleur d'origine a chang�.
		private void HandleOriginColorChanged(object sender)
		{
			this.colorSelector.SetVisible(true);
			this.ignoreChanged = true;
			this.colorSelector.Color = this.panel.OriginColorGet();
			this.ignoreChanged = false;
			this.panel.OriginColorSelect(this.panel.OriginColorRank());
		}

		// Couleur chang�e dans la roue.
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


		// Donne le style s�lectionn�.
		protected Text.TextStyle GetStyle()
		{
			int sel = this.list.SelectedPropertyRow;
			if ( sel == -1 )  return null;
			return this.list.List[sel] as Text.TextStyle;
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
		protected Widgets.TextStylesList	list;
		protected HToolBar					toolBarName;
		protected TextField					name;
		protected Widget					panelContainer;
		protected TextPanels.Abstract		panel;
		protected ColorSelector				colorSelector;

		protected bool						ignoreChanged = false;
	}
}
