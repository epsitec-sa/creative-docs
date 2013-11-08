//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TreeTableCellTree
	{
		public TreeTableCellTree(bool isValid, int level, NodeType type, string value, bool isSelected = false, bool isError = false)
		{
			this.IsValid    = isValid;
			this.Level      = level;
			this.Type       = type;
			this.Value      = value;
			this.IsSelected = isSelected;
			this.IsError    = isError;
		}


		public readonly bool					IsValid;
		public readonly int						Level;
		public readonly NodeType				Type;
		public readonly string					Value;
		public readonly bool					IsSelected;
		public readonly bool					IsError;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			if (!this.IsValid)
			{
				buffer.Append ("invalid ");
			}

			buffer.Append (this.Level);
			buffer.Append (" ");
			buffer.Append (this.Type);
			buffer.Append (" ");
			buffer.Append (this.Value);
			buffer.Append (" ");

			if (this.IsSelected)
			{
				buffer.Append (" selected");
			}
			if (this.IsError)
			{
				buffer.Append (" error");
			}

			return buffer.ToString ();
		}
	}
}
