﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryImageCategoryViewController : SummaryViewController<Entities.ImageCategoryEntity>
	{
		public SummaryImageCategoryViewController(string name, Entities.ImageCategoryEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				data.Add (
					new SummaryData
					{
						Name				= "ImageCategory",
						IconUri				= "Data.ArticleCategory",
						Title				= TextFormatter.FormatText ("Catégorie"),
						CompactTitle		= TextFormatter.FormatText ("Catégorie"),
						TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
						CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
						EntityMarshaler		= this.CreateEntityMarshaler (),
					});
			}
		}
	}
}