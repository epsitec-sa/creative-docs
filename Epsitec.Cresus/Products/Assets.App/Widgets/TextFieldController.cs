//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TextFieldController
	{
		public Color								EditColor
		{
			get
			{
				return this.editColor;
			}
			set
			{
				if (this.editColor != value)
				{
					this.editColor = value;
					this.UpdateUI ();
				}
			}
		}

		public bool								Hatch
		{
			get
			{
				return this.hatch;
			}
			set
			{
				if (this.hatch != value)
				{
					this.hatch = value;
					this.UpdateUI ();
				}
			}
		}

		public HatchFrameBox					FrameBox
		{
			get
			{
				return this.frameBox;
			}
		}

		public AbstractTextField				TextField
		{
			get
			{
				return this.textField;
			}
		}


		public void CreateUI(Widget parent, DockStyle dock, Size size, bool isMulti)
		{
			this.frameBox = new HatchFrameBox
			{
				Parent        = parent,
				Dock          = dock,
				PreferredSize = size,
			};

			if (isMulti)
			{
				this.textField = new TextFieldMulti
				{
					Parent = this.frameBox,
					Dock   = DockStyle.Fill,
				};
			}
			else
			{
				this.textField = new TextField
				{
					Parent = this.frameBox,
					Dock   = DockStyle.Fill,
				};
			}

			this.UpdateUI ();
		}

		private void UpdateUI()
		{
			if (this.frameBox != null)
			{
				if (this.hatch)
				{
					this.frameBox.Hatch = true;

					this.textField.BackColor = Color.Empty;
					this.textField.TextDisplayMode = TextFieldDisplayMode.UseBackColor;
				}
				else
				{
					this.frameBox.Hatch = false;

					if (!this.editColor.IsVisible)
					{
						this.textField.BackColor = Color.Empty;
						this.textField.TextDisplayMode = TextFieldDisplayMode.Default;
					}
					else
					{
						this.textField.BackColor = this.editColor;
						this.textField.TextDisplayMode = TextFieldDisplayMode.UseBackColor;
					}
				}
			}
		}


		private HatchFrameBox		frameBox;
		private AbstractTextField	textField;
		private Color				editColor;
		private bool				hatch;
	}
}
