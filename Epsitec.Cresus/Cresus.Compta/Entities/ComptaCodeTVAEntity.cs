//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaCodeTVAEntity
	{
		public FormattedText Diagnostic
		{
			get
			{
				if (this.ListeTaux.Taux.Count == 0)
				{
					return FormattedText.Format ("Aucun taux").ApplyBold ();
				}

				int count = this.ListeTaux.Taux.Where (x => x.ParDéfaut).Count ();
				if (count == 0)
				{
					return FormattedText.Format ("Pas de taux par défaut").ApplyBold ();
				}
				if (count > 1)
				{
					return FormattedText.Format ("Plusieurs taux par défaut").ApplyBold ();
				}

				return FormattedText.Empty;  // ok
			}
		}


		public FormattedText MenuDescription
		{
			get
			{
				return FormattedText.Concat (this.Code.ApplyBold (), "#", Converters.PercentToString (this.DefaultTauxValue));
			}
		}

		public decimal? DefaultTauxValue
		{
			get
			{
				if (this.ListeTaux == null)
				{
					return null;
				}
				else
				{
					return this.ListeTaux.DefaultTauxValue;
				}
			}
		}

		public FormattedText DefaultTauxNom
		{
			get
			{
				if (this.ListeTaux == null)
				{
					return FormattedText.Empty;
				}
				else
				{
					return this.ListeTaux.DefaultTauxNom;
				}
			}
		}
	}
}
