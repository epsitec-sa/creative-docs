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
	public abstract class AbstractObject
	{
		public AbstractObject()
		{
			this.Font = Font.GetFont ("Arial", "Regular");
			this.FontSize = 3.0;
		}


		public Rectangle Bounds
		{
			get;
			set;
		}

		public Font Font
		{
			get;
			set;
		}

		public double FontSize
		{
			get;
			set;
		}


		public virtual void Paint(IPaintPort port)
		{
		}
	}
}
