//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	public abstract class AbstractColumnMapper<ColumnType>
	{
		public AbstractColumnMapper(ColumnType column, ValidateFunction validate, double relativeWidth, FormattedText description, FormattedText tooltip)
			: this (column, validate, relativeWidth, ContentAlignment.MiddleLeft, description, tooltip)
		{
		}

		public AbstractColumnMapper(ColumnType column, ValidateFunction validate, double relativeWidth, ContentAlignment alignment, FormattedText description, FormattedText tooltip)
		{
			this.Column        = column;
			this.Validate      = validate;
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

		public ValidateFunction Validate
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


		public delegate FormattedText ValidateFunction(ColumnType column, ref FormattedText text);
	}
}
