//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ReferencePlaceholder))]

namespace Epsitec.Common.UI
{
	public class ReferencePlaceholder : Placeholder
	{
		public ReferencePlaceholder()
		{
		}

		public StructuredType EntityType
		{
			get;
			set;
		}



	}
}
