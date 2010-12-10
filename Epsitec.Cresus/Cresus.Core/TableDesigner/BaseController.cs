//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Widgets;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.TableDesigner
{
	public class BaseController
	{
		public BaseController(PriceCalculatorEntity priceCalculatorEntity)
		{
			this.priceCalculatorEntity = priceCalculatorEntity;
		}

		public void CreateUI(Widget parent)
		{
			int tabIndex = 1;

			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			var leftPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 300,
				DrawFullFrame = true,
				Dock = DockStyle.Left,
				Padding = new Margins (10),
			};

			{
				new StaticText
				{
					Parent = leftPane,
					Text = "Code :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.codeField = new TextFieldEx
				{
					Parent = leftPane,
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = tabIndex++,
				};
			}

			{
				new StaticText
				{
					Parent = leftPane,
					Text = "Nom :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.nameField = new TextFieldEx
				{
					Parent = leftPane,
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = tabIndex++,
				};
			}

			{
				new StaticText
				{
					Parent = leftPane,
					Text = "Description :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.descriptionField = new TextFieldMultiEx
				{
					Parent = leftPane,
					PreferredHeight = 100,
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					ScrollerVisibility = false,
					PreferredLayout = TextFieldMultiExPreferredLayout.PreserveScrollerHeight,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = tabIndex++,
				};

				this.Update ();
			}

			//	Connexion des événements.
			this.codeField.EditionAccepted += delegate
			{
				this.priceCalculatorEntity.Code = this.codeField.Text;
			};

			this.nameField.EditionAccepted += delegate
			{
				this.priceCalculatorEntity.Name = this.nameField.FormattedText;
			};

			this.descriptionField.EditionAccepted += delegate
			{
				this.priceCalculatorEntity.Description = this.descriptionField.FormattedText;
			};
		}


		public void Update()
		{
			this.codeField.Text = this.priceCalculatorEntity.Code;
			this.nameField.FormattedText = this.priceCalculatorEntity.Name;
			this.descriptionField.FormattedText = this.priceCalculatorEntity.Description;
		}


		private readonly PriceCalculatorEntity			priceCalculatorEntity;

		private TextFieldEx								codeField;
		private TextFieldEx								nameField;
		private TextFieldMultiEx						descriptionField;
	}
}
