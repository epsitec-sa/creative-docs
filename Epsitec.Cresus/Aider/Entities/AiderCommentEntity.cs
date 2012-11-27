//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
	}
}
