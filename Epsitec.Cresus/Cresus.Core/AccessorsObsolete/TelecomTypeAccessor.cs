//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Accessors
{
	public class TelecomTypeAccessor : AbstractEntityAccessor<Entities.TelecomContactEntity>
	{
		public TelecomTypeAccessor(object parentEntities, Entities.TelecomContactEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public override string IconUri
		{
			get
			{
				return "Data.Type";
			}
		}

		public override string Title
		{
			get
			{
				return "Type";
			}
		}

		protected override string GetSummary()
		{
			var builder = new StringBuilder ();

			builder.Append (this.TelecomType);
			builder.Append ("<br/>");

			return builder.ToString ();
		}


		public ComboInitializer TelecomTypeInitializer
		{
			get
			{
				ComboInitializer initializer = new ComboInitializer ();

				initializer.Content.Add ("fixnet", "Fixe");
				initializer.Content.Add ("mobile", "Mobile");
				initializer.Content.Add ("fax",    "Fax");

				initializer.DefaultInternalContent = "fixnet";

				return initializer;
			}
		}


		public string TelecomType
		{
			get
			{
				if (this.Entity.TelecomType != null)
				{
					return this.TelecomTypeInitializer.ConvertInternalToEdition (this.Entity.TelecomType.Code);
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.TelecomType == null)
				{
					this.Entity.TelecomType = new Entities.TelecomTypeEntity ();
				}

				this.Entity.TelecomType.Code = this.TelecomTypeInitializer.ConvertEditionToInternal (value);
			}
		}
	}
}
