//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public abstract class AbstractCalculator
	{
		public AbstractCalculator(AmortizedAmount amount)
		{
			this.ForcedAmount       = amount.ForcedAmount;
			this.BaseAmount         = amount.BaseAmount.GetValueOrDefault ();
			this.InitialAmount      = amount.InitialAmount.GetValueOrDefault ();
			this.ResidualAmount     = amount.ResidualAmount.GetValueOrDefault ();
			this.RoundAmount        = amount.RoundAmount.GetValueOrDefault ();
			this.Rate               = amount.Rate.GetValueOrDefault ();
			this.PeriodicityFactor  = amount.PeriodicityFactor;
			this.ProrataNumerator   = amount.ProrataNumerator.GetValueOrDefault ();
			this.ProrataDenominator = amount.ProrataDenominator.GetValueOrDefault ();
			this.YearCount          = amount.YearCount;
			this.YearRank           = amount.YearRank;
		}

		public abstract object Evaluate();


		#region Math facade
		//	Facade de System.Math pour les méthodes utiles dans le calcul d'un amortissement,
		//	avec uniquement le type decimal en entrée/sortie.

		protected decimal Abs(decimal x)
		{
			return System.Math.Abs (x);
		}

		protected decimal Truncate(decimal x)
		{
			return System.Math.Truncate (x);
		}

		protected decimal Min(decimal x, decimal y)
		{
			return System.Math.Min (x, y);
		}

		protected decimal Max(decimal x, decimal y)
		{
			return System.Math.Max (x, y);
		}

		protected decimal Floor(decimal x)
		{
			return System.Math.Floor (x);
		}

		protected decimal Ceiling(decimal x)
		{
			return System.Math.Ceiling (x);
		}

		protected decimal Pow(decimal x, decimal y)
		{
			return (decimal) System.Math.Pow ((double) x, (double) y);
		}

		protected decimal Exp(decimal x)
		{
			return (decimal) System.Math.Exp ((double) x);
		}

		protected decimal Log(decimal x, decimal y)
		{
			return (decimal) System.Math.Log ((double) x, (double) y);
		}

		protected decimal Log10(decimal x)
		{
			return (decimal) System.Math.Log10 ((double) x);
		}

		protected decimal Sqrt(decimal x)
		{
			return (decimal) System.Math.Sqrt ((double) x);
		}

		protected int Sign(decimal x)
		{
			return System.Math.Sign (x);
		}
		#endregion


		protected decimal ProrataFactor
		{
			get
			{
				if (this.ProrataDenominator != 0)
				{
					return this.ProrataNumerator / this.ProrataDenominator;
				}
				else
				{
					return 1;
				}
			}
		}

		protected decimal Round(decimal value)
		{
			if (this.RoundAmount > 0.0m)
			{
				if (value < 0.0m)
				{
					value -= this.RoundAmount/2.0m;
				}
				else
				{
					value += this.RoundAmount/2.0m;
				}

				value -= (value % this.RoundAmount);
			}

			return value;
		}

		protected decimal Residual(decimal value)
		{
			return System.Math.Max (value, this.ResidualAmount);
		}

		protected decimal Override(decimal value)
		{
			return this.ForcedAmount.GetValueOrDefault (value);
		}

		public readonly decimal?				ForcedAmount;		// valeur forcée facultative
		public readonly decimal					BaseAmount;			// valeur d'achat
		public readonly decimal					InitialAmount;		// valeur avant amortissement
		public readonly decimal					ResidualAmount;		// valeur résiduelle
		public readonly decimal					RoundAmount;		// arrondi
		public readonly decimal					Rate;				// taux d'amortissement
		public readonly decimal					PeriodicityFactor;	// facteur lié à la périodicité (0.25 si trimestriel par exemple)
		public readonly decimal					ProrataNumerator;	// numérateur du prorata
		public readonly decimal					ProrataDenominator;	// dénominateur du prorata
		public readonly decimal					YearCount;			// nombre total d'années
		public readonly int						YearRank;			// rang de l'année (0..YearCount-1)
	}
}
