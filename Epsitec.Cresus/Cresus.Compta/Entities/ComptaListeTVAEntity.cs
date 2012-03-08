//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaListeTVAEntity
	{
		public decimal? DefaultTauxValue
		{
			get
			{
				if (this.Taux == null || !this.Taux.Any ())
				{
					return null;
				}

				var taux = this.Taux.Where (x => x.ParDéfaut).FirstOrDefault ();
				if (taux == null)
				{
					return this.Taux.Last ().Taux;
				}
				else
				{
					return taux.Taux;
				}
			}
		}

		public FormattedText DefaultTauxNom
		{
			get
			{
				if (this.Taux == null || !this.Taux.Any ())
				{
					return FormattedText.Empty;
				}

				var taux = this.Taux.Where (x => x.ParDéfaut).FirstOrDefault ();
				if (taux == null)
				{
					return this.Taux.Last ().Nom;
				}
				else
				{
					return taux.Nom;
				}
			}
		}

		public FormattedText SummaryTaux
		{
			get
			{
				if (this.Taux == null || !this.Taux.Any ())
				{
					return FormattedText.Empty;
				}

				var list = new List<string> ();

				foreach (var taux in this.Taux)
				{
					FormattedText text = Converters.PercentToString (taux.Taux);

					if (taux.ParDéfaut)
					{
						text = text.ApplyBold ();
					}

					list.Add (text.ToString ());
				}

				return string.Join (", ", list);
			}
		}
	}
}
