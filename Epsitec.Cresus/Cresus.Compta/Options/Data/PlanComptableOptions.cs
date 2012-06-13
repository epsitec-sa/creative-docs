//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage du plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableOptions : AbstractOptions
	{
		public override void Clear()
		{
			base.Clear ();
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new PlanComptableOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new PlanComptableOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				// TODO...

				return this.StopSummaryBuilder ();
			}
		}
	}
}
