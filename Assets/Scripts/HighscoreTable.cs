using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;
    private string sceneName;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        sceneName = SceneManager.GetActiveScene().name;

        entryTemplate.gameObject.SetActive(false); //hide template

        // load highscores from json and convert to highscores object
        string jsonString = File.ReadAllText(Application.dataPath + "/Highscores/" + sceneName + "highscoreTable.txt");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // sort entry list by score on load
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    //swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        // trim list of highscores to top 10
        if (highscores.highscoreEntryList.Count > 10)
        {
            highscores.highscoreEntryList.RemoveRange(10, highscores.highscoreEntryList.Count - 10);
        }

        // build list of highscore entry transforms from current highscore list
        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }
    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 40f;
     
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default: rankString = rank + "TH"; break;

            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }
        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        // set background visibility by odd/even entry for readibility
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        // highlight first place
        if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
        }
        
        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string name)
    {
        // create new highscore entry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };

        // load highscores from json
        string jsonString = File.ReadAllText(Application.dataPath + "/Highscores/" + sceneName + "highscoreTable.txt");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // add new entry to list
        highscores.highscoreEntryList.Add(highscoreEntry);

        // save new list to json
        string json = JsonUtility.ToJson(highscores);
        File.WriteAllText(Application.dataPath + "/Highscores/" + sceneName + "highscoreTable.txt", json);

    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }
}
