//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;

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
