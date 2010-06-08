//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class CountryAccessor : AbstractEntityAccessor<Entities.CountryEntity>
	{
		public CountryAccessor(object parentEntities, Entities.CountryEntity entity, bool grouped)
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
			var editor = widget as Widgets.HintEditor;
			var entity = unspecifiedEntitie as AbstractEntity;
			var countries = Controllers.MainViewController.countries;

			foreach (var country in countries)
			{
				editor.Items.Add (null, country);
			}

			editor.ValueToDescriptionConverter = CountryAccessor.HintValueToDescriptionConverter;
			editor.HintComparer = CountryAccessor.HintComparer;
			editor.HintComparisonConverter = Misc.RemoveAccentsToLower;

			editor.SelectedItemIndex = editor.Items.FindIndexByValue (entity);
		}

		private static FormattedText HintValueToDescriptionConverter(object value)
		{
			var entity = value as Entities.CountryEntity;

			return FormattedText.FromSimpleText (string.Format ("{0} ({1})", entity.Name, entity.Code));
		}

		private static Widgets.HintComparerResult HintComparer(object value, string typed)
		{
			var entity = value as Entities.CountryEntity;

			var result1 = Widgets.HintEditor.Compare (Misc.RemoveAccentsToLower (entity.Code), typed);
			var result2 = Widgets.HintEditor.Compare (Misc.RemoveAccentsToLower (entity.Name), typed);

			return Widgets.HintEditor.Bestof (result1, result2);
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

		public string Code
		{
			get
			{
				return this.Entity.Code;
			}
			set
			{
				this.Entity.Code = value;
			}
		}
	}
}
