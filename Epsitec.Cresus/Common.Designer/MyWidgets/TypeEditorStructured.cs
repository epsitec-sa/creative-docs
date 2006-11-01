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
			this.buttonAdd.CaptionDruid = Res.Captions.Editor.Structured.Add.Druid;
			this.buttonAdd.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonAdd);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonPrev = new IconButton();
			this.buttonPrev.CaptionDruid = Res.Captions.Editor.Structured.Prev.Druid;
			this.buttonPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonPrev);

			this.buttonNext = new IconButton();
			this.buttonNext.CaptionDruid = Res.Captions.Editor.Structured.Next.Druid;
			this.buttonNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonNext);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonRemove = new IconButton();
			this.buttonRemove.CaptionDruid = Res.Captions.Editor.Structured.Remove.Druid;
			this.buttonRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRemove);

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
			this.array.LineHeight = 30;  // plus haut, à cause des descriptions et des icônes
			this.array.Dock = DockStyle.StackBegin;
			this.array.PreferredHeight = 200;
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
			this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);

			//	Crée le pied pour éditer la ligne sélectionnée.
			this.footer = new Widget(this);
			this.footer.Dock = DockStyle.StackBegin;
			this.footer.Margins = new Margins(0, 0, 5, 0);

			this.fieldName = new TextFieldEx(this.footer);
			this.fieldName.Margins = new Margins(1, 1, 0, 0);
			this.fieldName.Dock = DockStyle.Left;
			this.fieldName.ButtonShowCondition = ShowCondition.WhenModified;
			this.fieldName.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldName.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.fieldName.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

			this.buttonType = new Button(this.footer);
			this.buttonType.Text = "Changer le type";
			this.buttonType.Margins = new Margins(1, 1, 0, 0);
			this.buttonType.Dock = DockStyle.Left;
			this.buttonType.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonCaption = new Button(this.footer);
			this.buttonCaption.Text = "Changer la légende";
			this.buttonCaption.Margins = new Margins(1, 1, 0, 0);
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

				this.array.ColumnsWidthChanged -= new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.fieldName.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.fieldName.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
				this.buttonType.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonCaption.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			this.FieldsInput();
			this.UpdateArray();
			this.UpdateButtons();
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

		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle valeur dans la structure.
			int sel = this.array.SelectedRow;
			string name = this.GetNewName();
			StructuredTypeField field = new StructuredTypeField(name, StringType.Default);
			this.fields.Insert(sel+1, field);

			this.FieldsOutput();
			this.UpdateArray();
			this.UpdateButtons();
			this.array.SelectedRow = sel+1;
			this.array.ShowSelectedRow();
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

			this.FieldsOutput();
			this.UpdateArray();
			this.UpdateButtons();
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
			this.UpdateButtons();
			this.array.SelectedRow = sel+direction;
			this.array.ShowSelectedRow();
			this.OnContentChanged();
		}

		protected void ChangeType()
		{
		}

		protected void ChangeCaption()
		{
		}


		protected string GetNewName()
		{
			for (int i=1; i<10000; i++)
			{
				string name = string.Format("Rubrique{0}", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (!this.IsExistingName(name))
				{
					return name;
				}
			}
			return null;
		}

		protected bool IsExistingName(string name)
		{
			foreach (StructuredTypeField field in this.fields)
			{
				if (name == field.Id)
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
			StructuredType type = this.AbstractType as StructuredType;
			type.Fields.Clear();

			foreach (StructuredTypeField field in this.fields)
			{
				type.Fields.Add(field);
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

			this.fieldName.PreferredWidth = w1-1;
			this.buttonType.PreferredWidth = w2-1;
			this.buttonCaption.PreferredWidth = w3-1;
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
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if (this.ignoreChange)
			{
				return;
			}
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la ligne éditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
		}


		protected HToolBar						toolbar;
		protected IconButton					buttonAdd;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonRemove;

		protected MyWidgets.StringArray			array;

		protected Widget						footer;
		protected TextFieldEx					fieldName;
		protected Button						buttonType;
		protected Button						buttonCaption;

		protected int							lastIndex = -1;
		protected List<StructuredTypeField>		fields;
	}
}
