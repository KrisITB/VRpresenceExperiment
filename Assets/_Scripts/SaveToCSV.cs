using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class SaveToCSV : MonoBehaviour
{
    public void SaveCsv(int sampleID, int groupID, List<string> data)
    {
        string filePath = Application.persistentDataPath + "/Data/" + "Sample_Number-" + sampleID + "_Group_Number-" + groupID + ".csv";

        string delimiter = ",";
        StringBuilder sb = new StringBuilder();

        foreach (string dataString in data)
        {
            sb.AppendLine(string.Concat(dataString, delimiter));
            //sb.AppendLine(string.Join(delimiter, dataString));
        }

        File.WriteAllText(filePath, sb.ToString());
    }
}
