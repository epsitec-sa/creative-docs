//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>ICommandHandler</c> interface is used to identify classes which
    /// implement command handlers (see <see cref="CommandContext.GetCommandHandler"/>).
    /// </summary>
    public interface ICommandHandler
    {
        void UpdateCommandStates(object sender);
    }
}
