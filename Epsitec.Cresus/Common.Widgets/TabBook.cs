namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for TabBook.
	/// </summary>
	public class TabBook : AbstractGroup
	{
		public TabBook()
		{
		}
		
		public override Drawing.Rectangle	Inside
		{
			get
			{
				double width  = this.Client.Width;
				double height = this.Client.Height - this.TabHeight;
				
				Drawing.Rectangle rect = new Drawing.Rectangle (0, 0, width, height);
				
				rect.Inflate (-2, -2);
				
				return rect;
			}
		}
		
		public Direction					Direction
		{
			get { return this.direction; }
		}
		
		public TabPage[]					Pages
		{
			get
			{
				TabPage[] pages = new TabPage[this.tab_pages.Count];
				this.tab_pages.CopyTo (pages);
				return pages;
			}
		}
		
		public TabPage						ActivePage
		{
			get { return this.active_page; }
			set
			{
				if (this.active_page != value)
				{
					this.active_page = value;
					this.Invalidate ();
				}
			}
		}
		
		public int							PageCount
		{
			get { return this.tab_pages.Count; }
		}
		
		public virtual double				TabHeight
		{
			get { return 25; }
		}
		
		
		public void Add(TabPage page)
		{
			System.Diagnostics.Debug.Assert (page != null);
			
			TabBook old_book = page.Book;
			
			if (old_book != null)
			{
				old_book.Remove (page);
			}
			
			page.Bounds = this.Inside;
			page.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			
			this.tab_pages.Add (page);
			this.Children.Add (page);
			
			page.RankChanged += new System.EventHandler (HandlePageRankChanged);
			
			this.SortPages ();
			this.UpdateVisiblePages ();
		}

		public void Remove(TabPage page)
		{
			int index = this.tab_pages.IndexOf (page);
			
			if (index < 0)
			{
				throw new System.IndexOutOfRangeException ("Page not found");
			}
			
			this.RemoveAt (index);
		}
		
		public void RemoveAt(int index)
		{
			TabPage page = this.tab_pages[index] as TabPage;
			
			this.tab_pages.RemoveAt (index);
			this.Children.Remove (page);
			
			if (this.active_page == page)
			{
				if (index >= this.tab_pages.Count)
				{
					index --;
				}
				
				if (index > 0)
				{
					this.active_page = this.tab_pages[index] as TabPage;
				}
				else
				{
					this.active_page = null;
				}
			}
			
			page.RankChanged -= new System.EventHandler (HandlePageRankChanged);
		}
		
		public void Clear()
		{
			while (this.tab_pages.Count > 0)
			{
				this.RemoveAt (0);
			}
		}
		
		
		public TabPage FindPage(int index)
		{
			return this.tab_pages[index] as TabPage;
		}
		
		public int FindPage(TabPage page)
		{
			return this.tab_pages.IndexOf (page);
		}
		
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			
			Direction new_dir = this.RootDirection;
			Direction old_dir = this.direction;
			
			if (old_dir != new_dir)
			{
				this.UpdateDirection (new_dir);
			}
		}
		
		protected virtual void UpdateDirection(Direction dir)
		{
			this.direction = dir;
			this.Invalidate ();
		}
		
		protected virtual void UpdateVisiblePages()
		{
			foreach (TabPage page in this.tab_pages)
			{
				page.SetVisible (page == this.active_page);
			}
		}
		
		protected virtual void SortPages()
		{
			this.tab_pages.Sort (new TabComparer ());
			this.Invalidate ();
		}
		
		protected virtual void HandlePageRankChanged(object sender, System.EventArgs e)
		{
			this.SortPages ();
		}
		
		
		
		
		class TabComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				TabPage page_1 = x as TabPage;
				TabPage page_2 = y as TabPage;
				
				return page_1.Rank.CompareTo (page_2.Rank);
			}
		}
		
		
		
		
		protected System.Collections.ArrayList	tab_pages = new System.Collections.ArrayList ();
		protected TabPage						active_page;
		protected Direction						direction;
	}
}
