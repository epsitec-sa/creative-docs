﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryImageViewController : SummaryViewController<ImageEntity>
	{
		public SummaryImageViewController(string name, ImageEntity entity)
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

		private void CreateUIImage(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "Image",
					IconUri				= "Data.Image",
					Title				= TextFormatter.FormatText ("Image", "(", TextFormatter.Join (", ", this.Entity.ImageGroups.Select (group => group.Name)), ")"),
					CompactTitle		= TextFormatter.FormatText ("Image"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIPreview(Widget parent)
		{
			//	Crée l'aperçu de l'image.
			if (this.Entity.ImageBlob.IsNotNull ())
			{
				var store = this.Data.ImageDataStore;
				var data = store.GetImageData (this.Entity.ImageBlob.Code, 300);
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
			}
		}
	}
}
