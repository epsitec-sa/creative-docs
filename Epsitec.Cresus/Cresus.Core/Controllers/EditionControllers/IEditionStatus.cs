//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	/// <summary>
	/// The <c>IEditionStatus</c> interface returns the edition status for an
	/// entity.
	/// </summary>
	public interface IEditionStatus
	{
		/// <summary>
		/// Gets the edition status of the entity.
		/// </summary>
		/// <value>The edition status.</value>
		EditionStatus EditionStatus
		{
			get;
		}
	}
}
