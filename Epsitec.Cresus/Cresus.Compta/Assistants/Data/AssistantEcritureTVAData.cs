//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Assistant.Data
{
	/// <summary>
	/// Données génériques pour l'assistant "écriture avec TVA" la comptabilité.
	/// </summary>
	public class AssistantEcritureTVAData : AbstractAssistantData
	{
		public Date Date
		{
			get;
			set;
		}

		public ComptaCompteEntity Débit
		{
			get;
			set;
		}

		public ComptaCompteEntity Crédit
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

		public decimal Montant
		{
			get;
			set;
		}

		public ComptaCodeTVAEntity CodeTVA
		{
			get;
			set;
		}


		public Date? DateDébut
		{
			get;
			set;
		}

		public Date? DateFin
		{
			get;
			set;
		}

		public decimal TauxTVA1
		{
			get;
			set;
		}

		public decimal TauxTVA2
		{
			get;
			set;
		}

		public decimal MontantTVA1
		{
			get;
			set;
		}

		public decimal MontantTVA2
		{
			get;
			set;
		}

		public decimal MontantHT1
		{
			get;
			set;
		}

		public decimal MontantHT2
		{
			get;
			set;
		}
	}
}