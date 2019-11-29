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

#pragma warning disable IDE1006

namespace cashew
{
    public partial class MainForm : MetroForm
    {
        #region General

        private IMetroControl[] metroControls;
        private Control[] normalControls;
        private ToolStripMenuItem[] menuItems;
        private string[] cseditcodel;
        private string[] cseditrefl;

        public MainForm()
        {
            Splash splash = new Splash();
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

        #endregion General

        #region CS

        Thread script;
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
                        csediterrors.Text = "";
                        if (cseditref.Text == "Code")
                        {
                            cseditrefl = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        }
                        else
                        {
                            cseditcodel = cseditCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        }
                        CSharpCodeProvider provider = new CSharpCodeProvider();
                        CompilerParameters parameters = new CompilerParameters
                        {
                            GenerateInMemory = true,
                            GenerateExecutable = true
                        };
                        for (int i = 0; i < cseditrefl.Length; i++)
                            parameters.ReferencedAssemblies.Add(cseditrefl[i]);
                        CompilerResults results = provider.CompileAssemblyFromSource(parameters, string.Join("\r\n", cseditcodel));
                        if (results.Errors.HasErrors)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (CompilerError error in results.Errors)
                            {
                                sb.AppendLine(string.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                            }
                            throw new InvalidOperationException(sb.ToString());
                        }
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
            if ((script != null) && script.ThreadState == System.Threading.ThreadState.Running)
            {
                script.Abort();
            }
            else
            {
                try
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
                        GenerateInMemory = true,
                        GenerateExecutable = true,
                    };
                    parameters.ReferencedAssemblies.AddRange(cseditrefl);
                    CompilerResults results = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, string.Join("\r\n", cseditcodel));
                    if (results.Errors.HasErrors)
                        throw new InvalidOperationException(string.Join("\r\n\r\n", results.Errors.OfType<CompilerError>().Select(s => "Error in line " + s.Line.ToString() + ": " + s.ErrorNumber + " - " + s.ErrorText).ToArray()));
                    cseditrun.Text = "Stop";
                    script = new Thread(() =>
                    {
                        try
                        {
                            _ = results.CompiledAssembly.EntryPoint.Invoke(null, null);
                        }
                        finally
                        {
                            Invoke((MethodInvoker)delegate ()
                            {
                                cseditrun.Text = "Run";
                            });
                        }
                    });
                    script.Start();
                    csediterrors.Text = "Ready";
                }
                catch (Exception e1)
                {
                    csediterrors.Text = e1.Message;
                }
            }
            cseditrun.Text = script.ThreadState == System.Threading.ThreadState.Running ? "Stop" : "Run";
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
                        cseditrefl = new string[] { };
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

        private int sels = 0;
        private int sele = 0;
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
            if (htmlText.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected)
            {
                ISelection sel = htmlText.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                List<string> tmp = htmlText.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                //Not working: sele not working
                tmp.RemoveRange(sel.EndPosition.Line - 1, tmp.Count - sel.EndPosition.Line); //Determines line
                sels = string.Join("\r\n", tmp.ToArray()).ToCharArray().Length + sel.StartPosition.Column; //Line + Column
                sele = sels + sel.Length;
            }
            else { sels = 0; sele = htmlText.Text.Length; }
        }

        private void addToHTMLBox(string inFront, string atEnd)
        {
            htmlText.Text = htmlText.Text.Insert(sele, atEnd);
            htmlText.Text = htmlText.Text.Insert(sels, inFront);
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
            new Thread(() =>
            {
                ScriptEngine engine = Python.CreateEngine();
                try
                {
                    engine.Execute(pythonCode.Text);
                }
                catch (Exception e1)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        MetroMessageBox.Show(this, e1.Message, "Execution Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            }).Start();
        }

        #endregion Python
    }
}