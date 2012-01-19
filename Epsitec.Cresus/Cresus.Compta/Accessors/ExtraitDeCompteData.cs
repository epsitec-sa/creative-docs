//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données pour un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteData : AbstractData
	{
		public ExtraitDeCompteData()
		{
		}


		public Date? Date
		{
			get;
			set;
		}

		public ComptabilitéCompteEntity CP
		{
			get;
			set;
		}

		public FormattedText Pièce
		{
			get;
			set;
		}

		public FormattedText Libellé
		{
			get;
			set;
		}

		public decimal? Débit
		{
			get;
			set;
		}

		public decimal? Crédit
		{
			get;
			set;
		}

		public decimal? Solde
		{
			get;
			set;
		}
	}
}