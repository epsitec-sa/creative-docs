//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public struct ExportInstructions
	{
		public ExportInstructions(ExportFormat format, string filename)
		{
			this.Format    = format;
			this.Filename  = filename;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Format == ExportFormat.Unknown
					&& string.IsNullOrEmpty (this.Filename);
			}
		}

		public static ExportInstructions Empty = new ExportInstructions (ExportFormat.Unknown, null);

		public readonly ExportFormat			Format;
		public readonly string					Filename;
	}
}