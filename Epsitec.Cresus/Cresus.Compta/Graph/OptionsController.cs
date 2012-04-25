//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class OptionsController
	{
		public OptionsController(GraphOptions options)
		{
			this.options = options;
		}


		public void CreateUI(Widget parent, System.Action optionsChangedAction)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			var b1 = new Button
			{
				Parent         = frame,
				Text           = "Côte à côte",
				PreferredWidth = 80,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 1, 0, 0),
			};

			var b2 = new Button
			{
				Parent         = frame,
				Text           = "Cumulé",
				PreferredWidth = 80,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 1, 0, 0),
			};

			var b3 = new Button
			{
				Parent         = frame,
				Text           = "Lignes",
				PreferredWidth = 80,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 1, 0, 0),
			};

			var b4 = new Button
			{
				Parent         = frame,
				Text           = "Camembert",
				PreferredWidth = 80,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 1, 0, 0),
			};

			var b5 = new Button
			{
				Parent         = frame,
				Text           = "Tableau",
				PreferredWidth = 80,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 1, 0, 0),
			};

			var c = new CheckButton
			{
				Parent         = frame,
				Text           = "Axes",
				PreferredWidth = 60,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 0, 0, 0),
			};

			var style = new TextFieldCombo
			{
				Parent         = frame,
				IsReadOnly     = true,
				PreferredWidth = 150,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 0, 0, 0),
			};

			style.Items.Add ("Arc-en-ciel");
			style.Items.Add ("Arc-en-ciel pastel");
			style.Items.Add ("Arc-en-ciel foncé");
			style.Items.Add ("Gris");
			style.Items.Add ("Noir et blanc");
			style.Items.Add ("Rouge");
			style.Items.Add ("Vert");
			style.Items.Add ("Bleu");
			style.Items.Add ("Feu");

			b1.Clicked += delegate
			{
				this.options.Mode = GraphMode.SideBySide;
				optionsChangedAction ();
			};

			b2.Clicked += delegate
			{
				this.options.Mode = GraphMode.Stacked;
				optionsChangedAction ();
			};

			b3.Clicked += delegate
			{
				this.options.Mode = GraphMode.Lines;
				optionsChangedAction ();
			};

			b4.Clicked += delegate
			{
				this.options.Mode = GraphMode.Pie;
				optionsChangedAction ();
			};

			b5.Clicked += delegate
			{
				this.options.Mode = GraphMode.Array;
				optionsChangedAction ();
			};

			c.ActiveStateChanged += delegate
			{
				if (c.ActiveState == ActiveState.Yes)
				{
					this.options.PrimaryDimension = 0;
					this.options.SecondaryDimension = 1;
				}
				else
				{
					this.options.PrimaryDimension = 1;
					this.options.SecondaryDimension = 0;
				}

				optionsChangedAction ();
			};

			style.SelectedItemChanged += delegate
			{
				options.Style = (GraphStyle) style.SelectedItemIndex;
				optionsChangedAction ();
			};
		}


		private readonly GraphOptions			options;
	}
}
