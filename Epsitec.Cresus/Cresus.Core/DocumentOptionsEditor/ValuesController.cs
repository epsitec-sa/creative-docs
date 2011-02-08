//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Entities;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public class ValuesController
	{
		public ValuesController(Core.Business.BusinessContext businessContext, DocumentOptionsEntity documentOptionsEntity, Dictionary<string, string> options)
		{
			this.businessContext       = businessContext;
			this.documentOptionsEntity = documentOptionsEntity;
			this.options               = options;

			this.allOptions   = DocumentOption.GetAllDocumentOptions ().Where (x => !x.IsTitle).ToList ();
			this.titleOptions = DocumentOption.GetAllDocumentOptions ().Where (x => x.IsTitle).ToList ();

			this.buttons = new List<AbstractButton> ();
		}


		public void CreateUI(Widget parent)
		{
			this.optionsFrame = new Scrollable
			{
				Parent = parent,
				PreferredWidth = 300,
				Dock = DockStyle.Left,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = false,
			};

			this.optionsFrame.Viewport.IsAutoFitting = true;

			this.UpdateOptionButtons ();
		}


		public void Update()
		{
			this.UpdateOptionButtons ();
		}


		private void UpdateOptionButtons()
		{
			this.optionsFrame.Viewport.Children.Clear ();
			this.buttons.Clear ();

			string lastGroup = null;
			int tabIndex = 0;

			foreach (var option in this.allOptions)
			{
				if (this.options.ContainsKey (option.Name))
				{
					if (lastGroup != option.Group)
					{
						lastGroup = option.Group;

						if (!string.IsNullOrEmpty (option.Group))
						{
							var t = this.titleOptions.Where (x => x.Group == lastGroup).FirstOrDefault ();

							if (t != null)
							{
								var title = new StaticText
								{
									Parent = this.optionsFrame.Viewport,
									Text = t.Title,
									Dock = DockStyle.Top,
									Margins = new Margins (0, 0, 10, 5),
								};
							}
						}
					}

					string value = this.options[option.Name];

					if (option.Widget == DocumentOptionWidget.CheckButton)
					{
						var button = new CheckButton
						{
							Parent = this.optionsFrame.Viewport,
							Name = option.Name,
							Text = option.Description,
							ActiveState = (value == "true") ? ActiveState.Yes : ActiveState.No,
							Dock = DockStyle.Top,
							AutoToggle = false,
							TabIndex = ++tabIndex,
						};

						button.Clicked += delegate
						{
							this.CheckButtonAction (button.Name);
						};

						this.buttons.Add (button);
					}

					if (option.Widget == DocumentOptionWidget.RadioButton)
					{
						var button = new RadioButton
						{
							Parent = this.optionsFrame.Viewport,
							Name = option.Name,
							Group = option.Group,
							Text = option.Description,
							ActiveState = (value == "true") ? ActiveState.Yes : ActiveState.No,
							Dock = DockStyle.Top,
							AutoToggle = false,
							TabIndex = ++tabIndex,
						};

						button.Clicked += delegate
						{
							this.RadioButtonAction (button.Name);
						};

						this.buttons.Add (button);
					}
				}
			}
		}


		private void RadioButtonAction(string name)
		{
			string group = this.allOptions.Where (x => x.Name == name).Select (x => x.Group).FirstOrDefault ();

			if (!string.IsNullOrEmpty (group))
			{
				var groups = this.allOptions.Where (x => x.Group == group && x.Widget == DocumentOptionWidget.RadioButton).Select (x => x.Name);

				foreach (var key in this.options.Keys.ToArray ())
				{
					if (groups.Contains (key))
					{
						this.options[key] = "false";
					}
				}
			}

			this.CheckButtonAction (name);
		}

		private void CheckButtonAction(string name)
		{
			if (this.options[name] == "false")
			{
				this.options[name] = "true";
			}
			else
			{
				this.options[name] = "false";
			}

			this.UpdateActiveStateButtons ();
			this.SetDirty ();
		}

		private void UpdateActiveStateButtons()
		{
			foreach (var button in this.buttons)
			{
				if (button is CheckButton ||
					button is RadioButton )
				{
					if (this.options.ContainsKey (button.Name))
					{
						button.ActiveState = (this.options[button.Name] == "true") ? ActiveState.Yes : ActiveState.No;
					}
				}
			}
		}


		private void SetDirty()
		{
			this.businessContext.NotifyExternalChanges ();
		}


		private readonly Core.Business.BusinessContext		businessContext;
		private readonly DocumentOptionsEntity				documentOptionsEntity;
		private readonly Dictionary<string, string>			options;
		private readonly List<DocumentOption>				allOptions;
		private readonly List<DocumentOption>				titleOptions;
		private readonly List<AbstractButton>				buttons;

		private Scrollable									optionsFrame;
	}
}
