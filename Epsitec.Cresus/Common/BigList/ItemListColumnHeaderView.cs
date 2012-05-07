//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Processors;
using Epsitec.Common.BigList.Renderers;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (ItemListColumnHeaderView))]

namespace Epsitec.Common.BigList
{
	public partial class ItemListColumnHeaderView : Widget
	{
		public ItemListColumnHeaderView()
		{
			this.IsMasterView = true;
			
			this.policies  = new List<EventProcessorPolicy> ();
			this.processor = new ViewEventProcessor (this);

			this.policies.Add (
				new MouseDownProcessorPolicy
				{
					AutoFollow = false,
					SelectionPolicy = SelectionPolicy.OnMouseUp,
				});

			this.DefineColumnCollection (new ItemListColumnCollection ());
		}


		public bool								IsMasterView
		{
			get;
			set;
		}

		public ItemListColumnCollection			Columns
		{
			get
			{
				return this.columns;
			}
		}


		public TPolicy GetPolicy<TPolicy>()
			where TPolicy : EventProcessorPolicy, new ()
		{
			return this.policies.OfType<TPolicy> ().FirstOrDefault () ?? new TPolicy ();
		}

		public void SetPolicy<TPolicy>(TPolicy policy)
			where TPolicy : EventProcessorPolicy, new ()
		{
			this.policies.RemoveAll (x => x.GetType () == typeof (TPolicy));
			this.policies.Add (policy);
		}


		public void SelectSortColumn(ItemListColumn column)
		{
			this.Columns.SpecifySort (column, column.GetActiveSortOrder (toggle: column.SortIndex == 0));
		}

		
		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.processor.ProcessMessage (message, pos))
			{
				this.Invalidate ();
				message.Consumer = this;
			}
		}
		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.columns.Count == 0)
			{
				return;
			}

			double width  = this.Client.Width;
			double height = this.Client.Height;

			graphics.AddFilledRectangle (0, 0, width, height);
			graphics.RenderSolid (Color.FromBrightness (1));

			foreach (var column in this.columns.Where (x => x.IsVisible))
			{
				var ox = column.Layout.Definition.ActualOffset;
				var dx = column.Layout.Definition.ActualWidth;
				
				graphics.AddRectangle (ox, 0, dx, height);
				graphics.RenderSolid (Color.FromBrightness (0.3));
				
				graphics.Color = Color.FromBrightness (0);
				graphics.PaintText (ox, 0, dx, height, column.Title.ToSimpleText (), Font.DefaultFont, Font.DefaultFontSize, ContentAlignment.MiddleCenter);

				if (column.CanSort)
				{
					var index = column.SortIndex;
					var brightness = new double[] { 0.0, 0.5, 0.6, 0.7, 0.8 };
					graphics.Color = Color.FromBrightness (brightness[System.Math.Min (index, brightness.Length-1)]);

					switch (column.SortOrder)
					{
						case ItemSortOrder.Ascending:
							this.PaintGlyph (graphics, new Rectangle (ox, 0, dx, height), GlyphShape.TriangleDown);
							break;
						case ItemSortOrder.Descending:
							this.PaintGlyph (graphics, new Rectangle (ox, 0, dx, height), GlyphShape.TriangleUp);
							break;
					}
				}
			}

			this.processor.PaintOverlay (graphics, clipRect);
		}
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();

			this.RefreshColumnLayout ();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DefineColumnCollection (null);
			}

			base.Dispose (disposing);
		}


		private Rectangle GetColumnBounds(ItemListColumn column)
		{
			double height = this.Client.Height;
			
			var ox = column.Layout.Definition.ActualOffset;
			var dx = column.Layout.Definition.ActualWidth;

			return new Rectangle (ox, 0, dx, height);
		}

		private void PaintGlyph(Graphics graphics, Rectangle rectangle, GlyphShape glyphShape)
		{
			var cx = rectangle.Center.X;
			var cy = rectangle.Top;
			var dx = 10;

			if (glyphShape == GlyphShape.TriangleUp)
			{
				cy = rectangle.Bottom - 2;
			}
			else
			{
				cy = cy - dx + 2;
			}
			
			var bounds = new Rectangle (cx-dx/2, cy, dx, dx);

			using (var path = Epsitec.Common.Widgets.Adorners.Default.GetGlyphPath (bounds, this.PaintState, glyphShape))
			{
				graphics.PaintSurface (path);
			}
		}

		private void DefineColumnCollection(ItemListColumnCollection collection)
		{
			if (this.columns != null)
			{
				this.columns.CollectionChanged -= this.HandleColumnsCollectionChanged;
			}

			this.columns = collection;

			if (this.columns != null)
			{
				this.columns.CollectionChanged += this.HandleColumnsCollectionChanged;
			}
		}

		private void RefreshColumnLayout()
		{
			if (this.columns.Count == 0)
			{
				this.Visibility = false;
			}
			else
			{
				this.Visibility = true;
			}

			double headerWidth = this.Client.Width;

			this.columns.Layout (headerWidth);
		}


		private void HandleColumnsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			foreach (ItemListColumn column in e.OldItems)
			{
				column.SortOrderChanged -= this.HandleItemListColumnSortOrderChanged;
			}
			foreach (ItemListColumn column in e.NewItems)
			{
				column.SortOrderChanged += this.HandleItemListColumnSortOrderChanged;
			}

			this.Invalidate ();

			if (this.IsMasterView)
			{
				this.RefreshColumnLayout ();
			}
		}

		private void HandleItemListColumnSortOrderChanged(object sender)
		{
			this.Invalidate ();
		}



		private readonly List<EventProcessorPolicy>	policies;
		private readonly ViewEventProcessor		processor;
		
		private ItemListColumnCollection		columns;
	}
}