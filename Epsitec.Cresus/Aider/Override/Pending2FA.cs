//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Aider.Override
{
    public class Pending2FA
    {
        public Pending2FA(string loginName, string pin)
        {
            this.LoginName = loginName;
            this.Pin = pin;
            this.DateTime = System.DateTime.UtcNow;
        }

        public string LoginName { get; }
        public string Pin { get; }
        public System.DateTime DateTime { get; }
    }
}
