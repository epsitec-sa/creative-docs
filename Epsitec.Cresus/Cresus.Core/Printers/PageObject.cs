//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class PageObject
	{
		public PageObject(AbstractObject obj, int relativeObjectPageIndex, Point topLeft)
		{
			this.obj = obj;
			this.relativeObjectPageIndex = relativeObjectPageIndex;
			this.topLeft = topLeft;
		}

		public bool Paint(IPaintPort port)
		{
			return this.obj.Paint (port, this.relativeObjectPageIndex, this.topLeft);
		}


		private readonly AbstractObject obj;
		private readonly int relativeObjectPageIndex;
		private readonly Point topLeft;
	}
}
