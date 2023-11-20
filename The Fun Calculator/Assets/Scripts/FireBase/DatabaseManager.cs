using Firebase;
using Firebase.Database;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{
    //Get the roo reference location of the database
    public InputField Name;
    public InputField Gold;

    public Text NameText;

    public Text GoldText;

    private string userID;
    private DatabaseReference dbReference;

    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUser()
    {
        User newUser = new User(Name.text, int.Parse(Gold.text));
        string json = JsonUtility.ToJson(newUser);

        dbReference.Child("users").Child(userID).SetRawJsonValueAsync(json);
    }

    public IEnumerator GetName(Action<string> onCallback)
    {
        var userNameData = dbReference.Child("users").Child(userID).Child("name").GetValueAsync();

        yield return new WaitUntil(predicate: () => userNameData.IsCompleted);

        if(userNameData != null)
        {
            DataSnapshot snapshot = userNameData.Result;

            onCallback.Invoke(snapshot.Value.ToString());
        }
    }

    public IEnumerator GetGold(Action<int> onCallback)
    {
        var userGoldData = dbReference.Child("users").Child(userID).Child("gold").GetValueAsync();

        yield return new WaitUntil(predicate: () => userGoldData.IsCompleted);

        if (userGoldData != null)
        {
            DataSnapshot snapshot = userGoldData.Result;

            onCallback.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }

    public void GetUserInfo()
    {

        StartCoroutine(GetName ((string name) =>
        {
            NameText.text = "Name: " + name;
        }));

        StartCoroutine(GetGold ((int gold) =>
        {
            GoldText.text = "Gold: " + gold.ToString();
        }));

    }

    public void UpdateName()
    {
        dbReference.Child("users").Child(userID).Child("name").SetValueAsync(Name.text);
    }


    public void UpdateGold()
    {
        dbReference.Child("users").Child(userID).Child("gold").SetValueAsync(Gold.text);
    }


}
