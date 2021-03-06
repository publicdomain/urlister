﻿// <copyright file="MainForm.cs" company="PUblicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>
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
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using HtmlAgilityPack;
    using Microsoft.VisualBasic;
    using Microsoft.Win32;
    using Newtonsoft.Json;

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
        /// The browser dictionary.
        /// </summary>
        private Dictionary<string, string> browserDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Urlister.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            try
            {
                // The InitializeComponent() call is required for Windows Forms designer support.
                this.InitializeComponent();

                /* Set associated icon */

                // Set associated icon from exe file
                this.associatedIcon = Icon.ExtractAssociatedIcon(typeof(MainForm).GetTypeInfo().Assembly.Location);

                // Set public domain weekly tool strip menu item image
                this.moreReleasesPublicDomainGiftcomToolStripMenuItem.Image = this.associatedIcon.ToBitmap();

                /* Configure */

                // Set default browser path
                this.defaultBrowserPath = this.GetDefaultBrowserPath();

                // Check if set AND it's the first run
                if (string.IsNullOrEmpty(this.defaultBrowserPath))
                {
                    // Use internet explorer as fallback
                    this.defaultBrowserPath = "iexplore.exe";

                    // Check for first run
                    if (!File.Exists(this.urlisterSettingsFilePath))
                    {
                        // Advise user
                        MessageBox.Show($"No default browser path found!{Environment.NewLine}(Using \"iexplore.exe\" as a fallback){Environment.NewLine}{Environment.NewLine}Feel free to add more browsers manually.", "Default browser detection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                /* TODO Load settings [SaveSettings handling can be improved] */

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

                // Check for passed files
                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    // Set file list 
                    var fileList = Environment.GetCommandLineArgs().ToList();

                    // Remove first item (Executable)
                    fileList.RemoveAt(0);

                    // files
                    this.PopulateByFile(fileList);
                }

                // Check if must enable hotkeys
                if (this.enableHotkeysToolStripMenuItem.Checked)
                {
                    // Register HotKeys
                    this.RegisterHotkeys();
                }
            }
            catch (Exception ex)
            {
                // Advise user
                MessageBox.Show($"Error when initializing the program.{Environment.NewLine}{Environment.NewLine}Message:{Environment.NewLine}{ex.Message}", "Initialization error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Registers the hot key.
        /// </summary>
        /// <returns><c>true</c>, if hot key was registered, <c>false</c> otherwise.</returns>
        /// <param name="hWnd">H window.</param>
        /// <param name="id">Identifier.</param>
        /// <param name="fsModifiers">Fs modifiers.</param>
        /// <param name="vk">Vk.</param>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        /// <summary>
        /// Registers the hotkeys.
        /// </summary>
        private void RegisterHotkeys()
        {
            // Register hotkeys
            RegisterHotKey(this.Handle, 0, (int)KeyModifier.Shift, Keys.A.GetHashCode());
            RegisterHotKey(this.Handle, 1, (int)KeyModifier.Shift, Keys.S.GetHashCode());
            RegisterHotKey(this.Handle, 2, (int)KeyModifier.Shift, Keys.D.GetHashCode());
            RegisterHotKey(this.Handle, 3, (int)KeyModifier.Shift, Keys.Q.GetHashCode());
            RegisterHotKey(this.Handle, 4, (int)KeyModifier.Shift, Keys.W.GetHashCode());
        }

        /// <summary>
        /// Unregisters the hot key.
        /// </summary>
        /// <returns><c>true</c>, if hot key was unregistered, <c>false</c> otherwise.</returns>
        /// <param name="hWnd">H window.</param>
        /// <param name="id">Identifier.</param>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Unregisters the hotkeys.
        /// </summary>
        private void UnregisterHotkeys()
        {
            // Unregister hotkeys
            for (int id = 0; id < 5; id++)
            {
                // Unregister current ID
                UnregisterHotKey(this.Handle, id);
            }
        }

        /// <summary>
        /// Key modifier.
        /// </summary>
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        /// <summary>
        /// Windows procedure.
        /// </summary>
        /// <param name="m">The messge.</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Hotkey press
            if (m.Msg == 0x0312)
            {
                // Act on SHIFT + <key>
                switch ((Keys)(((int)m.LParam >> 16) & 0xFFFF))
                {
                    // A = Prev
                    case Keys.A:
                        // Perform back button click
                        this.backButton.PerformClick();

                        break;

                    // S = Next
                    case Keys.S:
                        // Perform next button click
                        this.nextButton.PerformClick();

                        break;

                    // D = Play
                    case Keys.D:
                        // Perform play button click
                        this.playButton.PerformClick();

                        break;

                    // Q = First
                    case Keys.Q:
                        // Perform end button click
                        this.beginButton.PerformClick();

                        break;

                    // W = Last
                    case Keys.W:
                        // Perform end button click
                        this.endButton.PerformClick();

                        break;
                }
            }
        }

        /// <summary>
        /// Handles the new tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNewToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Clear text box
            this.urlListTextBox.Clear();

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
        /// Handles the more releases public domain giftom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMoreReleasesPublicDomainGiftomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open current website
            Process.Start("https://publicdomaingift.com");
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
                $"Made for: nkormanik{Environment.NewLine}DonationCoder.com{Environment.NewLine}Day #90, Week #13 @ March 31, 2021",
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
            // Bounds
            if (this.urlListTextBox.Lines.Length < 1)
            {
                return;
            }

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
            if (this.intervalNumericUpDown.Value > this.urlListTextBox.Lines.Length)
            {
                return;
            }

            // Set url line
            string urlLine = this.urlListTextBox.Lines[(int)this.intervalNumericUpDown.Value - 1];

            // Highlight target line
            this.SelectLine((int)this.intervalNumericUpDown.Value - 1);

            // Open in browser
            this.OpenUrl(urlLine);
        }

        /// <summary>
        /// Handles the next button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNextButtonClick(object sender, EventArgs e)
        {
            // Must ve within range
            if (this.intervalNumericUpDown.Value < this.urlListTextBox.Lines.Length)
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
            this.intervalNumericUpDown.Value = this.urlListTextBox.Lines.Length;

            // Launch
            this.playButton.PerformClick();
        }

        /// <summary>
        /// Handles the URL listtext box text changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnurlListTextBoxTextChanged(object sender, EventArgs e)
        {
            // Update line count
            this.lineCounToolStripStatusLabel.Text = this.urlListTextBox.Lines.Length.ToString();

            // Bound numeric up down. Respect 1 minimum value.
            this.intervalNumericUpDown.Maximum = this.urlListTextBox.Lines.Length == 0 ? 1 : this.urlListTextBox.Lines.Length;
        }

        /// <summary>
        /// Gets the default browser path.
        /// </summary>
        /// <returns>The default browser path.</returns>
        private string GetDefaultBrowserPath()
        {
            // Declare default browser path variable
            string defaultBrowserPathCleaned = string.Empty;

            //# TODO this whole block can be improved... [i.e. better default browser detection]
            try
            {
                // Declare registry key
                RegistryKey registryKey = null;

                // Set registry key
                for (int i = 0; i < 3; i++)
                {
                    // Empty cleaned path variable 
                    defaultBrowserPathCleaned = string.Empty;

                    switch (i)
                    {
                        case 0:
                            try
                            {
                                var userChoice = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.htm\UserChoice", false);
                                var progId = userChoice.GetValue("ProgId");
                                registryKey = Registry.ClassesRoot.OpenSubKey(progId + @"\shell\open\command", false);
                            }
                            catch
                            {
                                // Next iteration
                                continue;
                            }
                            break;
                        case 1:
                            try
                            {
                                registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false);
                            }
                            catch
                            {
                                // Next iteration
                                continue;
                            }
                            break; ;
                        case 2:
                            try
                            {
                                registryKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);
                            }
                            catch
                            {
                                // Exit switch
                                break;
                            }
                            break;
                    }

                    // Check there's something to work with
                    if (registryKey != null)
                    {
                        // Set default browser path withi no quotes and no parameters
                        defaultBrowserPathCleaned = registryKey.GetValue(null).ToString().Replace("\"", string.Empty).Split(new string[] { "exe" }, 2, StringSplitOptions.RemoveEmptyEntries)[0] + "exe";

                        // Close registry key
                        registryKey.Close();
                    }
                }
            }
            catch
            {
                // On error, empty string
                return string.Empty;
            }

            // Return default browsers path
            return defaultBrowserPathCleaned;
        }

        /// <summary>
        /// Opens the URL.
        /// </summary>
        /// <param name="url">URL string.</param>
        private void OpenUrl(string url)
        {
            // Set browser
            string browser = this.browserComboBox.SelectedItem.ToString();

            // Set url by line
            string urlLine = this.urlListTextBox.Lines[(int)this.intervalNumericUpDown.Value - 1];

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

                    // Empty process nme
                    this.processName = string.Empty;

                }
                catch (Exception ex)
                {
                    // Advise user
                    MessageBox.Show($"Error when closing the browser.{Environment.NewLine}{Environment.NewLine}Message:{Environment.NewLine}{ex.Message}", "Browser closing error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Error handling
            try
            {
                // Set new process 
                Process process = new Process();

                // Act on browser
                switch (browser)
                {
                    // Default
                    case "Default":

                        // Set file name 
                        process.StartInfo.FileName = this.defaultBrowserPath;

                        break;

                    /*// Launch Edge
                    case "Edge":
                        this.process.StartInfo.FileName = $"microsoft-edge:{urlLine}";

                        break;*/

                    // Custom browsers
                    default:

                        // Set file name
                        process.StartInfo.FileName = this.browserDictionary[browser];

                        break;
                }

                // Set target URL
                process.StartInfo.Arguments = urlLine;

                // Start process
                process.Start();

                // Wait for GUI to load
                process.WaitForInputIdle();

                // Set process name
                this.processName = process.ProcessName;
            }
            catch (Exception ex)
            {
                // Advise user
                MessageBox.Show($"Error when launching browser.{Environment.NewLine}{Environment.NewLine}Message:{Environment.NewLine}{ex.Message}", "Browser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Selects the line.
        /// </summary>
        /// <param name="lineNumber">Line number.</param>
        private void SelectLine(int lineNumber)
        {
            // Select line by number
            string selectedText = this.urlListTextBox.Lines[lineNumber];
            int selectedTextPos = this.urlListTextBox.Text.IndexOf(selectedText, StringComparison.InvariantCulture);
            int selectedTextLen = selectedText.Length;
            this.urlListTextBox.Focus();
            this.urlListTextBox.Select(selectedTextPos, selectedTextLen);
            this.urlListTextBox.ScrollToCaret();
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

            // Process hotkeys
            if (this.enableHotkeysToolStripMenuItem.Checked)
            {
                this.RegisterHotkeys();
            }
            else
            {
                this.UnregisterHotkeys();
            }
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

            // Check if must  Unregister the hotkeys
            if (this.enableHotkeysToolStripMenuItem.Checked)
            {
                this.UnregisterHotkeys();
            }
        }

        /// <summary>
        /// Handles the open tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Prepare dialog properties 
            this.openFileDialog.Title = "Open file(s) with URLs";
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Filter = "TXT Files|*.txt|HTML Files|*.htm;*.html|All files (*.*)|*.*";

            // Show open file dialog
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Populate by opened file(s)
                    this.PopulateByFile(new List<string>(this.openFileDialog.FileNames));

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
            if (this.urlListTextBox.Text.Length > 0 && this.saveFileDialog.ShowDialog() == DialogResult.OK && this.saveFileDialog.FileName.Length > 0)
            {
                try
                {
                    // Update settings by GUI values
                    this.UpdateSettingsByGui();

                    // Save URL list to file
                    File.WriteAllText(this.saveFileDialog.FileName, this.urlListTextBox.Text);
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
            if (this.browserComboBox.Text.Length > 0 && this.browserComboBox.SelectedItem.ToString() != "Add new...")
            {
                this.urlisterSettings.Browser = this.browserComboBox.SelectedItem.ToString();
            }

            // Browsers
            if (this.browserDictionary.Count > 0)
            {
                this.urlisterSettings.Browsers = JsonConvert.SerializeObject(this.browserDictionary, Formatting.Indented);
            }
            else
            {
                // Clear browsers
                this.urlisterSettings.Browsers = string.Empty;
            }

            // Hotkeys
            this.urlisterSettings.EnableHotkeys = this.enableHotkeysToolStripMenuItem.Checked;

            // Line
            this.urlisterSettings.Line = (int)this.intervalNumericUpDown.Value;

            // URLs
            this.urlisterSettings.Urls = this.urlListTextBox.Lines;
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

            // Clear combo box
            this.browserComboBox.Items.Clear();

            // Add new & default
            this.browserComboBox.Items.AddRange(new object[] {
                "Add new...",
                "Default"
            });

            // Set browsers
            if (this.urlisterSettings.Browsers.Length > 0)
            {
                // Set browser dictionary
                this.browserDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.urlisterSettings.Browsers);

                // Add saved browsers
                foreach (var browser in this.browserDictionary)
                {
                    // Add to combo box
                    this.browserComboBox.Items.Add(browser.Key);
                }
            }

            // Browser
            this.browserComboBox.SelectedItem = this.urlisterSettings.Browser;

            // Hotkeys
            this.enableHotkeysToolStripMenuItem.Checked = this.urlisterSettings.EnableHotkeys;

            // URLs
            this.urlListTextBox.Lines = this.urlisterSettings.Urls;

            // Line
            this.intervalNumericUpDown.Value = this.urlisterSettings.Line;
        }

        /// <summary>
        /// Handles the cut tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Check length
            if (this.urlListTextBox.TextLength > 0)
            {
                // Copy to clipboard
                Clipboard.SetText(this.urlListTextBox.Text);

                // Clear text box
                this.urlListTextBox.Clear();
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
            if (this.urlListTextBox.TextLength > 0)
            {
                // Copy to clipboard
                Clipboard.SetText(this.urlListTextBox.Text);
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
            this.urlListTextBox.Text += Clipboard.GetText();
        }

        /// <summary>
        /// Handles the delete tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Clear text box
            this.urlListTextBox.Clear();
        }

        /// <summary>
        /// Handles the select all tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Select text box
            this.urlListTextBox.SelectAll();
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
        private void OnurlListTextBoxDragDrop(object sender, DragEventArgs e)
        {
            // Populate URL list by dropped files
            this.PopulateByFile(new List<string>((IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop)));
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
        /// Populates the URL list by file.
        /// </summary>
        /// <param name="filePathList">File path list.</param>
        private void PopulateByFile(List<string> filePathList)
        {
            try
            {
                // Declare link list
                var linkList = new List<string>();

                // Iterate dropped files
                foreach (string droppedFile in filePathList)
                {
                    // Process extensions
                    switch (Path.GetExtension(droppedFile).ToLowerInvariant())
                    {
                        // TEXT
                        case ".txt":

                            // Add valid links to list
                            linkList.AddRange(this.ProcessTextFile(droppedFile));

                            // Halt flow
                            break;

                        // HTML
                        case ".htm":
                        case ".html":

                            // Add valid links to list
                            linkList.AddRange(this.ProcessHtmlFile(droppedFile));

                            // Halt flow
                            break;

                        // URL
                        case ".url":

                            // Add valid links to list
                            linkList.AddRange(this.ProcesUrlFile(droppedFile));

                            // Halt flow
                            break;
                    }
                }

                // Check for prev content
                if (this.urlListTextBox.TextLength > 0)
                {
                    // Insert newline
                    this.urlListTextBox.Text += Environment.NewLine;
                }

                // Append dropped links
                this.urlListTextBox.Text += string.Join(Environment.NewLine, linkList);
            }
            catch (Exception ex)
            {
                // Inform user
                MessageBox.Show($"Could not finish operation:{Environment.NewLine}{ex.Message}", "Populate by file(s) error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Processes the text file.
        /// </summary>
        /// <returns>The link list.</returns>
        /// <param name="filePath">File path.</param>
        private List<string> ProcessTextFile(string filePath)
        {
            // Set link list 
            var linkList = new List<string>();

            // Declare trimmed line
            string trimmedline = string.Empty;

            // Iterate lines
            foreach (string line in File.ReadAllLines(filePath))
            {
                // Set trimmed line
                trimmedline = line.Trim();

                // Validate current line
                if (this.ValidateUri(trimmedline))
                {
                    // Add valid URI
                    linkList.Add(trimmedline);
                }
            }

            // Return valid link list
            return linkList;
        }

        /// <summary>
        /// Processes the html file.
        /// </summary>
        /// <returns>The links list.</returns>
        /// <param name="filePath">File path.</param>
        private List<string> ProcessHtmlFile(string filePath)
        {
            // Set document
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

            // Load current dropped file
            htmlDocument.Load(filePath);

            // Set link list 
            var linkList = new List<string>();

            // Extract links
            foreach (HtmlNode link in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
            {
                // Set attribute
                HtmlAttribute htmlAttribute = link.Attributes["href"];

                // Check
                if (htmlAttribute.Value.Contains("a") && this.ValidateUri(htmlAttribute.Value))
                {
                    // Add to link list 
                    linkList.Add(htmlAttribute.Value);
                }
            }

            // Return validlink list
            return linkList;
        }

        /// <summary>
        /// TODO Proceses the URL file. [May yconsider returning string. Returns list for uniformity purposes with TXT and HTML processing functions]
        /// </summary>
        /// <returns>The extracted URL.</returns>
        /// <param name="filePath">File path.</param>
        private List<string> ProcesUrlFile(string filePath)
        {
            // Set link list 
            var linkList = new List<string>();

            // Extracted link
            var link = string.Empty;

            // TODO Iterate lines [Can be done via indexOf]
            foreach (var line in File.ReadAllLines(filePath))
            {
                // Check for "URL="
                if (line.StartsWith("URL=", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Extract link
                    link = line.Split(new char[] { '=' })[1];

                    // Add to link list
                    linkList.Add(link);

                    // Halt flow
                    break;
                }
            }

            // Return extracted link list
            return linkList;
        }

        /// <summary>
        /// Handles the URL listtext box drag enter event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnurlListTextBoxDragEnter(object sender, DragEventArgs e)
        {
            // Set effect
            e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Handles the browser combo box key down event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBrowserComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            // Check for DELETE key
            if (e.KeyCode == Keys.Delete)
            {

                // Set item text
                string itemText = this.browserComboBox.GetItemText(this.browserComboBox.SelectedItem);

                // Ensure only custom browsers are processed and user confirms
                if (itemText == "Add new..." || itemText == "Default" || MessageBox.Show($"Would you like to remove \"{itemText}\"?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    // Halt flow
                    return;
                }

                // Prevent GUI updates
                this.browserComboBox.BeginUpdate();

                // Remove from browser dictionary
                this.browserDictionary.Remove(itemText);

                // Remove browser item
                this.browserComboBox.Items.RemoveAt(this.browserComboBox.SelectedIndex);

                // Set combo box to default
                this.browserComboBox.SelectedIndex = 1;

                // Update settings
                this.UpdateSettingsByGui();

                // TODO Save settings to disk [Check if it's more convenient to simply let FormClosing take care of it]
                this.SaveSettingsFile(this.urlisterSettingsFilePath);

                // ResumeGUI updates
                this.browserComboBox.EndUpdate();
            }
        }

        /// <summary>
        /// Handles the browser combo box selected index changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBrowserComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if add new is selected
            if (this.browserComboBox.GetItemText(this.browserComboBox.SelectedItem) == "Add new...")
            {
                // Prepare dialog properties 
                this.openFileDialog.Title = "Choose browser executable";
                this.openFileDialog.Multiselect = false;
                this.openFileDialog.Filter = "EXE Files (*.exe)|*.exe|All files (*.*)|*.*";

                // Show open file dialog
                if (this.openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Allow user to set the display name
                        string browserName = Interaction.InputBox("Set new browser display name:", "Browser name", Path.GetFileNameWithoutExtension(this.openFileDialog.SafeFileName));

                        // Check there is a name
                        if (browserName.Length > 0)
                        {
                            // Check for a previous dictionary entry
                            if (this.browserDictionary.ContainsKey(browserName))
                            {
                                // Update browser
                                this.browserDictionary[browserName] = this.openFileDialog.FileName;
                            }
                            else
                            {
                                // Add new browser
                                this.browserDictionary.Add(browserName, this.openFileDialog.FileName);
                            }
                        }

                        // Add to combo box
                        this.browserComboBox.Items.Add(browserName);
                    }
                    catch (Exception exception)
                    {
                        // Inform user
                        MessageBox.Show($"Error when opening \"{Path.GetFileName(this.openFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Open file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Set combo box to default
                this.browserComboBox.SelectedText = "Default";
            }
        }
    }
}
