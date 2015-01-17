//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup affichant le code C# complet d'une expression d'amortissement.
	/// </summary>
	public class ShowExpressionPopup : AbstractPopup
	{
		private ShowExpressionPopup(DataAccessor accessor, string expression)
		{
			this.accessor = accessor;
			this.expression = expression;
		}


		protected override Size DialogSize
		{
			get
			{
				return new Size (600, 550);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.ShowExpression.Title.ToString ());

			var line = new TextFieldMulti
			{
				Parent        = this.mainFrameBox,
				MaxLength     = this.expression.Length,
				Text          = this.expression,
				IsReadOnly    = true,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (this.DialogSize.Width, this.DialogSize.Height-AbstractPopup.titleHeight),
				Margins       = new Margins (0, 0, 0, 0),
			};

			line.TextLayout.DefaultFont = Font.GetFont ("Courier New", "Regular");  // bof

			this.CreateCloseButton ();
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, string expression)
		{
			if (target != null)
			{
				var popup = new ShowExpressionPopup (accessor, expression);
				popup.Create (target, leftOrRight: true);
			}
		}
		#endregion


		private readonly DataAccessor			accessor;
		private readonly string					expression;
	}
}