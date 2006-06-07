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
		}
		
		public RibbonPage(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		static RibbonPage()
		{
			Helpers.VisualPropertyMetadata metadataHeight = new Helpers.VisualPropertyMetadata (RibbonPage.FixHeight, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataPadding = new Helpers.VisualPropertyMetadata (RibbonPage.FixPadding, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Visual.MinHeightProperty.OverrideMetadata (typeof (RibbonPage), metadataHeight);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (RibbonPage), metadataHeight);
			Visual.PaddingProperty.OverrideMetadata (typeof (RibbonPage), metadataPadding);
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


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine l'onglet.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;
			adorner.PaintRibbonPageBackground(graphics, rect, this.PaintState);
		}

		protected virtual void OnRankChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("RankChanged");
			if (handler != null)
			{
				handler (this);
			}
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


		public event EventHandler				RankChanged
		{
			add
			{
				this.AddUserEventHandler ("RankChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("RankChanged", value);
			}
		}
		
		private static readonly double			FixHeight = 3 + 14 + 8 + 22 + 5 + 22 + 3;
		private static readonly Drawing.Margins	FixPadding = new Margins(3, 3, 3, 3);

		private int								rank;
		private RibbonButton					ribbonButton;
		private RibbonSectionCollection			items;
	}
}
