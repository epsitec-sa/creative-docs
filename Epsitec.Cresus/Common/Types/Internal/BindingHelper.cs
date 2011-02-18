//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Internal;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (BindingHelper))]

namespace Epsitec.Common.Types.Internal
{
	internal class BindingHelper : DependencyObject
	{
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (object), typeof (BindingHelper));
	}
}
