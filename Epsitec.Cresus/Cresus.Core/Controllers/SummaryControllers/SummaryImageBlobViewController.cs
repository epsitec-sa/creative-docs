//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryImageBlobViewController : SummaryViewController<Entities.ImageBlobEntity>
	{
		public SummaryImageBlobViewController(string name, Entities.ImageBlobEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIImage (data);
			}

			this.CreateUIPreview (this.TileContainer);
		}

		private void CreateUIImage(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "ImageBlob",
					IconUri				= "Data.ImageBlob",
					Title				= TextFormatter.FormatText ("Image bitmap"),
					CompactTitle		= TextFormatter.FormatText ("Image bitmap"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIPreview(Widget parent)
		{
			throw new System.NotImplementedException ();
#if false
			//	Crée l'aperçu de l'image.
			var store = this.Data.ImageDataStore;
			var data = store.GetImageData (this.Entity.Code, 300);
			Image image = data.GetImage ();

			var box = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 300,
				Padding = new Margins (10),
				DrawFrameState = FrameState.Bottom,
				Dock = DockStyle.Top,
				Margins = Widgets.Tiles.Tile.GetContainerPadding (Direction.Right),
			};

			var miniature = new Widgets.Miniature ()
			{
				Parent = box,
				Image = image,
				Dock = DockStyle.Fill,
			};
#endif
		}
	}
}