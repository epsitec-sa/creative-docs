using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer
{
	public interface IProxy
	{
		/// <summary>
		/// Adds the widget to the proxy. When a proxy is modified, all widgets
		/// connected to it will be updated.
		/// </summary>
		/// <param name="widget">The widget to add.</param>
		void AddWidget(Widget widget);

		int Rank
		{
			get;
		}
	}
}
