//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	public interface IController
	{
		void CreateUserInterface();
		void DisposeUserInterface();

		Placeholder Placeholder
		{
			get;
			set;
		}
	}
}
