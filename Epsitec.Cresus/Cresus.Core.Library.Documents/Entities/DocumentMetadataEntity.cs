//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentMetadataEntity
	{
		public FormattedText					DocumentStateShortDescription
		{
			get
			{
				var enumKeyValue = EnumKeyValues.GetEnumKeyValue (this.DocumentState);

				return enumKeyValue.Values.OrderBy (x => x.Length).FirstOrDefault ();
			}
		}

		public FormattedText					DocumentStateLongDescription
		{
			get
			{
				var enumKeyValue = EnumKeyValues.GetEnumKeyValue (this.DocumentState);

				return enumKeyValue.Values.OrderByDescending (x => x.Length).FirstOrDefault ();
			}
		}

		public bool								IsEditable
		{
			get
			{
				var state = this.DocumentState;
				
				switch (state & Business.DocumentState.ValueMask)
				{
					case Business.DocumentState.None:
					case Business.DocumentState.Valid:
						return false;

					case Business.DocumentState.Draft:
						if (state.HasFlag (Business.DocumentState.IsReferenced) ||
							state.HasFlag (Business.DocumentState.IsFrozen))
						{
							return false;
						}
						else
						{
							return true;
						}

					default:
						throw new System.NotSupportedException (string.Format ("DocumentState.{0} not supported", this.DocumentState));
				}
			}
		}

		public bool								IsValid
		{
			get
			{
				return (this.DocumentState & Business.DocumentState.ValueMask) == Business.DocumentState.Valid;
			}
		}

		public override FormattedText GetSummary()
		{
			var v = FormattedText.Empty;

			if (this.BusinessDocument.VariantId.HasValue)
			{
				v = FormattedText.Concat ("v", (this.BusinessDocument.VariantId+1).ToString ());
			}

			return TextFormatter.FormatText ("Document n°", this.IdA, ", ~", v, ", ~", this.DocumentStateShortDescription);
		}

		public override FormattedText GetCompactSummary()
		{
			if (this.DocumentCategory.IsNull ())
			{
				return TextFormatter.FormatText ("Document");
			}
			else
			{
				return TextFormatter.FormatText (this.DocumentCategory.Name);
			}
		}

		public FormattedText GetTitle()
		{
			return this.GetCompactSummary ();
		}


		public void SetDocumentStateValue(Business.DocumentState state)
		{
			var flags = this.DocumentState & Business.DocumentState.FlagsMask;

			this.DocumentState = state | flags;
		}

		public void SetDocumentStateFlag(Business.DocumentState flag, bool set = true)
		{
			if (set)
			{
				this.DocumentState = this.DocumentState.SetFlag (flag);
			}
			else
			{
				this.DocumentState = this.DocumentState.ClearFlag (flag);
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				a.Accumulate (this.DocumentTitle.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ());
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}
	}
}
