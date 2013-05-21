//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Library
{
	public class NotificationMessage
	{
		public string Title
		{
			get;
			set;
		}

		public FormattedText Body
		{
			get;
			set;
		}

		public Druid Dataset
		{
			get;
			set;
		}

		public EntityKey EntityKey
		{
			get;
			set;
		}
	}
}

