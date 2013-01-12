//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Pdf.Common
{
	public abstract class CommonSetup
	{
		public CommonSetup()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins = new Margins (100.0);
			this.TextStyle   = new TextStyle ()
			{
				Font     = Font.GetFont ("Arial", "Regular"),
				FontSize = 30.0,
			};
		}

		public Margins PageMargins
		{
			//	Marges globales de la page.
			set;
			get;
		}

		public TextStyle TextStyle
		{
			//	Style pour tous les textes.
			set;
			get;
		}
	}
}
