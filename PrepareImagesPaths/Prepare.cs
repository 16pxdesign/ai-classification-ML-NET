using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FeaturesExtraction;

namespace PreTrain
{
    public class Prepare
    {
        public static void BuildFile()
        {
            
            //get images paths
            var filesPath = Directory.GetFiles(GoPath.ImagesPathForPrepare(), "*", SearchOption.AllDirectories);

            //Create data file
            var dataPath = GoPath.SaveDataImagesPath("7.0.all.imagespaths.csv");
            CreateCsv(dataPath);

            //For each image make line
            for (int i = 0; i < filesPath.Length; i++)
            {
                Console.WriteLine(i + "/" + filesPath.Length);
                var name = Directory.GetParent(filesPath[i]).Name;
                var line = name + "," + filesPath[i];
                WriteToCSV(dataPath,line);
            }
        }

        public static void CreateCsv(string datasetPath)
        {
            string header = "Label,ImageSource\n";
            File.WriteAllText(datasetPath, header);
        }

        public static void WriteToCSV(string datasetPath, string line)
        {
            using (var file = new StreamWriter(datasetPath, true))
            {
                file.WriteLine(line);
            }
        }

    }
}
