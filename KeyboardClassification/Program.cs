using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardClassification
{
    class Program
    {
        static void Main(string[] args)
        {
            //pattern - ktore kawisze po kolei powinny byc wciskane w kodzie ASCI
            string[] pattern = new string[] { "190", "190", "84", "84", "73", "73", "69", "69", "53", "53",
                "16", "82", "82", "16", "79", "79", "65", "65", "78", "78", "76", "76", "13" };
            
            //n - liczba powtrozen podzialów zbiorow i klasyfikacji
            //correctCount - liczba poprawnie zakwalifikowanych probke
            //triesCount - liczba wszystkich prób
            int n=70, correctCount, triesCount;

            //trainingSamples - słownik ID elementów Sample ze zbioru treningowego
            Dictionary<int, Sample> trainingSamples;

            //testingSamples - słownik ID elementów Sample ze zbioru testowego
            Dictionary<int, Sample> testingSamples;

            Database samplesDatabase = new Database("C://Users//Asus//Desktop//MARTA//Semestr 6//Biometria//Projekt2//KDS2_data.sql");
            Filter filetr = new Filter(pattern);
            kNNClassifier classifier;
            float successRate;

            Dictionary<int, User> samplesByUsers = samplesDatabase.ExtractSamples();

            Console.WriteLine("Number of users before filtering: "+samplesByUsers.Keys.Count);

            //filtrowanie
            filetr.FilterDictionary(ref samplesByUsers,9);

            Console.WriteLine("Number of users after filtering: "+samplesByUsers.Keys.Count);

            foreach (var key in samplesByUsers.Keys)
            {
                //tworzenie wektorow cech
                samplesByUsers[key].GenerateFeatureVectors();
            }

            //dla róznych wartości k
            for(int k=3;k<=9;k++)
            {
                correctCount = 0;
                triesCount =0;
                //usrednianie klasyfikacji
                for (int i = 0; i < n; i++)
                {
                    foreach (var key in samplesByUsers.Keys)
                    {
                        //podział na zbiory testowego i treningowego
                        samplesByUsers[key].SeparateSamples(k);
                    }

                    //generowanie zbiorów treningowego i testowego
                    testingSamples = new Dictionary<int, Sample>();
                    trainingSamples = new Dictionary<int, Sample>();
                    foreach (var key in samplesByUsers.Keys)
                    {
                        foreach(var sample in samplesByUsers[key].FeatureVectors)
                        {
                            if (sample.SampleType == SampleGroup.TRAINING)
                                trainingSamples.Add(sample.SampleID, sample);
                            else
                                testingSamples.Add(sample.SampleID, sample);
                        }
                    }

                    //klasyfikacja
                    classifier = new kNNClassifier(trainingSamples, testingSamples);
                    successRate = classifier.ClassifyTestedSamples(k, ref correctCount, ref triesCount);
                    Console.WriteLine("Success rate for try number:" + i + " (k=" + k + ") equals: " + successRate + "%.\n");
                }
                float successRateK = (float)correctCount / (float)triesCount * 100;
                Console.WriteLine("SuccessRate rate for k=" + k + " equals: " + successRateK + "%. \n");
            }          


        }
    }
}
