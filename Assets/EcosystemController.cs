using System.Collections;
using UnityEngine;

public class EcosystemController : MonoBehaviour
{
    /*
     Let me explain the project the ecosystem a little bit. Each organism: bird, fish, and crab, have 3 states:
    Bird - fly, hover (stop), catch (move towards the fish and fly up)
    Fish - swim, stop, jump out of the water
    Crab - walk, flip, stop and build a sand castle

    They all interact with each other in different ways: bird flies around looking for fish until it catches the fish and flies away and despawns off screen. new one comes in and doesn the same thing.
    Fish swims around until it gets caught by the bird and dies. Then a new one comes in. 
    And a crab doesn't interact with them, but crab walks in, does whatever (because that's what crabs do), build something out of sand or dig and break something out of sand
    and then walk off screen where it despawns and a new one comes in.

    they all have sounds (bird makes hawk sounds, fish makes splashing noise when it swims, and crab has mr. krabs sound
    sandcastle is not an organism, its more of an environment interaction because crab doesn't interact with bird or fish.
     */
    
    public float speed = 10f;

    public GameObject crabPrefab;
    public AudioClip krabsWalking;
    private Vector2 crabSpawnPoint = new Vector2(-11.25f, -1.93f);
    private Vector2 crabFirstTarget = new Vector2(3.25f, -4f);
    private Vector2 crabPausePoint = new Vector2(6f, -4f);
    private Vector2 crabSecondTarget = new Vector2(15f, -4f);

    public GameObject fishPrefab;
    public AudioClip waterSplashSound;
    private Vector2 fishSpawnPoint = new Vector2(11.27f, 0.12f);
    private Vector2 fishFirstTarget = new Vector2(4.83f, -0.13f);
    private Vector2 fishSecondTarget = new Vector2(0.47f, 0.15f);
    private Vector2 fishPausePoint = new Vector2(-5.8f, 0.36f);
    private Vector2 fishFinalTarget = new Vector2(-6.26f, 1.75f);

    public GameObject birdPrefab;
    public AudioClip hawkScream;
    private Vector2 birdSpawnPoint = new Vector2(-11.18f, 3.63f);
    private Vector2 birdFirstTarget = new Vector2(-1.59f, 3.2f);
    private Vector2 birdSecondTarget = new Vector2(8.02f, 3.54f);
    private Vector2 birdThirdTarget = new Vector2(-6.25f, 3.61f);
    private Vector2 birdPauseTarget = new Vector2(-6.25f, 3.61f);
    private Vector2 birdFinalTarget = new Vector2(-9.42f, 6.82f);

    public GameObject sandcastlePrefab;
    private Vector2 sandcastlePosition = new Vector2(5.85f, -2.85f);

    void Start()
    {
        SpawnCrab();
        SpawnBird();
        SpawnFish();
    }
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Crab").Length == 0)
        {
            SpawnCrab();
        }
        if (GameObject.FindGameObjectsWithTag("Fish").Length == 0)
        {
            SpawnFish();
        }

