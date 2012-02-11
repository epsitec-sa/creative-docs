//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Widgets
{
	/// <summary>
	/// Ce widget 'attrape' la touche Tab, pour permettre une implémentation 100% personnelle.
	/// </summary>
	public class TabCatcherFrameBox : FrameBox
	{
		protected override bool PreProcessMessage(Message message, Point pos)
		{
			if (message.IsKeyType && message.KeyCode == KeyCode.Tab)
			{
				if (message.MessageType == MessageType.KeyDown)
				{
					this.OnTabPressed (message);
				}

				message.Swallowed = true;
				return true;
			}

			return base.PreProcessMessage (message, pos);
		}


		private void OnTabPressed(Message message)
		{
			if (this.TabPressed != null)
			{
				this.TabPressed (this, message);
			}
		}

		public delegate void TabPressedEventHandler(object sender, Message message);
		public event TabPressedEventHandler TabPressed;
	}
}
