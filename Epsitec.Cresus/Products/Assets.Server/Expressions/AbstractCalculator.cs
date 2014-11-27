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
			this.traceBuilder = new System.Text.StringBuilder ();

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


		public abstract decimal Evaluate();

		public string GetTraces()
		{
			//	ATTENTION: Même si VS dit que ce n'est pas utilisé, cela l'est quand
			//	même (AmortizationExpression.skeletonLines).
			return this.traceBuilder.ToString ();
		}


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
			//	Retourne la valeur arrondie selon this.RoundAmount.
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
			//	Retourne la valeur bornée selon this.ResidualAmount.
			return System.Math.Max (value, this.ResidualAmount);
		}

		protected decimal Override(decimal value)
		{
			//	Retourne la valeur imposée this.ForcedAmount si elle est définie.
			return this.ForcedAmount.GetValueOrDefault (value);
		}

		protected void Trace(params object[] args)
		{
			//	Conserve les textes donnés, qui seront affichés lors du debug
			//	de l'expression.
			foreach (var arg in args)
			{
				if (arg is decimal)
				{
					var value = (decimal) arg;
					this.traceBuilder.Append (value.ToString ("0.00"));
				}
				else
				{
					this.traceBuilder.Append (arg.ToString ());
				}

				this.traceBuilder.Append (" ");
			}

			this.traceBuilder.Append ("<br/>");
		}


		public struct Result
		{
			public Result(decimal value, string trace)
			{
				//	ATTENTION: Même si VS dit que ce n'est pas utilisé, cela l'est quand
				//	même (AmortizationExpression.skeletonLines).
				this.Value = value;
				this.Trace = trace;
				this.isEmpty = false;
			}

			private Result(bool isEmpty, decimal value, string trace)
			{
				this.Value = value;
				this.Trace = trace;
				this.isEmpty = isEmpty;
			}

			public bool IsEmpty
			{
				get
				{
					return this.isEmpty;
				}
			}

			public static Result Empty = new Result (true, 0, null);

			public readonly decimal				Value;
			public readonly string				Trace;
			private readonly bool				isEmpty;
		}


		public readonly System.Text.StringBuilder traceBuilder;

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
		public readonly decimal					YearRank;			// rang de l'année (0..YearCount-1)
	}
}
