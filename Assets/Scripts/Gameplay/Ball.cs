using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    Rigidbody2D rigidBody;

    [Header("General")]
    [SerializeField] Transform balls;
    [SerializeField] uint force;

    public static int hit = 0;

    public static int diamondsHit = 0;
    public static int numberOfBalls;
    
    bool isActive = true;
    bool enableDiamondAnimation = true;

    [SerializeField] BallDetails ballDetails;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        ballDetails.rigidBody = rigidBody;
        //StartCoroutine("PrintVariables");
    }

    IEnumerator PrintVariables()
    {
        while (true)
        {
            // Sync with Fixed Update
            yield return new WaitForFixedUpdate();

            Debug.Log($"Hit: {hit}\nNumber of Balls: {numberOfBalls}");

            // Wait for the rotation time to end
            yield return new WaitForSecondsRealtime(4);
        }
    }

    public void Initialize()
    {
        // If hit is greater than 0 then level changing.
        if (hit > 0)
        {
            // Enable Level Complete Screen
            UIManager.Instance.EnableLevelWin();

            // Change level text.
            UIManager.Instance.AddLevel();
            
            // Increase the minimum and maximum diamonds range for the level.
            GameManager.minDiamonds = GameManager.minDiamonds >= 8 ? 8 : GameManager.minDiamonds + 1;
            GameManager.maxDiamonds = GameManager.maxDiamonds >= 10 ? 10 : GameManager.maxDiamonds + 1;

            // Set hit to 0 to count the number of balls hit on that level.
            hit = 0;
        }

        // Else it is the first level
        else
            UIManager.Instance.AddLevel(true); // Initialize == true

        // Enable the diamonds depending on the level
        // Get the number of diamonds to be enabled
        GameManager.numberOfDiamonds = Random.Range(GameManager.minDiamonds, GameManager.maxDiamonds + 1);

        int randomInt = Random.Range(0, 3);
        numberOfBalls = GameManager.numberOfDiamonds + randomInt >= 10 ? 10 : GameManager.numberOfDiamonds + randomInt;
        
        // Change the wheel and the number of diamonds.
        FindObjectOfType<Motor>().SetWheel();

        // Set the UI balls based on the number of diamonds.
        for (int i = 0; i < numberOfBalls; i++)
            balls.GetChild(balls.childCount - (i + 1)).gameObject.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // DO A BUNCH OF STUFF RELATED TO REDUCING WHEEL HEALTH
        if (GameManager.difficulty == GameManager.Difficulty.Medium && !PowerUps.powerUpActive && isActive)
            FindObjectOfType<Motor>().ReduceHealth();

        if (enableDiamondAnimation && GameManager.isBonusLevel)
        {
            AudioManager.Instance.Play("BreakDiamond");
            AnimateDiamonds(true);

            Invoke("DisableDiamonds", 1f);
        }
        
        // Create a new Instance of the ball after shooting this one.
        CreateNewBall();

        // Deflect the ball on hitting the wheel.
        GetComponent<CircleCollider2D>().enabled = false;
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(new Vector2(Random.Range(-10f, 10f), -force), ForceMode2D.Impulse);
            
        AudioManager.Instance.Play("HitWheel");
            
        // Shake the Wheel
        collision.transform.GetChild(0).GetComponent<Rigidbody2D>().AddForce(new Vector2(0, force - 18), ForceMode2D.Impulse);

        // Destroy ball after a while.
        Destroy(gameObject, 1f);

        if (PowerUps.powerUpActive)
            PowerUps.powerUpActive = false;

        // If no balls are left after shooting this one then Game Over.
        if (NoBallsLeft() && !GameManager.isLevelComplete)
        {
            GameManager.isGameOver = true;
            Invoke("GameOver", .5f);
        }

        try { FindObjectOfType<GameplayButton>().hitValueChecked = true; }

        catch (System.Exception e) { Debug.LogException(e); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Diamond"))
        {
            isActive = false;

            // Disable the Diamond and enable the Breaking animation
            collision.transform.GetComponent<SpriteRenderer>().enabled = false;
            collision.enabled = false;
            collision.transform.GetChild(0).gameObject.SetActive(true);

            UIManager.Instance.AddScore();
            
            AudioManager.Instance.Play("BreakDiamond");

            // Increase the number of diamonds hit.
            diamondsHit++;

            Destroy(collision.gameObject, .5f);

            // If all the diamonds hit then Level Complete.
            if (diamondsHit == GameManager.numberOfDiamonds)
                LevelComplete();
        }

        else if (collision.gameObject.CompareTag("SpikeCollider"))
        {
            enableDiamondAnimation = false;
        }
    }

    void CreateNewBall()
    {
        GameObject newBall = Instantiate(gameObject);
        newBall.name = "Ball";
        newBall.transform.position = new Vector3(0, -3.5f, 0);
        newBall.transform.rotation = Quaternion.identity;
        UIManager.Instance.ball = newBall;
    }

    bool NoBallsLeft()
    {
        if (hit == numberOfBalls && GameManager.isBonusLevel)
        {
            GameManager.isBonusLevel = false;
            LevelComplete();
            return false;
        }

        else if (hit == numberOfBalls)
            return true;

        return false;
    }

    public void LevelComplete()
    {
        GameManager.isLevelComplete = true;

        diamondsHit = 0;
        // Decrease hit variable so that if the last ball hits the wheel, Game Over function is not called.
        hit = 1;
        Invoke("Initialize", .5f);
    }

    void GameOver() { UIManager.Instance.EnableGameOver(); }

    public void ContinueLevelSetup(int extraBalls)
    {
        for (int i = 0; i < extraBalls; i++)
            balls.GetChild(balls.childCount - (i + 1)).gameObject.SetActive(true);
    }

    void DisableDiamonds()
    {
        AnimateDiamonds();
    }

    void AnimateDiamonds(bool enable = false)
    {
        int numberOfDiamonds = (FindObjectOfType<Motor>().bonusLevelNumber + 1) * 5;
        UIManager.Instance.AddScore(false, numberOfDiamonds);

        UIManager.Instance.EnableDiamonds(numberOfDiamonds, enable);
    }
}