//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks
{
	internal class UIAction
	{
		public UIAction(System.Action<FrameBox, UIBuilder> action)
		{
			this.action      = action;
			this.fieldInfo   = null;
		}

		
		public FieldInfo						FieldInfo
		{
			get
			{
				return this.fieldInfo;
			}
			set
			{
				this.fieldInfo = value;
			}
		}

		
		public void Execute(FrameBox frame, UIBuilder builder)
		{
			if ((this.fieldInfo == null) ||
						(this.fieldInfo.Settings.FieldVisibilityMode == TileVisibilityMode.Visible))
			{
				this.InternalExecute (frame, builder);
			}
		}

		protected virtual void InternalExecute(FrameBox frame, UIBuilder builder)
		{
			this.action (frame, builder);
		}

		private readonly System.Action<FrameBox, UIBuilder> action;
		private FieldInfo						fieldInfo;
	}
}
