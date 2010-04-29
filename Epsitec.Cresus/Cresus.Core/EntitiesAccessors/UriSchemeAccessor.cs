//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.EntitiesAccessors
{
	public class UriSchemeAccessor : AbstractContactAccessor
	{
		public UriSchemeAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public Entities.UriContactEntity UriContact
		{
			get
			{
				return this.Entity as Entities.UriContactEntity;
			}
		}


		public override string IconUri
		{
			get
			{
				return "T";
			}
		}

		public override string Title
		{
			get
			{
				return "Type";
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.UriScheme);
				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
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

				return initializer;
			}
		}


		public string UriScheme
		{
			get
			{
				if (this.UriContact.UriScheme != null)
				{
					return this.UriSchemeInitializer.ConvertInternalToEdition (this.UriContact.UriScheme.Code);
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.UriContact.UriScheme == null)
				{
					this.UriContact.UriScheme = new Entities.UriSchemeEntity ();
				}

				this.UriContact.UriScheme.Code = this.UriSchemeInitializer.ConvertEditionToInternal(value);
			}
		}
	}
}
