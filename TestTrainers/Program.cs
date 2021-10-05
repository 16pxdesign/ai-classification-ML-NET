using System;
using System.IO;
using FeaturesExtraction;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using W8ML;

namespace TestTrainers
{
    class Program
    {
        private const uint TestTime = 100;

        static void Main(string[] args)
        {
            TestOne("v7.0.train.csv");
            //TestOneWithShufle("v7.0.train.csv", 5);
            //TestAllFiles();
        }

        private static void TestOne(string file, int iterations = 1)
        {
            int count = 1;
            do
            {
                string datasetPath = GoPath.DatasetPathExperiment(file);
                Console.WriteLine("************************************************************");
                Console.WriteLine("*  Testing file: " + datasetPath);
                Console.WriteLine("*-----------------------------------------------------------");
                TestTrainers(datasetPath, TestTime, false);
                count++;
            } while (count<iterations);
           
        }

        private static void TestOneWithShufle(string file, int iterations = 1)
        {
            int count = 1;
            do
            {
                string datasetPath = GoPath.DatasetPathExperiment(file);
                Console.WriteLine(datasetPath);
                Console.WriteLine($"*************************TEST NO {count}***********************************");
                Console.WriteLine("*  Testing file: " + datasetPath);
                Console.WriteLine("*-----------------------------------------------------------");
                TestTrainers(datasetPath, TestTime, true, count);
                count++;
            } while (count < iterations);
        }


        private static void TestAllFiles()
        {
            string datasetPath = GoPath.DatasetPathExperiment();
            var filesPath = Directory.GetFiles(datasetPath, "*", SearchOption.TopDirectoryOnly);
            Console.WriteLine("Start testing for: " + filesPath.Length + "inputs");
            foreach (var data in filesPath)
            {
                Console.WriteLine("************************************************************");
                Console.WriteLine("*  Testing file: " + data);
                Console.WriteLine("*-----------------------------------------------------------");
                TestTrainers(data, TestTime, false);
            }
        }

        private static void TestTrainers(string datasetPath, uint duration, bool shuffle, int seed = 0)
        {
            //Context
            var mlContext = new MLContext();

            //Load data
            IDataView dataView =
                mlContext.Data.LoadFromTextFile<InputData>(datasetPath, hasHeader: true, separatorChar: ',');

            if (shuffle)
            {
                dataView = mlContext.Data.ShuffleRows(dataView, seed: seed);
            }

            //Settings for experiment
            var multiclassExperimentSettings = new MulticlassExperimentSettings
            {
                MaxExperimentTimeInSeconds = duration
            };

            //Create experiment
            var multiclassClassificationExperiment =
                mlContext.Auto().CreateMulticlassClassificationExperiment(multiclassExperimentSettings);

            //Create handler to show results
            var progress = new Progress<RunDetail<MulticlassClassificationMetrics>>(m =>
            {
                Console.WriteLine($"Testing algorithm: {m.TrainerName} - Run time - {m.RuntimeInSeconds}");
                Console.WriteLine($"*** - MacroAccuracy: {m.ValidationMetrics.MacroAccuracy}");
                Console.WriteLine($"*** - MicroAccuracy: {m.ValidationMetrics.MicroAccuracy}");
                Console.WriteLine($"*** - LogLoss: {m.ValidationMetrics.LogLoss}");
                Console.WriteLine($"*** - LogLossReduction: {m.ValidationMetrics.LogLossReduction}");
                Console.WriteLine("*-----------------------------------------------------------");
                Console.WriteLine();
            });

            //Execute experiment
            var experimentResult =
                multiclassClassificationExperiment.Execute(dataView, "EmotionLabel", progressHandler: progress);

            //Prompt results
            Console.WriteLine("===============*===============");
            Console.WriteLine("Best trainer in time " + duration);
            Console.WriteLine($"Trainer name - {experimentResult.BestRun.TrainerName}");
            Console.WriteLine($"MacroAccuracy - {experimentResult.BestRun.ValidationMetrics.MacroAccuracy}");
            Console.WriteLine($"MicroAccuracy - {experimentResult.BestRun.ValidationMetrics.MicroAccuracy}");
            Console.WriteLine($"LogLoss - {experimentResult.BestRun.ValidationMetrics.LogLoss}"); ;
            Console.WriteLine($"LogLossReduction - {experimentResult.BestRun.ValidationMetrics.LogLossReduction}");
            Console.WriteLine("===============*===============");
            Console.WriteLine("===============*===============");

            //Prompt matrix
            ConfusionMatrix validationMetricsConfusionMatrix = experimentResult.BestRun.ValidationMetrics.ConfusionMatrix;
            var formattedConfusionTable = validationMetricsConfusionMatrix.GetFormattedConfusionTable();
            Console.WriteLine(formattedConfusionTable);
            Console.WriteLine();
        }
    }
}