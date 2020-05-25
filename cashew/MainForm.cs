using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
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
using MetroMessageBox = cashew.MessageBox.MetroMessageBox;

#pragma warning disable IDE1006

namespace cashew
{
    public partial class MainForm : MetroForm
    {
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Directory.Exists(Path.Combine(Path.GetTempPath(), "Cashew")))
                Directory.Delete(Path.Combine(Path.GetTempPath(), "Cashew"), true);
        }

        #region General

        private readonly ThreadState[] _runningStates =
            {ThreadState.Background, ThreadState.Running, ThreadState.StopRequested, ThreadState.WaitSleepJoin};

        private readonly IMetroControl[] _metroControls;
        private readonly Control[] _normalControls;
        private readonly ToolStripMenuItem[] _menuItems;
        private string[] _cseditcodel = new string[0];

        private string[] _cseditrefl =
        {
            "Microsoft.CSharp.dll", "System.dll", "System.Core.dll", "System.Data.dll",
            "System.Data.DataSetExtensions.dll", "System.Net.Http.dll", "System.Xml.dll", "System.Xml.Linq.dll"
        };

        public MainForm()
        {
            using (Splash splash = new Splash())
            {
                splash.Show();
                InitializeComponent();
                _metroControls = new IMetroControl[]
                {
                    nmtext, languageTabControl, cstab, infotab, nightmodeToggle, cseditopen, cseditrun, cseditsave,
                    csediterrorpanel, csediterrors, cseditref, infoPanel, htmltab, htmltitle,
                    htmlOptionsTile, /*htmlOptionsMenu, */htmlRefreshTile, htmlLoad, htmlSave, htmlLoadIndicator,
                    htmlUpdateToggle, htmlLiveLabel, livehider, nightmodehide, pythontab,
                    pythonSave, pythonRun, pythonOpen
                };
                _normalControls = new Control[] {htmlSep, htmldisplay, cseditCode, pythonCode, htmlText};
                _menuItems = new[]
                {
                    hTMLToolStripMenuItem, javaScriptToolStripMenuItem, cSSToolStripMenuItem, pHPToolStripMenuItem,
                    hTMLStructureSetupToolStripMenuItem, javaStructureSetupToolStripMenuItem,
                    cSSStructureSetupToolStripMenuItem, pHPStructureSetupToolStripMenuItem, linkToolStripMenuItem,
                    imageToolStripMenuItem, textToolStripMenuItem, tableToolStripMenuItem,
                    listsToolStripMenuItem, functionToolStripMenuItem, textToolStripMenuItem1,
                    alertBoxToolStripMenuItem, timeoutToolStripMenuItem, randomNumberToolStripMenuItem,
                    cSSCustomizeTagToolStripMenuItem, cSSCustomTagPropertiesToolStripMenuItem, textToolStripMenuItem3,
                    headingsToolStripMenuItem, boldbToolStripMenuItem, underlineuToolStripMenuItem,
                    italiciToolStripMenuItem,
                    deleteddelToolStripMenuItem, subscriptedSubToolStripMenuItem, superscriptedsupToolStripMenuItem,
                    tableFormatSetupToolStripMenuItem, tableHeadingthToolStripMenuItem,
                    newHorizontalItemtdToolStripMenuItem, newRowtrToolStripMenuItem,
                    orderedListSetupolToolStripMenuItem, unorderedListSetupulToolStripMenuItem,
                    listItemliToolStripMenuItem, heading1h1ToolStripMenuItem,
                    heading2h2ToolStripMenuItem, heading3h3ToolStripMenuItem, heading4h4ToolStripMenuItem,
                    heading5h5ToolStripMenuItem, heading6h6ToolStripMenuItem, textToolStripMenuItem2,
                    backgroundToolStripMenuItem, backgroundRepeatToolStripMenuItem, backgroundPositionToolStripMenuItem,
                    backgroundImageToolStripMenuItem, backgroundColorToolStripMenuItem,
                    backgroundAttachmentToolStripMenuItem,
                    fontToolStripMenuItem, sizeToolStripMenuItem, weightToolStripMenuItem, colorToolStripMenuItem,
                    directionToolStripMenuItem, lineHeightToolStripMenuItem, alignToolStripMenuItem,
                    letterSpacingToolStripMenuItem, decorationToolStripMenuItem, indentToolStripMenuItem,
                    shadowToolStripMenuItem, transformToolStripMenuItem, wordspacingToolStripMenuItem,
                    centercenterToolStripMenuItem,
                    paragraphpToolStripMenuItem
                };
                htmldisplay.DocumentText = htmlText.Text;
                metroToggle1_CheckedChanged(this, new EventArgs());
                languageTabControl.SelectedTab = infotab;
                splash.Hide();
            }

            cseditCode.SetHighlighting("C#");
            pythonCode.SetHighlighting("Python");
            htmlText.SetHighlighting("HTML");
            if (Directory.Exists(Path.Combine(Path.GetTempPath(), "Cashew")))
                Directory.Delete(Path.Combine(Path.GetTempPath(), "Cashew"), true);
            using (MemoryStream ms = new MemoryStream(Resources.IronPythonBCL))
            using (ZipArchive ar = new ZipArchive(ms))
                ar.ExtractToDirectory(Path.Combine(Path.GetTempPath(), "Cashew\\Python"));

            infoPanel.Text = infoPanel.Text.Replace("[PACKAGELIST]", string.Join("\r\n",
                XDocument.Parse(Resources.packages).Element("Project").Elements("ItemGroup")
                    .SelectMany(s => s.Elements("PackageReference"))
                    .Select(s => $"- {s.Attribute("Include").Value} {s.Attribute("Version").Value}").OrderBy(s => s)));
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

            foreach (IMetroControl c in _metroControls)
            {
                c.Style = Style;
                c.Theme = Theme;
            }

            foreach (Control c in _normalControls)
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

            foreach (ToolStripMenuItem t in _menuItems)
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
            pythonRun.Text = _pythonScript != null && _runningStates.Contains(_pythonScript.ThreadState)
                ? "Stop"
                : "Run";
            cseditrun.Text = _csScript != null && _runningStates.Contains(_csScript.ThreadState) ? "Stop" : "Run";
            htmlOptionsTile.Enabled = htmlText.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
        }

        #endregion General

        #region CS

        private Thread _csScript;

        private void metroLabel2_Click(object sender, EventArgs e) => MetroMessageBox.Show(this, csediterrors.Text,
            "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void metroPanel1_Click(object sender, EventArgs e) => MetroMessageBox.Show(this, csediterrors.Text,
            "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void cseditsave_Click(object sender, EventArgs e)
        {
            if (csSaveFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                if (csSaveFileDialog.FilterIndex == 1)
                {
                    if (cseditref.Text == "Code")
                        _cseditrefl = cseditCode.Text.Split(new[] {"\r\n"}, StringSplitOptions.None);
                    else
                        _cseditcodel = cseditCode.Text.Split(new[] {"\r\n"}, StringSplitOptions.None);
                    Stream s = File.OpenWrite(csSaveFileDialog.FileName);
                    new BinaryFormatter().Serialize(s, new[] {_cseditcodel, _cseditrefl});
                    s.Dispose();
                }
                else
                {
                    CompileCs(false, csSaveFileDialog.FilterIndex == 2, csSaveFileDialog.FileName);
                    //File.Copy(compileCS(false, csSaveFileDialog.FilterIndex == 2).PathToAssembly, csSaveFileDialog.FileName, true);
                }
            }
            catch (Exception e1)
            {
                MetroMessageBox.Show(this, e1.Message, "Failed to Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cseditrun_Click(object sender, EventArgs e)
        {
            if (_csScript != null && _runningStates.Contains(_csScript.ThreadState))
                _csScript.Abort();
            else
                try
                {
                    CompilerResults results = CompileCs();
                    cseditrun.Text = "Stop";
                    _csScript = new Thread(() =>
                    {
                        try
                        {
                            _ = results.CompiledAssembly.EntryPoint.Invoke(null, null);
                        }
                        catch
                        {
                            try
                            {
                                _ = results.CompiledAssembly.EntryPoint.Invoke(null, new object[] {new string[0]});
                            }
                            catch (Exception e1)
                            {
                                if (!e1.TryCast(out ThreadAbortException ex))
                                    Invoke((MethodInvoker) delegate
                                    {
                                        MetroMessageBox.Show(this, e1.Message, "Execution Failed",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    });
                            }
                        }
                    });
                    _csScript.Start();
                    csediterrors.Text = "Ready";
                }
                catch (Exception e1)
                {
                    csediterrors.Text = e1.Message;
                }
        }

        private CompilerResults CompileCs(bool memory = true, bool library = false, string of = "")
        {
            csediterrors.Text = "";
            if (cseditref.Text == "Code")
                _cseditrefl = cseditCode.Text.Split(new[] {"\r\n"}, StringSplitOptions.None);
            else
                _cseditcodel = cseditCode.Text.Split(new[] {"\r\n"}, StringSplitOptions.None);

            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = memory,
                GenerateExecutable = !library
            };
            parameters.OutputAssembly = memory ? parameters.OutputAssembly : of;
            parameters.ReferencedAssemblies.AddRange(_cseditrefl);
            CompilerResults results;
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                results = provider.CompileAssemblyFromSource(parameters, string.Join("\r\n", _cseditcodel));
                if (!results.Errors.HasErrors) return results;
                IEnumerable<CompilerError> err = results.Errors.OfType<CompilerError>();
                /*err.ToList().ForEach(s =>
                    {
                        TextMarker marker = new TextMarker(0, 5, TextMarkerType.WaveLine, Color.Red);
                        cseditCode.Document.MarkerStrategy.AddMarker(marker);
                    });
                    cseditCode.Update();*/
                throw new InvalidOperationException(string.Join("\r\n\r\n",
                    err.Select(s => "Error in line " + s.Line + ": " + s.ErrorNumber + " - " + s.ErrorText).ToArray()));
            }

            return results;
        }

        private void cseditopen_Click(object sender, EventArgs e)
        {
            if (csOpenFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                if (csOpenFileDialog.FilterIndex == 1)
                {
                    Stream s = File.OpenRead(csOpenFileDialog.FileName);
                    string[][] tmp = (string[][]) new BinaryFormatter().Deserialize(s);
                    s.Dispose();
                    _cseditcodel = tmp[0];
                    _cseditrefl = tmp[1];
                    cseditCode.Text = string.Join("\r\n", cseditref.Text == "References" ? _cseditcodel : _cseditrefl);
                }
                else
                {
                    CSharpDecompiler decompiler =
                        new CSharpDecompiler(csOpenFileDialog.FileName, new DecompilerSettings());
                    _cseditcodel = decompiler.DecompileWholeModuleAsString()
                        .Split(new[] {"\r\n"}, StringSplitOptions.None);
                    _cseditrefl = Assembly.LoadFrom(csOpenFileDialog.FileName).GetReferencedAssemblies()
                        .Where(s => !new[] {"mscorlib"}.Contains(s.Name))
                        .Select(s => string.IsNullOrWhiteSpace(s.CodeBase) ? s.Name + ".dll" : s.CodeBase).ToArray();
                    cseditCode.Text = string.Join("\r\n", cseditref.Text == "References" ? _cseditcodel : _cseditrefl);
                }
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1.ToString());
                MetroMessageBox.Show(this, e1.ToString(), "Failed to Load", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cseditref_Click(object sender, EventArgs e)
        {
            if (cseditref.Text == "References")
            {
                _cseditcodel = cseditCode.Text.Split(new[] {"\r\n"}, StringSplitOptions.None);
                cseditCode.Text = string.Join("\r\n", _cseditrefl);
                cseditCode.Refresh();
                cseditref.Text = "Code";
            }
            else
            {
                _cseditrefl = cseditCode.Text.Split(new[] {"\r\n"}, StringSplitOptions.None);
                cseditCode.Text = string.Join("\r\n", _cseditcodel);
                cseditCode.Refresh();
                cseditref.Text = "References";
            }
        }

        #endregion CS

        #region HTML

        private Point _sels = Point.Empty;
        private Point _sele = Point.Empty;
        private bool _htmlHasSelection;
        private bool _updateHtml = true;

        private void htmldisplay_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            htmlLoadIndicator.Visible = false;
            htmltitle.Text = htmldisplay.DocumentTitle;
        }

        private void metroToggle2_CheckedChanged(object sender, EventArgs e) => _updateHtml = htmlUpdateToggle.Checked;

        private void htmldisplay_Navigating(object sender, WebBrowserNavigatingEventArgs e) =>
            htmlLoadIndicator.Visible = true;

        private void HtmlText_TextChanged_1(object sender, EventArgs e)
        {
            if (!_updateHtml) return;
            htmldisplay.DocumentText = htmlText.Text;
            htmltitle.Text = htmldisplay.DocumentTitle;
        }

        private void htmlRefreshTile_Click(object sender, EventArgs e)
        {
            htmldisplay.DocumentText = htmlText.Text;
            htmltitle.Text = htmldisplay.DocumentTitle;
        }

        private void htmlOptionsTile_Click(object sender, EventArgs e) =>
            htmlOptionsMenu.Show(htmlOptionsTile.Location);

        private void htmlOptionsTile_MouseEnter(object sender, EventArgs e)
        {
            _htmlHasSelection = htmlText.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
            if (!_htmlHasSelection) return;
            ISelection sel = htmlText.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
            _sels.Y = sel.StartPosition.Line;
            _sels.X = sel.StartPosition.Column;
            _sele.Y = sel.EndPosition.Line;
            _sele.X = sel.EndPosition.Column;
        }

        private void AddToHtmlBox(string inFront, string atEnd)
        {
            if (!_htmlHasSelection) return;
            List<string> temp = htmlText.Text.Split(new[] {"\r\n"}, StringSplitOptions.None).ToList();
            temp[_sele.Y] = temp[_sele.Y].Insert(_sele.X, atEnd);
            temp[_sels.Y] = temp[_sels.Y].Insert(_sels.X, inFront);
            htmlText.Text = string.Join("\r\n", temp.ToArray());
        }

        private void htmlSave_Click(object sender, EventArgs e)
        {
            if (htmlSaveFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                File.WriteAllText(htmlSaveFileDialog.FileName, htmlText.Text);
            }
            catch (Exception e1)
            {
                MetroMessageBox.Show(this, e1.Message, "Saving Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void htmlLoad_Click(object sender, EventArgs e)
        {
            if (htmlOpenFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                htmlText.Text = File.ReadAllText(htmlOpenFileDialog.FileName);
            }
            catch (Exception e1)
            {
                MetroMessageBox.Show(this, e1.Message, "Loading Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Menu-HTML

        private void hTMLStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<html>\r\n<head>\r\n<title>Title</title>\r\n</head>\r\n<body>\r\n", "\r\n</body>\r\n</html>");

        private void linkToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<a href='http://www.example.com'>", "</a>");

        private void imageToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<img scr='", "' alt='Alternative Text'");

        private void centercenterToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<center>", "</center>");

        private void heading1h1ToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<h1>", "</h1>");

        private void heading2h2ToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<h2>", "</h2>");

        private void heading3h3ToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<h3>", "</h3>");

        private void heading4h4ToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<h4>", "</h4>");

        private void heading5h5ToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<h5>", "</h5>");

        private void heading6h6ToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<h6>", "</h6>");

        private void paragraphpToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<p>", "</p>");

        private void boldbToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<b>", "</b>");

        private void underlineuToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<u>", "</u>");

        private void italiciToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<i>", "</i>");

        private void deleteddelToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<del>", "</del>");

        private void subscriptedSubToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<sub>", "</sub>");

        private void superscriptedsupToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<sup>", "</sup>");

        private void tableFormatSetupToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<table border='1'>\r\n", "\r\n</table>");

        private void tableHeadingthToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<th>", "</th>");

        private void newRowtrToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<tr>", "");

        private void newHorizontalItemtdToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<td>", "</td>");

        private void orderedListSetupolToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<ol>\r\n", "\r\n</ol>");

        private void unorderedListSetupulToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<ul>\r\n", "</ul>\r\n");

        private void listItemliToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("<li>", "</li>");

        #endregion Menu-HTML

        #region Menu-Java

        private void javaStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<script type = 'text/javascript'>\r\n", "\r\n</script>");

        private void functionToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("function ", "()\r\n{\r\n\r\n}");

        private void textToolStripMenuItem1_Click(object sender, EventArgs e) => AddToHtmlBox("document.write('", "')");

        private void alertBoxToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("alert('", "')");

        private void timeoutToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("setTimeout('", "',TIME HERE)");

        private void randomNumberToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("Math.floor(Math.random()*MAXIMUM HERE)+MINIMUM HERE", "");

        #endregion Menu-Java

        #region Menu-CSS

        private void cSSStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<style type='text/css'>\r\n", "\r\n</style>");

        private void cSSCustomizeTagToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("", "\r\n{\r\n\r\n}");

        private void backgroundAttachmentToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("background-attachment:", ";");

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("background-color:", ";");

        private void backgroundImageToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("background-image:url('", "');");

        private void backgroundPositionToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("background-position:", ";");

        private void backgroundRepeatToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("background-repeat:", ";");

        private void fontToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("font-family:'", "';");

        private void sizeToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("font-size:", ";");

        private void weightToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("font-weight:", ";");

        private void colorToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("color:", ";");

        private void directionToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("direction:", ";");

        private void lineHeightToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("line-height:", ";");

        private void letterSpacingToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("letter-spacing:", ";");

        private void alignToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("text-align:", ";");

        private void decorationToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("text-decoration:", ";");

        private void indentToolStripMenuItem_Click(object sender, EventArgs e) => AddToHtmlBox("text-indent:", ";");

        private void shadowToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("text-shadow: COLOR_HERE X_IN_PX Y_IN_PX RADIUS_IN_PX;", "");

        private void transformToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("text-transform:", ";");

        private void wordspacingToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("word-spacing:", ";");

        #endregion Menu-CSS

        #region Menu-PHP

        private void pHPStructureSetupToolStripMenuItem_Click(object sender, EventArgs e) =>
            AddToHtmlBox("<?php\r\n", "\r\n?>");

        private void textToolStripMenuItem3_Click(object sender, EventArgs e) => AddToHtmlBox("echo '", "';");

        #endregion Menu-PHP

        #endregion HTML

        #region Python

        private Thread _pythonScript;

        private void pythonOpen_Click(object sender, EventArgs e)
        {
            if (pythonOpenFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                pythonCode.Text = File.ReadAllText(pythonOpenFileDialog.FileName);
            }
            catch (Exception e1)
            {
                MetroMessageBox.Show(this, e1.Message, "Loading Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pythonSave_Click(object sender, EventArgs e)
        {
            if (pythonSaveFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                File.WriteAllText(pythonSaveFileDialog.FileName, pythonCode.Text);
            }
            catch (Exception e1)
            {
                MetroMessageBox.Show(this, e1.Message, "Saving Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pythonRun_Click(object sender, EventArgs e)
        {
            if (_pythonScript != null && _runningStates.Contains(_pythonScript.ThreadState))
            {
                _pythonScript.Abort();
            }
            else
            {
                _pythonScript = new Thread(() =>
                {
                    ScriptEngine engine = Python.CreateEngine();
                    engine.SetSearchPaths(new[] {Path.Combine(Path.GetTempPath(), "Cashew\\Python")});
                    try
                    {
                        engine.Execute(pythonCode.Text);
                    }
                    catch (Exception e1)
                    {
                        if (!e1.TryCast(out ThreadAbortException _))
                            Invoke((MethodInvoker) delegate
                            {
                                MetroMessageBox.Show(this, e1.Message, "Execution Failed", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            });
                    }
                });
                _pythonScript.Start();
            }
        }

        #endregion Python
    }
}