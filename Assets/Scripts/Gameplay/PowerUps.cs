using UnityEngine;
using UnityEngine.UI;

public class PowerUps : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] float doubleWaitTime;
    [SerializeField] float tripleWaitTime;
    public static bool powerUpActive = false;

    public void Initialize()
    {
        buttons[0].interactable = true;
        buttons[1].interactable = false;
        buttons[2].interactable = true;
    }

    public void DoubleBalls()
    {
        buttons[0].interactable = false;
        buttons[1].interactable = true;

        Ball.hit -= 2;

        ShootAnother();
        Invoke("ShootAnother", doubleWaitTime);
    }

    public void TripleBalls()
    {
        buttons[1].interactable = false;
        buttons[2].interactable = true;

        Ball.hit -= 3;

        ShootAnother();
        Invoke("ShootAnother", doubleWaitTime);
        Invoke("ShootAnother", tripleWaitTime);
    }

    public void CompleteLevel()
    {
        buttons[2].interactable = false;
        buttons[0].interactable = true;

        Transform wheel = FindObjectOfType<Motor>().transform.GetChild(0);

        for (int i = 0; i < wheel.childCount; i++)
        {
            Transform child = wheel.GetChild(i);
            child.GetComponent<SpriteRenderer>().enabled = false;
            child.GetComponent<Collider2D>().enabled = false;
            child.GetChild(0).gameObject.SetActive(true);

            UIManager.Instance.AddScore();
            AudioManager.Instance.Play("BreakDiamond");
        }

        FindObjectOfType<Ball>().LevelComplete();
    }

    void ShootAnother()
    {
        powerUpActive = true;
        FindObjectOfType<GameplayButton>().ShootBall();
    }
}
