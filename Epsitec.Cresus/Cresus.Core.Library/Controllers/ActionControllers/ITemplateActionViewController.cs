//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public interface ITemplateActionViewController : IActionViewController
	{
		bool RequiresAdditionalEntity
		{
			get;
		}
	}
}
