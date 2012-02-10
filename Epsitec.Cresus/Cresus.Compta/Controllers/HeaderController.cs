//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

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
		public HeaderController(AbstractController controller)
		{
			this.controller = controller;
			this.columnMappers = this.controller.ColumnMappers;

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

		public void UpdateColumns()
		{
			this.headerFrames.Clear ();
			this.headerFrame.Children.Clear ();

			this.columnMappersShowed = this.columnMappers.Where (x => x.Show).ToList ();

			foreach (var description in this.columnMappersShowed.Select (x => x.Description))
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
					TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					Dock             = DockStyle.Fill,
					Margins          = new Margins (1, 1, 0, 0),
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

		public void HiliteColumn(ColumnType columnType)
		{
			//	Met en évidence une colonne à choix, en bleu clair.
			int column = 0;

			foreach (var mapper in this.columnMappersShowed)
			{
				var button = this.headerFrames[column++];
				button.BackColor = (mapper.Column == columnType) ? Color.FromHexa ("b3d7ff") : Color.Empty;
			}
		}


		private readonly AbstractController		controller;
		private readonly List<ColumnMapper>		columnMappers;
		private readonly List<FrameBox>			headerFrames;

		private List<ColumnMapper>				columnMappersShowed;
		private FrameBox						headerFrame;
	}
}
