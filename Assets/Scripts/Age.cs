using UnityEngine;
using UnityEngine.UI;

public class Age : MonoBehaviour
{
    [SerializeField] Slider age;
    [SerializeField] TMPro.TextMeshProUGUI ageText;

    private void Start()
    {
        age.onValueChanged.AddListener(delegate { SetText(); });
    }
    
    private void SetText()
    {
        ageText.text = age.value.ToString();
    }

    public void Confirm()
    {
        if (age.value < 13)
        {
            AdScript.Instance.SetConfiguration();
            PlayerPrefs.SetInt("isChild", 1);
        }

        PlayerPrefs.SetInt("ageSet", 1);
        gameObject.SetActive(false);
    }
}
