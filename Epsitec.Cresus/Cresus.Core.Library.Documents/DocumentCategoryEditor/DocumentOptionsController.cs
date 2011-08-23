//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.PlugIns;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentCategoryController
{
	public sealed class DocumentOptionsController
	{
		public DocumentOptionsController(DocumentCategoryController documentCategoryController)
		{
			this.documentCategoryController = documentCategoryController;
			this.businessContext            = this.documentCategoryController.BusinessContext;
			this.documentCategoryEntity     = this.documentCategoryController.DocumentCategoryEntity;

			this.verboseDocumentOptions = VerboseDocumentOption.GetAll ().Where (x => x.Option != DocumentOption.None);
			this.optionInformations = new List<OptionInformation> ();
			this.optionGroups = new List<OptionGroup> ();
			this.errorOptions = new List<DocumentOption> ();
		}


		public void CreateUI(Widget parent)
		{
			var tile = new ArrowedTileFrame (Direction.Right)
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 10, 0),
				Padding = TileArrow.GetContainerPadding (Direction.Right) + new Margins (Library.UI.Constants.TileInternalPadding),
			};

			tile.SetSelected (true);  // conteneur orange

			var box = new FrameBox
			{
				Parent = tile,
				Dock = DockStyle.Fill,
				BackColor = TileColors.SurfaceDefaultColors.First (),
			};

			this.checkButtonsFrame = new Scrollable
			{
				Parent = box,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = true,
			};
			this.checkButtonsFrame.Viewport.IsAutoFitting = true;
			this.checkButtonsFrame.ViewportPadding = new Margins (-1);

			this.CreateCheckButtons ();

			this.CreateMissing (box);
			this.CreateUseless (box);
			this.CreateError (box);
			this.UpdateErrorAndWarning ();
		}

		public void UpdateAfterDocumentTypeChanged()
		{
			this.CreateCheckButtons ();
			this.UpdateErrorAndWarning ();
		}


		public IEnumerable<DocumentOption> ErrorOptions
		{
			get
			{
				return this.errorOptions;
			}
		}


		private void CreateError(Widget parent)
		{
			this.errorFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = DocumentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.errorText = new StaticText
			{
				Parent = this.errorFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};
		}

		private void CreateUseless(Widget parent)
		{
			this.uselessFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = DocumentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.uselessText = new StaticText
			{
				Parent = this.uselessFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};
		}

		private void CreateMissing(Widget parent)
		{
			this.missingFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = DocumentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.missingText = new StaticText
			{
				Parent = this.missingFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};
		}

		private void CreateCheckButtons()
		{
			this.UpdateData ();

			var parent = this.checkButtonsFrame.Viewport;
			parent.Children.Clear ();

			this.firstGroup = true;
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used == x.Total), "Options parfaitement adaptées",  DocumentCategoryController.acceptedColor);
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used <  x.Total), "Options partiellement adaptées", DocumentCategoryController.toleratedColor);
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used == 0                     ), "Options inadaptées",             DocumentCategoryController.rejectedColor);
		}

		private void CreateGroup(Widget parent, IEnumerable<OptionGroup> optionGroups, FormattedText title, Color color)
		{
			if (optionGroups.Any ())
			{
				var frame = this.CreateColorizedFrameBox (parent, color);
				this.CreateTitle (frame, title);

				this.lastButton = '-';

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
			if (this.lastButton == 'r' || (group.IsRadio && this.lastButton != '-'))
			{
				//	Met une petite séparation.
				new FrameBox
				{
					Parent = parent,
					PreferredHeight = 5,
					Dock = DockStyle.Top,
				};
			}

			this.lastButton = group.IsRadio ? 'r' : 'c';

			int index = this.optionGroups.IndexOf (group);

			if (group.IsRadio)
			{
				var frame = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
				};

				var errorFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = DocumentCategoryController.errorBulletWidth,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, DocumentCategoryController.lineHeight, 0),
				};

				var uselessFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = DocumentCategoryController.errorBulletWidth,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, DocumentCategoryController.lineHeight, 0),
				};

				var leftFrame = new FrameBox
				{
					Parent = frame,
					Dock = DockStyle.Fill,
				};

				var rightFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = DocumentOptionsController.ratioWidth,
					Dock = DockStyle.Right,
				};

				var ratio = new StaticText
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
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Top,
				};

				string firstTooltip = (group.Used <= 1) ? "Enlève une option aux définitions" : string.Format ("Enlève {0} options aux définitions", group.Used.ToString ());
				ToolTip.Default.SetToolTip (first, firstTooltip);

				var firstState = ActiveState.Yes;

				for (int i = 0; i < group.OptionInformations.Count; i++)
				{
					var error = new StaticText
					{
						Parent = errorFrame,
						ContentAlignment = ContentAlignment.MiddleLeft,
						PreferredWidth = DocumentCategoryController.errorBulletWidth,
						PreferredHeight = DocumentCategoryController.lineHeight,
						Dock = DockStyle.Top,
					};

					group.OptionInformations[i].ErrorVisibility = errorFrame;
					group.OptionInformations[i].ErrorText = error;

					var useless = new StaticText
					{
						Parent = uselessFrame,
						ContentAlignment = ContentAlignment.MiddleLeft,
						PreferredWidth = DocumentCategoryController.errorBulletWidth,
						PreferredHeight = DocumentCategoryController.lineHeight,
						Dock = DockStyle.Top,
					};

					group.OptionInformations[i].UselessVisibility = uselessFrame;
					group.OptionInformations[i].UselessText = useless;

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
						PreferredHeight = DocumentCategoryController.lineHeight,
						Dock = DockStyle.Top,
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
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Top,
				};

				var error = new StaticText
				{
					Parent = frame,
					ContentAlignment = ContentAlignment.MiddleLeft,
					PreferredWidth = DocumentCategoryController.errorBulletWidth,
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Left,
				};

				group.OptionInformations[0].ErrorVisibility = error;
				group.OptionInformations[0].ErrorText = error;

				var useless = new StaticText
				{
					Parent = frame,
					ContentAlignment = ContentAlignment.MiddleLeft,
					PreferredWidth = DocumentCategoryController.errorBulletWidth,
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Left,
				};

				group.OptionInformations[0].UselessVisibility = useless;
				group.OptionInformations[0].UselessText = useless;

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

				var ratio = new StaticText
				{
					Parent = frame,
					Text = group.Ratio,
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth = DocumentOptionsController.ratioWidth,
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
			//	Retourne le contenu pour un tooltip, avec les valeurs.
			bool active = this.documentCategoryEntity.DocumentOptions.Contains (info.Entity);  // option cochée ?

			if (active)
			{
				//	Cherche s'il faut utiliser les puces.
				bool hasBullet = false;

				foreach (var verboseOption in this.verboseDocumentOptions)
				{
					if (info.PrintingOptionDictionary.Options.Contains (verboseOption.Option))
					{
						if (this.errorOptions.Contains (verboseOption.Option) ||
                            !this.RequiredDocumentOptionsContains (verboseOption.Option))
						{
							hasBullet = true;
						}
					}
				}

				//	Construit les lignes, une par option.
				var list = new List<string> ();

				int correctCount = 0;
				int errorCount   = 0;
				int uselessCount = 0;

				foreach (var verboseOption in this.verboseDocumentOptions)
				{
					if (info.PrintingOptionDictionary.Options.Contains (verboseOption.Option))
					{
						var description = info.PrintingOptionDictionary.GetOptionDescription (verboseOption, hasBullet: false, hiliteValue: true);

						if (this.errorOptions.Contains (verboseOption.Option))
						{
							description = description.ApplyFontColor (DocumentCategoryController.errorColor);

							if (hasBullet)
							{
								description = FormattedText.Concat (DocumentCategoryController.errorBullet, "  ", description);
							}

							errorCount++;
						}
						else if (!this.RequiredDocumentOptionsContains (verboseOption.Option))
						{
							description = description.ApplyFontColor (DocumentCategoryController.uselessColor);

							if (hasBullet)
							{
								description = FormattedText.Concat (DocumentCategoryController.uselessBullet, "  ", description);
							}

							uselessCount++;
						}
						else
						{
							if (hasBullet)
							{
								description = FormattedText.Concat (DocumentCategoryController.normalBullet, "  ", description);
							}

							correctCount++;
						}

						list.Add (description.ToString ());
					}
				}

				//	Génère les textes.
				FormattedText correctText = "";
				FormattedText errorText   = "";
				FormattedText uselessText = "";

				if (correctCount != 0)
				{
					if (correctCount == 1)
					{
						correctText = "Une option est définie correctement<br/>";
					}
					else
					{
						correctText = string.Format ("{0} options sont définies correctement<br/>", correctCount.ToString ());
					}

					if (hasBullet)
					{
						correctText = FormattedText.Concat (DocumentCategoryController.normalBullet, "  ", correctText);
					}
				}

				if (errorCount != 0)
				{
					if (errorCount == 1)
					{
						errorText = "Une option est définie plusieurs fois<br/>";
					}
					else
					{
						errorText = string.Format ("{0} options sont définies plusieurs fois<br/>", errorCount.ToString ());
					}

					if (hasBullet)
					{
						errorText = FormattedText.Concat (DocumentCategoryController.errorBullet, "  ", errorText);
					}

					errorText = errorText.ApplyFontColor (DocumentCategoryController.errorColor);
				}

				if (uselessCount != 0)
				{
					if (uselessCount == 1)
					{
						uselessText = "Une option est définie inutilement<br/>";
					}
					else
					{
						uselessText = string.Format ("{0} options sont définies inutilement<br/>", uselessCount.ToString ());
					}

					if (hasBullet)
					{
						uselessText = FormattedText.Concat (DocumentCategoryController.uselessBullet, "  ", uselessText);
					}

					uselessText = uselessText.ApplyFontColor (DocumentCategoryController.uselessColor);
				}

				//	Génère le texte final.
				return string.Concat (correctText, errorText, uselessText, DocumentOptionsController.MenuSeparator, string.Join ("<br/>", list));
			}
			else  // option non cochée ?
			{
				if (info.Used == 0)
				{
					return "Ce bouton n'est pas adapté";
				}
				else if (info.Used == 1)
				{
					return "Ajoute une option aux définitions";
				}
				else
				{
					return string.Format ("Ajoute {0} options aux définitions", info.Used.ToString ());
				}
			}
		}

		private FormattedText GetMissingTooltipDescription(FormattedText fix, List<DocumentOption> usedOptions)
		{
			var list = new List<DocumentOption> ();

			if (this.requiredDocumentOptions != null)
			{
				foreach (var option in this.requiredDocumentOptions)
				{
					if (!usedOptions.Contains (option))
					{
						list.Add (option);
					}
				}
			}

			return this.GetTooltipDescription (fix, list, DocumentCategoryController.missingColor);
		}

		private FormattedText GetTooltipDescription(FormattedText fix, List<DocumentOption> options, Color color)
		{
			//	Retourne le contenu pour un tooltip, sans les valeurs.
			var list = new List<string> ();

			foreach (var verboseOption in this.verboseDocumentOptions)
			{
				if (options.Contains (verboseOption.Option))
				{
					var description = verboseOption.Description;
					list.Add (description.ToString ());
				}
			}

			FormattedText x = string.Join ("<br/>", list);

			return FormattedText.Concat (fix, " :<br/>", DocumentOptionsController.MenuSeparator, x.ApplyFontColor (color));
		}

		private static string MenuSeparator
		{
			get
			{
				var hline = new string ('_', 80);
				return string.Concat ("<font size=\"40%\">", hline, "<br/> <br/></font>");
			}
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

			this.UpdateErrorAndWarning ();
			this.documentCategoryController.UpdateAfterOptionChanged ();
		}


		private void UpdateErrorAndWarning()
		{
			this.errorOptions.Clear ();

			var usedOptions    = new List<DocumentOption> ();
			var uselessOptions = new List<DocumentOption> ();

			int error   = 0;
			int missing = 0;
			int useless = 0;

			foreach (var documentOptionEntity in this.documentCategoryEntity.DocumentOptions)
			{
				var info = this.optionInformations.Where (x => x.Entity == documentOptionEntity).FirstOrDefault ();

				foreach (var option in info.Options)
				{
					if (this.RequiredDocumentOptionsContains (option))
					{
						if (usedOptions.Contains (option))
						{
							if (!this.errorOptions.Contains (option))
							{
								this.errorOptions.Add (option);
								error++;
							}
						}
						else
						{
							usedOptions.Add (option);
						}
					}
					else
					{
						if (!uselessOptions.Contains (option))
						{
							uselessOptions.Add (option);
						}
					}
				}
			}

			int required = this.RequiredDocumentOptionsCount;

			if (usedOptions.Count < required)
			{
				missing = required - usedOptions.Count;
			}

			useless = uselessOptions.Count;

			foreach (var info in this.optionInformations)
			{
				if (info.ErrorText != null)
				{
					if (error == 0)
					{
						info.ErrorVisibility.Visibility = false;
					}
					else
					{
						info.ErrorVisibility.Visibility = true;

						FormattedText text = null;

						foreach (var option in info.Options)
						{
							if (this.errorOptions.Contains (option))
							{
								text = DocumentCategoryController.errorBullet;
								break;
							}
						}

						info.ErrorText.FormattedText = text;

						if (text.IsNullOrEmpty)
						{
							ToolTip.Default.HideToolTipForWidget (info.ErrorText);
						}
						else
						{
							ToolTip.Default.SetToolTip (info.ErrorText, "Contient des options définies plusieurs fois dont <b>les valeurs sont aléatoires</b>");
						}
					}
				}

				if (info.UselessText != null)
				{
					if (useless == 0)
					{
						info.UselessVisibility.Visibility = false;
					}
					else
					{
						info.UselessVisibility.Visibility = true;

						FormattedText text = null;

						if (this.documentCategoryEntity.DocumentOptions.Contains (info.Entity))
						{
							foreach (var option in info.Options)
							{
								if (uselessOptions.Contains (option))
								{
									text = DocumentCategoryController.uselessBullet;
									break;
								}
							}
						}

						info.UselessText.FormattedText = text;

						if (text.IsNullOrEmpty)
						{
							ToolTip.Default.HideToolTipForWidget (info.UselessText);
						}
						else
						{
							ToolTip.Default.SetToolTip (info.UselessText, "Contient des options définies inutilement");
						}
					}
				}

				if (info.Button != null)
				{
					this.SetTooltip (info.Button, info);
				}
			}

			FormattedText errorMessage   = null;
			FormattedText errorTooltip   = null;
			FormattedText uselessMessage = null;
			FormattedText uselessTooltip = null;
			FormattedText missingMessage = null;
			FormattedText missingTooltip = null;

			if (error != 0)
			{
				if (error == 1)
				{
					errorMessage = "Il y a une option définie plusieurs fois";
					errorTooltip = this.GetTooltipDescription ("L'option suivante est définie plusieurs fois<br/>et a une <b>valeur aléatoire</b>", this.errorOptions, DocumentCategoryController.errorColor);
				}
				else
				{
					errorMessage = string.Format ("Il y a {0} options définies plusieurs fois", error.ToString ());
					errorTooltip = this.GetTooltipDescription ("Les options suivantes sont définies plusieurs fois<br/>et ont des <b>valeurs aléatoires</b>", this.errorOptions, DocumentCategoryController.errorColor);
				}

				errorMessage = FormattedText.Concat (DocumentCategoryController.errorBullet, "  ", errorMessage.ApplyBold ());
			}

			if (missing != 0)
			{
				if (missing == 1)
				{
					missingMessage = "Il y a une option indéfinie";
					missingTooltip = this.GetMissingTooltipDescription ("L'option suivante est indéfinie<br/>et prend la valeur par défaut", usedOptions);
				}
				else
				{
					missingMessage = string.Format ("Il y a {0} options indéfinies", missing.ToString ());
					missingTooltip = this.GetMissingTooltipDescription ("Les options suivantes sont indéfinies<br/>et prennent les valeurs par défaut", usedOptions);
				}

				missingMessage = FormattedText.Concat (DocumentCategoryController.missingBullet, "  ", missingMessage);
			}

			if (useless != 0)
			{
				if (useless == 1)
				{
					uselessMessage = "Il y a une option définie inutilement";
					uselessTooltip = this.GetTooltipDescription ("L'options suivante est définie inutilement", uselessOptions, DocumentCategoryController.uselessColor);
				}
				else
				{
					uselessMessage = string.Format ("Il y a {0} options définies inutilement", useless.ToString ());
					uselessTooltip = this.GetTooltipDescription ("Les options suivantes sont définies inutilement", uselessOptions, DocumentCategoryController.uselessColor);
				}

				uselessMessage = FormattedText.Concat (DocumentCategoryController.uselessBullet, "  ", uselessMessage);
			}

			if (this.documentCategoryEntity.DocumentOptions.Count == 0)
			{
				missingMessage = "Aucune option d'impression n'est choisie";
			}

			if (errorMessage.IsNullOrEmpty)
			{
				this.errorFrame.Visibility = false;
			}
			else
			{
				this.errorFrame.Visibility = true;
				this.errorText.FormattedText = errorMessage;

				ToolTip.Default.SetToolTip (this.errorFrame, errorTooltip);
			}

			if (uselessMessage.IsNullOrEmpty)
			{
				this.uselessFrame.Visibility = false;
			}
			else
			{
				this.uselessFrame.Visibility = true;
				this.uselessText.FormattedText = uselessMessage;

				ToolTip.Default.SetToolTip (this.uselessFrame, uselessTooltip);
			}

			if (missingMessage.IsNullOrEmpty)
			{
				this.missingFrame.Visibility = false;
			}
			else
			{
				this.missingFrame.Visibility = true;
				this.missingText.FormattedText = missingMessage;

				ToolTip.Default.SetToolTip (this.missingFrame, missingTooltip);
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
			this.requiredDocumentOptions = Epsitec.Cresus.Core.Documents.External.CresusCore.GetRequiredDocumentOptionsByDocumentType (this.documentCategoryEntity.DocumentType);
			
			this.optionInformations.Clear ();

			var optionEntities = this.businessContext.GetAllEntities<DocumentOptionsEntity> ().OrderBy (x => x.Name);
			foreach (var optionEntity in optionEntities)
			{
				this.optionInformations.Add (this.GetOptionInformation (optionEntity));
			}
		}

		private OptionInformation GetOptionInformation(DocumentOptionsEntity optionEntity)
		{
			var printingOptionDictionary = optionEntity.GetOptions ();
			var options = printingOptionDictionary.Options.Where (x => x != DocumentOption.None);

			int count = 0;
			int total = 0;

			if (this.requiredDocumentOptions != null)
			{
				foreach (var option in options)
				{
					if (this.RequiredDocumentOptionsContains (option))
					{
						count++;
					}

					total++;
				}
			}

			return new OptionInformation (optionEntity, printingOptionDictionary, options, count, total);
		}


		public bool RequiredDocumentOptionsContains(DocumentOption option)
		{
			return this.requiredDocumentOptions != null && this.requiredDocumentOptions.Contains (option);
		}

		public int RequiredDocumentOptionsCount
		{
			get
			{
				return (this.requiredDocumentOptions == null) ? 0 : this.requiredDocumentOptions.Count ();
			}
		}


		private class OptionGroup
		{
			public OptionGroup(OptionInformation optionInformation)
			{
				this.optionInformations = new List<OptionInformation> ();
				this.optionInformations.Add (optionInformation);
			}

			public DocumentOptionsEntity Entity
			{
				get
				{
					return this.optionInformations[0].Entity;
				}
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
			public OptionInformation(DocumentOptionsEntity entity, PrintingOptionDictionary printingOptionDictionary, IEnumerable<DocumentOption> options, int used, int total)
			{
				this.Entity                   = entity;
				this.PrintingOptionDictionary = printingOptionDictionary;
				this.options                  = options;
				this.Used                     = used;
				this.Total                    = total;
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

			public string RatioDescription
			{
				get
				{
					if (this.Used == this.Total && this.Used != 0)
					{
						if (this.Used == 1)
						{
							return "L'option est utile pour ce type de document";
						}
						else
						{
							return string.Format ("Les {0} options sont toutes utiles pour ce type de document", this.Used.ToString ());
						}
					}
					else
					{
						int uselessCount = this.Total - this.Used;
						string uselessText = "";

						if (uselessCount <= 1)
						{
							uselessText = "Une option est définie inutilement (rouge)";
						}
						else
						{
							uselessText = string.Format ("{0} options sont définies inutilement (rouge)", uselessCount.ToString ());
						}

						if (this.Used == 0)
						{
							return string.Format ("Aucune option utile pour ce type de document, sur un total de {0}", this.Total.ToString ());
						}
						else if (this.Used == 1)
						{
							return string.Format ("Une option utile pour ce type de document, sur un total de {0} (noir ou bleu)<br/>{1}", this.Total.ToString (), uselessText);
						}
						else
						{
							return string.Format ("{0} options utiles pour ce type de document, sur un total de {1} (noir et bleu)<br/>{2}", this.Used.ToString (), this.Total.ToString (), uselessText);
						}
					}
				}
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

			public Widget ErrorVisibility
			{
				get;
				set;
			}

			public StaticText ErrorText
			{
				get;
				set;
			}

			public Widget UselessVisibility
			{
				get;
				set;
			}

			public StaticText UselessText
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


		private static readonly int ratioWidth = 40;

		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;
		private readonly DocumentCategoryController			documentCategoryController;
		private readonly IEnumerable<VerboseDocumentOption>	verboseDocumentOptions;
		private readonly List<OptionInformation>			optionInformations;
		private readonly List<OptionGroup>					optionGroups;
		private readonly List<DocumentOption>				errorOptions;

		private Scrollable									checkButtonsFrame;
		private bool										firstGroup;
		private char										lastButton;
		private IEnumerable<DocumentOption>					requiredDocumentOptions;
		private FrameBox									errorFrame;
		private StaticText									errorText;
		private FrameBox									uselessFrame;
		private StaticText									uselessText;
		private FrameBox									missingFrame;
		private StaticText									missingText;
	}
}
