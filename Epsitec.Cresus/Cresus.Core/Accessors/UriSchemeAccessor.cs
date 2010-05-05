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
	public class UriSchemeAccessor : AbstractContactAccessor<Entities.UriContactEntity>
	{
		public UriSchemeAccessor(object parentEntities, Entities.UriContactEntity entity, bool grouped)
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

			builder.Append (this.UriScheme);
			builder.Append ("<br/>");

			return builder.ToString ();
		}


		public ComboInitializer UriSchemeInitializer
		{
			get
			{
				ComboInitializer initializer = new ComboInitializer ();

				initializer.Content.Add ("mailto", "Mail");
				initializer.Content.Add ("callto", "Skype");
				initializer.Content.Add ("sip",    "Session");
				initializer.Content.Add ("http",   "Web");

				initializer.DefaultInternalContent = "mailto";

				return initializer;
			}
		}


		public string UriScheme
		{
			get
			{
				if (this.Entity.UriScheme != null)
				{
					return this.UriSchemeInitializer.ConvertInternalToEdition (this.Entity.UriScheme.Code);
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.UriScheme == null)
				{
					this.Entity.UriScheme = new Entities.UriSchemeEntity ();
				}

				this.Entity.UriScheme.Code = this.UriSchemeInitializer.ConvertEditionToInternal (value);
			}
		}
	}
}
