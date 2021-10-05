using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using FeaturesExtraction;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace W8ML
{
    class Program
    {


        static void Main(string[] args)
        {
            for(int i = 0 ; i < 1; i++){
                //for v1,2,3,5.1,

                string datasetPath = GoPath.DatasetPath("v7.0.train.csv");
                string testsetPath = GoPath.DatasetPath("v7.0.test.csv"); //TODO: Change data PATH!!!


                Console.WriteLine($"Training file: {datasetPath}");
                Console.WriteLine($"Testing file: {testsetPath}");

                var mlContext = new MLContext();
                //Train data
                IDataView dataView =
                    mlContext.Data.LoadFromTextFile<InputData>(datasetPath, hasHeader: true, separatorChar: ',');
                dataView = mlContext.Data.ShuffleRows(dataView);
                //Test data 
                IDataView testDataView =
                    mlContext.Data.LoadFromTextFile<InputData>(testsetPath, hasHeader: true, separatorChar: ',');

                //Create pipeline
                var pipeline = LabTrainer.CreatePipeline(mlContext);

                //Train model
                var model = LabTrainer.TrainModel(pipeline, dataView);

                //Save model
                LabTrainer.SaveModel(mlContext, model, dataView);

                //Test data using testing data
                LabTrainer.EvaluateModel(mlContext, model, testDataView);

                

                //Predict data

                var inputData = new InputData() //TODO REMOVE OLD INPUT
                {
                    LeftEye = 5.3f,
                    RightEye = 5.3f,
                    LeftLip = 3.6f,
                    RightLip = 1.2f,
                    HeightLip = 1.2f,
                    WidthLip = 0.3f
                };

                //predict for first in test dataset
                var samplePrediction = mlContext.Data.CreateEnumerable<InputData>(testDataView, false).First();

                Console.WriteLine($"Predicting type: {samplePrediction.Type}");
                LabTrainer.PredictInput(mlContext, model, samplePrediction);


            }


            //AVG Metrics
            LabTrainer.AvgMetrics();
        }

       
    }
}