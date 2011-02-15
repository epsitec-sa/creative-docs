//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Print.Bands
{
	public enum TableColumnKeys
	{
		Quantity,
		DelayedQuantity,
		DelayedDate,
		ArticleId,
		ArticleDescription,
		Discount,
		UnitPrice,
		LinePrice,
		Vat,
		Total,
	}


	/// <summary>
	/// Représente une colonne d'une table TableBand.
	/// Elle pourra être visible ou cachée.
	/// </summary>
	public class TableColumn
	{
		public TableColumn(string title, double width, ContentAlignment alignment)
		{
			this.Title     = title;
			this.Width     = width;
			this.Alignment = alignment;
			this.Rank      = -1;
			this.Visible   = false;
		}

		public string			Title;
		public double			Width;
		public ContentAlignment	Alignment;
		public int				Rank;
		public bool				Visible;
	}
}
