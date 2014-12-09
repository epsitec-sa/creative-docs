//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP,

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Aider.Entities
{
	public partial class AiderPlaceEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}
		
		public AiderPlaceEntity Create(	BusinessContext context, 
										string name, 
										AiderOfficeManagementEntity office, 
										AiderAddressEntity address)
		{
			var newPlace = context.CreateAndRegisterEntity<AiderPlaceEntity> ();
			newPlace.Name = name;
			newPlace.Office = office;
			newPlace.Address = address;
			return newPlace;
		}

		public void Delete(BusinessContext context)
		{
			context.DeleteEntity (this);
		}
	}
}

