//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TreeTableCellFirst
	{
		public TreeTableCellFirst(int level, TreeTableFirstType type, string description, double relativeHeight = 1.0)
		{
			this.Level          = level;
			this.Type           = type;
			this.Description    = description;
			this.RelativeHeight = relativeHeight;
		}


		public bool								IsInvalid
		{
			get
			{
				return this.Type == TreeTableFirstType.Empty;
			}
		}

		public bool								IsValid
		{
			get
			{
				return this.Type != TreeTableFirstType.Empty;
			}
		}


		public readonly int						Level;
		public readonly TreeTableFirstType		Type;
		public readonly string					Description;
		public readonly double					RelativeHeight;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Level.ToString ());
			buffer.Append (" ");
			buffer.Append (this.Type.ToString ());
			buffer.Append (" ");
			buffer.Append (this.Description);
			buffer.Append (" ");
			buffer.Append (this.RelativeHeight.ToString ());

			return buffer.ToString ();
		}
	}
}
