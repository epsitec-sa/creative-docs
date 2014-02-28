//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Cette structure contient tous les détails permettant de calculer un amortissement.
	/// </summary>
	public struct AmortizationDetails
	{
		public AmortizationDetails(AmortizationDefinition def, ProrataDetails prorata,
								   decimal? initialValue, decimal? baseValue)
		{
			this.Def          = def;
			this.Prorata      = prorata;
							
			this.InitialValue = initialValue;
			this.BaseValue    = baseValue;
			this.DeltaValue   = null;
			this.FinalValue   = null;

			this.UpdateValues (out this.DeltaValue, out this.FinalValue);
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Def.IsEmpty;
			}
		}


		private void UpdateValues(out decimal? deltaValue, out decimal? finalValue)
		{
			//	Retourne la valeur finale amortie.
			//	S'il existe une valeur forcée, elle a la priorité.
			//	Sinon, la formule de base est:
			//	FinalValue = InitialValue - (BaseValue * EffectiveRate * ProrataQuotient)
			//	Avec encore un calcul de l'arrondi.
			deltaValue = null;
			finalValue = null;

			if (!this.IsEmpty)
			{
				if (this.InitialValue.HasValue &&
					this.BaseValue.HasValue &&
					!this.Def.IsEmpty)
				{
					//	Calcule la diminution de la valeur.
					deltaValue = this.BaseValue.Value * this.Def.EffectiveRate;

					if (this.Prorata.Quotient.HasValue)  // y a-t-il un prorata ?
					{
						deltaValue *= this.Prorata.Quotient.Value;
					}

					//	Calcule la valeur finale.
					finalValue = this.InitialValue.Value - deltaValue;

					//	Effectue encore un arrondi éventuel.
					finalValue = AmortizationDetails.Round (finalValue.Value, this.Def.Round);

					//	Plafonne selon la valeur résiduelle.
					finalValue = System.Math.Max (finalValue.Value, this.Def.Residual);
				}
			}
		}


		public static decimal Round(decimal value, decimal round)
		{
			//	Retourne un montant arrondi.
			if (round > 0.0m)
			{
				if (value < 0)
				{
					value -= round/2;
				}
				else
				{
					value += round/2;
				}

				return value - (value % round);
			}
			else
			{
				return value;
			}
		}


		public static AmortizationDetails Empty = new AmortizationDetails (AmortizationDefinition.Empty, ProrataDetails.Empty, null, null);

		public readonly AmortizationDefinition	Def;
		public readonly ProrataDetails			Prorata;

		public readonly decimal?				InitialValue;
		public readonly decimal?				BaseValue;
		public readonly decimal?				DeltaValue;
		public readonly decimal?				FinalValue;
	}
}
