using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Strings : Abstract
	{
		public Strings(Module module, PanelsContext context, ResourceAccess access) : base(module, context, access)
		{
			int tabIndex = 0;

			this.primaryCulture = new IconButtonMark(this);
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = SiteMark.OnBottom;
			this.primaryCulture.MarkDimension = 5;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;
			this.primaryCulture.TabIndex = tabIndex++;
			this.primaryCulture.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.array = new MyWidgets.StringArray(this);
			this.array.Columns = 3;
			this.array.SetColumnsRelativeWidth(0, 0.30);
			this.array.SetColumnsRelativeWidth(1, 0.35);
			this.array.SetColumnsRelativeWidth(2, 0.35);
			this.array.SetDynamicsToolTips(0, true);
			this.array.SetDynamicsToolTips(1, false);
			this.array.SetDynamicsToolTips(2, false);
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
			this.array.CellCountChanged += new EventHandler (this.HandleArrayCellCountChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelStatic = new StaticText(this);
			this.labelStatic.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelStatic.Text = Res.Strings.Viewers.Strings.Edit;
			this.labelStatic.Visibility = (this.module.Mode != DesignerMode.Build);

			this.labelEdit = new TextFieldEx(this);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.labelEdit.TabIndex = tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);

			this.primaryEdit = new TextFieldMulti(this);
			this.primaryEdit.Name = "PrimaryEdit";
			this.primaryEdit.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryEdit.TabIndex = tabIndex++;
			this.primaryEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryEdit = new TextFieldMulti(this);
			this.secondaryEdit.Name = "SecondaryEdit";
			this.secondaryEdit.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryEdit.TabIndex = tabIndex++;
			this.secondaryEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelAbout = new StaticText(this);
			this.labelAbout.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelAbout.Text = Res.Strings.Viewers.Strings.About;

			this.primaryAbout = new TextFieldMulti(this);
			this.primaryAbout.Name = "PrimaryAbout";
			this.primaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryAbout.TabIndex = tabIndex++;
			this.primaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryAbout = new TextFieldMulti(this);
			this.secondaryAbout.Name = "SecondaryAbout";
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryAbout.TabIndex = tabIndex++;
			this.secondaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

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
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

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


		protected override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Strings;
			}
		}

		public override ResourceAccess.Type BundleType
		{
			get
			{
				return ResourceAccess.Type.Strings;
			}
		}


		
		protected override void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			//	Version spéciale pour les trois colonnes de Strings.
			this.array.TotalRows = this.access.AccessCount;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.access.AccessCount)
				{
					this.UpdateArrayField(0, first+i, null, "Name");
					this.UpdateArrayField(1, first+i, null, "String");
					this.UpdateArrayField(2, first+i, this.secondaryCulture, (this.secondaryCulture == null) ? null : "String");
				}
				else
				{
					this.UpdateArrayField(0, first+i, null, null);
					this.UpdateArrayField(1, first+i, null, null);
					this.UpdateArrayField(2, first+i, null, null);
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
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
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
				this.SetTextField(this.labelEdit, 0, null, null);
				this.SetTextField(this.primaryEdit, 0, null, null);
				this.SetTextField(this.primaryAbout, 0, null, null);
				this.SetTextField(this.secondaryEdit, 0, null, null);
				this.SetTextField(this.secondaryAbout, 0, null, null);
			}
			else
			{
				int index = this.access.AccessIndex;

				this.SetTextField(this.labelEdit, index, null, "Name");
				this.SetTextField(this.primaryEdit, index, null, "String");
				this.SetTextField(this.primaryAbout, index, null, "About");

				if (this.secondaryCulture == null)
				{
					this.SetTextField(this.secondaryEdit, 0, null, null);
					this.SetTextField(this.secondaryAbout, 0, null, null);
				}
				else
				{
					this.SetTextField(this.secondaryEdit, index, this.secondaryCulture, "String");
					this.SetTextField(this.secondaryAbout, index, this.secondaryCulture, "About");
				}

				AbstractTextField edit = null;
				if (column == 0)  edit = this.labelEdit;
				if (column == 1)  edit = this.primaryEdit;
				if (column == 2)  edit = this.secondaryEdit;
				if (edit != null && edit.Visibility)
				{
					edit.Focus();
					edit.SelectAll();
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
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			box.Top -= 4;
			Rectangle rect, r;

			int lines = System.Math.Max((int)box.Height/50, 4);
			int editLines = lines*2/3;
			int aboutLines = lines-editLines;
			double cultureHeight = 20;
			double editHeight = editLines*13+8;
			double aboutHeight = aboutLines*13+8;

			//	Il faut obligatoirement s'occuper d'abord de this.array, puisque les autres
			//	widgets dépendent des largeurs relatives de ses colonnes.
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


		void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
			this.UpdateClientGeometry();
		}

		void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.access.AccessIndex = this.array.SelectedRow;
			this.UpdateEdit();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
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
				this.access.SetField(sel, null, "String", new ResourceAccess.Field(text));
				this.UpdateArrayField(1, sel, null, "String");
			}

			if (edit == this.secondaryEdit)
			{
				this.access.SetField(sel, this.secondaryCulture, "String", new ResourceAccess.Field(text));
				this.UpdateArrayField(2, sel, this.secondaryCulture, "String");
			}

			if (edit == this.primaryAbout)
			{
				this.access.SetField(sel, null, "About", new ResourceAccess.Field(text));
			}

			if (edit == this.secondaryAbout)
			{
				this.access.SetField(sel, this.secondaryCulture, "About", new ResourceAccess.Field(text));
			}
		}

		void HandleCursorChanged(object sender)
		{
			//	Le curseur a été déplacé dans un texte éditable.
			if ( this.ignoreChange )  return;

			this.lastActionIsReplace = false;
		}


		protected StaticText				labelStatic;
		protected TextFieldEx				labelEdit;
		protected TextFieldMulti			primaryEdit;
		protected TextFieldMulti			secondaryEdit;
		protected StaticText				labelAbout;
		protected TextFieldMulti			primaryAbout;
		protected TextFieldMulti			secondaryAbout;
	}
}
