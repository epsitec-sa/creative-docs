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

		/// <summary>
		/// Updates this proxy with the widget. Read the connected widget properties.
		/// </summary>
		void Update();

		/// <summary>
		/// Gets the rank of the proxy in the user interface. Proxies with smaller
		/// values are displayed first
		/// </summary>
		/// <value>The rank.</value>
		int Rank
		{
			get;
		}

		/// <summary>
		/// Gets the base name of the icon, without prefix, suffix and extension.
		/// </summary>
		/// <value>The name of the icon.</value>
		string IconName
		{
			get;
		}

		/// <summary>
		/// Gets the width of the data column.
		/// </summary>
		/// <value>The pixel width of the data column.</value>
		double DataColumnWidth
		{
			get;
		}

		/// <summary>
		/// Gets the vertical spacing for rows.
		/// </summary>
		/// <value>The pixel spacing of the rows.</value>
		double RowsSpacing
		{
			get;
		}
	}
}
