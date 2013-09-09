//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Entities
{
	public partial class AiderWarningSourceEntity
	{
		public override FormattedText GetTitle()
		{
			return this.Name;
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Description, "\n", "Création:", this.CreationDate);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Description.Lines.FirstOrDefault ());
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.CreationDate.GetEntityStatus ());
				a.Accumulate (this.WarningEntity.GetEntityStatus ());

				return a.EntityStatus;
			}
		}

		partial void GetWarnings(ref IList<IAiderWarning> value)
		{
			if (this.warnings == null)
			{
				this.warnings = new List<IAiderWarning> ();
				
				//	TODO: resolve and load list of warnings
			}

			value = this.warnings;
		}

		public static TSource Create<TSource, TItem>(BusinessContext context, System.DateTime date, FormattedText name, FormattedText description)
			where TSource : AiderWarningSourceEntity, new ()
			where TItem : AbstractEntity, IAiderWarning, new ()
		{
			var warningSource = context.CreateEntity<TSource> ();

			warningSource.CreationDate  = date;
			warningSource.Name          = name;
			warningSource.Description   = description;
			warningSource.WarningEntity = EntityInfo<TItem>.GetTypeId ().ToString ();

			return warningSource;
		}


		private List<IAiderWarning>				warnings;
	}
}