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
				string instruction = this.GetInstruction(line, pass, pc, lineCounter, symbols, errorLines, errorTexts);
				if (instruction != null)
				{
					List<int> codes = new List<int>();
					string err = this.processor.AssemblyInstruction(instruction, codes);

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

				lineCounter++;
			}
		}

		protected string GetInstruction(string instruction, int pass, int pc, int lineCounter, Dictionary<string, int> symbols, List<int> errorLines, List<string> errorTexts)
		{
			//	Prépare une instruction pour l'assemblage.
			int index = instruction.IndexOf(";");  // commentaire à la fin de la ligne ?
			if (index != -1)
			{
				instruction = instruction.Substring(0, index);  // enlève le commentaire
			}

			instruction = instruction.ToUpper().Trim();

			//	Gère les définitions de symboles, du genre "TOTO = 12*TITI".
			string[] defs = instruction.Split('=');
			if (defs.Length == 2)  // symbol = value ?
			{
				if (pass == 0)  // première passe ?
				{
					string symbol = defs[0].Trim();
					if (this.IsRegister(symbol))
					{
						errorLines.Add(lineCounter);
						errorTexts.Add("Il n'est pas possible d'utiliser un nom de registre.");
					}
					else
					{
						int value = Misc.ParseHexa(defs[1].Trim());  // TODO: évaluer l'expression
						if (symbols.ContainsKey(symbol))
						{
							errorLines.Add(lineCounter);
							errorTexts.Add("Symbol déjà défini.");
						}
						else
						{
							symbols.Add(symbol, value);
						}
					}
				}
				return null;  // ce n'est pas une instruction
			}

			//	Gère les définitions d'étiquettes, du genre "LOOP: MOVE A,B".
			index = instruction.IndexOf(":");
			if (index != -1)
			{
				string[] labels = instruction.Split(':');
				if (labels[0].IndexOf(' ') == -1)  // label en début de ligne (et pas "TSET A:#2") ?
				{
					if (pass == 0)  // première passe ?
					{
						if (this.IsRegister(labels[0]))
						{
							errorLines.Add(lineCounter);
							errorTexts.Add("Il n'est pas possible d'utiliser un nom de registre.");
						}
						else
						{
							if (symbols.ContainsKey(labels[0]))
							{
								errorLines.Add(lineCounter);
								errorTexts.Add("Etiquette déjà définie.");
								return null;
							}
							else
							{
								symbols.Add(labels[0], pc);
							}
						}
					}

					instruction = instruction.Substring(index+1);  // enlève le label: au début
				}
			}

			//	Effectue les substitutions dans les arguments.
			string[] seps = { " ", ",", ":", "<TAB/>" };
			string[] words = instruction.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);

			if (words.Length == 0)
			{
				return null;
			}

			if (words.Length == 3 && instruction.IndexOf(':') != -1)  // instruction du type "TSET A:#2" ?
			{
				string t = words[1];
				words[1] = words[2];  // permute ("TSET A:#2" devient "TSET #2 A")
				words[2] = t;
			}

			if (words.Length >= 2)
			{
				for (int i=1; i<words.Length; i++)  // passe en revue les arguments
				{
					string word = words[i];

					if (this.IsRegister(word))  // r ?
					{
						//	Aucune substitution à effectuer s'il s'agit d'un registre.
					}
					else if (word.Length >= 1 && word[0] == '#')  // #val ?
					{
						word = word.Substring(1);
						word = this.WordSubstitute(word, pass, symbols);
						word = string.Concat("#", word);
					}
					else  // ADDR ?
					{
						word = this.WordSubstitute(word, pass, symbols);
					}

					words[i] = word;
				}
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			foreach (string word in words)
			{
				builder.Append(word);
				builder.Append(" ");
			}
			return builder.ToString();
		}

		protected string WordSubstitute(string word, int pass, Dictionary<string, int> symbols)
		{
			if (pass == 0)  // première passe ?
			{
				word = "0";  // valeur quelconque, juste pour assembler une instruction avec le bon nombre de bytes
			}
			else  // deuxième passe ?
			{
				if (symbols.ContainsKey(word))
				{
					word = string.Concat(symbols[word].ToString("X3"), "h");
				}
			}

			return word;
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