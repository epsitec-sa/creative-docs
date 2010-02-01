//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.App.BanquePiguet
{

	class PrintDialog : Window
	{

		public PrintDialog(Application application)
		{
			this.Application = application;
		}

		protected Application Application
		{
			get;
			set;
		}


	}

}
