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
	public class NaturalPersonAccessor : AbstractEntityAccessor<Entities.NaturalPersonEntity>
	{
		public NaturalPersonAccessor(object parentEntities, Entities.NaturalPersonEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
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

			if (this.Entity.Title != null)
			{
				var titleEntity = this.Entity.Title;
				builder.Append (titleEntity.Name);
				builder.Append ("<br/>");
			}

			builder.Append (Misc.SpacingAppend (this.Entity.Firstname, this.Entity.Lastname));
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
				if (this.Entity.Title != null)
				{
					return this.Entity.Title.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Title == null)
				{
					this.Entity.Title = new Entities.PersonTitleEntity ();
				}

				this.Entity.Title.Name = value;
			}
		}

		public string NaturalBirthDate
		{
			get
			{
				return this.Entity.BirthDate.ToString ();
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
				if (this.Entity.Gender != null)
				{
					return this.Entity.Gender.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Gender == null)
				{
					this.Entity.Gender = new Entities.PersonGenderEntity ();
				}

				this.Entity.Gender.Name = value;
			}
		}
	}
}
