using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core
{
	public static class Misc
	{
		public static string Icon(string icon)
		{
			//	Retourne le nom complet d'une icône.
			//?return string.Format ("manifest:Epsitec.Common.Designer.Images.{0}.icon", icon);
			return string.Format ("manifest:Epsitec.Cresus.Core.Images.{0}.icon", icon);
		}

	}
}
