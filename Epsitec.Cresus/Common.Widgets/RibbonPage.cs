using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RibbonPage représente une page du RibbonBook.
	/// </summary>
	public class RibbonPage : AbstractGroup, Collections.IWidgetCollectionHost
	{
		public RibbonPage()
		{
			this.items = new RibbonSectionCollection(this);

			this.ribbonButton = new RibbonButton(null);
			this.ribbonButton.ContentAlignment = ContentAlignment.MiddleCenter;
			
			this.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.Padding = new Margins(3, 3, 3, 3);
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

		public RibbonSectionCollection			Items
		{
			get
			{
				return this.items;
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
			Rectangle rect = this.Client.Bounds;
			adorner.PaintRibbonPageBackground(graphics, rect, this.PaintState);
		}


		#region IWidgetCollectionHost Members
		Collections.WidgetCollection Collections.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}

		public void NotifyInsertion(Widget widget)
		{
			RibbonSection item = widget as RibbonSection;

			item.SetEmbedder(this);
			item.Margins = new Margins(0, 2, 0, 0);
			item.Dock = DockStyle.Left;
		}

		public void NotifyRemoval(Widget widget)
		{
			RibbonSection item = widget as RibbonSection;

			this.Children.Remove(item);
		}

		public void NotifyPostRemoval(Widget widget)
		{
		}
		#endregion

		#region RibbonSectionCollection Class
		public class RibbonSectionCollection : Collections.WidgetCollection
		{
			public RibbonSectionCollection(RibbonPage page) : base(page)
			{
				this.AutoEmbedding = true;
			}

			public new RibbonSection this[int index]
			{
				get
				{
					return base[index] as RibbonSection;
				}
			}

			public new RibbonSection this[string name]
			{
				get
				{
					return base[name] as RibbonSection;
				}
			}
		}
		#endregion

		
		protected static readonly double		FixHeight = 14 + 8 + 22 + 5 + 22;

		protected int							rank;
		protected RibbonButton					ribbonButton;
		protected RibbonSectionCollection		items;
	}
}
