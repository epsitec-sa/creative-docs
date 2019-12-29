//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.WebCore.Server.Core
{
    public class AuthenticationResult
    {
        public AuthenticationResult(bool validUserPassword, bool requirePinValidation)
        {
            this.ValidUserPassword = validUserPassword;
            this.RequirePinValidation = requirePinValidation;
        }
        public bool ValidUserPassword { get; }
        public bool RequirePinValidation { get; }
    }
}
