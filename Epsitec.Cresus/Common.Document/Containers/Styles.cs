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
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);

			this.buttonNew = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.StyleNew.icon");
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNew);
			this.toolBar.Items.Add(this.buttonNew);
			ToolTip.Default.SetToolTip(this.buttonNew, "Nouveau style");

			this.buttonDuplicate = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.DuplicateItem.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, "Dupliquer le style");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, "Style plus haut");

			this.buttonDown = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, "Style plus bas");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.DeleteItem.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, "Supprimer le style");

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.FlyOverChanged += new EventHandler(this.HandleTableFlyOverChanged);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.DefHeight = 16;

			this.colorSelector = new ColorSelector();
			this.colorSelector.ColorPalette.ColorsCollection = this.document.GlobalSettings.ColorsCollection;
			this.colorSelector.HasCloseButton = true;
			this.colorSelector.Dock = DockStyle.Bottom;
			this.colorSelector.DockMargins = new Margins(0, 0, 10, 0);
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += new EventHandler(this.HandleColorSelectorClosed);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.colorSelector.Parent = this;
			this.colorSelector.SetVisible(false);
		}
		

		// Met en évidence l'objet survolé par la souris.
		public override void Hilite(Objects.Abstract hiliteObject)
		{
			if ( !this.IsVisible )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.table.HiliteColor = context.HiliteSurfaceColor;

			int total = this.document.PropertiesStyle.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Properties.Abstract property = this.document.PropertiesStyle[i] as Properties.Abstract;
				bool hilite = Abstract.IsObjectUseByProperty(property, hiliteObject);
				this.TableHiliteRow(i, hilite);
			}
		}

		
		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			this.UpdateTable();
			this.UpdateToolBar();
		}

		// Effectue la mise à jour des propriétés.
		protected override void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			foreach ( Properties.Abstract property in propertyList )
			{
				int row = this.document.PropertiesStyle.IndexOf(property);
				if ( row != -1 )
				{
					this.TableUpdateRow(row);
				}
			}

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
			int total = this.table.Rows;
			int sel = this.document.PropertiesStyle.Selected;

			this.buttonUp.SetEnabled(sel != -1 && sel > 0);
			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonDown.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDelete.SetEnabled(sel != -1);
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			int rows = this.document.PropertiesStyle.Count;
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(4, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 12);
				this.table.SetWidthColumn(1, 94);
				this.table.SetWidthColumn(2, 84);
				this.table.SetWidthColumn(3, 22);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, "Type");
			this.table.SetHeaderTextH(2, "Nom");
			this.table.SetHeaderTextH(3, "Nb");

			for ( int i=0 ; i<rows ; i++ )
			{
				this.TableFillRow(i);
				this.TableUpdateRow(i);
			}

			this.UpdatePanel();
		}

		// Peuple une ligne de la table, si nécessaire.
		protected void TableFillRow(int row)
		{
			if ( this.table[0, row].IsEmpty )
			{
				GlyphButton gb = new GlyphButton();
				gb.ButtonStyle = ButtonStyle.None;
				gb.Dock = DockStyle.Fill;
				this.table[0, row].Insert(gb);
			}

			for ( int column=1 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.Alignment = (column==3) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					st.DockMargins = new Margins(4, 4, 0, 0);
					this.table[column, row].Insert(st);
				}
			}
		}

		// Met à jour le contenu d'une ligne de la table.
		protected void TableUpdateRow(int row)
		{
			Properties.Abstract property = this.document.PropertiesStyle[row] as Properties.Abstract;
			bool selected = (row == this.document.PropertiesStyle.Selected);
			GlyphButton gb;
			StaticText st;

			gb = this.table[0, row].Children[0] as GlyphButton;
			gb.GlyphShape = GlyphShape.None;
			this.table[0, row].IsHilite = false;

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = Properties.Abstract.Text(property.Type);

			st = this.table[2, row].Children[0] as StaticText;
			st.Text = property.StyleName;

			st = this.table[3, row].Children[0] as StaticText;
			st.Text = property.Owners.Count.ToString();

			this.table.SelectRow(row, selected);
		}

		// Hilite une ligne de la table.
		protected void TableHiliteRow(int row, bool hilite)
		{
			if ( this.table[0, row].IsHilite != hilite )
			{
				this.table[0, row].IsHilite = hilite;
				GlyphButton gb = this.table[0, row].Children[0] as GlyphButton;
				gb.GlyphShape = hilite ? GlyphShape.ArrowRight : GlyphShape.None;
			}
		}

		// Met à jour le panneau pour éditer la propriété sélectionnée.
		protected void UpdatePanel()
		{
			this.colorSelector.SetVisible(false);
			this.colorSelector.BackColor = Color.Empty;

			if ( this.panelStyleName != null )
			{
				panelStyleName.Dispose();
				panelStyleName = null;
			}

			if ( this.panel != null )
			{
				this.panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				this.panel.Dispose();
				this.panel = null;
			}

			int sel = this.document.PropertiesStyle.Selected;

			if ( sel >= this.document.PropertiesStyle.Count )
			{
				sel = this.document.PropertiesStyle.Count-1;
				this.table.SelectRow(sel, true);
			}

			if ( sel != -1 )
			{
				Properties.Abstract property = this.document.PropertiesStyle[sel] as Properties.Abstract;

				this.panel = property.CreatePanel(this.document);
				if ( this.panel == null )  return;
				this.panel.Property = property;
				this.panel.IsExtendedSize = true;
				this.panel.IsLayoutDirect = true;
				this.panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);
				this.panel.TabIndex = 100-1;
				this.panel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
				this.panel.Dock = DockStyle.Bottom;
				this.panel.DockMargins = new Margins(0, 0, 5, 0);
				this.panel.Parent = this;

				this.panelStyleName = new Panels.StyleName(this.document);
				this.panelStyleName.Property = property;
				this.panelStyleName.IsExtendedSize = false;
				this.panelStyleName.IsLayoutDirect = true;
				this.panelStyleName.TabIndex = 100-2;
				this.panelStyleName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
				this.panelStyleName.Dock = DockStyle.Bottom;
				this.panelStyleName.DockMargins = new Margins(0, 0, 5, 0);
				this.panelStyleName.Parent = this;
			}
		}


		// Crée une nouvelle propriété.
		private void HandleButtonNew(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenu(pos);
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		// Duplique une propriété.
		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			int sel = this.document.PropertiesStyle.Selected;
			this.document.Modifier.StyleDuplicate(sel);
		}

		// Monte d'une ligne la propriété sélectionnée.
		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			int sel = this.document.PropertiesStyle.Selected;
			this.document.Modifier.StyleSwap(sel, sel-1);
		}

		// Descend d'une ligne la propriété sélectionnée.
		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			int sel = this.document.PropertiesStyle.Selected;
			this.document.Modifier.StyleSwap(sel, sel+1);
		}

		// Supprime la propriété sélectionnée.
		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			int sel = this.document.PropertiesStyle.Selected;
			this.document.Modifier.StyleDelete(sel);
		}

		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			this.document.Modifier.OpletQueueEnable = false;
			this.document.PropertiesStyle.Selected = this.table.SelectedRow;
			this.document.Modifier.OpletQueueEnable = true;

			this.UpdatePanel();
			this.UpdateToolBar();
		}

		// La cellule survolée a changé.
		private void HandleTableFlyOverChanged(object sender)
		{
			int rank = this.table.FlyOverRow;

			Properties.Abstract property = null;
			if ( rank != -1 )
			{
				property = this.document.PropertiesStyle[rank] as Properties.Abstract;
			}

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer) )
			{
				obj.IsHilite = obj.PropertyExist(property);
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

				MenuItem item = new MenuItem("StyleCreate", "", Properties.Abstract.Text(type), "", Properties.Abstract.TypeName(type));
				item.Pressed += new MessageEventHandler(this.HandleMenuPressed);
				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			int sel = this.document.PropertiesStyle.Selected;
			Properties.Type type = Properties.Abstract.TypeName(item.Name);
			this.document.Modifier.StyleCreate(sel, type);
		}
		#endregion


		protected HToolBar				toolBar;
		protected IconButton			buttonNew;
		protected IconButton			buttonDuplicate;
		protected IconButton			buttonUp;
		protected IconButton			buttonDown;
		protected IconButton			buttonDelete;
		protected CellTable				table;
		protected Panels.StyleName		panelStyleName;
		protected Panels.Abstract		panel;
		protected ColorSelector			colorSelector;
		protected bool					ignoreChanged = false;
	}
}
