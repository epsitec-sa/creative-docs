//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (0)]
	public class EditionDocumentOptionsViewController0 : EditionViewController<DocumentOptionsEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 640;
		}

		protected override void CreateBricks(BrickWall<DocumentOptionsEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.SpecialController")
				.Title (this.CustomizedTitle)
				.Attribute (BrickMode.FullHeightStretch)
				.Input ()
				  .Field (x => x).WithSpecialController ()
				.End ()
				;
		}

		private FormattedText CustomizedTitle
		{
			get
			{
				var title = "Options d'impression du document";  // TODO: Comment obtenir le titre original ???
				return FormattedText.Concat (title, " — réglages");
			}
		}
	}
}
