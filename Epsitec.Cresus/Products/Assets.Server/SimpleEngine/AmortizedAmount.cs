//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public struct AmortizedAmount
	{
		public AmortizedAmount(DataAccessor accessor)
		{
			this.accessor = accessor;
			this.values = new AmortizedAmountValues ();
		}


		public EntryAccounts EntryAccounts
		{
			//	Retourne la liste des comptes à utiliser pour passer une écriture liée
			//	à l'événement contenant ce montant.
			get
			{
				if (this.accessor != null)
				{
					var obj = this.accessor.GetObject (BaseType.Assets, this.AssetGuid);
					if (obj != null)
					{
						var timestamp = new Timestamp (this.Date, 0);

						return new EntryAccounts
						(
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account1),
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account2),
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account3),
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account4),
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account5),
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account6),
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account7),
							ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.Account8)
						);
					}
				}

				return EntryAccounts.Empty;
			}
		}


		#region Facade
		public AmortizationType AmortizationType
		{
			get
			{
				return this.values.AmortizationType;
			}
			set
			{
				this.values.AmortizationType = value;
			}
		}

		public decimal? InitialAmount
		{
			get
			{
				return this.values.InitialAmount;
			}
			set
			{
				this.values.InitialAmount = value;
			}
		}

		public decimal? BaseAmount
		{
			get
			{
				return this.values.BaseAmount;
			}
			set
			{
				this.values.BaseAmount = value;
			}
		}

		public decimal? EffectiveRate
		{
			get
			{
				return this.values.EffectiveRate;
			}
			set
			{
				this.values.EffectiveRate = value;
			}
		}

		public decimal? ProrataNumerator
		{
			get
			{
				return this.values.ProrataNumerator;
			}
			set
			{
				this.values.ProrataNumerator = value;
			}
		}

		public decimal? ProrataDenominator
		{
			get
			{
				return this.values.ProrataDenominator;
			}
			set
			{
				this.values.ProrataDenominator = value;
			}
		}

		public decimal? RoundAmount
		{
			get
			{
				return this.values.RoundAmount;
			}
			set
			{
				this.values.RoundAmount = value;
			}
		}

		public decimal? ResidualAmount
		{
			get
			{
				return this.values.ResidualAmount;
			}
			set
			{
				this.values.ResidualAmount = value;
			}
		}

		public EntryScenario EntryScenario
		{
			get
			{
				return this.values.EntryScenario;
			}
			set
			{
				this.values.EntryScenario = value;
			}
		}

		public System.DateTime Date
		{
			get
			{
				return this.values.Date;
			}
			set
			{
				this.values.Date = value;
			}
		}

		public Guid AssetGuid
		{
			get
			{
				return this.values.AssetGuid;
			}
			set
			{
				this.values.AssetGuid = value;
			}
		}

		public Guid EventGuid
		{
			get
			{
				return this.values.EventGuid;
			}
			set
			{
				this.values.EventGuid = value;
			}
		}

		public Guid EntryGuid
		{
			get
			{
				return this.values.EntryGuid;
			}
			set
			{
				this.values.EntryGuid = value;
			}
		}
		#endregion


		public decimal?							FinalAmortizedAmount
		{
			//	Calcule la valeur amortie finale, en tenant compte de l'arrondi et de la
			//	valeur résiduelle.
			get
			{
				if (this.AmortizationType == AmortizationType.Unknown)
				{
					return this.InitialAmount;
				}
				else
				{
					var rounded = this.RoundedAmortizedAmount;

					if (rounded.HasValue && this.ResidualAmount.HasValue)
					{
						return System.Math.Max (rounded.Value, this.ResidualAmount.Value);
					}
					else
					{
						return rounded;
					}
				}
			}
		}

		public decimal?							RoundedAmortizedAmount
		{
			//	Calcule la valeur amortie arrondie, sans tenir compte de la valeur résiduelle.
			get
			{
				if (this.AmortizationType == AmortizationType.Unknown)
				{
					return this.InitialAmount;
				}
				else
				{
					var brut = this.BrutAmortizedAmount;

					if (brut.HasValue && this.RoundAmount.HasValue)
					{
						return AmortizedAmount.Round (brut.Value, this.RoundAmount.Value);
					}
					else
					{
						return brut;
					}
				}
			}
		}

		public decimal?							BrutAmortizedAmount
		{
			//	Calcule la valeur amortie, sans tenir compte de l'arrondi ni de la valeur
			//	résiduelle.
			get
			{
				return this.InitialAmount.GetValueOrDefault (0.0m) - this.BrutAmortization;
			}
		}

		public decimal							FinalAmortization
		{
			//	Calcule l'amortissement final effectif.
			get
			{
				return this.InitialAmount.GetValueOrDefault (0.0m)
					 - this.FinalAmortizedAmount.GetValueOrDefault (0.0m);
			}
		}

		public decimal							BrutAmortization
		{
			//	Calcule l'amortissement brut, qu'il faudra soustraire à la valeur initiale
			//	pour obtenir la valeur amortie.
			get
			{
				if (this.AmortizationType == AmortizationType.Unknown)
				{
					return 0.0m;
				}
				else
				{
					decimal value;
					if (this.AmortizationType == AmortizationType.Linear)
					{
						value = this.BaseAmount.GetValueOrDefault (0.0m);
					}
					else
					{
						value = this.InitialAmount.GetValueOrDefault (0.0m);
					}

					return value * this.EffectiveRate.GetValueOrDefault (1.0m) * this.Prorata;
				}
			}
		}

		public decimal							Prorata
		{
			//	Retourne le facteur multiplicateur "au prorata", compris entre 0 et 1.
			get
			{
				if (this.ProrataNumerator.HasValue &&
					this.ProrataDenominator.HasValue &&
					this.ProrataDenominator.Value != 0.0m)
				{
					var prorata = this.ProrataNumerator.Value/ this.ProrataDenominator.Value;

					prorata = System.Math.Max (prorata, 0.0m);
					prorata = System.Math.Min (prorata, 1.0m);  // garde-fou

					return prorata;
				}
				else
				{
					return 1.0m;  // 100%
				}
			}
		}


		public static bool operator ==(AmortizedAmount a, AmortizedAmount b)
		{
			return a.InitialAmount      == b.InitialAmount
				&& a.BaseAmount         == b.BaseAmount
				&& a.EffectiveRate      == b.EffectiveRate
				&& a.ProrataNumerator   == b.ProrataNumerator
				&& a.ProrataDenominator == b.ProrataDenominator
				&& a.RoundAmount        == b.RoundAmount
				&& a.ResidualAmount     == b.ResidualAmount
				&& a.AmortizationType   == b.AmortizationType;
		}

		public static bool operator !=(AmortizedAmount a, AmortizedAmount b)
		{
			return a.InitialAmount      != b.InitialAmount
				|| a.BaseAmount         != b.BaseAmount
				|| a.EffectiveRate      != b.EffectiveRate
				|| a.ProrataNumerator   != b.ProrataNumerator
				|| a.ProrataDenominator != b.ProrataDenominator
				|| a.RoundAmount        != b.RoundAmount
				|| a.ResidualAmount     != b.ResidualAmount
				|| a.AmortizationType   != b.AmortizationType;
		}


		public void CreateEntry()
		{
			if (this.accessor == null)
			{
				return;
			}

			using (var entries = new Entries (this.accessor))
			{
				entries.CreateEntry (this);
			}
		}

		public void RemoveEntry()
		{
			if (this.accessor == null)
			{
				return;
			}

			using (var entries = new Entries (this.accessor))
			{
				entries.RemoveEntry (this);
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


		private readonly DataAccessor			accessor;
		private readonly AmortizedAmountValues	values;
	}
}
