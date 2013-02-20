using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	public class DonnéesDuBudget : AbstractObjetComptable
	{
		public decimal?			PériodePrécédente;
		public decimal?			PériodePénultième;
		public decimal?			Budget;
		public decimal?			BudgetProrata;
		public decimal?			BudgetFutur;
		public decimal?			BudgetFuturProrata;
	}
}
