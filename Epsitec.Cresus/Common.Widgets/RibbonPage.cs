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

		
		static RibbonPage()
		{
			Helpers.VisualPropertyMetadata metadataHeight = new Helpers.VisualPropertyMetadata(RibbonPage.FixHeight, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Visual.MinHeightProperty.OverrideMetadata(typeof(RibbonPage), metadataHeight);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(RibbonPage), metadataHeight);
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
				}
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


		protected static readonly double		FixHeight = 14 + 8 + 22 + 5 + 22;

		protected int							rank;
		protected RibbonButton					ribbonButton;
	}
}
