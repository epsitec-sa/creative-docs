using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
	/// </summary>
	public class TypeEditorStructured : AbstractTypeEditor
	{
		public TypeEditorStructured()
		{
			//	Cr�e la toolbar.
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

			//	Cr�e le tableau principal.
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
			this.array.LineHeight = 30;  // plus haut, � cause des descriptions et des ic�nes
			this.array.Dock = DockStyle.StackBegin;
			this.array.PreferredHeight = 200;
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
			this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);

			//	Cr�e le pied pour �diter la ligne s�lectionn�e.
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
			this.buttonType.Text = "Choisir un type";
			this.buttonType.Margins = new Margins(1, 1, 0, 0);
			this.buttonType.Dock = DockStyle.Left;
			this.buttonType.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonCaption = new Button(this.footer);
			this.buttonCaption.Text = "Choisir une l�gende";
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
			//	Met � jour le contenu de l'�diteur.
			this.ignoreChange = true;
			this.AccessInitialise();
			this.UpdateArray();
			this.UpdateButtons();
			this.ignoreChange = false;
		}

		protected void UpdateButtons()
		{
			int sel = this.array.SelectedRow;

			this.buttonPrev.Enable = (sel != -1 && sel > 0);
			this.buttonNext.Enable = (sel != -1 && sel < this.AccessCount-1);
			this.buttonRemove.Enable = (sel != -1);

			this.fieldName.Enable = (sel != -1);
			this.buttonType.Enable = (sel != -1);
			this.buttonCaption.Enable = (sel != -1);
		}

		protected void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			this.array.TotalRows = this.AccessCount;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.AccessCount)
				{
					StructuredTypeField field = this.AccessGet(first+i);
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
			StructuredType type = this.AbstractType as StructuredType;
			type.Fields.Add("Vide", StringType.Default);

			this.AccessInitialise();
			this.UpdateArray();
			this.UpdateButtons();
			this.OnContentChanged();
		}

		protected void ArrayRemove()
		{
			//	Supprime une valeur de la structure.
			StructuredType type = this.AbstractType as StructuredType;
		}

		protected void ArrayMove(int direction)
		{
			//	D�place une valeur dans la structure.
			StructuredType type = this.AbstractType as StructuredType;
		}

		protected void RenumCollection()
		{
			//	Renum�rote toute la structure.
			StructuredType type = this.AbstractType as StructuredType;
		}

		protected void ChangeType()
		{
			StructuredType type = this.AbstractType as StructuredType;
		}

		protected void ChangeCaption()
		{
			StructuredType type = this.AbstractType as StructuredType;
		}


		protected void AccessInitialise()
		{
			StructuredType type = this.AbstractType as StructuredType;
			System.Diagnostics.Debug.Assert(type != null);

			this.ids = new List<string>();
			foreach(string key in type.GetFieldIds())
			{
				this.ids.Add(key);
			}
		}

		protected int AccessCount
		{
			get
			{
				return this.ids.Count;
			}
		}

		protected StructuredTypeField AccessGet(int index)
		{
			if (index >= 0 && index < this.ids.Count)
			{
				string key = this.ids[index];
				StructuredType type = this.AbstractType as StructuredType;
				return type.Fields[key];
			}
			else
			{
				return StructuredTypeField.Empty;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
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
			//	La largeur des colonnes a chang�.
			this.UpdateClientGeometry();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a chang�.
			this.AccessInitialise();
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne s�lectionn�e a chang�.
			this.UpdateButtons();
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte �ditable a chang�.
			if (this.ignoreChange)
			{
				return;
			}
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appel� lorsque la ligne �ditable pour le label voit son focus changer.
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

		protected List<string>					ids;
	}
}
