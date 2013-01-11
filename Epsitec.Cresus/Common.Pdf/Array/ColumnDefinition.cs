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
			//	Titre de la colonne, imprimé dans l'en-tête.
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
			//		Colonne A: Absolute, AbsoluteWidth = 100
			//		Colonne B: Stretch,  StretchFactor = 1
			//		Colonne C: Stretch,  StretchFactor = 2
			//	Si la largeur à disposition est de 700, les colonnes auront:
			//		Colonne A: 100
			//		Colonne B: 200
			//		Colonne C: 400
			//	Autrement dit, les colonnes Stretch se partagent la place restante,
			//	répartie selon les facteurs de stretch (StretchFactor).
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
