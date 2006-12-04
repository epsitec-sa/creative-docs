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
				this.window.Text = "Choix des rubriques";  // Res.Strings.Dialog.StructuredSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				//	Crée l'en-tête du tableau.
				this.header = new Widget(this.window.Root);
				this.header.Dock = DockStyle.Top;
				this.header.Margins = new Margins(0, 0, 4, 0);

				this.headerName = new HeaderButton(this.header);
				this.headerName.Text = Res.Strings.Viewers.Types.Structured.Name;
				this.headerName.Style = HeaderButtonStyle.Top;
				this.headerName.Dock = DockStyle.Left;

				this.headerCaption = new HeaderButton(this.header);
				this.headerCaption.Text = Res.Strings.Viewers.Types.Structured.Caption;
				this.headerCaption.Style = HeaderButtonStyle.Top;
				this.headerCaption.Dock = DockStyle.Left;

				this.headerType = new HeaderButton(this.header);
				this.headerType.Text = Res.Strings.Viewers.Types.Structured.Type;
				this.headerType.Style = HeaderButtonStyle.Top;
				this.headerType.Dock = DockStyle.Left;

				//	Crée le tableau principal.
				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.Columns = 6;
				this.array.SetColumnsRelativeWidth(0, 0.28);
				this.array.SetColumnsRelativeWidth(1, 0.07);
				this.array.SetColumnsRelativeWidth(2, 0.28);
				this.array.SetColumnsRelativeWidth(3, 0.07);
				this.array.SetColumnsRelativeWidth(4, 0.28);
				this.array.SetColumnsRelativeWidth(5, 0.07);
				this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(1, ContentAlignment.MiddleCenter);
				this.array.SetColumnAlignment(2, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(3, ContentAlignment.MiddleCenter);
				this.array.SetColumnAlignment(4, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(5, ContentAlignment.MiddleCenter);
				this.array.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				this.array.SetColumnBreakMode(2, TextBreakMode.Ellipsis | TextBreakMode.Split);
				this.array.SetColumnBreakMode(4, TextBreakMode.Ellipsis | TextBreakMode.Split);
				this.array.SetDynamicToolTips(0, true);
				this.array.SetDynamicToolTips(1, false);
				this.array.SetDynamicToolTips(2, false);
				this.array.SetDynamicToolTips(3, false);
				this.array.SetDynamicToolTips(4, false);
				this.array.SetDynamicToolTips(5, false);
				this.array.LineHeight = TableConfiguration.arrayLineHeight;
				this.array.Dock = DockStyle.Fill;
				this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);

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

				this.slider = new HSlider(footer);
				this.slider.PreferredWidth = 80;
				this.slider.Dock = DockStyle.Right;
				this.slider.Margins = new Margins(0, 0, 4, 4);
				this.slider.TabIndex = tabIndex++;
				this.slider.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 50.0M;
				this.slider.SmallChange = 5.0M;
				this.slider.LargeChange = 10.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) TableConfiguration.arrayLineHeight;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				//?ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.Icon.Tooltip.Size);
			}

			this.UpdateButtons();
			this.UpdateArray();

			this.window.ShowDialog();
		}

		public void Initialise(Module module, UI.Collections.ItemTableColumnCollection columns)
		{
			//	Initialise le dialogue avec l'objet table.
			this.module = module;
			this.resourceAccess = module.AccessCaptions;

			this.columns = new Epsitec.Common.UI.Collections.ItemTableColumnCollection();
			foreach (UI.ItemTableColumn column in columns)
			{
				this.columns.Add(column);
			}

			this.columnsReturned = null;
		}

		public UI.Collections.ItemTableColumnCollection Columns
		{
			get
			{
				return this.columnsReturned;
			}
		}


		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons en fonction de la ligne sélectionnée dans le tableau.
			int sel = this.array.SelectedRow;
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.columns.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.columns.Count)
				{
					UI.ItemTableColumn column = this.columns[first+i];
					string name = column.FieldId;

					this.array.SetLineString(0, first+i, name);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(3, first+i, "");
					this.array.SetLineState(3, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(4, first+i, "");
					this.array.SetLineState(4, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(5, first+i, "");
					this.array.SetLineState(5, first+i, MyWidgets.StringList.CellState.Normal);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(3, first+i, "");
					this.array.SetLineState(3, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(4, first+i, "");
					this.array.SetLineState(4, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(5, first+i, "");
					this.array.SetLineState(5, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void UpdateColumnsWidth()
		{
			//	Place les widgets en dessus et en dessous du tableau en fonction des
			//	largeurs des colonnes.
			double w1 = this.array.GetColumnsAbsoluteWidth(0) + this.array.GetColumnsAbsoluteWidth(1);
			double w2 = this.array.GetColumnsAbsoluteWidth(2) + this.array.GetColumnsAbsoluteWidth(3);
			double w3 = this.array.GetColumnsAbsoluteWidth(4) + this.array.GetColumnsAbsoluteWidth(5);

			this.headerName.PreferredWidth = w1;
			this.headerCaption.PreferredWidth = w2;
			this.headerType.PreferredWidth = w3+1;
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

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
			}

			this.UpdateButtons();
		}

		private void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a été double cliquée.
			this.UpdateButtons();

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.columnsReturned = this.columns;
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

			this.columnsReturned = this.columns;
		}


		protected static double					arrayLineHeight = 20;

		protected Module						module;
		protected ResourceAccess				resourceAccess;
		protected UI.Collections.ItemTableColumnCollection columns;
		protected UI.Collections.ItemTableColumnCollection columnsReturned;

		protected Widget						header;
		protected HeaderButton					headerName;
		protected HeaderButton					headerCaption;
		protected HeaderButton					headerType;
		protected MyWidgets.StringArray			array;

		protected Button						buttonOk;
		protected Button						buttonCancel;
		protected HSlider						slider;
	}
}
