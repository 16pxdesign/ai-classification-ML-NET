using System;
using System.Collections.Generic;
using System.Linq;
using FeaturesExtraction;
using Microsoft.ML;
using Microsoft.ML.Data;
using W8ML;

namespace CrossValid
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                //Data
                string datasetPath = GoPath.DatasetPath("v7.0.train.csv");
                string testsetPath = GoPath.DatasetPath("v7.0.test.csv"); //TODO: Change data PATH!!!
                string allPath = GoPath.DatasetPath("v7.0.all.csv"); //TODO: Change data PATH!!!

                Console.WriteLine($"Training file: {datasetPath}");
                Console.WriteLine($"Testing file: {testsetPath}");

                var mlContext = new MLContext();
                //Train data
                IDataView dataView =
                    mlContext.Data.LoadFromTextFile<InputDataExtra>(datasetPath, hasHeader: true, separatorChar: ',');
                //Test data 
                IDataView testDataView =
                    mlContext.Data.LoadFromTextFile<InputDataExtra>(testsetPath, hasHeader: true, separatorChar: ',');
                IDataView alDataView =
                    mlContext.Data.LoadFromTextFile<InputDataExtra>(allPath, hasHeader: true, separatorChar: ',');

                //Create pipeline
                var pipeline = LabTrainer.CreatePipeline(mlContext);

                //Train model
                var model = LabTrainer.TrainModel(pipeline, dataView);

                //Save model
                LabTrainer.SaveModel(mlContext, model, dataView);

                //Test data using testing data
                LabTrainer.EvaluateModel(mlContext, model, testDataView);

                var crossValidationResults =
                    mlContext.MulticlassClassification.CrossValidate(alDataView, pipeline, 5, "Label");


                Cross.PrintMulticlassClassificationFoldsAverageMetrics(crossValidationResults);

            }while(false);


        }

    
    }
    }
