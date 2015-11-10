//	Copyright © 2014-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Aider.Enumerations;
using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.Database;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeTaskEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("Tâche");
		}

		public override FormattedText GetSummary()
		{
			switch (this.Process.Type)
			{
				case OfficeProcessType.PersonsOutputProcess:
					return this.GetExitProcessSummary ();

			}
			return this.GetCompactSummary ();
		}

		public AiderPersonEntity GetExitProcessPerson()
		{
			return this.Process.GetSourceEntity<AiderPersonEntity> (this.GetDataContext ());
		}

		public IList<AiderGroupParticipantEntity> GetExitProcessParticipations()
		{	
			var person = this.GetExitProcessPerson ();
			return person
					.GetParticipations ()
					.Where (p => p.Group.IsInTheSameParish (this.Office.ParishGroup)).ToList ();
		}

		private FormattedText GetExitProcessSummary ()
		{
			var person = this.GetExitProcessPerson ();
			return TextFormatter.FormatText ("Sortie " + person.GetCompactSummary ());
		}
	}
}
