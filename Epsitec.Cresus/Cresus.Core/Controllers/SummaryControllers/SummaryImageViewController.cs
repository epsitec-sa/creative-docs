//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			this.CreateUIPreviewPanel ();
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

		private void CreateUIPreviewPanel()
		{
			//	Crée le conteneur.
		}
	}
}
