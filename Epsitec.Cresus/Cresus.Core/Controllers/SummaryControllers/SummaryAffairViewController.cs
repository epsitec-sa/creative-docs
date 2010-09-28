//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryAffairViewController : SummaryViewController<AffairEntity>
	{
		public SummaryAffairViewController(string name, AffairEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIAffair (data);
			}
		}

		private void CreateUIAffair(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "Affair",
					IconUri				= "Data.Affair",
					Title				= TextFormatter.FormatText ("Affaire"),
					CompactTitle		= TextFormatter.FormatText ("Affaire"),
					TextAccessor		= this.CreateAccessor (x => TextFormatter.FormatText (x.IdA)),
					CompactTextAccessor = this.CreateAccessor (x => TextFormatter.FormatText (x.IdA)),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}
	}
}
