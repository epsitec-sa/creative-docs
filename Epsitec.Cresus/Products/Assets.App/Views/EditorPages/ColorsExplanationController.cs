//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	/// <summary>
	/// Affiche en pied de page une série de "commentaires" composés d'un échantillon de
	/// couleur et d'un texte explicatif.
	/// </summary>
	public class ColorsExplanationController
	{
		public ColorsExplanationController()
		{
			this.typesToShow  = new HashSet<FieldColorType> ();
		}


		public void ClearTypesToShow()
		{
			this.typesToShow.Clear ();
		}

		public void AddTypesToShow(IEnumerable<FieldColorType> types)
		{
			foreach (var type in types)
			{
				this.typesToShow.Add (type);
			}
		}

		public void AddTypeToShow(FieldColorType type)
		{
			this.typesToShow.Add (type);
		}


		public void CreateUI(Widget parent)
		{
			//	Crée le pied de page pour les commentaires.
			this.footer = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
			};

			//	Partie principale à gauche pour les lignes.
			this.frame = new FrameBox
			{
				Parent          = this.footer,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (10, 10, 10, 0),
			};

			//	Partie grise à droite, pour prolonger l'ascenseur vertical.
			new FrameBox
			{
				Parent          = this.footer,
				Dock            = DockStyle.Right,
				PreferredWidth  = AbstractView.scrollerDefaultBreadth,
				BackColor       = ColorManager.WindowBackgroundColor,
			};
		}

		public void Update()
		{
			//	Met à jour les couleurs des échantillons.
			if (this.footer == null)
			{
				return;
			}

			bool show = this.typesToShow.Any ();
			this.footer.Visibility = show;

			if (!show)
			{
				return;
			}

			//	Crée les différentes lignes, de bas en haut.
			this.frame.Children.Clear ();

			var widths = new int[ColorsExplanationController.columnsCount];
			for (int column=0; column<ColorsExplanationController.columnsCount; column++)
			{
				widths[column] = GetColumnWidth (column);
			}

			int rank = 0;
			FrameBox line = null;

			foreach (var item in this.ItemsToShow)
			{
				if (rank%ColorsExplanationController.columnsCount == 0)  // crée une nouvelle ligne ?
				{
					line = new FrameBox
					{
						Parent          = this.frame,
						Dock            = DockStyle.Bottom,
						PreferredHeight = AbstractFieldController.lineHeight,
						Margins         = new Margins (0, 0, 0, 10),
					};
				}

				int width = widths[rank%ColorsExplanationController.columnsCount];
				this.CreateColorExplanation (line, item, width);

				rank++;
			}
		}

		private void CreateColorExplanation(Widget parent, Item item, int width)
		{
			//	Crée un commentaire composé d'un carré coloré suivi d'un texte.
			const int h = AbstractFieldController.lineHeight;

			var colorSample = new FrameBox
			{
				Parent        = parent,
				BackColor     = item.Color,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (h, h),
			};

			var label = new StaticText
			{
				Parent        = parent,
				Text          = item.Description,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (width+20, h),
				Margins       = new Margins (10, 0, 0, 0),
			};

			if (!string.IsNullOrEmpty (item.Tooltip))  // tooltip existe ?
			{
				ToolTip.Default.SetToolTip (colorSample, item.Tooltip);
				ToolTip.Default.SetToolTip (label,       item.Tooltip);
			}
		}

		private int GetColumnWidth(int column)
		{
			//	Retourne la largeur requise pour une colonne, en fonction du plus
			//	large des textes.
			int width = 0;
			int rank = 0;

			foreach (var item in this.ItemsToShow)
			{
				if (rank%ColorsExplanationController.columnsCount == column)
				{
					width = System.Math.Max (width, item.Description.GetTextWidth ());
				}

				rank++;
			}

			return width;
		}

		private IEnumerable<Item> ItemsToShow
		{
			//	Retourne les items à montrer.
			get
			{
				return this.Items.Where (x => this.typesToShow.Contains (x.Type));
			}
		}

		private IEnumerable<Item> Items
		{
			//	Retourne tous les items.
			get
			{
				yield return new Item
				(
					FieldColorType.Editable,
					"Champ pouvant être défini",
					"Cette couleur indique un champ pouvant être défini par cet événement",
					AbstractFieldController.GetBackgroundColor (PropertyState.Synthetic, isReadOnly: false, isError: false)
				);

				yield return new Item
				(
					FieldColorType.Defined,
					"Champ défini",
					"Cette couleur indique un champ défini par cet événement",
					AbstractFieldController.GetBackgroundColor (PropertyState.Single, isReadOnly: false, isError: false)
				);

				yield return new Item
				(
					FieldColorType.Readonly,
					"Champ ne pouvant pas être défini",
					"Cette couleur indique un champ ne pouvant pas être défini par cet événement",
					AbstractFieldController.GetBackgroundColor (PropertyState.Synthetic, isReadOnly: true, isError: false)
				);

				yield return new Item
				(
					FieldColorType.Result,
					"Résultat d'un calcul",
					"Cette couleur indique le résultat d'un calcul ne pouvant pas être modifié",
					AbstractFieldController.GetBackgroundColor (PropertyState.Single, isReadOnly: true, isError: false)
				);

				yield return new Item
				(
					FieldColorType.Error,
					"Erreur",
					"Cette couleur indique un champ incorrectement rempli",
					AbstractFieldController.GetBackgroundColor (PropertyState.Synthetic, isReadOnly: false, isError: true)
				);
			}
		}

		private struct Item
		{
			public Item(FieldColorType type, string description, string tooltip, Color color)
			{
				this.Type        = type;
				this.Description = description;
				this.Tooltip     = tooltip;
				this.Color       = color;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Type == FieldColorType.Unknown
						&& string.IsNullOrEmpty (this.Description)
						&& string.IsNullOrEmpty (this.Tooltip)
						&& this.Color.IsEmpty;
				}
			}

			public static Item Empty = new Item (FieldColorType.Unknown, null, null, Color.Empty);

			public readonly FieldColorType		Type;
			public readonly string				Description;
			public readonly string				Tooltip;
			public readonly Color				Color;
		}


		private const int columnsCount = 3;

		private readonly HashSet<FieldColorType> typesToShow;

		private FrameBox						footer;
		private FrameBox						frame;
	}
}
