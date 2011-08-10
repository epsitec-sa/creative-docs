//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.PlugIns;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public sealed class DocumentCategoryOptionsSubController
	{
		public DocumentCategoryOptionsSubController(IBusinessContext businessContext, DocumentCategoryEntity documentCategoryEntity)
		{
			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (documentCategoryEntity.IsNotNull ());

			this.businessContext        = businessContext;
			this.documentCategoryEntity = documentCategoryEntity;

			this.optionInformations = new List<OptionInformation> ();
			this.optionGroups = new List<OptionGroup> ();
			this.warningOptions = new List<DocumentOption> ();
		}


		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.checkButtonsFrame = new Scrollable
			{
				Parent = box,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = true,
				Margins = new Margins (0, 0, 10, 0),
			};
			this.checkButtonsFrame.Viewport.IsAutoFitting = true;
			this.checkButtonsFrame.ViewportPadding = new Margins (-1);

			this.CreateCheckButtons ();

			this.CreateWarning (box);
			this.UpdateWarning ();
		}

		public void UpdateAfterDocumentTypeChanged()
		{
			this.CreateCheckButtons ();
			this.UpdateWarning ();
		}


		private void CreateWarning(Widget parent)
		{
			this.warning = new StaticText
			{
				Parent = parent,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				BackColor = Color.FromRgb (255.0/255.0, 147.0/255.0, 147.0/255.0),  // rouge
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 5, 0),
			};
		}

		private void CreateCheckButtons()
		{
			this.UpdateData ();

			var parent = this.checkButtonsFrame.Viewport;
			parent.Children.Clear ();

			this.firstGroup = true;
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used == x.Total), "Options parfaitement adaptées",  Color.FromRgb (221.0/255.0, 255.0/255.0, 227.0/255.0));  // vert clair
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used <  x.Total), "Options partiellement adaptées", Color.FromRgb (255.0/255.0, 246.0/255.0, 224.0/255.0));  // orange clair
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used == 0                     ), "Options pas adaptées",           Color.FromRgb (255.0/255.0, 224.0/255.0, 224.0/255.0));  // rouge clair
		}

		private void CreateGroup(Widget parent, IEnumerable<OptionGroup> optionGroups, FormattedText title, Color color)
		{
			if (optionGroups.Any ())
			{
				var frame = this.CreateColorizedFrameBox (parent, color);
				this.CreateTitle (frame, title);

				foreach (var group in optionGroups)
				{
					this.CreateCheckButton (frame, group);
				}
			}
		}

		private FrameBox CreateColorizedFrameBox(Widget parent, Color color)
		{
			var box = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = color,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, this.firstGroup ? 0 : -1, 0),
				Padding = new Margins (5),
			};

			this.firstGroup = false;

			return box;
		}

		private void CreateTitle(Widget parent, FormattedText title)
		{
			new StaticText
			{
				Parent = parent,
				FormattedText = FormattedText.Concat (title, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};
		}

		private void CreateCheckButton(Widget parent, OptionGroup group)
		{
			int index = this.optionGroups.IndexOf (group);

			if (group.IsRadio)
			{
				var frame = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
				};

				var leftFrame = new FrameBox
				{
					Parent = frame,
					Dock = DockStyle.Fill,
				};

				var rightFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = 40,
					Dock = DockStyle.Right,
				};

				new StaticText
				{
					Parent = rightFrame,
					Text = group.Ratio,
					ContentAlignment = ContentAlignment.MiddleRight,
					Dock = DockStyle.Fill,
				};

				var first = new RadioButton
				{
					Parent = leftFrame,
					FormattedText = "Aucune option",
					Name = string.Concat (index.ToString (), ".-1"),
					Group = group.Name,
					PreferredHeight = 15,
					Dock = DockStyle.Top,
				};

				var firstState = ActiveState.Yes;

				for (int i = 0; i < group.OptionInformations.Count; i++)
				{
					var entity = group.OptionInformations[i].Entity;
					bool check = this.documentCategoryEntity.DocumentOptions.Contains (entity);

					if (check)
					{
						firstState = ActiveState.No;
					}

					var button = new RadioButton
					{
						Parent = leftFrame,
						FormattedText = entity.Name,
						Name = string.Concat (index.ToString (), ".", i.ToString ()),
						Group = group.Name,
						ActiveState = check ? ActiveState.Yes : ActiveState.No,
						PreferredHeight = 15,
						Dock = DockStyle.Top,
						Margins = new Margins (0, 0, 0, (i == group.OptionInformations.Count-1) ? 5 : 0),
					};

					group.OptionInformations[i].Button = button;

					button.ActiveStateChanged += delegate
					{
						this.ButtonClicked (button);
					};
				}

				first.ActiveState = firstState;

				first.ActiveStateChanged += delegate
				{
					this.ButtonClicked (first);
				};
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = parent,
					PreferredHeight = 15,
					Dock = DockStyle.Top,
				};

				var entity = group.OptionInformations[0].Entity;
				bool check = this.documentCategoryEntity.DocumentOptions.Contains (entity);

				var button = new CheckButton
				{
					Parent = frame,
					FormattedText = entity.Name,
					Name = index.ToString (),
					ActiveState = check ? ActiveState.Yes : ActiveState.No,
					Dock = DockStyle.Fill,
				};

				group.OptionInformations[0].Button = button;

				button.ActiveStateChanged += delegate
				{
					this.ButtonClicked (button);
				};

				new StaticText
				{
					Parent = frame,
					Text = group.Ratio,
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth = 40,
					Dock = DockStyle.Right,
				};
			}
		}

		private void SetTooltip(Widget button, OptionInformation info)
		{
			var description = this.GetTooltipDescription (info);

			if (!description.IsNullOrEmpty)
			{
				ToolTip.Default.SetToolTip (button, description);
			}
		}

		private FormattedText GetTooltipDescription(OptionInformation info)
		{
			var list = new List<string> ();

			foreach (var option in Documents.Verbose.VerboseDocumentOption.GetAll ())
			{
				if (this.documentOptions != null &&
					this.documentOptions.Contains (option.Option) &&
					info.PrintingOptionDictionary.Options.Contains (option.Option))
				{
					var description = info.PrintingOptionDictionary.GetOptionDescription (option, hasBullet: false, hiliteValue: true);

					if (this.warningOptions.Contains (option.Option))
					{
						description = description.ApplyFontColor (Color.FromName ("Red"));
					}

					list.Add (description.ToString ());
				}
			}

			return string.Join ("<br/>", list);
		}


		private void ButtonClicked(AbstractButton button)
		{
			var parts = button.Name.Split ('.');

			if (parts.Length == 1 && button is CheckButton)
			{
				int index = int.Parse (parts[0]);
				var entity = this.optionGroups[index].OptionInformations[0].Entity;

				if (button.ActiveState == ActiveState.Yes)
				{
					if (!this.documentCategoryEntity.DocumentOptions.Contains (entity))
					{
						this.documentCategoryEntity.DocumentOptions.Add (entity);
					}
				}
				else
				{
					if (this.documentCategoryEntity.DocumentOptions.Contains (entity))
					{
						this.documentCategoryEntity.DocumentOptions.Remove (entity);
					}
				}
			}

			if (parts.Length == 2 && button is RadioButton)
			{
				if (button.ActiveState == ActiveState.Yes)
				{
					int index = int.Parse (parts[0]);
					int group = int.Parse (parts[1]);

					for (int i = 0; i < this.optionGroups[index].OptionInformations.Count; i++)
					{
						var entity = this.optionGroups[index].OptionInformations[i].Entity;

						if (this.documentCategoryEntity.DocumentOptions.Contains (entity))
						{
							this.documentCategoryEntity.DocumentOptions.Remove (entity);
						}
					}

					if (group != -1)
					{
						var entity = this.optionGroups[index].OptionInformations[group].Entity;
						this.documentCategoryEntity.DocumentOptions.Add (entity);
					}
				}
			}

			this.UpdateWarning ();
		}


		private void UpdateWarning()
		{
			this.warningOptions.Clear ();
			var options = new List<DocumentOption> ();
			int error = 0;

			foreach (var documentOptionEntity in this.documentCategoryEntity.DocumentOptions)
			{
				var info = this.optionInformations.Where (x => x.Entity == documentOptionEntity).FirstOrDefault ();

				foreach (var option in info.Options)
				{
					if (this.documentOptions != null && this.documentOptions.Contains (option))
					{
						if (options.Contains (option))
						{
							this.warningOptions.Add (option);
							error++;
						}
						else
						{
							options.Add (option);
						}
					}
				}
			}

			foreach (var info in this.optionInformations)
			{
				if (info.Button != null)
				{
					this.SetTooltip (info.Button, info);
				}
			}

			if (error == 0)
			{
				this.warning.Visibility = false;
			}
			else
			{
				this.warning.Visibility = true;

				FormattedText message;
				if (error == 1)
				{
					message = "Il y a une option définie plusieurs fois !";
				}
				else
				{
					message = string.Format ("Il y a {0} options définies plusieurs fois !", error.ToString ());
				}

				this.warning.FormattedText = message.ApplyBold ();
			}
		}


		private void UpdateData()
		{
			this.UpdateOptionInformations ();
			this.UpdateOptionGroup ();
		}

		private void UpdateOptionGroup()
		{
			this.optionGroups.Clear ();

			foreach (var info in this.optionInformations)
			{
				if (!info.IsRadio)
				{
					var group = new OptionGroup (info);

					foreach (var friend in this.optionInformations)
					{
						if (friend != info && !friend.IsRadio && friend.HasSameOptions (info))
						{
							info.IsRadio = true;
							friend.IsRadio = true;
							group.OptionInformations.Add (friend);
						}
					}

					this.optionGroups.Add (group);
				}
			}
		}

		private void UpdateOptionInformations()
		{
			this.documentOptions = Epsitec.Cresus.Core.Documents.External.CresusCore.GetRequiredDocumentOptionsByDocumentType (this.documentCategoryEntity.DocumentType);
			this.optionInformations.Clear ();

			var optionEntities = this.businessContext.GetAllEntities<DocumentOptionsEntity> ();
			foreach (var optionEntity in optionEntities)
			{
				this.optionInformations.Add (this.GetOptionInformation (optionEntity));
			}
		}

		private OptionInformation GetOptionInformation(DocumentOptionsEntity optionEntity)
		{
			var printingOptionDictionary = optionEntity.GetOptions ();

			int count = 0;
			int total = 0;

			if (this.documentOptions != null)
			{
				foreach (var option in printingOptionDictionary.Options)
				{
					if (this.documentOptions.Contains (option))
					{
						count++;
					}

					total++;
				}
			}

			return new OptionInformation (optionEntity, printingOptionDictionary, count, total, printingOptionDictionary.Options);
		}


		private class OptionGroup
		{
			public OptionGroup(OptionInformation optionInformation)
			{
				this.optionInformations = new List<OptionInformation> ();
				this.optionInformations.Add (optionInformation);
			}

			public List<OptionInformation> OptionInformations
			{
				get
				{
					return this.optionInformations;
				}
			}

			public string Ratio
			{
				get
				{
					return string.Concat (this.Used.ToString (), "/", this.Total.ToString ());
				}
			}

			public int Used
			{
				get
				{
					return this.optionInformations[0].Used;
				}
			}

			public int Total
			{
				get
				{
					return this.optionInformations[0].Total;
				}
			}

			public string Name
			{
				get
				{
					return this.optionInformations[0].Entity.Name.ToString ();
				}
			}

			public bool IsRadio
			{
				get
				{
					return this.optionInformations.Count > 1;
				}
			}

			private readonly List<OptionInformation> optionInformations;
		}

		private class OptionInformation
		{
			public OptionInformation(DocumentOptionsEntity entity, PrintingOptionDictionary printingOptionDictionary, int used, int total, IEnumerable<DocumentOption> options)
			{
				this.Entity                   = entity;
				this.PrintingOptionDictionary = printingOptionDictionary;
				this.Used                     = used;
				this.Total                    = total;
				this.options                  = options;
			}

			public DocumentOptionsEntity Entity
			{
				get;
				private set;
			}

			public PrintingOptionDictionary PrintingOptionDictionary
			{
				get;
				private set;
			}

			public int Used
			{
				get;
				private set;
			}

			public int Total
			{
				get;
				private set;
			}

			public bool IsRadio
			{
				get;
				set;
			}

			public AbstractButton Button
			{
				get;
				set;
			}

			public IEnumerable<DocumentOption> Options
			{
				get
				{
					return this.options;
				}
			}

			public bool HasSameOptions(OptionInformation that)
			{
				if (this.options.Count () != that.options.Count ())
				{
					return false;
				}

				foreach (var option in this.options)
				{
					if (!that.options.Contains (option))
					{
						return false;
					}
				}

				return true;
			}

			private readonly IEnumerable<DocumentOption> options;
		}


		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;
		private readonly List<OptionInformation>			optionInformations;
		private readonly List<OptionGroup>					optionGroups;
		private readonly List<DocumentOption>				warningOptions;

		private Scrollable									checkButtonsFrame;
		private bool										firstGroup;
		private IEnumerable<DocumentOption>					documentOptions;
		private StaticText									warning;
	}
}
