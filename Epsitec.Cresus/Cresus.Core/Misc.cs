//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class Misc
	{
		public static string GetResourceIconUri(string icon)
		{
			//	Retourne le nom complet d'une icône.
			return string.Format ("manifest:Epsitec.Cresus.Core.Images.{0}.icon", icon);
		}

	}
}
