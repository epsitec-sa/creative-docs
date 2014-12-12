//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	/// <summary>
	/// Contrôleur permettant de choisir l'ensemble des groupes-ratio de
	/// l'objet en édition.
	/// On peut en créer de nouveaux, en supprimer et en modifier.
	/// </summary>
	public class GuidRatioFieldsController : AbstractFieldController
	{
		public GuidRatioFieldsController(DataAccessor accessor)
			: base (accessor)
		{
			this.accessor = accessor;

			this.controllers = new List<GuidRatioFieldController> ();
			this.lines = new Dictionary<ObjectField, Line> ();
		}


		public override IEnumerable<FieldColorType> FieldColorTypes
		{
			get
			{
				foreach (var controller in this.controllers)
				{
					foreach (var type in controller.FieldColorTypes)
					{
						yield return type;
					}
				}
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateTitle (parent);

			var controllersFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.CreateControllers (controllersFrame);
			this.UpdatePropertyState ();
		}


		public void Update()
		{
			//	Met à jour les contrôleurs, en fonction de l'objet en édition.
			this.UpdateControllers ();
		}


		private void CreateTitle(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractFieldController.lineHeight,
			};

			new StaticText
			{
				Parent         = frame,
				Text           = "Groupe",
				Dock           = DockStyle.Left,
				PreferredWidth = AbstractFieldController.maxWidth - 45,
				Margins        = new Margins (100+10, 0, 0, 0),
			};

			new StaticText
			{
				Parent         = frame,
				Text           = "Ratio",
				Dock           = DockStyle.Left,
				PreferredWidth = 45,
			};
		}

		private void CreateControllers(Widget parent)
		{
			//	On crée un contrôleur par ObjectField.GroupGuidRatio, toujours.
			this.controllers.Clear ();
			this.lines.Clear ();

			foreach (var field in DataAccessor.GroupGuidRatioFields)
			{
				this.CreateController (parent, field);
			}
		}

		private void CreateController(Widget parent, ObjectField field)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				Anchor          = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 10, 0, 0),
			};

			var label = new StaticText
			{
				Parent           = frame,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = 100,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var controllerFrame = new FrameBox
			{
				Parent          = frame,
				Dock            = DockStyle.Fill,
				PreferredHeight = AbstractFieldController.lineHeight,
			};

			var controller = new GuidRatioFieldController (this.accessor)
			{
				Accessor      = this.accessor,
				LabelWidth    = 0,
				EditWidth     = AbstractFieldController.maxWidth + 15,
			};

			controller.CreateUI (controllerFrame);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.selectedField = of;
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldGuidRatio (of);
				controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (of);

				this.OnValueEdited (of);
				this.UpdateControllers ();
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.OnShowHistory (target, of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.OnSetFieldFocus (field);
			};

			this.controllers.Add (controller);

			var line = new Line (frame, label, controller);
			this.lines.Add (field, line);
		}


		private void UpdateControllers()
		{
			//	Montre les contrôleurs utilisés par l'objet en édition, dans l'ordre
			//	alphabétique des noms, et cache les autres. Ainsi, il est possible qu'un
			//	contrôleur en édition soit déplacé, sans que cela n'interfère en rien
			//	sur l'édition en cours.
			//	On met à jour le TabIndex de la ligne parente, afin d'avoir un ordre de
			//	parcourt de haut en bas logique avec la touche Tab.

			foreach (var controller in this.controllers)
			{
				controller.IsReadOnly = this.isReadOnly;
			}

			//	Cache toutes les lignes.
			foreach (var field in DataAccessor.GroupGuidRatioFields)
			{
				var line = this.lines[field];
				line.Frame.Visibility = false;
			}

			//	Montre les bonnes lignes existantes.
			int y = 0;
			int tabIndex = 0;

			foreach (var field in GroupsGuidRatioLogic.GetSortedFields (this.accessor))
			{
				var label = (y == 0) ? Res.Strings.FieldControllers.GuidRatio.List.ToString () : "";  // légende uniquement pour le premier
				this.UpdateController (field, y, ref tabIndex, label);
				y += AbstractFieldController.lineHeight + 4;  // en dessous
			}

			//	Montre une dernière ligne "nouveau".
			var ff = this.FreeField;
			if (ff != ObjectField.Unknown)  // limite pas encore attenite ?
			{
				this.UpdateController (ff, y, ref tabIndex, Res.Strings.FieldControllers.GuidRatio.New.ToString ());
			}
		}

		private void UpdateController(ObjectField field, int y, ref int tabIndex, string label)
		{
			//	Met à jour un contrôleur. On le rend visible et on met à jour les
			//	données qu'il représente.
			var line = this.lines[field];

			line.Frame.Visibility = true;
			line.Frame.Margins    = new Margins (0, 0, y, 0);
			line.Frame.BackColor  = (field == this.selectedField) ? ColorManager.WindowBackgroundColor : Color.Empty;
			line.Frame.TabIndex   = ++tabIndex;

			line.Label.Text = label;

			line.Controller.Field         = field;
			line.Controller.Value         = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
			line.Controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);
		}



		private ObjectField FreeField
		{
			//	Retourne le premier champ libre, qui sera créé s'il est rempli par l'utilisateur.
			get
			{
				foreach (var field in DataAccessor.GroupGuidRatioFields)
				{
					var gr = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
					if (gr.IsEmpty)
					{
						return field;
					}
				}

				return ObjectField.Unknown;
			}
		}


		private struct Line
		{
			public Line(FrameBox frame, StaticText label, GuidRatioFieldController controller)
			{
				this.Frame      = frame;
				this.Label      = label;
				this.Controller = controller;
			}

			public readonly FrameBox					Frame;
			public readonly StaticText					Label;
			public readonly GuidRatioFieldController	Controller;
		}


		private readonly List<GuidRatioFieldController>	controllers;
		private readonly Dictionary<ObjectField, Line>	lines;

		private ObjectField								selectedField;
	}
}
