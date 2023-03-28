using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameplayButton : Button
{
    bool isPressed = true;
    public bool hitValueChecked = true;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        
        if (isPressed && hitValueChecked && !GameManager.isGameOver && !GameManager.isLevelComplete)
        {
            hitValueChecked = false;
            isPressed = false;
            ShootBall();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isPressed = true;
    }

    public void ShootBall()
    {
        BallDetails ballDetails = gameObject.GetComponent<BallDetails>();
        // Add Force to ball
        ballDetails.rigidBody.AddForce(Vector2.up * ballDetails.force, ForceMode2D.Impulse);

        // Reduce the balls from the UI
        if (!PowerUps.powerUpActive)
        {
            for (int i = 0; i < ballDetails.balls.childCount; i++)
                if (ballDetails.balls.GetChild(i).gameObject.activeSelf)
                {
                    ballDetails.balls.GetChild(i).gameObject.SetActive(false);
                    break;
                }
        }

        // Increase hit variable which keeps count of the number of balls shot.
        Ball.hit++;
    }
}
