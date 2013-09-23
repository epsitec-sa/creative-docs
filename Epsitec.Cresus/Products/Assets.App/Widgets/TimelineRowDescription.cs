//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineRowDescription
	{
		public TimelineRowDescription(TimelineRowType type, string description, double relativeHeight = 1.0)
		{
			this.Type           = type;
			this.Description    = description;
			this.RelativeHeight = relativeHeight;
		}


		public readonly TimelineRowType			Type;
		public readonly string					Description;
		public readonly double					RelativeHeight;
		
		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Type);
			buffer.Append (" ");
			buffer.Append (this.RelativeHeight.ToString ());

			return buffer.ToString ();
		}
	}
}
