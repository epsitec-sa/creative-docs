//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderPersonWarningEntity
	{
		public override FormattedText GetTitle()
		{
			return AiderWarningImplementation.GetTitle (this);
		}

		public override FormattedText GetSummary()
		{
			return AiderWarningImplementation.GetSummary (this);
		}

		public override FormattedText GetCompactSummary()
		{
			return AiderWarningImplementation.GetCompactSummary (this);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				AiderWarningImplementation.Accumulate (this, a);

				a.Accumulate (this.Person.IsNull () ? EntityStatus.Empty : EntityStatus.Valid);

				return a.EntityStatus;
			}
		}

		public static AiderPersonWarningEntity Create(BusinessContext businessContext, AiderPersonEntity person, string parishGroupPath,
													  WarningType warningType, FormattedText title, AiderWarningSourceEntity warningSource = null)
		{
			if ((string.IsNullOrEmpty (parishGroupPath)) ||
				(AiderGroupIds.IsWithinNoParish (parishGroupPath)))
			{
				return null;
			}

			if (AiderGroupIds.IsWithinParish (parishGroupPath) == false)
			{
				throw new System.ArgumentException ("Expected parish group path, got " + parishGroupPath);
			}

			var warning = businessContext.CreateAndRegisterEntity<AiderPersonWarningEntity> ();

			warning.StartDate = Date.Today;
			warning.EndDate   = null;

			warning.HideUntilDate = null;
			warning.Title         = title;
//-			warning.Description   = null;
			warning.WarningType   = warningType;
			warning.WarningTarget = WarningTarget.Person;
			warning.WarningSource = warningSource;

			warning.ParishGroupPath = parishGroupPath;
			warning.Person          = person;

			person.AddWarningInternal (warning);

			return warning;
		}

		public static AiderPersonWarningEntity Create(BusinessContext businessContext, AiderPersonEntity person, string parishGroupPath,
													  WarningType warningType, FormattedText title, FormattedText description,
													  AiderWarningSourceEntity warningSource = null)
		{
			var warning = AiderPersonWarningEntity.Create (businessContext, person, parishGroupPath, warningType, title, warningSource);

			if (warning != null)
			{
				warning.Description = description;
			}

			return warning;
		}

		public static void Delete(BusinessContext businessContext, AiderPersonWarningEntity warning)
		{
			if (warning.IsNotNull ())
			{
				var person = warning.Person;

				person.RemoveWarningInternal (warning);
				businessContext.DeleteEntity (warning);
			}
		}
	}
}