//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.UI
{
	/// <summary>
	/// Summary description for TextRefAdapter.
	/// </summary>
	
	[Common.UI.Adapters.Controller (1, typeof (CommandController))]
	
	public class CommandAdapter : Common.UI.Adapters.AbstractStringAdapter
	{
		public CommandAdapter(Application application)
		{
			this.application = application;
		}
		
		
		
		public Application						Application
		{
			get
			{
				return this.application;
			}
		}
		
		
		private Application						application;
	}
}
