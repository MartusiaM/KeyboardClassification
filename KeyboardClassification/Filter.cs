using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardClassification
{
    class Filter
    {
        string[] _pattern;

        public Filter(string[] pattern)
        {
            _pattern = new string[pattern.Length];
            pattern.CopyTo(_pattern, 0);
        }
        public void FilterDictionary(ref Dictionary<int, User> samplesByUsers, int n)
        {
            FilterEnter(ref samplesByUsers); //usuwanie reszty symboli po pierwszym enterze
            FilterLength(ref samplesByUsers);   //usuwanie tych probek, ktore sa za dlugie
            FilterMatch(ref samplesByUsers); //bledny napis
            FilterUsers(ref samplesByUsers, n); //usuwanie tych uzytkownikow ktorzy maja za malo probek
        }

        private void FilterEnter(ref Dictionary<int, User> samplesByUsers)
        {
            foreach(var key in samplesByUsers.Keys)
            {
                for(int i=0;i<samplesByUsers[key].Inputs.Count;i++)
                {
                    int lastOperation = -1;
                    String newSample = "";
                    String[] singleOperation = samplesByUsers[key].Inputs[i].Split(' ');
                    for(int j=0;j<singleOperation.Length;j++)
                    {
                        String[] parts = singleOperation[j].Split('_');
                        if (parts[1].Equals("13"))
                        {
                            lastOperation = j;
                            break;
                        }                  
                    }

                    for(int j=0;j<=lastOperation;j++)
                    {
                        newSample += singleOperation[j] + ' ';
                    }

                    samplesByUsers[key].Inputs[i] = newSample;
                }
            }
        }

        private void FilterLength(ref Dictionary<int, User> samplesByUsers)
        {
            foreach (var key in samplesByUsers.Keys)
            {
                List<string> elemToDelete = new List<string>();
                for(int i=0;i<samplesByUsers[key].Inputs.Count;i++)
                {
                    String[] singleOperation = samplesByUsers[key].Inputs[i].Split(' ');
                    if (singleOperation.Length != 24)
                        elemToDelete.Add(samplesByUsers[key].Inputs[i]);
                }

                foreach(var obj in elemToDelete)
                {
                    samplesByUsers[key].Inputs.Remove(obj);
                }
            }
        }

        private void FilterUsers(ref Dictionary<int, User> samplesByUsers, int minProbeNumber)
        {
            List<int> keysToDelete = new List<int>();
            foreach(var key in samplesByUsers.Keys)
            {
                if (samplesByUsers[key].Inputs.Count < minProbeNumber)
                    keysToDelete.Add(key);
            }

            foreach(var key in keysToDelete)
            {
                samplesByUsers.Remove(key);
            }
        }

        private void FilterMatch(ref Dictionary<int, User> samplesByUsers)
        {
            foreach (var key in samplesByUsers.Keys)
            {
                List<string> elemToDelete = new List<string>();
                foreach (var sample in samplesByUsers[key].Inputs)
                {
                    String[] singleOperations = sample.Split(' ');
                    for(int i=0;i<singleOperations.Length-1;i++)
                    {
                        String[] parts = singleOperations[i].Split('_');
                        if(!parts[1].Equals(_pattern[i]))
                            elemToDelete.Add(sample);
                    }
   
                }

                foreach (var obj in elemToDelete)
                {
                    samplesByUsers[key].Inputs.Remove(obj);
                }
            }
        }
    }
}
