using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ACEOCustomBuildables
{
    static class LogHelper
    {
        // This code was written by Humoresque
        /// <summary>
        /// Opens the folder where the game's log file is.
        /// </summary>
        /// <param name="logger">This will be used to log errors and a start message. Cannot be null></param>
        public static void OpenGameLog(in Action<string> logger)
        {
            if (logger == null)
            {
                return;
            }

            logger("[Mod Neutral] Starting to open game log!");

            try
            {
                OpenFileInExplorer(Application.persistentDataPath, logger);
            }
            catch (Exception ex)
            {
                logger($"[Mod Error] Failed to open game log with error \"{ex.Message}\"");
            }
        }

        private static void OpenFileInExplorer(in string path, in Action<string> logger)
        {
            if (logger == null)
            {
                return;
            }

            if (!Directory.Exists(path))
            {
                logger("[Mod Error] Path to open File Explorer to in is not a directory!");
                return;
            }

            path.TrimEnd(new char[] { '\\', '/' }); // For no trailing slashes

            Application.OpenURL($"file:///{path}");
        }
    }
}
