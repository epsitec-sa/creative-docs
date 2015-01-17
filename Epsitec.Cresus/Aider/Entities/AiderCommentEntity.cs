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
		
		public static void CombineComments(IComment entity, string text)
		{
			if (string.IsNullOrWhiteSpace (text))
			{
				return;
			}

			//	With the null reference virtualizer, we don't need to handle explicitely the case
			//	when there is no comment defined yet.

			var comment = entity.Comment;
			var combinedText = TextFormatter.FormatText (comment.Text, "~\n\n", text);

			//	HACK This is a temporary hack to avoid texts with 800 or more chars with are not
			//	allowed in this field. The type of the field should be corrected to allow texts of
			//	unlimited size.

			if (combinedText.Length >= AiderCommentEntity.MaximumCommentLength)
			{
				return;
			}

			comment.Text = combinedText;
		}

		public static void CombineSystemComments(IComment entity, string text)
		{
			if (string.IsNullOrWhiteSpace (text))
			{
				return;
			}
			
			//	With the null reference virtualizer, we don't need to handle explicitely the case
			//	when there is no comment defined yet.

			var comment = entity.Comment;
			var combinedText = string.Concat (text, "\n\n", comment.SystemText ?? "").Trim ();

			//	HACK This is a temporary hack to avoid texts with 800 or more chars with are not
			//	allowed in this field. The type of the field should be corrected to allow texts of
			//	unlimited size.

			if (combinedText.Length >= AiderCommentEntity.MaximumCommentLength)
			{
				combinedText = combinedText.Substring (0, AiderCommentEntity.MaximumCommentLength-1);
			}

			comment.SystemText = combinedText;
		}

		public const int MaximumCommentLength = 800;
	}
}
