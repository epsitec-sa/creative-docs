﻿//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Reporting;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderEventPlaceEntity
	{
		public static AiderEventPlaceEntity Create(BusinessContext context, string placeName,  bool isSharedPlace, AiderOfficeManagementEntity office)
		{
			var place = context.CreateAndRegisterEntity<AiderEventPlaceEntity> ();
			place.Name        = placeName;
			place.OfficeOwner = office;
			place.Shared      = isSharedPlace;
			return place;
		}
	}
}
