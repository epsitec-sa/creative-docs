//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (4)]
    public sealed class ActionAiderPersonWarningViewController4ProcessNewHousehold : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Traiter");
		}

		public override ActionExecutor GetExecutor()
		{
            return ActionExecutor.Create<bool,bool>(this.Execute);
		}

		private void Execute(bool createNewHousehold,bool subscribe)
		{
            this.Entity.Person.RemoveWarningInternal(this.Entity);
            this.BusinessContext.DeleteEntity(this.Entity);

            if (createNewHousehold)
            {
                var newHousehold = this.GetNewHousehold();
                var aiderHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity>();
                aiderHousehold.HouseholdMrMrs = HouseholdMrMrs.Auto;

                var aiderAddressEntity = aiderHousehold.Address;
                var eChAddressEntity = newHousehold.Address;


                var houseNumber = StringUtils.ParseNullableInt(SwissPostStreet.StripHouseNumber(eChAddressEntity.HouseNumber));
                var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement(eChAddressEntity.HouseNumber);

                if (string.IsNullOrWhiteSpace(houseNumberComplement))
                {
                    houseNumberComplement = null;
                }

                aiderAddressEntity.AddressLine1 = eChAddressEntity.AddressLine1;
                aiderAddressEntity.Street = eChAddressEntity.Street;
                aiderAddressEntity.HouseNumber = houseNumber;
                aiderAddressEntity.HouseNumberComplement = houseNumberComplement;
                aiderAddressEntity.Town = this.GetAiderTownEntity(newHousehold.Address);

                //Link household to ECh Entity
                if (newHousehold.Adult1.IsNotNull())
                {
                    EChDataImporter.SetupHousehold(this.BusinessContext, this.Entity.Person, aiderHousehold, newHousehold, isHead1: true);
                }

                if (newHousehold.Adult2.IsNotNull())
                {
                    EChDataImporter.SetupHousehold(this.BusinessContext, this.Entity.Person, aiderHousehold, newHousehold, isHead2: true);
                }

                foreach (var child in newHousehold.Children)
                {
                    EChDataImporter.SetupHousehold(this.BusinessContext, this.Entity.Person, aiderHousehold, newHousehold, isChild: true);
                }
                
            }
            if (subscribe)
            {

            }
		}

        private AiderTownEntity GetAiderTownEntity(eCH_AddressEntity address)
        {
            var townExample = new AiderTownEntity()
            {
                SwissZipCodeId = address.SwissZipCodeId
            };

            return this.BusinessContext.DataContext.GetByExample<AiderTownEntity>(townExample).FirstOrDefault();
        }

        private eCH_ReportedPersonEntity GetNewHousehold()
        {
            var echHouseholdExample = new eCH_ReportedPersonEntity()
            {
                Adult1 = this.Entity.Person.eCH_Person
            };
            return this.BusinessContext.DataContext.GetByExample<eCH_ReportedPersonEntity>(echHouseholdExample).FirstOrDefault();
        }

        protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
        {
            var householdAddress = this.Entity.Person.Address.StreetUserFriendly;
            var newHousehold = this.GetNewHousehold();
            var analyse = TextFormatter.FormatText("Résultat de l'analyse:\n");
            if (newHousehold.Address.StreetUserFriendly.Equals(householdAddress))
            {
                analyse = analyse.AppendLine(TextFormatter.FormatText("Ménage identique que le précédent!\n",newHousehold.Address.GetCompactSummary()));
                form
                .Title(this.GetTitle())
                .Text(analyse)
                .Field<bool>()
                    .Title("Créer le nouveau ménage")
                    .InitialValue(false)
                .End()
                .Field<bool>()
                    .Title("Souscrire à un abonnement BN")
                    .InitialValue(false)
                .End();
            }
            else
            {
                analyse = analyse.AppendLine(TextFormatter.FormatText("Nouveau ménage:\n",newHousehold.Address.GetCompactSummary()));
				form
					.Title (this.GetTitle ())
					.Text (analyse)
					.Field<bool> ()
						.Title ("Créer le nouveau ménage")
						.InitialValue (true)
					.End ()
					.Field<bool> ()
						.Title ("Souscrire à un abonnement BN")
						.InitialValue (true)
					.End ()
				.End ();
            } 
        }
	}
}
