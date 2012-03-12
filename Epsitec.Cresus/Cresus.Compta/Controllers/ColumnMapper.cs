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
		public ColumnMapper(FormattedText tooltip)
			: this (ColumnType.None, 0, ContentAlignment.MiddleLeft, FormattedText.Empty, tooltip, true, false)
		{
		}

		public ColumnMapper(ColumnType column, double relativeWidth, ContentAlignment alignment, FormattedText description, bool show = true, bool hideForSearch = false, bool edition = true)
			: this (column, relativeWidth, alignment, description, FormattedText.Null, show, hideForSearch, edition)
		{
		}

		public ColumnMapper(ColumnType column, double relativeWidth, FormattedText description, FormattedText tooltip, bool show = true, bool hideForSearch = false, bool edition = true)
			: this (column, relativeWidth, ContentAlignment.MiddleLeft, description, tooltip, show, hideForSearch, edition)
		{
		}

		public ColumnMapper(ColumnType column, FormattedText description, FormattedText tooltip, bool enable = true, bool show = true, int lineLayout = 0)
			: this (column, 0, ContentAlignment.MiddleLeft, description, tooltip, enable: enable, show: show, lineLayout: lineLayout)
		{
			//	Ce constructeur est utilisé par les assistants.
		}

		public ColumnMapper(ColumnType column, double relativeWidth, ContentAlignment alignment, FormattedText description, FormattedText tooltip, bool show = true, bool hideForSearch = false, bool edition = true, bool enable = true, int lineLayout = 0)
		{
			this.Column        = column;
			this.RelativeWidth = relativeWidth;
			this.Alignment     = alignment;
			this.Description   = description;
			this.Tooltip       = tooltip;
			this.Show          = show;
			this.HideForSearch = hideForSearch;
			this.Edition       = edition;
			this.Enable        = enable;
			this.LineLayout    = lineLayout;
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

		public bool Edition
		{
			get;
			private set;
		}

		public bool Enable
		{
			get;
			private set;
		}

		public int LineLayout
		{
			get;
			private set;
		}
	}
}
