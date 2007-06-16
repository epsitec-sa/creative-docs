using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de repr�senter les ressources d'un module.
	/// </summary>
	public class Strings : Abstract
	{
		public Strings(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.primaryCulture = new IconButtonMark(this);
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = ButtonMarkDisposition.Below;
			this.primaryCulture.MarkDimension = 5;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;

			this.array = new MyWidgets.StringArray(this);
			this.array.Columns = 3;
			this.array.SetColumnsRelativeWidth(0, 0.30);
			this.array.SetColumnsRelativeWidth(1, 0.35);
			this.array.SetColumnsRelativeWidth(2, 0.35);
			this.array.SetColumnsAbsoluteWidth(0, Abstract.leftArrayWidth-20);
			this.array.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.array.SetDynamicToolTips(0, true);
			this.array.SetDynamicToolTips(1, false);
			this.array.SetDynamicToolTips(2, false);
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
			this.array.CellCountChanged += new EventHandler (this.HandleArrayCellCountChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);

			this.labelStatic = new StaticText(this);
			this.labelStatic.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelStatic.Text = Res.Strings.Viewers.Strings.Edit;
			this.labelStatic.Visibility = (this.module.Mode != DesignerMode.Build);

			this.labelEdit = new MyWidgets.TextFieldExName(this);
			this.labelEdit.ButtonShowCondition = ShowCondition.WhenModified;
			this.labelEdit.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			this.labelEdit.TabIndex = this.tabIndex++;
			this.labelEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);
			this.currentTextField = this.labelEdit;

			this.primaryEdit = new TextFieldMulti(this);
			this.primaryEdit.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryEdit.TabIndex = this.tabIndex++;
			this.primaryEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryEdit = new TextFieldMulti(this);
			this.secondaryEdit.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryEdit.TabIndex = this.tabIndex++;
			this.secondaryEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.labelAbout = new StaticText(this);
			this.labelAbout.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelAbout.Text = Res.Strings.Viewers.Strings.About;

			this.primaryAbout = new TextFieldMulti(this);
			this.primaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryAbout.TabIndex = this.tabIndex++;
			this.primaryAbout.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryAbout = new TextFieldMulti(this);
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryAbout.TabIndex = this.tabIndex++;
			this.secondaryAbout.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.UpdateCultures();
			this.UpdateEdit();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.array.ColumnsWidthChanged -= new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged -= new EventHandler (this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

				this.primaryEdit.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryEdit.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryAbout.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryAbout.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryAbout.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryAbout.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Strings;
			}
		}

		
		public override void Update()
		{
			//	Met � jour le contenu du Viewer.
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected override void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			//	Version sp�ciale pour les trois colonnes de Strings.
			this.array.TotalRows = this.access.AccessCount;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.access.AccessCount)
				{
					this.UpdateArrayField(0, first+i, null, ResourceAccess.FieldType.Name);
					this.UpdateArrayField(1, first+i, null, ResourceAccess.FieldType.String);
					this.UpdateArrayField(2, first+i, this.secondaryCulture, (this.secondaryCulture == null) ? ResourceAccess.FieldType.None : ResourceAccess.FieldType.String);
				}
				else
				{
					this.UpdateArrayField(0, first+i, null, ResourceAccess.FieldType.None);
					this.UpdateArrayField(1, first+i, null, ResourceAccess.FieldType.None);
					this.UpdateArrayField(2, first+i, null, ResourceAccess.FieldType.None);
				}
			}

			this.array.SelectedRow = this.access.AccessIndex;
		}

		protected override void SelectEdit(bool secondary)
		{
			AbstractTextField edit = secondary ? this.secondaryEdit : this.primaryEdit;

			this.Window.MakeActive();
			edit.Focus();
			edit.SelectAll();
		}

		protected override void UpdateEdit()
		{
			//	Met � jour les lignes �ditables en fonction de la s�lection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.access.AccessIndex;
			int column = this.array.SelectedColumn;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
				column = -1;
			}

			if (sel == -1)
			{
				this.SetTextField(this.labelEdit, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.primaryEdit, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.primaryAbout, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.secondaryEdit, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.secondaryAbout, 0, null, ResourceAccess.FieldType.None);
			}
			else
			{
				int index = this.access.AccessIndex;

				this.SetTextField(this.labelEdit, index, null, ResourceAccess.FieldType.Name);
				this.SetTextField(this.primaryEdit, index, null, ResourceAccess.FieldType.String);
				this.SetTextField(this.primaryAbout, index, null, ResourceAccess.FieldType.About);

				if (this.secondaryCulture == null)
				{
					this.SetTextField(this.secondaryEdit, 0, null, ResourceAccess.FieldType.None);
					this.SetTextField(this.secondaryAbout, 0, null, ResourceAccess.FieldType.None);
				}
				else
				{
					this.SetTextField(this.secondaryEdit, index, this.secondaryCulture, ResourceAccess.FieldType.String);
					this.SetTextField(this.secondaryAbout, index, this.secondaryCulture, ResourceAccess.FieldType.About);
				}

				AbstractTextField edit = null;
				if (column == 0)  edit = this.labelEdit;
				if (column == 1)  edit = this.primaryEdit;
				if (column == 2)  edit = this.secondaryEdit;
				if (edit != null && edit.Visibility)
				{
					edit.SelectAll();
					edit.Focus();
				}
				else
				{
					this.labelEdit.Cursor = 100000;
					this.primaryEdit.Cursor = 100000;
					this.secondaryEdit.Cursor = 100000;
				}
			}

			this.ignoreChange = iic;

			this.UpdateCommands();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			Rectangle rect, r;

			int lines = System.Math.Max((int)box.Height/50, 4);
			int editLines = lines*2/3;
			int aboutLines = lines-editLines;
			double cultureHeight = 20;
			double editHeight = editLines*13+8;
			double aboutHeight = aboutLines*13+8;

			//	Il faut obligatoirement s'occuper d'abord de this.array, puisque les autres
			//	widgets d�pendent des largeurs relatives de ses colonnes.
			rect = box;
			rect.Top -= cultureHeight+5;
			rect.Bottom += editHeight+5+aboutHeight+5;
			this.array.SetManualBounds(rect);

			rect = box;
			rect.Bottom = rect.Top-cultureHeight-5;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)-1;
			this.primaryCulture.SetManualBounds(rect);

			if (this.secondaryCultures != null)
			{
				rect.Left = rect.Right+2;
				rect.Width = this.array.GetColumnsAbsoluteWidth(2);
				double w = System.Math.Floor(rect.Width/this.secondaryCultures.Length);
				for (int i=0; i<this.secondaryCultures.Length; i++)
				{
					r = rect;
					r.Left += w*i;
					r.Width = w;
					if (i == this.secondaryCultures.Length-1)
					{
						r.Right = rect.Right;
					}
					this.secondaryCultures[i].SetManualBounds(r);
				}
			}

			rect = box;
			rect.Top = rect.Bottom+editHeight+aboutHeight+5;
			rect.Bottom = rect.Top-editHeight;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelStatic.SetManualBounds(rect);
			rect.Width += 5+1;
			r = rect;
			r.Bottom = r.Top-21;
			this.labelEdit.SetManualBounds(r);
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryEdit.SetManualBounds(rect);
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryEdit.SetManualBounds(rect);

			rect = box;
			rect.Top = rect.Bottom+aboutHeight;
			rect.Bottom = rect.Top-aboutHeight;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelAbout.SetManualBounds(rect);
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryAbout.SetManualBounds(rect);
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryAbout.SetManualBounds(rect);
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant � un texte �ditable.
			subfield = 0;

			if (textField == this.labelEdit)
			{
				field = 0;
				return;
			}

			if (textField == this.primaryEdit)
			{
				field = 1;
				return;
			}

			if (textField == this.primaryAbout)
			{
				field = 3;
				return;
			}

			if (this.secondaryCulture != null)
			{
				if (textField == this.secondaryEdit)
				{
					field = 2;
					return;
				}

				if (textField == this.secondaryAbout)
				{
					field = 4;
					return;
				}
			}

			field = -1;
			subfield = -1;
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'�diter des index.
			if (subfield == 0)
			{
				switch (field)
				{
					case 0:
						return this.labelEdit;

					case 1:
						return this.primaryEdit;

					case 2:
						return this.secondaryEdit;

					case 3:
						return this.primaryAbout;

					case 4:
						return this.secondaryAbout;

				}
			}

			return null;
		}

		public static void SearchCreateFilterGroup(AbstractGroup parent, EventHandler handler)
		{
			StaticText label;
			CheckButton check;
			
			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Strings.Edit;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 0, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Strings.About;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 16, 0);

			check = new CheckButton(parent);
			check.Name = "0";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*0, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.Label);

			check = new CheckButton(parent);
			check.Name = "1";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryText);

			check = new CheckButton(parent);
			check.Name = "2";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryText);

			check = new CheckButton(parent);
			check.Name = "3";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryAbout);

			check = new CheckButton(parent);
			check.Name = "4";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryAbout);

			// (*)	Ce num�ro correspond � field dans ResourceAccess.SearcherIndexToAccess !
		}


		private void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a chang�.
			this.UpdateClientGeometry();

			Abstract.leftArrayWidth = this.array.GetColumnsAbsoluteWidth(0)+20;
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a chang�.
			this.UpdateArray();
			this.array.ShowSelectedRow();
		}

		private void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a chang�.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne s�lectionn�e a chang�.
			if (!this.mainWindow.Terminate())
			{
				return;
			}

			this.access.AccessIndex = this.array.SelectedRow;
			this.mainWindow.LocatorFix();

			this.UpdateEdit();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appel� lorsque la ligne �ditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte �ditable a chang�.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;
			int sel = this.access.AccessIndex;

			if (edit == this.labelEdit)
			{
				this.UpdateFieldName(edit, sel);
			}

			if (edit == this.primaryEdit)
			{
				this.access.SetField(sel, null, ResourceAccess.FieldType.String, new ResourceAccess.Field(text));
				this.UpdateArrayField(1, sel, null, ResourceAccess.FieldType.String);
			}

			if (edit == this.secondaryEdit)
			{
				this.access.SetField(sel, this.secondaryCulture, ResourceAccess.FieldType.String, new ResourceAccess.Field(text));
				this.UpdateArrayField(2, sel, this.secondaryCulture, ResourceAccess.FieldType.String);
			}

			if (edit == this.primaryAbout)
			{
				this.access.SetField(sel, null, ResourceAccess.FieldType.About, new ResourceAccess.Field(text));
			}

			if (edit == this.secondaryAbout)
			{
				this.access.SetField(sel, this.secondaryCulture, ResourceAccess.FieldType.About, new ResourceAccess.Field(text));
			}

			this.UpdateModificationsCulture();
		}

		private void HandleCursorChanged(object sender)
		{
			//	Le curseur a �t� d�plac� dans un texte �ditable.
			if ( this.ignoreChange )  return;

			this.lastActionIsReplace = false;
		}


		protected StaticText					labelStatic;
		protected MyWidgets.TextFieldExName		labelEdit;
		protected TextFieldMulti				primaryEdit;
		protected TextFieldMulti				secondaryEdit;
		protected StaticText					labelAbout;
		protected TextFieldMulti				primaryAbout;
		protected TextFieldMulti				secondaryAbout;
	}
}
