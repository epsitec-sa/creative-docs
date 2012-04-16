//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage du résumé pèriodique de la comptabilité.
	/// </summary>
	public class RésuméPériodiqueOptions : AbstractOptions
	{
		public override void Clear()
		{
			base.Clear ();

			this.NumberOfMonths = 3;
		}


		public int NumberOfMonths
		{
			set;
			get;
		}

		public bool Cumul
		{
			set;
			get;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new RésuméPériodiqueOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new RésuméPériodiqueOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			var d = dst as RésuméPériodiqueOptions;
			d.NumberOfMonths = this.NumberOfMonths;
			d.Cumul          = this.Cumul;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as RésuméPériodiqueOptions;

			return this.NumberOfMonths == o.NumberOfMonths &&
				   this.Cumul          == o.Cumul;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				this.AppendSummaryBuilder (string.Format ("Par {0} mois", this.NumberOfMonths.ToString ()));

				return this.StopSummaryBuilder ();
			}
		}
	}
}
