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

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionLegalPersonViewController : EntityViewController<Entities.LegalPersonEntity>
	{
		public EditionLegalPersonViewController(string name, Entities.LegalPersonEntity entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			UIBuilder builder = new UIBuilder (container);
			
			builder.CreateHeaderEditorTile ();

			var person = this.Entity;
			var group = builder.CreateGroupingTile ("Data.LegalPerson", "Personne morale", true);

			var accessor = new EntitiesAccessors.LegalPersonAccessor (null, person as Entities.LegalPersonEntity, false);
			var tile = builder.CreateEditionTile (group, accessor, this);

			builder.CreateFooterEditorTile ();

			builder.CreateTextField (tile.Container, 0, "Nom complet", accessor.LegalPerson.Name, x => accessor.LegalPerson.Name = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 150, "Nom court", accessor.LegalPerson.ShortName, x => accessor.LegalPerson.ShortName = x, Validators.StringValidator.Validate);
			builder.CreateMargin (tile.Container, true);
			builder.CreateTextFieldMulti (tile.Container, 100, "Complément", accessor.LegalPerson.Complement, x => accessor.LegalPerson.Complement = x, null);

			UI.SetInitialFocus (container);
		}
	}
}
