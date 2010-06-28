//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	/// <summary>
	/// The <c>ICreationController</c> interface provides some creation related
	/// methods, independently of the underlying entity type.
	/// </summary>
	public interface ICreationController
	{
		/// <summary>
		/// Gets the upgrade controller mode based on the active entity.
		/// </summary>
		/// <value>The upgrade controller mode.</value>
		ViewControllerMode UpgradeControllerMode
		{
			get;
		}
	}
}
