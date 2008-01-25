//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.FormEngine
{
	public interface IFormResourceProvider : IStructuredTypeProviderId
	{
		string GetXmlSource(Druid id);
		string GetCaptionDefaultLabel(Druid id);
		
		ResourceManager ResourceManager
		{
			get;
		}
	}
}
