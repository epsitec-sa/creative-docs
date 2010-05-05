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
	public class LegalPersonAccessor : AbstractEntityAccessor<Entities.LegalPersonEntity>
	{
		public LegalPersonAccessor(object parentEntities, Entities.LegalPersonEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
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

		protected override string GetSummary()
		{
			var builder = new StringBuilder ();

			builder.Append (this.Entity.Name);
			builder.Append ("<br/>");

			return builder.ToString ();
		}
	}
}
