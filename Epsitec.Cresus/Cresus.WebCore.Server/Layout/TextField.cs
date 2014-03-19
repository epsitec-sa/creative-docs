//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents an edition field used to edit a single line text value.
	/// </summary>
	internal sealed class TextField : AbstractField<TextField>
	{
		public bool								IsPassword
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["isPassword"] = this.IsPassword;

			return brick;
		}
	}
}
