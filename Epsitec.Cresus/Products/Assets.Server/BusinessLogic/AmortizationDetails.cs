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
			this.Def           = def;
			this.Prorata       = prorata;
							
			this.InitialValue  = initialValue;
			this.BaseValue     = baseValue;
			this.ForcedValue   = forcedValue;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Def.IsEmpty;
			}
		}

		public decimal?							FinalValue
		{
			//	Retourne la valeur finale amortie.
			//	S'il existe une valeur forcée, elle a la priorité.
			//	Sinon, la formule de base est:
			//	FinalValue = InitialValue - (BaseValue * EffectiveRate * ProrataQuotient)
			//	Avec encore un calcul de l'arrondi.
			get
			{
				if (!this.IsEmpty)
				{
					if (this.ForcedValue.HasValue)  // y a-t-il une valeur forcée ?
					{
						return this.ForcedValue;
					}
					else if (this.InitialValue.HasValue &&
							 this.BaseValue.HasValue &&
							 !this.Def.IsEmpty)
					{
						var delta = this.BaseValue.Value * this.Def.EffectiveRate;

						if (this.Prorata.Quotient.HasValue)
						{
							delta *= this.Prorata.Quotient.Value;
						}

						var value = this.InitialValue.Value - delta;

						value = AmortizationDetails.Round (value, this.Def.Round);

						return value;
					}
				}

				return null;
			}
		}


		public void AddAdditionnalFields(DataEvent e)
		{
			this.Def.AddAdditionnalFields (e);
			this.Prorata.AddAdditionnalFields (e);

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

			if (this.ForcedValue.HasValue)
			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsForcedValue, this.ForcedValue.Value);
				e.AddProperty (p);
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

		public readonly AmortizationDefinition Def;
		public readonly ProrataDetails		Prorata;

		public readonly decimal?			InitialValue;
		public readonly decimal?			BaseValue;
		public readonly decimal?			ForcedValue;
	}
}
