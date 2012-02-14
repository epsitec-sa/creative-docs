//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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
		public override void SetComptaEntity(ComptaEntity compta)
		{
			base.SetComptaEntity (compta);
			this.Clear ();
		}


		public override void Clear()
		{
			base.Clear ();

			this.Journal = this.comptaEntity.Journaux.FirstOrDefault ();
		}


		public ComptaJournalEntity Journal
		{
			//	null = tous les journaux
			get;
			set;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new JournalOptions ();
			this.emptyOptions.SetComptaEntity (this.comptaEntity);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new JournalOptions ();
			options.SetComptaEntity (this.comptaEntity);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			var d = dst as JournalOptions;
			d.Journal = this.Journal;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as JournalOptions;

			return this.Journal == o.Journal;
		}

		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (this.Journal == null)
				{
					this.AppendSummaryBuilder ("Tous les journaux");
				}
				else
				{
					this.AppendSummaryBuilder (string.Format ("Journal \"{0}\"", this.Journal.Nom));
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
