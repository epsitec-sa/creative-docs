//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentMetadataEntity
	{
		public FormattedText DocumentStateShortDescription
		{
			get
			{
				switch (this.DocumentState)
				{
					case Business.DocumentState.Draft:
						return "brouillon";

					case Business.DocumentState.Inactive:
						return "inactif";

					case Business.DocumentState.Active:
						return "actif";

					default:
						return FormattedText.Empty;
				}

			}
		}

		public FormattedText DocumentStateLongDescription
		{
			get
			{
				switch (this.DocumentState)
				{
					case Business.DocumentState.Draft:
						return "Document brouillon";

					case Business.DocumentState.Inactive:
						return "Document inactif";

					case Business.DocumentState.Active:
						return "Document actif";

					default:
						return FormattedText.Empty;
				}

			}
		}

		/// <summary>
		/// Gets a value indicating whether this document is frozen. This is related to
		/// the document's state; currently, only the <see cref="DocumentState.Draft"/>
		/// is considered to be not frozen (i.e. editable).
		/// </summary>
		/// <value>
		///   <c>true</c> if this document is frozen; otherwise, <c>false</c>.
		/// </value>
		public bool								IsFrozen
		{
			get
			{
				//?return false;  //? TODO: provisoire !!!
				switch (this.DocumentState)
				{
					case Business.DocumentState.None:
						return true;

					case Business.DocumentState.Draft:
						return false;
					
					case Business.DocumentState.Inactive:
					case Business.DocumentState.Active:
						return true;

					default:
						throw new System.NotSupportedException (string.Format ("DocumentState.{0} not supported", this.DocumentState));
				}
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
