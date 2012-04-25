//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphOptions
	{
		public GraphOptions()
		{
			this.Mode               = GraphMode.SideBySide;
			this.Style              = GraphStyle.Rainbow;
			this.PrimaryDimension   = 1;
			this.SecondaryDimension = 0;
		}


		public GraphMode Mode
		{
			get;
			set;
		}

		public GraphStyle Style
		{
			get;
			set;
		}

		public int PrimaryDimension
		{
			get;
			set;
		}

		public int SecondaryDimension
		{
			get;
			set;
		}
	}
}
