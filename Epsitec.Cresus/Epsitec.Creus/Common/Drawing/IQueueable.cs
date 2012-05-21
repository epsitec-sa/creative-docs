//	Copyright © 2007-2008, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	internal interface IQueueable
	{
		QueueStatus QueueStatus
		{
			get;
		}

		void ChangePendingCounter(int change);
		void ChangeWorkingCounter(int change);
	}
}
