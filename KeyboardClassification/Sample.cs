using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardClassification
{
    public enum SampleGroup
    {
        TRAINING,
        TEST,
        PROFILE
    }

    public class Sample
    {
        private static int IDGenerator = 0;
        public int SampleID { get; set; }
        public int UserID { get; set; }
        public SampleGroup SampleType { get; set; }
        public List<Key> SampleText { get; set; }

        public Sample()
        {
            SampleID = IDGenerator++;
            SampleType = SampleGroup.TEST;
        }

        public void ExtractingKeysList(string input)
        {
            SampleText = new List<Key>();
            Dictionary<char, SampleHelper> signDictionary = new Dictionary<char, SampleHelper>(); //jest slownik bo znajki sa rozne, jesli program ma dzialac dla wzorow z powtarzajacymi sie nzakami nalezy zmienic ta strukture

            CreateHelper(ref signDictionary, input);

            CreateKeys(signDictionary, input);

        }

        void CreateKeys(Dictionary<char, SampleHelper> signDictionary, string input)
        {
            SampleHelper[] sequence = new SampleHelper[signDictionary.Count];

            foreach (var key in signDictionary.Keys)
                sequence[signDictionary[key].sequenceNumber] = signDictionary[key];

            //tworzenie listy z kolejnoscia wciskanych przyciskow
            for (int i = 0; i < sequence.Length; i++)
            {
                SampleHelper sample = sequence[i];
                
                if(sequence.Length>(i+1))
                {
                    Key newKey = new Key();
                    newKey.Sign = sample.sign;
                    newKey.DwellTime = sample.uTime - sample.dTime;
                    newKey.FlightTime = sequence[i + 1].dTime - sample.uTime;
                    SampleText.Add(newKey);
                } 
            }
        }

        void CreateHelper(ref Dictionary<char, SampleHelper> signDictionary, string input)
        {
            int seqnumber = 0;
            String[] singleOperations = input.Split(' ');
            for (int i = 0; i < singleOperations.Length; i++)
            {
                String[] parts = singleOperations[i].Split('_');

                if (parts[0].Equals("d"))
                {
                    //przucisk wcisniety
                    SampleHelper samHelp = new SampleHelper();
                    samHelp.dTime = Int32.Parse(parts[2]);
                    samHelp.sign = (char)Int32.Parse(parts[1]);
                    samHelp.sequenceNumber = seqnumber++;
                    signDictionary.Add(samHelp.sign, samHelp);
                }
                else if (parts[0].Equals("u"))
                {
                    //przycisk puszczony
                    //jesli to nie jest ENTER
                    if(parts[1]!= "13")
                        signDictionary[(char)Int32.Parse(parts[1])].uTime = Int32.Parse(parts[2]);
                }
            }
        }

        private class SampleHelper
        {
            public int sequenceNumber { get; set; }
            public char sign { get; set; }
            public int dTime { get; set; }
            public int uTime { get; set; }

        }
    }


}
