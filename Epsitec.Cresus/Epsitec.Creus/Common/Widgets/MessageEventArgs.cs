//	Copyright © 2003-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>MessageEventArgs</c> describes a user generated UI event
	/// (for instance mouse moved, key pressed, etc.)
	/// </summary>
	public sealed class MessageEventArgs : Support.CancelEventArgs
	{
		public MessageEventArgs(Message message, Point point)
		{
			this.message = message;
			this.point   = point;
		}


		public Message							Message
		{
			get
			{
				return this.message;
			}
		}

		public Point							Point
		{
			get
			{
				return this.point;
			}
		}


		private readonly Message				message;
		private readonly Point					point;
	}
}
