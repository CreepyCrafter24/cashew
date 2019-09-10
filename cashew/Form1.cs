using System;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using ICSharpCode.TextEditor.Document;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using System.Reflection;
using System.IO.Compression;
using MetroFramework.Interfaces;
using CC_Functions.Misc;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
#pragma warning disable IDE1006

namespace cashew {
    public partial class MAIN : MetroForm {
        #region General
        IMetroControl[] metroControls;
        Control[] normalControls;
        ToolStripMenuItem[] menuItems;
        string[] cseditcodel;
        string[] cseditrefl;
        string TempPath = Path.GetTempPath() + "cashew";
        public MAIN() {
            Splash splash = new Splash();
            splash.Show();
            InitializeComponent();
            metroControls = new IMetroControl[] { nmtext, languageTabControl, cstab, infotab, nightmodeToggle, cseditopen, cseditrun, cseditsave, csediterrorpanel, csediterrors, cseditref, infoPanel, htmltab, htmltitle, htmlOptionsTile, htmlOptionsMenu, htmlRefreshTile, htmlLoad, htmlSave, htmlLoadIndicator, htmlUpdateToggle, htmlLiveLabel, livehider, nightmodehide, pythontab,
                pythonSave, pythonRun, pythonOpen};
            normalControls = new Control[] { htmlSep, htmldisplay, cseditcode, pythonCode, htmlText };
            menuItems = new ToolStripMenuItem[] { hTMLToolStripMenuItem, javaScriptToolStripMenuItem, cSSToolStripMenuItem, pHPToolStripMenuItem, hTMLStructureSetupToolStripMenuItem, javaStructureSetupToolStripMenuItem, cSSStructureSetupToolStripMenuItem, pHPStructureSetupToolStripMenuItem, linkToolStripMenuItem, imageToolStripMenuItem, textToolStripMenuItem, tableToolStripMenuItem,
                listsToolStripMenuItem, functionToolStripMenuItem, textToolStripMenuItem1, alertBoxToolStripMenuItem, timeoutToolStripMenuItem, randomNumberToolStripMenuItem, cSSCustomizeTagToolStripMenuItem, cSSCustomTagPropertiesToolStripMenuItem, textToolStripMenuItem3, headingsToolStripMenuItem, boldbToolStripMenuItem, underlineuToolStripMenuItem, italiciToolStripMenuItem,
                deleteddelToolStripMenuItem, subscriptedSubToolStripMenuItem, superscriptedsupToolStripMenuItem, tableFormatSetupToolStripMenuItem, tableHeadingthToolStripMenuItem, newHorizontalItemtdToolStripMenuItem, newRowtrToolStripMenuItem, orderedListSetupolToolStripMenuItem, unorderedListSetupulToolStripMenuItem, listItemliToolStripMenuItem, heading1h1ToolStripMenuItem,
                heading2h2ToolStripMenuItem, heading3h3ToolStripMenuItem, heading4h4ToolStripMenuItem, heading5h5ToolStripMenuItem, heading6h6ToolStripMenuItem, textToolStripMenuItem2, backgroundToolStripMenuItem, backgroundRepeatToolStripMenuItem, backgroundPositionToolStripMenuItem, backgroundImageToolStripMenuItem, backgroundColorToolStripMenuItem, backgroundAttachmentToolStripMenuItem,
                fontToolStripMenuItem, sizeToolStripMenuItem, weightToolStripMenuItem, colorToolStripMenuItem, directionToolStripMenuItem, lineHeightToolStripMenuItem, alignToolStripMenuItem, letterSpacingToolStripMenuItem, decorationToolStripMenuItem, indentToolStripMenuItem, shadowToolStripMenuItem, transformToolStripMenuItem, wordspacingToolStripMenuItem, centercenterToolStripMenuItem,
                paragraphpToolStripMenuItem};
            cseditrefl = new string[1] { "System.Windows.Forms.dll" };
            htmldisplay.DocumentText = htmlText.Text;
            metroToggle1_CheckedChanged(this, new EventArgs());
            languageTabControl.SelectedTab = infotab;
            if (Directory.Exists(TempPath)) { Directory.Delete(TempPath, true); }
            Directory.CreateDirectory(TempPath + @"\xshd");
            File.WriteAllBytes(TempPath + @"\tmp.zip", Resources.xshd);
            ZipFile.ExtractToDirectory(TempPath + @"\tmp.zip", TempPath + @"\xshd");
            File.Delete(TempPath + @"\tmp.zip");
            HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(TempPath + @"\xshd"));
            try {
                if (Directory.Exists(TempPath + @"\Python\")) {
                    Directory.Delete(TempPath + @"\Python\");
                }
                try { Directory.CreateDirectory(TempPath + @"\Python\"); } catch { }
                File.WriteAllBytes(TempPath + @"\Python.zip", Resources.Python);
                ZipFile.ExtractToDirectory(TempPath + @"\Python.zip", TempPath + @"\Python");
            } catch (Exception e) { MessageBox.Show(e.ToString()); }
            splash.Hide();
        }

        private void MAIN_Load(object sender, EventArgs e) => BringToFront();

        private void metroToggle1_CheckedChanged(object sender, EventArgs e) {
            if (nightmodeToggle.Checked) {
                Theme = MetroThemeStyle.Dark;
                Style = MetroColorStyle.Magenta;
            } else {
                Theme = MetroThemeStyle.Light;
                Style = MetroColorStyle.Blue;
            }
            foreach (MetroFramework.Interfaces.IMetroControl c in metroControls) {
                c.Style = Style;
                c.Theme = Theme;
            }
            foreach (Control c in normalControls) {
                if (Theme == MetroThemeStyle.Dark) {
                    c.BackColor = Color.Black;
                    c.ForeColor = Color.Black;
                } else {
                    c.BackColor = Color.White;
                    c.ForeColor = Color.White;
                }
            }
            foreach (ToolStripMenuItem t in menuItems) {
                if (Theme == MetroThemeStyle.Dark) {
                    t.BackColor = Color.Black;
                    t.ForeColor = Color.FromArgb(255, 0, 151);
                }
                else {
                    t.BackColor = Color.White;
                    t.ForeColor = Color.FromArgb(45, 137, 239);
                }
            }
            Refresh();
        }
        #endregion

        #region CS
        MethodInfo script;
        private void metroLabel2_Click(object sender, EventArgs e) => MessageBox.Show(csediterrors.Text, "Errors");
        private void metroPanel1_Click(object sender, EventArgs e) => MessageBox.Show(csediterrors.Text, "Errors");
        private void cseditsave_Click(object sender, EventArgs e) {
            if (csSaveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    if (cseditref.Text == "Code") {
                        cseditrefl = cseditcode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    } else {
                        cseditcodel = cseditcode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    }
                    Stream s = File.OpenWrite(csSaveFileDialog.FileName);
                    new BinaryFormatter().Serialize(s, new string[][] { cseditcodel, cseditrefl });
                    s.Dispose();
                } catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Failed to Save");
                }
            }
        }

        private void cseditrun_Click(object sender, EventArgs e) {
            if (cseditrun.Text == "Run") {
                try {
                    csediterrors.Text = "";
                    if (cseditref.Text == "Code") {
                        cseditrefl = cseditcode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    } else {
                        cseditcodel = cseditcode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    }
                    CSharpCodeProvider provider = new CSharpCodeProvider();
                    CompilerParameters parameters = new CompilerParameters
                    {
                        GenerateInMemory = true,
                        GenerateExecutable = true
                    };
                    for (int i = 0; i < cseditrefl.Length; i++)
                        parameters.ReferencedAssemblies.Add(cseditrefl[i]);
                    CompilerResults results = provider.CompileAssemblyFromSource(parameters, ArrayFormatter.ArrayToString(cseditcodel));
                    if (results.Errors.HasErrors)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (CompilerError error in results.Errors)
                        {
                            sb.AppendLine(string.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                        }
                        throw new InvalidOperationException(sb.ToString());
                    }
                    Assembly assembly = results.CompiledAssembly;
                    Type program = assembly.GetType("Project.Program");
                    script = program.GetMethod("Main");
                    cseditrun.Text = "Stop";
                    cseditexecutor.RunWorkerAsync();
                    csediterrors.Text = "Ready";
                } catch (Exception e1) {
                    csediterrors.Text = e1.Message;
                }
            } else {
                cseditexecutor.CancelAsync();
            }
        }
        
        private void cseditopen_Click(object sender, EventArgs e) {
            if (csOpenFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    if (csOpenFileDialog.FilterIndex == 1) {
                        Stream s = File.OpenRead(csOpenFileDialog.FileName);
                        string[][] tmp = (string[][])new BinaryFormatter().Deserialize(s);
                        s.Dispose();
                        cseditcodel = tmp[0];
                        cseditrefl = tmp[1];
                        if (cseditref.Text == "References")
                            cseditcode.Text = ArrayFormatter.ArrayToString(cseditcodel);
                        else
                            cseditcode.Text = ArrayFormatter.ArrayToString(cseditrefl);
                    } else {
                        CSharpDecompiler decompiler = new CSharpDecompiler(csOpenFileDialog.FileName, new DecompilerSettings());
                        cseditcodel = decompiler.DecompileWholeModuleAsString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        cseditrefl = new string[]{};
                        if (cseditref.Text == "References")
                            cseditcode.Text = ArrayFormatter.ArrayToString(cseditcodel);
                        else
                            cseditcode.Text = ArrayFormatter.ArrayToString(cseditrefl);
                    }
                } catch (Exception e1) {
                    MessageBox.Show(e1.ToString(), "Failed to Load");
                }
            }
        }

