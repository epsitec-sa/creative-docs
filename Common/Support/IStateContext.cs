//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// Restores the state of an object
    /// </summary>
    public interface IStateContext : IDisposable
    {
        public void RestorePreviousState();
    }
}
