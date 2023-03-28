using UnityEngine;
using System.Collections;

public class Motor : MonoBehaviour
{
    [SerializeField] public int wheelSpeed;
    [HideInInspector] public int currentSpeed;
    [SerializeField] Transform originalWheel;
    [SerializeField] Transform[] bonusWheel;

    [SerializeField] Sprite[] wheelSprites;
    [SerializeField] Sprite[] bonusWheelSprites;
    [SerializeField] Sprite[] mediumWheelSprites;
    int currentSpriteIndex = 0;
    int wheelHealth = 3;
    
    public int bonusLevelNumber = 0;

    private void Start()
    {
        currentSpeed = wheelSpeed;
        //StartCoroutine("PrintVariables");
    }

    IEnumerator PrintVariables()
    {
        while (true)
        {
            // Sync with Fixed Update
            yield return new WaitForFixedUpdate();

            Debug.Log($"Health: {wheelHealth}");

            // Wait for the rotation time to end
            yield return new WaitForSecondsRealtime(3);
        }
    }

    IEnumerator RandomRotation()
    {
        while (true)
        {
            // Sync with Fixed Update
            yield return new WaitForFixedUpdate();

            // Pick a speed and time
            if (GameManager.level > 2)
                currentSpeed = Random.Range(0, 2) == 0 ? currentSpeed : -currentSpeed;

            int timeForRotation = Random.Range(1, 4);

            // Set the motor speed
            SetMotor();

            // Wait for the rotation time to end
            yield return new WaitForSecondsRealtime(timeForRotation);
        }
    }

    private void SetMotor()
    {
        JointMotor2D motor = new JointMotor2D();
        motor.motorSpeed = currentSpeed;
        motor.maxMotorTorque = 10000;

        GetComponent<WheelJoint2D>().motor = motor;
    }

    public void SetWheel()
    {
        wheelHealth = 3;

        if (transform.childCount != 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            StopCoroutine("RandomRotation");
        }

        Transform wheel;

        // Bonus Level
        if (GameManager.level % 5 == 0 && GameManager.difficulty == GameManager.Difficulty.Easy)
        {
            wheel = Instantiate(bonusWheel[bonusLevelNumber]);
            bonusLevelNumber = bonusLevelNumber >= 3 ? 3 : bonusLevelNumber + 1;
        }
        
        // Normal Level
        else
        {
            wheel = Instantiate(originalWheel);
            
            // Set random diamonds active
            ActivateDiamonds(wheel);
            
            // Change the wheel sprite
            SetWheelSprite(wheel);
        }
        
        
        // Change its parent to this object and set its position
        wheel.SetParent(transform);
        wheel.localPosition = new Vector3(0, 0, 0);

        // Attach the rigid body of the wheel to the Motor
        transform.GetComponent<WheelJoint2D>().connectedBody = wheel.GetComponent<Rigidbody2D>();

        // Start Rotation
        StartCoroutine("RandomRotation");
    }

    void SetWheelSprite(Transform wheel)
    {
        if (GameManager.difficulty == GameManager.Difficulty.Easy || GameManager.difficulty == GameManager.Difficulty.Hard)
        {
            // Get a wheel different from the previous one
            int spriteNumber = Random.Range(0, wheelSprites.Length);

            while (spriteNumber == currentSpriteIndex)
                spriteNumber = Random.Range(0, wheelSprites.Length);

            currentSpriteIndex = spriteNumber;
            // Set the sprite
            wheel.gameObject.GetComponent<SpriteRenderer>().sprite = wheelSprites[spriteNumber];
        }

        else if (GameManager.difficulty == GameManager.Difficulty.Medium || GameManager.difficulty == GameManager.Difficulty.Expert)
        {
            wheel.gameObject.GetComponent<SpriteRenderer>().sprite = mediumWheelSprites[0];
            currentSpriteIndex = 0;
        }
    }

    void ActivateDiamonds(Transform wheel)
    {
        for (int i = 0; i < GameManager.numberOfDiamonds; i++)
        {
            int indexOfDiamond = Random.Range(0, wheel.childCount);

            while (wheel.GetChild(indexOfDiamond).gameObject.activeSelf)
                indexOfDiamond = Random.Range(0, wheel.childCount);

            wheel.GetChild(indexOfDiamond).gameObject.SetActive(true);
        }

        // Delete the extra diamonds
        for (int i = 0; i < wheel.childCount; i++)
            if (wheel.GetChild(i).gameObject.activeSelf == false)
                Destroy(wheel.GetChild(i).gameObject);
    }

    public void ReduceHealth()
    {
        
        if (wheelHealth > 0)
            wheelHealth--;
     
        if (wheelHealth == 0)
        {
            GameManager.isGameOver = true;
            wheelHealth = 3;
            Invoke("GameOver", .5f);
        }

        else
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = mediumWheelSprites[++currentSpriteIndex];
    }

    void GameOver() { UIManager.Instance.EnableGameOver(); }

    public void MediumRetrySetup()
    {
        currentSpriteIndex = -1;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = mediumWheelSprites[++currentSpriteIndex];
        wheelHealth = 3;
    }
}
