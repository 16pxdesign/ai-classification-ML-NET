using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DlibDotNet;

namespace FeaturesExtraction
{
    class ExtractFace
    {
        public static bool BASIC = false;

        public static void CreateCsv(string datasetPath)
        {
            string header = null;
            if (BASIC)
            {
                header = "label,leftEyebrow,rightEyebrow,leftLip,rightLip,lipHeight,lipWidth\n";
            }
            else
            {
                header = "label,leftEyebrow,rightEyebrow,leftLip,rightLip,lipHeight,eye,openmouth,mouthline\n";
            }
            File.WriteAllText(datasetPath, header);
        }


        public static void WriteToCSV(string datasetPath, string line)
        {
            using (var file = new StreamWriter(datasetPath, true))
            {
                file.WriteLine(line);
            }
        }

        public static string Extract(string filePath, bool save)
        {
            string features = "";
            // Set up Dlib Face Detector
            using (var fd = Dlib.GetFrontalFaceDetector())
            {

                using (var sp = ShapePredictor.Deserialize(Path.Combine(GoPath.ProjectPath(), "shape_predictor_68_face_landmarks.dat")))
                {
                    // load input image
                    var img = Dlib.LoadImage<RgbPixel>(filePath);

                    // find all faces in the image
                    var faces = fd.Operator(img);

                    // for each face draw over the facial landmarks
                    foreach (var face in faces)
                    {
                        // find the landmark points for this face
                        var shape = sp.Detect(img, face);

                        var facePoints = new Face();

                        // draw the landmark points on the image
                        for (var i = 0; i < shape.Parts; i++)
                        {

                            var point = shape.GetPart((uint)i);
                            var rect = new Rectangle(point);

                           

                            var match = new int[] { 22, 23, 40, 43, 34, 52, 58, 49, 55 };

                            //Draw rectangles on face
                            Dlib.DrawRectangle(img, rect,
                                color: Array.Exists(match, x => x == i + 1)
                                    ? new RgbPixel(255, 0, 0)
                                    : new RgbPixel(255, 255, 0), thickness: 4);

                            //Add point to face obj
                            facePoints.AddPoint(i + 1, point);
                        }

                        if (BASIC)
                        {
                            features = Directory.GetParent(filePath).Name + ", " + facePoints.getBasicString();

                        }
                        else
                        {
                            features = Directory.GetParent(filePath).Name + ", " + facePoints.getExtraString();

                        }


                    }

                    // Export the modified image
                    if (save)
                    {
                        var newPath = Path.Combine(GoPath.OutputPath(Directory.GetParent(filePath).Name), Path.GetFileName(filePath));
                        Dlib.SaveJpeg(img, newPath);
                    }
                }
            }

            return features;
        }

   

        public static void ExtractFeaturesToFile(string dataPath, string[] filesPath, bool saveImages)
        {
            CreateCsv(dataPath);

            //For each image extract features
            for (int i = 0; i < filesPath.Length; i++)
            {
               
                Console.WriteLine(i + "/" + filesPath.Length);

                //new Thread(new ThreadStart(() => Extract(filesPath[i], saveImages))).Start();
       
                var featureString = Extract(filesPath[i], saveImages);
                Console.WriteLine(featureString);
                WriteToCSV(dataPath, featureString);
            }
        }
    }
}
