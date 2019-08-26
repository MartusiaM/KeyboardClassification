using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardClassification
{
    class kNNClassifier
    {
        //trainingSamples - słownik ID elementów Sample ze zbioru treningowego
        Dictionary<int, Sample> _trainingSamples;

        //testingSamples - słownik ID elementów Sample ze zbioru testowego
        Dictionary<int, Sample> _testingSamples;

        
        public kNNClassifier(Dictionary<int,Sample> training, Dictionary<int, Sample> testing)
        {
            _trainingSamples = new Dictionary<int, Sample>(training);
            _testingSamples = new Dictionary<int, Sample>(testing);
        }

        public float ClassifyTestedSamples(int k, ref int positiveCount, ref int allCount)
        {
            int triesCount=0, successfulIdentifications=0;

            foreach(var key in _testingSamples.Keys)
            {
                triesCount++;
                if (ClassifySample(_testingSamples[key], k))
                    successfulIdentifications++;
            }

            positiveCount += successfulIdentifications;
            allCount += triesCount;

            return (float)successfulIdentifications/(float)triesCount*100;
        }

        bool ClassifySample(Sample testedSample, int k)
        {
            //pomocnicz slownik _distanceSamples - dist dla poszczegolnych probek
            SortedDictionary<float, List<int>> _distanceSamples = new SortedDictionary<float, List<int>>();

            foreach (var _trainSampleKey in _trainingSamples.Keys)
            {
                float dist = CountDistance(testedSample, _trainingSamples[_trainSampleKey]);
                if (_distanceSamples.Keys.Contains(dist))
                    _distanceSamples[dist].Add(_trainingSamples[_trainSampleKey].SampleID);
                else
                {
                    _distanceSamples.Add(dist, new List<int>());
                    _distanceSamples[dist].Add(_trainingSamples[_trainSampleKey].SampleID);
                }

            }

            SortedDictionary<float, List<int>> top = new SortedDictionary<float, List<int>>(_distanceSamples.Take(k).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

            return testedSample.UserID == FindBestMatch(ref top);
        }

        int FindBestMatch(ref SortedDictionary<float, List<int>> topDistance)
        {
            Dictionary<int, int> _userPoints = new Dictionary<int, int>();

            //uzupelnianie slownika
            foreach(var key in topDistance.Keys)
            {
                for(int i=0;i<topDistance[key].Count;i++)
                {
                    if (!_userPoints.Keys.Contains(_trainingSamples[topDistance[key][i]].UserID))
                        _userPoints.Add(_trainingSamples[topDistance[key][i]].UserID, 0);
                }
            }

            //wyliczanie punktow
            for(int sum=0;sum<topDistance.Keys.Count;sum++)
            {
                float key = topDistance.Keys.ElementAt(sum);
                for (int i = 0; i < topDistance[key].Count; i++)
                {
                    _userPoints[_trainingSamples[topDistance[key][i]].UserID] += topDistance.Keys.Count - sum;
                }
            }

            //znajdz najwieksza liczbe punktow
            int max = -1;
            foreach(var userKey in _userPoints.Keys)
            {
                if (_userPoints[userKey] > max)
                    max = _userPoints[userKey];
            }

            List<int> usersID = new List<int>();
            foreach (var userKey in _userPoints.Keys)
            {
                if (_userPoints[userKey] == max)
                    usersID.Add(userKey);
            }

            if (usersID.Count == 1)
                return usersID[0];
            else
            {
                foreach (var key in topDistance.Keys)
                {
                    for (int i = 0; i < topDistance[key].Count; i++)
                    {
                        if (usersID.Contains(_trainingSamples[topDistance[key][i]].UserID))
                            return _trainingSamples[topDistance[key][i]].UserID;
                    }
                }
            }

            return -1;
        } 

        float CountDistance(Sample testedSample, Sample trainingSample)
        {
            float sum1 = 0, sum2 = 0;
            for(int i=0;i<testedSample.SampleText.Count;i++)
            {
                sum1 += Math.Abs(testedSample.SampleText[i].DwellTime - trainingSample.SampleText[i].DwellTime);
                sum2 += Math.Abs(testedSample.SampleText[i].FlightTime - trainingSample.SampleText[i].FlightTime);
            }

            return sum1 + sum2;
        }

    }
}
