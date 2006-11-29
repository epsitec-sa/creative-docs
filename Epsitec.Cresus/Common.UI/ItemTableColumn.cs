//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemTableColumn))]

namespace Epsitec.Common.UI
{
	using GridLength=Epsitec.Common.Widgets.Layouts.GridLength;

	public class ItemTableColumn : DependencyObject
	{
		public ItemTableColumn()
		{
		}

		public ItemTableColumn(string fieldId)
			: this ()
		{
			this.FieldId = fieldId;
		}

		public ItemTableColumn(string fieldId, double width)
			: this (fieldId, new GridLength (width))
		{
		}

		public ItemTableColumn(string fieldId, GridLength width)
			: this (fieldId)
		{
			this.Width = width;
		}

		public string FieldId
		{
			get
			{
				return (string) this.GetValue (ItemTableColumn.FieldIdProperty);
			}
			set
			{
				this.SetValue (ItemTableColumn.FieldIdProperty, value);
			}
		}

		public GridLength Width
		{
			get
			{
				return (GridLength) this.GetValue (ItemTableColumn.WidthProperty);
			}
			set
			{
				this.SetValue (ItemTableColumn.WidthProperty, value);
			}
		}

		public static readonly DependencyProperty FieldIdProperty	= DependencyProperty.Register ("FieldId", typeof (string), typeof (ItemTableColumn));
		public static readonly DependencyProperty WidthProperty		= DependencyProperty.Register ("Width", typeof (GridLength), typeof (ItemTableColumn));
	}
}
