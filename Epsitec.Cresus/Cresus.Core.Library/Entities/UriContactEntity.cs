//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class UriContactEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Uri, "(", FormattedText.Join (", ", this.ContactGroups.Select (role => role.Name).ToArray ()), ")");
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Uri);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Uri.GetEntityStatus ());
				a.Accumulate (this.ContactGroups.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}


		[Action (ActionClasses.Output, Library.Address.Res.CaptionIds.ActionButton.Mail)]
		public void SendMail()
		{
		}
	}
}
