//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class ColumnMapper
	{
		public ColumnMapper(ColumnType column, double relativeWidth, ContentAlignment alignment, FormattedText description)
			: this (column, relativeWidth, alignment, description, FormattedText.Null)
		{
		}

		public ColumnMapper(ColumnType column, double relativeWidth, FormattedText description, FormattedText tooltip)
			: this (column, relativeWidth, ContentAlignment.MiddleLeft, description, tooltip)
		{
		}

		public ColumnMapper(ColumnType column, double relativeWidth, ContentAlignment alignment, FormattedText description, FormattedText tooltip)
		{
			this.Column        = column;
			this.RelativeWidth = relativeWidth;
			this.Alignment     = alignment;
			this.Description   = description;
			this.Tooltip       = tooltip;
		}

		public ColumnType Column
		{
			get;
			private set;
		}

		public double RelativeWidth
		{
			get;
			private set;
		}

		public ContentAlignment Alignment
		{
			get;
			private set;
		}

		public FormattedText Description
		{
			get;
			private set;
		}

		public FormattedText Tooltip
		{
			get;
			private set;
		}
	}
}
