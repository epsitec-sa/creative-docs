//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>IEntitySpecialController</c> interface is used to identify controllers which
	/// implement a special user interface to edit an entity.
	/// </summary>
	public interface IEntitySpecialController
	{
		void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly);
	}
}
