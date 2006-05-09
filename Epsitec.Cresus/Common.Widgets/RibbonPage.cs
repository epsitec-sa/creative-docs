using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RibbonPage représente une page du RibbonBook.
	/// </summary>
	public class RibbonPage : AbstractGroup
	{
		public RibbonPage()
		{
			this.ribbonButton = new RibbonButton(null);
			this.ribbonButton.ContentAlignment = ContentAlignment.MiddleCenter;
			
			this.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
		}
		
		public RibbonPage(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public string							RibbonTitle
		{
			get
			{
				return this.ribbonButton.Text;
			}

			set
			{
				if (this.ribbonButton.Text != value)
				{
					this.ribbonButton.Text = value;
					if ( this.Book != null )
					{
						this.Book.UpdateButtons();
					}
				}
			}
		}

		public Rectangle						TabBounds
		{
			get
			{
				return this.ribbonButton.ActualBounds;
			}

			set
			{
				this.ribbonButton.SetManualBounds(value);
			}
		}

		public Size								TabSize
		{
			get
			{
				TextLayout tl = new TextLayout(this.ResourceManager);
				tl.Text = this.ribbonButton.Text;
				return tl.SingleLineSize;
			}
		}
		
		public RibbonButton						RibbonButton
		{
			get
			{
				return this.ribbonButton;
			}
		}
		
		public RibbonBook						Book
		{
			get
			{
				RibbonBook book = this.Parent as RibbonBook;
				return book;
			}
		}
		
		public Direction						Direction
		{
			get
			{
				RibbonBook book = this.Book;
				
				if ( book == null )
				{
					return Direction.None;
				}
				
				return book.Direction;
			}
		}
		public int								Rank
		{
			get
			{
				return this.rank;
			}

			set
			{
				if ( this.rank != value )
				{
					this.rank     = value;
					this.TabIndex = this.rank;
					this.OnRankChanged();
				}
			}
		}
		
		
		public event EventHandler				RankChanged
		{
			add
			{
				this.AddUserEventHandler("RankChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("RankChanged", value);
			}
		}

		
		protected virtual void OnRankChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("RankChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine l'onglet.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
		}
		

		protected int							rank;
		protected RibbonButton					ribbonButton;
	}
}
