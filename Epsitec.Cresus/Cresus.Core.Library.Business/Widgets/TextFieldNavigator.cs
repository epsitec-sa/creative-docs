//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Widget TextField qui intercepte les touche Tab/Return, pour envoyer des événements
	/// permettant de naviguer comme dans un tableur.
	/// </summary>
	public class TextFieldNavigator : TextField
	{
		public TextFieldNavigator()
		{
		}

		public TextFieldNavigator(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.IsKeyType)
			{
				if (message.KeyCode == KeyCode.Tab)
				{
					this.OnNavigate (message.IsShiftPressed ? -1 : 1, 0);
					message.Swallowed = true;
					return;
				}

				if (message.KeyCode == KeyCode.Return)
				{
					this.OnNavigate (0, message.IsShiftPressed ? -1 : 1);
					message.Swallowed = true;
					return;
				}
			}

			base.ProcessMessage (message, pos);
		}


		private void OnNavigate(int hDir, int vDir)
		{
			if (Navigate != null)
			{
				Navigate (this, hDir, vDir);
			}
		}

		public delegate void NavigatorHandler(TextFieldNavigator sender, int hDir, int vDir);
		public event NavigatorHandler Navigate;
	}
}
