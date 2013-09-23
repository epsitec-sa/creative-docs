//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineRowDescription
	{
		public TimelineRowDescription(TimelineRowType type)
		{
			this.Type = type;
		}

		
		public readonly TimelineRowType			Type;
		
		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Type);

			return buffer.ToString ();
		}
	}
}
