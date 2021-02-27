[1mdiff --git a/Urlister/Urlister/MainForm.cs b/Urlister/Urlister/MainForm.cs[m
[1mindex 633eda6..8ff1d7f 100644[m
[1m--- a/Urlister/Urlister/MainForm.cs[m
[1m+++ b/Urlister/Urlister/MainForm.cs[m
[36m@@ -514,15 +514,8 @@[m [mnamespace Urlister[m
             {[m
                 try[m
                 {[m
[31m-                    // Check for prev content[m
[31m-                    if (this.urlListTextBox.TextLength > 0)[m
[31m-                    {[m
[31m-                        // Insert newline[m
[31m-                        this.urlListTextBox.Text += Environment.NewLine;[m
[31m-                    }[m
[31m-[m
[31m-                    // Load from disk[m
[31m-                    this.urlListTextBox.Text += File.ReadAllText(this.openFileDialog.FileName);[m
[32m+[m[32m                    // Populate by opened file(s)[m
[32m+[m[32m                    this.PopulateByFile(new List<string>(this.openFileDialog.FileNames));[m
 [m
                     // Set GUI[m
                     this.SetGuiByLoadedSettings();[m
[36m@@ -730,14 +723,45 @@[m [mnamespace Urlister[m
         /// <param name="sender">Sender object.</param>[m
         /// <param name="e">Event arguments.</param>[m
         private void OnurlListTextBoxDragDrop(object sender, DragEventArgs e)[m
[32m+[m[32m        {[m
[32m+[m[32m            // Populate URL list by dropped files[m
[32m+[m[32m            this.PopulateByFile(new List<string>((IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop)));[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        /// <summary>[m
[32m+[m[32m        /// Validates the URI.[m
[32m+[m[32m        /// </summary>[m
[32m+[m[32m        /// <returns><c>true</c>, if URI was validated, <c>false</c> otherwise.</returns>[m
[32m+[m[32m        /// <param name="possibleUri">Possible URI.</param>[m
[32m+[m[32m        private bool ValidateUri(string possibleUri)[m
[32m+[m[32m        {[m
[32m+[m[32m            // Return TryCreate result[m
[32m+[m[32m            return Uri.TryCreate(possibleUri, UriKind.Absolute, out var uri) &&[m
[32m+[m[32m                            (uri.Scheme == Uri.UriSchemeHttps ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeHttp ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeFtp ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeMailto ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeFile ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeNews ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeNntp ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeGopher ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeNetTcp ||[m
[32m+[m[32m                            uri.Scheme == Uri.UriSchemeNetPipe);[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        /// <summary>[m
[32m+[m[32m        /// Populates the URL list by file.[m
[32m+[m[32m        /// </summary>[m
[32m+[m[32m        /// <param name="filePathList">File path list.</param>[m
[32m+[m[32m        private void PopulateByFile(List<string> filePathList)[m
         {[m
             try[m
             {[m
[31m-                // Declare dropped links[m
[31m-                string droppedLinks = string.Empty;[m
[32m+[m[32m                // Declare links[m
[32m+[m[32m                string links = string.Empty;[m
 [m
                 // Iterate dropped files[m
[31m-                foreach (string droppedFile in new List<string>((IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop)))[m
[32m+[m[32m                foreach (string droppedFile in filePathList)[m
                 {[m
                     // Process extensions[m
                     switch (Path.GetExtension(droppedFile).ToLowerInvariant())[m
[36m@@ -746,7 +770,7 @@[m [mnamespace Urlister[m
                         case ".txt":[m
 [m
                             // Append valid link lines[m
[31m-                            droppedLinks += this.ProcessTextFile(droppedFile);[m
[32m+[m[32m                            links += this.ProcessTextFile(droppedFile);[m
 [m
                             // Halt flow[m
                             break;[m
[36m@@ -756,7 +780,7 @@[m [mnamespace Urlister[m
                         case ".html":[m
 [m
                             // Append valid links[m
[31m-                            droppedLinks += this.ProcessHtmlFile(droppedFile);[m
[32m+[m[32m                            links += this.ProcessHtmlFile(droppedFile);[m
 [m
                             // Halt flow[m
                             break;[m
[36m@@ -765,7 +789,7 @@[m [mnamespace Urlister[m
                         case ".url":[m
 [m
                             // Append extracted link[m
[31m-                            droppedLinks += this.ProcesUrlFile(droppedFile);[m
[32m+[m[32m                            links += this.ProcesUrlFile(droppedFile);[m
 [m
                             // Halt flow[m
                             break;[m
[36m@@ -780,36 +804,15 @@[m [mnamespace Urlister[m
                 }[m
 [m
                 // Append dropped links[m
[31m-                this.urlListTextBox.Text += droppedLinks;[m
[32m+[m[32m                this.urlListTextBox.Text += links;[m
             }[m
             catch (Exception ex)[m
             {[m
                 // Inform user[m
[31m-                MessageBox.Show($"Could not finish operation:{Environment.NewLine}{ex.Message}", "Drag & Drop error", MessageBoxButtons.OK, MessageBoxIcon.Error);[m
[32m+[m[32m                MessageBox.Show($"Could not finish operation:{Environment.NewLine}{ex.Message}", "Populate by file(s) error", MessageBoxButtons.OK, MessageBoxIcon.Error);[m
             }[m
         }[m
 [m
[31m-        /// <summary>[m
[31m-        /// Validates the URI.[m
[31m-        /// </summary>[m
[31m-        /// <returns><c>true</c>, if URI was validated, <c>false</c> otherwise.</returns>[m
[31m-        /// <param name="possibleUri">Possible URI.</param>[m
[31m-        private bool ValidateUri(string possibleUri)[m
[31m-        {[m
[31m-            // Return TryCreate result[m
[31m-            return Uri.TryCreate(possibleUri, UriKind.Absolute, out var uri) &&[m
[31m-                            (uri.Scheme == Uri.UriSchemeHttps ||[m
[31m-                            uri.Scheme == Uri.UriSchemeHttp ||[m
[31m-                            uri.Scheme == Uri.UriSchemeFtp ||[m
[31m-                            uri.Scheme == Uri.UriSchemeMailto ||[m
[31m-                            uri.Scheme == Uri.UriSchemeFile ||[m
[31m-                            uri.Scheme == Uri.UriSchemeNews ||[m
[31m-                            uri.Scheme == Uri.UriSchemeNntp ||[m
[31m-                            uri.Scheme == Uri.UriSchemeGopher ||[m
[31m-                            uri.Scheme == Uri.UriSchemeNetTcp ||[m
[31m-                            uri.Scheme == Uri.UriSchemeNetPipe);[m
[31m-        }[m
[31m-[m
         /// <summary>[m
         /// Processes the text file.[m
         /// </summary>[m
