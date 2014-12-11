//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	/// <summary>
	/// Contrôleur permettant de choisir les valeurs des arguments d'une méthode
	/// d'amortissement donnée.
	/// </summary>
	public class ArgumentValueFieldsController : AbstractFieldController
	{
		public ArgumentValueFieldsController(DataAccessor accessor)
			: base (accessor)
		{
			this.accessor = accessor;

			this.controllers = new List<AbstractFieldController> ();
		}


		public void SetMethod(Guid methodGuid)
		{
			this.controllers.Clear ();
			this.controllersFrame.Children.Clear ();

			var methodObj = this.accessor.GetObject (BaseType.Methods, methodGuid);

			foreach (var field in DataAccessor.ArgumentFields)
			{
				var argumentGuid = ObjectProperties.GetObjectPropertyGuid (methodObj, null, field);

				if (!argumentGuid.IsEmpty)
				{
					var argumentObj = this.accessor.GetObject (BaseType.Arguments, argumentGuid);

					this.CreateController (argumentObj, field);
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


		private void CreateController(DataObject argumentObj, ObjectField field)
		{
			var name = ObjectProperties.GetObjectPropertyString (argumentObj, null, ObjectField.Name);
			var type = (ArgumentType) ObjectProperties.GetObjectPropertyInt (argumentObj, null, ObjectField.ArgumentType);

			var frame = new FrameBox
			{
				Parent          = this.controllersFrame,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 10, 0, 0),
			};

			var label = new StaticText
			{
				Parent           = frame,
				Text             = name,
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

			switch (type)
			{
				case ArgumentType.Decimal:
					this.CreateControllerDecimal (controllerFrame, field, DecimalFormat.Real);
					break;

				case ArgumentType.Amount:
					this.CreateControllerDecimal (controllerFrame, field, DecimalFormat.Amount);
					break;

				case ArgumentType.Rate:
					this.CreateControllerDecimal (controllerFrame, field, DecimalFormat.Rate);
					break;

				case ArgumentType.Int:
					this.CreateControllerInt (controllerFrame, field);
					break;

				case ArgumentType.Bool:
					this.CreateControllerBool (controllerFrame, field);
					break;

				case ArgumentType.Date:
					this.CreateControllerDate(controllerFrame, field);
					break;

				case ArgumentType.String:
					this.CreateControllerString (controllerFrame, field);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid ArgumentType {0}", type));
			}
		}

		private void CreateControllerDecimal(Widget parent, ObjectField field, DecimalFormat format)
		{
			var controller = new DecimalFieldController (this.accessor)
			{
				LabelWidth    = 0,
				EditWidth     = 100,
				Field         = field,
				DecimalFormat = format,
			};

			controller.CreateUI (parent);

			this.controllers.Add (controller);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldDecimal (field);
				controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.OnShowHistory (target, field);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.OnSetFieldFocus (field);
			};
		}

		private void CreateControllerInt(Widget parent, ObjectField field)
		{
			var controller = new IntFieldController (this.accessor)
			{
				LabelWidth    = 0,
				EditWidth     = 100,
				Field         = field,
			};

			controller.CreateUI (parent);

			this.controllers.Add (controller);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldInt (field);
				controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.OnShowHistory (target, field);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.OnSetFieldFocus (field);
			};
		}

		private void CreateControllerBool(Widget parent, ObjectField field)
		{
			var controller = new BoolFieldController (this.accessor)
			{
				LabelWidth    = 0,
				EditWidth     = 100,
				Field         = field,
			};

			controller.CreateUI (parent);

			this.controllers.Add (controller);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value ? 1:0);

				controller.Value         = this.accessor.EditionAccessor.GetFieldInt (field) == 1;
				controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.OnShowHistory (target, field);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.OnSetFieldFocus (field);
			};
		}

		private void CreateControllerDate(Widget parent, ObjectField field)
		{
			var controller = new DateFieldController (this.accessor)
			{
				LabelWidth    = 0,
				EditWidth     = 100,
				Field         = field,
			};

			controller.CreateUI (parent);

			this.controllers.Add (controller);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldDate (field);
				controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.OnShowHistory (target, field);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.OnSetFieldFocus (field);
			};
		}

		private void CreateControllerString(Widget parent, ObjectField field)
		{
			var controller = new StringFieldController (this.accessor)
			{
				LabelWidth    = 0,
				EditWidth     = 100,
				Field         = field,
			};

			controller.CreateUI (parent);

			this.controllers.Add (controller);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldString (field);
				controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.OnShowHistory (target, field);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.OnSetFieldFocus (field);
			};
		}



		private readonly List<AbstractFieldController>	controllers;

		private FrameBox								controllersFrame;
	}
}
