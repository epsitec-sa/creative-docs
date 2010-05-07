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


		public override void HintInitialize(Widgets.HintEditor editor)
		{
			var countries = Controllers.MainViewController.countries;

			foreach (var country in countries)
			{
				editor.Items.Add (null, country);
			}

			editor.ValueToDescriptionConverter = CountryAccessor.HintValueToDescriptionConverter;
			editor.HintComparer = CountryAccessor.HintComparer;
			editor.HintComparisonConverter = Misc.RemoveAccentsToLower;
		}

		private static string HintValueToDescriptionConverter(object value)
		{
			var entity = value as Entities.CountryEntity;

			return string.Format ("{0} ({1})", entity.Name, entity.Code);
		}

		private static Widgets.HintComparerResult HintComparer(object value, string hint)
		{
			var entity = value as Entities.CountryEntity;

			hint = Misc.RemoveAccentsToLower (hint);

			var result1 = Widgets.HintEditor.Compare (Misc.RemoveAccentsToLower (entity.Code), hint);
			var result2 = Widgets.HintEditor.Compare (Misc.RemoveAccentsToLower (entity.Name), hint);

			return Widgets.HintEditor.Bestof (result1, result2);
		}
	}
}
