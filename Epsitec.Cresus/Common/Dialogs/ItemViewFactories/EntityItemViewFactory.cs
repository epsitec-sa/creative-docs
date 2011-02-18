//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Dialogs.ItemViewFactories;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;
using Epsitec.Common.Widgets;

[assembly: ItemViewFactory (typeof (EntityItemViewFactory), ItemType=typeof (AbstractEntity))]

namespace Epsitec.Common.Dialogs.ItemViewFactories
{
	internal sealed class EntityItemViewFactory : IItemViewFactory
	{
		public EntityItemViewFactory()
		{
			this.textTemplate = new StaticText ();
		}

		#region IItemViewFactory Members

		public ItemViewWidget CreateUserInterface(ItemView itemView)
		{
			ItemViewWidget container  = new ItemViewWidget (itemView);
			
			StaticText widget = new StaticText ()
			{
				Embedder = container,
				Text = EntityItemViewFactory.GetText (itemView),
				Dock = DockStyle.Fill
			};

			return container;
		}

		public void DisposeUserInterface(ItemViewWidget widget)
		{
			widget.Dispose ();
		}

		public Drawing.Size GetPreferredSize(ItemView itemView)
		{
#if true
			return new Drawing.Size (200, 16);
#else
			this.textTemplate.Text = EntityItemViewFactory.GetText (itemView);
			Drawing.Size size = this.textTemplate.GetBestFitSize ();

			double dx = System.Math.Max (size.Width, itemView.Owner.PreferredLayoutWidth);
			double dy = size.Height + 2;

			return new Drawing.Size (dx, dy);
#endif
		}

		#endregion

		private static string GetText(ItemView itemView)
		{
			AbstractEntity data = itemView.Item as AbstractEntity;
			Entities.ISearchable searchable = data as Entities.ISearchable;

			string text;

			if (searchable == null)
			{
				text = data == null ? "" : data.Dump ();
			}
			else
			{
				text = searchable.SearchValue ?? "";
			}

			return TextLayout.ConvertToTaggedText (text);
		}

		private StaticText textTemplate;
	}
}
