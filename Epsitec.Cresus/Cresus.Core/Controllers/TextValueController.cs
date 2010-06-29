//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;
using Epsitec.Cresus.DataLayer;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers
{
	public class TextValueController : IWidgetUpdater
	{
		public TextValueController(Marshaler marshaler)
		{
			this.marshaler = marshaler;
		}


		public void Attach(AbstractTextField widget)
		{
			this.widget = widget;
			this.Update ();

			new MarshalerValidator (this.widget, this.marshaler);

			widget.AcceptingEdition +=
				delegate
				{
					string text = TextConverter.ConvertToSimpleText (widget.Text);
					this.marshaler.SetStringValue (text);
				};

			widget.KeyboardFocusChanged += (sender, e) => this.Update ();
		}

		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.widget != null)
			{
				this.widget.Text = TextConverter.ConvertToTaggedText (this.marshaler.GetStringValue ());
			}
		}

		#endregion

		private readonly Marshaler marshaler;
		private Widget widget;
	}
}
