//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>ActionClasses</c> enumeration defines classes used to categorize actions.
    /// See also <see cref="ActionInfo"/> and the <c>[Action]</c> attribute.
    /// </summary>
    public enum ActionClasses
    {
        None,

        Create,
        Delete,
        Validate,
        Clear,
        Cancel,

        Start,
        NextStep,
        Stop,

        Output,
        Input,
    }
}
