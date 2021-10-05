using System;
using System.IO;
using DlibDotNet;

namespace FeaturesExtraction
{
    class Program
    {
        static void Main(string[] args)
        {
            //get images paths
            var filesPath = Directory.GetFiles(GoPath.ImagesPath(), "*", SearchOption.AllDirectories);

            //Create data file
            var dataPath = GoPath.SaveDatasetPath("v20x.0.all.csv");

            //Generate file based on images in folder
            ExtractFace.ExtractFeaturesToFile(dataPath, filesPath, true);
        }
    }
}
