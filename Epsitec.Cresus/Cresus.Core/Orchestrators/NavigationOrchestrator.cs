//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators
{
	public class NavigationOrchestrator
	{
		public void Add(CoreViewController parentController, CoreViewController controller)
		{
		}

		public void Remove(CoreViewController controller)
		{
		}


	}

	public class NavigationFieldNode
	{

		public EntityFieldPath Path
		{
			get;
			set;
		}

		public Marshaler Marshaler
		{
			get;
			set;
		}
	}

	public class NavigationViewNode : NavigationFieldNode
	{
		public Druid EntityId
		{
			get;
			set;
		}

		public ViewControllerMode Mode
		{
			get;
			set;
		}
	}
}
