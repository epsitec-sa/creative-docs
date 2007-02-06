//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemTable))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemTable</c> class is used to represent items in a table, with
	/// column headers and scroll bars.
	/// </summary>
	public class ItemTable : Widgets.FrameBox, IListHost<ItemTableColumn>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ItemTable"/> class.
		/// </summary>
		public ItemTable()
		{
			this.vScroller = new VScroller (this);
			this.hScroller = new HScroller (this);
			this.surface = new Widget (this);
			this.headerStripe = new Widget (this);
			this.columnHeader = new ItemPanelColumnHeader (this.headerStripe);
			this.itemPanel = new ItemPanel (this.surface);

			this.vScroller.IsInverted = true;
			this.vScroller.Value = 0;
			
			this.vScroller.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			this.hScroller.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			this.headerStripe.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.surface.Anchor = AnchorStyles.All;

			this.headerStripe.PreferredHeight = this.columnHeader.PreferredHeight;

			this.UpdateGeometry ();

			this.hScroller.ValueChanged += this.HandleScrollerValueChanged;
			this.vScroller.ValueChanged += this.HandleScrollerValueChanged;
			this.surface.SizeChanged += this.HandleSurfaceSizeChanged;
			this.itemPanel.ApertureChanged += this.HandleItemPanelApertureChanged;
			this.itemPanel.ContentsSizeChanged += this.HandleContentsSizeChanged;
			this.itemPanel.CurrentChanged += this.HandleItemPanelCurrentChanged;
			
			this.itemPanel.AddEventHandler (Visual.PreferredHeightProperty, this.HandleItemPanelSizeChanged);
			this.itemPanel.AddEventHandler (Visual.PreferredWidthProperty, this.HandleItemPanelSizeChanged);

			this.itemPanel.Layout = ItemPanelLayout.VerticalList;
			this.itemPanel.ItemSelection = ItemPanelSelectionMode.ExactlyOne;
			this.itemPanel.GroupSelection = ItemPanelSelectionMode.None;

			this.columnHeader.ItemPanel = this.itemPanel;

			//	Link the item panel with its table, so that an ItemViewFactory
			//	can find the table and the column templates, if it needs to do
			//	so :
			
			ItemTable.SetItemTable (this.itemPanel, this);
		}

		private void UpdateGeometry()
		{
			double topMargin    = this.headerStripe.Visibility ? this.headerStripe.PreferredHeight : 0;
			double bottomMargin = this.hScroller.Visibility ? this.hScroller.PreferredHeight : 0;
			double rightMargin  = this.vScroller.Visibility ? this.vScroller.PreferredWidth : 0;
			double frameMargin  = this.FrameVisibility ? 1 : 0;

			this.headerStripe.Margins = new Drawing.Margins (0, rightMargin, 0, 0);
			this.vScroller.Margins = new Drawing.Margins (0, 0, topMargin, bottomMargin);
			this.hScroller.Margins = new Drawing.Margins (0, rightMargin, 0, 0);
			this.surface.Margins = new Drawing.Margins (frameMargin, rightMargin+frameMargin, topMargin+frameMargin, bottomMargin+frameMargin);
		}

		public ItemTable(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public ICollectionView					Items
		{
			get
			{
				return this.ItemPanel.Items;
			}
			set
			{
				this.ItemPanel.Items = value;
			}
		}

		public ItemPanel						ItemPanel
		{
			get
			{
				return this.itemPanel;
			}
		}

		public ItemPanelColumnHeader			ColumnHeader
		{
			get
			{
				return this.columnHeader;
			}
		}

		public bool								HeaderVisibility
		{
			get
			{
				return this.headerStripe.Visibility;
			}
			set
			{
				if (this.headerStripe.Visibility != value)
				{
					this.headerStripe.Visibility = value;
					this.UpdateGeometry ();
				}
			}
		}

		public ItemTableScrollMode				HorizontalScrollMode
		{
			get
			{
				return (ItemTableScrollMode) this.GetValue (ItemTable.HorizontalScrollModeProperty);
			}
			set
			{
				this.SetValue (ItemTable.HorizontalScrollModeProperty, value);
			}
		}

		public ItemTableScrollMode				VerticalScrollMode
		{
			get
			{
				return (ItemTableScrollMode) this.GetValue (ItemTable.VerticalScrollModeProperty);
			}
			set
			{
				this.SetValue (ItemTable.VerticalScrollModeProperty, value);
			}
		}

		public bool								FrameVisibility
		{
			get
			{
				return (bool) this.GetValue (ItemTable.FrameVisibilityProperty);
			}
			set
			{
				this.SetValue (ItemTable.FrameVisibilityProperty, value);
			}
		}

		public StructuredType					SourceType
		{
			get
			{
				return this.sourceType;
			}
			set
			{
				if (this.sourceType != value)
				{
					this.sourceType = value;
					this.UpdateColumnHeader ();
				}
			}
		}

		public Collections.ItemTableColumnCollection Columns
		{
			get
			{
				if (this.columns == null)
				{
					this.columns = new Collections.ItemTableColumnCollection (this);
				}
				
				return this.columns;
			}
		}

		public void DefineDefaultColumns(StructuredType sourceType, double width)
		{
			System.Threading.Interlocked.Increment (ref this.suspendColumnUpdates);
			
			try
			{
				this.SourceType = sourceType;
				this.Columns.Clear ();
				
				foreach (string fieldId in sourceType.GetFieldIds ())
				{
					this.Columns.Add (new ItemTableColumn (fieldId, width));
				}
			}
			finally
			{
				System.Threading.Interlocked.Decrement (ref this.suspendColumnUpdates);
			}

			this.UpdateColumnHeader ();
		}

		public Drawing.Size GetDefaultItemSize(ItemView itemView)
		{
			return this.defaultItemSize;
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			Widgets.IAdorner adorner = Widgets.Adorners.Factory.Active;

			if (!this.BackColor.IsEmpty)
			{
				Drawing.Rectangle rect = Drawing.Rectangle.Intersection (clipRect, this.Client.Bounds);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}

			if (this.FrameVisibility)
			{
				double x1 = 0;
				double x2 = this.vScroller.Visibility ? this.vScroller.ActualBounds.Left-1 : this.Client.Bounds.Right-1;
				double y1 = this.hScroller.Visibility ? this.hScroller.ActualBounds.Top : this.Client.Bounds.Bottom;
				double y2 = this.headerStripe.Visibility ? this.headerStripe.ActualBounds.Bottom-1 : this.Client.Bounds.Top-1;

				graphics.AddLine (x1+0.5, y1+0.5, x1+0.5, y2+0.5);  // trait vertical gauche
				graphics.AddLine (x2+0.5, y1+0.5, x2+0.5, y2+0.5);  // trait vertical droite
				graphics.AddLine (x1+0.5, y2+0.5, x2+0.5, y2+0.5);  // trait horizontal supérieur
				graphics.AddLine (x1+0.5, y1+0.5, x2+0.5, y1+0.5);  // trait horizontal inférieur
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}

		public Drawing.Margins GetPanelPadding()
		{
			return this.surface.Margins;
		}

		private void UpdateApertureProtected()
		{
			if (this.suspendScroll == 0)
			{
				System.Threading.Interlocked.Increment (ref this.suspendScroll);

				try
				{
					this.UpdateAperture ();
				}
				finally
				{
					System.Threading.Interlocked.Decrement (ref this.suspendScroll);
				}
			}
		}

		private void UpdateAperture()
		{
			Drawing.Margins padding = Drawing.Margins.Zero;

			this.aperture = Drawing.Rectangle.Deflate (this.Client.Bounds, this.GetPanelPadding ()).Size;

			Drawing.Size scrollSize = this.GetScrollSize (this.aperture);

			this.UpdateScrollRatios ();

			switch (this.VerticalScrollMode)
			{
				case ItemTableScrollMode.Linear:
					if (scrollSize.Height > 0)
					{
						this.vScroller.SmallChange = (decimal) (this.itemPanel.ItemViewDefaultSize.Height / scrollSize.Height);
						this.vScroller.LargeChange = (decimal) (aperture.Height / scrollSize.Height);
					}
					break;

				case ItemTableScrollMode.ItemBased:
					this.vScroller.SmallChange = 1;
					this.vScroller.LargeChange = this.itemPanel.GetVisibleLineCount (this.aperture.Height);
					this.vScroller.MinValue = 0;
					this.vScroller.MaxValue = System.Math.Max (0, this.itemPanel.GetTotalLineCount () - this.vScroller.LargeChange);
					padding.Bottom = this.aperture.Height - this.itemPanel.GetLineHeight () * (double) (this.vScroller.LargeChange);
					break;
			}

			if (scrollSize.Width > 0)
			{
				this.hScroller.SmallChange = (decimal) (aperture.Width * 0.2 / scrollSize.Width);
				this.hScroller.LargeChange = (decimal) (aperture.Width / scrollSize.Width);
			}
			
			double aW = aperture.Width;
			double aH = aperture.Height;

			double ox = System.Math.Floor ((double) this.hScroller.Value * scrollSize.Width);
			double oy = this.GetVerticalScrollOffset ();

			this.itemPanel.Aperture = new Drawing.Rectangle (ox+1, oy, aW, aH);
			this.itemPanel.AperturePadding = padding;

			if (this.itemPanel.PreferredHeight < aH)
			{
				oy = this.itemPanel.PreferredHeight - aH;
			}

			double dx = System.Math.Max (this.itemPanel.PreferredWidth, aW);
			double dy = System.Math.Max (this.itemPanel.PreferredHeight, aH);

			this.itemPanel.SetManualBounds (new Drawing.Rectangle (-ox, -oy, dx, dy));
			this.columnHeader.SetManualBounds (new Drawing.Rectangle (-ox, 0, this.columnHeader.GetTotalWidth (), this.columnHeader.PreferredHeight));
		}

		private void UpdateScrollRatios()
		{
			double hRatio = System.Math.Max (0, System.Math.Min (1, (aperture.Width+1) / (this.itemPanel.PreferredWidth+1)));
			double vRatio = System.Math.Max (0, System.Math.Min (1, (aperture.Height+1) / (this.itemPanel.PreferredHeight+1)));

			this.hScroller.VisibleRangeRatio = (decimal) hRatio;
			this.vScroller.VisibleRangeRatio = (decimal) vRatio;
		}

		private double GetVerticalScrollOffset()
		{
			switch (this.VerticalScrollMode)
			{
				case ItemTableScrollMode.Linear:
					return this.GetVerticalScrollOffsetLinear ();

				case ItemTableScrollMode.ItemBased:
					return this.GetVerticalScrollOffsetItemBased ();
			}

			return 0;
		}

		private double GetVerticalScrollOffsetLinear()
		{
			double dist = (double) (this.vScroller.Value) * this.GetScrollHeight ();
			double pos = this.itemPanel.GetContentsSize ().Height - dist;

			return pos - this.aperture.Height;
		}

		private void SetVerticalScrollValueLinear(double offset)
		{
			double height = this.GetScrollHeight ();
			
			if (height > 0)
			{
				double pos = offset + this.aperture.Height;
				double dist = this.itemPanel.GetContentsSize ().Height - pos;
				this.vScroller.Value = (decimal) (dist / height);
			}
		}

		private double GetVerticalScrollOffsetItemBased()
		{
			int index = (int) this.vScroller.Value;
			int total = this.itemPanel.GetTotalLineCount ();

			index = System.Math.Min (total-1, index);
			index = System.Math.Max (0, index);
			
			ItemView view = this.itemPanel.GetItemView (index);
			double   pos  = (view == null) ? this.itemPanel.GetContentsSize ().Height : view.Bounds.Top;
			
			return pos - this.aperture.Height;
		}

		private void SetVerticalScrollValueItemBased(double offset)
		{
			double pos = offset + this.aperture.Height - 0.5;
			
			ItemView view = this.itemPanel.FindItemView (
				delegate (ItemView item)
				{
					Drawing.Rectangle bounds = item.Bounds;
					
					if ((bounds.Bottom < pos) &&
						(bounds.Top > pos))
					{
						return true;
					}

					return false;
				});

			if (view != null)
			{
				this.vScroller.Value = view.Index;
			}
		}

		private double GetScrollWidth()
		{
			Drawing.Size size = this.itemPanel.GetContentsSize ();
			return System.Math.Max (0, size.Width - this.aperture.Width);
		}

		private double GetScrollHeight()
		{
			Drawing.Size size = this.itemPanel.GetContentsSize ();
			return System.Math.Max (0, size.Height - this.aperture.Height);
		}

		private Drawing.Size GetScrollSize(Drawing.Size aperture)
		{
			Drawing.Size panelSize = this.itemPanel.GetContentsSize ();

			double dx = System.Math.Max (0, panelSize.Width  - aperture.Width);
			double dy = System.Math.Max (0, panelSize.Height - aperture.Height);

			return new Drawing.Size (dx, dy);
		}

		private void UpdateColumnHeader()
		{
			if ((this.suspendColumnUpdates == 0) &&
				(this.columns != null) &&
				(this.columns.Count > 0) &&
				(this.sourceType != null))
			{
				Support.ResourceManager manager = Widgets.Helpers.VisualTree.FindResourceManager (this);
				
				this.columnHeader.ClearColumns ();

				double minWidth  = 0;
				double minHeight = 0;

				int headerIndex = 0;

				for (int i = 0; i < this.columns.Count; i++)
				{
					ItemTableColumn column = this.columns[i];
					
					if (string.IsNullOrEmpty (column.FieldId))
					{
						//	There is no field defined for this column. This means
						//	that the contents of the column will default to the
						//	full item itself.
						
						Support.Druid captionId = column.CaptionId;

						if (captionId.IsValid)
						{
							this.columnHeader.AddColumn (i, null, captionId);
						}
						else
						{
							continue;
						}
					}
					else
					{
						StructuredTypeField field = this.sourceType.GetField (column.FieldId);
						
						if (field.IsEmpty)
						{
							continue;
						}
						else
						{
							Support.Druid captionId = column.CaptionId;

							if (captionId.IsEmpty)
							{
								this.columnHeader.AddColumn (i, field);
							}
							else
							{
								this.columnHeader.AddColumn (i, field.Id, captionId);
							}
						}
					}

					Drawing.Size size = this.itemPanel.ItemViewDefaultSize;

					if (column.TemplateId.IsValid)
					{
						size = Panel.GetPanelDefaultSize (column.TemplateId, manager);
					}

					double width = column.Width.IsAbsolute ? column.Width.Value : size.Width;

					this.columnHeader.SetColumnWidth (headerIndex, width);
					this.columnHeader.SetColumnComparer (headerIndex, column.Comparer);

					minWidth += width;
					minHeight = System.Math.Max (minHeight, size.Height);
					headerIndex += 1;
				}

				this.defaultItemSize = new Drawing.Size (minWidth, minHeight);

				this.itemPanel.AsyncRefresh ();
			}
		}

		private void HandleSurfaceSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateApertureProtected ();
		}

		private void HandleScrollerValueChanged(object sender)
		{
			this.UpdateApertureProtected ();
		}

		private void HandleItemPanelSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateApertureProtected ();
		}
		
		private void HandleContentsSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateApertureProtected ();
		}

		private void HandleItemPanelApertureChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.SynchronizeScrollers ((Drawing.Rectangle) e.NewValue);
		}

		private void HandleItemPanelCurrentChanged(object sender)
		{
		}

		private void SynchronizeScrollers(Drawing.Rectangle value)
		{
			if (this.suspendScroll == 0)
			{
				Drawing.Size scrollSize = this.GetScrollSize (this.aperture);

				double sx = scrollSize.Width;
				double sy = scrollSize.Height;
				double ox = value.Left - 1;
				double oy = value.Top;

				System.Threading.Interlocked.Increment (ref this.suspendScroll);

				try
				{
					if (sx > 0)
					{
						//this.hScroller.Value = (decimal) (ox / sx);
					}
					if (sy > 0)
					{
						switch (this.VerticalScrollMode)
						{
							case ItemTableScrollMode.Linear:
								this.SetVerticalScrollValueLinear (value.Bottom);
								break;
							case ItemTableScrollMode.ItemBased:
								this.SetVerticalScrollValueItemBased (value.Bottom);
								break;
						}
					}

					this.UpdateAperture ();
				}
				finally
				{
					System.Threading.Interlocked.Decrement (ref this.suspendScroll);
				}
			}
		}

		private void OnFrameVisibilityChanged(DependencyPropertyChangedEventArgs e)
		{
 			this.UpdateGeometry ();
		}

		private void OnScrollModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.hScroller.Visibility = this.HorizontalScrollMode != ItemTableScrollMode.None;
			this.vScroller.Visibility = this.VerticalScrollMode != ItemTableScrollMode.None;
			
			this.UpdateGeometry ();
		}


		
		#region IListHost<ItemTableColumn> Members

		HostedList<ItemTableColumn> IListHost<ItemTableColumn>.Items
		{
			get
			{
				return this.Columns;
			}
		}

		void IListHost<ItemTableColumn>.NotifyListInsertion(ItemTableColumn item)
		{
			this.UpdateColumnHeader ();
		}

		void IListHost<ItemTableColumn>.NotifyListRemoval(ItemTableColumn item)
		{
			this.UpdateColumnHeader ();
		}

		#endregion


		public static void SetItemTable(DependencyObject obj, ItemTable value)
		{
			if (value == null)
			{
				obj.ClearValue (ItemTable.ItemTableProperty);
			}
			else
			{
				obj.SetValue (ItemTable.ItemTableProperty, value);
			}
		}

		public static ItemTable GetItemTable(DependencyObject obj)
		{
			return (ItemTable) obj.GetValue (ItemTable.ItemTableProperty);
		}

		private static void NotifyFrameVisibilityChanged(DependencyObject o, object oldValue, object newValue)
		{
			ItemTable that = o as ItemTable;
			that.OnFrameVisibilityChanged (new DependencyPropertyChangedEventArgs (ItemTable.FrameVisibilityProperty, oldValue, newValue));
		}

		private static void NotifyHorizontalScrollModeChanged(DependencyObject o, object oldValue, object newValue)
		{
			ItemTable that = o as ItemTable;
			that.OnScrollModeChanged (new DependencyPropertyChangedEventArgs (ItemTable.HorizontalScrollModeProperty, oldValue, newValue));
		}

		private static void NotifyVerticalScrollModeChanged(DependencyObject o, object oldValue, object newValue)
		{
			ItemTable that = o as ItemTable;
			that.OnScrollModeChanged (new DependencyPropertyChangedEventArgs (ItemTable.VerticalScrollModeProperty, oldValue, newValue));
		}

		public static readonly DependencyProperty ItemTableProperty = DependencyProperty.RegisterAttached ("ItemTable", typeof (ItemTable), typeof (ItemTable));
		
		public static readonly DependencyProperty FrameVisibilityProperty = DependencyProperty.Register ("FrameVisibility", typeof (bool), typeof (ItemTable), new DependencyPropertyMetadata (true, ItemTable.NotifyFrameVisibilityChanged));
		public static readonly DependencyProperty VerticalScrollModeProperty = DependencyProperty.Register ("VerticalScrollMode", typeof (ItemTableScrollMode), typeof (ItemTable), new DependencyPropertyMetadata (ItemTableScrollMode.Linear, ItemTable.NotifyVerticalScrollModeChanged));
		public static readonly DependencyProperty HorizontalScrollModeProperty = DependencyProperty.Register ("HorizontalScrollMode", typeof (ItemTableScrollMode), typeof (ItemTable), new DependencyPropertyMetadata (ItemTableScrollMode.Linear, ItemTable.NotifyHorizontalScrollModeChanged));

		private VScroller vScroller;
		private HScroller hScroller;
		private ItemPanelColumnHeader columnHeader;
		private Collections.ItemTableColumnCollection columns;
		private Widget surface;
		private Widget headerStripe;
		private ItemPanel itemPanel;
		private StructuredType sourceType;
		private int suspendColumnUpdates;
		private Drawing.Size defaultItemSize;
		private Drawing.Size aperture;
		private int suspendScroll;
	}
}
