//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Views.TreeGraphicControllers
{
	public class TreeGraphicState
	{
		public TreeGraphicState()
		{
			this.fields = new List<ObjectField> ();
			this.fontFactors = new List<double> ();

			this.ColumnWidth = 100;
		}


		public double							ColumnWidth;

		public List<ObjectField>				Fields
		{
			get
			{
				return this.fields;
			}
		}

		public List<double>						FontFactors
		{
			get
			{
				return this.fontFactors;
			}
		}


		public SortingInstructions				SortingInstructions
		{
			get
			{
				if (this.fields.Count == 0)
				{
					return SortingInstructions.Empty;
				}
				else if (this.fields.Count == 1)
				{
					return new SortingInstructions (this.fields[0], SortedType.Ascending, ObjectField.Unknown, SortedType.None);
				}
				else
				{
					return new SortingInstructions (this.fields[0], SortedType.Ascending, this.fields[1], SortedType.Ascending);
				}
			}
		}


		private readonly List<ObjectField>		fields;
		private readonly List<double>			fontFactors;
	}
}
