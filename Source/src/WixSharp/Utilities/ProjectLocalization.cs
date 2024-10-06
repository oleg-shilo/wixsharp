// Ignore Spelling: Deconstruct

using System;
using System.Globalization;

namespace WixSharp
{
    /// <summary>
    /// Represents set of project localization information.
    /// </summary>
    public class ProjectLocalization
    {
        /// <summary>
        /// </summary>
        /// <param name="language">Culture info name. Example: "en-US"</param>
        /// <param name="localizationFile">Optional path to the localization file</param>
        public ProjectLocalization(string language, string localizationFile = null)
            : this(CultureInfo.GetCultureInfo(language), localizationFile)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="cultureInfo">Culture info</param>
        /// <param name="localizationFile">Optional path to the localization file</param>
        public ProjectLocalization(CultureInfo cultureInfo, string localizationFile = null)
            : this(cultureInfo.Name, cultureInfo.TextInfo.ANSICodePage.ToString(), cultureInfo.LCID, localizationFile)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="language">Culture info name. Example: "en-US"</param>
        /// <param name="codePage">The ANSI code page identifier. Example: "1252" for en-US.</param>
        /// <param name="localizationFile">Optional path to the localization file</param>
        public ProjectLocalization(string language, string codePage, string localizationFile = null)
            : this(language, codePage, new CultureInfo(language).LCID, localizationFile)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="language">Culture info name. Example: "en-US"</param>
        /// <param name="codePage">The ANSI code page identifier. Example: "1252" for en-US.</param>
        /// <param name="lcid">Language code Id</param>
        /// <param name="localizationFile">Optional path to the localization file</param>
        ProjectLocalization(string language, string codePage, int lcid, string localizationFile = null)
        {
            if (language.IsEmpty()) throw new ArgumentException("Invalid localization language.", nameof(language));
            if (codePage.IsEmpty()) throw new ArgumentException("Invalid localization code page", nameof(codePage));

            this.Language = language;
            this.CodePage = codePage;
            this.LocalizationFile = localizationFile;
            this.LanguageCodeId = lcid;
        }

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language { get; }

        /// <summary>
        /// Gets the code page.
        /// </summary>
        /// <value>The code page.</value>
        public string CodePage { get; }

        /// <summary>
        /// Gets the localization file.
        /// </summary>
        /// <value>The localization file.</value>
        public string LocalizationFile { get; }

        /// <summary>
        /// Gets the language code identifier.
        /// </summary>
        /// <value>The language code identifier.</value>
        public int LanguageCodeId { get; }

        internal void BindTo(Project project)
        {
            if (project is null)
                throw new ArgumentNullException(nameof(project));

            project.Language = this.Language;
            project.Codepage = this.CodePage;
            project.LocalizationFile = this.LocalizationFile ?? "";
        }
    }
}