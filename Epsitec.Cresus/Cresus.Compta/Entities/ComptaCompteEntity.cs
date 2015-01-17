//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaCompteEntity
	{
		public FormattedText[] CodesTVAMenuDescription
		{
			//	Retourne la liste des descriptions des codes TVA pour le menu d'un AutoCompleteTextField.
			get
			{
				var list = new List<FormattedText> ();

				foreach (var codeTVA in this.CodesTVAPossibles.Where (x => !x.Désactivé))
				{
					list.Add (codeTVA.MenuDescription);
				}

				return list.ToArray ();
			}
		}

		public FormattedText CodesTVASummary
		{
			get
			{
				if (this.CodesTVAPossibles.Count == 0)
				{
					return FormattedText.Empty;
				}
				else
				{
					var list = new List<string> ();

					foreach (var codeTVA in this.CodesTVAPossibles)
					{
						list.Add (codeTVA.Code.ToString ());
					}

					return string.Format ("({0}×) {1}", this.CodesTVAPossibles.Count.ToString (), string.Join (", ", list));
				}
			}
		}


		public ComptaBudgetEntity GetBudget(ComptaPériodeEntity période)
		{
			return this.Budgets.Where (x => x.Période == période).FirstOrDefault ();
		}


		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.Numéro);
			yield return TextFormatter.FormatText (this.Titre);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Numéro, this.Titre);
		}
	}
}
