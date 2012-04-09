//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données pour le résumé TVA de la comptabilité.
	/// </summary>
	public class RésuméTVAData : AbstractData
	{
		public RésuméTVAData()
		{
		}


		public bool CodeTVA
		{
			get;
			set;
		}

		public FormattedText Compte
		{
			get;
			set;
		}

		public FormattedText Titre
		{
			get;
			set;
		}

		public decimal Montant
		{
			get;
			set;
		}

		public decimal TVA
		{
			get;
			set;
		}
	}
}