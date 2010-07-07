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


		public virtual double RequiredHeight(double width)
		{
			return 0;
		}


		public virtual void InitializePages(double width, double initialHeight, double middleheight, double finalHeight)
		{
		}

		public virtual int PageCount
		{
			get
			{
				return 1;
			}
		}

		public virtual void Paint(IPaintPort port, int page, Point topLeft)
		{
		}
	}
}
