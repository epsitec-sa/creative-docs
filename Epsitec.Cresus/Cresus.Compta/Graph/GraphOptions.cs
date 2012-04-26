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
			this.mode               = GraphMode.SideBySide;
			this.style              = GraphStyle.Rainbow;
			this.primaryDimension   = 1;
			this.secondaryDimension = 0;
		}


		public GraphMode Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		public GraphStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
			}
		}

		public int PrimaryDimension
		{
			get
			{
				return this.primaryDimension;
			}
			set
			{
				this.primaryDimension = value;
			}
		}

		public int SecondaryDimension
		{
			get
			{
				return this.secondaryDimension;
			}
			set
			{
				this.secondaryDimension = value;
			}
		}


		private GraphMode			mode;
		private GraphStyle			style;
		private int					primaryDimension;
		private int					secondaryDimension;
	}
}
