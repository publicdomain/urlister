// <copyright file="MainForm.cs" company="PUblicDomain.com">
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
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Gets or sets the associated icon.
        /// </summary>
        /// <value>The associated icon.</value>
        private Icon associatedIcon;

        /// <summary>
        /// The process.
        /// </summary>
        private Process process = null;

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
            this.moreReleasesPublicDomainGiftcomToolStripMenuItem.Image = this.associatedIcon.ToBitmap();

            // Select default browser in combo box
            this.browserComboBox.SelectedItem = "Default";
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
                $"Made for: nkormanik{Environment.NewLine}DonationCoder.com{Environment.NewLine}Day #1, Week #1 @ Janury 01, 2021",
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
            if (this.intervalNumericUpDown.Value > 0)
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
        }

        /// <summary>
        /// Opens the URL.
        /// </summary>
        /// <param name="url">URL.</param>
        private void OpenUrl(string url)
        {
            // BSet bowser
            string browser = this.browserComboBox.SelectedItem.ToString();

            // Set url by line
            string urlLine = this.urlListtextBox.Lines[(int)this.intervalNumericUpDown.Value - 1];

            // TODO Check if must kill process 
            if (this.closeBrowserToolStripMenuItem.Checked && this.process != null)
            {
                // Close process main window
                this.process.CloseMainWindow();
            }

            // Check for no process
            if (this.process == null)
            {
                // Set new process 
                this.process = new Process();

                // Configure it
                this.process.StartInfo.UseShellExecute = true;
            }

            // Error handling
            try
            {
                // Act on browser
                switch (browser)
                {
                    // Default
                    case "Default":
                        this.process.StartInfo.FileName = $"{urlLine}";
                        break;

                    // Launch Edge
                    case "Edge":
                        this.process.StartInfo.FileName = $"microsoft-edge:{urlLine}";
                        break;

                    // TODO Other specific browsers
                    default:

                        this.process.StartInfo.FileName = $"{browser}.exe";
                        this.process.StartInfo.Arguments = urlLine;
                        break;
                }

                // Start = launch browser
                this.process.Start();
            }
            catch (Exception ex)
            {
                // Advise user
                MessageBox.Show($"Error when launching browser. Message:{Environment.NewLine}{Environment.NewLine}{ex.Message}", "Browser error");
            }
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
        }

        /// <summary>
        /// Handles the main form form closing event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO Add code
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
            // TODO Add code
        }

        /// <summary>
        /// Sets the GUI by loaded settings.
        /// </summary>
        private void SetGuiByLoadedSettings()
        {
            // TODO Add code
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
    }
}
