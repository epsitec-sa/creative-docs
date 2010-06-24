//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	/// <summary>
	/// The <c>ICreationStatus</c> interface returns the creation status for an
	/// entity.
	/// </summary>
	public interface ICreationStatus
	{
		/// <summary>
		/// Gets the creation status of the entity.
		/// </summary>
		/// <value>The creation status.</value>
		CreationStatus CreationStatus
		{
			get;
		}
	}
}
