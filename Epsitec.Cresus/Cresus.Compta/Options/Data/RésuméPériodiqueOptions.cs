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

			this.NumberOfMonths = 3;  // périodicité trimestrielle
			this.Cumul          = false;
			this.HideZero       = true;
			this.HasGraphics    = true;
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

		public bool HideZero
		{
			//	Affiche en blanc les montants nuls ?
			get;
			set;
		}

		public bool HasGraphics
		{
			get;
			set;
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
			d.HideZero       = this.HideZero;
			d.HasGraphics    = this.HasGraphics;

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
				   this.Cumul          == o.Cumul          &&
				   this.HideZero       == o.HideZero       &&
				   this.HasGraphics    == o.HasGraphics;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				this.AppendSummaryBuilder (RésuméPériodiqueOptions.MonthsToDescription (this.NumberOfMonths));

				if (this.Cumul)
				{
					this.AppendSummaryBuilder ("Chiffres cumulés");
				}

				if (this.HideZero)
				{
					this.AppendSummaryBuilder ("Affiche en blanc les montants nuls");
				}

				if (this.HasGraphics)
				{
					this.AppendSummaryBuilder ("Graphique");
				}

				return this.StopSummaryBuilder ();
			}
		}

		public static string MonthsToDescription(int months)
		{
			switch (months)
			{
				case 1:
					return "Mensuel";

				case 2:
					return "Bimestriel";

				case 3:
					return "Trimestriel";

				case 6:
					return "Semestriel";

				case 12:
					return "Annuel";

				default:
					return string.Format ("Par {0} mois", months.ToString ());
			}
		}
	}
}
