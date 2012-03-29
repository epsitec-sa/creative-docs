//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	/// <summary>
	/// The <c>MouseDownProcessorPolicy</c> enumeration defines how the <see cref="MouseDownProcessor"/>
	/// behaves.
	/// </summary>
	public class MouseDownProcessorPolicy : EventProcessorPolicy
	{
		public MouseDownProcessorPolicy()
		{
			this.AutoFollow = true;
			this.AutoScroll = true;

			this.AutoScrollDelay  = SystemInformation.InitialKeyboardDelay;
			this.AutoScrollRepeat = SystemInformation.KeyboardRepeatPeriod;
		}


		/// <summary>
		/// Gets or sets a value indicating whether the selection should automatically follow
		/// the mouse while the user is dragging.
		/// </summary>
		/// <value>
		///   <c>true</c> to follow the mouse while dragging; otherwise, <c>false</c>.
		/// </value>
		public bool								AutoFollow
		{
			get;
			set;
		}

		public bool								AutoScroll
		{
			get;
			set;
		}

		public double							AutoScrollDelay
		{
			get;
			set;
		}

		public double							AutoScrollRepeat
		{
			get;
			set;
		}

		public bool								SelectOnRelease
		{
			get;
			set;
		}
	}
}
