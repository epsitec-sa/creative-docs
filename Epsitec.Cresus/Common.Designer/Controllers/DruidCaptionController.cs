//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Controllers;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

[assembly: Controller(typeof(Epsitec.Common.Designer.Controllers.DruidCaptionController))]

namespace Epsitec.Common.Designer.Controllers
{
	public class DruidCaptionController : AbstractDruidController
	{
		public DruidCaptionController(string parameter) : base(parameter)
		{
		}
	}
}
