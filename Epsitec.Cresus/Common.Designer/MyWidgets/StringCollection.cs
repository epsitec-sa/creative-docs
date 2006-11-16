using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Tableau permettant d'éditer une collection de strings.
	/// </summary>
	public class StringCollection : Widget
	{
		public StringCollection() : base()
		{
			this.AutoEngage = false;
			this.AutoFocus = true;
			this.AutoDoubleClick = true;

			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;

			this.staticTexts = new List<StaticText>();
			this.glyphButtons = new List<GlyphButton>();
			this.textFields = new List<TextFieldMulti>();

			this.CreateGrid();
			this.CreateToolbar();
			this.UpdateButtons();
		}

		public StringCollection(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public List<string> Collection
		{
			//	Collection de strings éditée par le widget.
			get
			{
				return this.strings;
			}

			set
			{
				this.strings = value;

				if (this.strings == null)
				{
					this.strings = new List<string>();
				}

				if (this.selectedRow == -1)
				{
					this.SelectedRow = 0;
				}

				if (this.selectedRow > this.strings.Count-1)
				{
					this.SelectedRow = System.Math.Max(this.strings.Count-1, 0);
				}

				this.AdaptGrid();
				this.UpdateGrid();
				this.UpdateArrows();
				this.UpdateButtons();
			}
		}

		public AbstractTextField FocusedTextField
		{
			//	Retourne la ligne éditable qui a le focus.
			get
			{
				int sel = this.SelectedRow;
				if (sel == -1)
				{
					return null;
				}
				else
				{
					return this.textFields[sel];
				}
			}
		}

		public AbstractTextField GetTextField(int index)
		{
			//	Retourne une ligne éditable.
			if (index < this.textFields.Count)
			{
				return this.textFields[index];
			}

			return null;
		}

		public int GetIndex(AbstractTextField textField)
		{
			//	Cherche l'index d'une ligne éditable.
			for (int i=0; i<this.textFields.Count; i++)
			{
				if (textField == this.textFields[i])
				{
					return i;
				}
			}

			return -1;
		}


		protected void CreateGrid()
		{
			//	Crée le tableau principal.
			this.grid = new GridLayoutEngine();

			this.grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(20, GridUnitType.Absolute)));
			this.grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(20, GridUnitType.Absolute)));
			this.grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Proportional)));
			this.grid.ColumnDefinitions[1].RightBorder = -1;
			
			this.grid.RowDefinitions.Add(new RowDefinition());
			this.grid.RowDefinitions[0].BottomBorder = 3;
			
			LayoutEngine.SetLayoutEngine(this, this.grid);
		}

		protected void CreateToolbar()
		{
			//	Crée la barre d'outils dans la première ligne du tableau.
			Widget toolbar = new Widget();
			GridLayoutEngine.SetColumn(toolbar, 2);
			GridLayoutEngine.SetRow(toolbar, 0);
			this.Children.Add(toolbar);

			this.buttonAdd = new IconButton(toolbar);
			this.buttonAdd.IconName = Misc.Icon("StringAdd");
			this.buttonAdd.Dock = DockStyle.Left;
			this.buttonAdd.AutoFocus = false;
			this.buttonAdd.Pressed += new MessageEventHandler(this.HandleButtonAddPressed);
			ToolTip.Default.SetToolTip(this.buttonAdd, Res.Strings.StringCollection.Add);

			this.buttonDuplicate = new IconButton(toolbar);
			this.buttonDuplicate.IconName = Misc.Icon("StringDuplicate");
			this.buttonDuplicate.Dock = DockStyle.Left;
			this.buttonDuplicate.AutoFocus = false;
			this.buttonDuplicate.Pressed += new MessageEventHandler(this.HandleButtonDuplicatePressed);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, Res.Strings.StringCollection.Duplicate);

			this.buttonRemove = new IconButton(toolbar);
			this.buttonRemove.IconName = Misc.Icon("StringRemove");
			this.buttonRemove.Dock = DockStyle.Left;
			this.buttonRemove.AutoFocus = false;
			this.buttonRemove.Pressed += new MessageEventHandler(this.HandleButtonRemovePressed);
			ToolTip.Default.SetToolTip(this.buttonRemove, Res.Strings.StringCollection.Remove);

			IconSeparator sep = new IconSeparator(toolbar);
			sep.IsHorizontal = true;
			sep.Dock = DockStyle.Left;

			this.buttonPrev = new IconButton(toolbar);
			this.buttonPrev.IconName = Misc.Icon("StringPrev");
			this.buttonPrev.Dock = DockStyle.Left;
			this.buttonPrev.AutoFocus = false;
			this.buttonPrev.Pressed += new MessageEventHandler(this.HandleButtonPrevPressed);
			ToolTip.Default.SetToolTip(this.buttonPrev, Res.Strings.StringCollection.Prev);

			this.buttonNext = new IconButton(toolbar);
			this.buttonNext.IconName = Misc.Icon("StringNext");
			this.buttonNext.Dock = DockStyle.Left;
			this.buttonNext.AutoFocus = false;
			this.buttonNext.Pressed += new MessageEventHandler(this.HandleButtonNextPressed);
			ToolTip.Default.SetToolTip(this.buttonNext, Res.Strings.StringCollection.Next);

			HSlider slider = new HSlider(toolbar);
			slider.PreferredWidth = 80;
			slider.Margins = new Margins(2, 2, 3, 3);
			slider.MinValue = 1.0M;
			slider.MaxValue = 5.0M;  // 1..5
			slider.SmallChange = 1.0M;
			slider.LargeChange = 2.0M;
			slider.Resolution = 1.0M;
			slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			slider.Value = 1.0M;
			slider.Dock = DockStyle.Right;
		}

		protected void AdaptGrid()
		{
			//	Adapte le nombre de lignes du tableau en fonction de la collection.
			int sc = System.Math.Max(this.strings.Count, 1);

			while (this.grid.RowDefinitions.Count != sc+1)
			{
				int count = this.grid.RowDefinitions.Count;

				if (count < sc+1)  // ajoute une ligne ?
				{
					this.grid.RowDefinitions.Add(new RowDefinition());
					this.grid.RowDefinitions[count].TopBorder = -1;

					StaticText fix = new StaticText();
					fix.Text = count.ToString();
					fix.ContentAlignment = ContentAlignment.MiddleRight;
					fix.Margins = new Margins(0, 4, 0, 0);
					//	TODO: bug pour Pierre avec ce mode !
					//?fix.VerticalAlignment = VerticalAlignment.BaseLine;
					GridLayoutEngine.SetColumn(fix, 0);
					GridLayoutEngine.SetRow(fix, count);
					this.Children.Add(fix);
					this.staticTexts.Add(fix);

					GlyphButton button = new GlyphButton();
					button.GlyphShape = GlyphShape.None;
					button.Pressed += new MessageEventHandler(this.HandleButtonTextPressed);
					GridLayoutEngine.SetColumn(button, 1);
					GridLayoutEngine.SetRow(button, count);
					this.Children.Add(button);
					this.glyphButtons.Add(button);

					TextFieldMulti field = new TextFieldMulti();
					field.TextChanged += new EventHandler(this.HandleTextChanged);
					field.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleTextFocusChanged);
					//	TODO: bug pour Pierre avec ce mode !
					//?field.VerticalAlignment = VerticalAlignment.BaseLine;
					field.TabIndex = count;
					field.TabNavigation = TabNavigationMode.ActivateOnTab;
					field.TextNavigator.AllowTabInsertion = false;
					this.AdaptTextFieldLines(field);
					GridLayoutEngine.SetColumn(field, 2);
					GridLayoutEngine.SetRow(field, count);
					this.Children.Add(field);
					this.textFields.Add(field);
				}
				else  // supprime une ligne ?
				{
					this.glyphButtons[count-2].Pressed -= new MessageEventHandler(this.HandleButtonTextPressed);
					this.textFields[count-2].TextChanged -= new EventHandler(this.HandleTextChanged);
					this.textFields[count-2].KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleTextFocusChanged);

					this.Children.Remove(this.staticTexts[count-2]);
					this.Children.Remove(this.glyphButtons[count-2]);
					this.Children.Remove(this.textFields[count-2]);

					this.grid.RowDefinitions.RemoveAt(count-1);

					this.staticTexts.RemoveAt(count-2);
					this.glyphButtons.RemoveAt(count-2);
					this.textFields.RemoveAt(count-2);
				}
			}
		}

		protected void AdaptTextFieldLines()
		{
			//	Adapte le nombre de lignes des lignes éditables.
			for (int i=0; i<this.textFields.Count; i++)
			{
				this.AdaptTextFieldLines(this.textFields[i]);
			}
		}

		protected void AdaptTextFieldLines(TextFieldMulti field)
		{
			//	Adapte le nombre de lignes d'une ligne éditable.
			double h = 20;
			if (this.lineCount > 1)
			{
				h = 8 + 13*this.lineCount;  // empyrique !
			}

			field.PreferredHeight = h;
		}

		protected void UpdateGrid()
		{
			//	Adapte les textes des lignes éditables en fonction de la collection.
			for (int i=0; i<this.textFields.Count; i++)
			{
				string text = "";

				if (i < this.strings.Count)
				{
					text = this.strings[i];
				}

				this.textFields[i].Text = text;
			}
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons pour ajouter/supprimer/déplacer la ligne sélectionnée.
			int sel = this.SelectedRow;
			int count = this.textFields.Count;
			bool enable = (this.strings != null);

			this.buttonAdd.Enable = enable;
			this.buttonDuplicate.Enable = (enable && sel != -1);
			this.buttonRemove.Enable = (enable && sel != -1 && count > 1);
			this.buttonPrev.Enable = (enable && sel != -1 && sel > 0);
			this.buttonNext.Enable = (enable && sel != -1 && sel < count-1);
		}

		protected void UpdateArrows()
		{
			//	Met à jour les flèches ">" dans les boutons de la 2ème colonne.
			for (int i=0; i<this.textFields.Count; i++)
			{
				this.glyphButtons[i].GlyphShape = (this.selectedRow == i) ? GlyphShape.ArrowRight : GlyphShape.None;
			}
		}


		protected int SelectedRow
		{
			//	Ligne sélectionnée dans le tableau.
			get
			{
				if (this.selectedRow >= this.textFields.Count)  // la ligne sélectionnée a été détruite ?
				{
					return -1;
				}

				return this.selectedRow;
			}
			set
			{
				if (this.selectedRow != value)
				{
					this.selectedRow = value;

					this.UpdateArrows();
					this.UpdateButtons();
				}
			}
		}

		protected void SetFocusInSelection()
		{
			//	Met le focus dans la ligne éditable sélectionnée.
			int sel = this.SelectedRow;
			if (sel != -1)
			{
				this.textFields[sel].Focus();
				this.textFields[sel].SelectAll();
			}

			this.OnStringFocusChanged();
		}


		protected void HandleButtonAddPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour créer une nouvelle ligne est pressé.
			int sel = this.SelectedRow;
			if (sel == -1)
			{
				sel = this.strings.Count-1;  // si aucune ligne sélectionnée -> insère à la fin
			}

			this.strings.Insert(sel+1, "");
			this.AdaptGrid();
			this.UpdateGrid();
			this.SelectedRow = sel+1;
			this.SetFocusInSelection();
			this.OnStringTextChanged();
		}

		protected void HandleButtonDuplicatePressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour dupliquer une ligne est pressé.
			int sel = this.SelectedRow;
			System.Diagnostics.Debug.Assert(sel != -1);

			this.strings.Insert(sel+1, this.strings[sel]);
			this.AdaptGrid();
			this.UpdateGrid();
			this.SelectedRow = sel+1;
			this.SetFocusInSelection();
			this.OnStringTextChanged();
		}

		protected void HandleButtonRemovePressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour supprimer une ligne est pressé.
			int sel = this.SelectedRow;
			System.Diagnostics.Debug.Assert(sel != -1);

			this.strings.RemoveAt(sel);
			this.AdaptGrid();
			this.UpdateGrid();

			if (sel >= this.strings.Count)
			{
				sel = this.strings.Count-1;
			}
			this.selectedRow = -1;  // pour forcer SelectedRow à refaire son travail
			this.SelectedRow = sel;
			this.SetFocusInSelection();
			this.OnStringTextChanged();
		}

		protected void HandleButtonPrevPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour monter une ligne est pressé.
			int sel = this.SelectedRow;
			System.Diagnostics.Debug.Assert(sel != -1);

			string s = this.strings[sel];
			this.strings[sel] = this.strings[sel-1];
			this.strings[sel-1] = s;

			this.UpdateGrid();
			this.SelectedRow = sel-1;
			this.SetFocusInSelection();
		}

		protected void HandleButtonNextPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour descendre une ligne est pressé.
			int sel = this.SelectedRow;
			System.Diagnostics.Debug.Assert(sel != -1);

			string s = this.strings[sel];
			this.strings[sel] = this.strings[sel+1];
			this.strings[sel+1] = s;

			this.UpdateGrid();
			this.SelectedRow = sel+1;
			this.SetFocusInSelection();
		}

		private void HandleSliderChanged(object sender)
		{
			//	Appelé lorsque le slider pour le nombre de lignes éditables a été déplacé.
			HSlider slider = sender as HSlider;
			this.lineCount = (double) slider.Value;  // 1..5
			this.AdaptTextFieldLines();
		}

		protected void HandleButtonTextPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton ">" pour sélectionner une ligne est pressé.
			GlyphButton button = sender as GlyphButton;
			this.SelectedRow = glyphButtons.IndexOf(button);
			this.SetFocusInSelection();
		}

		void HandleTextChanged(object sender)
		{
			//	Appelé lorsqu'une ligne éditable a changé.
			TextFieldMulti field = sender as TextFieldMulti;
			int i = textFields.IndexOf(field);
			if (this.strings.Count == 0)
			{
				this.strings.Add("");
			}
			this.strings[i] = field.Text;
			this.OnStringTextChanged();
		}

		protected void HandleTextFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsqu'une ligne éditable voit son focus changer.
			TextFieldMulti field = sender as TextFieldMulti;
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.SelectedRow = textFields.IndexOf(field);
				this.OnStringFocusChanged();
			}
		}


		#region Events handler
		protected virtual void OnStringTextChanged()
		{
			//	Génère un événement pour dire qu'une string a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("StringTextChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler StringTextChanged
		{
			add
			{
				this.AddUserEventHandler("StringTextChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("StringTextChanged", value);
			}
		}

		protected virtual void OnStringFocusChanged()
		{
			//	Génère un événement pour dire qu'une string a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("StringFocusChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler StringFocusChanged
		{
			add
			{
				this.AddUserEventHandler("StringFocusChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("StringFocusChanged", value);
			}
		}
		#endregion


		protected List<string>				strings;
		protected GridLayoutEngine			grid;
		protected IconButton				buttonAdd;
		protected IconButton				buttonDuplicate;
		protected IconButton				buttonRemove;
		protected IconButton				buttonPrev;
		protected IconButton				buttonNext;
		protected List<StaticText>			staticTexts;
		protected List<GlyphButton>			glyphButtons;
		protected List<TextFieldMulti>		textFields;
		protected int						selectedRow = -1;
		protected double					lineCount = 1;
	}
}
