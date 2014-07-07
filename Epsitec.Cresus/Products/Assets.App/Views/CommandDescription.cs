﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Décrit complètement une commande d'une toolbar.
	/// </summary>
	public struct CommandDescription
	{
		public CommandDescription(string icon, string tooltip)
		{
			this.Icon    = icon;
			this.Tooltip = tooltip;
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.Icon)
					&& string.IsNullOrEmpty (this.Tooltip);
			}
		}

		public static CommandDescription Empty = new CommandDescription (null, null);

		public readonly string					Icon;
		public readonly string					Tooltip;
	}
}