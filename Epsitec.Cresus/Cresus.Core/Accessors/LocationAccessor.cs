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
				return "Numéro postal et ville";
			}
		}

		protected override string GetSummary()
		{
			var builder = new StringBuilder ();

			builder.Append (Misc.SpacingAppend (this.Entity.PostalCode, this.Entity.Name));
			builder.Append ("<br/>");

			return builder.ToString ();
		}

		public override AbstractEntity Create()
		{
			return null;
		}


		public void HintInitialize(Widgets.HintEditor editor)
		{
			var locations = Controllers.MainViewController.locations;

			foreach (var location in locations)
			{
				editor.Items.Add (null, location);
			}

			editor.ValueToDescriptionConverter = LocationAccessor.HintValueToDescriptionConverter;
			editor.HintComparer = LocationAccessor.HintComparer;
		}

		private static string HintValueToDescriptionConverter(object value)
		{
			var entity = value as Entities.LocationEntity;

			return string.Format ("{0} {1}", entity.PostalCode, entity.Name);
		}

		private static Widgets.HintComparerResult HintComparer(object value, string hint)
		{
			var entity = value as Entities.LocationEntity;

			hint = Misc.RemoveAccentsToLower (hint);

			var result1 = Widgets.HintEditor.Compare (Misc.RemoveAccentsToLower (entity.PostalCode), hint);
			var result2 = Widgets.HintEditor.Compare (Misc.RemoveAccentsToLower (entity.Name), hint);

			return Widgets.HintEditor.Bestof (result1, result2);
		}


		public string LocationPostalCode
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

		public string LocationName
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
	}
}
