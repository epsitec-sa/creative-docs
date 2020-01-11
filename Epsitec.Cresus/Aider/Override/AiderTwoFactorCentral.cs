//	Copyright © 2020, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Override
{
    /// <summary>
    /// The <c>AiderTwoFactorCentral</c> is a thread safe singleton class
    /// which can can be used to generate PINs, store them and handle user
    /// 2FA access requests.
    /// </summary>
    public sealed class AiderTwoFactorCentral
    {
        private AiderTwoFactorCentral()
        {
            this.pending2FApins = new Dictionary<string, Pending2FA> ();
            this.randomGenerator = new System.Random ();
        }


        public int GetRandomNumber(int min, int max)
        {
            lock (this.randomGenerator)
            {
                return this.randomGenerator.Next (min, max + 1);
            }
        }

        public void AddPin(string loginName, string pin)
        {
            lock (this.pending2FApins)
            {
                var expiredLoginNames = this.pending2FApins.Where (x => x.Value.ElapsedSeconds >= 60).Select (x => x.Key).ToList ();
                
                expiredLoginNames.ForEach (x => this.RemovePin (x));
                
                this.pending2FApins[loginName] = new Pending2FA (loginName, pin);
                System.Console.WriteLine ("Add PIN {0} for {1}", pin, loginName);
            }
        }

        public void RemovePin(string loginName)
        {
            lock (this.pending2FApins)
            {
                System.Console.WriteLine ("Remove {0}", loginName);
                this.pending2FApins.Remove (loginName);
            }
        }

        public bool CheckPin(string loginName, string pin)
        {
            lock (this.pending2FApins)
            {
                if ((this.pending2FApins.TryGetValue (loginName, out var info)) &&
                    (info.Pin == pin) &&
                    (info.ElapsedSeconds < 60))
                {
                    return true;
                }
            }

            return false;
        }

        public static AiderTwoFactorCentral Instance => AiderTwoFactorCentral.instance.Value;

        private static readonly System.Lazy<AiderTwoFactorCentral> instance = new System.Lazy<AiderTwoFactorCentral> (() => new AiderTwoFactorCentral (), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private readonly Dictionary<string, Pending2FA> pending2FApins;
        private readonly System.Random randomGenerator;
    }
}
