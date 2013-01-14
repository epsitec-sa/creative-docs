//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Array
{
	public class CellContent
	{
		public CellContent(FormattedText text)
		{
			this.text            = text;
			this.backgroundColor = Color.Empty;
		}

		public CellContent(FormattedText text, Color backgroundColor)
		{
			this.text            = text;
			this.backgroundColor = backgroundColor;
		}


		public FormattedText Text
		{
			get
			{
				return this.text;
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return this.backgroundColor;
			}
		}


		private readonly FormattedText text;
		private readonly Color backgroundColor;
	}
}
