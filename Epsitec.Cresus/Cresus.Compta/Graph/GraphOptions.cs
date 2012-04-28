//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

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
			this.startAtZero        = true;
			this.explodedPie        = true;
			this.hasLegend          = true;
			this.primaryDimension   = 1;
			this.secondaryDimension = 0;
			this.primaryFilter      = new List<FormattedText> ();
			this.secondaryFilter    = new List<FormattedText> ();
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

		public bool StartAtZero
		{
			get
			{
				return this.startAtZero;
			}
			set
			{
				this.startAtZero = value;
			}
		}

		public bool ExplodedPie
		{
			get
			{
				return this.explodedPie;
			}
			set
			{
				this.explodedPie = value;
			}
		}

		public bool HasLegend
		{
			get
			{
				return this.hasLegend;
			}
			set
			{
				this.hasLegend = value;
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

		public bool HasFilter
		{
			get
			{
				return this.primaryFilter.Count != 0 || this.secondaryFilter.Count != 0;
			}
		}

		public List<FormattedText> PrimaryFilter
		{
			get
			{
				return this.primaryFilter;
			}
		}

		public List<FormattedText> SecondaryFilter
		{
			get
			{
				return this.secondaryFilter;
			}
		}


		private GraphMode				mode;
		private GraphStyle				style;
		private bool					startAtZero;
		private bool					explodedPie;
		private bool					hasLegend;
		private int						primaryDimension;
		private int						secondaryDimension;
		private List<FormattedText>		primaryFilter;
		private List<FormattedText>		secondaryFilter;
	}
}
