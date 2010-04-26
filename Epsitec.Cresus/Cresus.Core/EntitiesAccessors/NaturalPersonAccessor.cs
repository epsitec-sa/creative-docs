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
	public class NaturalPersonAccessor : AbstractAccessor
	{
		public NaturalPersonAccessor(AbstractEntity entity, bool grouped)
			: base (entity, grouped)
		{
		}


		public Entities.NaturalPersonEntity NaturalPerson
		{
			get
			{
				return this.Entity as Entities.NaturalPersonEntity;
			}
		}


		public override string Icon
		{
			get
			{
				return "Data.NaturalPerson";
			}
		}

		public override string Title
		{
			get
			{
				return "Personne physique";
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				if (this.NaturalPerson.Title != null)
				{
					var titleEntity = this.NaturalPerson.Title;
					builder.Append (titleEntity.Name);
					builder.Append ("<br/>");
				}

				builder.Append (Misc.SpacingAppend (this.NaturalPerson.Firstname, this.NaturalPerson.Lastname));
				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}


		public string NaturalTitle
		{
			get
			{
				if (this.NaturalPerson.Title != null)
				{
					return this.NaturalPerson.Title.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.NaturalPerson.Title == null)
				{
					this.NaturalPerson.Title = new Entities.PersonTitleEntity ();
				}

				this.NaturalPerson.Title.Name = value;
			}
		}

		public string NaturalBirthDate
		{
			get
			{
				return this.NaturalPerson.BirthDate.ToString ();
			}
			set
			{
				//?this.NaturalPerson.BirthDate = Date.FromString(value);  // TODO:
			}
		}
	}
}
