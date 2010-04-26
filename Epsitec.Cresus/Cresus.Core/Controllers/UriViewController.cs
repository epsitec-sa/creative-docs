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
	public class UriViewController : EntityViewController
	{
		public UriViewController(string name)
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
			var accessor = new EntitiesAccessors.UriContactAccessor (this.Entity as Entities.UriContactEntity);

			FrameBox frame = this.CreateEditionTile (accessor, ViewControllerMode.None);

			this.CreateTextField (frame, 100, "Type", accessor.UriScheme, x => accessor.UriScheme = x, Validators.StringValidator.Validate);
			this.CreateTextField (frame, 0, "Adresse mail", accessor.UriContact.Uri, x => accessor.UriContact.Uri = x, Validators.StringValidator.Validate);

			this.SetInitialFocus ();
		}
	}
}
