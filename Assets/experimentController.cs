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

[System.Serializable]
public class coinSpawn
{
    public GameObject coin;
    public GameObject spawn;
    
    public coinSpawn(GameObject coin, GameObject spawn)
    {
        this.coin = coin;
        this.spawn = spawn;
    }
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
    public int maxTotalCoins = 5;
    public float timeBeforeSpawn = 90.0f;
    public bool spawnCoins = false;
    public float timeToSpawn = 30.0f;

    public GameObject experimentEnv;
    public GameObject participantUI;

    public List<GameObject> availSpawn = new List<GameObject>();
    public List<coinSpawn> coinSpawnList = new List<coinSpawn>();


    private PROGRAM_STATUS currentStatus = PROGRAM_STATUS.TEST;

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

        for(int i = 0; i < coinSpawns.Count - 1; i++)
        {
            availSpawn.Add(coinSpawns[i]);
        }

        SetNextVE();
    }

    // Update is called once per frame
    void Update()
    {
        if ((OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && OVRInput.Get(OVRInput.Button.SecondaryThumbstick)) || Input.GetKeyDown(KeyCode.E)) SetNextVE();
        if ((OVRInput.Get(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.RawButton.X)) || Input.GetKeyDown(KeyCode.R)) ResetCoins();

        if (currentStatus == PROGRAM_STATUS.TEST || currentStatus == PROGRAM_STATUS.TRIAL)
        {
            if (!spawnCoins)
            {
                if (timeElapsed >= timeBeforeSpawn)
                {
                    spawnCoins = true;
                    timeElapsed = 0.0f;
                }

                timeElapsed += Time.deltaTime;
            }

            if (coinSpawnCount < maxCoins && spawnCoins)
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
            SetNextVE();
            buttonController.instance.SetActive(false);
            currentStatus = PROGRAM_STATUS.TRIAL;
        }
    }

    public void UIStart(string PID)
    {
        if(PID != "")
        {
            participantID = int.Parse(PID);
            experimentEnv.SetActive(true);
            participantUI.SetActive(false);
            currentStatus = PROGRAM_STATUS.TRIAL;
            SetNextVE();
        }
    }

    private void SetNextVE()
    {
        DeleteCoins();
        trialNumber++;
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
            //ovrManager.isInsightPassthroughEnabled = false;
            centerEye.clearFlags = CameraClearFlags.Skybox;
        } else {
            //ovrManager.isInsightPassthroughEnabled = true;
            centerEye.clearFlags = CameraClearFlags.SolidColor;
        }

        coinSpawnCount = 0;
        coinCount = 0;
        timeElapsed = 0.0f;
    }

    private void SpawnCoin()
    {
        if (coinSpawnCount < maxCoins)
        {
            int spawnPoint = Random.Range(0, availSpawn.Count);
            GameObject newCoin = null;

            if (toonVE)
            {
                newCoin = Instantiate(toonCoinPrefab, availSpawn[spawnPoint].transform.position, Quaternion.identity);
            }
            else
            {
                newCoin = Instantiate(realCoinPrefab, availSpawn[spawnPoint].transform.position, Quaternion.identity);
            }

            coinSpawn newSpawn = new coinSpawn(newCoin, availSpawn[spawnPoint]);
            coinSpawnList.Add(newSpawn);
            availSpawn.Remove(availSpawn[spawnPoint]);  

            coinSpawnCount++;
        }
    }

    public void AddCoinCount(GameObject coin) 
    {
        foreach (coinSpawn spawn in coinSpawnList)
        {
            if(spawn.coin == coin)
            {
                availSpawn.Add(spawn.spawn);
                coinSpawnList.Remove(spawn);
                break;
            }
        }

        Destroy(coin);
        coinSpawnCount--;
        coinCount++;
    }

    private void ResetCoins()
    {
        foreach (coinSpawn spawn in coinSpawnList)
        {
            spawn.coin.transform.position = spawn.spawn.transform.position;
        }
    }

    public void ResetCoin(GameObject coin)
    {
        foreach (coinSpawn spawn in coinSpawnList)
        {
            if (spawn.coin == coin)
            {
                coin.transform.position = spawn.spawn.transform.position;
                break;
            }
        }
    }

    private void DeleteCoins()
    {
        foreach(coinSpawn spawn in coinSpawnList)
        {
            availSpawn.Add(spawn.spawn);
            Destroy(spawn.coin);
        }

        coinSpawnList.Clear();
    }
}
