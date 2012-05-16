//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Options.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage du journal des écritures de la comptabilité.
	/// </summary>
	public class JournalOptions : AbstractOptions
	{
		public override void Clear()
		{
			base.Clear ();

			this.JournalId = 1;  // premier journal
		}


		public int JournalId
		{
			//	0 = tous les journaux
			get;
			set;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new JournalOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new JournalOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			if (dst == this)
			{
				return;
			}

			var d = dst as JournalOptions;
			d.JournalId = this.JournalId;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as JournalOptions;

			return this.JournalId == o.JournalId;
		}

		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (this.JournalId == 0)
				{
					this.AppendSummaryBuilder (JournalOptionsController.AllJournaux);
				}
				else
				{
					var journal = this.compta.Journaux.Where (x => x.Id == this.JournalId).FirstOrDefault ();

					if (journal == null)
					{
						this.AppendSummaryBuilder ("Journal inconnu");
					}
					else
					{
						this.AppendSummaryBuilder (string.Format ("Journal \"{0}\"", journal.Nom));
					}
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
