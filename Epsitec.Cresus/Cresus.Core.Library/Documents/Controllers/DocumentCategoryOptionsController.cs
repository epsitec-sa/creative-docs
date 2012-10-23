﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Core.Resolvers;

namespace Epsitec.Cresus.Core.DocumentCategoryController
{
	public sealed class DocumentOptionsController
	{
		public DocumentOptionsController(DocumentCategoryController documentCategoryController)
		{
			this.documentCategoryController = documentCategoryController;
			this.businessContext            = this.documentCategoryController.BusinessContext;
			this.documentCategory     = this.documentCategoryController.DocumentCategoryEntity;

			this.verboseDocumentOptions = VerboseDocumentOption.GetAll ().Where (x => x.Option != DocumentOption.None);
			this.optionInformations = new List<OptionInformation> ();
			this.additionnalOptionInformations = new List<OptionInformation> ();
			this.optionGroups = new List<OptionGroup> ();
			this.errorOptions = new List<DocumentOption> ();

			this.detailFrames = new List<FrameBox> ();
			this.detailButtons = new List<GlyphButton> ();
			this.detailEntities = new List<DocumentOptionsEntity> ();
		}


		public void CreateUI(Widget parent, double width)
		{
			this.width = width;

			var tile = new ArrowedTileFrame (Direction.Right)
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
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

			this.CreateError (box, "missing", out this.missingFrame, out this.missingText);
			this.CreateError (box, "useless", out this.uselessFrame, out this.uselessText);
			this.CreateError (box, "error",   out this.errorFrame,   out this.errorText);

			this.UpdateErrorAndWarning ();
		}

		public void UpdateAfterDocumentTypeChanged()
		{
			this.selectedOptionInformation = null;
			this.selectedSpecialDetail = null;

			this.CreateCheckButtons ();
			this.UpdateErrorAndWarning ();
			this.UpdateDetailButtons ();
		}


		public IEnumerable<DocumentOption> ErrorOptions
		{
			get
			{
				return this.errorOptions;
			}
		}


		private void CreateError(Widget parent, string name, out FrameBox frame, out StaticText text)
		{
			frame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = DocumentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			text = new StaticText
			{
				Parent = frame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};

			var detailButton = new GlyphButton
			{
				Parent = frame,
				GlyphShape = GlyphShape.TriangleRight,
				ButtonStyle = ButtonStyle.ToolItem,
				PreferredWidth = DocumentCategoryController.lineHeight,
				PreferredHeight = DocumentCategoryController.errorHeight,
				Dock = DockStyle.Right,
			};

			ToolTip.Default.SetToolTip (detailButton, "Montre le détail");

			detailButton.Clicked += delegate
			{
				if (this.selectedSpecialDetail == name)
				{
					this.selectedSpecialDetail = null;
				}
				else
				{
					this.selectedSpecialDetail = name;
				}

				this.selectedOptionInformation = null;

				this.UpdateDetailButtons ();
			};
		}

