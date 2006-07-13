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
		}

		public StringCollection(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public ICollection<string> Collection
		{
			get
			{
				return this.collection;
			}
			set
			{
				this.collection = value;

				this.AdaptGrid();
				this.UpdateGrid();
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

			this.buttonRemove = new IconButton(toolbar);
			this.buttonRemove.IconName = Misc.Icon("StringRemove");
			this.buttonRemove.Dock = DockStyle.Left;

			IconSeparator sep = new IconSeparator(toolbar);
			sep.IsHorizontal = true;
			sep.Dock = DockStyle.Left;

			this.buttonPrev = new IconButton(toolbar);
			this.buttonPrev.IconName = Misc.Icon("StringPrev");
			this.buttonPrev.Dock = DockStyle.Left;

			this.buttonNext = new IconButton(toolbar);
			this.buttonNext.IconName = Misc.Icon("StringNext");
			this.buttonNext.Dock = DockStyle.Left;
		}

		protected void AdaptGrid()
		{
			//	Adapte le nombre de lignes du tableau en fonction de la collection.
			while (this.grid.RowDefinitions.Count != this.collection.Count+1)
			{
				int count = this.grid.RowDefinitions.Count;

				if (count < this.collection.Count+1)
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
					button.Pressed += new MessageEventHandler(this.HandleButtonPressed);
					GridLayoutEngine.SetColumn(button, 1);
					GridLayoutEngine.SetRow(button, count);
					this.Children.Add(button);
					this.glyphButtons.Add(button);

					TextField field = new TextField();
					field.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleTextFocusChanged);
					field.VerticalAlignment = VerticalAlignment.BaseLine;
					GridLayoutEngine.SetColumn(field, 2);
					GridLayoutEngine.SetRow(field, count);
					this.Children.Add(field);
					this.textFields.Add(field);
				}
				else
				{
					this.glyphButtons[count-2].Pressed -= new MessageEventHandler(this.HandleButtonPressed);
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

			System.Diagnostics.Debug.Assert(this.collection.Count == this.staticTexts.Count);
			System.Diagnostics.Debug.Assert(this.collection.Count == this.glyphButtons.Count);
			System.Diagnostics.Debug.Assert(this.collection.Count == this.textFields.Count);
		}

		protected void UpdateGrid()
		{
			//	Adapte le contenu des lignes éditables en fonction de la collection.
			string[] array = new string[this.collection.Count];
			this.collection.CopyTo(array, 0);

			for (int i=0; i<array.Length; i++ )
			{
				this.textFields[i].Text = array[i];
			}
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
				}
			}
		}

		protected void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;
			this.SelectedRow = glyphButtons.IndexOf(button);
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


		protected ICollection<string>		collection;
		protected GridLayoutEngine			grid;
		protected IconButton				buttonAdd;
		protected IconButton				buttonRemove;
		protected IconButton				buttonPrev;
		protected IconButton				buttonNext;
		protected List<StaticText>			staticTexts;
		protected List<GlyphButton>			glyphButtons;
		protected List<TextField>			textFields;
		protected int						selectedRow = -1;
	}
}
