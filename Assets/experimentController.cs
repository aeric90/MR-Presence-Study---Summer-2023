using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PROGRAM_STATUS
{
    START,
    TRIAL,
    POST_TRIAL,
    END
}

public class experimentController : MonoBehaviour {

    public Material skyBoxMat;
    public OVRManager ovrManager;
    public List<GameObject> conditions = new List<GameObject>();

    private PROGRAM_STATUS currentStatus = PROGRAM_STATUS.START;

    public int[,] conditionSquare = {   {0, 1, 3, 4, 5, 2 }, 
                                        {1, 4, 0, 2, 3, 5 }, 
                                        {4, 2, 1, 5, 0, 3 }, 
                                        {2, 5, 4, 3, 1, 0 },
                                        {5, 3, 2, 0, 4, 1 },
                                        {3, 0, 5, 1, 2, 4 }};

    private int participantID = 0;
    private int trialNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonPush()
    {
        if(currentStatus == PROGRAM_STATUS.POST_TRIAL)
        {
            //  TRANSITION TO NEW TRIAL
        }
    }
}
