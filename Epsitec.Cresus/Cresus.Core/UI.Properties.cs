//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (Epsitec.Cresus.Core.UI.Properties))]

namespace Epsitec.Cresus.Core
{
	public static partial class UI
	{
		internal sealed class Properties : DependencyObject
		{
			public static readonly DependencyProperty		IsWindowPositionSaverActiveProperty	= DependencyProperty<Properties>.RegisterAttached ("isWindowPositionSaverActive", typeof (bool));
		}
	}
}
