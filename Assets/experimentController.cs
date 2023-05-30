using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PROGRAM_STATUS
{
    TEST,
    START,
    TRIAL,
    POST_TRIAL,
    END
}

public class experimentController : MonoBehaviour {

    public static experimentController instance;

    public Material skyBoxMat;
    public OVRManager ovrManager;
    public OVRPassthroughLayer passthroughLayer;
    public Camera centerEye;
    public List<GameObject> conditions = new List<GameObject>();
    public GameObject realCoinPrefab;
    public GameObject toonCoinPrefab;
    public int maxCoins = 5;
    public float timeToSpawn = 1.0f;


    private PROGRAM_STATUS currentStatus = PROGRAM_STATUS.TRIAL;

    public int[,] conditionSquare = {   {0, 1, 3, 4, 5, 2 }, 
                                        {1, 4, 0, 2, 3, 5 }, 
                                        {4, 2, 1, 5, 0, 3 }, 
                                        {2, 5, 4, 3, 1, 0 },
                                        {5, 3, 2, 0, 4, 1 },
                                        {3, 0, 5, 1, 2, 4 }};

    public List<GameObject> coinSpawns = new List<GameObject>();

    private int participantID = 1;
    private int trialNumber = 0;
    private int currentVE = -1;
    private bool toonVE = false;
    private int coinCount = 0;
    private int coinSpawnCount = 0;
    private float timeElapsed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetNextVE();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One) && currentStatus == PROGRAM_STATUS.TEST) SetNextVE();

        if (currentStatus == PROGRAM_STATUS.TEST || currentStatus == PROGRAM_STATUS.TRIAL)
        {
            if (coinSpawnCount < maxCoins)
            {
                if (timeElapsed >= timeToSpawn)
                {
                    SpawnCoin();
                    timeElapsed = 0.0f;
                }

                timeElapsed += Time.deltaTime;
            }

            if(coinCount >= maxCoins)
            {
                if (trialNumber < 5)
                {
                    buttonController.instance.SetActive(true);
                    currentStatus = PROGRAM_STATUS.POST_TRIAL;
                } else
                {
                    buttonController.instance.SetEnd();
                    currentStatus = PROGRAM_STATUS.END;
                }

            }
        }
    }

    public void buttonPush()
    {
        if(currentStatus == PROGRAM_STATUS.POST_TRIAL)
        {
            trialNumber++;
            SetNextVE();
            buttonController.instance.SetActive(false);
            currentStatus = PROGRAM_STATUS.TRIAL;
        }
    }

    private void SetNextVE()
    {
        if (currentStatus == PROGRAM_STATUS.TEST || participantID == 0)
        {
            if(trialNumber >= conditions.Count) trialNumber = 0;
            ChangeVE(trialNumber);
        } else
        {
            ChangeVE(conditionSquare[participantID % 6, trialNumber]);
        }
    }

    private void ChangeVE(int newVE)
    {
        if(currentVE >= 0) conditions[currentVE].SetActive(false);
        currentVE = newVE;
        conditions[currentVE].SetActive(true);
        if(conditions[currentVE].name.Contains("Real")) {
            passthroughLayer.edgeRenderingEnabled = false;
            toonVE = false;
        } else {
            passthroughLayer.edgeRenderingEnabled = true;
            toonVE = true;
        }

        if (conditions[currentVE].name.Contains("High")) {
            ovrManager.isInsightPassthroughEnabled = false;
            centerEye.clearFlags = CameraClearFlags.Skybox;
        } else {
            ovrManager.isInsightPassthroughEnabled = true;
            centerEye.clearFlags = CameraClearFlags.SolidColor;
        }

        coinSpawnCount = 0;
        coinCount = 0;
        timeElapsed = 0.0f;
    }

    private void SpawnCoin()
    {
        int spawnPoint = Random.Range(0, coinSpawns.Count);

        if (toonVE)
        {
            Instantiate(toonCoinPrefab, coinSpawns[6].transform.position, Quaternion.identity);
        } else
        {
            Instantiate(realCoinPrefab, coinSpawns[6].transform.position, Quaternion.identity);
        }
          
        coinSpawnCount++;
    }

    public void AddCoinCount()
    {
        coinCount++;
    }
}
