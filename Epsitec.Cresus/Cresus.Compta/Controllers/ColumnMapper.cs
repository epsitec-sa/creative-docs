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
		public ColumnMapper(ColumnType column, double relativeWidth, ContentAlignment alignment, FormattedText description, bool show = true, bool hideForSearch = false)
			: this (column, relativeWidth, alignment, description, FormattedText.Null, show, hideForSearch)
		{
		}

		public ColumnMapper(ColumnType column, double relativeWidth, FormattedText description, FormattedText tooltip, bool show = true, bool hideForSearch = false)
			: this (column, relativeWidth, ContentAlignment.MiddleLeft, description, tooltip, show, hideForSearch)
		{
		}

		public ColumnMapper(ColumnType column, double relativeWidth, ContentAlignment alignment, FormattedText description, FormattedText tooltip, bool show = true, bool hideForSearch = false)
		{
			this.Column        = column;
			this.RelativeWidth = relativeWidth;
			this.Alignment     = alignment;
			this.Description   = description;
			this.Tooltip       = tooltip;
			this.Show          = show;
			this.HideForSearch = hideForSearch;
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
			set;
		}

		public FormattedText Tooltip
		{
			get;
			private set;
		}

		public bool HideForSearch
		{
			get;
			private set;
		}

		public bool Show
		{
			get;
			set;
		}
	}
}
