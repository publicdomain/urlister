// <copyright file="MainForm.cs" company="PUblicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>
using System.Threading;
using System.Globalization;
using System.Text;

namespace Urlister
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using HtmlAgilityPack;
    using Microsoft.Win32;

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Gets or sets the associated icon.
        /// </summary>
        /// <value>The associated icon.</value>
        private Icon associatedIcon = null;

        /// <summary>
        /// The process.
        /// </summary>
        private Process process = new Process();

        /// <summary>
        /// The name of the process.
        /// </summary>
        private string processName = string.Empty;

        /// <summary>
        /// The default browser path.
        /// </summary>
        private string defaultBrowserPath = string.Empty;

        /// <summary>
        /// The urlister settings.
        /// </summary>
        private UrlisterSettings urlisterSettings = new UrlisterSettings();

        /// <summary>
        /// The urlister settings file path.
        /// </summary>
        private string urlisterSettingsFilePath = "UrlisterSettings.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Urlister.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            InitializeComponent();

            /* Set associated icon */

            // Set associated icon from exe file
            this.associatedIcon = Icon.ExtractAssociatedIcon(typeof(MainForm).GetTypeInfo().Assembly.Location);

            // Set public domain weekly tool strip menu item image
            this.dailyReleasesPublicDomainDailycomToolStripMenuItem.Image = this.associatedIcon.ToBitmap();

            /* Configure */

            // Set default browser path
            this.defaultBrowserPath = this.GetDefaultBrowserPath();

            // Check if set
            if (string.IsNullOrEmpty(this.defaultBrowserPath))
            {
                // Advise user
                MessageBox.Show("No default browser path found!");

                // Remove default browser path from combo box
                this.browserComboBox.Items.RemoveAt(0);
            }

            /* TODO Load settings [SaveSettings handling can be improved]*/

            // Check for settings file
            if (!File.Exists(this.urlisterSettingsFilePath))
            {
                // Create new settings file
                this.SaveSettingsFile(this.urlisterSettingsFilePath);
            }

            // Load settings from disk
            this.urlisterSettings = this.LoadSettingsFile(this.urlisterSettingsFilePath);

            // Set GUI
            this.SetGuiByLoadedSettings();

            // Set top most
            this.TopMost = this.alwaysOnTopToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Handles the new tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNewToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Clear text box
            this.urlListtextBox.Clear();

            // Reset lines
            this.intervalNumericUpDown.Value = 1;
        }

        /// <summary>
        /// Handles the exit tool strip menu item1 click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItem1Click(object sender, EventArgs e)
        {
            // Close program
            this.Close();
        }

        /// <summary>
        /// Handles the weekly releases public domain weeklycom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMoreReleasesPublicDomainGiftomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open current website
            Process.Start("https://publicdomaindaily.com");
        }

        /// <summary>
        /// Handles the original thread donation codercom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open original thread @ DonationCoder
            Process.Start("https://www.donationcoder.com/forum/index.php?topic=34285");
        }

        /// <summary>
        /// Handles the source code githubcom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open GitHub repository
            Process.Start("https://github.com/publicdomain/urlister");
        }

        /// <summary>
        /// Handles the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Set license text
            var licenseText = $"CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication{Environment.NewLine}" +
                $"https://creativecommons.org/publicdomain/zero/1.0/legalcode{Environment.NewLine}{Environment.NewLine}" +
                $"Libraries and icons have separate licenses.{Environment.NewLine}{Environment.NewLine}" +
                $"Pencil icon by Clker-Free-Vector-Images - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/vectors/pencil-marks-notes-agenda-list-308509/{Environment.NewLine}{Environment.NewLine}" +
                $"Pencil icon by Clker-Free-Vector-Images - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/vectors/pencil-marks-notes-agenda-list-308509/{Environment.NewLine}{Environment.NewLine}" +
                $"Patreon icon used according to published brand guidelines{Environment.NewLine}" +
                $"https://www.patreon.com/brand{Environment.NewLine}{Environment.NewLine}" +
                $"GitHub mark icon used according to published logos and usage guidelines{Environment.NewLine}" +
                $"https://github.com/logos{Environment.NewLine}{Environment.NewLine}" +
                $"DonationCoder icon used with permission{Environment.NewLine}" +
                $"https://www.donationcoder.com/forum/index.php?topic=48718{Environment.NewLine}{Environment.NewLine}" +
                $"PublicDomain icon is based on the following source images:{Environment.NewLine}{Environment.NewLine}" +
                $"Bitcoin by GDJ - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/vectors/bitcoin-digital-currency-4130319/{Environment.NewLine}{Environment.NewLine}" +
                $"Letter P by ArtsyBee - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/illustrations/p-glamour-gold-lights-2790632/{Environment.NewLine}{Environment.NewLine}" +
                $"Letter D by ArtsyBee - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/illustrations/d-glamour-gold-lights-2790573/{Environment.NewLine}{Environment.NewLine}";

            // Set title
            string programTitle = typeof(MainForm).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;

            // Set version for generating semantic version 
            Version version = typeof(MainForm).GetTypeInfo().Assembly.GetName().Version;

            // Set about form
            var aboutForm = new AboutForm(
                $"About {programTitle}",
                $"{programTitle} {version.Major}.{version.Minor}.{version.Build}",
                $"Made for: nkormanik{Environment.NewLine}DonationCoder.com{Environment.NewLine}Day #32, Week #05 @ February 01, 2021",
                licenseText,
                this.Icon.ToBitmap())
            {

                // Set about form icon
                Icon = this.associatedIcon
            };

            // Show about form
            aboutForm.ShowDialog();
        }

        /// <summary>
        /// Handles the begin button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBeginButtonClick(object sender, EventArgs e)
        {
            // First line number
            this.intervalNumericUpDown.Value = 1;

            // Launch
            this.playButton.PerformClick();
        }

        /// <summary>
        /// Handles the back button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBackButtonClick(object sender, EventArgs e)
        {
            // Take one from line number
            if (this.intervalNumericUpDown.Value > 1)
            {
                this.intervalNumericUpDown.Value -= 1;

                // Launch
                this.playButton.PerformClick();
            }
        }

        /// <summary>
        /// Handles the play button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPlayButtonClick(object sender, EventArgs e)
        {
            // Bounds
            if (this.intervalNumericUpDown.Value > this.urlListtextBox.Lines.Length)
            {
                return;
            }

            // Set url line
            string urlLine = this.urlListtextBox.Lines[(int)this.intervalNumericUpDown.Value - 1];

            // Highlight target line
            this.SelectLine((int)this.intervalNumericUpDown.Value - 1);

            // Open in browser
            OpenUrl(urlLine);
        }

        /// <summary>
        /// Handles the next button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNextButtonClick(object sender, EventArgs e)
        {
            // Must ve within range
            if (this.intervalNumericUpDown.Value < this.urlListtextBox.Lines.Length)
            {
                // Add one to line number
                this.intervalNumericUpDown.Value += 1;

                // Launch
                this.playButton.PerformClick();
            }
        }

        /// <summary>
        /// Handles the end button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnEndButtonClick(object sender, EventArgs e)
        {
            // Last line number
            this.intervalNumericUpDown.Value = this.urlListtextBox.Lines.Length;

            // Launch
            this.playButton.PerformClick();
        }

        /// <summary>
        /// Handles the URL listtext box text changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUrlListtextBoxTextChanged(object sender, EventArgs e)
        {
            // Update line count
            this.lineCounToolStripStatusLabel.Text = this.urlListtextBox.Lines.Length.ToString();

            // Bound numeric up down. Respect 1 minimum value.
            this.intervalNumericUpDown.Maximum = this.urlListtextBox.Lines.Length == 0 ? 1 : this.urlListtextBox.Lines.Length;
        }

        /// <summary>
        /// Opens the URL.
        /// </summary>
        /// <param name="url">URL.</param>
        private void OpenUrl(string url)
        {
            // Set browser
            string browser = this.browserComboBox.SelectedItem.ToString();

            // Set url by line
            string urlLine = this.urlListtextBox.Lines[(int)this.intervalNumericUpDown.Value - 1];

            // TODO Check if must kill process 
            if (this.closeBrowserToolStripMenuItem.Checked && !string.IsNullOrEmpty(this.processName))
            {
                try
                {
                    // TODO Iterate all [Can be more specific]
                    foreach (var browserProcess in Process.GetProcessesByName(this.processName))
                    {
                        // Close gracefully
                        browserProcess.CloseMainWindow();
                    }
                }
                catch (Exception ex)
                {
                    // Advise user
                    MessageBox.Show($"Error when cloding browser. Message:{Environment.NewLine}{Environment.NewLine}{ex.Message}", "Browser closing error");
                }
            }

            // Error handling
            try
            {
                // Act on browser
                switch (browser)
                {
                    // Default
                    case "Default":
                        //this.process.StartInfo.FileName = this.defaultBrowserPath;
                        this.process.StartInfo.FileName = urlLine;
                        //this.process.StartInfo.Arguments = urlLine;
                        break;

                    // Launch Edge
                    case "Edge":
                        this.process.StartInfo.FileName = $"microsoft-edge:{urlLine}";
                        break;

                    // TODO Other specific browsers matching name
                    default:
                        /*this.process.StartInfo.FileName = $"{browser.ToLower()}.exe";
                        this.process.StartInfo.Arguments = urlLine;*/
                        break;
                }

                // Start process
                this.process.Start();

                // Wait for GUI to load
                this.process.WaitForInputIdle();

                // Set process name
                this.processName = this.process.ProcessName;

                //#
                MessageBox.Show(this.processName);

            }
            catch (Exception ex)
            {
                // Advise user
                MessageBox.Show($"Error when launching browser. Message:{Environment.NewLine}{Environment.NewLine}{ex.Message}", "Browser error");
            }
        }

        /// <summary>
        /// Gets the default browser path.
        /// </summary>
        /// <returns>The default browser path.</returns>
        private string GetDefaultBrowserPath()
        {
            // Declare default browser path variable
            string defaultBrowserPathCleaned = string.Empty;

            try
            {
                // Declare registry key
                RegistryKey registryKey = null;

                // Set registry key
                for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            registryKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);
                            break;
                        case 1:
                            registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false);
                            break;
                        case 2:
                            registryKey = Registry.ClassesRoot.OpenSubKey(Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.htm\UserChoice", false).GetValue("ProgId") + @"\shell\open\command", false);
                            break;
                    }

                    if (registryKey != null)
                    {
                        // Exit for
                        break;
                    }
                }

                // Check there's something to work with
                if (registryKey != null)
                {
                    // Set default browser path withi no quotes and no parameters
                    defaultBrowserPathCleaned = registryKey.GetValue(null).ToString().Replace("\"", "").Split(new string[] { "exe" }, 21, StringSplitOptions.RemoveEmptyEntries)[0] + "exe";

                    //Close registry key
                    registryKey.Close();
                }

            }
            catch
            {
                // On error, empty string
                return string.Empty;
            }

            //Return default browsers path
            return defaultBrowserPathCleaned;
        }

        /// <summary>
        /// Selects the line.
        /// </summary>
        /// <param name="lineNumber">Line number.</param>
        private void SelectLine(int lineNumber)
        {
            // Select line by number
            string SelectedText = this.urlListtextBox.Lines[lineNumber];
            int SelectedTextPos = this.urlListtextBox.Text.IndexOf(SelectedText, StringComparison.InvariantCulture);
            int SelectedTextLen = SelectedText.Length;
            this.urlListtextBox.Focus();
            this.urlListtextBox.Select(SelectedTextPos, SelectedTextLen);
            this.urlListtextBox.ScrollToCaret();
        }

        /// <summary>
        /// Handles the interval numeric up down key press event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnIntervalNumericUpDownKeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for enter
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                // Hit play button
                this.playButton.PerformClick();
            }
        }

        /// <summary>
        /// Handles the options tool strip menu item drop down item clicked event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOptionsToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Set tool strip menu item
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            // Toggle checked
            toolStripMenuItem.Checked = !toolStripMenuItem.Checked;

            // Set topmost by check box
            this.TopMost = this.alwaysOnTopToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Handles the main form form closing event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            // Update settings by current GUI
            this.UpdateSettingsByGui();

            // Save to disk
            this.SaveSettingsFile(this.urlisterSettingsFilePath);
        }

        /// <summary>
        /// Handles the open tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Show open file dialog
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Load from disk
                    this.urlListtextBox.Text = File.ReadAllText(this.openFileDialog.FileName);

                    // Set GUI
                    this.SetGuiByLoadedSettings();

                    // Reset to line #1
                    this.intervalNumericUpDown.ResetText();

                    // Select first line
                    this.SelectLine(0);
                }
                catch (Exception exception)
                {
                    // Inform user
                    MessageBox.Show($"Error when opening \"{Path.GetFileName(this.openFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Open file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles the save tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open save file dialog
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK && this.saveFileDialog.FileName.Length > 0)
            {
                try
                {
                    // Update settings by GUI values
                    this.UpdateSettingsByGui();

                    // Save URL list to file
                    File.WriteAllText(this.saveFileDialog.FileName, this.urlListtextBox.Text);
                }
                catch (Exception exception)
                {
                    // Inform user
                    MessageBox.Show($"Error when saving to \"{Path.GetFileName(this.saveFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Save file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Inform user
                MessageBox.Show($"Saved file to \"{Path.GetFileName(this.saveFileDialog.FileName)}\"", "Settings file saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Updates the settings by GUI.
        /// </summary>
        private void UpdateSettingsByGui()
        {
            // Always on top
            this.urlisterSettings.AlwaysOnTop = this.alwaysOnTopToolStripMenuItem.Checked;

            // Close browser
            this.urlisterSettings.CloseBrowser = this.closeBrowserToolStripMenuItem.Checked;

            // Browser
            if (this.browserComboBox.Text.Length > 0)
            {
                this.urlisterSettings.Browser = this.browserComboBox.SelectedItem.ToString();
            }

            // Line
            this.urlisterSettings.Line = (int)this.intervalNumericUpDown.Value;

            // URLs
            this.urlisterSettings.Urls = this.urlListtextBox.Lines;
        }

        /// <summary>
        /// Sets the GUI by loaded settings.
        /// </summary>
        private void SetGuiByLoadedSettings()
        {
            // Always on top
            this.alwaysOnTopToolStripMenuItem.Checked = this.urlisterSettings.AlwaysOnTop;

            // Close browser
            this.closeBrowserToolStripMenuItem.Checked = this.urlisterSettings.CloseBrowser;

            // Browser
            this.browserComboBox.SelectedItem = this.urlisterSettings.Browser;

            // Line
            this.intervalNumericUpDown.Value = this.urlisterSettings.Line;

            // URLs
            this.urlListtextBox.Lines = this.urlisterSettings.Urls;
        }

        /// <summary>
        /// Handles the cut tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Check length
            if (this.urlListtextBox.TextLength > 0)
            {
                // Copy to clipboard
                Clipboard.SetText(this.urlListtextBox.Text);

                // Clear text box
                this.urlListtextBox.Clear();
            }
        }

        /// <summary>
        /// Handles the copy tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Check length
            if (this.urlListtextBox.TextLength > 0)
            {
                // Copy to clipboard
                Clipboard.SetText(this.urlListtextBox.Text);
            }
        }

        /// <summary>
        /// Handles the paste tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Append clipboard text
            this.urlListtextBox.Text += Clipboard.GetText();
        }

        /// <summary>
        /// Handles the delete tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Clear text box
            this.urlListtextBox.Clear();
        }

        /// <summary>
        /// Handles the select all tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Select text box
            this.urlListtextBox.SelectAll();
        }

        /// <summary>
        /// Loads the settings file.
        /// </summary>
        /// <returns>The settings file.</returns>
        /// <param name="settingsFilePath">Settings file path.</param>
        private UrlisterSettings LoadSettingsFile(string settingsFilePath)
        {
            // Use file stream
            using (FileStream fileStream = File.OpenRead(settingsFilePath))
            {
                // Set xml serialzer
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UrlisterSettings));

                // Return populated settings data
                return xmlSerializer.Deserialize(fileStream) as UrlisterSettings;
            }
        }

        /// <summary>
        /// Saves the settings file.
        /// </summary>
        /// <param name="settingsFilePath">Settings file path.</param>
        private void SaveSettingsFile(string settingsFilePath)
        {
            try
            {
                // Use stream writer
                using (StreamWriter streamWriter = new StreamWriter(settingsFilePath, false))
                {
                    // Set xml serialzer
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(UrlisterSettings));

                    // Serialize settings data
                    xmlSerializer.Serialize(streamWriter, this.urlisterSettings);
                }
            }
            catch (Exception exception)
            {
                // Advise user
                MessageBox.Show($"Error saving settings file.{Environment.NewLine}{Environment.NewLine}Message:{Environment.NewLine}{exception.Message}", "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the URL listtext box drag drop event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUrlListtextBoxDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // Iterate dropped files
                foreach (string droppedFile in new List<string>((IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop)))
                {
                    // Check for .txt
                    if (droppedFile.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Validate links

                        // Append contents
                        this.urlListtextBox.Text += File.ReadAllText(droppedFile);
                    }
                    else
                    {
                        // Set document
                        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

                        // Load current dropped file
                        htmlDocument.Load(droppedFile);

                        // Set string 
                        StringBuilder linkLines = new StringBuilder();

                        // Extract links
                        foreach (HtmlNode link in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
                        {
                            // Set attribute
                            HtmlAttribute htmlAttribute = link.Attributes["href"];

                            // Check
                            if (htmlAttribute.Value.Contains("a"))
                            {
                                // Add to text box
                                linkLines.AppendLine(htmlAttribute.Value); ;
                            }
                        }

                        // Append link lines
                        this.urlListtextBox.Text += linkLines.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Inform user
                MessageBox.Show($"Could not finish operation:{Environment.NewLine}{ex.Message}", "Drag & Drop error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validates the URI.
        /// </summary>
        /// <returns><c>true</c>, if URI was validated, <c>false</c> otherwise.</returns>
        /// <param name="possibleUri">Possible URI.</param>
        private bool ValidateUri(string possibleUri)
        {
            // Return TryCreate result
            return Uri.TryCreate(possibleUri, UriKind.Absolute, out var uri) &&
                            (uri.Scheme == Uri.UriSchemeHttps ||
                            uri.Scheme == Uri.UriSchemeHttp ||
                            uri.Scheme == Uri.UriSchemeFtp ||
                            uri.Scheme == Uri.UriSchemeMailto ||
                            uri.Scheme == Uri.UriSchemeFile ||
                            uri.Scheme == Uri.UriSchemeNews ||
                            uri.Scheme == Uri.UriSchemeNntp ||
                            uri.Scheme == Uri.UriSchemeGopher ||
                            uri.Scheme == Uri.UriSchemeNetTcp ||
                            uri.Scheme == Uri.UriSchemeNetPipe);
        }

        /// <summary>
        /// Handles the URL listtext box drag enter event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUrlListtextBoxDragEnter(object sender, DragEventArgs e)
        {
            // Set effect
            e.Effect = DragDropEffects.Copy;
        }
    }
}
