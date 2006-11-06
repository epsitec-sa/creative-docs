using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorStructured : AbstractTypeEditor
	{
		public TypeEditorStructured()
		{
			//	Crée la toolbar.
			this.toolbar = new HToolBar(this);
			this.toolbar.Dock = DockStyle.StackBegin;

			this.buttonAdd = new IconButton();
			this.buttonAdd.CaptionId = Res.Captions.Editor.Structured.Add.Id;
			this.buttonAdd.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonAdd);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonPrev = new IconButton();
			this.buttonPrev.CaptionId = Res.Captions.Editor.Structured.Prev.Id;
			this.buttonPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonPrev);

			this.buttonNext = new IconButton();
			this.buttonNext.CaptionId = Res.Captions.Editor.Structured.Next.Id;
			this.buttonNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonNext);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonRemove = new IconButton();
			this.buttonRemove.CaptionId = Res.Captions.Editor.Structured.Remove.Id;
			this.buttonRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRemove);

			this.slider = new HSlider(toolbar);
			this.slider.PreferredWidth = 80;
			this.slider.Margins = new Margins(2, 2, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 50.0M;
			this.slider.SmallChange = 5.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			this.slider.Value = (decimal) TypeEditorStructured.arrayLineHeight;
			this.slider.Dock = DockStyle.Right;

			//	Crée l'en-tête du tableau.
			this.header = new Widget(this);
			this.header.Dock = DockStyle.StackBegin;
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
			this.array = new StringArray(this);
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
			this.array.LineHeight = TypeEditorStructured.arrayLineHeight;
			this.array.Dock = DockStyle.StackBegin;
			this.array.PreferredHeight = 200;
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
			this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);

			//	Crée le pied pour éditer la ligne sélectionnée.
			this.footer = new Widget(this);
			this.footer.Dock = DockStyle.StackBegin;
			this.footer.Margins = new Margins(0, 0, 5, 0);

			this.fieldName = new TextFieldEx(this.footer);
			this.fieldName.Margins = new Margins(1, 0, 0, 0);
			this.fieldName.Dock = DockStyle.Left;
			this.fieldName.ButtonShowCondition = ShowCondition.WhenModified;
			this.fieldName.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldName.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.fieldName.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

			this.buttonType = new Button(this.footer);
			this.buttonType.CaptionId = Res.Captions.Editor.Structured.ChangeType.Id;
			this.buttonType.Margins = new Margins(1, 0, 0, 0);
			this.buttonType.Dock = DockStyle.Left;
			this.buttonType.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonCaption = new Button(this.footer);
			this.buttonCaption.CaptionId = Res.Captions.Editor.Structured.ChangeCaption.Id;
			this.buttonCaption.Margins = new Margins(1, 0, 0, 0);
			this.buttonCaption.Dock = DockStyle.Left;
			this.buttonCaption.Clicked += new MessageEventHandler(this.HandleButtonClicked);
		}

		public TypeEditorStructured(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.buttonAdd.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonPrev.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonNext.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonRemove.Clicked -= new MessageEventHandler(this.HandleButtonClicked);

				this.slider.ValueChanged -= new EventHandler(this.HandleSliderChanged);

				this.array.ColumnsWidthChanged -= new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked -= new EventHandler(this.HandleArraySelectedRowDoubleClicked);

				this.fieldName.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.fieldName.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
				this.buttonType.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonCaption.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			builder.Append(this.fields.Count.ToString());
			builder.Append("×: ");

			for (int i=0; i<this.fields.Count; i++)
			{
				StructuredTypeField field = this.fields[i];
				builder.Append(field.Id);

				if (i < this.fields.Count-1)
				{
					builder.Append(", ");
				}
			}
			
			return builder.ToString();
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			this.FieldsInput();
			this.UpdateArray();
			this.UpdateButtons();
			this.UpdateEdit();
			this.ignoreChange = false;
		}

		protected void UpdateButtons()
		{
			int sel = this.array.SelectedRow;

			this.buttonPrev.Enable = (sel != -1 && sel > 0);
			this.buttonNext.Enable = (sel != -1 && sel < this.fields.Count-1);
			this.buttonRemove.Enable = (sel != -1);

			this.fieldName.Enable = (sel != -1);
			this.buttonType.Enable = (sel != -1);
			this.buttonCaption.Enable = (sel != -1);
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
						//?ResourceBundle bundle = ResourceManager.GetSourceBundle(caption);
						//?ResourceBundle.Field rf = bundle[caption.Druid];
						//?string dn = ResourceAccess.SubAllFilter(rf.Name);

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
						//?ResourceBundle bundle = ResourceManager.GetSourceBundle(caption);
						//?ResourceBundle.Field rf = bundle[druid];
						//?string dn = ResourceAccess.SubAllFilter(rf.Name);

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

		protected void UpdateEdit()
		{
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				this.fieldName.Text = "";
			}
			else
			{
				StructuredTypeField field = this.fields[sel];
				this.fieldName.Text = field.Id;
			}
		}


		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle valeur dans la structure.
			int sel = this.array.SelectedRow;
			
			AbstractType type = null;
			if (sel != -1)
			{
				StructuredTypeField actualField = this.fields[sel];
				type = actualField.Type as AbstractType;
			}

			string name = this.GetNewName();
			StructuredTypeField newField = new StructuredTypeField(name, type, Druid.Empty, 0);
			this.fields.Insert(sel+1, newField);

			this.FieldsOutput();
			this.UpdateArray();

			this.array.SelectedRow = sel+1;
			this.array.ShowSelectedRow();

			this.UpdateButtons();
			this.UpdateEdit();

			this.fieldName.SelectAll();
			this.fieldName.Focus();
			
			this.OnContentChanged();
		}

		protected void ArrayRemove()
		{
			//	Supprime une valeur de la structure.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			this.fields.RemoveAt(sel);

			if (sel > this.fields.Count-1)
			{
				sel = this.fields.Count-1;
			}

			this.FieldsOutput();
			this.UpdateArray();

			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();

			this.UpdateButtons();
			this.UpdateEdit();
			this.OnContentChanged();
		}

		protected void ArrayMove(int direction)
		{
			//	Déplace une valeur dans la structure.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField field = this.fields[sel];
			this.fields.RemoveAt(sel);
			this.fields.Insert(sel+direction, field);

			this.FieldsOutput();
			this.UpdateArray();

			this.array.SelectedRow = sel+direction;
			this.array.ShowSelectedRow();

			this.UpdateButtons();
			this.UpdateEdit();
			this.OnContentChanged();
		}

		protected void ChangeType()
		{
			//	Choix du type.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];
			AbstractType type = actualField.Type as AbstractType;
			Druid druid = (type == null) ? Druid.Empty : type.Caption.Id;

			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Types, druid, null);
			if (druid.IsEmpty)
			{
				return;
			}

			type = TypeRosetta.GetTypeObject(this.module.ResourceManager.GetCaption(druid));
			System.Diagnostics.Debug.Assert(type != null);
			StructuredTypeField newField = new StructuredTypeField(actualField.Id, type, actualField.CaptionId, actualField.Rank);
			this.fields[sel] = newField;

			this.FieldsOutput();
			this.UpdateArray();
			this.OnContentChanged();
		}

		protected void ChangeCaption()
		{
			//	Choix du caption.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];
			Druid druid = actualField.CaptionId;

			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Captions, druid, null);
			if (druid.IsEmpty)
			{
				return;
			}

			StructuredTypeField newField = new StructuredTypeField(actualField.Id, actualField.Type, druid, actualField.Rank);
			this.fields[sel] = newField;

			this.FieldsOutput();
			this.UpdateArray();
			this.OnContentChanged();
		}


		protected string GetNewName()
		{
			//	Cherche un nouveau nom jamais utilisé.
			for (int i=1; i<10000; i++)
			{
				string name = string.Format(Res.Strings.Viewers.Types.Structured.NewName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (!this.IsExistingName(name, -1))
				{
					return name;
				}
			}
			return null;
		}

		protected bool IsExistingName(string name, int exclude)
		{
			//	Indique si un nom existe.
			for (int i=0; i<this.fields.Count; i++)
			{
				StructuredTypeField field = this.fields[i];

				if (i != exclude && name == field.Id)
				{
					return true;
				}
			}

			return false;
		}


		protected void FieldsInput()
		{
			//	Lit le dictionnaire des rubriques (StructuredTypeField) dans la liste
			//	interne (this.fields).
			if (this.lastIndex != this.resourceSelected)  // pas encore lu ?
			{
				this.lastIndex = this.resourceSelected;

				StructuredType type = this.AbstractType as StructuredType;
				this.fields = new List<StructuredTypeField>();

				foreach (string id in type.GetFieldIds())
				{
					StructuredTypeField field = type.Fields[id];
					this.fields.Add(field);
				}
			}
		}

		protected void FieldsOutput()
		{
			//	Le dictionnaire des rubriques (StructuredTypeField) est régénéré à partir
			//	de la liste interne (this.fields). 
			StructuredType type = this.AbstractType as StructuredType;
			type.Fields.Clear();

			for (int i=0; i<this.fields.Count; i++)
			{
				StructuredTypeField actualField = this.fields[i];
				StructuredTypeField newField = new StructuredTypeField(actualField.Id, actualField.Type, actualField.CaptionId, i);

				type.Fields.Add(newField);
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if (this.footer == null)
			{
				return;
			}

			double w1 = this.array.GetColumnsAbsoluteWidth(0);
			double w2 = this.array.GetColumnsAbsoluteWidth(1) + this.array.GetColumnsAbsoluteWidth(2);
			double w3 = this.array.GetColumnsAbsoluteWidth(3) + this.array.GetColumnsAbsoluteWidth(4);

			this.headerName.PreferredWidth = w1;
			this.headerType.PreferredWidth = w2;
			this.headerCaption.PreferredWidth = w3+1;

			this.fieldName.PreferredWidth = w1-1;
			this.buttonType.PreferredWidth = w2-1;
			this.buttonCaption.PreferredWidth = w3+1;
		}


		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.buttonAdd)
			{
				this.ArrayAdd();
			}

			if (sender == this.buttonPrev)
			{
				this.ArrayMove(-1);
			}

			if (sender == this.buttonNext)
			{
				this.ArrayMove(1);
			}

			if (sender == this.buttonRemove)
			{
				this.ArrayRemove();
			}

			if (sender == this.buttonType)
			{
				this.ChangeType();
			}

			if (sender == this.buttonCaption)
			{
				this.ChangeCaption();
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
			TypeEditorStructured.arrayLineHeight = (double) slider.Value;
			this.array.LineHeight = TypeEditorStructured.arrayLineHeight;
		}

		private void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
			this.UpdateClientGeometry();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.FieldsInput();
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();
			this.UpdateEdit();

			if (this.array.SelectedColumn == 0)
			{
				this.fieldName.SelectAll();
				this.fieldName.Focus();
			}
		}

		private void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	Une ligne a été double-cliquée.
			int column = this.array.SelectedColumn;

			if (column == 1 || column == 2)
			{
				this.ChangeType();
			}

			if (column == 3 || column == 4)
			{
				this.ChangeCaption();
			}
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if (this.ignoreChange)
			{
				return;
			}

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualfield = this.fields[sel];
			string name = this.fieldName.Text;

			if (!Misc.IsValidLabel(ref name))
			{
				this.mainWindow.DialogError(Res.Strings.Error.Name.Invalid);

				this.ignoreChange = true;
				this.fieldName.Text = actualfield.Id;
				this.fieldName.SelectAll();
				this.ignoreChange = false;

				return;
			}

			if (this.IsExistingName(name, sel))
			{
				this.mainWindow.DialogError(Res.Strings.Error.Name.AlreadyExist);

				this.ignoreChange = true;
				this.fieldName.Text = actualfield.Id;
				this.fieldName.SelectAll();
				this.ignoreChange = false;

				return;
			}

			StructuredTypeField newField = new StructuredTypeField(name, actualfield.Type, actualfield.CaptionId, actualfield.Rank);
			this.fields[sel] = newField;

			this.FieldsOutput();
			this.UpdateArray();
			this.OnContentChanged();

			this.ignoreChange = true;
			this.fieldName.Text = name;
			this.fieldName.SelectAll();
			this.ignoreChange = false;
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la ligne éditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
		}


		protected static double					arrayLineHeight = 20;

		protected HToolBar						toolbar;
		protected IconButton					buttonAdd;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonRemove;
		protected HSlider						slider;

		protected Widget						header;
		protected HeaderButton					headerName;
		protected HeaderButton					headerType;
		protected HeaderButton					headerCaption;
		protected MyWidgets.StringArray			array;

		protected Widget						footer;
		protected TextFieldEx					fieldName;
		protected Button						buttonType;
		protected Button						buttonCaption;

		protected int							lastIndex = -1;
		protected List<StructuredTypeField>		fields;
	}
}
