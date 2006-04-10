using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Viewer
	{
		public Viewer(Module module)
		{
			this.module = module;
		}

		public void Dispose()
		{
		}


		protected Module					module;
	}
}
