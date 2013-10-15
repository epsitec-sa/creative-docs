//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractFieldController
	{
		public int TabIndex;
		public int EditWidth = 300;

		public string Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}


		public virtual void CreateUI(Widget parent)
		{
			this.frameBox = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 22,
				Margins         = new Margins (0, 0, 0, 2),
			};

			this.CreateLabel ();
		}

		protected void CreateLabel()
		{
			new StaticText
			{
				Parent           = this.frameBox,
				Text             = this.label,
				ContentAlignment = ContentAlignment.TopRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = AbstractFieldController.labelWidth,
				Margins          = new Margins (0, 10, 3, 0),
			};
		}


		#region Events handler
		protected void OnValueChanged()
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged (this);
			}
		}

		public delegate void ValueChangedEventHandler(object sender);
		public event ValueChangedEventHandler ValueChanged;
		#endregion



		protected static readonly int labelWidth = 100;

		protected FrameBox						frameBox;
		private string							label;
	}
}
