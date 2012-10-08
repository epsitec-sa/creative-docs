//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class SerializedDocumentBlobEntity
	{
		public override FormattedText GetSummary()
		{
			return this.GetCompactSummary ();
		}

		public override FormattedText GetCompactSummary()
		{
			return this.LastModificationDate.ToString ();
		}


		partial void OnDataChanged(byte[] oldValue, byte[] newValue)
		{
			this.SetHashes (newValue);
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.StrongHash.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}


		[Action (ActionClasses.Output, Library.Documents.Res.CaptionIds.ActionButton.Print)]
		public void Call()
		{
		}
	}
}
