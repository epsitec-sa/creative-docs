//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Graph
{
	/// <summary>
	/// Cette classe gère les échanges de données graphiques avec StringList.
	/// </summary>
	public class GraphicData
	{
		public GraphicData(GraphicMode mode, FormattedText name, decimal minValue, decimal maxValue)
		{
			this.mode     = mode;
			this.name     = name;
			this.minValue = minValue;
			this.maxValue = maxValue;

			this.values = new List<decimal> ();
		}


		public GraphicMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		public FormattedText Name
		{
			get
			{
				return this.name;
			}
		}

		public decimal MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public decimal MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public List<decimal> Values
		{
			get
			{
				return this.values;
			}
		}


		private readonly GraphicMode		mode;
		private readonly FormattedText		name;
		private readonly decimal			minValue;
		private readonly decimal			maxValue;
		private readonly List<decimal>		values;
	}
}
