//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.textTemplate.Text = EntityItemViewFactory.GetText (itemView);

			return this.textTemplate.GetBestFitSize ();
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
