//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// Assembleur CALM selon le principe bien connu des deux passes.
	/// La 1ère passe récolte les définitions de symboles et d'étiquettes dans un dictionnaire.
	/// La 2ème passe génère le code.
	/// </summary>
	public class Assembler
	{
		public Assembler(Components.AbstractProcessor processor, Components.Memory memory)
		{
			this.processor = processor;
			this.memory = memory;
		}

		public void Action(TextFieldMulti field, Window window)
		{
			//	Assemble les instructions CALM du programme.
			this.field = field;
			this.window = window;

			this.memory.ClearRam();

			this.RemoveErrors();

			string[] seps = {"<br/>"};
			string[] lines = this.field.Text.Split(seps, System.StringSplitOptions.None);

			int instructionCounter = 0;
			int byteCounter = 0;
			List<int> errorLines = new List<int>();
			List<string> errorTexts = new List<string>();

			Dictionary<string, int> symbols = new Dictionary<string, int>();
			this.processor.RomSymbols(Components.Memory.RomBase, symbols);

			for (int pass=0; pass<2; pass++)
			{
				this.DoPass(lines, pass, symbols, errorLines, errorTexts, ref instructionCounter, ref byteCounter);

				if (errorLines.Count != 0)
				{
					break;
				}
			}

			string icon, message;
			if (errorLines.Count == 0)
			{
				if (instructionCounter == 0)
				{
					icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
					message = "<b>Le programme ne contient aucune instruction.</b>";
				}
				else
				{
					icon = "manifest:Epsitec.Common.Dialogs.Images.Information.icon";
					message = string.Format("<b>Assemblage correct de {0} instruction(s) pour un total de {1} octet(s).</b><br/>Cliquez le bouton [R/S] pour exécuter le programme.", instructionCounter.ToString(), byteCounter.ToString());
				}
			}
			else
			{
				this.InsertErrors(errorLines, errorTexts);

				icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
				message = string.Format("<b>Il y a eu {0} erreur(s) lors de l'assemblage.</b>", errorLines.Count.ToString());
			}

			string title = "Dauphin";
			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, message, null, null);
			dialog.Owner = this.window;
			dialog.OpenDialog();

			this.field.Cursor = 0;  // remet le curseur au début pour éviter les débordements dans TextLayout !
		}


		protected void DoPass(string[] lines, int pass, Dictionary<string, int> symbols, List<int> errorLines, List<string> errorTexts, ref int instructionCounter, ref int byteCounter)
		{
			//	Première ou deuxième passe de l'assemblage.
			int pc = Components.Memory.RamBase;
			int lineCounter = 0;

			foreach (string line in lines)
			{
				string instruction = this.RemoveComment(line);

				string err;
				instruction = this.GetSymbol(instruction, pass, pc, symbols, out err);
				if (err == null)
				{
					if (!string.IsNullOrEmpty(instruction))
					{
						instruction = this.processor.AssemblyPreprocess(instruction);
						instruction = this.GetInstruction(instruction, pass, pc, symbols, out err);
						if (err == null)
						{
							if (!string.IsNullOrEmpty(instruction))
							{
								List<int> codes = new List<int>();
								err = this.processor.AssemblyInstruction(instruction, codes);

								if (codes.Count == 0)
								{
									if (pass == 1)  // deuxième passe ?
									{
										if (!string.IsNullOrEmpty(err))
										{
											errorLines.Add(lineCounter);
											errorTexts.Add(Misc.RemoveTags(Misc.FirstLine(err)));
										}
									}
								}
								else
								{
									if (pass == 0)  // première passe ?
									{
										pc += codes.Count;
									}
									else  // deuxième passe ?
									{
										foreach (int code in codes)
										{
											this.memory.Write(pc++, code);
											byteCounter++;
										}
										instructionCounter++;
									}
								}
							}
						}
						else
						{
							errorLines.Add(lineCounter);
							errorTexts.Add(Misc.RemoveTags(err));
						}
					}
				}
				else
				{
					errorLines.Add(lineCounter);
					errorTexts.Add(Misc.RemoveTags(err));
				}
				
				lineCounter++;
			}
		}

		protected string RemoveComment(string instruction)
		{
			//	Supprime l'éventuel commentaire "; blabla" en fin de ligne.
			instruction = instruction.ToUpper().Trim();
			int index = instruction.IndexOf(";");  // commentaire à la fin de la ligne ?
			if (index != -1)
			{
				instruction = instruction.Substring(0, index);  // enlève le commentaire
			}
			return instruction;
		}

		protected string GetSymbol(string instruction, int pass, int pc, Dictionary<string, int> symbols, out string err)
		{
			//	Prépare une instruction pour l'assemblage.
			//	Gère les définitions de symboles, du genre "TOTO = 12*TITI".
			string[] defs = instruction.Split('=');
			if (defs.Length == 2)  // symbol = value ?
			{
				if (pass == 0)  // première passe ?
				{
					string symbol = defs[0].Trim();

					if (this.IsRegister(symbol))
					{
						err = "Il n'est pas possible d'utiliser un nom de registre.";
					}
					else if (!this.IsSymbol(symbol))
					{
						err = "Nom de symbol incorrect.";
					}
					else if (symbols.ContainsKey(symbol))
					{
						err = "Symbol déjà défini.";
					}
					else
					{
						int value = this.Expression(defs[1].Trim(), pass, pc, symbols, out err);
						if (err == null)
						{
							symbols.Add(symbol, value);
						}
					}
				}
				else  // deuxième passe ?
				{
					err = null;
				}
				return null;  // ce n'est pas une instruction
			}

			//	Gère les définitions d'étiquettes, du genre "LOOP: MOVE A,B".
			int index = instruction.IndexOf(":");
			if (index != -1)
			{
				string label = instruction.Substring(0, index).Trim();
				if (!string.IsNullOrEmpty(label) && label.IndexOf(" ") == -1)
				{
					if (pass == 0)  // première passe ?
					{
						if (this.IsRegister(label))
						{
							err = "Il n'est pas possible d'utiliser un nom de registre.";
							return null;
						}
						else if (!this.IsSymbol(label))
						{
							err = "Nom d'étiquette incorrect.";
							return null;
						}
						else if (symbols.ContainsKey(label))
						{
							err = "Etiquette déjà définie.";
							return null;
						}
						else
						{
							symbols.Add(label, pc);
						}
					}

					instruction = instruction.Substring(index+1);  // enlève le "label:" au début
				}
			}

			err = null;
			return instruction;
		}

		protected string GetInstruction(string instruction, int pass, int pc, Dictionary<string, int> symbols, out string err)
		{
			//	Prépare une instruction pour l'assemblage.
			//	Gère les définitions de symboles, du genre "TOTO = 12*TITI".
			//	Effectue les substitutions dans les arguments.
			string[] seps = {" "};
			string[] words = instruction.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);

			if (words.Length == 0)
			{
				err = null;
				return null;
			}

			if (words.Length >= 2)
			{
				for (int i=1; i<words.Length; i++)  // passe en revue les arguments
				{
					string word = words[i];
					if (string.IsNullOrEmpty(word))
					{
						continue;
					}

					if (this.IsRegister(word))  // r ?
					{
						//	Aucune substitution à effectuer s'il s'agit d'un registre.
					}
					else if (word.Length >= 1 && word[0] == '#')  // #val ?
					{
						int value = this.Expression(word.Substring(1), pass, pc, symbols, out err);
						if (err == null)
						{
							word = string.Concat("#", value.ToString("X3"), "H");
						}
						else
						{
							return null;
						}
					}
					else  // ADDR ?
					{
						int value = this.Expression(word, pass, pc, symbols, out err);
						if (err == null)
						{
							word = string.Concat(value.ToString("X3"), "H");
						}
						else
						{
							return null;
						}
					}

					words[i] = word;
				}
			}

			err = null;  // ok

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			foreach (string word in words)
			{
				if (!string.IsNullOrEmpty(word))
				{
					builder.Append(word);
					builder.Append(" ");
				}
			}
			return builder.ToString();
		}

		protected int Expression(string expression, int pass, int pc, Dictionary<string, int> symbols, out string err)
		{
			//	Evalue une expression.
			err = null;

			int value = 0;
			int index = 0;
			while (index < expression.Length)
			{
				int n = this.SkipNumber(expression, ref index, out err);
				if (err != null)
				{
					string symbol = this.SkipSymbol(expression, ref index, out err);
					if (err != null)
					{
						err = "Expression incorrecte.";
						return -1;
					}

					if (symbols.ContainsKey(symbol))
					{
						n = symbols[symbol];
					}
					else
					{
						if (pass == 0)  // première passe ?
						{
							n = 0;  // valeur quelconque, juste pour assembler une instruction avec le bon nombre de bytes
						}
						else  // deuxième passe ?
						{
							err = "Symbole indéfini.";
							return -1;
						}
					}
				}

				value = n;
			}

			return value;
		}

		protected int SkipNumber(string expression, ref int index, out string err)
		{
			int value;

			int i = index;
			while (i < expression.Length)
			{
				char c = expression[i];
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))
				{
					i++;
				}
				else
				{
					break;
				}
			}

			string sub = expression.Substring(index, i-index);

			char n = 'D';
			if (i < expression.Length)
			{
				n = expression[i++];
			}

			if (n == 'D')  // décimal ?
			{
				if (!int.TryParse(expression, out value))
				{
					err = "Valeur incorecte.";
					return -1;
				}
			}
			else if (n == 'H')  // héxadécimal ?
			{
				value = Misc.ParseHexa(sub, -1, -1);
				if (value == -1)
				{
					err = "Valeur incorecte.";
					return -1;
				}
			}
			else
			{
				err = "Base incorrecte.";
				return -1;
			}

			index = i;
			err = null;
			return value;
		}

		protected string SkipSymbol(string expression, ref int index, out string err)
		{
			//	Indique si le mot correspond à un nom de symbole.
			int i = index;
			while (i < expression.Length)
			{
				char c = expression[i];

				if (c >= '0' && c <= '9')
				{
					if (i == index)
					{
						err = "Nom incorrect.";
						return null;
					}
					i++;
				}
				else if (c >= 'A' && c <= 'Z')
				{
					i++;
				}
				else if (c == '_')
				{
					i++;
				}
				else
				{
					break;
				}
			}

			string sub = expression.Substring(index, i-index);
			index = i;
			err = null;
			return sub;
		}

		protected bool IsRegister(string word)
		{
			//	Indique si le mot correspond à un nom de registre du processeur.
			foreach (string register in this.processor.RegisterNames)
			{
				if (word == register)
				{
					return true;
				}
			}
			return false;
		}

		protected bool IsSymbol(string word)
		{
			//	Indique si le mot correspond à un nom de symbole valide.
			if (word.Length == 0)
			{
				return false;
			}

			if (word[0] >= '0' && word[0] <= '9')
			{
				return false;
			}

			foreach (char c in word)
			{
				if (word[0] >= '0' && word[0] <= '9')
				{
					continue;
				}
				else if (word[0] >= 'A' && word[0] <= 'Z')
				{
					continue;
				}
				else if (word[0] == '_')
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			return true;
		}


		protected void RemoveErrors()
		{
			//	Supprime toutes les erreurs.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			string programm = this.field.Text;

			string[] seps = {"<br/>"};
			string[] lines = programm.Split(seps, System.StringSplitOptions.None);

			int total = lines.Length;
			for (int i=lines.Length-1; i>=0; i--)
			{
				if (string.IsNullOrEmpty(lines[i]))
				{
					total--;
				}
				else
				{
					break;
				}
			}

			for (int i=0; i<total; i++)
			{
				string line = lines[i];

				string simply = Misc.RemoveTags(line).Trim();
				if (!simply.StartsWith("^ "))
				{
					builder.Append(line);
					builder.Append("<br/>");
				}
			}

			this.field.Text = builder.ToString();
		}

		protected void InsertErrors(List<int> errorLines, List<string> errorTexts)
		{
			//	Insère les erreurs dans le texte du programme.
			string programm = this.field.Text;

			for (int i=errorLines.Count-1; i>=0; i--)
			{
				int index1 = Misc.LineIndex(programm, errorLines[i]+0);
				int index2 = Misc.LineIndex(programm, errorLines[i]+1);

				string balast = Misc.Balast(programm.Substring(index1));
				string err = string.Concat(balast, "^ ", errorTexts[i], "<br/>");
				programm = programm.Insert(index2, err);
			}

			this.field.Text = programm;
		}


		protected Components.AbstractProcessor processor;
		protected Components.Memory memory;
		protected Window window;
		protected TextFieldMulti field;
	}
}