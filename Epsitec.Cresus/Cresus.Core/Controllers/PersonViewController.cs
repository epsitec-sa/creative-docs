//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class PersonViewController : AbstractViewController
	{
		public PersonViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
		{
			this.controllers = new List<CoreController> ();
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			foreach (CoreController controller in this.controllers)
			{
				yield return controller;
			}
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.entity != null);

			Entities.AbstractPersonEntity person = this.entity as Entities.AbstractPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			int index = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				Widgets.SimpleTile tile = new Widgets.SimpleTile
				{
					Parent = container,
					Dock = DockStyle.Top,
					Margins = new Margins(0, 0, 0, (index<person.Contacts.Count-1) ? -1:0),
					ArrowLocation = Direction.Right,
					IconUri = "Data.Person",
					Title = "Personne",
					Content = this.Description(contact),
				};

				index++;
			}
		}


		/// <summary>
		/// Retourne un texte multiligne court de description d'une personne.
		/// </summary>
		/// <value>The description.</value>
		private string Description(Entities.AbstractContactEntity contact)
		{
				Entities.AbstractPersonEntity person = this.entity as Entities.AbstractPersonEntity;

				if (person == null)
				{
					return null;
				}

				StringBuilder builder = new StringBuilder ();

				builder.Append (Misc.SpacingAppend(contact.NaturalPerson.Firstname, contact.NaturalPerson.Lastname));
				builder.Append ("<br/>");

				return builder.ToString ();
		}


		private List<CoreController> controllers;
	}
}
