﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Contrôleur permettant de choisir l'ensemble des arguments de
	/// l'objet en édition (qui est une méthode d'amortissement).
	/// On peut en créer de nouveaux, en supprimer et en modifier.
	/// </summary>
	public class ArgumentToUseFieldsController : AbstractFieldController
	{
		public ArgumentToUseFieldsController(DataAccessor accessor)
			: base (accessor)
		{
			this.accessor = accessor;

			this.controllers = new List<ArgumentToUseFieldController> ();
		}


		public IEnumerable<Guid> ArgumentGuids
		{
			get
			{
				foreach (var field in ArgumentToUseFieldsController.GetSortedFields (this.accessor))
				{
					var guid = this.accessor.EditionAccessor.GetFieldGuid (field);

					if (!guid.IsEmpty)
					{
						yield return guid;
					}
				}
			}
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
			this.controllersFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Top,
			};
		}


		public void Update()
		{
			//	Met à jour les contrôleurs, en fonction de l'objet en édition.
			this.CreateControllers ();
			this.UpdatePropertyState ();
		}


		private void CreateControllers()
		{
			this.controllers.Clear ();
			this.controllersFrame.Children.Clear ();

			int rank = 0;
			foreach (var field in ArgumentToUseFieldsController.GetSortedFields (this.accessor))
			{
				this.CreateController (this.controllersFrame, rank++, field);
			}

			this.CreateController (this.controllersFrame, rank++, ObjectField.Unknown);
		}

		private void CreateController(Widget parent, int rank, ObjectField field)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 0, 0, 0),
			};

			string s = null;

			if (field == ObjectField.Unknown)
			{
				s = "Nouvel argument";
			}
			else if (rank == 0)
			{
				s = "Arguments";
			}

			new StaticText
			{
				Parent           = frame,
				Text             = s,
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

			var controller = new ArgumentToUseFieldController (this.accessor)
			{
				Accessor      = this.accessor,
				LabelWidth    = 0,
				EditWidth     = AbstractFieldController.maxWidth,
				Field         = field,
			};

			if (field != ObjectField.Unknown)
			{
				//	Bouton "poubelle" pour toutes les lignes, sauf la dernière "nouvel argument".
				controller.PropertyState = PropertyState.Deletable;
			}

			if (field != ObjectField.Unknown)
			{
				controller.Value = this.accessor.EditionAccessor.GetFieldGuid (field);
			}

			controller.CreateUI (controllerFrame);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.selectedField = of;
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldGuid (of);
				controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (of);

				this.OnValueEdited (of);
				this.Update ();
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
		}


		//??private void UpdateControllers()
		//??{
		//??	//	Montre les contrôleurs utilisés par l'objet en édition, dans l'ordre
		//??	//	alphabétique des noms, et cache les autres. Ainsi, il est possible qu'un
		//??	//	contrôleur en édition soit déplacé, sans que cela n'interfère en rien
		//??	//	sur l'édition en cours.
		//??	//	On met à jour le TabIndex de la ligne parente, afin d'avoir un ordre de
		//??	//	parcourt de haut en bas logique avec la touche Tab.
		//??
		//??	foreach (var controller in this.controllers)
		//??	{
		//??		controller.IsReadOnly = this.isReadOnly;
		//??	}
		//??
		//??	//	Cache toutes les lignes.
		//??	foreach (var field in DataAccessor.ArgumentFields)
		//??	{
		//??		var line = this.lines[field];
		//??		line.Frame.Visibility = false;
		//??	}
		//??
		//??	//	Montre les bonnes lignes existantes.
		//??	int y = 0;
		//??	int tabIndex = 0;
		//??
		//??	foreach (var field in ArgumentToUseFieldsController.GetSortedFields (this.accessor))
		//??	{
		//??		var label = (y == 0) ? "Arguments" : "";  // légende uniquement pour le premier
		//??		this.UpdateController (field, y, ref tabIndex, label);
		//??		y += AbstractFieldController.lineHeight + 4;  // en dessous
		//??	}
		//??
		//??	//	Montre une dernière ligne "nouveau".
		//??	var ff = this.FreeField;
		//??	if (ff != ObjectField.Unknown)  // limite pas encore attenite ?
		//??	{
		//??		this.UpdateController (ff, y, ref tabIndex, "Nouvel argument");
		//??	}
		//??}
		//??
		//??private void UpdateController(ObjectField field, int y, ref int tabIndex, string label)
		//??{
		//??	//	Met à jour un contrôleur. On le rend visible et on met à jour les
		//??	//	données qu'il représente.
		//??	var line = this.lines[field];
		//??
		//??	line.Frame.Visibility = true;
		//??	line.Frame.Margins    = new Margins (0, 0, y, 0);
		//??	line.Frame.BackColor  = (field == this.selectedField) ? ColorManager.WindowBackgroundColor : Color.Empty;
		//??	line.Frame.TabIndex   = ++tabIndex;
		//??
		//??	line.Label.Text = label;
		//??
		//??	line.Controller.Field         = field;
		//??	line.Controller.Value         = this.accessor.EditionAccessor.GetFieldGuid (field);
		//??	line.Controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);
		//??}



		private ObjectField FreeField
		{
			//	Retourne le premier champ libre, qui sera créé s'il est rempli par l'utilisateur.
			get
			{
				foreach (var field in DataAccessor.ArgumentFields)
				{
					var gr = this.accessor.EditionAccessor.GetFieldGuid (field);
					if (gr.IsEmpty)
					{
						return field;
					}
				}

				return ObjectField.Unknown;
			}
		}


		#region Sorted fields
		public static IEnumerable<ObjectField> GetSortedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs, triés par ordre alphabétique des noms
			//	des arguments en édition.
			return ArgumentToUseFieldsController.GetUsedFields (accessor)
				.OrderBy (x => ArgumentToUseFieldsController.GetSortingValue (accessor, x));
		}

		private static string GetSortingValue(DataAccessor accessor, ObjectField field)
		{
			//	Retourne le nom d'un argument, en vue du tri.
			var guid = accessor.EditionAccessor.GetFieldGuid (field);
			return ArgumentsLogic.GetSummary (accessor, guid);
		}

		private static IEnumerable<ObjectField> GetUsedFields(DataAccessor accessor)
		{
			//	Retourne la liste des arguments utilisés par l'objet en édition, non triée.
			foreach (var field in DataAccessor.ArgumentFields)
			{
				var guid = accessor.EditionAccessor.GetFieldGuid (field);
				if (!guid.IsEmpty)
				{
					yield return field;
				}
			}
		}
		#endregion


		private readonly List<ArgumentToUseFieldController>	controllers;

		private FrameBox								controllersFrame;
		private ObjectField								selectedField;
	}
}
