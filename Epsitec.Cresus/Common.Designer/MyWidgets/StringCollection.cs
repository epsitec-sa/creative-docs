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

			this.grid = new GridLayoutEngine();
			this.grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(20, GridUnitType.Absolute)));
			this.grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Proportional)));
			LayoutEngine.SetLayoutEngine(this, this.grid);

			this.staticTexts = new List<StaticText>();
			this.textFields = new List<TextField>();
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

				if (this.grid.RowCount != this.collection.Count)
				{
					this.AdaptGrid();
				}

				this.UpdateGrid();
			}
		}


		protected void AdaptGrid()
		{
			//	Adapte le nombre de lignes du tableau en fonction de la collection.
			while (this.grid.RowDefinitions.Count != this.collection.Count)
			{
				int count = this.grid.RowDefinitions.Count;

				if (count < this.collection.Count)
				{
					this.grid.RowDefinitions.Add(new RowDefinition());

					if (count > 0)
					{
						this.grid.RowDefinitions[count].TopBorder = -1;
					}

					StaticText fix = new StaticText();
					fix.Text = (count+1).ToString();
					fix.ContentAlignment = ContentAlignment.MiddleRight;
					fix.Margins = new Margins(0, 4, 0, 0);
					fix.VerticalAlignment = VerticalAlignment.BaseLine;
					GridLayoutEngine.SetColumn(fix, 0);
					GridLayoutEngine.SetRow(fix, count);
					this.Children.Add(fix);
					this.staticTexts.Add(fix);

					TextField field = new TextField();
					field.VerticalAlignment = VerticalAlignment.BaseLine;
					GridLayoutEngine.SetColumn(field, 1);
					GridLayoutEngine.SetRow(field, count);
					this.Children.Add(field);
					this.textFields.Add(field);
				}
				else
				{
					this.Children.Remove(this.staticTexts[count-1]);
					this.Children.Remove(this.textFields[count-1]);

					this.grid.RowDefinitions.RemoveAt(count-1);
					this.staticTexts.RemoveAt(count-1);
					this.textFields.RemoveAt(count-1);
				}
			}
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



		protected ICollection<string>		collection;
		protected GridLayoutEngine			grid;
		protected List<StaticText>			staticTexts;
		protected List<TextField>			textFields;
	}
}
