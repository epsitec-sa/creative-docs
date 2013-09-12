//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;


namespace Epsitec.Aider.Entities
{
	public partial class AiderCommentEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Text);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Text);
		}
		
		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Text.GetEntityStatus ());
				a.Accumulate (this.SystemText.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
