//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TreeTableColumnSeparator
	{
		public TreeTableColumnSeparator(int rank = -1, int direction = 0)
		{
			this.Rank      = rank;
			this.Direction = direction;
		}

		public readonly int Rank;
		public readonly int Direction;

		public bool Left
		{
			get
			{
				return this.Direction < 0;
			}
		}

		public bool Right
		{
			get
			{
				return this.Direction > 0;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Rank != -1;
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.Rank == -1;
			}
		}
	}
}
