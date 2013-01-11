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
		public ColumnDefinition(FormattedText title, ColumnType columnType = ColumnType.Absolute, double absoluteWidth = 0, double stretchFactor = 1, ContentAlignment alignment = ContentAlignment.TopLeft, double? fontSize = null)
		{
			this.Title         = title;
			this.ColumnType    = columnType;
			this.AbsoluteWidth = absoluteWidth;
			this.StretchFactor = stretchFactor;
			this.Alignment     = alignment;
			this.FontSize      = fontSize;
		}

		public FormattedText Title
		{
			set;
			get;
		}

		public ColumnType ColumnType
		{
			set;
			get;
		}

		public double AbsoluteWidth
		{
			//	Largeur absolue (en dixième de millimètre), en mode ColumnType.Absolute.
			set;
			get;
		}

		public double StretchFactor
		{
			//	Facteur multiplicatif, en mode ColumnType.Stretch.
			//	Par exemple:
			//		A: Absolute, AbsoluteWidth = 100
			//		B: Stretch,  StretchFactor = 1
			//		C: Stretch,  StretchFactor = 2
			//	Si la largeur à disposition est de 700, les colonnes auront:
			//		A: 100
			//		B: 200
			//		C: 400
			//	Autrement dit, les colonnes Stretch se partagent la place restante,
			//	selon les facteurs de stretch.
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
