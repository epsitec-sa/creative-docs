//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	/// <summary>
	/// Tout ce qui est défini ici est accessible par l'utilisateur dans les expressions
	/// qui déterminent le calcul de l'amortissement.
	/// </summary>
	public abstract class AbstractCalculator
	{
		public AbstractCalculator(AmortizationDetails details)
		{
			this.traceBuilder = new System.Text.StringBuilder ();

			this.Range       = details.Def.Range;
			this.CurrentDate = details.Def.CurrentDate;
			this.FirstDate   = details.History.FirstDate;
			this.FirstAmount = details.History.FirstAmount;
			this.BaseDate    = details.History.BaseDate;
			this.BaseAmount  = details.History.BaseAmount;
			this.InputAmount = details.History.InputAmount;
			this.Periodicity = details.Def.Periodicity;
		}


		public abstract decimal Evaluate();

		public string GetTraces()
		{
			//	ATTENTION: Même si VS dit que ce n'est pas utilisé, cela l'est quand
			//	même (AmortizationExpression.skeletonLines).
			return this.traceBuilder.ToString ();
		}

		public string Error;


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

		protected decimal Round(decimal value, decimal round)
		{
			//	Retourne la valeur arrondie.
			if (round > 0.0m)
			{
				if (value < 0.0m)
				{
					value -= round/2.0m;
				}
				else
				{
					value += round/2.0m;
				}

				value -= (value % round);
			}

			return value;
		}

		protected decimal Residual(decimal value, decimal residual)
		{
			//	Retourne la valeur bornée selon this.ResidualAmount.
			return System.Math.Max (value, residual);
		}

		protected void Trace(params object[] args)
		{
			//	Conserve les objets donnés, qui seront affichés lors du debug
			//	de l'expression.
			bool first = true;

			foreach (var arg in args)
			{
				if (!first)
				{
					this.traceBuilder.Append (" ");
				}

				if (arg is decimal)  // il peut s'agir d'un montant, d'un taux, d'un réel, etc.
				{
					var value = (decimal) arg;
					var s = TypeConverters.DecimalToString (value, 5);
					this.traceBuilder.Append (s);
				}
				else if (arg is int)
				{
					var i = (int) arg;
					var s = TypeConverters.IntToString (i);
					this.traceBuilder.Append (s);
				}
				else if (arg is System.DateTime)
				{
					var date = (System.DateTime) arg;
					var s = TypeConverters.DateToString (date);
					this.traceBuilder.Append (s);
				}
				else if (arg is DateRange)
				{
					var range = (DateRange) arg;
					var s1 = TypeConverters.DateToString (range.IncludeFrom);
					var s2 = TypeConverters.DateToString (range.ExcludeTo.AddTicks (-1));
					this.traceBuilder.Append (string.Concat (s1, "..", s2));
				}
				else
				{
					this.traceBuilder.Append (arg.ToString ());
				}

				first = false;
			}

			this.traceBuilder.Append ("<br/>");
		}


		protected System.DateTime StartDate
		{
			get
			{
				return this.Range.IncludeFrom;
			}
		}

		protected System.DateTime EndDate
		{
			get
			{
				return this.Range.ExcludeTo;
			}
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

				if (this.CurrentDate == this.Range.IncludeFrom)
				{
					v = this.CurrentDate;
				}
				else
				{
					//	Le mois en cours est amorti intégralement. Ainsi, un objet entré le 31 mars
					//	sera amorti tout le mois de mars.
					v = new System.DateTime (this.CurrentDate.Year, this.CurrentDate.Month, 1);
				}

				decimal n = AbstractCalculator.GetMonthsCount (v)                    - AbstractCalculator.GetMonthsCount (this.Range.IncludeFrom);
				decimal d = AbstractCalculator.GetMonthsCount (this.Range.ExcludeTo) - AbstractCalculator.GetMonthsCount (this.Range.IncludeFrom);

				n = d - n;
				n = System.Math.Max (n, 0);
				n = System.Math.Min (n, d);
				return n / d;
			}
		}

		protected decimal ProrataFactor360
		{
			get
			{
				//	Retourne le facteur pour le prorata au jour, sur la base d'une année de 360 jours.
				decimal n = AbstractCalculator.GetDaysCount (this.CurrentDate)     - AbstractCalculator.GetDaysCount (this.Range.IncludeFrom);
				decimal d = AbstractCalculator.GetDaysCount (this.Range.ExcludeTo) - AbstractCalculator.GetDaysCount (this.Range.IncludeFrom);

				n = d - n;
				n = System.Math.Max (n, 0);
				n = System.Math.Min (n, d);
				return n / d;
			}
		}

		protected decimal ProrataFactor365
		{
			//	Retourne le facteur pour le prorata au jour effectif.
			get
			{
				int n = this.CurrentDate.Subtract (this.Range.IncludeFrom).Days;
				int d = this.Range.ExcludeTo.Subtract (this.Range.IncludeFrom).Days;

				n = d - n;
				n = System.Math.Max (n, 0);
				n = System.Math.Min (n, d);
				return (decimal) n / (decimal) d;
			}
		}

		protected static decimal Days(System.DateTime a, System.DateTime b)
		{
			return System.Math.Abs (a.Subtract (b).Days);
		}

		protected static decimal Days30(System.DateTime a, System.DateTime b)
		{
			var aa = AbstractCalculator.GetDaysCount (a);
			var bb = AbstractCalculator.GetDaysCount (b);

			return System.Math.Abs (aa - bb);
		}

		protected static decimal Months(System.DateTime a, System.DateTime b)
		{
			var aa = AbstractCalculator.GetMonthsCount (a);
			var bb = AbstractCalculator.GetMonthsCount (b);

			return System.Math.Abs (aa - bb);
		}

		private static decimal GetMonthsCount(System.DateTime date)
		{
			//	Retourne le nombre de mois écoulés depuis le 01.01.0000.
			//	L'origine est sans importance, car le résultat est utilisé pour
			//	calculer une différence entre 2 dates !
			return date.Year*12
				+ (date.Month-1);
		}

		private static decimal GetDaysCount(System.DateTime date)
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
	

		public readonly System.Text.StringBuilder traceBuilder;

		public readonly DateRange				Range;
		public readonly System.DateTime			CurrentDate;
		public readonly System.DateTime			FirstDate;
		public readonly decimal					FirstAmount;		// valeur d'achat
		public readonly System.DateTime			BaseDate;
		public readonly decimal					BaseAmount;			// dernière valeur modifiée
		public readonly decimal					InputAmount;		// valeur avant amortissement
		public readonly Periodicity				Periodicity;
	}
}
