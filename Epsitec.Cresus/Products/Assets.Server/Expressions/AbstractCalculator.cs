//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public abstract class AbstractCalculator
	{
		public AbstractCalculator(AmortizationDetails details)
		{
			this.traceBuilder = new System.Text.StringBuilder ();

			this.Range              = details.Def.Range;
			this.Date               = details.Def.Date;
			this.BaseAmount         = details.History.BaseAmount;
			this.StartYearAmount    = details.Def.StartYearAmount;
			this.InitialAmount      = details.History.InitialAmount;

			this.Periodicity        = details.Def.Periodicity;
			this.YearRank           = details.History.YearRank;
			this.PeriodRank         = details.History.PeriodRank;
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


		protected decimal PeriodCount
		{
			//	Retourne 1/2/4/12.
			get
			{
				return 12.0m / AmortizedAmount.GetPeriodMonthCount (this.Periodicity);
			}
		}

		protected decimal PeriodicityFactor
		{
			get
			{
				return AmortizedAmount.GetPeriodMonthCount (this.Periodicity) / 12.0m;
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

		//??protected decimal Override(decimal value)
		//??{
		//??	//	Retourne la valeur imposée this.ForcedAmount si elle est définie.
		//??	return this.ForcedAmount.GetValueOrDefault (value);
		//??}

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


		#region Prorata
		//	Retourne le facteur pour le prorata, compris entre 0.0 et 1.0.
		//	Pour une dateValue en début d'année, on retourne 1.0.
		//	Pour une dateValue en milieu d'année, on retourne 0.5.
		//	Pour une dateValue en fin d'année, on retourne 0.0.

		protected decimal ProrataFactor12
		{
			//	Retourne le facteur pour le prorata au mois.
			get
			{
				System.DateTime v;

				if (this.Date == this.Range.IncludeFrom)
				{
					v = this.Date;
				}
				else
				{
					//	Le mois en cours est amorti intégralement. Ainsi, un objet entré le 31 mars
					//	sera amorti tout le mois de mars.
					v = new System.DateTime (this.Date.Year, this.Date.Month, 1);
				}

				int n = AbstractCalculator.GetMonthsCount (v)                    - AbstractCalculator.GetMonthsCount (this.Range.IncludeFrom);
				int d = AbstractCalculator.GetMonthsCount (this.Range.ExcludeTo) - AbstractCalculator.GetMonthsCount (this.Range.IncludeFrom);

				n = d - n;
				n = System.Math.Max (n, 0);
				n = System.Math.Min (n, d);
				return (decimal) n / (decimal) d;
			}
		}

		protected decimal ProrataFactor360
		{
			get
			{
				//	Retourne le facteur pour le prorata au jour, sur la base d'une année de 360 jours.
				int n = AbstractCalculator.GetDaysCount (this.Date)            - AbstractCalculator.GetDaysCount (this.Range.IncludeFrom);
				int d = AbstractCalculator.GetDaysCount (this.Range.ExcludeTo) - AbstractCalculator.GetDaysCount (this.Range.IncludeFrom);

				n = d - n;
				n = System.Math.Max (n, 0);
				n = System.Math.Min (n, d);
				return (decimal) n / (decimal) d;
			}
		}

		protected decimal ProrataFactor365
		{
			//	Retourne le facteur pour le prorata au jour effectif.
			get
			{
				int n = this.Date.Subtract (this.Range.IncludeFrom).Days;
				int d = this.Range.ExcludeTo.Subtract (this.Range.IncludeFrom).Days;

				n = d - n;
				n = System.Math.Max (n, 0);
				n = System.Math.Min (n, d);
				return (decimal) n / (decimal) d;
			}
		}

		private static int GetMonthsCount(System.DateTime date)
		{
			//	Retourne le nombre de mois écoulés depuis le 01.01.0000.
			//	L'origine est sans importance, car le résultat est utilisé pour
			//	calculer une différence entre 2 dates !
			return date.Year*12
				+ (date.Month-1);
		}

		private static int GetDaysCount(System.DateTime date)
		{
			//	Retourne le nombre de jours écoulés depuis le 01.01.0000,
			//	en se basant sur 12 mois à 30 jours par année.
			//	L'origine est sans importance, car le résultat est utilisé pour
			//	calculer une différence entre 2 dates !
			return date.Year*12*30
				+ (date.Month-1)*30
				+ System.Math.Min ((date.Day-1), 30-1);
		}
		#endregion
	

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

		public readonly DateRange				Range;
		public readonly System.DateTime			Date;
		public readonly decimal					BaseAmount;			// valeur d'achat
		public readonly decimal					StartYearAmount;	// valeur au début de l'année
		public readonly decimal					InitialAmount;		// valeur avant amortissement

		public readonly decimal					Rate;				// taux d'amortissement
		public readonly decimal					YearCount;			// nombre total d'années
		public readonly Periodicity				Periodicity;
		public readonly decimal					RoundAmount;		// arrondi
		public readonly decimal					ResidualAmount;		// valeur résiduelle
		public readonly decimal					YearRank;			// rang de l'année (0..YearCount-1)
		public readonly decimal					PeriodRank;			// rang de la période (0..n)
	}
}
