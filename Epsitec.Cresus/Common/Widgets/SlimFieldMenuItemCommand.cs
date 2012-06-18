//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets.Behaviors;

namespace Epsitec.Common.Widgets
{
	public class SlimFieldMenuItemCommand : SlimFieldMenuItem
	{
		public SlimFieldMenuItemCommand(SlimFieldMenuItemCommandCode code, System.Action<SlimFieldMenuBehavior> command = null)
			: base (SlimFieldMenuItemCommand.GetSymbolText (code), style: SlimFieldMenuItemCommand.GetSymbolStyle (code))
		{
			if (command == null)
			{
				command = SlimFieldMenuItemCommand.GetDefaultCommand (code);
			}

			this.code = code;
			this.command = command;
		}

		
		public SlimFieldMenuItemCommandCode		Code
		{
			get
			{
				return this.code;
			}
		}


		public override bool ExecuteCommand(SlimFieldMenuBehavior source)
		{
			if (this.command == null)
			{
				return false;
			}
			else
			{
				this.command (source);
				return true;
			}
		}
		
		private static string GetSymbolText(SlimFieldMenuItemCommandCode code)
		{
			switch (code)
			{
				case SlimFieldMenuItemCommandCode.Clear:
					return "✘";

				case SlimFieldMenuItemCommandCode.Extra:
					return "plus…";

				default:
					return "";
			}
		}

		private static SlimFieldMenuItemStyle GetSymbolStyle(SlimFieldMenuItemCommandCode code)
		{
			switch (code)
			{
				case SlimFieldMenuItemCommandCode.Clear:
					return SlimFieldMenuItemStyle.Symbol;

				case SlimFieldMenuItemCommandCode.Extra:
					return SlimFieldMenuItemStyle.Extra;
				
				default:
					return SlimFieldMenuItemStyle.Extra;
			}
		}

		private static System.Action<SlimFieldMenuBehavior> GetDefaultCommand(SlimFieldMenuItemCommandCode code)
		{
			switch (code)
			{
				case SlimFieldMenuItemCommandCode.Clear:
					return item => item.Clear ();

				default:
					return null;
			}
		}
		
		private readonly SlimFieldMenuItemCommandCode code;
		private readonly System.Action<SlimFieldMenuBehavior> command;
	}
}
