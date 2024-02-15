//	Copyright © 2019-2020, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Cresus.WebCore.Server.Core
{
    public class AuthenticationResult
    {
        public AuthenticationResult(bool validUserPassword, User2FALogin user2FALogin)
        {
            this.ValidUserPassword = validUserPassword;
            this.User2FALogin = user2FALogin;
        }
        public bool ValidUserPassword { get; }
        public User2FALogin User2FALogin { get; }
    }
}
