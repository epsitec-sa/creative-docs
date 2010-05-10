using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// StyleCombo est un widget "combo" pour les styles graphique, de paragraphe ou de caractère.
	/// </summary>
	public class StyleCombo : TextFieldCombo
	{
		public StyleCombo()
		{
		}
		
		public StyleCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Document Document
		{
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}

		public StyleCategory StyleCategory
		{
			get
			{
				return this.styleCategory;
			}

			set
			{
				this.styleCategory = value;
			}
		}

		public int ExcludeRank
		{
			//	Ligne éventuelle à exclure.
			get
			{
				return this.excludeRank;
			}

			set
			{
				this.excludeRank = value;
			}
		}

		public bool IsDeep
		{
			//	Attributs cherchés en profondeur, dans les parents.
			get
			{
				return this.isDeep;
			}

			set
			{
				this.isDeep = value;
			}
		}

		public bool IsNoneLine
		{
			//	Première ligne avec <aucun>.
			get
			{
				return this.isNoneLine;
			}

			set
			{
				this.isNoneLine = value;
			}
		}

		public override int SelectedItemIndex
		{
			get
			{
				return this.selectedIndex;
			}

			set
			{
				this.selectedIndex = value;
			}
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				//	TODO: ...
			}
			
			base.Dispose(disposing);
		}


		protected override void Navigate(int dir)
		{
		}
		

		protected override AbstractMenu CreateMenu()
		{
			int count = 0;
			int sel = -1;
			Common.Text.TextStyle[] styles = null;

			if ( this.styleCategory == StyleCategory.Graphic )
			{
				count = this.document.Aggregates.Count;
				sel = this.GetSelectedStyle(this.document.Aggregates, this.Text);
			}
		
			if ( this.styleCategory == StyleCategory.Paragraph || this.styleCategory == StyleCategory.Character )
			{
				styles = this.document.TextStyles(this.styleCategory);
				count = styles.Length;
				sel = this.GetSelectedTextStyle(styles, this.Text);
			}

			if ( count == 0 )  return null;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Margins margins = adorner.GeometryArrayMargins;

			double width = 109+96+margins.Left+margins.Right;
			double h = count*32+1+margins.Bottom+margins.Top;

			if ( this.styleCategory == StyleCategory.Graphic )
			{
				AggregateList list = new AggregateList();
				list = new AggregateList();
				list.Document = this.document;
				list.List = this.document.Aggregates;
				list.ExcludeRank = this.excludeRank;
				list.IsHeader = false;
				list.IsNoneLine = this.isNoneLine;
				list.IsDeep = this.isDeep;
				list.HScroller = false;
				list.VScroller = false;
				list.IsHiliteColumn = false;
				list.SelectedRank = sel;
				list.FixWidth = width;
				this.list = list;
			}

			if ( this.styleCategory == StyleCategory.Paragraph || this.styleCategory == StyleCategory.Character )
			{
				TextStylesList list = new TextStylesList();
				list.Document = this.document;
				list.Category = this.styleCategory;
				list.List = styles;
				list.ExcludeRank = this.excludeRank;
				list.IsHeader = false;
				list.IsNoneLine = this.isNoneLine;
				list.IsDeep = this.isDeep;
				list.HScroller = false;
				list.VScroller = false;
				list.IsHiliteColumn = false;
				list.SelectedRank = sel;
				list.FixWidth = width;
				this.list = list;
			}
			
			TextFieldComboMenu menu = new TextFieldComboMenu();
			menu.Contents = this.list;
			menu.AdjustSize();
			
			//	On n'a pas le droit de définir le "SelectedFontFace" avant d'avoir fait
			//	cette mise à jour du contenu avec la nouvelle taille ajustée, sinon on
			//	risque d'avoir un offset incorrect pour le début...
			this.list.UpdateContents();
			this.list.FinalSelectionChanged += this.HandleListSelectionActivated;
			
			MenuItem.SetMenuHost(this, new StyleMenuHost(menu, this.list));

			return menu;
		}

		protected override void OnComboClosed()
		{
			base.OnComboClosed();
			
			if ( this.list != null )
			{
				this.list.FinalSelectionChanged -= this.HandleListSelectionActivated;
				this.list.Dispose();
				this.list = null;
			}

			if ( this.Window != null )
			{
				this.Window.RestoreLogicalFocus();
			}
		}

		private void HandleListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.
			int sel = this.MapComboListToIndex(this.list.SelectedRow);
			if ( sel == -1 )  return;
			this.list.SelectRow(sel, true);

			int rank = this.list.RowToRank(this.list.SelectedRow);
			if ( rank == -1 )  rank = -2;  // ligne <aucun> ?
			this.selectedIndex = rank;

			this.CloseCombo(CloseMode.Accept);
		}


		protected int GetSelectedStyle(UndoableList aggregates, string currentStyle)
		{
			//	Cherche le rang du style graphique actuellement en édition.
			for ( int i=0 ; i<aggregates.Count ; i++ )
			{
				Properties.Aggregate aggregate = aggregates[i] as Properties.Aggregate;

				if ( aggregate.AggregateName == currentStyle )
				{
					return i;
				}
			}

			return -1;
		}

		protected int GetSelectedTextStyle(Text.TextStyle[] styles, string currentStyle)
		{
			//	Cherche le rang du style de texte actuellement en édition.
			for ( int i=0 ; i<styles.Length ; i++ )
			{
				Text.TextStyle style = styles[i];

				if ( Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(style)) == currentStyle )
				{
					return i;
				}
			}

			return -1;
		}
		
		
		#region StyleMenuHost Class
		public class StyleMenuHost : IMenuHost
		{
			public StyleMenuHost(AbstractMenu menu, AbstractStyleList list)
			{
				this.menu = menu;
				this.list = list;
			}
			
			
			public void GetMenuDisposition(Widget item, ref Drawing.Size size, out Drawing.Point location, out Animation animation)
			{
				//	Détermine la hauteur maximale disponible par rapport à la position
				//	actuelle :
				
				Drawing.Point pos = Common.Widgets.Helpers.VisualTree.MapVisualToScreen(item, new Drawing.Point(0, 1));
				Drawing.Point hot = Common.Widgets.Helpers.VisualTree.MapVisualToScreen(item, new Drawing.Point(0, 1));
				ScreenInfo screenInfo = ScreenInfo.Find(hot);
				Drawing.Rectangle workingArea = screenInfo.WorkingArea;
				
				double maxHeight = pos.Y - workingArea.Bottom;
				double w = size.Width;
				double h = size.Height;

				animation = Animation.RollDown;
				if ( h > maxHeight )  // dépasse en bas ?
				{
					if ( maxHeight > 100 )  // place minimale ?
					{
						h = maxHeight;
						this.list.VScroller = true;
						w += 16;
					}
					else	// déroule contre le haut ?
					{
						pos = Common.Widgets.Helpers.VisualTree.MapVisualToScreen(item, new Drawing.Point(0, item.ActualHeight-1));
						maxHeight = workingArea.Top-pos.Y;
						if ( h > maxHeight )  // dépasse en haut ?
						{
							h = maxHeight;
							this.list.VScroller = true;
							w += 16;
						}
						pos.Y += h;
						animation = Animation.RollUp;
					}
				}
				pos.Y -= h;

				if ( pos.X+w > workingArea.Right )  // dépasse à droite ?
				{
					pos.X = workingArea.Right-w;
				}

				location = pos;
				size = new Size(w, h);
			}
			
			private AbstractMenu				menu;
			private AbstractStyleList			list;
		}
		#endregion
		
		
		protected Document						document;
		protected StyleCategory					styleCategory = StyleCategory.Graphic;
		protected int							excludeRank = -1;
		protected bool							isDeep = false;
		protected bool							isNoneLine = false;
		protected AbstractStyleList				list;
		protected int							selectedIndex = -1;
	}
}
