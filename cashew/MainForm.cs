using CC_Functions.Misc;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.TextEditor.Document;
using IronPython.Hosting;
using MetroFramework;
using MetroFramework.Forms;
using MetroFramework.Interfaces;
using Microsoft.CSharp;
using Microsoft.Scripting.Hosting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ThreadState = System.Threading.ThreadState;

#pragma warning disable IDE1006

namespace cashew
{
    public partial class MainForm : MetroForm
    {
        #region General
        private readonly ThreadState[] runningStates = new ThreadState[] { ThreadState.Background, ThreadState.Running, ThreadState.StopRequested, ThreadState.WaitSleepJoin };
        private readonly IMetroControl[] metroControls;
        private readonly Control[] normalControls;
        private readonly ToolStripMenuItem[] menuItems;
        private string[] cseditcodel;
        private string[] cseditrefl;

        public MainForm()
        {
            using (Splash splash = new Splash())
            {
                splash.Show();
                InitializeComponent();
                metroControls = new IMetroControl[] { nmtext, languageTabControl, cstab, infotab, nightmodeToggle, cseditopen, cseditrun, cseditsave, csediterrorpanel, csediterrors, cseditref, infoPanel, htmltab, htmltitle, htmlOptionsTile, htmlOptionsMenu, htmlRefreshTile, htmlLoad, htmlSave, htmlLoadIndicator, htmlUpdateToggle, htmlLiveLabel, livehider, nightmodehide, pythontab,
                pythonSave, pythonRun, pythonOpen};
                normalControls = new Control[] { htmlSep, htmldisplay, cseditCode, pythonCode, htmlText };
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
                splash.Hide();
            }
            cseditCode.SetHighlighting("C#");
            pythonCode.SetHighlighting("Python");
            htmlText.SetHighlighting("HTML");
        }

        private void MAIN_Load(object sender, EventArgs e) => BringToFront();

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            if (nightmodeToggle.Checked)
            {
                Theme = MetroThemeStyle.Dark;
                Style = MetroColorStyle.Magenta;
            }
            else
            {
                Theme = MetroThemeStyle.Light;
                Style = MetroColorStyle.Blue;
            }
            foreach (IMetroControl c in metroControls)
            {
                c.Style = Style;
                c.Theme = Theme;
            }
            foreach (Control c in normalControls)
            {
                if (Theme == MetroThemeStyle.Dark)
                {
                    c.BackColor = Color.Black;
                    c.ForeColor = Color.Black;
                }
                else
                {
                    c.BackColor = Color.White;
                    c.ForeColor = Color.White;
                }
            }
            foreach (ToolStripMenuItem t in menuItems)
            {
                if (Theme == MetroThemeStyle.Dark)
                {
                    t.BackColor = Color.Black;
                    t.ForeColor = Color.FromArgb(255, 0, 151);
                }
                else
                {
                    t.BackColor = Color.White;
                    t.ForeColor = Color.FromArgb(45, 137, 239);
                }
            }
            Refresh();
        }

        private void buttonFix_Tick(object sender, EventArgs e)
        {
            pythonRun.Text = pythonScript != null && runningStates.Contains(pythonScript.ThreadState) ? "Stop" : "Run";
            cseditrun.Text = csScript != null && runningStates.Contains(csScript.ThreadState) ? "Stop" : "Run";
            htmlOptionsTile.Enabled = htmlText.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
        }

        #endregion General

        #region CS

