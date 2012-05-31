//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	public class SoldesColumn
	{
		public SoldesColumn()
		{
			this.DateDébut = new Date (1, 1, 1);
		}

		public FormattedText NuméroCompte
		{
			get;
			set;
		}

		public Date DateDébut
		{
			get;
			set;
		}

		public FormattedText Description
		{
			get
			{
				if (this.NuméroCompte.IsNullOrEmpty)
				{
					return FormattedText.Concat ("Vide").ApplyItalic ();
				}
				else
				{
					return FormattedText.Concat (this.NuméroCompte, " — ", Converters.DateToString (this.DateDébut));
				}
			}
		}


		public void CopyTo(SoldesColumn dst)
		{
			dst.NuméroCompte = this.NuméroCompte;
			dst.DateDébut    = this.DateDébut;
		}

		public bool Compare(SoldesColumn other)
		{
			return this.NuméroCompte == other.NuméroCompte &&
				   this.DateDébut    == other.DateDébut;
		}
	}
}
