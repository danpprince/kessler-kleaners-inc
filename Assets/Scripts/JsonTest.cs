using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Score {
    public int mass;
    public int strokeCount;

    public Score(int mass_, int strokeCount_) {
        mass = mass_;
        strokeCount = strokeCount_;
    }
}

public class JsonTest : MonoBehaviour
{
    void Start()
    {
        string json = JsonUtility.ToJson(new Score(123, 4));
        System.IO.File.WriteAllText(Application.persistentDataPath + "/example-score.json", json);
    }
}
