//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Common.Designer.Controllers
{
	public class FilterController
	{
		public FilterController()
		{
		}

		public FrameBox CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
			};

			this.label = new StaticText
			{
				Parent = frame,
				Text = "Rechercher",
				PreferredWidth = 64,
				Dock = DockStyle.Left,
			};

			this.field = new TextField
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				TabIndex = 1,
			};

			this.clearButton = new GlyphButton
			{
				Parent = frame,
				GlyphShape = GlyphShape.Close,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			//	Connexion des événements.
			this.field.TextChanged += delegate
			{
				this.OnFilterChanged ();
			};

			this.clearButton.Clicked += delegate
			{
				this.field.Text = null;
				this.field.SelectAll ();
				this.field.Focus ();
			};

			return frame;
		}


		public void ClearFilter()
		{
			this.field.Text = null;
		}

		public bool HasFilter
		{
			get
			{
				return !string.IsNullOrEmpty (this.field.Text);
			}
		}

		public string Filter
		{
			get
			{
				return this.field.Text;
			}
			set
			{
				this.field.Text = value;
			}
		}

		public void SetFocus()
		{
			this.field.SelectAll ();
			this.field.Focus ();
		}


		private void OnFilterChanged()
		{
			var handler = this.FilterChanged;

			if (handler != null)
			{
				handler (this);
			}
		}


		public event Support.EventHandler	FilterChanged;

		private StaticText					label;
		private TextField					field;
		private GlyphButton					clearButton;
	}
}
