//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	/// La 1�re passe r�colte les d�finitions de variables et d'�tiquettes dans un dictionnaire.
	/// La 2�me passe g�n�re le code.
	/// </summary>
	public class Assembler
	{
		public Assembler(Components.AbstractProcessor processor, Components.Memory memory)
		{
			this.processor = processor;
			this.memory = memory;
		}

		public bool Action(TextFieldMulti field, Window window, bool verbose)
		{
			//	Assemble les instructions CALM du programme.
			this.field = field;
			this.window = window;

			this.memory.ClearRam();

			//	Enl�ve les �ventuelles erreurs pr�c�dentes.
			this.RemoveErrors();

			string[] seps = {"<br/>"};
			string[] lines = this.field.Text.Split(seps, System.StringSplitOptions.None);

			int instructionCounter = 0;
			int byteCounter = 0;

			//	Ces 2 listes stockent toutes les erreurs rencontr�es.
			List<int> errorLines = new List<int>();
			List<string> errorTexts = new List<string>();

			//	Cr�e le dictionnaire principal pour les variables et �tiquettes.
			Dictionary<string, int> variables = new Dictionary<string, int>();
			this.processor.RomVariables(Components.Memory.RomBase, variables);
			this.memory.RomVariables(variables);

			//	Longueurs des instructions, construite lors de la 1�re passe et utilis�es � la 2�me.
			List<int> instructionLengths = new List<int>();

			//	Effectue les deux passes.
			for (int pass=0; pass<2; pass++)
			{
				this.DoPass(lines, pass, variables, instructionLengths, errorLines, errorTexts, ref instructionCounter, ref byteCounter);

				if (errorLines.Count != 0)
				{
					break;  // si erreur pendant la 1�re passe -> pas de 2�me
				}
			}

			//	Dialogue selon le d�roulement de l'assemblage.
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
					message = string.Format("<b>Assemblage correct de {0} instruction(s) pour un total de {1} octet(s).</b><br/>Cliquez le bouton [R/S] pour ex�cuter le programme.", instructionCounter.ToString(), byteCounter.ToString());
				}
			}
			else
			{
				this.InsertErrors(errorLines, errorTexts);

				icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
				message = string.Format("<b>Il y a eu {0} erreur(s) lors de l'assemblage.</b>", errorLines.Count.ToString());
			}

			if (verbose || errorLines.Count != 0)
			{
				string title = "Dauphin";
				Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, message, null, null);
				dialog.Owner = this.window;
				dialog.OpenDialog();
			}

			return errorLines.Count == 0;
		}


		protected void DoPass(string[] lines, int pass, Dictionary<string, int> variables, List<int> instructionLengths, List<int> errorLines, List<string> errorTexts, ref int instructionCounter, ref int byteCounter)
		{
			//	Premi�re ou deuxi�me passe de l'assemblage (pass = 0..1).
			//	La 1�re passe r�colte les d�finitions de variables et d'�tiquettes dans un dictionnaire.
			//	La 2�me passe g�n�re le code.
			int pc = Components.Memory.RamBase;  // on place toujours le code au d�but de la RAM
			int lineCounter = 0;
			this.ending = false;

			foreach (string line in lines)
			{
				string instruction = this.RemoveComment(line);
				string err;

				if (this.ProcessPseudo(instruction, pass, variables, ref pc, out err))
				{
					if (err == null)
					{
						if (this.ending)
						{
							break;
						}
					}
					else
					{
						errorLines.Add(lineCounter);
						errorTexts.Add(err);
					}
				}
				else
				{
					instruction = this.ProcessVariables(instruction, pass, pc, variables, out err);
					if (err == null)
					{
						if (!string.IsNullOrEmpty(instruction))
						{
							int npc = pc;  // npc est faux � la 1�re passe, mais c'est sans importance !
							if (pass == 1 && instructionCounter < instructionLengths.Count)  // deuxi�me passe ?
							{
								npc += instructionLengths[instructionCounter];  // npc = adresse apr�s l'instruction, pour adressage relatif
							}

							instruction = this.processor.AssemblyPreprocess(instruction);
							instruction = this.PrepareInstruction(instruction, pass, npc, variables, out err);
							if (err == null)
							{
								if (!string.IsNullOrEmpty(instruction))
								{
									List<int> codes = new List<int>();
									err = this.processor.AssemblyInstruction(instruction, codes);

									if (codes.Count == 0)
									{
										if (pass == 1)  // deuxi�me passe ?
										{
											if (!string.IsNullOrEmpty(err))
											{
												errorLines.Add(lineCounter);
												errorTexts.Add(Misc.RemoveTags(Assembler.FirstLine(err)));
											}
										}
									}
									else
									{
										if (pass == 0)  // premi�re passe ?
										{
											pc += codes.Count;
											instructionLengths.Add(codes.Count);
										}
										else  // deuxi�me passe ?
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
				}
				
				lineCounter++;
			}
		}

		protected string RemoveComment(string instruction)
		{
			//	Supprime l'�ventuel commentaire "; blabla" en fin de ligne.
			instruction = TextLayout.ConvertToSimpleText(instruction);
			instruction = instruction.ToUpper().Trim();
			int index = instruction.IndexOf(";");  // commentaire � la fin de la ligne ?
			if (index != -1)
			{
				instruction = instruction.Substring(0, index);  // enl�ve le commentaire
			}
			return instruction;
		}

		protected bool ProcessPseudo(string instruction, int pass, Dictionary<string, int> variables, ref int pc, out string err)
		{
			//	Traite les pseudos-instructions .TITLE, .LOC, etc.
			//	Retourne true si une pseudo a �t� trait�e.
			err = null;

			if (!instruction.StartsWith("."))
			{
				return false;
			}

			string[] seps = {" ", "\t"};
			string[] words = instruction.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);

			switch (words[0])
			{
				case ".TITLE":
					if (words.Length == 1)
					{
						err = "Il manque le texte du titre.";
					}
					break;

				case ".LOC":
					if (words.Length == 2)
					{
						int value = this.Expression(words[1], pass, true, variables, out err);
						if (err == null)
						{
							pc = value;
						}
					}
					else
					{
						err = ".LOC doit �tre suivi d'une adresse.";
					}
					break;

				case ".END":
					if (words.Length == 1)
					{
						this.ending = true;
					}
					else
					{
						err = "Il ne peut pas y avoir d'argument.";
					}
					break;

				default:
					err = "Pseudo-instruction inconnue.";
					break;
			}

			return true;
		}

		protected string ProcessVariables(string instruction, int pass, int pc, Dictionary<string, int> variables, out string err)
		{
			//	Traite les variables dans une instruction.
			//	Retourne l'instruction expurg�e des variables.
			//	G�re les d�finitions de variables du genre "TOTO = 12*TITI", retourne NULL.
			//	G�re les �tiquettes, du genre "LOOP: MOVE A,B", retourne "MOVE A,B".
			string[] defs = instruction.Split('=');
			if (defs.Length == 2)  // variable = expression ?
			{
				if (pass == 0)  // premi�re passe ?
				{
					string variable   = defs[0].Trim();
					string expression = defs[1].Trim();

					if (this.IsRegister(variable))
					{
						err = "Il n'est pas possible d'utiliser un nom de registre.";
					}
					else if (!this.IsVariable(variable))
					{
						err = "Nom de variable incorrect.";
					}
					else if (variables.ContainsKey(variable))
					{
						err = "Variable ou �tiquette d�j� d�finie.";
					}
					else
					{
						int value = this.Expression(expression, pass, false, variables, out err);
						if (err == null)
						{
							variables.Add(variable, value);
						}
					}
				}
				else  // deuxi�me passe ?
				{
					err = null;
				}
				return null;  // ce n'est pas une instruction
			}

			//	G�re les d�finitions d'�tiquettes, du genre "LOOP: MOVE A,B".
			int index = instruction.IndexOf(":");
			if (index != -1)
			{
				string label = instruction.Substring(0, index).Trim();
				if (!string.IsNullOrEmpty(label) && label.IndexOf(" ") == -1 && label.IndexOf("\t") == -1)
				{
					if (pass == 0)  // premi�re passe ?
					{
						if (this.IsRegister(label))
						{
							err = "Il n'est pas possible d'utiliser un nom de registre.";
							return null;
						}
						else if (!this.IsVariable(label))
						{
							err = "Nom d'�tiquette incorrect.";
							return null;
						}
						else if (variables.ContainsKey(label))
						{
							err = "Variable ou �tiquette d�j� d�finie.";
							return null;
						}
						else
						{
							variables.Add(label, pc);
						}
					}

					instruction = instruction.Substring(index+1);  // enl�ve le "label:" au d�but
				}
			}

			err = null;
			return instruction;
		}

		protected string PrepareInstruction(string instruction, int pass, int npc, Dictionary<string, int> variables, out string err)
		{
			//	Pr�pare une instruction pour l'assemblage.
			//	Effectue les substitutions dans les arguments en fonction des variables.
			//	Par exemple, remplace "MOVE #TOTO+1,A" par "MOVE H'12 A", si TOTO=H'11.
			//	Par exemple, remplace "MOVE {SP}+TITI,A" par "MOVE {SP}+H'02 A", si TITI=H'02.
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
						//	Aucune substitution � effectuer s'il s'agit d'un registre.
					}
					else if (word.Length >= 1 && word[0] == '#')  // #val ?
					{
						int value = this.Expression(word.Substring(1), pass, true, variables, out err);
						if (err == null)
						{
							word = string.Concat("#H'", value.ToString("X3"));
						}
						else
						{
							return null;
						}
					}
					else  // ADDR ?
					{
						string text;
						int mode;
						this.processor.AssemblySplitAddr(word, out text, out mode);

						if (text.StartsWith("R^"))  // adressage relatif ?
						{
							text = text.Substring(2);  // enl�ve le R^
							int value = this.Expression(text, pass, true, variables, out err);
							if (err == null)
							{
								value -= npc;

								if (System.Math.Abs(value) > 0x7FF)
								{
									err = "D�placement relatif trop grand.";
									return null;
								}

								text = string.Concat("{PC}+H'", value.ToString("X3"));
							}
							else
							{
								return null;
							}
						}
						else  // adressage absolu ?
						{
							int value = this.Expression(text, pass, true, variables, out err);
							if (err == null)
							{
								text = string.Concat("H'", value.ToString("X3"));
							}
							else
							{
								return null;
							}
						}

						word = this.processor.AssemblyCombineAddr(text, mode);
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

		protected int Expression(string expression, int pass, bool acceptUndefined, Dictionary<string, int> variables, out string err)
		{
			//	Evalue une expression pouvant contenir les 4 op�rations de base et des parenth�ses.
			List<string> words = Assembler.Fragment(expression);
			List<int> values = new List<int>();
			List<string> ops = new List<string>();
			int level = 0;

			foreach (string word in words)
			{
				int value = Assembler.GetValue(word, pass, acceptUndefined, variables, out err);
				if (value == Misc.undefined)
				{
					if (err != null)
					{
						return Misc.undefined;
					}

					switch (word)
					{
						case "(":
							level++;
							break;

						case ")":
							if (level == 0)
							{
								err = "Parenth�se ferm�e en trop.";
								return Misc.undefined;
							}
							level--;
							err = Assembler.Reduce(values, ops, level);
							if (err != null)
							{
								return Misc.undefined;
							}
							break;

						default:
							ops.Add(word);
							break;
					}
				}
				else
				{
					values.Add(value);

					err = Assembler.Reduce(values, ops, level);
					if (err != null)
					{
						return Misc.undefined;
					}
				}
			}

			if (level != 0)
			{
				err = "Il manque une parenth�se ferm�e.";
				return Misc.undefined;
			}

			if (values.Count != 1 || ops.Count != 0)
			{
				err = "Expression incorrecte.";
				return Misc.undefined;
			}

			err = null;
			return values[0];
		}

		protected static string Reduce(List<int> values, List<string> ops, int level)
		{
			//	Si la pile des valeurs contient deux valeurs (A et B) et la pile des op�rations n'est pas vide,
			//	remplace les deux valeurs par A op B.
			if (values.Count == level+2 && ops.Count > 0)
			{
				switch (ops[ops.Count-1])
				{
					case "+":
						values[values.Count-2] = values[values.Count-2] + values[values.Count-1];
						break;

					case "-":
						values[values.Count-2] = values[values.Count-2] - values[values.Count-1];
						break;

					case "*":
						values[values.Count-2] = values[values.Count-2] * values[values.Count-1];
						break;

					case "/":
						values[values.Count-2] = values[values.Count-2] / values[values.Count-1];
						break;

					default:
						return "Op�ration inconnue.";
				}

				values.RemoveAt(values.Count-1);  // enl�ve la derni�re valeur
				ops.RemoveAt(ops.Count-1);  // enl�ve l'op�ration effectu�e
			}
			else if (values.Count == level+1 && ops.Count > 0)
			{
				switch (ops[ops.Count-1])
				{
					case "+":
						ops.RemoveAt(ops.Count-1);  // enl�ve l'op�ration effectu�e
						break;

					case "-":
						values[values.Count-1] = -values[values.Count-1];
						ops.RemoveAt(ops.Count-1);  // enl�ve l'op�ration effectu�e
						break;
				}
			}

			return null;
		}

		protected static List<string> Fragment(string expression)
		{
			//	Fragmente une expression en "mots" �l�mentaires.
			//	Par exemple, "12+H'34*(TOTO-1)" devient "12", "+", "H'34", "*", "(", "TOTO", "-", "1", ")".
			expression = Misc.RemoveSpaces(expression);  // enl�ve les espaces
			List<string> words = new List<string>();

			int i = 0;
			int start = 0;
			char type = ' ';
			while (i < expression.Length)
			{
				char c = expression[i++];

				char newType;
				if (c >= '0' && c <= '9')
				{
					if (type == 't')
					{
						newType = 't';
					}
					else
					{
						newType = 'n';
					}
				}
				else if (i < expression.Length && expression[i] == '\'')  // "X'" ?
				{
					newType = 'n';
				}
				else if (c == '\'')
				{
					newType = 'n';
				}
				else if (c >= 'A' && c <= 'Z')
				{
					if (type == 'n')
					{
						newType = 'n';
					}
					else
					{
						newType = 't';
					}
				}
				else if (c == '_')
				{
					newType = 't';
				}
				else
				{
					newType = 's';
				}

				if (newType != type || newType == 's')
				{
					if (type != ' ')
					{
						string word = expression.Substring(start, i-start-1).Trim();
						if (!string.IsNullOrEmpty(word))
						{
							words.Add(word);
						}
					}

					type = newType;
					start = i-1;
				}
			}

			if (type != ' ')
			{
				string word = expression.Substring(start, i-start).Trim();
				if (!string.IsNullOrEmpty(word))
				{
					words.Add(word);
				}
			}

			return words;
		}

		protected static int GetValue(string word, int pass, bool acceptUndefined, Dictionary<string, int> variables, out string err)
		{
			//	Cherche une valeur, qui peut �tre soit une constante, soit une variable.
			int value = Assembler.GetNumber(word, out err);
			
			if (err != null)
			{
				return Misc.undefined;
			}

			if (value == Misc.undefined)  // pas une constante ?
			{
				string variable = Assembler.GetString(word);
				if (variable == null)  // pas une variable ?
				{
					err = null;
					return Misc.undefined;
				}

				if (variables.ContainsKey(variable))  // variable d�finie ?
				{
					value = variables[variable];  // prend la valeur de la variable
				}
				else
				{
					if (pass == 0 && acceptUndefined)  // premi�re passe ?
					{
						value = 0;  // valeur quelconque, juste pour assembler une instruction avec le bon nombre de bytes
					}
					else  // deuxi�me passe ?
					{
						err = string.Format("Variable \"{0}\" ind�finie.", variable);
						return Misc.undefined;
					}
				}
			}
			err = null;  // ok
			return value;
		}

		protected static int GetNumber(string word, out string err)
		{
			//	Cherche une constante.
			if (word.Length == 0)
			{
				err = null;
				return Misc.undefined;
			}

			if (word[0] >= '0' && word[0] <= '9')  // d�cimal par d�faut ?
			{
				int value;
				if (!int.TryParse(word, out value))
				{
					err = "Nombre d�cimal incorrect (H' si hexa).";
					return Misc.undefined;
				}
				err = null;
				return value;
			}

			if (word.Length > 1 && word[1] == '\'')
			{
				if (word[0] == 'H')  // hexad�cimal ?
				{
					int value = Misc.ParseHexa(word.Substring(2), Misc.undefined, Misc.undefined);
					if (value == Misc.undefined)
					{
						err = "Nombre hexa incorrect.";
						return Misc.undefined;
					}
					err = null;
					return value;
				}
				else if (word[0] == 'D')  // d�cimal ?
				{
					int value;
					if (!int.TryParse(word.Substring(2), out value))
					{
						err = "Nombre d�cimal incorrect.";
						return Misc.undefined;
					}
					err = null;
					return value;
				}
				else
				{
					err = "D' = d�cimal et H' = hexa.";
					return Misc.undefined;
				}
			}

			err = null;
			return Misc.undefined;
		}

		protected static string GetString(string word)
		{
			//	Cherche une cha�ne (un nom de variable ou une op�ration).
			if (word.Length == 0)
			{
				return null;
			}

			if (word[0] >= 'A' && word[0] <= 'Z')
			{
				return word;
			}

			if (word[0] == '_')
			{
				return word;
			}

			return null;
		}

		protected bool IsRegister(string word)
		{
			//	Indique si le mot correspond � un nom de registre du processeur.
			foreach (string register in this.processor.RegisterNames)
			{
				if (word == register)
				{
					return true;
				}
			}
			return false;
		}

		protected bool IsVariable(string word)
		{
			//	Indique si le mot correspond � un nom de variable valide.
			//	Exemples valides: "TOTO", "TOTO23", "TOTO_2", "_TOTO", "A1000"
			//	Exemples invalides: "2TOTO", "TOTO 2"
			if (word.Length == 0)
			{
				return false;
			}

			if ((word[0] < 'A' || word[0] > 'Z') && word[0] != '_')
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
			string program = this.field.Text;

			string[] seps = {"<br/>"};
			string[] lines = program.Split(seps, System.StringSplitOptions.None);

			int total = lines.Length;
			for (int i=lines.Length-1; i>=0; i--)
			{
				if (string.IsNullOrEmpty(lines[i]))
				{
					total--;  // supprime les lignes vides � la fin
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
				if (!simply.StartsWith("^ "))  // erreur dans cette ligne ?
				{
					builder.Append(line);
					builder.Append("<br/>");
				}
			}

			this.field.Text = builder.ToString();
		}

		protected void InsertErrors(List<int> errorLines, List<string> errorTexts)
		{
			//	Ins�re les erreurs dans le texte du programme.
			string program = TextLayout.ConvertToSimpleText(this.field.Text);  // remplace <br/> par \n et <tab/> par \t

			int cursor = 0;
			for (int i=errorLines.Count-1; i>=0; i--)  // commence par la fin pour ne pas perturber les rangs des lignes
			{
				int index1 = Assembler.LineIndex(program, errorLines[i]+0);
				int index2 = Assembler.LineIndex(program, errorLines[i]+1);

				string balast = Assembler.Balast(program.Substring(index1));
				string err = string.Concat(balast, "^ ", errorTexts[i], "\n");
				program = program.Insert(index2, err);

				cursor = index2-1;
			}

			this.field.Text = TextLayout.ConvertToTaggedText(program);  // remplace \n par <br/> et \t par <tab/>
			this.field.Cursor = cursor;
		}


		protected static string Balast(string text)
		{
			//	Retourne tout le balast pr�sent au d�but d'une ligne.
			//	TextLayout.ConvertToSimpleText doit avoir �t� ex�cut� (<br/> et <tab/> remplac�s par \n et \t).
			int index = 0;
			while (true)
			{
				int i;

				i = text.IndexOf(" ", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				i = text.IndexOf(",", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				i = text.IndexOf(":", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				i = text.IndexOf("\t", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				break;
			}

			return text.Substring(0, index);
		}

		protected static int LineIndex(string text, int lineRank)
		{
			//	Cherche l'index de la ni�me ligne d'un texte.
			//	TextLayout.ConvertToSimpleText doit avoir �t� ex�cut� (<br/> et <tab/> remplac�s par \n et \t).
			if (lineRank == 0)
			{
				return 0;
			}

			int startIndex = 0;
			while (true)
			{
				int index = text.IndexOf("\n", startIndex);
				if (index == -1)
				{
					return text.Length;
				}
				startIndex = index+1;

				if (--lineRank == 0)
				{
					return startIndex;
				}
			}
		}

		protected static string FirstLine(string err)
		{
			//	Retourne la premi�re ligne d'un texte.
			int index = err.IndexOf("<br/>");
			if (index == -1)
			{
				return err;
			}
			else
			{
				return err.Substring(0, index);
			}
		}


		protected static readonly int undefined = int.MinValue;

		protected Components.AbstractProcessor	processor;
		protected Components.Memory				memory;
		protected Window						window;
		protected TextFieldMulti				field;
		protected string						filename;
		protected bool							ending;
	}
}