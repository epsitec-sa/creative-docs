//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public sealed class CatcherFrameBox : FrameBox
	{
		public CatcherFrameBox()
		{
			this.keyCodes = new List<KeyCode> ();
		}


		public List<KeyCode> KeyCodes
		{
			get
			{
				return this.keyCodes;
			}
		}


		public delegate void KeyCodeCaught(KeyCode keyCode, bool isShiftPressed);

		public KeyCodeCaught CatcherAction
		{
			get
			{
				return this.catcherAction;
			}
			set
			{
				this.catcherAction = value;
			}
		}


		protected override bool PreProcessMessage(Message message, Point pos)
		{
			if (message.IsKeyType)
			{
				if (this.keyCodes.Contains (message.KeyCode))
				{
					if (message.MessageType == MessageType.KeyDown && this.catcherAction != null)
					{
						this.catcherAction (message.KeyCode, message.IsShiftPressed);
					}

					return false;
				}
			}

			return base.PreProcessMessage (message, pos);
		}


		private readonly List<KeyCode>	keyCodes;

		private KeyCodeCaught			catcherAction;
	}
}
