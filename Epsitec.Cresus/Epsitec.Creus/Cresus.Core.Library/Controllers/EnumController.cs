//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// Ce contrôleur fait le pont entre un widget ItemPicker et une énumération C# quelconque.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EnumController<T> : IWidgetUpdater
		where T : struct  // <T> doit être une énumération C# native
	{
		public EnumController(IEnumerable<EnumKeyValues<T>> enumeration)
		{
			this.enumeration = enumeration;
		}


		public ValueToFormattedTextConverter ValueToDescriptionConverter
		{
			get;
			set;
		}

		public System.Action<T> ValueSetter
		{
			get;
			set;
		}

		public Expression<System.Func<T>> ValueGetter
		{
			set
			{
				this.valueGetterExpression = value;
				this.valueGetter = null;
			}
		}

		public T GetValue()
		{
			if (this.valueGetterExpression == null)
			{
				return default (T);
			}
			if (this.valueGetter == null)
			{
				this.valueGetter = this.valueGetterExpression.Compile ();
			}

			return this.valueGetter ();
		}


		public void Attach(ItemPickerButtons widget)
		{
			this.attachedWidget = widget;

			foreach (var e in this.enumeration)
			{
				widget.Items.Add (e.Key.ToString (), e);
			}

			widget.ValueToDescriptionConverter = delegate (object o)
			{
				var e = o as EnumKeyValues<T>;
				return this.ValueToDescriptionConverter (e.Values);
			};

			widget.Cardinality = EnumValueCardinality.ExactlyOne;
			widget.RefreshContents ();

			this.Update ();

			widget.SelectedItemChanged += delegate
			{
				var key = widget.Items.GetValue (widget.SelectedItemIndex) as EnumKeyValues<T>;
				T result = key.Key;
				this.ValueSetter (result);
			};
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.attachedWidget != null)
			{
				if (this.attachedWidget is ItemPickerButtons)
				{
					var itemPicker = this.attachedWidget as ItemPickerButtons;

					itemPicker.ClearSelection ();

					var initialValue = this.GetValue ();
					int index = itemPicker.Items.FindIndexByKey (initialValue.ToString ());

					if (index != -1)
					{
						itemPicker.AddSelection (new int[] { index });
					}
				}
				else
				{
				}
			}
		}

		#endregion

		private readonly IEnumerable<EnumKeyValues<T>> enumeration;

		private Expression<System.Func<T>> valueGetterExpression;
		private System.Func<T> valueGetter;
		private Widget attachedWidget;
	}
}