        if (GameObject.FindGameObjectsWithTag("Bird").Length == 0)
        {
            SpawnBird();
        }
    }

    private void SpawnCastle()
    {
        if (sandcastlePrefab == null)
        {
            Debug.LogWarning("Sandcastle prefab is not assigned!");
            return;
        }

        GameObject existingSandcastle = GameObject.FindWithTag("Sandcastle");
        if (existingSandcastle == null)
        {
            Instantiate(sandcastlePrefab, sandcastlePosition, Quaternion.identity);
            Debug.Log("Sandcastle instantiated at " + sandcastlePosition);
        }
        else
        {
            Destroy(existingSandcastle);
            Debug.Log("Existing sandcastle destroyed.");
        }
    }

    private void SpawnBird()
    {
        if (birdPrefab != null)
        {
            GameObject bird = Instantiate(birdPrefab, birdSpawnPoint, Quaternion.identity);
            EcosystemController birdController = bird.AddComponent<EcosystemController>();
            birdController.speed = speed;
            birdController.StartCoroutine(birdController.BirdBehavior());
        }
    }

    private void SpawnCrab()
    {
        if (crabPrefab != null)
        {
            GameObject crab = Instantiate(crabPrefab, crabSpawnPoint, Quaternion.identity);
            EcosystemController crabController = crab.GetComponent<EcosystemController>();
            if (crabController == null)
            {
                crabController = crab.AddComponent<EcosystemController>();
            }
            crabController.speed = speed;
            crabController.sandcastlePrefab = sandcastlePrefab;
            crabController.StartCoroutine(crabController.CrabBehavior());
        }
    }

    private void SpawnFish()
    {
        if (fishPrefab != null)
        {
            GameObject bird = GameObject.FindGameObjectWithTag("Bird");
            if (bird == null || (bird.transform.position.x < -10))
            {
                if (fishPrefab != null)
                {
                    GameObject fish = Instantiate(fishPrefab, fishSpawnPoint, Quaternion.identity);
                    EcosystemController fishController = fish.AddComponent<EcosystemController>();
                    fishController.speed = speed;
                    fishController.StartCoroutine(fishController.FishBehavior());
                }
            }
        }
    }


        private IEnumerator BirdBehavior() {
         yield return StartCoroutine(MoveToPosition(birdFirstTarget));

        yield return StartCoroutine(MoveToPosition(birdSecondTarget));

          yield return StartCoroutine(MoveToPosition(birdThirdTarget));
        PlayHawk();

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(MoveToPosition(birdFinalTarget));
          Destroy(gameObject);
}


        private IEnumerator FishBehavior()
    {
        yield return StartCoroutine(MoveToPosition(fishFirstTarget));
        PlaySplash();

        yield return StartCoroutine(MoveToPosition(fishSecondTarget));

        yield return StartCoroutine(MoveToPosition(fishPausePoint));
            PlaySplash();
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(RotateToZ(-45.71f));
        PlaySplash();
        yield return StartCoroutine(MoveToFinalPositionWithRotation(fishFinalTarget, -63.04f));

        Destroy(gameObject);
    }

    private IEnumerator CrabBehavior()
    {
        PlayKrabs();
        yield return StartCoroutine(MoveToPosition(crabFirstTarget));

        yield return StartCoroutine(Flip());

        yield return StartCoroutine(MoveToPosition(crabPausePoint));
       // Debug.Log("A crab is being destroyed and a new one will spawn.");
        SpawnCastle();
        yield return new WaitForSeconds(5f);

        yield return StartCoroutine(MoveToPosition(crabSecondTarget));
        PlayKrabs();

        Debug.Log("A crab is being destroyed and a new one will spawn.");
        SpawnCrab();

        Destroy(gameObject);
        SpawnCrab();
    }

    private IEnumerator MoveToPosition(Vector2 target, bool playSound = false)
    {
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void PlaySplash()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        Debug.Log("PlaySplash called!");
        if (audioSource != null || waterSplashSound != null)
            {
            Debug.Log("Setting and playing splash sound");
            audioSource.Play();
            Debug.Log("Splash sound should be playing now!");
        }
        }

    private void PlayHawk()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        Debug.Log("Hawk called!");
        if (audioSource != null || hawkScream != null)
        {
            Debug.Log("Hawk Audio");
            audioSource.Play();
            Debug.Log("Hawk Played!");
        }
    }

    private void PlayKrabs()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        Debug.Log("Krabs called!");
        if (audioSource != null || hawkScream != null)
        {
            Debug.Log("Krabs audio");
            audioSource.Play();
            Debug.Log("Krabs Played!");
        }
    }

    private IEnumerator MoveToFinalPositionWithRotation(Vector2 target, float targetZ)
    {
        PlaySplash();
        float initialZ = transform.eulerAngles.z;
        float rotationDifference = targetZ - initialZ;
        float rotationSpeed = 20f;
        float moveSpeed = speed * Time.deltaTime;
        float step = 0;

        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed);
            step += rotationSpeed * Time.deltaTime;
            float newZ = Mathf.Lerp(initialZ, targetZ, step / 20f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newZ);

            yield return null;
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetZ);
    }

    private IEnumerator RotateToZ(float targetZ)
    {
        float startZ = transform.eulerAngles.z;
        float zDifference = targetZ - startZ;
        float rotationAmount = 0f;
        float rotationSpeed = 100f;

        while (Mathf.Abs(rotationAmount) < Mathf.Abs(zDifference))
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, step * Mathf.Sign(zDifference));
            rotationAmount += step * Mathf.Sign(zDifference);
            yield return null;
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetZ);
    }

    private IEnumerator Flip()
    {
        float rotationAmount = 0f;
        while (rotationAmount < 360f)
        {
            float rotateStep = 60f * Time.deltaTime;
            transform.Rotate(0, 0, rotateStep);
            rotationAmount += rotateStep;
            yield return null;
        }
    }
}