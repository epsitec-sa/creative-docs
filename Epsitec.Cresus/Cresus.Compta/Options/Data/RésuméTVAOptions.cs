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
			this.HideZero = false;
		}


		public bool HideZero
		{
			//	Affiche en blanc les montants nuls ?
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
			d.HideZero = this.HideZero;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as RésuméTVAOptions;

			return this.HideZero == o.HideZero;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (this.HideZero)
				{
					this.AppendSummaryBuilder ("Affiche en blanc les montants nuls");
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
