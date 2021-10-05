using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FeaturesExtraction
{
    public static class GoPath
    {
        public static string ProjectPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\.."));
        }

        public static string DatasetPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\SavedDatasets"));
        }

        public static string MainPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\.."));
        }

        public static string ImagesPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\..\Images"));
        }

        public static string DatasetPath(string name)
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\SavedDatasets\" + name));
        }

        public static string OutputPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\..\Output"));
        }

        public static string OutputPath(string name)
        {
            var path =  Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\..\Output\"+ name));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string DatasetPathExperiment()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\SavedDatasets\Experiment"));

        }

        public static string SaveDatasetPath(string name)
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\..\SavedDatasets\" + name));
        }

        public static string ImagesPathForPrepare()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\Images"));
        }

        public static string SaveDataImagesPath(string name)
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\SavedDatasets\" + name));

        }

        public static string DatasetPathExperiment(string name)
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, @"..\..\..\..\SavedDatasets\Experiment\" + name));

        }
    }
}