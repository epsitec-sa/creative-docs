//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Widgets;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.TableDesigner
{
	public class DesignerTable
	{
		public DesignerTable()
		{
			this.dimensions = new List<DesignerDimension> ();
			this.values = new DesignerValues ();
		}


		public List<DesignerDimension> Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}

		public DesignerValues Values
		{
			get
			{
				return this.values;
			}
		}


		private readonly List<DesignerDimension>		dimensions;
		private readonly DesignerValues					values;
	}
}
