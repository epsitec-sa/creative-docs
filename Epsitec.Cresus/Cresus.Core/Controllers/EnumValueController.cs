//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	public class EnumValueController<T> : IWidgetUpdater
	{
		public EnumValueController(Marshaler marshaler, IEnumerable<EnumKeyValues<T>> possibleItems = null, System.Func<EnumKeyValues<T>, FormattedText> getUserText = null)
		{
			this.marshaler     = marshaler;
			this.possibleItems = possibleItems;
			this.getUserText   = getUserText;

			if (this.possibleItems != null)
			{
				System.Diagnostics.Debug.Assert (this.getUserText != null);
			}
		}


		public void Attach(AutoCompleteTextField widget)
		{
			foreach (var item in possibleItems)
			{
				var key = EnumConverter<T>.ConvertToNumericString (item.Key);

				widget.Items.Add (key, item);
			}

			widget.ValueToDescriptionConverter = value => this.getUserText (value as EnumKeyValues<T>);
			widget.HintComparer = (value, text) => EnumValueController<T>.MatchUserText (value as string[], text);
			widget.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);

			this.widget = widget;
			this.Update ();

			widget.AcceptingEdition +=
				delegate
				{
					int    index = widget.SelectedItemIndex;
					string key   = index < 0 ? null : widget.Items.GetKey (index);
					this.marshaler.SetStringValue (key);
				};

			widget.KeyboardFocusChanged += (sender, e) => this.Update ();
		}


		private static HintComparerResult MatchUserText(string[] value, string userText)
		{
			if (string.IsNullOrWhiteSpace (userText))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var result = Widgets.HintComparerResult.NoMatch;

			foreach (var text in value)
			{
				var itemText = TextConverter.ConvertToLowerAndStripAccents (text);
				result = Widgets.AutoCompleteTextField.Bestof (result, Widgets.AutoCompleteTextField.Compare (itemText, userText));
			}

			return result;
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.widget != null)
			{
				this.widget.SelectedItemIndex = this.widget.Items.FindIndexByKey (this.marshaler.GetStringValue ());
			}
		}

		#endregion

		private readonly Marshaler marshaler;
		private readonly IEnumerable<EnumKeyValues<T>> possibleItems;
		private readonly System.Func<EnumKeyValues<T>, FormattedText> getUserText;
		private AutoCompleteTextField widget;
	}

	public abstract class EnumKeyValues
	{
		public static EnumKeyValues<T> Create<T>(T key, params string[] values)
		{
			return new EnumKeyValues<T> (key, values);
		}

		public abstract string[] Values
		{
			get;
		}
	}

	public class EnumKeyValues<T> : EnumKeyValues
	{
		public EnumKeyValues(T key, params string[] values)
		{
			this.key = key;
			this.values = values;
		}

		
		public T Key
		{
			get
			{
				return this.key;
			}
		}

		public override string[] Values
		{
			get
			{
				return this.values;
			}
		}


		private readonly T key;
		private readonly string[] values;
	}
}