        private void cseditref_Click(object sender, EventArgs e) {
            if (cseditref.Text == "References") {
                cseditcodel = cseditcode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                cseditcode.Text = ArrayFormatter.ArrayToString(cseditrefl);
                cseditcode.Refresh();
                cseditref.Text = "Code";
            } else {
                cseditrefl = cseditcode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                cseditcode.Text = ArrayFormatter.ArrayToString(cseditcodel);
                cseditcode.Refresh();
                cseditref.Text = "References";
            }
        }

        private void cseditexecutor_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            while (cseditexecutor.CancellationPending == false) {
                MethodInfo q = script;
                object o = q.Invoke(null, null);
                return;
            }
        }

        private void cseditexecutor_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) => cseditrun.Text = "Run";
        #endregion

        #region HTML
        int sels = 0;
        int sele = 0;
        bool UpdateHTML = true;
        private void htmldisplay_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            htmlLoadIndicator.Visible = false;
            htmltitle.Text = htmldisplay.DocumentTitle;
        }

        private void metroToggle2_CheckedChanged(object sender, EventArgs e) => UpdateHTML = htmlUpdateToggle.Checked;
        private void htmldisplay_Navigating(object sender, WebBrowserNavigatingEventArgs e) => htmlLoadIndicator.Visible = true;

        private void HtmlText_TextChanged_1(object sender, EventArgs e)
        {
            if (UpdateHTML)
            {
                htmldisplay.DocumentText = htmlText.Text;
                htmltitle.Text = htmldisplay.DocumentTitle;
            }
        }

        private void htmlRefreshTile_Click(object sender, EventArgs e) {
            htmldisplay.DocumentText = htmlText.Text;
            htmltitle.Text = htmldisplay.DocumentTitle;
        }

        private void htmlOptionsTile_Click(object sender, EventArgs e) => htmlOptionsMenu.Show(htmlOptionsTile.Location);
        private void htmlOptionsTile_MouseEnter(object sender, EventArgs e) {
            if (htmlText.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected) {
                ISelection sel = htmlText.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                List<string> tmp = htmlText.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                //Not working: sele not working
                tmp.RemoveRange(sel.EndPosition.Line - 1, tmp.Count - sel.EndPosition.Line); //Determins line
                sels = ArrayFormatter.ArrayToString(tmp.ToArray()).ToCharArray().Length + sel.StartPosition.Column; //Line + Column
                sele = sels + sel.Length;
            } else { sels = 0; sele = htmlText.Text.Length; }
        }

        private void addToHTMLBox(string inFront, string atEnd) {
            htmlText.Text = htmlText.Text.Insert(sele, atEnd);
            htmlText.Text = htmlText.Text.Insert(sels, inFront);
        }

        private void htmlSave_Click(object sender, EventArgs e) {
            if (htmlSaveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    File.WriteAllText(htmlSaveFileDialog.FileName, htmlText.Text);
                } catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Saving Failed");
                }
            }
        }

        private void htmlLoad_Click(object sender, EventArgs e) {
            if (htmlOpenFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    htmlText.Text = File.ReadAllText(htmlOpenFileDialog.FileName);
                } catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Loading Failed");
                }
            }
        }
        #region Menu-HTML
        private void hTMLStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<html>\r\n<head>\r\n<title>Title</title>\r\n</head>\r\n<body>\r\n", "\r\n</body>\r\n</html>");
        private void linkToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<a href='http://www.example.com'>", "</a>");
        private void imageToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<img scr='", "' alt='Alternative Text'");
        private void centercenterToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<center>", "</center>");
        private void heading1h1ToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<h1>", "</h1>");
        private void heading2h2ToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<h2>", "</h2>");
        private void heading3h3ToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<h3>", "</h3>");
        private void heading4h4ToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<h4>", "</h4>");
        private void heading5h5ToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<h5>", "</h5>");
        private void heading6h6ToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<h6>", "</h6>");
        private void paragraphpToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<p>", "</p>");
        private void boldbToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<b>", "</b>");
        private void underlineuToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<u>", "</u>");
        private void italiciToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<i>", "</i>");
        private void deleteddelToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<del>", "</del>");
        private void subscriptedSubToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<sub>", "</sub>");
        private void superscriptedsupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<sup>", "</sup>");
        private void tableFormatSetupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<table border='1'>\r\n", "\r\n</table>");
        private void tableHeadingthToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<th>", "</th>");
        private void newRowtrToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<tr>", "");
        private void newHorizontalItemtdToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<td>", "</td>");
        private void orderedListSetupolToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<ol>\r\n", "\r\n</ol>");
        private void unorderedListSetupulToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<ul>\r\n", "</ul>\r\n");
        private void listItemliToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<li>", "</li>");
        #endregion
        #region Menu-Java
        private void javaStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<script type = 'text/javascript'>\r\n", "\r\n</script>");
        private void functionToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("function ", "()\r\n{\r\n\r\n}");
        private void textToolStripMenuItem1_Click(object sender, EventArgs e) => addToHTMLBox("document.write('", "')");
        private void alertBoxToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("alert('", "')");
        private void timeoutToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("setTimeout('", "',TIME HERE)");
        private void randomNumberToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("Math.floor(Math.random()*MAXIMUM HERE)+MINIMUM HERE", "");
        #endregion
        #region Menu-CSS
        private void cSSStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<style type='text/css'>\r\n", "\r\n</style>");
        private void cSSCustomizeTagToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("", "\r\n{\r\n\r\n}");
        private void backgroundAttachmentToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("background-attachment:", ";");
        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("background-color:", ";");
        private void backgroundImageToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("background-image:url('", "');");
        private void backgroundPositionToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("background-position:", ";");
        private void backgroundRepeatToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("background-repeat:", ";");
        private void fontToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("font-family:'", "';");
        private void sizeToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("font-size:", ";");
        private void weightToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("font-weight:", ";");
        private void colorToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("color:", ";");
        private void directionToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("direction:", ";");
        private void lineHeightToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("line-height:", ";");
        private void letterSpacingToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("letter-spacing:", ";");
        private void alignToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("text-align:", ";");
        private void decorationToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("text-decoration:", ";");
        private void indentToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("text-indent:", ";");
        private void shadowToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("text-shadow: COLOR_HERE X_IN_PX Y_IN_PX RADIUS_IN_PX;", "");
        private void transformToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("text-transform:", ";");
        private void wordspacingToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("word-spacing:", ";");
        #endregion
        #region Menu-PHP
        private void pHPStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<?php\r\n", "\r\n?>");
        private void textToolStripMenuItem3_Click(object sender, EventArgs e) => addToHTMLBox("echo '", "';");
        #endregion

        #endregion

        #region Python
        private void pythonOpen_Click(object sender, EventArgs e) {
            if (pythonOpenFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    pythonCode.Text = File.ReadAllText(pythonOpenFileDialog.FileName);
                } catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Loading Failed");
                }
            }
        }

        private void pythonSave_Click(object sender, EventArgs e) {
            if (pythonSaveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    File.WriteAllText(pythonSaveFileDialog.FileName, pythonCode.Text);
                }
                catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Saving Failed");
                }
            }
        }

        private void pythonRun_Click(object sender, EventArgs e) {
            File.WriteAllText(TempPath + @"\Python\tmp.py", pythonCode.Text);
            Process process = Process.Start(new ProcessStartInfo { FileName = TempPath + @"\Python\python.exe", Arguments = TempPath + @"\Python\tmp.py", UseShellExecute = true });
        }
        #endregion
    }
}

//Fix HTML Tab; Add syntax tree to cs?
