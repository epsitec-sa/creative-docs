using System;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TabPage représente une page du TabBook.
	/// </summary>
	public class TabPage : AbstractGroup
	{
		public TabPage()
		{
		}
		
		public string						Title
		{
			get { return this.Text; }
			set { this.Text = value; }
		}
		
		public TabBook						Book
		{
			get
			{
				TabBook book = this.Parent as TabBook;
				return book;
			}
		}
		
		public Direction					Direction
		{
			get
			{
				TabBook book = this.Book;
				
				if (book == null)
				{
					return Direction.None;
				}
				
				return book.Direction;
			}
		}
		public int							Rank
		{
			get { return this.rank; }
			set
			{
				if (this.rank != value)
				{
					this.rank = value;
					this.OnRankChanged (System.EventArgs.Empty);
				}
			}
		}
		
		
		public event System.EventHandler	RankChanged;
		
		
		
		protected virtual void OnRankChanged(System.EventArgs e)
		{
			if (this.RankChanged !=  null)
			{
				this.RankChanged (this, e);
			}
		}
		
		
		protected int					rank;
	}
}
