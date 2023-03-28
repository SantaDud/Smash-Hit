using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class Connection : MonoBehaviour
{
    [Space]
    [Header("Connection")]
    public bool connected;
    public bool checking;
    // Start is called before the first frame update
    void Start()
    {
        connected = true;
    }
    

    void Checking()
    {
        StartCoroutine(CheckInternet());
    }

    IEnumerator CheckInternet()
    {
        UnityWebRequest request = new UnityWebRequest("https://google.com");
        yield return request.SendWebRequest();

        connected = (request.error == null) ? true : false;

        checking = false;
    }

    private void Update()
    {
        if (!checking)
        {
            checking = true;
            Invoke("Checking", 2f);
        }
    }
}
