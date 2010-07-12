//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationRelationViewController : CreationViewController<RelationEntity>
	{
		public CreationRelationViewController(string name, Entities.RelationEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(Widgets.TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreatePanelTitleTile ("Data.Customer", "Client à créer...");

				this.CreateUINewNaturalPersonButton (builder);
				this.CreateUINewLegalPersonButton (builder);	
				
				builder.EndPanelTitleTile ();
			}
		}

		private void CreateUINewNaturalPersonButton(UIBuilder builder)
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent ("Personne privée", "Crée un client de type personne privée"),
				PreferredHeight = 52,
			};

			button.Clicked +=
				delegate
				{
					this.CreateRealEntity (
						(context, customer) =>
						{
							customer.FirstContactDate = Date.Today;
							customer.Person = context.CreateEntity<NaturalPersonEntity> ();
						});
				};
			
			builder.Add (button);
		}

		private void CreateUINewLegalPersonButton(UIBuilder builder)
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent ("Entreprise", "Crée un client de type entreprise"),
				PreferredHeight = 52,
			};

			button.Clicked +=
				delegate
				{
					this.CreateRealEntity (
						(context, customer) =>
						{
							customer.FirstContactDate = Date.Today;
							customer.Person = context.CreateEntity<LegalPersonEntity> ();
						});
				};
			
			builder.Add (button);
		}

		
		protected override EditionStatus GetEditionStatus()
		{
			if (this.Entity.Person.UnwrapNullEntity () == null)
			{
				return EditionStatus.Empty;
			}
			else
			{
				return EditionStatus.Valid;
			}
		}
	}
}
