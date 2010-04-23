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
		public PersonViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.entity != null);

			Widgets.SimpleTile tile = new Widgets.SimpleTile
			{
				Parent = container,
				Dock = DockStyle.Fill,
				ArrowLocation = Direction.Right,
				IconUri = "Data.Person",
				Title = "Personne",
				Content = this.Description,
			};
		}


		/// <summary>
		/// Retourne un texte multiligne court de description d'une personne.
		/// </summary>
		/// <value>The description.</value>
		private string Description
		{
			get
			{
				Entities.AbstractPersonEntity person = this.entity as Entities.AbstractPersonEntity;

				if (person == null)
				{
					return null;
				}

				return person.Contacts[0].NaturalPerson.Firstname;  // TODO: !
			}
		}
	}
}
