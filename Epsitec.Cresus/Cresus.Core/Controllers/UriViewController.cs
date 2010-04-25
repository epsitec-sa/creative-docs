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
			this.uriContact = this.Entity as Entities.UriContactEntity;
			System.Diagnostics.Debug.Assert (this.uriContact != null);

			FrameBox frame = this.CreateEditionTile (this.Entity, ViewControllerMode.None, EntitySummary.GetIcon (this.uriContact), EntitySummary.GetTitle (this.uriContact));

			this.CreateTextField (frame, 100, "Type", this.UriScheme, x => this.UriScheme = x, Validators.StringValidator.Validate);
			this.CreateTextField (frame, 0, "Adresse mail", this.uriContact.Uri, x => this.uriContact.Uri = x, Validators.StringValidator.Validate);

			this.SetInitialFocus ();
		}


		private string UriScheme
		{
			get
			{
				if (this.uriContact.UriScheme != null)
				{
					return this.uriContact.UriScheme.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.uriContact.UriScheme == null)
				{
					this.uriContact.UriScheme = new Entities.UriSchemeEntity ();
				}

				this.uriContact.UriScheme.Name = value;
			}
		}


		private Entities.UriContactEntity uriContact;
	}
}
