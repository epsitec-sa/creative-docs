﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Accessors
{
	public class LocationAccessor : AbstractEntityAccessor<Entities.LocationEntity>
	{
		public LocationAccessor(object parentEntities, Entities.LocationEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public override string IconUri
		{
			get
			{
				return null;
			}
		}

		public override string Title
		{
			get
			{
				return null;
			}
		}

		protected override string GetSummary()
		{
			return null;
		}

		public override AbstractEntity Create()
		{
			return null;
		}


		public override void WidgetInitialize(Widget widget, object unspecifiedEntitie)
		{
			var editor = widget as Widgets.AutoCompleteTextField;
			var entity = unspecifiedEntitie as AbstractEntity;
			var locations = Controllers.MainViewController.locations;

			foreach (var location in locations)
			{
				editor.Items.Add (null, location);
			}

			editor.ValueToDescriptionConverter = LocationAccessor.HintValueToDescriptionConverter;
			editor.HintComparer = LocationAccessor.HintComparer;
			editor.HintComparisonConverter = Misc.RemoveAccentsToLower;

			editor.SelectedItemIndex = editor.Items.FindIndexByValue (entity);
		}

		private static FormattedText HintValueToDescriptionConverter(object value)
		{
			var entity = value as Entities.LocationEntity;

			return FormattedText.FromSimpleText (string.Format ("{0} {1}", entity.PostalCode, entity.Name));
		}

		private static Widgets.HintComparerResult HintComparer(object value, string typed)
		{
			var entity = value as Entities.LocationEntity;

			var result1 = Widgets.AutoCompleteTextField.Compare (Misc.RemoveAccentsToLower (entity.PostalCode), typed);
			var result2 = Widgets.AutoCompleteTextField.Compare (Misc.RemoveAccentsToLower (entity.Name), typed);

			return Widgets.AutoCompleteTextField.Bestof (result1, result2);
		}


		public string Name
		{
			get
			{
				return this.Entity.Name;
			}
			set
			{
				this.Entity.Name = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return this.Entity.PostalCode;
			}
			set
			{
				this.Entity.PostalCode = value;
			}
		}
	}
}
