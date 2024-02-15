//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
    public class LockTimeoutException
        : System.ApplicationException,
            System.Runtime.Serialization.ISerializable
    {
        public LockTimeoutException()
            : base("Timed out waiting for lock.") { }
    }
}
