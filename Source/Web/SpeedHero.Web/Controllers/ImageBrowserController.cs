﻿namespace SpeedHero.Web.Controllers
{
    using System;
    using System.IO;
    using Kendo.Mvc.UI;

    public class ImageBrowserController : EditorImageBrowserController
    {
        private const string ContentFolderRoot = "~/Content/";
        private const string PrettyName = "Images/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/shared/" };

        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath
        {
            get
            {
                return this.CreateUserFolder();
            }
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(ContentFolderRoot, "UserFiles", PrettyName);

            var path = Server.MapPath(virtualPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    this.CopyFolder(Server.MapPath(sourceFolder), path);
                }
            }

            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                this.CopyFolder(folder, dest);
            }
        }
    }
}