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
			this.textFields = new List<TextField>();

			this.CreateGrid();
			this.CreateToolbar();
			this.UpdateButtons();
		}

		public StringCollection(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public ICollection<string> Collection
		{
#if false
			get
			{
				ICollection<string> collection = new ICollection<string>();
				foreach (string text in this.strings)
				{
					collection.Add(text);
				}
				return collection;
			}
#endif
			set
			{
				string[] list = new string[value.Count];
				value.CopyTo(list, 0);
				this.strings = new List<string>();
				this.strings.AddRange(list);

				this.AdaptGrid();
				this.UpdateGrid();
				this.UpdateButtons();
			}
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
		}

		protected void AdaptGrid()
		{
			//	Adapte le nombre de lignes du tableau en fonction de la collection.
			while (this.grid.RowDefinitions.Count != this.strings.Count+1)
			{
				int count = this.grid.RowDefinitions.Count;

				if (count < this.strings.Count+1)
				{
					this.grid.RowDefinitions.Add(new RowDefinition());
					this.grid.RowDefinitions[count].TopBorder = -1;

					StaticText fix = new StaticText();
					fix.Text = count.ToString();
					fix.ContentAlignment = ContentAlignment.MiddleRight;
					fix.Margins = new Margins(0, 4, 0, 0);
					fix.VerticalAlignment = VerticalAlignment.BaseLine;
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

					TextField field = new TextField();
					field.TextChanged += new EventHandler(this.HandleTextChanged);
					field.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleTextFocusChanged);
					field.VerticalAlignment = VerticalAlignment.BaseLine;
					field.TabIndex = count;
					field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
					GridLayoutEngine.SetColumn(field, 2);
					GridLayoutEngine.SetRow(field, count);
					this.Children.Add(field);
					this.textFields.Add(field);
				}
				else
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

			System.Diagnostics.Debug.Assert(this.strings.Count == this.staticTexts.Count);
			System.Diagnostics.Debug.Assert(this.strings.Count == this.glyphButtons.Count);
			System.Diagnostics.Debug.Assert(this.strings.Count == this.textFields.Count);
		}

		protected void UpdateGrid()
		{
			//	Adapte le contenu des lignes éditables en fonction de la collection.
			for (int i=0; i<this.strings.Count; i++)
			{
				this.textFields[i].Text = this.strings[i];
			}
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons pour ajouter/supprimer/déplacer une ligne.
			int sel = this.SelectedRow;
			int count = this.textFields.Count;
			bool enable = (this.strings != null);

			this.buttonAdd.Enable = enable;
			this.buttonDuplicate.Enable = (enable && sel != -1);
			this.buttonRemove.Enable = (enable && sel != -1);
			this.buttonPrev.Enable = (enable && sel != -1 && sel > 0);
			this.buttonNext.Enable = (enable && sel != -1 && sel < count-1);
		}


		protected int SelectedRow
		{
			//	Ligne sélectionnée dans le tableau.
			get
			{
				if (this.selectedRow >= this.textFields.Count)
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

					//	Met à jour les flèches ">" dans les boutons de la 2ème colonne.
					for (int i=0; i<this.textFields.Count; i++)
					{
						this.glyphButtons[i].GlyphShape = (this.selectedRow == i) ? GlyphShape.ArrowRight : GlyphShape.None;
					}

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
		}


		protected void HandleButtonAddPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour créer une nouvelle ligne est cliqué.
			int sel = this.SelectedRow;
			if (sel == -1)
			{
				sel = this.strings.Count-1;
			}

			this.strings.Insert(sel+1, "");
			this.AdaptGrid();
			this.UpdateGrid();
			this.SelectedRow = sel+1;
			this.SetFocusInSelection();
		}

		protected void HandleButtonDuplicatePressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour dupliquer une ligne est cliqué.
			int sel = this.SelectedRow;
			if (sel == -1)
			{
				sel = this.strings.Count-1;
			}

			this.strings.Insert(sel+1, this.strings[sel]);
			this.AdaptGrid();
			this.UpdateGrid();
			this.SelectedRow = sel+1;
			this.SetFocusInSelection();
		}

		protected void HandleButtonRemovePressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour supprimer une ligne est cliqué.
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
		}

		protected void HandleButtonPrevPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour monter une ligne est cliqué.
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
			//	Appelé lorsque le bouton pour descendre une ligne est cliqué.
			int sel = this.SelectedRow;
			System.Diagnostics.Debug.Assert(sel != -1);

			string s = this.strings[sel];
			this.strings[sel] = this.strings[sel+1];
			this.strings[sel+1] = s;

			this.UpdateGrid();
			this.SelectedRow = sel+1;
			this.SetFocusInSelection();
		}

		protected void HandleButtonTextPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour sélectionner une ligne est cliqué.
			GlyphButton button = sender as GlyphButton;
			this.SelectedRow = glyphButtons.IndexOf(button);
		}

		void HandleTextChanged(object sender)
		{
			//	Appelé lorsqu'une ligne éditable a changé.
			TextField field = sender as TextField;
			int i = textFields.IndexOf(field);
			this.strings[i] = field.Text;
		}

		protected void HandleTextFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsqu'une ligne éditable voit son focus changer.
			TextField field = sender as TextField;
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.SelectedRow = textFields.IndexOf(field);
			}
		}


		protected List<string>				strings;
		protected GridLayoutEngine			grid;
		protected IconButton				buttonAdd;
		protected IconButton				buttonDuplicate;
		protected IconButton				buttonRemove;
		protected IconButton				buttonPrev;
		protected IconButton				buttonNext;
		protected List<StaticText>			staticTexts;
		protected List<GlyphButton>			glyphButtons;
		protected List<TextField>			textFields;
		protected int						selectedRow = -1;
	}
}
