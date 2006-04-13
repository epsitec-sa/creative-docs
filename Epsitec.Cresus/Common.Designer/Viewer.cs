using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Viewer : Widget
	{
		public Viewer(Module module)
		{
			this.module = module;

			this.labelsIndex = new List<string>();

			int tabIndex = 0;

			this.primaryCulture = new TextField(this);
			this.primaryCulture.IsReadOnly = true;
			this.primaryCulture.TabIndex = tabIndex++;
			this.primaryCulture.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryCulture = new TextFieldCombo(this);
			this.secondaryCulture.IsReadOnly = true;
			this.secondaryCulture.ComboClosed += new EventHandler(this.HandleSecondaryCultureComboClosed);
			this.secondaryCulture.TabIndex = tabIndex++;
			this.secondaryCulture.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.array = new MyWidgets.StringArray(this);
			this.array.Columns = 3;
			this.array.SetColumnsRelativeWidth(0, 0.30);
			this.array.SetColumnsRelativeWidth(1, 0.35);
			this.array.SetColumnsRelativeWidth(2, 0.35);
			this.array.SetDynamicsToolTips(0, true);
			this.array.SetDynamicsToolTips(1, false);
			this.array.SetDynamicsToolTips(2, false);
			this.array.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelEdit = new StaticText(this);
			this.labelEdit.Alignment = ContentAlignment.MiddleRight;
			this.labelEdit.Text = Res.Strings.String.Edit;

			this.primaryEdit = new TextFieldMulti(this);
			this.primaryEdit.Name = "PrimaryEdit";
			this.primaryEdit.TextChanged += new EventHandler(this.HandleEditTextChanged);
			this.primaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryEdit.TabIndex = tabIndex++;
			this.primaryEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryEdit = new TextFieldMulti(this);
			this.secondaryEdit.Name = "SecondaryEdit";
			this.secondaryEdit.TextChanged += new EventHandler(this.HandleEditTextChanged);
			this.secondaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryEdit.TabIndex = tabIndex++;
			this.secondaryEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelAbout = new StaticText(this);
			this.labelAbout.Alignment = ContentAlignment.MiddleRight;
			this.labelAbout.Text = Res.Strings.String.About;

			this.primaryAbout = new TextField(this);
			this.primaryAbout.Name = "PrimaryAbout";
			this.primaryAbout.TextChanged += new EventHandler(this.HandleAboutTextChanged);
			this.primaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryAbout.TabIndex = tabIndex++;
			this.primaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryAbout = new TextField(this);
			this.secondaryAbout.Name = "SecondaryAbout";
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleAboutTextChanged);
			this.secondaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryAbout.TabIndex = tabIndex++;
			this.secondaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.UpdateCultures();
			this.HandleArraySelectedRowChanged(null);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.secondaryCulture.ComboClosed -= new EventHandler(this.HandleSecondaryCultureComboClosed);
				
				this.array.CellsQuantityChanged -= new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);
				
				this.primaryEdit.TextChanged -= new EventHandler(this.HandleEditTextChanged);
				this.primaryEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryEdit.TextChanged -= new EventHandler(this.HandleEditTextChanged);
				this.secondaryEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryAbout.TextChanged -= new EventHandler(this.HandleAboutTextChanged);
				this.primaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryAbout.TextChanged -= new EventHandler(this.HandleAboutTextChanged);
				this.secondaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}
		}


		public AbstractTextField CurrentTextField
		{
			//	Retourne le texte éditable en cours d'édition.
			get
			{
				return this.currentTextField;
			}
		}

		public void DoSearch(string search, bool isReverse, bool isCase, bool isAbout)
		{
			//	Effectue une recherche.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				sel = 0;
			}

			int dir = isReverse ? -1 : 1;

			if (!isCase)
			{
				search = search.ToLower();
			}

			int column = -1;
			int index = -1;

			for (int i=0; i<this.labelsIndex.Count; i++)
			{
				sel += dir;

				if (sel >= this.labelsIndex.Count)
				{
					sel = 0;
				}

				if (sel < 0)
				{
					sel = this.labelsIndex.Count-1;
				}

				string label     = this.labelsIndex[sel];
				string primary   = this.primaryBundle[label].AsString;
				string secondary = this.secondaryBundle[label].AsString;
				string pAbout    = this.primaryBundle[label].About;
				string sAbout    = this.secondaryBundle[label].About;

				if ( secondary == null )  secondary = "";
				if ( pAbout    == null )  pAbout    = "";
				if ( sAbout    == null )  sAbout    = "";

				if (!isCase)
				{
					label     = label.ToLower();
					primary   = primary.ToLower();
					secondary = secondary.ToLower();
					pAbout    = pAbout.ToLower();
					sAbout    = sAbout.ToLower();
				}

				if (label.Contains(search))
				{
					break;
				}

				index = primary.IndexOf(search);
				if (index != -1)
				{
					column = 1;
					break;
				}

				index = secondary.IndexOf(search);
				if (index != -1)
				{
					column = 2;
					break;
				}

				if (isAbout)
				{
					index = pAbout.IndexOf(search);
					if (index != -1)
					{
						column = 3;
						break;
					}

					index = sAbout.IndexOf(search);
					if (index != -1)
					{
						column = 4;
						break;
					}
				}
			}

			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();

			AbstractTextField edit = null;
			if (column == 1)  edit = this.primaryEdit;
			if (column == 2)  edit = this.secondaryEdit;
			if (column == 3)  edit = this.primaryAbout;
			if (column == 4)  edit = this.secondaryAbout;
			if (edit != null)
			{
				this.Window.MakeActive();
				edit.Focus();
				edit.CursorFrom  = index;
				edit.CursorTo    = index+search.Length;
				edit.CursorAfter = false;
			}
		}

		public void DoFilter(string filter, bool isBegin, bool isCase)
		{
			//	Change le filtre des ressources visibles.
			string label = "";
			int sel = this.array.SelectedRow;
			if (sel != -1 && sel < this.labelsIndex.Count)
			{
				label = this.labelsIndex[sel];
			}

			this.UpdateLabelsIndex(filter, isBegin, isCase);
			this.UpdateArray();

			sel = this.labelsIndex.IndexOf(label);
			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
			this.module.Notifier.NotifyInfoAccessChanged();
		}

		public void DoAccess(string name)
		{
			//	Change la ressource visible.
			int sel = this.array.SelectedRow;

			if ( name == "AccessFirst" )  sel = 0;
			if ( name == "AccessPrev"  )  sel --;
			if ( name == "AccessNext"  )  sel ++;
			if ( name == "AccessLast"  )  sel = 1000000;

			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
			this.module.Notifier.NotifyInfoAccessChanged();
		}

		public void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			this.primaryBundle.Remove(sel);

			this.labelsIndex.RemoveAt(sel);
			this.UpdateArray();

			sel = System.Math.Min(sel, this.labelsIndex.Count-1);
			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
			this.module.Notifier.NotifyInfoAccessChanged();
			this.module.Modifier.IsDirty = true;
		}

		public void DoDuplicate()
		{
			//	Duplique la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			string name = this.labelsIndex[sel];
			string newName = Misc.CopyName(name);
			int newSel = sel+1;

			ResourceBundle.Field field = this.primaryBundle[sel];
			ResourceBundle.Field newField = new ResourceBundle.Field(this.primaryBundle, field.Xml);
			newField.SetName(newName);
			this.primaryBundle.Insert(newSel, newField);

			field = this.secondaryBundle[name];
			if ( field != null )
			{
				newField = new ResourceBundle.Field(this.secondaryBundle, field.Xml);
				newField.SetName(newName);
				this.secondaryBundle.Add(newField);
			}

			this.labelsIndex.Insert(newSel, newName);
			this.UpdateArray();

			this.array.SelectedRow = newSel;
			this.array.ShowSelectedRow();
			this.module.Notifier.NotifyInfoAccessChanged();
			this.module.Modifier.IsDirty = true;
		}

		public void DoMove(int direction)
		{
			//	Déplace la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			int newSel = sel+direction;
			if ( newSel < 0 || newSel >= this.labelsIndex.Count )  return;

			ResourceBundle.Field field = this.primaryBundle[sel];
			this.primaryBundle.Remove(sel);
			this.primaryBundle.Insert(newSel, field);

			string label = this.labelsIndex[sel];
			this.labelsIndex.RemoveAt(sel);
			this.labelsIndex.Insert(newSel, label);
			this.UpdateArray();

			this.array.SelectedRow = newSel;
			this.array.ShowSelectedRow();
			this.module.Notifier.NotifyInfoAccessChanged();
			this.module.Modifier.IsDirty = true;
		}

		public string InfoAccessText
		{
			//	Donne le texte d'information sur l'accès en cours.
			get
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				int sel = this.array.SelectedRow;
				if (sel == -1)
				{
					builder.Append("-");
				}
				else
				{
					builder.Append((sel+1).ToString());
				}

				builder.Append("/");
				builder.Append(this.labelsIndex.Count.ToString());

				if (this.labelsIndex.Count < this.primaryBundle.FieldCount)
				{
					builder.Append(" (");
					builder.Append(this.primaryBundle.FieldCount.ToString());
					builder.Append(")");
				}

				return builder.ToString();
			}
		}


		protected void UpdateCultures()
		{
			//	Met à jour les widgets pour les cultures.
			ResourceBundleCollection bundles = this.module.Bundles;

			this.primaryBundle = bundles[ResourceLevel.Default];
			this.primaryCulture.Text = Misc.LongCulture(this.primaryBundle.Culture.Name);

			this.secondaryBundle = null;
			this.secondaryCulture.Items.Clear();
			for ( int b=0 ; b<bundles.Count ; b++ )
			{
				ResourceBundle bundle = bundles[b];
				if ( bundle != this.primaryBundle )
				{
					this.secondaryCulture.Items.Add(Misc.LongCulture(bundle.Culture.Name));
					if ( this.secondaryBundle == null )
					{
						this.secondaryBundle = bundle;
					}
				}
			}

			if ( this.secondaryBundle == null )
			{
				this.secondaryCulture.Text = "";
			}
			else
			{
				this.secondaryCulture.Text = Misc.LongCulture(this.secondaryBundle.Culture.Name);
			}

			this.UpdateLabelsIndex("", false, false);
		}

		protected void UpdateLabelsIndex(string filter, bool isBegin, bool isCase)
		{
			//	Construit l'index en fonction des ressources primaires.
			this.labelsIndex.Clear();

			if (!isCase)
			{
				filter = filter.ToLower();
			}

			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				if (filter != "")
				{
					if (isCase)
					{
						if (isBegin)
						{
							if (!field.Name.StartsWith(filter))  continue;
						}
						else
						{
							if (!field.Name.Contains(filter))  continue;
						}
					}
					else
					{
						string name = field.Name.ToLower();
						if (isBegin)
						{
							if (!name.StartsWith(filter))  continue;
						}
						else
						{
							if (!name.Contains(filter))  continue;
						}
					}
				}

				this.labelsIndex.Add(field.Name);
			}
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.labelsIndex.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.labelsIndex.Count)
				{
					ResourceBundle.Field primaryField = this.primaryBundle[this.labelsIndex[first+i]];
					ResourceBundle.Field secondaryField = this.secondaryBundle[this.labelsIndex[first+i]];

					this.array.SetLineString(0, first+i, primaryField.Name);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.UpdateArrayField(1, first+i, primaryField);
					this.UpdateArrayField(2, first+i, secondaryField);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineString(1, first+i, "");
					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void UpdateArrayField(int row, int column, ResourceBundle.Field field)
		{
			if (field != null)
			{
				string text = field.AsString;
				if (text != null && text != "")
				{
					this.array.SetLineString(row, column, text);
					this.array.SetLineState(row, column, MyWidgets.StringList.CellState.Normal);
					return;
				}
			}

			this.array.SetLineString(row, column, "");
			this.array.SetLineState(row, column, MyWidgets.StringList.CellState.Warning);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			Rectangle rect;

			//	Il faut obligatoirement s'occuper d'abord de this.array, puisque les autres
			//	widgets dépendent des largeurs relatives de ses colonnes.
			rect = box;
			rect.Top -= 20+5;
			rect.Bottom += 47+5+20+5;
			this.array.Bounds = rect;

			rect = box;
			rect.Bottom = rect.Top-20;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryCulture.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryCulture.Bounds = rect;

			rect = box;
			rect.Top = rect.Bottom+47+20+5;
			rect.Bottom = rect.Top-47;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelEdit.Bounds = rect;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryEdit.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryEdit.Bounds = rect;

			rect = box;
			rect.Top = rect.Bottom+20;
			rect.Bottom = rect.Top-20;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelAbout.Bounds = rect;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryAbout.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryAbout.Bounds = rect;
		}


		protected void SetTextField(AbstractTextField field, string text)
		{
			if (text == null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = text;
			}
		}

		
		void HandleSecondaryCultureComboClosed(object sender)
		{
			//	Changement de la culture secondaire.
			ResourceBundleCollection bundles = this.module.Bundles;

			for ( int b=0 ; b<bundles.Count ; b++ )
			{
				ResourceBundle bundle = bundles[b];

				if ( Misc.LongCulture(bundle.Culture.Name) == secondaryCulture.Text )
				{
					this.secondaryBundle = bundle;
					break;
				}
			}
		}

		void HandleArrayCellsQuantityChanged(object sender)
		{
			this.UpdateArray();
		}

		void HandleArrayCellsContentChanged(object sender)
		{
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			this.ignoreChange = true;

			int sel = this.array.SelectedRow;

			if (sel >= this.labelsIndex.Count)
			{
				sel = -1;
			}

			if ( sel == -1 )
			{
				this.primaryEdit.Enable = false;
				this.secondaryEdit.Enable = false;
				this.primaryAbout.Enable = false;
				this.secondaryAbout.Enable = false;

				this.primaryEdit.Text = "";
				this.secondaryEdit.Text = "";
				this.primaryAbout.Text = "";
				this.secondaryAbout.Text = "";
			}
			else
			{
				this.primaryEdit.Enable = true;
				this.secondaryEdit.Enable = true;
				this.primaryAbout.Enable = true;
				this.secondaryAbout.Enable = true;

				string label = this.labelsIndex[sel];

				this.SetTextField(this.primaryEdit, this.primaryBundle[label].AsString);
				this.SetTextField(this.secondaryEdit, this.secondaryBundle[label].AsString);

				this.SetTextField(this.primaryAbout, this.primaryBundle[label].About);
				this.SetTextField(this.secondaryAbout, this.secondaryBundle[label].About);
			}

			this.module.Notifier.NotifyInfoAccessChanged();

			this.ignoreChange = false;
		}

		void HandleEditTextChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextFieldMulti edit = sender as TextFieldMulti;
			string text = edit.Text;
			int sel = this.array.SelectedRow;
			string label = this.labelsIndex[sel];

			if (edit == this.primaryEdit)
			{
				this.primaryBundle[label].SetStringValue(text);
			}

			if (edit == this.secondaryEdit)
			{
				this.secondaryBundle[label].SetStringValue(text);
			}

			int column = (edit == this.primaryEdit) ? 1 : 2;
			MyWidgets.StringList.CellState state = (text == "") ? MyWidgets.StringList.CellState.Warning : MyWidgets.StringList.CellState.Normal;

			this.array.SetLineString(column, sel, text);
			this.array.SetLineState(column, sel, state);

			this.module.Modifier.IsDirty = true;
		}

		void HandleAboutTextChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextField edit = sender as TextField;
			string text = edit.Text;
			int sel = this.array.SelectedRow;
			string label = this.labelsIndex[sel];

			if (edit == this.primaryAbout)
			{
				this.primaryBundle[label].SetAbout(text);
			}

			if (edit == this.secondaryAbout)
			{
				this.secondaryBundle[label].SetAbout(text);
			}

			this.module.Modifier.IsDirty = true;
		}


		void HandleEditKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.currentTextField = sender as AbstractTextField;
			}
		}


		protected Module					module;
		protected List<string>				labelsIndex;
		protected bool						ignoreChange = false;

		protected TextField					primaryCulture;
		protected TextFieldCombo			secondaryCulture;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected MyWidgets.StringArray		array;
		protected StaticText				labelEdit;
		protected TextFieldMulti			primaryEdit;
		protected TextFieldMulti			secondaryEdit;
		protected StaticText				labelAbout;
		protected TextField					primaryAbout;
		protected TextField					secondaryAbout;
		protected AbstractTextField			currentTextField;
	}
}
