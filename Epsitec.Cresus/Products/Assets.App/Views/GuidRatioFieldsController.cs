//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class GuidRatioFieldsController : AbstractFieldController
	{
		public GuidRatioFieldsController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.lines = new Dictionary<ObjectField, Line> ();
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateTitle (parent);

			this.controllersFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.CreateControllers (this.controllersFrame);
		}


		public void Update()
		{
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
				PreferredWidth = 350,
				Margins        = new Margins (85+10+5, 0, 0, 0),
			};

			new StaticText
			{
				Parent         = frame,
				Text           = "Ratio",
				Dock           = DockStyle.Left,
				PreferredWidth = 90,
			};
		}

		private void CreateControllers(Widget parent)
		{
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
			};

			var label = new StaticText
			{
				Parent           = frame,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = 85,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var controllerFrame = new FrameBox
			{
				Parent          = frame,
				Dock            = DockStyle.Fill,
				PreferredHeight = AbstractFieldController.lineHeight,
			};

			var controller = new GuidRatioFieldController
			{
				Accessor      = this.accessor,
				LabelWidth    = 0,
				EditWidth     = 395,
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


			var line = new Line (frame, label, controller);
			this.lines.Add (field, line);
		}


		private void UpdateControllers()
		{
			//	Cache toutes les lignes.
			foreach (var field in DataAccessor.GroupGuidRatioFields)
			{
				var line = this.lines[field];
				line.Frame.Visibility = false;
			}

			//	Montre les bonnes lignes existantes.
			int y = 0;
			foreach (var field in this.SortedFields)
			{
				var label = (y == 0) ? "Regroupements" : "";
				this.UpdateController (field, y, label);
				y += AbstractFieldController.lineHeight + 4;
			}

			//	Montre une dernière ligne "nouveau".
			var ff = this.FreeField;
			if (ff != ObjectField.Unknown)
			{
				this.UpdateController (ff, y, "Nouveau");
			}
		}

		private void UpdateController(ObjectField field, int y, string label)
		{
			var line = this.lines[field];

			line.Frame.Visibility = true;
			line.Frame.Margins = new Margins (0, 0, y, 0);
			line.Frame.BackColor = (field == this.selectedField) ? ColorManager.WindowBackgroundColor : Color.Empty;

			line.Label.Text = label;

			line.Controller.Field         = field;
			line.Controller.Value         = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
			line.Controller.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (field);
		}


		private IEnumerable<ObjectField> SortedFields
		{
			get
			{
				return this.UsedFields.OrderBy (x => this.GetName (x));
			}
		}

		private string GetName(ObjectField field)
		{
			var gr = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
			return GroupsLogic.GetFullName (this.accessor, gr.Guid);
		}

		private IEnumerable<ObjectField> UsedFields
		{
			get
			{
				for (int i=0; i<10; i++)
				{
					var field = ObjectField.GroupGuidRatio+i;

					var gr = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
					if (!gr.IsEmpty)
					{
						yield return field;
					}
				}
			}
		}

		private ObjectField FreeField
		{
			get
			{
				for (int i=0; i<10; i++)
				{
					var field = ObjectField.GroupGuidRatio+i;

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

	
		private readonly DataAccessor					accessor;
		private readonly Dictionary<ObjectField, Line>	lines;

		private FrameBox								controllersFrame;
		private ObjectField								selectedField;
	}
}
