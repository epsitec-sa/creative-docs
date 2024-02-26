//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>ILabelGetter</c> interface provides a single method called <see cref="GetLabel"/>,
    /// used to retrieve a label with the specified level of detail.
    /// </summary>
    public interface ILabelGetter
    {
        FormattedText GetLabel(LabelDetailLevel detailLevel);
    }
}
