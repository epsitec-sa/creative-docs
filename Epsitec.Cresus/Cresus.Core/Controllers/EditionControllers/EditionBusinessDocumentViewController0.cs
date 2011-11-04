//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (0)]
	public class EditionBusinessDocumentViewController0 : EditionViewController<BusinessDocumentEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 800;
		}

		protected override void CreateBricks(BrickWall<BusinessDocumentEntity> wall)
		{
			wall.AddBrick ()
				.Title (this.CustomizedTitle)
				.Icon ("Data.DocumentItems")
				.Attribute (BrickMode.FullHeightStretch)
				.Input ()
				  .Field (x => x).WithSpecialController (0)  // lignes du document
				.End ()
				;
		}

		private FormattedText CustomizedTitle
		{
			//	Comme l'éditeur de lignes d'un document occupe une grande largeur, les tuiles de gauche ne sont
			//	généralement plus visibles. Il est donc important de rappeler le nom du type du document dans
			//	le titre (par exemple "Facture — lignes du document").
			get
			{
				var documentMetadata = this.BusinessContext.GetMasterEntity<DocumentMetadataEntity> ();
				var title = documentMetadata.GetTitle ();
				return FormattedText.Concat (title, " — lignes du document");
			}
		}
	}
}
