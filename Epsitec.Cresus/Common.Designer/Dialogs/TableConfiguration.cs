using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de configurer les rubriques d'une table.
	/// </summary>
	public class TableConfiguration : Abstract
	{
		public TableConfiguration(MainWindow mainWindow) : base(mainWindow)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("TableConfiguration", 500, 400, true);
				this.window.Text = Res.Strings.Dialog.TableDescription.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				//	Crée la barre d'outils.
				this.toolbar = new HToolBar(this.window.Root);
				this.toolbar.Dock = DockStyle.Top;

				this.buttonAdd = new IconButton();
				this.buttonAdd.CaptionId = Res.Captions.Dialog.TableConfiguration.Add.Id;
				this.buttonAdd.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.toolbar.Items.Add(this.buttonAdd);

				this.buttonRemove = new IconButton();
				this.buttonRemove.CaptionId = Res.Captions.Dialog.TableConfiguration.Remove.Id;
				this.buttonRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.toolbar.Items.Add(this.buttonRemove);

				this.toolbar.Items.Add(new IconSeparator());

				this.buttonTemplateAdd = new IconButton();
				this.buttonTemplateAdd.CaptionId = Res.Captions.Dialog.TableConfiguration.TemplateAdd.Id;
				this.buttonTemplateAdd.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.toolbar.Items.Add(this.buttonTemplateAdd);

				this.buttonTemplateRemove = new IconButton();
				this.buttonTemplateRemove.CaptionId = Res.Captions.Dialog.TableConfiguration.TemplateRemove.Id;
				this.buttonTemplateRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.toolbar.Items.Add(this.buttonTemplateRemove);

				this.toolbar.Items.Add(new IconSeparator());

				this.buttonPrev = new IconButton();
				this.buttonPrev.CaptionId = Res.Captions.Dialog.TableConfiguration.Prev.Id;
				this.buttonPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.toolbar.Items.Add(this.buttonPrev);

				this.buttonNext = new IconButton();
				this.buttonNext.CaptionId = Res.Captions.Dialog.TableConfiguration.Next.Id;
				this.buttonNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.toolbar.Items.Add(this.buttonNext);

				this.toolbar.Items.Add(new IconSeparator());

				this.buttonSort = new IconButton();
				this.buttonSort.CaptionId = Res.Captions.Dialog.TableConfiguration.Sort.Id;
				this.buttonSort.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.toolbar.Items.Add(this.buttonSort);

				this.slider = new HSlider(toolbar);
				this.slider.PreferredWidth = 80;
				this.slider.Margins = new Margins(2, 2, 4, 4);
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 50.0M;
				this.slider.SmallChange = 5.0M;
				this.slider.LargeChange = 10.0M;
				this.slider.Resolution = 1.0M;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				this.slider.Value = (decimal) TableConfiguration.arrayLineHeight;
				this.slider.Dock = DockStyle.Right;

				//	Crée l'en-tête du tableau.
				this.header = new Widget(this.window.Root);
				this.header.Dock = DockStyle.Top;
				this.header.Margins = new Margins(0, 0, 4, 0);

				this.headerUse = new HeaderButton(this.header);
				this.headerUse.Text = "";
				this.headerUse.Style = HeaderButtonStyle.Top;
				this.headerUse.Dock = DockStyle.Left;

				this.headerName = new HeaderButton(this.header);
				this.headerName.Text = Res.Strings.Dialog.TableDescription.Name;
				this.headerName.Style = HeaderButtonStyle.Top;
				this.headerName.Dock = DockStyle.Left;

				this.headerCaption = new HeaderButton(this.header);
				this.headerCaption.Text = Res.Strings.Dialog.TableDescription.Caption;
				this.headerCaption.Style = HeaderButtonStyle.Top;
				this.headerCaption.Dock = DockStyle.Left;

				//	Crée le tableau principal.
				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.Columns = 3;
				this.array.SetColumnsRelativeWidth(0, 0.07);
				this.array.SetColumnsRelativeWidth(1, 0.50);
				this.array.SetColumnsRelativeWidth(2, 0.50);
				this.array.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
				this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(2, ContentAlignment.MiddleLeft);
				this.array.SetColumnBreakMode(1, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				this.array.SetColumnBreakMode(2, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				this.array.LineHeight = TableConfiguration.arrayLineHeight;
				this.array.Dock = DockStyle.Fill;
				this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonOk = new Button(footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOk.Dock = DockStyle.Left;
				this.buttonOk.Margins = new Margins(0, 6, 0, 0);
				this.buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOkClicked);
				this.buttonOk.TabIndex = tabIndex++;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Left;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = tabIndex++;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Crée les boutons du bas.
				this.footer = new Widget(this.window.Root);
				this.footer.Dock = DockStyle.Bottom;
				this.footer.Margins = new Margins(0, 0, 4, 8);

				this.footerUse = new Widget(this.footer);  // widget invisible
				this.footerUse.Dock = DockStyle.Left;
				this.footerUse.Margins = new Margins(1, 0, 0, 0);

				this.footerName = new Button(this.footer);
				this.footerName.Text = "Choix de l'interface";  // Res.Strings.Dialog.TableDescription.Name;
				this.footerName.Dock = DockStyle.Left;
				this.footerName.Margins = new Margins(1, 0, 0, 0);
				this.footerName.Clicked += new MessageEventHandler(this.HandleFooterNameClicked);

				this.footerCaption = new Button(this.footer);
				this.footerCaption.Text = "Choix de la légende";  // Res.Strings.Dialog.TableDescription.Caption;
				this.footerCaption.Dock = DockStyle.Left;
				this.footerCaption.Margins = new Margins(1, 0, 0, 0);
				this.footerCaption.Clicked += new MessageEventHandler(this.HandleFooterCaptionClicked);
			}

			this.array.SelectedRow = -1;

			this.UpdateButtons();
			this.UpdateArray();

			this.window.ShowDialog();
		}

		public void Initialise(Module module, StructuredType structuredType, List<UI.ItemTableColumn> columns)
		{
			//	Initialise le dialogue avec l'objet table.
			this.module = module;
			this.resourceAccess = module.AccessCaptions;

			//	Construit la liste de toutes les rubriques existantes.
			List<UI.ItemTableColumn> fullList = new List<UI.ItemTableColumn>();
			foreach (string fieldId in structuredType.GetFieldIds())
			{
				fullList.Add(new UI.ItemTableColumn(fieldId));
			}

			//	Met toutes les rubriques utilisées.
			this.items = new List<Item>();
			foreach (UI.ItemTableColumn column in columns)
			{
				this.items.Add(new Item(true, column));
			}

			//	Ajoute toutes les rubriques existantes mais pas utilisées.
			foreach (UI.ItemTableColumn column in fullList)
			{
				if (!TableConfiguration.Contains(columns, column))
				{
					this.items.Add(new Item(false, column));
				}
			}

			this.columnsReturned = null;
		}

		public List<UI.ItemTableColumn> Columns
		{
			get
			{
				return this.columnsReturned;
			}
		}


		protected static bool Contains(List<UI.ItemTableColumn> columns, UI.ItemTableColumn column)
		{
			foreach (UI.ItemTableColumn c in columns)
			{
				if (c.FieldId == column.FieldId)
				{
					return true;
				}
			}
			return false;
		}

		protected List<UI.ItemTableColumn> SelectedList
		{
			get
			{
				List<UI.ItemTableColumn> list = new List<Epsitec.Common.UI.ItemTableColumn>();

				foreach (Item item in this.items)
				{
					if (item.Used)
					{
						list.Add(item.Column);
					}
				}

				return list;
			}
		}


		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons en fonction de la ligne sélectionnée dans le tableau.
			int sel = this.array.SelectedRow;

			this.buttonAdd.Enable = (sel != -1 && !this.items[sel].Used && !this.items[sel].IsTemplate);
			this.buttonRemove.Enable = (sel != -1 && this.items[sel].Used && !this.items[sel].IsTemplate);

			this.buttonTemplateAdd.Enable = (sel != -1);
			this.buttonTemplateRemove.Enable = (sel != -1 && this.items[sel].IsTemplate);

			this.buttonPrev.Enable = (sel != -1 && sel > 0);
			this.buttonNext.Enable = (sel != -1 && sel < this.items.Count-1);

			this.footerName.Enable = (sel != -1 && this.items[sel].IsTemplate);
			this.footerCaption.Enable = (sel != -1);
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.items.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.items.Count)
				{
					Item item = this.items[first+i];
					string name = item.Column.FieldId;

					string icon = item.Used ? Misc.Image("TypeEnumYes") : "";
					MyWidgets.StringList.CellState cs = item.Used ? MyWidgets.StringList.CellState.Normal : MyWidgets.StringList.CellState.Unused;

					if (item.IsTemplate)
					{
						name = Misc.Italic("Interface");

						ResourceBundle bundle = this.module.ResourceManager.GetBundle(item.Column.TemplateId);
						if (bundle != null)
						{
							Caption caption = this.module.ResourceManager.GetCaption(bundle.Id);
							if (caption != null)
							{
								name = caption.Name;
							}
						}

						icon = Misc.Image("ObjectPanel");
					}

					string description = "";
					if (!item.Column.CaptionId.IsEmpty)
					{
						Caption caption = this.module.ResourceManager.GetCaption(item.Column.CaptionId);
						if (caption != null)
						{
							description = ResourceAccess.GetCaptionNiceDescription(caption, TableConfiguration.arrayLineHeight);
						}
					}

					this.array.SetLineString(0, first+i, icon);
					this.array.SetLineState(0, first+i, cs);

					this.array.SetLineString(1, first+i, name);
					this.array.SetLineState(1, first+i, cs);

					this.array.SetLineString(2, first+i, description);
					this.array.SetLineState(2, first+i, cs);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void UpdateColumnsWidth()
		{
			//	Place les widgets en dessus et en dessous du tableau en fonction des
			//	largeurs des colonnes.
			double w1 = this.array.GetColumnsAbsoluteWidth(0);
			double w2 = this.array.GetColumnsAbsoluteWidth(1);
			double w3 = this.array.GetColumnsAbsoluteWidth(2);

			this.headerUse.PreferredWidth = w1;
			this.headerName.PreferredWidth = w2;
			this.headerCaption.PreferredWidth = w3+1;

			this.footerUse.PreferredWidth = w1-1;
			this.footerName.PreferredWidth = w2-1;
			this.footerCaption.PreferredWidth = w3+1;
		}


		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle rubrique dans la table.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			this.items[sel].Used = true;
			this.UpdateArray();
			this.UpdateButtons();
		}

		protected void ArrayRemove()
		{
			//	Supprime une rubrique de la table.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			this.items[sel].Used = false;
			this.UpdateArray();
			this.UpdateButtons();
		}

		protected void ArrayTemplateAdd()
		{
			//	Ajoute un template dans la table.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Panels, ResourceAccess.TypeType.None, Druid.Empty, null);
			if (druid.IsEmpty)  // annuler ?
			{
				return;
			}

			Item item = new Item(druid);
			this.items.Insert(sel+1, item);
			this.array.SelectedRow = sel+1;

			this.UpdateArray();
			this.UpdateButtons();
		}

		protected void ArrayTemplateRemove()
		{
			//	Supprime un template dans la table.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			if (this.items[sel].IsTemplate)
			{
				this.items.RemoveAt(sel);

				if (sel > this.items.Count-1)
				{
					sel = this.items.Count-1;
				}
				this.array.SelectedRow = sel;

				this.UpdateArray();
				this.UpdateButtons();
			}
		}

		protected void ArrayMove(int direction)
		{
			//	Déplace une rubrique dans la table.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Item item = this.items[sel];
			this.items.RemoveAt(sel);
			this.items.Insert(sel+direction, item);

			this.array.SelectedRow = sel+direction;
			this.array.ShowSelectedRow();

			this.UpdateButtons();
			this.UpdateArray();
		}

		protected void ArraySort()
		{
			//	Met les rubrique de la table en tête de liste.
			List<Item> unused = new List<Item>();

			int i=0;
			while (i<this.items.Count)
			{
				Item item = this.items[i];

				if (item.Used)
				{
					i++;
				}
				else
				{
					unused.Add(item);
					this.items.RemoveAt(i);
				}
			}

			foreach (Item item in unused)
			{
				this.items.Add(item);
			}

			this.array.SelectedRow = -1;
			this.UpdateButtons();
			this.UpdateArray();
		}


		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.buttonAdd)
			{
				this.ArrayAdd();
			}

			if (sender == this.buttonRemove)
			{
				this.ArrayRemove();
			}

			if (sender == this.buttonTemplateAdd)
			{
				this.ArrayTemplateAdd();
			}

			if (sender == this.buttonTemplateRemove)
			{
				this.ArrayTemplateRemove();
			}

			if (sender == this.buttonPrev)
			{
				this.ArrayMove(-1);
			}

			if (sender == this.buttonNext)
			{
				this.ArrayMove(1);
			}

			if (sender == this.buttonSort)
			{
				this.ArraySort();
			}
		}

		private void HandleSliderChanged(object sender)
		{
			//	Appelé lorsque le slider a été déplacé.
			if (this.array == null)
			{
				return;
			}

			HSlider slider = sender as HSlider;
			TableConfiguration.arrayLineHeight = (double) slider.Value;
			this.array.LineHeight = TableConfiguration.arrayLineHeight;
		}

		private void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
			this.UpdateColumnsWidth();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		private void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();

			if (this.array.SelectedColumn == 0)  // clic dans la colonne de gauche ?
			{
				if (this.buttonAdd.Enable)
				{
					this.ArrayAdd();
				}
				else if (this.buttonRemove.Enable)
				{
					this.ArrayRemove();
				}
			}
		}

		private void HandleFooterNameClicked(object sender, MessageEventArgs e)
		{
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.items[sel].Column.TemplateId;
			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Panels, ResourceAccess.TypeType.None, druid, null);
			if (druid.IsEmpty)  // annuler ?
			{
				return;
			}

			this.items[sel].Column.TemplateId = druid;
			this.UpdateArray();
		}

		private void HandleFooterCaptionClicked(object sender, MessageEventArgs e)
		{
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.items[sel].Column.CaptionId;
			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Captions, ResourceAccess.TypeType.None, druid, null);
			if (druid.IsEmpty)  // annuler ?
			{
				return;
			}

			this.items[sel].Column.CaptionId = druid;
			this.UpdateArray();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.columnsReturned = this.SelectedList;
		}


		protected class Item
		{
			public Item(bool used, UI.ItemTableColumn column)
			{
				this.used = used;
				this.column = column;
			}

			public Item(Druid druid)
			{
				this.used = true;
				this.column = new UI.ItemTableColumn();
				this.column.TemplateId = druid;
			}

			public bool Used
			{
				get
				{
					return this.used;
				}
				set
				{
					this.used = value;
				}
			}

			public UI.ItemTableColumn Column
			{
				get
				{
					return this.column;
				}
			}

			public bool IsTemplate
			{
				get
				{
					return !this.column.TemplateId.IsEmpty;
				}
			}

			protected bool						used;
			protected UI.ItemTableColumn		column;
		}


		protected static double					arrayLineHeight = 20;

		protected Module						module;
		protected ResourceAccess				resourceAccess;
		protected List<Item>					items;
		protected List<UI.ItemTableColumn>		columnsReturned;

		protected HToolBar						toolbar;
		protected IconButton					buttonAdd;
		protected IconButton					buttonRemove;
		protected IconButton					buttonTemplateAdd;
		protected IconButton					buttonTemplateRemove;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonSort;
		protected HSlider						slider;

		protected Widget						header;
		protected HeaderButton					headerUse;
		protected HeaderButton					headerName;
		protected HeaderButton					headerCaption;

		protected MyWidgets.StringArray			array;

		protected Widget						footer;
		protected Widget						footerUse;
		protected Button						footerName;
		protected Button						footerCaption;

		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
