using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir une rubrique (string) d'une ressource de type
	/// 'structure de donn�es' (StructuredType).
	/// </summary>
	public class BindingSelector : Abstract
	{
		public BindingSelector(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("BindingSelector", 500, 400, true);
				this.window.Text = Res.Strings.Dialog.BindingSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Titre.
				this.title = new StaticText(this.window.Root);
				this.title.PreferredHeight = 30;
				this.title.Dock = DockStyle.Top;
				this.title.Margins = new Margins(0, 0, 0, 5);

				//	Cr�e l'en-t�te du tableau.
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

				//	Cr�e le tableau principal.
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
				this.array.LineHeight = BindingSelector.arrayLineHeight;
				this.array.Dock = DockStyle.Fill;
				this.array.Margins = new Margins(0, 0, 0, 4);
				this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonUse = new Button(footer);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.BindingSelector.Button.Use;
				this.buttonUse.Dock = DockStyle.Right;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = 10;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.slider = new HSlider(footer);
				this.slider.PreferredWidth = 80;
				this.slider.Dock = DockStyle.Left;
				this.slider.Margins = new Margins(0, 0, 4, 4);
				this.slider.TabIndex = 1;
				this.slider.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 50.0M;
				this.slider.SmallChange = 5.0M;
				this.slider.LargeChange = 10.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) BindingSelector.arrayLineHeight;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				//?ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.Icon.Tooltip.Size);

				//	Cr�e le bouton pour le mode.
				this.checkReadonly = new CheckButton(this.window.Root);
				this.checkReadonly.Text = Res.Strings.Dialog.BindingSelector.Button.Readonly;
				this.checkReadonly.Dock = DockStyle.Bottom;
				this.checkReadonly.Margins = new Margins(0, 0, 2, 4);
				this.checkReadonly.ActiveStateChanged += new EventHandler(this.HandleCheckReadonlyActiveStateChanged);

				//	Cr�e le bouton pour l'h�ritage.
				this.checkInherit = new CheckButton(this.window.Root);
				this.checkInherit.Text = Res.Strings.Dialog.BindingSelector.Button.Inherit;
				this.checkInherit.Dock = DockStyle.Bottom;
				this.checkInherit.Margins = new Margins(0, 0, 2, 4);
				this.checkInherit.ActiveStateChanged += new EventHandler(this.HandleCheckInheritActiveStateChanged);
			}

			this.UpdateTitle();
			this.UpdateButtons();
			this.UpdateArray();
			this.SelectArray();
			this.UpdateMode();

			this.window.ShowDialog();
		}

		public void Initialise(Module module, StructuredType type, ObjectModifier.ObjectType objectType, Binding binding)
		{
			//	Initialise le dialogue avec le binding actuel.
			this.module = module;
			this.structuredType = type;
			this.objectType = objectType;
			this.initialBinding = binding;

			this.selectedBinding = null;
			this.isOk = false;

			this.FieldsInput();
		}

		public Binding SelectedBinding
		{
			//	Retourne le binding s�lectionn� par l'utilisateur, ou null si le bouton
			//	'h�rit�' a �t� coch�.
			get
			{
				return this.selectedBinding;
			}
		}

		public bool IsOk
		{
			get
			{
				return this.isOk;
			}
		}

		protected string Field
		{
			//	Rubrique s�lectionn�e.
			//	TODO: il faudra am�liorer la gestion du pr�fixe "*." !
			get
			{
				if (this.initialBinding == null)
				{
					return null;
				}
				else
				{
					string field = this.initialBinding.Path;
					if (field.StartsWith("*."))
					{
						field = field.Substring(2);  // enl�ve "*."
					}

					return field;
				}
			}
			set
			{
				string field = string.Concat("*.", value);  // ajoute "*."

				if (this.initialBinding == null)
				{
					this.initialBinding = new Binding(BindingMode.TwoWay, field);
				}
				else
				{
					this.initialBinding = new Binding(this.initialBinding.Mode, field);
				}
			}
		}

		protected BindingMode Mode
		{
			//	Mode s�lectionn�.
			get
			{
				if (this.initialBinding == null)
				{
					return BindingMode.None;
				}
				else
				{
					return this.initialBinding.Mode;
				}
			}
			set
			{
				if (this.initialBinding == null)
				{
					this.initialBinding = new Binding(value, "");
				}
				else
				{
					this.initialBinding = new Binding(value, this.initialBinding.Path);
				}
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

				if (this.objectType == ObjectModifier.ObjectType.SubPanel &&
					!(field.Type is StructuredType))
				{
					continue;
				}

				if (this.objectType == ObjectModifier.ObjectType.Placeholder &&
					(field.Type is StructuredType))
				{
					continue;
				}

				if (this.objectType == ObjectModifier.ObjectType.Table &&
					field.Relation != FieldRelation.Collection)
				{
					continue;
				}

				this.fields.Add(field);
			}
		}

		protected void SelectArray()
		{
			//	S�lectionne la bonne ligne dans le tableau.
			string field = this.Field;

			for (int i=0; i<this.fields.Count; i++)
			{
				if (this.fields[i].Id == field)
				{
					this.array.SelectedRow = i;
					return;
				}
			}

			this.array.SelectedRow = -1;
		}


		protected void UpdateTitle()
		{
			//	Met � jour le titre qui donne le nom de la ressource StructuredType.
			string text = string.Concat("<font size=\"200%\"><b>", this.structuredType.Caption.Name, "</b></font>");
			this.title.Text = text;
		}

		protected void UpdateButtons()
		{
			//	Met � jour tous les boutons en fonction de la ligne s�lectionn�e dans le tableau.
			int sel = this.array.SelectedRow;

			this.buttonUse.Enable = (sel != -1 || this.initialBinding == null);
		}

		protected void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			this.array.TotalRows = this.fields.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.fields.Count)
				{
					StructuredTypeField field = this.fields[first+i];
					string name = field.Id;
					string iconRelation = "";

					string captionType = "";
					string iconType = "";
					AbstractType type = field.Type as AbstractType;
					if (type != null)
					{
						if (type is StructuredType)
						{
							if (field.Relation == FieldRelation.Reference )  iconRelation = Misc.Image("RelationReference");
//-							if (field.Relation == Relation.Bijective )  iconRelation = Misc.Image("RelationBijective");
							if (field.Relation == FieldRelation.Collection)  iconRelation = Misc.Image("RelationCollection");
						}

						Caption caption = this.module.ResourceManager.GetCaption(type.Caption.Id);

						if (this.array.LineHeight >= 30)  // assez de place pour 2 lignes ?
						{
							string nd = ResourceAccess.GetCaptionNiceDescription(caption, 0);  // texte sur 1 ligne
							captionType = string.Concat(caption.Name, ":<br/>", nd);
						}
						else
						{
							captionType = ResourceAccess.GetCaptionNiceDescription(caption, 0);  // texte sur 1 ligne
						}

#if false
						iconType = this.resourceAccess.DirectGetIcon(caption.Id);
						if (!string.IsNullOrEmpty(iconType))
						{
							iconType = Misc.ImageFull(iconType);
						}
#endif
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
							captionText = ResourceAccess.GetCaptionNiceDescription(caption, 0);  // texte sur 1 ligne
						}

						if (!string.IsNullOrEmpty(caption.Icon))
						{
							iconText = Misc.ImageFull(caption.Icon);
						}
					}

					this.array.SetLineString(0, first+i, name);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(1, first+i, iconRelation);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(2, first+i, captionText);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(3, first+i, iconText);
					this.array.SetLineState(3, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(4, first+i, captionType);
					this.array.SetLineState(4, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(5, first+i, iconType);
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

		protected void UpdateMode()
		{
			//	Met � jour le bouton pour le mode.
			this.ignoreChanged = true;

			this.checkInherit.Visibility = (this.objectType == ObjectModifier.ObjectType.SubPanel);
			this.checkInherit.ActiveState = (this.initialBinding == null) ? ActiveState.Yes : ActiveState.No;

			this.checkReadonly.ActiveState = (this.initialBinding != null && this.Mode == BindingMode.OneWay) ? ActiveState.Yes : ActiveState.No;
			this.checkReadonly.Enable = (this.initialBinding != null);

			if (this.initialBinding == null)
			{
				this.array.SelectedRow = -1;
			}
			
			this.ignoreChanged = false;
		}


		private void HandleSliderChanged(object sender)
		{
			//	Appel� lorsque le slider a �t� d�plac�.
			if (this.array == null)
			{
				return;
			}

			HSlider slider = sender as HSlider;
			BindingSelector.arrayLineHeight = (double) slider.Value;
			this.array.LineHeight = BindingSelector.arrayLineHeight;
		}

		private void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a chang�.
			this.UpdateColumnsWidth();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a chang�.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne s�lectionn�e a chang�.
			if (this.ignoreChanged)
			{
				return;
			}

			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
				this.Field = this.fields[sel].Id;
			}

			this.UpdateButtons();
			this.UpdateMode();
		}

		private void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne s�lectionn�e a �t� double cliqu�e.
			this.UpdateButtons();
			this.UpdateMode();

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.selectedBinding = this.initialBinding;
			this.isOk = true;
		}

		private void HandleCheckInheritActiveStateChanged(object sender)
		{
			if (this.ignoreChanged)
			{
				return;
			}

			if (this.initialBinding == null)
			{
				this.initialBinding = new Binding(BindingMode.TwoWay, "");
			}
			else
			{
				this.initialBinding = null;
			}

			this.UpdateMode();
			this.UpdateButtons();
		}

		private void HandleCheckReadonlyActiveStateChanged(object sender)
		{
			//	Bouton "Lecture seule" actionn�.
			if (this.ignoreChanged)
			{
				return;
			}

			if (this.Mode == BindingMode.TwoWay)
			{
				this.Mode = BindingMode.OneWay;
			}
			else
			{
				this.Mode = BindingMode.TwoWay;
			}

			this.UpdateMode();
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

			this.selectedBinding = this.initialBinding;
			this.isOk = true;
		}


		protected static double					arrayLineHeight = 20;

		protected Module						module;
		protected StructuredType				structuredType;
		protected ObjectModifier.ObjectType		objectType;
		protected List<StructuredTypeField>		fields;
		protected Binding						initialBinding;
		protected Binding						selectedBinding;
		protected bool							isOk;

		protected StaticText					title;
		protected Widget						header;
		protected HeaderButton					headerName;
		protected HeaderButton					headerCaption;
		protected HeaderButton					headerType;
		protected MyWidgets.StringArray			array;
		protected CheckButton					checkInherit;
		protected CheckButton					checkReadonly;

		protected Button						buttonUse;
		protected Button						buttonCancel;
		protected HSlider						slider;
	}
}
