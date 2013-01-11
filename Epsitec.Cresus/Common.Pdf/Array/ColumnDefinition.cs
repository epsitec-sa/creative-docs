//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Array
{
	public class ColumnDefinition
	{
		public ColumnDefinition(FormattedText name, double? width, ContentAlignment alignment = ContentAlignment.TopLeft, double? fontSize = null)
		{
			this.Name      = name;
			this.Width     = width;
			this.Alignment = alignment;
			this.FontSize  = fontSize;
		}

		public FormattedText Name
		{
			set;
			get;
		}

		public double? Width
		{
			set;
			get;
		}

		public ContentAlignment Alignment
		{
			set;
			get;
		}

		public double? FontSize
		{
			set;
			get;
		}
	}
}
