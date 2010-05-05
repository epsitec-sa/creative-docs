//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Accessors
{
	public class LegalPersonAccessor : AbstractAccessor
	{
		public LegalPersonAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public Entities.LegalPersonEntity LegalPerson
		{
			get
			{
				return this.Entity as Entities.LegalPersonEntity;
			}
		}


		public override string IconUri
		{
			get
			{
				return "Data.LegalPerson";
			}
		}

		public override string Title
		{
			get
			{
				return "Persone morale";
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.LegalPerson.Name);
				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}
	}
}
