//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Accessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère l'en-tête des colonnes qui vient au-dessus du widget StringArray.
	/// </summary>
	public class HeaderController
	{
		public HeaderController()
		{
			this.headerFrames = new List<FrameBox> ();
		}


		public FrameBox CreateUI(FrameBox parent)
		{
			this.headerFrame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Top,
				Margins = new Margins (0, 0, 0, -1),
			};

			return this.headerFrame;
		}

		public void UpdateColumns(IEnumerable<FormattedText> descriptions)
		{
			this.headerFrames.Clear ();
			this.headerFrame.Children.Clear ();

			foreach (var description in descriptions)
			{
				var frame = new FrameBox
				{
					Parent        = this.headerFrame,
					DrawFullFrame = true,
					Dock          = DockStyle.Left,
					Margins       = new Margins (0, -1, 0, 0),
				};

				new StaticText
				{
					Parent           = frame,
					FormattedText    = description,
					ContentAlignment = ContentAlignment.MiddleCenter,
					Dock             = DockStyle.Fill,
				};

				this.headerFrames.Add (frame);
			}
		}

		public void UpdateGeometry(StringArray array)
		{
			//	Met à jour la géométrie de l'en-tête en fonction des largeurs des colonnes.
			for (int c = 0; c < this.headerFrames.Count; c++)
			{
				this.headerFrames[c].PreferredWidth = array.GetColumnsAbsoluteWidth (c) + 1;
			}
		}

		public void HiliteColumn(int column)
		{
			//	Met en évidence une colonne à choix, en bleu clair.
			for (int c = 0; c < this.headerFrames.Count; c++)
			{
				var button = this.headerFrames[c];

				button.BackColor = (c == column) ? Color.FromHexa ("b3d7ff") : Color.Empty;
			}
		}


		private readonly List<FrameBox>			headerFrames;

		private FrameBox						headerFrame;
	}
}
