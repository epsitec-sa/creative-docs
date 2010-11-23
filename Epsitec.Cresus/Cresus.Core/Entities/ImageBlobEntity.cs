//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business.Finance;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ImageBlobEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.FileUri);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.FileName);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Code };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.FileName.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.FileUri.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.FileMimeType.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
