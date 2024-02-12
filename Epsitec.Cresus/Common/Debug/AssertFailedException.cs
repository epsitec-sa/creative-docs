//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
    /// <summary>
    /// La classe AssertFailedException permet de signaler l'échec d'une
    /// assertion.
    /// </summary>
    public class AssertFailedException : FailureException
    {
        public AssertFailedException() { }

        public AssertFailedException(string message)
            : base(string.Format("Assert failed: {0}", message)) { }
    }
}
