﻿// // <copyright file="UrlisterSettings.cs" company="PublicDomain.com">
// //     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
// //     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// // </copyright>
// // <auto-generated />

namespace Urlister
{
    // Directives
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Urlister settings.
    /// </summary>
    public class UrlisterSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Urlister.UrlisterSettings"/> class.
        /// </summary>
        public UrlisterSettings()
        {
            // Parameterless constructor
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Urlister.UrlisterSettings"/> always on top.
        /// </summary>
        /// <value><c>true</c> if always on top; otherwise, <c>false</c>.</value>
        public bool AlwaysOnTop { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Urlister.UrlisterSettings"/> closes browser.
        /// </summary>
        /// <value><c>true</c> if close browser; otherwise, <c>false</c>.</value>
        public bool CloseBrowser { get; set; } = false;

        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        /// <value>The browser.</value>
        public string Browser { get; set; } = "Default";

        /// <summary>
        /// Gets or sets the browsers.
        /// </summary>
        /// <value>The browsers.</value>
        public string Browsers { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Urlister.UrlisterSettings"/> enable hotkeys.
        /// </summary>
        /// <value><c>true</c> if enable hotkeys; otherwise, <c>false</c>.</value>
        public bool EnableHotkeys { get; set; } = false;

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>The line.</value>
        public int Line { get; set; } = 1;

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        /// <value>The urls.</value>
        public string[] Urls { get; set; }
    }
}