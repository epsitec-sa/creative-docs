//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.Settings
{
	public class VectorValueSetting
	{
		public VectorValueSetting()
		{
		}

		public InclusionMode InclusionMode
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}
	}
}
