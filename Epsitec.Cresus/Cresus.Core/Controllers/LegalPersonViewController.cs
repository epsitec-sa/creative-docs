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
	public class LegalPersonViewController : EntityViewController
	{
		public LegalPersonViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);
			var person = this.Entity as Entities.LegalPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			var legalPerson = person as Entities.LegalPersonEntity;
			this.CreateSimpleTile (container, "Data.LegalPerson", "Personne morale", this.GetLegalPersonSummary (legalPerson));

			this.AdjustLastTile (container);
		}



		private string GetLegalPersonSummary(Entities.LegalPersonEntity legalPerson)
		{
			var builder = new StringBuilder ();

			builder.Append (legalPerson.Name);
			builder.Append ("<br/>");

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}
	}
}
