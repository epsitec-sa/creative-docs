//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Export
{
	public struct ExportInstructions
	{
		public ExportInstructions(string filename, bool hasHeader, bool inverted)
		{
			this.Filename  = filename;
			this.HasHeader = hasHeader;
			this.Inverted  = inverted;
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.Filename);
			}
		}

		public static ExportInstructions Empty = new ExportInstructions (null, false, false);

		public readonly string					Filename;
		public readonly bool					HasHeader;
		public readonly bool					Inverted;
	}
}