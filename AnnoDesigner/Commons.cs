﻿using AnnoDesigner.model;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoDesigner
{
    public class Commons : ICommons
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly IUpdateHelper _updateHelper;
        private static string _selectedLanguage;

        public event EventHandler SelectedLanguageChanged;

        #region ctor

        private static readonly Lazy<Commons> lazy = new Lazy<Commons>(() => new Commons());

        public static Commons Instance
        {
            get { return lazy.Value; }
        }

        static Commons()
        {
            _updateHelper = new UpdateHelper(App.ApplicationPath, new AppSettings());
        }

        private Commons()
        {
        }

        #endregion

        public IUpdateHelper UpdateHelper
        {
            get { return _updateHelper; }
        }

        public string SelectedLanguage
        {
            get
            {
                if (_selectedLanguage != null && Localization.Localization.LanguageCodeMap.ContainsKey(_selectedLanguage))
                {
                    return _selectedLanguage;
                }
                else
                {
                    _selectedLanguage = "English";
                    return _selectedLanguage;
                }
            }
            set
            {
                _selectedLanguage = value ?? "English";
                SelectedLanguageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static bool CanWriteInFolder(string folderPathToCheck = null)
        {
            var result = false;

            try
            {
                if (string.IsNullOrWhiteSpace(folderPathToCheck))
                {
                    folderPathToCheck = App.ApplicationPath;
                }

                var testFile = Path.Combine(folderPathToCheck, "test.test");
                File.WriteAllText(testFile, "test");

                if (File.Exists(testFile))
                {
                    File.Delete(testFile);
                }

                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Cannot write to folder (\"{folderPathToCheck}\").");
            }

            return result;
        }

        public static void RestartApplication(bool asAdmin, string parameters, string path = null)
        {
            var arguments = string.Empty;
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                arguments = parameters;
            }

            arguments = arguments.Trim();

            if (string.IsNullOrWhiteSpace(path))
            {
                path = App.ExecutablePath;
            }

            var psi = new ProcessStartInfo();
            psi.FileName = path;
            psi.Arguments = arguments;

            if (asAdmin)
            {
                psi.Verb = "runas";
            }

            logger.Trace($"{nameof(RestartApplication)} {nameof(asAdmin)}: {asAdmin}{Environment.NewLine}Path: \"{psi.FileName}\"{Environment.NewLine}Arguments: {psi.Arguments}");

            var process = new Process();
            process.StartInfo = psi;
            process.Start();

            Environment.Exit(-1);
            //Application.Current.Shutdown();//sometimes hangs
        }
    }
}
