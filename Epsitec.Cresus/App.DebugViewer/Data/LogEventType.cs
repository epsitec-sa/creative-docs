//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DebugViewer.Data
{
	public enum LogEventType
	{
		None,
		SetDbField,
		SetLiveField,
		WidgetFocus,
		WindowFocus,
		WindowShow,
		MouseDown,
		Nav,
		UserMessage,
		Cmd,
		Trace,
	};
}
