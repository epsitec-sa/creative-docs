using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir une rubrique (string) d'une ressource de type
	/// 'structure de données' (StructuredType).
	/// </summary>
	public class StructuredSelector : Abstract
	{
		public StructuredSelector(MainWindow mainWindow) : base(mainWindow)
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
				this.WindowInit("StructuredSelector", 500, 300, true);
				this.window.Text = Res.Strings.Dialog.StructuredSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				//	Titre.
				this.title = new StaticText(this.window.Root);
				this.title.PreferredHeight = 30;
				this.title.Dock = DockStyle.Top;
				this.title.Margins = new Margins(0, 0, 0, 5);

				//	Crée l'en-tête du tableau.
				this.header = new Widget(this.window.Root);
				this.header.Dock = DockStyle.Top;
				this.header.Margins = new Margins(0, 0, 4, 0);

				this.headerName = new HeaderButton(this.header);
				this.headerName.Text = Res.Strings.Viewers.Types.Structured.Name;
				this.headerName.Style = HeaderButtonStyle.Top;
				this.headerName.Dock = DockStyle.Left;

				this.headerType = new HeaderButton(this.header);
				this.headerType.Text = Res.Strings.Viewers.Types.Structured.Type;
				this.headerType.Style = HeaderButtonStyle.Top;
				this.headerType.Dock = DockStyle.Left;

				this.headerCaption = new HeaderButton(this.header);
				this.headerCaption.Text = Res.Strings.Viewers.Types.Structured.Caption;
				this.headerCaption.Style = HeaderButtonStyle.Top;
				this.headerCaption.Dock = DockStyle.Left;

				//	Crée le tableau principal.
				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.Columns = 5;
				this.array.SetColumnsRelativeWidth(0, 0.30);
				this.array.SetColumnsRelativeWidth(1, 0.30);
				this.array.SetColumnsRelativeWidth(2, 0.05);
				this.array.SetColumnsRelativeWidth(3, 0.30);
				this.array.SetColumnsRelativeWidth(4, 0.05);
				this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(2, ContentAlignment.MiddleCenter);
				this.array.SetColumnAlignment(3, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(4, ContentAlignment.MiddleCenter);
				this.array.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				this.array.SetColumnBreakMode(1, TextBreakMode.Ellipsis | TextBreakMode.Split);
				this.array.SetColumnBreakMode(3, TextBreakMode.Ellipsis | TextBreakMode.Split);
				this.array.SetDynamicToolTips(0, true);
				this.array.SetDynamicToolTips(1, false);
				this.array.SetDynamicToolTips(2, false);
				this.array.SetDynamicToolTips(3, false);
				this.array.SetDynamicToolTips(4, false);
				this.array.LineHeight = StructuredSelector.arrayLineHeight;
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

				this.buttonUse = new Button(footer);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.StructuredSelector.Button.Use;
				this.buttonUse.Dock = DockStyle.Left;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = tabIndex++;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;

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
				this.slider.Value = (decimal) StructuredSelector.arrayLineHeight;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				//?ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.Icon.Tooltip.Size);
			}

			this.UpdateTitle();
			this.UpdateButtons();
			this.UpdateArray();
			this.SelectArray();

			this.window.ShowDialog();
		}

		public void Initialise(ResourceAccess access, Module module, StructuredType type, string field)
		{
			this.resourceAccess = access;
			this.module = module;
			this.structuredType = type;
			this.initialField = field;
			this.selectedField = null;

			this.FieldsInput();
		}

		public string SelectedField
		{
			get
			{
				return this.selectedField;
			}
		}

		protected void FieldsInput()
		{
			//	Lit le dictionnaire des rubriques (StructuredTypeField) dans la liste
			//	interne (this.fields).
			StructuredType type = this.structuredType;
			this.fields = new List<StructuredTypeField>();

			foreach (string id in type.GetFieldIds())
			{
				StructuredTypeField field = type.Fields[id];
				this.fields.Add(field);
			}
		}

		protected void SelectArray()
		{
			//	Sélectionne la bonne ligne dans le tableau.
			for (int i=0; i<this.fields.Count; i++)
			{
				if (this.fields[i].Id == this.initialField)
				{
					this.array.SelectedRow = i;
					return;
				}
			}

			this.array.SelectedRow = -1;
		}


		protected void UpdateTitle()
		{
			//	Met à jour le titre qui donne le nom de la ressource StructuredType.
			string text = string.Concat("<font size=\"200%\"><b>", this.structuredType.Caption.Name, "</b></font>");
			this.title.Text = text;
		}

		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons en fonction de la ligne sélectionnée dans le tableau.
			int sel = this.array.SelectedRow;

			this.buttonUse.Enable = (sel != -1);
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.fields.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.fields.Count)
				{
					StructuredTypeField field = this.fields[first+i];
					string name = field.Id;

					string captionType = "";
					string iconType = "";
					AbstractType type = field.Type as AbstractType;
					if (type != null)
					{
						Caption caption = this.module.ResourceManager.GetCaption(type.Caption.Id);

						if (this.array.LineHeight >= 30)  // assez de place pour 2 lignes ?
						{
							string nd = ResourceAccess.GetCaptionNiceDescription(caption, 0);  // texte sur 1 ligne
							captionType = string.Concat(caption.Name, ":<br/>", nd);
						}
						else
						{
							captionType = caption.Name;
						}

						iconType = this.resourceAccess.DirectGetIcon(caption.Id);
						if (!string.IsNullOrEmpty(iconType))
						{
							iconType = Misc.ImageFull(iconType);
						}
					}

					string captionText = "";
					string iconText = "";
					Druid druid = field.CaptionId;
					if (druid.IsValid)
					{
						Caption caption = this.module.ResourceManager.GetCaption(druid);

						if (this.array.LineHeight >= 30)  // assez de place pour 2 lignes ?
						{
							string nd = ResourceAccess.GetCaptionNiceDescription(caption, 0);  // texte sur 1 ligne
							captionText = string.Concat(caption.Name, ":<br/>", nd);
						}
						else
						{
							captionText = caption.Name;
						}

						if (!string.IsNullOrEmpty(caption.Icon))
						{
							iconText = Misc.ImageFull(caption.Icon);
						}
					}

					this.array.SetLineString(0, first+i, name);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(1, first+i, captionType);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(2, first+i, iconType);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(3, first+i, captionText);
					this.array.SetLineState(3, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(4, first+i, iconText);
					this.array.SetLineState(4, first+i, MyWidgets.StringList.CellState.Normal);
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
				}
			}
		}

		protected void UpdateColumnsWidth()
		{
			//	Place les widgets en dessus et en dessous du tableau en fonction des
			//	largeurs des colonnes.
			double w1 = this.array.GetColumnsAbsoluteWidth(0);
			double w2 = this.array.GetColumnsAbsoluteWidth(1) + this.array.GetColumnsAbsoluteWidth(2);
			double w3 = this.array.GetColumnsAbsoluteWidth(3) + this.array.GetColumnsAbsoluteWidth(4);

			this.headerName.PreferredWidth = w1;
			this.headerType.PreferredWidth = w2;
			this.headerCaption.PreferredWidth = w3+1;
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

		private void HandleButtonUseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
				this.selectedField = this.fields[sel].Id;
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
			StructuredSelector.arrayLineHeight = (double) slider.Value;
			this.array.LineHeight = StructuredSelector.arrayLineHeight;
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
			this.UpdateButtons();
		}

		private void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a été double cliquée.
			this.UpdateButtons();

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
				this.selectedField = this.fields[sel].Id;
			}
		}


		protected static double					arrayLineHeight = 20;

		protected ResourceAccess				resourceAccess;
		protected Module						module;
		protected StructuredType				structuredType;
		protected List<StructuredTypeField>		fields;
		protected string						initialField;
		protected string						selectedField;

		protected StaticText					title;
		protected Widget						header;
		protected HeaderButton					headerName;
		protected HeaderButton					headerType;
		protected HeaderButton					headerCaption;
		protected MyWidgets.StringArray			array;

		protected Button						buttonUse;
		protected Button						buttonCancel;
		protected HSlider						slider;
	}
}
