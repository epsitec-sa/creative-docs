//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// Ce contr�leur fait le pont entre un widget ItemPicker et une �num�ration quelconque.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EnumController<T> : IWidgetUpdater
		where T : struct
	{
		public EnumController(IEnumerable<EnumKeyValues<T>> enumeration)
		{
			this.enumeration = enumeration;
		}


		public System.Func<object, FormattedText> ValueToDescriptionConverter
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


		public void Attach(ItemPicker widget)
		{
			foreach (var e in this.enumeration)
			{
				widget.Items.Add (e.Key.ToString (), e);
			}

			widget.ValueToDescriptionConverter = delegate (object o)
			{
				var e = o as EnumKeyValues<T>;
				return this.ValueToDescriptionConverter (e.Values);
			};

			widget.Cardinality = BusinessLogic.EnumValueCardinality.ExactlyOne;
			widget.CreateUI ();

			var initialValue = this.GetValue ();
			int index = widget.Items.FindIndexByKey (initialValue.ToString ());
			if (index != -1)
			{
				widget.AddSelection (new int[] { index });
			}

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
#if false
			if (this.widget != null)
			{
				if (this.widget is ItemPicker)
				{
				}
				else
				{
				}
			}
#endif
		}

		#endregion

		private readonly IEnumerable<EnumKeyValues<T>> enumeration;

		private Expression<System.Func<T>> valueGetterExpression;
		private System.Func<T> valueGetter;
	}
}