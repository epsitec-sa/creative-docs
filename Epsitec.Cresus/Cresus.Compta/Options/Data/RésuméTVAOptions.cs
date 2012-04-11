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
	/// Cette classe décrit les options d'affichage du résumé TVA de la comptabilité.
	/// </summary>
	public class RésuméTVAOptions : AbstractOptions
	{
		public override void Clear()
		{
			base.Clear ();

			this.MontreEcritures = false;
			this.MontantTTC      = false;
			this.ParCodesTVA     = false;
		}


		public bool MontreEcritures
		{
			get;
			set;
		}

		public bool MontantTTC
		{
			get;
			set;
		}

		public bool ParCodesTVA
		{
			get;
			set;
		}

		public decimal? MontantLimite
		{
			get;
			set;
		}

		public decimal? PourcentLimite
		{
			get;
			set;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new RésuméTVAOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new RésuméTVAOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			var d = dst as RésuméTVAOptions;
			d.MontreEcritures = this.MontreEcritures;
			d.MontantTTC      = this.MontantTTC;
			d.ParCodesTVA     = this.ParCodesTVA;
			d.MontantLimite   = this.MontantLimite;
			d.PourcentLimite  = this.PourcentLimite;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as RésuméTVAOptions;

			return this.MontreEcritures == o.MontreEcritures &&
				   this.MontantTTC      == o.MontantTTC      &&
				   this.ParCodesTVA     == o.ParCodesTVA     &&
				   this.MontantLimite   == o.MontantLimite   &&
				   this.PourcentLimite  == o.PourcentLimite;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (this.MontreEcritures)
				{
					this.AppendSummaryBuilder ("Montre les écritures");
				}

				if (this.MontantTTC)
				{
					this.AppendSummaryBuilder ("Montants TTC");
				}

				if (this.ParCodesTVA)
				{
					this.AppendSummaryBuilder ("Par code TVA");
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
