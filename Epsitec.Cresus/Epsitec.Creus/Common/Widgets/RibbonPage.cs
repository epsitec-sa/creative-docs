using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RibbonPage représente une page du RibbonBook.
	/// </summary>
	public class RibbonPage : AbstractGroup, Collections.IWidgetCollectionHost<RibbonSection>
	{
		public RibbonPage()
		{
			this.items = new RibbonSectionCollection (this);
			this.ribbonButton = new RibbonButton (null);
		}

		public RibbonPage(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public RibbonPage(RibbonBook book)
			: this ()
		{
			book.Pages.Add (this);
		}


		static RibbonPage()
		{
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataPadding = Visual.PaddingProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (RibbonPage.FixHeight);
			metadataPadding.DefineDefaultValue (RibbonPage.FixPadding);

			Visual.MinHeightProperty.OverrideMetadata (typeof (RibbonPage), metadataDy);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (RibbonPage), metadataDy);
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
			adorner.PaintRibbonPageBackground (graphics, rect, this.GetPaintState ());
		}

		protected virtual void OnRankChanged()
		{
			var handler = this.GetUserEventHandler ("RankChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		#region IWidgetCollectionHost Members
		Collections.WidgetCollection<RibbonSection> Collections.IWidgetCollectionHost<RibbonSection>.GetWidgetCollection()
		{
			return this.Items;
		}

		public void NotifyInsertion(RibbonSection item)
		{
			item.SetEmbedder(this);
			item.Margins = new Margins(0, 2, 0, 0);
			item.Dock = DockStyle.Left;
		}

		public void NotifyRemoval(RibbonSection item)
		{
			this.Children.Remove(item);
		}

		public void NotifyPostRemoval(RibbonSection item)
		{
		}
		#endregion

		#region RibbonSectionCollection Class
		public class RibbonSectionCollection : Collections.WidgetCollection<RibbonSection>
		{
			public RibbonSectionCollection(RibbonPage page) : base(page)
			{
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
