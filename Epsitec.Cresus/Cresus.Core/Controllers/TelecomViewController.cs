//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class TelecomViewController : EntityViewController
	{
		public TelecomViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.TelecomContactAccessor (this.Entity as Entities.TelecomContactEntity);

			FrameBox frame = this.CreateEditionTile (accessor, ViewControllerMode.None);

			this.CreateTextField (frame, 150, "Type du numéro", accessor.TelecomType, x => accessor.TelecomType = x, Validators.StringValidator.Validate);
			this.CreateMargin (frame, 10);
			this.CreateTextField (frame, 150, "Numéro de téléphone", accessor.TelecomContact.Number, x => accessor.TelecomContact.Number = x, Validators.StringValidator.Validate);
			this.CreateTextField (frame, 100, "Numéro interne", accessor.TelecomContact.Extension, x => accessor.TelecomContact.Extension = x, Validators.StringValidator.Validate);

			this.SetInitialFocus ();
		}
	}
}
