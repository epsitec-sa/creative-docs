//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemTable))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemTable</c> class is used to represent items in a table, with
	/// column headers and scroll bars.
	/// </summary>
	public class ItemTable : Widget, IListHost<ItemTableColumn>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ItemTable"/> class.
		/// </summary>
		public ItemTable()
		{
			this.AutoDoubleClick = true;

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

			this.surface.TabNavigationMode = TabNavigationMode.None;

			this.columnHeader.PreferredHeight *= ItemTable.HeaderHeightFactor;
			this.headerStripe.PreferredHeight = this.columnHeader.PreferredHeight;

			this.UpdateGeometry ();

			this.hScroller.ValueChanged += this.HandleScrollerValueChanged;
			this.vScroller.ValueChanged += this.HandleScrollerValueChanged;
			this.surface.SizeChanged += this.HandleSurfaceSizeChanged;
			this.itemPanel.ApertureChanged += this.HandleItemPanelApertureChanged;
			this.itemPanel.ContentsSizeChanged += this.HandleContentsSizeChanged;
			this.itemPanel.CurrentChanged += this.HandleItemPanelCurrentChanged;
			this.itemPanel.Clicked += this.HandleItemPanelClicked;
			this.itemPanel.AddEventHandler (ItemPanel.LayoutProperty, this.HandleItemPanelLayoutChanged);
			this.itemPanel.AddEventHandler (ItemPanel.LayoutGroupsProperty, this.HandleItemPanelLayoutChanged);
			this.itemPanel.AddEventHandler (ItemPanel.LayoutSubgroupsProperty, this.HandleItemPanelLayoutChanged);
			
//-			this.itemPanel.Layout = ItemPanelLayout.VerticalList;
			this.itemPanel.ItemSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			this.itemPanel.GroupSelectionMode = ItemPanelSelectionMode.None;

			this.columnHeader.ItemPanel = this.itemPanel;
			this.columnHeader.ColumnWidthChanged += this.HandleColumnHeaderColumnWidthChanged;
			
			this.itemPanel.PreferredLayoutWidth = this.columnHeader.GetTotalWidth ();

			//	Link the item panel with its table, so that an ItemViewFactory
			//	can find the table and the column templates, if it needs to do
			//	so :
			
			ItemTable.SetItemTable (this.itemPanel, this);
		}

		private void HandleColumnHeaderColumnWidthChanged(object sender, ColumnWidthChangeEventArgs e)
		{
			this.UpdateItemPanelLayoutWidth ();
			this.surface.Invalidate ();
		}

		private void UpdateItemPanelLayoutWidth()
		{
			double headerWidth   = this.columnHeader.GetTotalWidth ();
			double apertureWidth = this.apertureSize.Width;

			this.itemPanel.PreferredLayoutWidth = System.Math.Max (headerWidth, apertureWidth);
		}

		private void UpdateGeometry()
		{
			double topMargin    = this.headerStripe.Visibility ? this.headerStripe.PreferredHeight : 0;
			double bottomMargin = this.hScroller.Visibility ? this.hScroller.PreferredHeight : 0;
			double rightMargin  = this.vScroller.Visibility ? this.vScroller.PreferredWidth : 0;
			double frameMargin  = this.FrameVisibility ? 1 : 0;

			this.headerStripe.Margins = new Margins (0, rightMargin, 0, 0);
			this.vScroller.Margins = new Margins (0, 0, topMargin, bottomMargin);
			this.hScroller.Margins = new Margins (0, rightMargin, 0, 0);
			this.surface.Margins = new Margins (frameMargin, rightMargin+frameMargin, topMargin+frameMargin, bottomMargin+frameMargin);
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
				if (this.Items != value)
				{
					this.ItemPanel.Items = value;

					if ((value != null) &&
						(value.SourceCollection != null) &&
						(this.sourceType == null))
					{
						object firstItem;
						IStructuredTypeProvider provider = null;

						if (Collection.TryGetFirst (value.SourceCollection, out firstItem))
						{
							provider = firstItem as IStructuredTypeProvider;
						}
						else
						{
							System.Type collectionType = value.SourceCollection.GetType ();
							System.Type itemType = TypeRosetta.GetEnumerableItemType (collectionType);

							if ((itemType != null) &&
								(typeof (DependencyObject).IsAssignableFrom (itemType)))
							{
								//	We have an empty collection of DependencyObject derived
								//	items; use this information to derive the object type.

								provider = DependencyObjectType.FromSystemType (itemType);
							}
						}


						if (provider != null)
						{
							this.SourceType = provider.GetStructuredType ();
						}
						
					}
				}
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

		public bool								SeparatorVisibility
		{
			get
			{
				return this.separatorVisibility;
			}
			set
			{
				if (this.separatorVisibility != value)
				{
					this.separatorVisibility = value;
					this.Invalidate ();
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

		public IStructuredType					SourceType
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

		public void DefineDefaultColumns(IStructuredType sourceType, double width)
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

		public Size GetDefaultItemSize(ItemView itemView)
		{
			return this.defaultItemSize;
		}

		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			if (base.AboutToGetFocus (dir, mode, out focus))
			{
				if ((focus == this) &&
					(this.itemPanel != null))
				{
					return this.itemPanel.InternalAboutToGetFocus (dir, mode, out focus);
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		protected override void OnSizeChanged(Size oldValue, Size newValue)
		{
			base.OnSizeChanged (oldValue, newValue);

			this.UpdateItemPanelLayoutWidth ();

			if (this.itemPanel.Layout != ItemPanelLayout.VerticalList)
			{
				this.itemPanel.RefreshLayout ();
			}
		}
		
		protected override void ProcessMessage(Message message, Point pos)
		{
			base.ProcessMessage(message, pos);

			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseWheel)
				{
					if (this.vScroller.IsVisible)
					{
						if (message.Wheel < 0)
						{
							this.vScroller.Value += this.vScroller.SmallChange;
						}

						if (message.Wheel > 0)
						{
							this.vScroller.Value -= this.vScroller.SmallChange;
						}
					}
				}

				message.Consumer = this;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			Widgets.IAdorner adorner = Widgets.Adorners.Factory.Active;

			double x1 = 0;
			double x2 = this.vScroller.Visibility ? this.vScroller.ActualBounds.Left : this.Client.Bounds.Right;
			double y1 = this.hScroller.Visibility ? this.hScroller.ActualBounds.Top : this.Client.Bounds.Bottom;
			double y2 = this.headerStripe.Visibility ? this.headerStripe.ActualBounds.Bottom : this.Client.Bounds.Top;
			
			Rectangle bounds = Rectangle.FromOppositeCorners (x1, y1, x2, y2);

			if (this.BackColor.IsEmpty)
			{
				adorner.PaintArrayBackground (graphics, bounds, this.PaintState);

				if ((this.GetItemTableLayout () == ItemPanelLayout.VerticalList) &&
					(this.SeparatorVisibility))
				{
					this.PaintColumnSeparators (graphics, adorner, bounds);
				}
			}
			else
			{
				Rectangle rect = Rectangle.Intersection (clipRect, this.Client.Bounds);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);

				if (this.FrameVisibility)
				{
					graphics.AddLine (bounds.Left+0.5,  bounds.Bottom+0.5, bounds.Left+0.5,  bounds.Top+0.5);		//	trait vertical gauche
					graphics.AddLine (bounds.Right+0.5, bounds.Bottom+0.5, bounds.Right+0.5, bounds.Top+0.5);		//	trait vertical droite
					graphics.AddLine (bounds.Left+0.5,  bounds.Top+0.5,    bounds.Right+0.5, bounds.Top+0.5);		//	trait horizontal supérieur
					graphics.AddLine (bounds.Left+0.5,  bounds.Bottom+0.5, bounds.Right+0.5, bounds.Bottom+0.5);	//	trait horizontal inférieur
					graphics.RenderSolid (adorner.ColorBorder);
				}
			}
		}

		private void PaintColumnSeparators(Graphics graphics, Widgets.IAdorner adorner, Rectangle bounds)
		{
			//	Affiche les séparations verticales des colonnes.
			double x = bounds.Left + 0.5 - this.horizontalOffset;
			
			for (int i = 0; i < this.columnHeader.ColumnCount; i++)
			{
				Widgets.Layouts.ColumnDefinition cd = this.ColumnHeader.GetColumnDefinition (i);
				x += cd.ActualWidth;
				graphics.AddLine (x, bounds.Bottom, x, bounds.Top);
			}
			
			graphics.RenderSolid (Color.FromAlphaRgb (0.2, adorner.ColorBorder.R, adorner.ColorBorder.G, adorner.ColorBorder.B));
		}

		public Margins GetPanelPadding()
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
			Margins padding = Margins.Zero;

			this.apertureSize = this.ComputeApertureSize ();

			Size scrollSize = this.GetScrollSize (this.apertureSize - this.itemPanel.AperturePadding.Size);

			this.UpdateScrollRatios ();

			switch (this.GetVerticalScrollMode ())
			{
				case ItemTableScrollMode.Linear:
					if (scrollSize.Height > 0)
					{
						this.vScroller.SmallChange = (decimal) (this.itemPanel.ItemViewDefaultSize.Height / scrollSize.Height);
						this.vScroller.LargeChange = (decimal) (apertureSize.Height / scrollSize.Height);
					}
					break;

				case ItemTableScrollMode.ItemBased:
					this.vScroller.SmallChange = 1;
					this.vScroller.LargeChange = this.itemPanel.GetVisibleRowCount (this.apertureSize.Height);
					this.vScroller.MinValue = 0;
					this.vScroller.MaxValue = System.Math.Max (0, this.itemPanel.GetTotalRowCount () - this.vScroller.LargeChange);
					padding.Bottom = this.apertureSize.Height - this.itemPanel.GetRowHeight () * (double) (this.vScroller.LargeChange);
					break;
			}

			if (scrollSize.Width > 0)
			{
				this.hScroller.SmallChange = (decimal) (apertureSize.Width * 0.2 / scrollSize.Width);
				this.hScroller.LargeChange = (decimal) (apertureSize.Width / scrollSize.Width);
			}
			
			double aW = apertureSize.Width;
			double aH = apertureSize.Height;

			double ox = System.Math.Floor ((double) this.hScroller.Value * scrollSize.Width);
			double oy = this.GetVerticalScrollOffset ();

			this.itemPanel.Aperture = new Rectangle (ox, oy, aW, aH);
			this.itemPanel.AperturePadding = padding;

			if (this.itemPanel.PreferredHeight < aH)
			{
				oy = this.itemPanel.PreferredHeight - aH;
			}

			this.horizontalOffset = ox;
			
			double dx = System.Math.Max (this.itemPanel.PreferredWidth, aW);
			double dy = System.Math.Max (this.itemPanel.PreferredHeight, aH);

			Drawing.Size headerSize = this.columnHeader.GetBestFitSize ();

			this.itemPanel.SetManualBounds (new Rectangle (-ox, -oy, dx, dy));
			this.columnHeader.SetManualBounds (new Rectangle (-ox, 0, headerSize.Width, headerSize.Height));
		}

		private Size ComputeApertureSize()
		{
			return Rectangle.Deflate (this.Client.Bounds, this.GetPanelPadding ()).Size;
		}

		private ItemTableScrollMode GetVerticalScrollMode()
		{
			ItemTableScrollMode mode = this.VerticalScrollMode;

			if (mode == ItemTableScrollMode.ItemBased)
			{
				//	A table with groups can not be configured in the item based
				//	scroll mode :
				
				if ((this.Items != null) &&
					(this.Items.Groups.Count > 0))
				{
					mode = ItemTableScrollMode.Linear;
				}
			}

			return mode;
		}

		private void UpdateScrollRatios()
		{
			double hRatio = System.Math.Max (0, System.Math.Min (1, (apertureSize.Width+1) / (this.itemPanel.PreferredWidth+1)));
			double vRatio = System.Math.Max (0, System.Math.Min (1, (apertureSize.Height+1) / (this.itemPanel.PreferredHeight+1)));

			this.hScroller.VisibleRangeRatio = (decimal) hRatio;
			this.vScroller.VisibleRangeRatio = (decimal) vRatio;
		}

		private double GetVerticalScrollOffset()
		{
			switch (this.GetVerticalScrollMode ())
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

			return pos - this.apertureSize.Height;
		}

		private void SetVerticalScrollValueLinear(double offset)
		{
			double height = this.GetScrollHeight ();
			
			if (height > 0)
			{
				double pos = offset + this.apertureSize.Height;
				double dist = this.itemPanel.GetContentsSize ().Height - pos;
				this.vScroller.Value = (decimal) (dist / height);
			}
		}

		private double GetVerticalScrollOffsetItemBased()
		{
			int index = (int) this.vScroller.Value;
			int total = this.itemPanel.GetTotalRowCount ();

			index = System.Math.Min (total-1, index);
			index = System.Math.Max (0, index);
			
			ItemView view = this.itemPanel.GetItemViewAtRow (index);
			double   pos  = (view == null) ? this.itemPanel.GetContentsSize ().Height : this.itemPanel.GetItemViewBounds (view).Top;
			
			return pos - this.apertureSize.Height;
		}

		private void SetVerticalScrollValueItemBased(double offset)
		{
			double pos = offset + this.apertureSize.Height - this.itemPanel.AperturePadding.Height - 0.5;
			
			ItemView view = this.itemPanel.FindItemView (
				delegate (ItemView candidate)
				{
					Rectangle bounds = this.itemPanel.GetItemViewBounds (candidate);
					
					if ((bounds.Bottom < pos) &&
						(bounds.Top > pos))
					{
						return true;
					}

					return false;
				});

			if (view != null)
			{
				this.vScroller.Value = view.RowIndex;
			}
		}

		private double GetScrollWidth()
		{
			Size size = this.itemPanel.GetContentsSize ();
			return System.Math.Max (0, size.Width - this.apertureSize.Width);
		}

		private double GetScrollHeight()
		{
			Size size = this.itemPanel.GetContentsSize ();
			return System.Math.Max (0, size.Height - this.apertureSize.Height);
		}

		private Size GetScrollSize(Size aperture)
		{
			Size panelSize = this.itemPanel.GetContentsSize ();

			double dx = System.Math.Max (0, panelSize.Width  - aperture.Width);
			double dy = System.Math.Max (0, panelSize.Height - aperture.Height);

			return new Size (dx, dy);
		}

		private ItemPanelLayout GetItemTableLayout()
		{
			ItemPanelLayout layout;
			
			if ((this.Items != null) &&
				(this.Items.GroupDescriptions.Count > 0))
			{
				layout = this.itemPanel.LayoutGroups;
			}
			else
			{
				layout = this.itemPanel.Layout;
			}
			
			return layout;
		}

		private void UpdateColumnHeader()
		{
			if ((this.suspendColumnUpdates == 0) &&
				(this.columns != null) &&
				(this.columns.Count > 0) &&
				(this.sourceType != null))
			{
				Support.ResourceManager manager = Widgets.Helpers.VisualTree.FindResourceManager (this);

				bool useRealColumns = this.GetItemTableLayout () == ItemPanelLayout.VerticalList;
				
				this.columnHeader.ClearColumns ();

				double minWidth  = 0;
				double minHeight = 0;

				int headerIndex = 0;

				for (int i = 0; i < this.columns.Count; i++)
				{
					ItemTableColumn column = this.columns[i];
					Support.Druid captionId = Support.Druid.Empty;
					
					if (string.IsNullOrEmpty (column.FieldId))
					{
						//	There is no field defined for this column. This means
						//	that the contents of the column will default to the
						//	full item itself.
						
						captionId = column.CaptionId;

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
						
						if (field == null)
						{
							continue;
						}
						else
						{
							captionId = column.CaptionId;

							if (captionId.IsEmpty)
							{
								captionId = field.CaptionId;
							}

							this.columnHeader.AddColumn (i, field.Id, captionId);
						}
					}

					Size   size  = this.itemPanel.ItemViewDefaultSize;
					double width = column.Width.Value;

					if (column.TemplateId.IsValid)
					{
						size = Panel.GetPanelDefaultSize (column.TemplateId, manager);
					}
					else
					{
						size.Width = size.Width / this.columns.Count;
					}

					switch (column.Width.GridUnitType)
					{
						case Widgets.Layouts.GridUnitType.Absolute:
							width = column.Width.Value;
							break;

						case Widgets.Layouts.GridUnitType.Auto:
							width = size.Width;
							break;

						case Widgets.Layouts.GridUnitType.Proportional:
							//	TODO: gérer les colonnes proportionnelles
							break;
					}

					if (useRealColumns)
					{
						this.columnHeader.SetColumnWidth (headerIndex, width);
						this.columnHeader.SetColumnFixedWidth (headerIndex, false);
						this.columnHeader.SetColumnComparer (headerIndex, column.Comparer);
						this.columnHeader.SetColumnSortable (headerIndex, column.SortDirection != ListSortDirection.None);
						this.columnHeader.SetColumnVisibility (headerIndex, true);
						this.columnHeader.SetColumnContentAlignment (headerIndex, column.ContentAlignment);
//-						this.columnHeader.SetColumnSort (headerIndex, column.SortDirection);
					}
					else
					{
						this.columnHeader.AdjustColumnWidth (headerIndex);
						this.columnHeader.SetColumnFixedWidth (headerIndex, true);
						this.columnHeader.SetColumnComparer (headerIndex, column.Comparer);
						this.columnHeader.SetColumnSortable (headerIndex, column.SortDirection != ListSortDirection.None);
						this.columnHeader.SetColumnVisibility (headerIndex, captionId.IsValid);
						this.columnHeader.SetColumnContentAlignment (headerIndex, Drawing.ContentAlignment.MiddleCenter);
//-						this.columnHeader.SetColumnSort (headerIndex, column.SortDirection);
					}

					minWidth += width;
					minHeight = System.Math.Max (minHeight, size.Height);
					headerIndex += 1;
				}

				this.defaultItemSize = new Size (minWidth, minHeight);
				this.columnHeader.UpdateColumnSorts ();

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
			this.Invalidate ();
		}

		private void HandleContentsSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateApertureProtected ();
		}

		private void HandleItemPanelApertureChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.SynchronizeScrollers ((Rectangle) e.NewValue);
		}

		private void HandleItemPanelLayoutChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.Invalidate ();
			this.UpdateColumnHeader ();
		}

		private void HandleItemPanelClicked(object sender, MessageEventArgs e)
		{
			this.itemPanel.Focus ();
		}

		private void HandleItemPanelCurrentChanged(object sender)
		{
		}

		private void SynchronizeScrollers(Rectangle value)
		{
			if (this.suspendScroll == 0)
			{
				Size scrollSize = this.GetScrollSize (this.apertureSize - this.itemPanel.AperturePadding.Size);

				double sx = scrollSize.Width;
				double sy = scrollSize.Height;
				double ox = value.Left;
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
						switch (this.GetVerticalScrollMode ())
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

		public static readonly double HeaderHeightFactor = 1.3;

		private VScroller								vScroller;
		private HScroller								hScroller;
		private ItemPanelColumnHeader					columnHeader;
		private Collections.ItemTableColumnCollection	columns;
		private Widget									surface;
		private Widget									headerStripe;
		private ItemPanel								itemPanel;
		private IStructuredType							sourceType;
		private int										suspendColumnUpdates;
		private Size									defaultItemSize;
		private Size									apertureSize;
		private int										suspendScroll;
		private double									horizontalOffset;
		private bool									separatorVisibility = true;
	}
}
