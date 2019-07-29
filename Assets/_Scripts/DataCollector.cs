using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollector
{
    private static List<string> data = new List<string>();

    public void UpdateDataSet(string newData)
    {
        data.Add(newData);
    }

    public List<string> GetDataSet()
    {
        return data;
    }

}