		private void CreateCheckButtons()
		{
			this.UpdateData ();

			var parent = this.checkButtonsFrame.Viewport;
			parent.Children.Clear ();

			this.detailFrames.Clear ();
			this.detailButtons.Clear ();
			this.detailEntities.Clear ();

			this.selectedOptionInformation = null;
			this.selectedSpecialDetail = null;

			this.firstGroup = true;
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used == x.Total), "Options parfaitement adaptées",  DocumentCategoryController.acceptedColor);
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used <  x.Total), "Options partiellement adaptées", DocumentCategoryController.toleratedColor);
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used == 0                     ), "Options inadaptées",             DocumentCategoryController.rejectedColor);

			this.UpdateButtonStates ();
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
				Padding = new Margins (0, 0, 5, 5),
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
				Margins = new Margins (5, 0, 0, 5),
			};
		}

		private void CreateCheckButton(Widget parent, OptionGroup group)
		{
			if (this.lastButton == 'r' || (group.IsRadio && this.lastButton != '-'))
			{
				//	Met une petite séparation horizontale.
				new FrameBox
				{
					Parent = parent,
					PreferredHeight = 5,
					Dock = DockStyle.Top,
				};
			}

			this.lastButton = group.IsRadio ? 'r' : 'c';

			if (group.IsRadio)  // plusieurs boutons radio ?
			{
				for (int i = 0; i < group.OptionInformations.Count; i++)
				{
					this.CreateCheckButton (parent, group, i, true);
				}
			}
			else  // un seul bouton à cocher ?
			{
				this.CreateCheckButton (parent, group, 0, false);
			}
		}

		private void CreateCheckButton(Widget parent, OptionGroup group, int i, bool isRadio)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = DocumentCategoryController.lineHeight,
				Dock = DockStyle.Top,
			};

			new FrameBox
			{
				Parent = frame,
				PreferredWidth = 5,
				PreferredHeight = DocumentCategoryController.lineHeight,
				Dock = DockStyle.Left,
			};

			var error = new StaticText
			{
				Parent = frame,
				ContentAlignment = ContentAlignment.MiddleLeft,
				PreferredWidth = DocumentCategoryController.errorBulletWidth,
				PreferredHeight = DocumentCategoryController.lineHeight,
				Dock = DockStyle.Left,
			};

			group.OptionInformations[i].ErrorVisibility = error;
			group.OptionInformations[i].ErrorText = error;

			var useless = new StaticText
			{
				Parent = frame,
				ContentAlignment = ContentAlignment.MiddleLeft,
				PreferredWidth = DocumentCategoryController.errorBulletWidth,
				PreferredHeight = DocumentCategoryController.lineHeight,
				Dock = DockStyle.Left,
			};

			group.OptionInformations[i].UselessVisibility = useless;
			group.OptionInformations[i].UselessText = useless;

			int index = this.optionGroups.IndexOf (group);

			AbstractButton button;

			if (isRadio)
			{
				FormattedText text;

				if (i == 0)
				{
					text = "Aucune option";
				}
				else
				{
					var entity = group.OptionInformations[i].Entity;
					text = entity.Name;
				}

				button = new RadioButton
				{
					Parent = frame,
					FormattedText = text,
					Name = string.Concat (index.ToString (), ".", i.ToString ()),
					AutoToggle = false,
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Fill,
				};
			}
			else
			{
				button = new CheckButton
				{
					Parent = frame,
					FormattedText = group.OptionInformations[i].Entity.Name,
					Name = index.ToString (),
					AutoToggle = false,
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Fill,
				};
			}

			group.OptionInformations[i].Button = button;

			button.Clicked += delegate
			{
				this.ButtonClicked (button);
			};

			//	Bouton '>' pour voir les détails.
			if (isRadio && i == 0)
			{
				new FrameBox
				{
					Parent = frame,
					PreferredWidth = DocumentCategoryController.lineHeight,
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Right,
				};
			}
			else
			{
				var detailButton = new GlyphButton
				{
					Parent = frame,
					GlyphShape = GlyphShape.TriangleRight,
					ButtonStyle = ButtonStyle.ToolItem,
					Name = i.ToString (),
					PreferredWidth = DocumentCategoryController.lineHeight,
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Right,
				};

				ToolTip.Default.SetToolTip (detailButton, "Montre le détail");

				this.detailFrames.Add (frame);
				this.detailButtons.Add (detailButton);
				this.detailEntities.Add (group.OptionInformations[i].Entity);

				detailButton.Clicked += delegate
				{
					this.DetailButtonClicked (detailButton, group);
				};
			}

			//	Priorité 1..n.
			{
				var priorityFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = 10,
					PreferredHeight = DocumentCategoryController.lineHeight,
					Dock = DockStyle.Right,
					Margins = new Margins (0, -1, 0, 0),
				};

				group.OptionInformations[i].PriorityWidget = this.CreatePriorityWidget (priorityFrame, index, i);
			}

			//	Ratio "46/51" par exemple.
			var ratio = new StaticText
			{
				Parent = frame,
				Text = group.Ratio (isRadio ? (i == 0) : false),
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth = DocumentOptionsController.ratioWidth,
				PreferredHeight = DocumentCategoryController.lineHeight,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 3, 0, 0),
			};
		}

		private FrameBox CreatePriorityWidget(Widget parent, int index, int i)
		{
			//	Crée le widget permettant de visualiser et de modifier la priorité.
			var frame = new FrameBox
			{
				Parent = parent,
				Name = string.Concat (index.ToString (), ".", i.ToString ()),
				DrawFullFrame = true,
				PreferredHeight = DocumentCategoryController.lineHeight,
				Dock = DockStyle.Fill,
			};

			ToolTip.Default.SetToolTip (frame, "Priorité des options");

			var valueField = new StaticText
			{
				Parent = frame,
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
			};

			frame.Clicked += delegate
			{
				this.PriorityClicked (frame);
			};

			return frame;
		}

		private void SetPriorityValue(FrameBox priorityWidget, int value)
		{
			//	Initialise la valeur de la priorité dans le widget ad-hoc.
			if (value == -1)
			{
				priorityWidget.Visibility = false;
			}
			else
			{
				priorityWidget.Visibility = true;

				int count = this.documentCategory.DocumentOptions.Count;
				int display = count-value;  // 1..n inversé

				var valueField = priorityWidget.Children.OfType<StaticText> ().FirstOrDefault ();
				valueField.Text = display.ToString ();
			}
		}

		private void PriorityClicked(FrameBox button)
		{
			//	Affiche le menu permettant de choisir la priorité.
			var parts = button.Name.Split ('.');
			int index = int.Parse (parts[0]);
			int group = int.Parse (parts[1]);

			var entity = this.optionGroups[index].OptionInformations[group].Entity;

			int count = this.documentCategory.DocumentOptions.Count;
			index = this.documentCategory.DocumentOptions.IndexOf (entity);

			if (count > 1)  // au moins 2 choix ?
			{
				var menu = new VMenu ();

				for (int i = count-1; i >= 0; i--)
				{
					string icon = Misc.IconProvider.GetResourceIconUri ((i == index) ? "Button.RadioYes" : "Button.RadioNo");
					string text = string.Format ("Priorité {0}", (count-i).ToString ());

					if (i == count-1)
					{
						text = string.Concat (text, " (la plus forte)");
					}

					if (i == 0)
					{
						text = string.Concat (text, " (la plus faible)");
					}

					var item = new MenuItem ("", icon, text, null);
					item.TabIndex = i;
					menu.Items.Add (item);

					item.Clicked += delegate
					{
						this.PriorityChange (index, item.TabIndex);
					};
				}

				TextFieldCombo.AdjustComboSize (button, menu, false);
				var pos = button.MapClientToScreen (new Point (button.ActualWidth-1, button.ActualHeight));
				menu.ShowAsContextMenu (button, pos);
			}
		}

		private void PriorityChange(int currentIndex, int newIndex)
		{
			//	Modifie le priorité d'une collection d'options.
			var entity = this.documentCategory.DocumentOptions.ElementAt (currentIndex);

			this.documentCategory.DocumentOptions.RemoveAt (currentIndex);
			this.documentCategory.DocumentOptions.Insert (newIndex, entity);

			this.UpdateButtonStates ();
			this.documentCategoryController.UpdateAfterOptionChanged ();
		}

		private void ButtonClicked(AbstractButton button)
		{
			//	Appelé lorsque le bouton check ou radio d'une collection d'options est cliqué.
			var parts = button.Name.Split ('.');

			if (parts.Length == 1 && button is CheckButton)
			{
				int index = int.Parse (parts[0]);
				var entity = this.optionGroups[index].OptionInformations[0].Entity;

				if (this.documentCategory.DocumentOptions.Contains (entity))
				{
					this.documentCategory.DocumentOptions.Remove (entity);
				}
				else
				{
					this.documentCategory.DocumentOptions.Add (entity);
				}
			}

			if (parts.Length == 2 && button is RadioButton)
			{
				int index = int.Parse (parts[0]);
				int group = int.Parse (parts[1]);

				for (int i = 0; i < this.optionGroups[index].OptionInformations.Count; i++)
				{
					var entity = this.optionGroups[index].OptionInformations[i].Entity;

					if (this.documentCategory.DocumentOptions.Contains (entity))
					{
						this.documentCategory.DocumentOptions.Remove (entity);
					}
				}

				if (group != 0)
				{
					var entity = this.optionGroups[index].OptionInformations[group].Entity;
					this.documentCategory.DocumentOptions.Add (entity);
				}
			}

			this.selectedOptionInformation = null;
			this.selectedSpecialDetail = null;

			this.UpdateButtonStates ();
			this.UpdateDetailButtons ();
			this.UpdateErrorAndWarning ();
			this.documentCategoryController.UpdateAfterOptionChanged ();
		}

		private void DetailButtonClicked(AbstractButton detailButton, OptionGroup group)
		{
			//	Appelé lorsque le bouton '>' pour les détails d'une collection d'options est cliqué.
			int i = int.Parse (detailButton.Name);

			if (this.selectedOptionInformation == group.OptionInformations[i])
			{
				this.selectedOptionInformation = null;
			}
			else
			{
				this.selectedOptionInformation = group.OptionInformations[i];
			}

			this.selectedSpecialDetail = null;

			this.UpdateDetailButtons ();
		}


		private List<FormattedText> GetMissingDetailDescription(FormattedText fix, List<DocumentOption> usedOptions)
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

			return this.GetDetailDescription (fix, list, DocumentCategoryController.missingColor);
		}

		private List<FormattedText> GetDetailDescription(FormattedText fix, List<DocumentOption> options, Color color)
		{
			//	Retourne les lignes décrivant des options, sans les valeurs.
			var result = new List<FormattedText> ();
			result.Add (fix);
			result.Add (null);  // trait de séparation

			foreach (var verboseOption in this.verboseDocumentOptions)
			{
				if (options.Contains (verboseOption.Option))
				{
					var description = FormattedText.Concat (verboseOption.ShortDescription);
					result.Add (description.ApplyFontColor (color));
				}
			}

			return result;
		}


		private List<FormattedText> GetDetailDescription(OptionInformation info)
		{
			//	Retourne les lignes décrivant une OptionInformation, avec les valeurs.
			//	Cherche s'il faut utiliser les puces.
			var result = new List<FormattedText> ();

			result.Add (info.Entity.Name.ApplyFontSize (15));
			result.Add (null);  // trait de séparation
			result.Add ("Résumé :");

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
			var list = new List<FormattedText> ();

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

					list.Add (description);
				}
			}

			//	Génère les textes.
			if (correctCount != 0)
			{
				FormattedText correctText = "";

				if (correctCount == 1)
				{
					correctText = "Une option est définie correctement";
				}
				else
				{
					correctText = string.Format ("{0} options sont définies correctement", correctCount.ToString ());
				}

				if (hasBullet)
				{
					correctText = FormattedText.Concat (DocumentCategoryController.normalBullet, "  ", correctText);
				}

				result.Add (correctText);
			}

			if (errorCount != 0)
			{
				FormattedText errorText   = "";

				if (errorCount == 1)
				{
					errorText = "Une option est définie plusieurs fois";
				}
				else
				{
					errorText = string.Format ("{0} options sont définies plusieurs fois", errorCount.ToString ());
				}

				if (hasBullet)
				{
					errorText = FormattedText.Concat (DocumentCategoryController.errorBullet, "  ", errorText);
				}

				result.Add (errorText.ApplyFontColor (DocumentCategoryController.errorColor));
			}

			if (uselessCount != 0)
			{
				FormattedText uselessText = "";

				if (uselessCount == 1)
				{
					uselessText = "Une option est définie inutilement";
				}
				else
				{
					uselessText = string.Format ("{0} options sont définies inutilement", uselessCount.ToString ());
				}

				if (hasBullet)
				{
					uselessText = FormattedText.Concat (DocumentCategoryController.uselessBullet, "  ", uselessText);
				}

				result.Add (uselessText.ApplyFontColor (DocumentCategoryController.uselessColor));
			}

			//	Génère le résultat final.
			result.Add (null);  // trait de séparation
			result.Add ("Options :");
			result.AddRange (list);

			return result;
		}


		private void UpdateButtonStates()
		{
			//	Met à jour les états ActiveState des boutons à cocher et des boutons radio.
			foreach (var group in this.optionGroups)
			{
				if (group.IsRadio)
				{
					var firstCheck = true;

					for (int i = 1; i < group.OptionInformations.Count; i++)
					{
						var entity = group.OptionInformations[i].Entity;
						bool check = this.documentCategory.DocumentOptions.Contains (entity);

						if (check)
						{
							firstCheck = false;
						}
					}

					for (int i = 0; i < group.OptionInformations.Count; i++)
					{
						int priority;
						bool check;

						if (i == 0)
						{
							priority = -1;
							check = firstCheck;
						}
						else
						{
							var entity = group.OptionInformations[i].Entity;
							priority = this.documentCategory.DocumentOptions.IndexOf (entity);
							check = (priority != -1);
						}

						group.OptionInformations[i].Button.ActiveState = check ? ActiveState.Yes : ActiveState.No;
						this.SetPriorityValue (group.OptionInformations[i].PriorityWidget, priority);
					}
				}
				else
				{
					var entity = group.OptionInformations[0].Entity;
					int priority = this.documentCategory.DocumentOptions.IndexOf (entity);
					bool check = (priority != -1);

					group.OptionInformations[0].Button.ActiveState = check ? ActiveState.Yes : ActiveState.No;
					this.SetPriorityValue (group.OptionInformations[0].PriorityWidget, priority);
				}
			}
		}

		private void UpdateErrorAndWarning()
		{
			this.errorOptions.Clear ();

			var usedOptions    = new List<DocumentOption> ();
			var uselessOptions = new List<DocumentOption> ();

			int error   = 0;
			int missing = 0;
			int useless = 0;

			foreach (var documentOptionEntity in this.documentCategory.DocumentOptions)
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

			foreach (var info in this.optionInformations.Union (this.additionnalOptionInformations))
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

						if (text.IsNullOrEmpty ())
						{
							ToolTip.Default.HideToolTipForWidget (info.ErrorText);
						}
						else
						{
							ToolTip.Default.SetToolTip (info.ErrorText, "Contient des options définies plusieurs fois dont les valeurs dépendent des priorités");
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

						if (this.documentCategory.DocumentOptions.Contains (info.Entity))
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

						if (text.IsNullOrEmpty ())
						{
							ToolTip.Default.HideToolTipForWidget (info.UselessText);
						}
						else
						{
							ToolTip.Default.SetToolTip (info.UselessText, "Contient des options définies inutilement");
						}
					}
				}
			}

			FormattedText errorMessage   = null;
			FormattedText uselessMessage = null;
			FormattedText missingMessage = null;

			this.errorDetails   = null;
			this.missingDetails = null;
			this.uselessDetails = null;

			if (error != 0)
			{
				var title = new FormattedText ("Attention<br/>").ApplyFontSize (15);
				var details = "<br/>Les priorités sont affichées dans des petits rectangles, sous forme de chiffres compris entre 1 et 9. La valeur la plus petite correspond à la priorité la plus forte.";

				if (error == 1)
				{
					errorMessage = "Il y a une option définie plusieurs fois";
					this.errorDetails = this.GetDetailDescription (title+"L'option suivante est définie plusieurs fois et a une valeur qui dépend des priorités."+details, this.errorOptions, DocumentCategoryController.errorColor);
				}
				else
				{
					errorMessage = string.Format ("Il y a {0} options définies plusieurs fois", error.ToString ());
					this.errorDetails = this.GetDetailDescription (title+"Les options suivantes sont définies plusieurs fois et ont des valeurs qui dépendent des priorités."+details, this.errorOptions, DocumentCategoryController.errorColor);
				}

				errorMessage = FormattedText.Concat (DocumentCategoryController.errorBullet, "  ", errorMessage);
			}

			if (missing != 0)
			{
				var title = new FormattedText ("Information<br/>").ApplyFontSize (15);

				if (missing == 1)
				{
					missingMessage = "Il y a une option indéfinie";
					this.missingDetails = this.GetMissingDetailDescription (title+"L'option suivante est indéfinie et prend la valeur par défaut.", usedOptions);
				}
				else
				{
					missingMessage = string.Format ("Il y a {0} options indéfinies", missing.ToString ());
					this.missingDetails = this.GetMissingDetailDescription (title+"Les options suivantes sont indéfinies et prennent les valeurs par défaut.", usedOptions);
				}

				missingMessage = FormattedText.Concat (DocumentCategoryController.missingBullet, "  ", missingMessage);
			}

			if (useless != 0)
			{
				var title = new FormattedText ("Avertissement<br/>").ApplyFontSize (15);

				if (useless == 1)
				{
					uselessMessage = "Il y a une option définie inutilement";
					this.uselessDetails = this.GetDetailDescription (title+"L'options suivante est définie inutilement.", uselessOptions, DocumentCategoryController.uselessColor);
				}
				else
				{
					uselessMessage = string.Format ("Il y a {0} options définies inutilement", useless.ToString ());
					this.uselessDetails = this.GetDetailDescription (title+"Les options suivantes sont définies inutilement.", uselessOptions, DocumentCategoryController.uselessColor);
				}

				uselessMessage = FormattedText.Concat (DocumentCategoryController.uselessBullet, "  ", uselessMessage);
			}

			if (this.documentCategory.DocumentOptions.Count == 0)
			{
				missingMessage = "Aucune option d'impression n'est choisie";
			}

			if (errorMessage.IsNullOrEmpty ())
			{
				this.errorFrame.Visibility = false;
			}
			else
			{
				this.errorFrame.Visibility = true;
				this.errorText.FormattedText = errorMessage;
			}

			if (uselessMessage.IsNullOrEmpty ())
			{
				this.uselessFrame.Visibility = false;
			}
			else
			{
				this.uselessFrame.Visibility = true;
				this.uselessText.FormattedText = uselessMessage;
			}

			if (missingMessage.IsNullOrEmpty ())
			{
				this.missingFrame.Visibility = false;
			}
			else
			{
				this.missingFrame.Visibility = true;
				this.missingText.FormattedText = missingMessage;
			}
		}


		private void UpdateDetailButtons()
		{
			//	Met à jour la couleur de fond des boutons.
			for (int i = 0; i < this.detailFrames.Count; i++)
			{
				if (this.selectedOptionInformation != null && this.detailEntities[i] == this.selectedOptionInformation.Entity)
				{
					this.detailFrames[i].BackColor = Widgets.Tiles.TileColors.SurfaceSelectedContainerColors.First ();
				}
				else
				{
					this.detailFrames[i].BackColor = Color.Empty;
				}
			}

			//	Met à jour le résumé dans la partie centrale (SummaryController).
			this.documentCategoryController.SummaryController.DetailTexts.Clear ();

			this.errorFrame.BackColor   = (this.selectedSpecialDetail == "error"  ) ? Widgets.Tiles.TileColors.SurfaceSelectedContainerColors.First () : Color.Empty;
			this.uselessFrame.BackColor = (this.selectedSpecialDetail == "useless") ? Widgets.Tiles.TileColors.SurfaceSelectedContainerColors.First () : Color.Empty;
			this.missingFrame.BackColor = (this.selectedSpecialDetail == "missing") ? Widgets.Tiles.TileColors.SurfaceSelectedContainerColors.First () : Color.Empty;

			if (this.selectedOptionInformation != null)
			{
				this.documentCategoryController.SummaryController.DetailTexts.AddRange (this.GetDetailDescription (this.selectedOptionInformation));
			}
			else if (this.selectedSpecialDetail == "error" && this.errorDetails != null)
			{
				this.documentCategoryController.SummaryController.DetailTexts.AddRange (this.errorDetails);
			}
			else if (this.selectedSpecialDetail == "useless" && this.uselessDetails != null)
			{
				this.documentCategoryController.SummaryController.DetailTexts.AddRange (this.uselessDetails);
			}
			else if (this.selectedSpecialDetail == "missing" && this.missingDetails != null)
			{
				this.documentCategoryController.SummaryController.DetailTexts.AddRange (this.missingDetails);
			}

			this.documentCategoryController.SummaryController.UpdateAfterOptionChanged ();
		}


		private void UpdateData()
		{
			this.UpdateOptionInformations ();
			this.UpdateOptionGroup ();
		}

		private void UpdateOptionGroup()
		{
			this.optionGroups.Clear ();
			this.additionnalOptionInformations.Clear ();

			foreach (var info in this.optionInformations)
			{
				if (!info.IsRadio)
				{
					var group = new OptionGroup (info);

					foreach (var friend in this.optionInformations)
					{
						if (friend != info && !friend.IsRadio && friend.HasSameOptions (info))
						{
							if (group.OptionInformations.Count == 1)
							{
								var dummy = new OptionInformation (info);
								dummy.IsDummy = true;
								group.OptionInformations.Insert (0, dummy);  // pour le bouton "aucune option"

								this.additionnalOptionInformations.Add (dummy);
							}

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
			this.requiredDocumentOptions = EntityPrinterFactoryResolver.FindRequiredDocumentOptions (this.documentCategory.DocumentType);
			
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

			public string Ratio (bool isDummy)
			{
				if (isDummy)
				{
					return string.Concat ("0/", this.Total.ToString ());
				}
				else
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

			public OptionInformation(OptionInformation model)
			{
				this.Entity                   = model.Entity;
				this.PrintingOptionDictionary = model.PrintingOptionDictionary;
				this.options                  = model.Options;
				this.Used                     = model.Used;
				this.Total                    = model.Total;
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

			public bool IsDummy
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

			public FrameBox PriorityWidget
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

		private readonly DocumentCategoryController			documentCategoryController;
		private readonly BusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategory;
		private readonly IEnumerable<VerboseDocumentOption>	verboseDocumentOptions;
		private readonly List<OptionInformation>			optionInformations;
		private readonly List<OptionInformation>			additionnalOptionInformations;
		private readonly List<OptionGroup>					optionGroups;
		private readonly List<DocumentOption>				errorOptions;
		private readonly List<FrameBox>						detailFrames;
		private readonly List<GlyphButton>					detailButtons;
		private readonly List<DocumentOptionsEntity>		detailEntities;

		private double										width;
		private Scrollable									checkButtonsFrame;
		private bool										firstGroup;
		private char										lastButton;
		private IEnumerable<DocumentOption>					requiredDocumentOptions;
		private FrameBox									errorFrame;
		private StaticText									errorText;
		private List<FormattedText>							errorDetails;
		private FrameBox									uselessFrame;
		private StaticText									uselessText;
		private List<FormattedText>							uselessDetails;
		private FrameBox									missingFrame;
		private StaticText									missingText;
		private List<FormattedText>							missingDetails;
		private OptionInformation							selectedOptionInformation;
		private string										selectedSpecialDetail;
	}
}
