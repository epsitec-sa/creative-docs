//	Copyright © 2020, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
    public static class AdminFence
    {
        public static void Ensure(AiderUserEntity entity = default)
        {
            var user = AiderUserManager.Current.AuthenticatedUser;

            if ((entity != null) && AdminFence.IsValidUserChange (user, entity))
            {
                //  OK, same user as logged-in user... user may modify its own
                //  parameters
            }
            else if (user.HasPowerLevel (UserPowerLevel.AdminUser))
            {
                //  OK, user is an administrator and may change the user's password
            }
            else
            {
                var message = "Vous n'avez pas les droits nécessaires pour réaliser cette opération.";

                if (user == null)
                {
                    Logic.BusinessRuleException (message);
                }
                else
                {
                    Logic.BusinessRuleException (user, message);
                }
            }
        }

        private static bool IsValidUserChange(AiderUserEntity user, AiderUserEntity entity)
        {
            if ((user.LoginName != entity.LoginName) ||
                (user.Contact.DisplayName != entity.Contact.DisplayName) ||
                (user.Office.OfficeName != entity.Office.OfficeName) ||
                (user.DisplayName != entity.DisplayName) ||
                (user.SecondFactorMode != entity.SecondFactorMode) ||
                (user.Role.Name != entity.Role.Name) ||
                (user.PowerLevel != entity.PowerLevel) ||
                (user.Disabled != entity.Disabled) ||
                (user.EnableGroupEditionCanton != entity.EnableGroupEditionCanton) ||
                (user.EnableGroupEditionParish != entity.EnableGroupEditionParish) ||
                (user.EnableGroupEditionRegion != entity.EnableGroupEditionRegion))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
