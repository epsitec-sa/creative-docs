//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Cette structure contient tous les détails permettant de calculer un amortissement.
	/// </summary>
	public struct AmortizationDetails
	{
		public AmortizationDetails(AmortizationDefinition def, ProrataDetails prorata,
								   decimal? initialValue, decimal? baseValue, decimal? forcedValue)
		{
			this.Def          = def;
			this.Prorata      = prorata;
							
			this.InitialValue = initialValue;
			this.BaseValue    = baseValue;
			this.ForcedValue  = forcedValue;
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


		public void AddAdditionnalFields(DataEvent e)
		{
			//	Ajoute le contenu de la structure dans un événement, sous forme de
			//	champs additionnels.
			if (this.InitialValue.HasValue)
			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsInitialValue, this.InitialValue.Value);
				e.AddProperty (p);
			}

			if (this.BaseValue.HasValue)
			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsBaseValue, this.BaseValue.Value);
				e.AddProperty (p);
			}

			if (this.DeltaValue.HasValue)
			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsDeltaValue, this.DeltaValue.Value);
				e.AddProperty (p);
			}

			if (this.ForcedValue.HasValue)
			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsForcedValue, this.ForcedValue.Value);
				e.AddProperty (p);
			}

			this.Prorata.AddAdditionnalFields (e);
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
				if (this.ForcedValue.HasValue)  // y a-t-il une valeur forcée ?
				{
					finalValue = this.ForcedValue;
				}
				else if (this.InitialValue.HasValue &&
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
				}
			}
		}


		private static decimal Round(decimal value, decimal round)
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


		public static AmortizationDetails Empty = new AmortizationDetails (AmortizationDefinition.Empty, ProrataDetails.Empty, null, null, null);

		public readonly AmortizationDefinition	Def;
		public readonly ProrataDetails			Prorata;

		public readonly decimal?				InitialValue;
		public readonly decimal?				BaseValue;
		public readonly decimal?				ForcedValue;
		public readonly decimal?				DeltaValue;
		public readonly decimal?				FinalValue;
	}
}
