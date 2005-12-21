using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelStyles permet de repr�senter la collection de styles.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelStyles : Epsitec.Common.Widgets.Widget
	{
		public PanelStyles(Drawer drawer)
		{
			this.drawer = drawer;

			this.toolBar = new HToolBar(this);

			this.buttonNew = new IconButton(@"file:images/stylenew.icon");
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNew);
			this.toolBar.Items.Add(this.buttonNew);
			ToolTip.Default.SetToolTip(this.buttonNew, "Nouveau style");

			this.buttonDuplicate = new IconButton(@"file:images/duplicate.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, "Dupliquer le style");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton(@"file:images/up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, "Style plus haut");

			this.buttonDown = new IconButton(@"file:images/down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, "Style plus bas");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton(@"file:images/delete.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, "Supprimer le style");

			this.table = new CellTable(this);
			this.table.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.DefHeight = 20;

			this.panel = new Widget(this);

			this.Styles.StyleListChanged += new EventHandler(this.HandleStyleListChanged);
			this.Styles.OneStyleChanged += new StyleEventHandler(this.HandleOneStyleChangedChanged);
		}
		

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.Styles.StyleListChanged -= new EventHandler(this.HandleStyleListChanged);
				this.Styles.OneStyleChanged -= new StyleEventHandler(this.HandleOneStyleChangedChanged);

				this.drawer = null;
				this.toolBar = null;
				this.buttonNew = null;
				this.buttonDuplicate = null;
				this.buttonUp = null;
				this.buttonDown = null;
				this.buttonDelete = null;
				this.table = null;
				this.panel = null;
				this.colorSelector = null;
				this.contextMenu = null;
				this.originColorPanel = null;
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.table == null )  return;

			double dy = this.toolBar.DefaultHeight;

			Drawing.Rectangle rect = this.Client.Bounds;
			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-dy;
			this.toolBar.Bounds = r;

			r.Top = r.Bottom+1;  // chevauchement d'un pixel
			r.Bottom = r.Top-120;
			this.table.Bounds = r;

			r.Top = r.Bottom-10;
			r.Bottom = rect.Bottom;
			this.panel.Bounds = r;

			if ( this.colorSelector != null )
			{
				rect.Left   = 0;
				rect.Right  = this.panel.Width;
				rect.Bottom = 0;
				rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.panel.Height-this.leftHeightUsed);
				this.colorSelector.Bounds = rect;
			}
		}


		public void UpdateAll(int sel)
		{
			//	Met � jour tout le panneau.
			this.UpdateTable();
			this.TableSelect(sel, true, false);
			sel = this.TableSelect();
			if ( sel == -1 )
			{
				this.UpdatePanels(null);
			}
			else
			{
				this.UpdatePanels(this.Styles.GetProperty(sel));
			}
			this.UpdateToolBar();
		}


		private void HandleStyleListChanged(object sender)
		{
			//	La liste des styles a chang�.
			this.UpdateAll(this.TableSelect());
		}

		private void HandleOneStyleChangedChanged(int styleID)
		{
			//	Un style de la collection a chang�, suite � la modification
			//	d'un objet qui utilisait un style.
			AbstractProperty property = null;
			int sel = -1;
			int rows = this.table.Rows;
			for ( int i=0 ; i<rows ; i++ )
			{
				property = this.Styles.GetProperty(i);
				if ( property.StyleID == styleID )
				{
					sel = i;
					break;
				}
			}

			this.TableSelect(sel, true, false);
			this.UpdatePanels(property);
			this.UpdateToolBar();
		}


		protected void UpdateToolBar()
		{
			//	Met � jour les boutons de la toolbar.
			int total = this.table.Rows;
			int sel = this.TableSelect();

			this.buttonUp.SetEnabled(sel != -1 && sel > 0);
			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonDown.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDelete.SetEnabled(sel != -1);
		}

		private void HandleButtonNew(object sender, MessageEventArgs e)
		{
			//	Cr�e une nouvelle propri�t�.
			IconButton button = sender as IconButton;
			Drawing.Point pos = button.MapClientToScreen(new Drawing.Point(0, 0));
			this.CreateMenu(pos);
		}

		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			//	Duplique une propri�t�.
			int sel = this.TableSelect();
			AbstractProperty property = this.Styles.GetProperty(sel);
			this.CommandStyleCreate(property.Type);

			sel = this.TableSelect();
			AbstractProperty newProp = this.Styles.GetProperty(sel);
			int    id   = newProp.StyleID;
			string name = newProp.StyleName;
			property.CopyTo(newProp);
			newProp.StyleID   = id;
			newProp.StyleName = name;
			this.UpdateTable();
			this.UpdatePanels(newProp);
		}

		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			//	Monte d'une ligne la propri�t� s�lectionn�e.
			this.drawer.UndoBeginning("StyleUp");
			int sel = this.TableSelect();
			this.Styles.SwapProperty(sel, sel-1);
			this.drawer.UndoValidate();

			this.UpdateTable();
			this.TableSelect(sel-1, true, false);
			this.UpdateToolBar();
		}

		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			//	Descend d'une ligne la propri�t� s�lectionn�e.
			this.drawer.UndoBeginning("StyleDown");
			int sel = this.TableSelect();
			this.Styles.SwapProperty(sel, sel+1);
			this.drawer.UndoValidate();

			this.UpdateTable();
			this.TableSelect(sel+1, true, false);
			this.UpdateToolBar();
		}

		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			//	Supprime la propri�t� s�lectionn�e.
			int sel = this.TableSelect();
			AbstractProperty property = this.Styles.GetProperty(sel);

			this.drawer.UndoBeginning("StyleDeleted", null, 0, property.Type, property.StyleID);
			this.drawer.StyleFreeAll(property);
			this.Styles.RemoveProperty(sel);
			this.drawer.UndoValidate();

			this.UpdateTable();
			if ( sel >= this.table.Rows-1 )  sel = this.table.Rows-1;
			this.TableSelect(sel, true, false);
			this.UpdatePanels((sel == -1) ? null : this.Styles.GetProperty(sel));
			this.UpdateToolBar();
		}


		protected void UpdateTable()
		{
			//	Met � jour le contenu de la table.
			int rows = this.Styles.TotalProperty;
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(2, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 84);
				this.table.SetWidthColumn(1, 110);
			}

			this.table.SetHeaderTextH(0, "Type");
			this.table.SetHeaderTextH(1, "Nom");

			StaticText		st;
			TextField		edit;

			for ( int i=0 ; i<rows ; i++ )
			{
				AbstractProperty property = this.Styles.GetProperty(i);
				AbstractProperty refProp = this.drawer.NewProperty(property.Type);

				if ( this.table[0, i].IsEmpty )
				{
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					this.table[0, i].Insert(st);
				}
				st = this.table[0, i].Children[0] as StaticText;
				st.Text = refProp.Text;

				if ( this.table[1, i].IsEmpty )
				{
					edit = new TextField();
					edit.Dock = DockStyle.Fill;
					edit.Name = i.ToString();
					edit.Clicked += new MessageEventHandler(this.HandleListTextClicked);
					edit.TextChanged += new EventHandler(this.HandleListTextChanged);
					this.table[1, i].Insert(edit);
				}
				edit = this.table[1, i].Children[0] as TextField;
				this.ignoreListTextChanged = true;
				edit.Text = property.StyleName;
				this.ignoreListTextChanged = false;
			}

			this.UpdateToolBar();
		}

		protected void TableSelect(int sel, bool showSelect, bool selectText)
		{
			//	S�lectionne une ligne dans la table.
			int total = this.table.Rows;
			bool exist = false;
			for ( int i=0 ; i<total ; i++ )
			{
				this.table.SelectRow(i, i==sel);
				exist |= (i==sel);
			}

			if ( exist )
			{
				if ( showSelect )  this.table.ShowSelect();
				this.table.Update();

				Cell cell = this.table[1, sel];
				TextField edit = cell.Children[0] as TextField;
				if ( selectText )  edit.SelectAll();
				if ( edit.IsVisible )  edit.Focus();
			}
		}

		protected int TableSelect()
		{
			//	Retourne la ligne s�lectionn�e dans la table.
			int total = this.table.Rows;
			for ( int i=0 ; i<total ; i++ )
			{
				if ( this.table.IsCellSelected(i, 0) )  return i;
			}
			return -1;
		}

		private void HandleTableSelectionChanged(object sender)
		{
			//	Liste cliqu�e.
			int sel = this.TableSelect();
			if ( sel == -1 )  return;
			this.TableSelect(sel, false, true);
			this.UpdatePanels(this.Styles.GetProperty(sel));
			this.UpdateToolBar();
		}

		private void HandleListTextClicked(object sender, MessageEventArgs e)
		{
			//	Ligne �ditable dans la liste cliqu�e.
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			this.TableSelect(sel, false, false);
			this.UpdatePanels(this.Styles.GetProperty(sel));
			this.UpdateToolBar();
		}

		private void HandleListTextChanged(object sender)
		{
			//	Texte de la ligne �ditable dans la liste chang�.
			if ( this.ignoreListTextChanged )  return;
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			AbstractProperty property = this.Styles.GetProperty(sel);

			this.drawer.UndoBeginning("StyleName", null, 0, property.Type, property.StyleID);
			this.drawer.IconObjects.StylesCollection.UndoWillBeChanged(property);
			this.drawer.UndoValidate();

			property.StyleName = edit.Text;
			this.UpdatePanels(property);
			this.drawer.SetProperty(property, false);
		}


		protected void UpdatePanels(AbstractProperty property)
		{
			//	Cr�e le panneau pour �diter la transformation.
			//	Supprime tous les panneaux, sauf le ColorSelector.
			AbstractPanel panel;
			int i = 0;
			while ( i < this.panel.Children.Count )
			{
				if ( this.panel.Children[i] is ColorSelector )
				{
					i ++;
				}
				else
				{
					panel = this.panel.Children[i] as AbstractPanel;
					if ( panel != null )
					{
						panel.Changed -= new EventHandler(this.HandlePanelChanged);
						panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
					}
					this.panel.Children.RemoveAt(i);
				}
			}

			//	Cr�e une fois pour toutes le ColorSelector.
			if ( this.colorSelector == null )
			{
				this.colorSelector = new ColorSelector();
				this.colorSelector.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Bottom;
				this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
				this.colorSelector.TabIndex = 100;
				this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
				this.colorSelector.Parent = this.panel;
			}

			//	Cr�e le panneau selon le style s�lectionn� dans la liste.
			Drawing.Rectangle rect;
			Widget originColorLastPanel = null;
			if ( property == null )
			{
				this.leftHeightUsed = 0;
			}
			else
			{
				panel = property.CreatePanel(this.drawer);
				panel.ExtendedSize = true;
				panel.SetProperty(property);
				panel.StyleDirect = true;

				panel.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.All;
				Drawing.Margins margins = new Drawing.Margins();
				margins.Left   = 1;
				margins.Right  = 1;
				margins.Top    = 1;
				margins.Bottom = this.panel.Height-1-panel.DefaultHeight;
				panel.AnchorMargins = margins;
				panel.Changed += new EventHandler(this.HandlePanelChanged);
				panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);
				panel.Parent = this.panel;

				if ( panel.PropertyType == this.originColorType )
				{
					originColorLastPanel = panel;
				}

				this.leftHeightUsed = panel.Height;
			}

			//	Positionne le ColorSelector.
			rect = new Drawing.Rectangle();
			rect.Left   = 0;
			rect.Right  = this.panel.Width;
			rect.Bottom = 0;
			rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.panel.Height-this.leftHeightUsed);
			this.colorSelector.Bounds = rect;

			this.HandleOriginColorChanged(originColorLastPanel, true);
		}

		private void HandlePanelChanged(object sender)
		{
			//	Le contenu d'un panneau a �t� chang�.
			AbstractPanel panel = sender as AbstractPanel;
			AbstractProperty newProperty = panel.GetProperty();

			int sel = this.TableSelect();
			if ( sel == -1 )  return;
			AbstractProperty currentProperty = this.Styles.GetProperty(sel);

			this.drawer.UndoBeginning("StyleChanged", null, 0, currentProperty.Type, currentProperty.StyleID);
			this.drawer.UndoSelectionWillBeChanged();
			this.drawer.IconObjects.StylesCollection.UndoWillBeChanged(currentProperty);
			this.drawer.UndoValidate();

			newProperty.StyleName = currentProperty.StyleName;
			newProperty.StyleID   = currentProperty.StyleID;
			newProperty.CopyTo(currentProperty);

			this.drawer.SetProperty(newProperty, false);
		}

		private void HandleOriginColorChanged(object sender)
		{
			//	Le widget qui d�termine la couleur d'origine a chang�.
			this.HandleOriginColorChanged(sender, false);
		}

		private void HandleOriginColorChanged(object sender, bool lastOrigin)
		{
			this.originColorPanel = null;

			foreach ( Widget widget in this.panel.Children )
			{
				AbstractPanel panel = widget as AbstractPanel;
				if ( panel == null )  continue;
				Widget wSender = sender as Widget;
				if ( panel == wSender )
				{
					this.originColorPanel = panel;
					panel.OriginColorSelect( lastOrigin ? this.originColorRank : -1 );
				}
				else
				{
					panel.OriginColorDeselect();
				}
			}

			if ( this.originColorPanel == null )
			{
				this.colorSelector.SetEnabled(false);
				this.colorSelector.BackColor = Drawing.Color.Empty;
			}
			else
			{
				this.colorSelector.SetEnabled(true);
				this.colorSelector.BackColor = IconContext.ColorStyleBack;
				this.ignoreColorChanged = true;
				this.colorSelector.Color = this.originColorPanel.OriginColorGet();
				this.ignoreColorChanged = false;
				this.originColorType = this.originColorPanel.PropertyType;
				this.originColorRank = this.originColorPanel.OriginColorRank();
			}
		}

		private void HandleColorSelectorChanged(object sender)
		{
			//	Couleur chang�e dans la roue.
			if ( this.ignoreColorChanged || this.originColorPanel == null )  return;
			this.originColorPanel.OriginColorChange(this.colorSelector.Color);

			AbstractProperty newProperty = this.originColorPanel.GetProperty();
			this.originColorPanel.Multi = false;
		
			int sel = this.TableSelect();
			if ( sel == -1 )  return;
			AbstractProperty currentProperty = this.Styles.GetProperty(sel);

			this.drawer.UndoBeginning("StyleChanged", null, 0, currentProperty.Type, currentProperty.StyleID);
			this.drawer.UndoSelectionWillBeChanged();
			this.drawer.IconObjects.StylesCollection.UndoWillBeChanged(currentProperty);
			this.drawer.UndoValidate();

			newProperty.StyleName = currentProperty.StyleName;
			newProperty.StyleID   = currentProperty.StyleID;
			newProperty.CopyTo(currentProperty);

			this.drawer.SetProperty(newProperty, false);
		}


		protected void CreateMenu(Drawing.Point pos)
		{
			//	Construit le menu pour choisir le style.
			this.contextMenu = new VMenu();
			this.contextMenu.Host = this;

			ObjectMemory memo = new ObjectMemory();
			double back = -1;
			int tp = memo.TotalProperty;
			for ( int i=0 ; i<tp ; i++ )
			{
				AbstractProperty property = memo.Property(i);
				if ( !property.StyleAbility )  continue;

				if ( back != -1 && back != property.BackgroundIntensity )
				{
					this.contextMenu.Items.Add(new MenuSeparator());
				}
				back = property.BackgroundIntensity;

				MenuItem item = new MenuItem("StyleCreate(this.Name)", "", property.Text, "", AbstractProperty.TypeName(property.Type));
				this.contextMenu.Items.Add(item);
			}
			this.contextMenu.AdjustSize();
			this.contextMenu.ShowAsContextMenu(this.Window, pos);
		}

		public void CommandStyleCreate(PropertyType type)
		{
			//	Cr�e un nouveau style.
			//	Ex�cut� suite � la commande contenue dans le menu.
			AbstractProperty property = this.drawer.NewProperty(type);
			if ( property == null )  return;

			this.drawer.UndoBeginning("StyleCreate");
			int sel = this.Styles.CreateProperty(property);
			this.drawer.UndoValidate();

			if ( sel == -1 )  return;
			this.UpdateTable();
			this.TableSelect(sel, true, true);
			this.UpdatePanels(this.Styles.GetProperty(sel));
			this.UpdateToolBar();
		}
		

		protected StylesCollection Styles
		{
			//	Retourne la collection des styles.
			get { return this.drawer.IconObjects.StylesCollection; }
		}


		protected Drawer						drawer;
		protected HToolBar						toolBar;
		protected IconButton					buttonNew;
		protected IconButton					buttonDuplicate;
		protected IconButton					buttonUp;
		protected IconButton					buttonDown;
		protected IconButton					buttonDelete;
		protected CellTable						table;
		protected Widget						panel;
		protected ColorSelector					colorSelector;
		protected int							nextID = 1;
		protected VMenu							contextMenu;
		protected AbstractPanel					originColorPanel = null;
		protected PropertyType					originColorType = PropertyType.None;
		protected int							originColorRank = -1;
		protected double						leftHeightUsed = 0;
		protected bool							ignoreColorChanged = false;
		protected bool							ignoreListTextChanged = false;
	}
}
