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
	public class NaturalPersonAccessor : AbstractAccessor
	{
		public NaturalPersonAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public Entities.NaturalPersonEntity NaturalPerson
		{
			get
			{
				return this.Entity as Entities.NaturalPersonEntity;
			}
		}


		public override string IconUri
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

		protected override string GetSummary()
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

			return builder.ToString ();
		}


		public ComboInitializer TitleInitializer
		{
			get
			{
				ComboInitializer initializer = new ComboInitializer ();

				initializer.Content.Add ("Monsieur",     "Monsieur");
				initializer.Content.Add ("Madame",       "Madame");
				initializer.Content.Add ("Mademoiselle", "Mademoiselle");

				return initializer;
			}
		}

		public ComboInitializer GenderInitializer
		{
			get
			{
				ComboInitializer initializer = new ComboInitializer ();

				initializer.Content.Add ("unknown", "Inconnu");
				initializer.Content.Add ("male",    "Homme");
				initializer.Content.Add ("female",  "Femme");

				initializer.DefaultInternalContent = "unknown";

				return initializer;
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

		public string Gender
		{
			get
			{
				if (this.NaturalPerson.Gender != null)
				{
					return this.NaturalPerson.Gender.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.NaturalPerson.Gender == null)
				{
					this.NaturalPerson.Gender = new Entities.PersonGenderEntity ();
				}

				this.NaturalPerson.Gender.Name = value;
			}
		}
	}
}
