using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public Color unitColor;

    void Awake()
    {
        Debug.Log(Application.persistentDataPath + "/savefile.json");
        if (Instance != null) {
            Destroy(gameObject);
        }

        //Set this as the instance if there is not already a gamemanager
        Instance = this;

        //transfer between scenes
        DontDestroyOnLoad(gameObject);
            //Prevents error on first bootup of game
        if (File.Exists(Application.persistentDataPath + "/savefile.json")) {
            loadColor();
        } else {
            Debug.Log("Save file not found, using default color.");
            unitColor = Color.black; // Set to a default color if file doesn't exist
        }
    }


    void Update()
    {
        
    }

    [System.Serializable]
        class saveData {
        public Color unitColor;
    }

    public void saveColor() {
        saveData data = new saveData();
        data.unitColor = unitColor;

        //converts data to json file
        string jsonData = JsonUtility.ToJson(data);

        //saves json file to memory
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", jsonData);
    }

    public void loadColor() {
            //gets the json from persistent data path and converts to string
        string jsonData = File.ReadAllText(Application.persistentDataPath + "/savefile.json");
            //converts string to unity readable data--Deserialisation
        saveData data = JsonUtility.FromJson<saveData>(jsonData);
            //applies that data to the game manager
        unitColor = data.unitColor;
    }
}