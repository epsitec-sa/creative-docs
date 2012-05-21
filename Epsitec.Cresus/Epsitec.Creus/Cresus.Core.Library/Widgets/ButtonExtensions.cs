//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Classe d'extension pour Epsitec.Common.Widgets.Button & fils.
	/// </summary>
	public static class ButtonExtensions
	{
		public static void SelectConfirmationButton(this ConfirmationButton button, bool selected)
		{
			//	Modifie l'aspect d'un bouton ConfirmationButton pour montrer qu'il est sélectionné ou pas.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			if (selected)
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
				button.SetSelected (true);
				button.ActiveState = ActiveState.Yes;
				button.BackColor = adorner.ColorCaption;
			}
			else
			{
				button.ButtonStyle = ButtonStyle.Confirmation;
				button.SetSelected (false);
				button.ActiveState = ActiveState.No;
				button.BackColor = Color.Empty;
			}
		}
	}
}
