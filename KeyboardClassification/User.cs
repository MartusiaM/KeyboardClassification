using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardClassification
{
    public class User
    {
        public int UserID { get; set; }
        public List<string> Inputs { get; set; }
        public List<Sample> FeatureVectors { get; set; }
        public Sample Profile { get; set; }

        public void GenerateProfile(int trainingSetCount)
        {
            Profile = new Sample();
            Profile.SampleType = SampleGroup.PROFILE;
            Profile.SampleText = new List<Key>(FeatureVectors[0].SampleText);

            for(int i=0;i<Profile.SampleText.Count;i++)
            {
                int averageDwell = 0;
                int averageFlight = 0;

                foreach (var el in FeatureVectors)
                {
                    if (el.SampleType == SampleGroup.TRAINING)
                    {
                        averageDwell += el.SampleText[i].DwellTime;
                        averageFlight += el.SampleText[i].FlightTime;
                    }
                }

                Profile.SampleText[i].DwellTime = averageDwell / trainingSetCount;
                Profile.SampleText[i].FlightTime = averageFlight / trainingSetCount;
            }   
        }
        public void SeparateSamples(int trainingSetCount)
        {
            List<int> indexes = new List<int>();
            Random rand = new Random(DateTime.Now.Millisecond);

            foreach(var ftVector in FeatureVectors)
            {
                ftVector.SampleType = SampleGroup.TEST;
            }

            for(int i=0;i<trainingSetCount;i++)
            {
                int pos = -1;
                do
                {
                    pos = rand.Next(0, FeatureVectors.Count);
                } while (indexes.Contains(pos));
                indexes.Add(pos);

                FeatureVectors[pos].SampleType = SampleGroup.TRAINING;
            }   

        }
        public void GenerateFeatureVectors()
        {
            FeatureVectors = new List<Sample>();

            foreach(var input in Inputs)
            {
                Sample newSample = CreateSample(input);
                if (newSample!=null)
                FeatureVectors.Add(newSample);
            }
        }

        private Sample CreateSample(string input)
        {
            Sample sample = new Sample();

            sample.UserID = UserID;
            sample.ExtractingKeysList(input);

            if (sample.SampleText.Count != 11)
                return null;

            return sample;
        }
    }
}
