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
			this.telecomContact = this.Entity as Entities.TelecomContactEntity;
			System.Diagnostics.Debug.Assert (this.telecomContact != null);

			FrameBox frame = this.CreateEditionTile (this.Entity, ViewControllerMode.None, EntitySummary.GetIcon (this.telecomContact), EntitySummary.GetTitle (this.telecomContact));

			this.CreateTextField (frame, "Type du numéro", this.TelecomType, x => this.TelecomType = x, Validators.StringValidator.Validate);
			this.CreateMargin (frame, 10);
			this.CreateTextField (frame, "Numéro de téléphone", this.telecomContact.Number, x => this.telecomContact.Number = x, Validators.StringValidator.Validate);
			this.CreateTextField (frame, "Numéro interne", this.telecomContact.Extension, x => this.telecomContact.Extension = x, Validators.StringValidator.Validate);
		}


		private string TelecomType
		{
			get
			{
				if (this.telecomContact.TelecomType != null)
				{
					return this.telecomContact.TelecomType.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.telecomContact.TelecomType == null)
				{
					this.telecomContact.TelecomType = new Entities.TelecomTypeEntity ();
				}

				this.telecomContact.TelecomType.Name = value;
			}
		}


		private Entities.TelecomContactEntity telecomContact;
	}
}
