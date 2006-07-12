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

			this.textFields = new List<TextField>();
		}

		public StringCollection(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public List<string> List
		{
			get
			{
				return this.list;
			}
			set
			{
				this.list = value;

				if (this.grid.RowCount != this.list.Count)
				{
					this.AdaptGrid();
				}

				this.UpdateGrid();
			}
		}


		protected void AdaptGrid()
		{
			while (this.grid.RowCount != this.list.Count)
			{
				int count = this.grid.RowCount;

				if (count < this.list.Count)
				{
					this.grid.RowDefinitions.Add(new RowDefinition());

					StaticText fix = new StaticText();
					fix.Text = (count+1).ToString();
					GridLayoutEngine.SetColumn(fix, 0);
					GridLayoutEngine.SetRow(fix, count);
					this.Children.Add(fix);

					TextField field = new TextField();
					GridLayoutEngine.SetColumn(field, 1);
					GridLayoutEngine.SetRow(field, count);
					this.Children.Add(field);
					this.textFields.Add(field);
				}
				else
				{
					this.grid.RowDefinitions.RemoveAt(count-1);
					this.textFields.RemoveAt(count-1);
				}
			}
		}

		protected void UpdateGrid()
		{
			for (int i=0; i<this.list.Count; i++)
			{
				this.textFields[i].Text = this.list[i];
			}
		}



		protected List<string>				list;
		protected GridLayoutEngine			grid;
		protected List<TextField>			textFields;
	}
}
