using System;
using System.Linq;
using CrossValid;
using FeaturesExtraction;
using Microsoft.ML;
using W8ML;

namespace ShufleAndSplit4._0
{
    class Program
    {
        private static bool SPLIT = true;

        static void Main(string[] args)
        {
            int seed = 1;
            do
            {
                for (int i = 0; i < 1; i++)
                {
                    Console.WriteLine($"Seed: {seed}");

                    //Data
                    string datasetPath = GoPath.DatasetPath("v3.0.all.csv");
                    string testsetPath = GoPath.DatasetPath("3.0.sort.test.csv"); //If split off

                    //Prompts
                    Console.WriteLine($"Training file: {datasetPath}");
                    Console.WriteLine($"Testing file: {testsetPath}");

                    var mlContext = new MLContext();
                    //Train data
                    IDataView dataView =
                        mlContext.Data.LoadFromTextFile<InputData>(datasetPath, hasHeader: true, separatorChar: ',');
                    //Test data 
                    IDataView testDataView = null;
                    IDataView trainDataView = null;
                    if (!SPLIT)
                    {
                        testDataView =
                            mlContext.Data.LoadFromTextFile<InputData>(testsetPath, hasHeader: true,
                                separatorChar: ',');
                    }
                    else
                    {
                        //Split data from one file
                        var splitData = Shuffle.SplitAndShuffle(dataView, mlContext, seed);
                        trainDataView = splitData.TrainSet;
                        testDataView = splitData.TestSet;
                    }



                    //Create pipeline
                    var pipeline = LabTrainer.CreatePipeline(mlContext);

                    //Train model
                    var model = LabTrainer.TrainModel(pipeline, trainDataView);

                    //Save model
                    LabTrainer.SaveModel(mlContext, model, trainDataView);

                    //Test data using testing data
                    LabTrainer.EvaluateModel(mlContext, model, testDataView);

                    //Predict data
                    if (false)
                    {
                        var samplePrediction = mlContext.Data.CreateEnumerable<InputData>(testDataView, false).First();

                        Console.WriteLine($"Predicting type: {samplePrediction.Type}");

                        LabTrainer.PredictInput(mlContext, model, samplePrediction);
                    }

                    //show cross validation
                    if (true)
                    {
                        var crossValidationResults =
                            mlContext.MulticlassClassification.CrossValidate(dataView, pipeline, 5, "Label");
                        Cross.PrintMulticlassClassificationFoldsAverageMetrics(crossValidationResults);
                    }
                }

                seed++;
            } while (seed < 10);

            //Get avg metrics for all loop
            LabTrainer.AvgMetrics();
        }
    }
}