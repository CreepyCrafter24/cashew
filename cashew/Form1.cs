#region MakeApp()
using System;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using System.IO;
using CCFunctions;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
#pragma warning disable IDE1006
namespace cashew {
    public partial class MAIN : MetroForm {
        #region General
        MetroFramework.Interfaces.IMetroControl[] metroControls;
        Control[] normalControls;
        ToolStripMenuItem[] menuItems;
        string[] cseditcodel;
        string[] cseditrefl;
        public MAIN() {
            InitializeComponent();
            metroControls = new MetroFramework.Interfaces.IMetroControl[] { nmtext, languageTabControl, cstab, infotab, nightmodeToggle, cseditopen, cseditrun, cseditsave, cseditcode, csedit, cslive, csediterrorpanel, csediterrors, cseditref, infoPanel, htmltab, csinftc, csedittp, cslivetp, htmltext, htmltitle, htmlOptionsTile, htmlOptionsMenu, htmlRefreshTile, htmlLoad, htmlSave,
                htmlLoadIndicator, htmlUpdateToggle, htmlLiveLabel, livehider, nightmodehide, cslivenotyetimplemented, pythontab, pythonSave, pythonRun, pythonOpen, pythonCode, pythonExtract};
            normalControls = new Control[] { htmlSep, htmldisplay };
            menuItems = new ToolStripMenuItem[] { hTMLToolStripMenuItem, javaScriptToolStripMenuItem, cSSToolStripMenuItem, pHPToolStripMenuItem, hTMLStructureSetupToolStripMenuItem, javaStructureSetupToolStripMenuItem, cSSStructureSetupToolStripMenuItem, pHPStructureSetupToolStripMenuItem, linkToolStripMenuItem, imageToolStripMenuItem, textToolStripMenuItem, tableToolStripMenuItem,
                listsToolStripMenuItem, functionToolStripMenuItem, textToolStripMenuItem1, alertBoxToolStripMenuItem, timeoutToolStripMenuItem, randomNumberToolStripMenuItem, cSSCustomizeTagToolStripMenuItem, cSSCustomTagPropertiesToolStripMenuItem, textToolStripMenuItem3, headingsToolStripMenuItem, boldbToolStripMenuItem, underlineuToolStripMenuItem, italiciToolStripMenuItem,
                deleteddelToolStripMenuItem, subscriptedSubToolStripMenuItem, superscriptedsupToolStripMenuItem, tableFormatSetupToolStripMenuItem, tableHeadingthToolStripMenuItem, newHorizontalItemtdToolStripMenuItem, newRowtrToolStripMenuItem, orderedListSetupolToolStripMenuItem, unorderedListSetupulToolStripMenuItem, listItemliToolStripMenuItem, heading1h1ToolStripMenuItem,
                heading2h2ToolStripMenuItem, heading3h3ToolStripMenuItem, heading4h4ToolStripMenuItem, heading5h5ToolStripMenuItem, heading6h6ToolStripMenuItem, textToolStripMenuItem2, backgroundToolStripMenuItem, backgroundRepeatToolStripMenuItem, backgroundPositionToolStripMenuItem, backgroundImageToolStripMenuItem, backgroundColorToolStripMenuItem, backgroundAttachmentToolStripMenuItem,
                fontToolStripMenuItem, sizeToolStripMenuItem, weightToolStripMenuItem, colorToolStripMenuItem, directionToolStripMenuItem, lineHeightToolStripMenuItem, alignToolStripMenuItem, letterSpacingToolStripMenuItem, decorationToolStripMenuItem, indentToolStripMenuItem, shadowToolStripMenuItem, transformToolStripMenuItem, wordspacingToolStripMenuItem, centercenterToolStripMenuItem,
                paragraphpToolStripMenuItem};
            cseditrefl = new string[1] { "System.Windows.Forms.dll" };
            csinftc.Appearance = TabAppearance.Buttons;
            csinftc.ItemSize = new Size(0, 1);
            csinftc.Multiline = true;
            csinftc.SizeMode = TabSizeMode.Fixed;
            htmldisplay.DocumentText = htmltext.Text;
            metroToggle1_CheckedChanged(this, new EventArgs());
            languageTabControl.SelectedTab = infotab;
            csinftc.SelectedTab = csedittp;
        }

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
        System.Reflection.MethodInfo script;
        private void metroLabel2_Click(object sender, EventArgs e) => MessageBox.Show(csediterrors.Text, "Errors");
        private void metroPanel1_Click(object sender, EventArgs e) => MessageBox.Show(csediterrors.Text, "Errors");
        private void csedit_Click(object sender, EventArgs e) => csinftc.SelectedTab = csedittp;
        private void cslive_Click(object sender, EventArgs e) => csinftc.SelectedTab = cslivetp;
        private void cseditsave_Click(object sender, EventArgs e) {
            if (csSaveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    if (cseditref.Text == "Code") {
                        cseditrefl = cseditcode.Lines;
                    }
                    else {
                        cseditcodel = cseditcode.Lines;
                    }
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Create(csSaveFileDialog.FileName);
                    bf.Serialize(file, new string[][] { cseditcodel, cseditrefl });
                    file.Close();
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
                        cseditrefl = cseditcode.Lines;
                    } else {
                        cseditcodel = cseditcode.Lines;
                    }
                    script = Compiling.CScriptToMethod(Misc.ArrayToString(cseditcodel, true), "Project", "Program", "Main", cseditrefl, new Microsoft.CSharp.CSharpCodeProvider(), new System.CodeDom.Compiler.CompilerParameters(), true, true);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(csOpenFileDialog.FileName, FileMode.Open);
                    string[][] tmp = (string[][])bf.Deserialize(file);
                    cseditcodel = tmp[0];
                    cseditrefl = tmp[1];
                    file.Close();
                    if (cseditref.Text == "References") {
                        cseditcode.Lines = cseditcodel;
                    } else {
                        cseditcode.Lines = cseditrefl;
                    }
                } catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Failed to Load");
                }
            }
        }

        private void cseditref_Click(object sender, EventArgs e) {
            if (cseditref.Text == "References") {
                cseditcodel = cseditcode.Lines;
                cseditcode.Lines = cseditrefl;
                cseditref.Text = "Code";
            } else {
                cseditrefl = cseditcode.Lines;
                cseditcode.Lines = cseditcodel;
                cseditref.Text = "References";
            }
        }

        private void cseditexecutor_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            while (cseditexecutor.CancellationPending == false) {
                System.Reflection.MethodInfo q = script;
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
        private void htmltext_TextChanged(object sender, EventArgs e) {
            if (UpdateHTML) {
                htmldisplay.DocumentText = htmltext.Text;
                htmltitle.Text = htmldisplay.DocumentTitle;
            }
        }

        private void htmlRefreshTile_Click(object sender, EventArgs e) {
            htmldisplay.DocumentText = htmltext.Text;
            htmltitle.Text = htmldisplay.DocumentTitle;
        }

        private void htmlOptionsTile_Click(object sender, EventArgs e) => htmlOptionsMenu.Show(htmlOptionsTile.Location);
        private void htmlOptionsTile_MouseEnter(object sender, EventArgs e) {
            sels = htmltext.SelectionStart;
            sele = htmltext.SelectionLength;
        }

        private void addToHTMLBox(string inFront, string atEnd) {
            htmltext.Text = htmltext.Text.Insert(sels + sele, atEnd);
            htmltext.Text = htmltext.Text.Insert(sels, inFront);
        }

        private void htmlSave_Click(object sender, EventArgs e) {
            if (htmlSaveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    File.WriteAllLines(htmlSaveFileDialog.FileName, htmltext.Lines);
                } catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Saving Failed");
                }
            }
        }

        private void htmlLoad_Click(object sender, EventArgs e) {
            if (htmlOpenFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    htmltext.Lines = File.ReadAllLines(htmlOpenFileDialog.FileName);
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
                    pythonCode.Lines = File.ReadAllLines(pythonOpenFileDialog.FileName);
                } catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Loading Failed");
                }
            }
        }

        private void pythonSave_Click(object sender, EventArgs e) {
            if (pythonSaveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    File.WriteAllLines(pythonSaveFileDialog.FileName, pythonCode.Lines);
                }
                catch (Exception e1) {
                    MessageBox.Show(e1.Message, "Saving Failed");
                }
            }
        }

        private void pythonRun_Click(object sender, EventArgs e) {
            File.WriteAllLines(Path.GetTempPath() + @"Python\tmp.py",pythonCode.Lines);
            Process process = Process.Start(new ProcessStartInfo { FileName = Path.GetTempPath() + @"Python\python.exe", Arguments = Path.GetTempPath() + @"Python\tmp.py", UseShellExecute = true });
        }

        private void metroTile1_Click(object sender, EventArgs e) {
            if (Directory.Exists(Path.GetTempPath() + "Python")) {
                Directory.Delete(Path.GetTempPath() + "Python");
            }
            try { Directory.CreateDirectory(Path.GetTempPath() + "Python"); } catch { }
            File.WriteAllBytes(Path.GetTempPath() + "Python.zip", Resources.Python);
            System.IO.Compression.ZipFile.ExtractToDirectory(Path.GetTempPath() + "Python.zip", Path.GetTempPath() + "Python");
        }
        #endregion
    }
}
#endregion