        Thread csScript;
        private void metroLabel2_Click(object sender, EventArgs e) => MetroMessageBox.Show(this, csediterrors.Text, "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void metroPanel1_Click(object sender, EventArgs e) => MetroMessageBox.Show(this, csediterrors.Text, "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void cseditsave_Click(object sender, EventArgs e)
        {
            if (csSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (csSaveFileDialog.FilterIndex == 1)
                    {
                        if (cseditref.Text == "Code")
                        {
                            cseditrefl = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        }
                        else
                        {
                            cseditcodel = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        }
                        Stream s = File.OpenWrite(csSaveFileDialog.FileName);
                        new BinaryFormatter().Serialize(s, new string[][] { cseditcodel, cseditrefl });
                        s.Dispose();
                    }
                    else
                    {
                        File.Copy(compileCS(false).PathToAssembly, csSaveFileDialog.FileName);
                    }
                }
                catch (Exception e1)
                {
                    MetroMessageBox.Show(this, e1.Message, "Failed to Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cseditrun_Click(object sender, EventArgs e)
        {
            if ((csScript != null) && runningStates.Contains(csScript.ThreadState))
            {
                csScript.Abort();
            }
            else
            {
                try
                {
                    CompilerResults results = compileCS();
                    cseditrun.Text = "Stop";
                        csScript = new Thread(() =>
                        {
                            try
                            {
                                _ = results.CompiledAssembly.EntryPoint.Invoke(null, null);
                            }
                            catch (Exception e1)
                            {
                                if (!e1.tryCast(out ThreadAbortException ex))
                                    Invoke((MethodInvoker)delegate ()
                                    {
                                        MetroMessageBox.Show(this, e1.Message, "Execution Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    });
                            }
                        });
                    csScript.Start();
                    csediterrors.Text = "Ready";
                }
                catch (Exception e1)
                {
                    csediterrors.Text = e1.Message;
                }
            }
            cseditrun.Text = csScript.ThreadState == System.Threading.ThreadState.Running ? "Stop" : "Run";
        }

        private CompilerResults compileCS(bool memory = true)
        {
            csediterrors.Text = "";
            if (cseditref.Text == "Code")
            {
                cseditrefl = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            else
            {
                cseditcodel = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = memory,
                GenerateExecutable = true,
            };
            parameters.ReferencedAssemblies.AddRange(cseditrefl);
            CompilerResults results;
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                results = provider.CompileAssemblyFromSource(parameters, string.Join("\r\n", cseditcodel));
                if (results.Errors.HasErrors)
                    throw new InvalidOperationException(string.Join("\r\n\r\n", results.Errors.OfType<CompilerError>().Select(s => "Error in line " + s.Line.ToString() + ": " + s.ErrorNumber + " - " + s.ErrorText).ToArray()));
            }
            return results;
        }

        private void cseditopen_Click(object sender, EventArgs e)
        {
            if (csOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (csOpenFileDialog.FilterIndex == 1)
                    {
                        Stream s = File.OpenRead(csOpenFileDialog.FileName);
                        string[][] tmp = (string[][])new BinaryFormatter().Deserialize(s);
                        s.Dispose();
                        cseditcodel = tmp[0];
                        cseditrefl = tmp[1];
                        if (cseditref.Text == "References")
                            cseditCode.Text = string.Join("\r\n", cseditcodel);
                        else
                            cseditCode.Text = string.Join("\r\n", cseditrefl);
                    }
                    else
                    {
                        CSharpDecompiler decompiler = new CSharpDecompiler(csOpenFileDialog.FileName, new DecompilerSettings());
                        cseditcodel = decompiler.DecompileWholeModuleAsString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        cseditrefl = Assembly.LoadFrom(csOpenFileDialog.FileName)
                            .GetReferencedAssemblies().Where(s => !new string[] { "mscorlib" }.Contains(s.Name))
                            .Select(s => string.IsNullOrWhiteSpace(s.CodeBase) ? (s.Name + ".dll") : s.CodeBase).ToArray();
                        if (cseditref.Text == "References")
                            cseditCode.Text = string.Join("\r\n", cseditcodel);
                        else
                            cseditCode.Text = string.Join("\r\n", cseditrefl);
                    }
                }
                catch (Exception e1)
                {
                    MetroMessageBox.Show(this, e1.ToString(), "Failed to Load", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cseditref_Click(object sender, EventArgs e)
        {
            if (cseditref.Text == "References")
            {
                cseditcodel = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                cseditCode.Text = string.Join("\r\n", cseditrefl);
                cseditCode.Refresh();
                cseditref.Text = "Code";
            }
            else
            {
                cseditrefl = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                cseditCode.Text = string.Join("\r\n", cseditcodel);
                cseditCode.Refresh();
                cseditref.Text = "References";
            }
        }

        #endregion CS

        #region HTML

        private Point sels = Point.Empty;
        private Point sele = Point.Empty;
        private bool htmlHasSelection = false;
        private bool UpdateHTML = true;

        private void htmldisplay_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
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

        private void htmlRefreshTile_Click(object sender, EventArgs e)
        {
            htmldisplay.DocumentText = htmlText.Text;
            htmltitle.Text = htmldisplay.DocumentTitle;
        }

        private void htmlOptionsTile_Click(object sender, EventArgs e) => htmlOptionsMenu.Show(htmlOptionsTile.Location);

        private void htmlOptionsTile_MouseEnter(object sender, EventArgs e)
        {
            htmlHasSelection = htmlText.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
            if (htmlHasSelection)
            {
                ISelection sel = htmlText.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                sels.Y = sel.StartPosition.Line;
                sels.X = sel.StartPosition.Column;
                sele.Y = sel.EndPosition.Line;
                sele.X = sel.EndPosition.Column;
            }
        }

        private void addToHTMLBox(string inFront, string atEnd)
        {
            if (htmlHasSelection)
            {
                List<string> temp = htmlText.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                temp[sele.Y] = temp[sele.Y].Insert(sele.X, atEnd);
                temp[sels.Y] = temp[sels.Y].Insert(sels.X, inFront);
                htmlText.Text = string.Join("\r\n", temp.ToArray());
            }
        }

        private void htmlSave_Click(object sender, EventArgs e)
        {
            if (htmlSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(htmlSaveFileDialog.FileName, htmlText.Text);
                }
                catch (Exception e1)
                {
                    MetroMessageBox.Show(this, e1.Message, "Saving Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void htmlLoad_Click(object sender, EventArgs e)
        {
            if (htmlOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    htmlText.Text = File.ReadAllText(htmlOpenFileDialog.FileName);
                }
                catch (Exception e1)
                {
                    MetroMessageBox.Show(this, e1.Message, "Loading Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        #endregion Menu-HTML

        #region Menu-Java

        private void javaStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<script type = 'text/javascript'>\r\n", "\r\n</script>");

        private void functionToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("function ", "()\r\n{\r\n\r\n}");

        private void textToolStripMenuItem1_Click(object sender, EventArgs e) => addToHTMLBox("document.write('", "')");

        private void alertBoxToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("alert('", "')");

        private void timeoutToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("setTimeout('", "',TIME HERE)");

        private void randomNumberToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("Math.floor(Math.random()*MAXIMUM HERE)+MINIMUM HERE", "");

        #endregion Menu-Java

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

        #endregion Menu-CSS

        #region Menu-PHP

        private void pHPStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) => addToHTMLBox("<?php\r\n", "\r\n?>");

        private void textToolStripMenuItem3_Click(object sender, EventArgs e) => addToHTMLBox("echo '", "';");

        #endregion Menu-PHP

        #endregion HTML

        #region Python
        Thread pythonScript;
        private void pythonOpen_Click(object sender, EventArgs e)
        {
            if (pythonOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pythonCode.Text = File.ReadAllText(pythonOpenFileDialog.FileName);
                }
                catch (Exception e1)
                {
                    MetroMessageBox.Show(this, e1.Message, "Loading Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pythonSave_Click(object sender, EventArgs e)
        {
            if (pythonSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(pythonSaveFileDialog.FileName, pythonCode.Text);
                }
                catch (Exception e1)
                {
                    MetroMessageBox.Show(this, e1.Message, "Saving Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pythonRun_Click(object sender, EventArgs e)
        {
            if ((pythonScript != null) && runningStates.Contains(pythonScript.ThreadState))
            {
                pythonScript.Abort();
            }
            else
            {
                pythonScript = new Thread(() =>
                {
                    ScriptEngine engine = Python.CreateEngine();
                    try
                    {
                        engine.Execute(pythonCode.Text);
                    }
                    catch (Exception e1)
                    {
                        if (!e1.tryCast(out ThreadAbortException ex))
                            Invoke((MethodInvoker)delegate ()
                            {
                                MetroMessageBox.Show(this, e1.Message, "Execution Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            });
                    }
                });
                pythonScript.Start();
            }
        }

        #endregion Python
    }
}