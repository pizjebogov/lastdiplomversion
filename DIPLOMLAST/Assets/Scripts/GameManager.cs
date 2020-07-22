using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;
using System.Runtime.ExceptionServices;

public class GameManager : MonoBehaviour
{
    public GameObject head, body, legs, spine,upperpart;
    public string mode, state, pose;
    /*
     mode: HeadMode,BodyMode,LegMode, SpineMode, GoingZeroMode,ProgramWave  mb ChairPose,ChairModifiedPose,DinnerPose,Verticalize, Mode5
     state: Calibrating,Options,Quit,SwitchingPose
     pose: Head,Body,Leg,Spine
     */
    public bool blockedhead, blockedbody, blockedspine;
    public bool up, down;
    public float headangle, bodyangle, legangle, spineangle,localheadangle,localbodyangle,locallegangle,localspineangle;
    public float rotationspeed;
    public GameObject anchorheadbody, anchorbodylegs;
    public float AngularHeadLimit, AngularBodyLimit, AngularSpineLimit;
    public Interface Interface;
    public float motiondegrees,variant,percentage;
    public float[] ZeroAngles = new float[4];
    public float wavecount;
    public string ip, port, id, information;
    public bool connectedtoserver;
    void Start()
    {
        rotationspeed = 10;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        headangle=Convert.ToSingle(Math.Round(head.transform.eulerAngles.z > 180 ? -(head.transform.eulerAngles.z - 360) : -head.transform.eulerAngles.z,0));
        bodyangle= Convert.ToSingle(Math.Round(body.transform.eulerAngles.z > 180 ? (body.transform.eulerAngles.z - 360) : body.transform.eulerAngles.z, 0));
        legangle=Convert.ToSingle(Math.Round(legs.transform.eulerAngles.z > 180 ? (legs.transform.eulerAngles.z - 360) : legs.transform.eulerAngles.z, 0));
        spineangle = Convert.ToSingle(Math.Round(spine.transform.eulerAngles.z > 180 ? -(spine.transform.eulerAngles.z - 360) : -spine.transform.eulerAngles.z, 0));
        localheadangle = Convert.ToSingle(Math.Round(head.transform.localEulerAngles.z > 180 ? -(head.transform.localEulerAngles.z - 360) : -head.transform.localEulerAngles.z, 0));
        localbodyangle = Convert.ToSingle(Math.Round(body.transform.localEulerAngles.z > 180 ? (body.transform.localEulerAngles.z - 360) : body.transform.localEulerAngles.z, 0));
        locallegangle = Convert.ToSingle(Math.Round(legs.transform.localEulerAngles.z > 180 ? (legs.transform.localEulerAngles.z - 360) : legs.transform.localEulerAngles.z, 0));
        localspineangle = Convert.ToSingle(Math.Round(spine.transform.localEulerAngles.z > 180 ? -(spine.transform.localEulerAngles.z - 360) : -spine.transform.localEulerAngles.z, 0));
        if (Input.GetMouseButton(0) && (up || down))
        {
            updownrotate();
        }
        else if (Input.GetMouseButtonUp(0) && (up || down))
        {
            up = false;
            down = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PoseStaying("Chair"));
        }
        

    }
    public void blocksomething(string something)
    {
        switch (something)
        {
            case ("Head"):
                if (blockedhead) 
                {
                    head.GetComponent<Rigidbody>().isKinematic = true;
                    head.GetComponent<Rigidbody>().useGravity = false;
                    Destroy(head.GetComponent<ConfigurableJoint>());
                    blockedhead = false;
                }
                else if (!blockedhead)
                {
                    AddJoint(head, spine);
                    blockedhead = true;
                }
                break;
            case ("Body"):
                if (blockedbody)
                {
                    body.GetComponent<Rigidbody>().isKinematic = true;
                    body.GetComponent<Rigidbody>().useGravity = false;
                    Destroy(body.GetComponent<ConfigurableJoint>());
                    legs.GetComponent<Rigidbody>().isKinematic = true;
                    legs.GetComponent<Rigidbody>().useGravity = false;
                    Destroy(legs.GetComponent<ConfigurableJoint>());
                    blockedbody = false;
                }
                else if (!blockedbody)
                {
                    AddJoint(body, spine);
                    AddJoint(legs, body);
                    blockedbody = true;
                }
                break;
            case ("Spine"):
                if (blockedspine)
                {
                    spine.GetComponent<HingeJoint>().connectedBody = upperpart.GetComponent<Rigidbody>();
                    spine.GetComponent<Rigidbody>().isKinematic = true;
                    spine.GetComponent<Rigidbody>().useGravity = false;
                    blockedspine = false;
                }
                else if (!blockedspine) 
                {
                    spine.GetComponent<HingeJoint>().connectedBody = null;
                    spine.GetComponent<Rigidbody>().isKinematic = false;
                    spine.GetComponent<Rigidbody>().useGravity = true;
                    
                    blockedspine = true;
                }

                break;
        }
    }

    public void AddJoint(GameObject from, GameObject to)
    {
        ConfigurableJoint cj = from.AddComponent<ConfigurableJoint>();
        from.GetComponent<Rigidbody>().isKinematic = false;
        from.GetComponent<Rigidbody>().useGravity = true;
        cj.connectedBody = to.GetComponent<Rigidbody>();
        cj.xMotion = ConfigurableJointMotion.Locked;
        cj.yMotion = ConfigurableJointMotion.Locked;
        cj.zMotion = ConfigurableJointMotion.Locked;
        cj.angularXMotion = ConfigurableJointMotion.Locked;
        cj.angularYMotion = ConfigurableJointMotion.Locked;
        cj.angularZMotion = ConfigurableJointMotion.Locked;

    }
    public void switchmode(string whatmode)
    {
        if (whatmode == mode)
        {
            mode = null;
        }
        else
        {
            mode = whatmode;
        }
    }
    public void switchtostate(string whatstate)
    {
        if (whatstate == state)
        {
            state = null;
        }
        else
        {
            state = whatstate;
        }
    }
    public void switchupdown(string upordown)
    {
        if (upordown == "Up")
        {
            up = true;
            down = false;
        }
        else if (upordown == "Down")
        {
            up = false;
            down = true;
        }
    }

    public void updownrotate()
    {
        float newangle;
        float rotatedegrees = (rotationspeed * Time.deltaTime);
        if (up)
        {
            switch (mode)
            {
                case ("HeadMode"):
                    newangle = Mathf.Clamp(localheadangle + rotatedegrees, 0, AngularHeadLimit);
                    rotatedegrees = newangle - localheadangle;
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotatedegrees);
                    break;
                case ("BodyMode"):
                    newangle = Mathf.Clamp(localbodyangle + rotatedegrees, 0, AngularBodyLimit);
                    rotatedegrees = newangle - localbodyangle;
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotatedegrees);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, 1.95f* rotatedegrees);

                    break;
                case ("LegMode"):

                    newangle = Mathf.Clamp(locallegangle+rotatedegrees, (-2 * localbodyangle) - 1, 1);
                    rotatedegrees = newangle - locallegangle;
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotatedegrees);
                    break;
                case ("SpineMode"):
                    newangle = Mathf.Clamp((spineangle + rotatedegrees), -15, AngularSpineLimit);
                    rotatedegrees = newangle - spineangle;
                    spine.transform.Rotate(Vector3.back, rotatedegrees);
                    break;
            }
        }
        else if (down)
        {
            switch (mode)
            {
                case ("HeadMode"):
                    newangle = Mathf.Clamp(localheadangle - rotatedegrees, 0, AngularHeadLimit);
                    rotatedegrees = Mathf.Abs(localheadangle - newangle);
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotatedegrees);
                    break;
                case ("BodyMode"):
                    if(localbodyangle * -2 <= locallegangle+3 && localbodyangle *-2 >=locallegangle-3) { 
                    newangle = Mathf.Clamp(localbodyangle - rotatedegrees, 0, AngularBodyLimit);
                    rotatedegrees =Mathf.Abs(localbodyangle - newangle);
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotatedegrees);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, 1.95f * rotatedegrees);
                    }
                    else {Interface.InfoModeText.GetComponent<Text>().text="You should calibrate legs to " + localbodyangle*-2 + " degrees" ; }
                    
                    break;
                case ("LegMode"):
                    newangle = Mathf.Clamp(locallegangle - rotatedegrees, (-2 * localbodyangle)-1, 1);
                    rotatedegrees = Mathf.Abs(locallegangle - newangle);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, rotatedegrees);
                    break;
                case ("SpineMode"):
                    newangle = Mathf.Clamp(spineangle - rotatedegrees, -15, AngularSpineLimit);
                    rotatedegrees = spineangle-newangle;
                    spine.transform.Rotate(Vector3.forward, rotatedegrees);
                    break;
            }
        }
    }

    public void goingtoangle(float angle)
    {
        switch (mode)
        {
            case ("HeadMode"):
                if (localheadangle > angle)
                {
                    motiondegrees = Mathf.Abs(angle-localheadangle);
                }
                else
                {
                    motiondegrees = localheadangle-angle;
                }
                state = "Calibrating";
                InvokeRepeating("rotatingtocurrentangle", 0.1f, 0.1f);
                break;
            case ("BodyMode"):
                if (localbodyangle > angle)
                {
                    motiondegrees =  Mathf.Abs(angle-localbodyangle);
                }
                else
                {
                    motiondegrees = localbodyangle-angle;
                }
                if (localbodyangle * -2 <= locallegangle + 3 && localbodyangle * -2 >= locallegangle - 3)
                {
                    state = "Calibrating";
                    InvokeRepeating("rotatingtocurrentangle", 0.1f, 0.1f);
                }
                else
                {
                    Interface.InfoModeText.GetComponent<Text>().text = "You should calibrate legs to " + localbodyangle * -2 + " degrees";
                }
                break;
            case ("LegMode"):
                if (locallegangle > angle)
                {
                    motiondegrees = Mathf.Abs(angle-locallegangle);
                }
                else
                {
                    motiondegrees = locallegangle-angle;
                }
                state = "Calibrating";
                InvokeRepeating("rotatingtocurrentangle", 0.1f, 0.1f);
                break;
            case ("SpineMode"):
                if (localspineangle > angle)
                {
                    motiondegrees = Mathf.Abs(angle-localspineangle);
                }
                else
                {
                    motiondegrees = localspineangle-angle;
                }
                state = "Calibrating";
                InvokeRepeating("rotatingtocurrentangle", 0.1f, 0.1f);
                break;
        }
        
    }
    public void rotatingtocurrentangle()
    {
        if (motiondegrees < -rotationspeed / 20)
        {
            switch (mode)
            {
                case ("HeadMode"):
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed / 20);
                    motiondegrees += rotationspeed / 20;
                    break;
                case ("BodyMode"):
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed / 20);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, 1.95f * rotationspeed / 20);
                    motiondegrees += rotationspeed / 20;
                    break;
                case ("LegMode"):
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotationspeed / 20);
                    motiondegrees += rotationspeed / 20;
                    break;
                case ("SpineMode"):
                    spine.transform.Rotate(Vector3.back, rotationspeed / 20);
                    motiondegrees += rotationspeed / 20;
                    break;

            }
        }
        else if(motiondegrees > rotationspeed / 20)
        {
            switch (mode)
            {
                case ("HeadMode"):
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed / 20);
                    motiondegrees -= rotationspeed / 20;
                    break;
                case ("BodyMode"):
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed / 20);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, 1.95f * rotationspeed / 20);
                    motiondegrees -= rotationspeed / 20;
                    break;
                case ("LegMode"):
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, rotationspeed / 20);
                    motiondegrees -= rotationspeed / 20;
                    break;
                case ("SpineMode"):
                    spine.transform.Rotate(Vector3.forward, rotationspeed / 20);
                    motiondegrees -= rotationspeed / 20;
                    break;

            }
        }
        else
        {
            motiondegrees = 0;
            CancelInvoke("rotatingtocurrentangle");
            state = null;
        }
    }
    public void stoprotatingtoangle()
    {
        motiondegrees = 0;
        CancelInvoke("rotatingtocurrentangle");
        state = null;
    }

    public void CountZero()
    {
        ZeroAngles[0] = localheadangle;
        if (localbodyangle * -2 == locallegangle)
        {
            variant = 1;
            ZeroAngles[1] =  localbodyangle;
            ZeroAngles[2] = 0;

        }
        else
        {
            variant = 2;
            ZeroAngles[1] = localbodyangle;
            ZeroAngles[2] = Mathf.Abs((localbodyangle * -2) + Mathf.Abs(locallegangle));
        }
        if (localspineangle > 0)
        {
            ZeroAngles[3] = localspineangle;
        }
        else
        {
            ZeroAngles[3] = Mathf.Abs(localspineangle);
        }
        for(int i = 0; i < 4; i++)
        {
            percentage += ZeroAngles[i];
        }
    }
    public void GoingZero()
    {
        if(ZeroAngles[0]>rotationspeed/20 || ZeroAngles[1] > rotationspeed / 20 || ZeroAngles[2] > rotationspeed / 20 || ZeroAngles[3] > rotationspeed / 20)
        {
            if (variant == 1)
            {
                if(ZeroAngles[0] > rotationspeed / 20)
                {
                    pose = "Head";
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed / 20);
                    ZeroAngles[0] -= rotationspeed / 20;
                }
                else if (ZeroAngles[1] > rotationspeed / 20)
                {
                    pose = "Body";
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed / 20);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotationspeed / 10);
                    ZeroAngles[1] -= rotationspeed / 20;
                }
                else if (ZeroAngles[3] > rotationspeed / 20)
                {
                    pose = "Spine";
                    if (localspineangle > rotationspeed/20) 
                    {
                        spine.transform.Rotate(Vector3.forward, rotationspeed / 20);
                        ZeroAngles[3] -= rotationspeed / 20;
                    }
                    else if (localspineangle < -rotationspeed / 20)
                    {
                        spine.transform.Rotate(Vector3.back, rotationspeed / 20);
                        ZeroAngles[3] -= rotationspeed / 20;
                    }
                }
            }
            else if (variant == 2)
            {
                if (ZeroAngles[0] > rotationspeed / 20)
                {
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed / 20);
                    ZeroAngles[0] -= rotationspeed / 20;
                    pose = "Head";
                }

                else if (ZeroAngles[2] > rotationspeed / 20)
                {
                    if (locallegangle < -2 * localbodyangle)
                    {
                        legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotationspeed / 20);
                        ZeroAngles[2] -= rotationspeed / 20;
                        pose = "Legs";
                    }
                    else
                    {
                        legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, rotationspeed / 20);
                        ZeroAngles[2] -= rotationspeed / 20;
                        pose = "Legs";
                    }
                }
                else if (ZeroAngles[1] > rotationspeed / 20)
                {
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed / 20);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotationspeed / 10);
                    ZeroAngles[1] -= rotationspeed / 20;
                    pose = "Body";
                }
                else if (ZeroAngles[3] > rotationspeed / 20)
                {
                    pose = "Spine";
                    if (localspineangle > rotationspeed / 20)
                    {
                        spine.transform.Rotate(Vector3.forward, rotationspeed / 20);
                        ZeroAngles[3] -= rotationspeed / 20;
                    }
                    else if (localspineangle < -rotationspeed / 20)
                    {
                        spine.transform.Rotate(Vector3.back, rotationspeed / 20);
                        ZeroAngles[3] -= rotationspeed / 20;
                    }
                }
            }
        }
        else
        {
            CancelInvoke("GoingZero");
            for (int i = 0; i < 4; i++)
            {
                ZeroAngles[i] = 0;
            }
            if (state == "Quit")
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }
            else if(state=="Chair"|| state == "ModifiedChair" || state == "Dinner" || state == "Verticalize" || state == "Mode5")
            {
                pose = null;
                StartCoroutine(PoseStaying(state));
            }
            if (mode == "ProgramWave")
            {
                pose = null;
                InvokeRepeating("WaveProg", 0.1f, 0.1f);
            }
            else if (mode == "GoingZeroMode")
            {
                pose = null;
                state = null;

            }
        }
    }
    public IEnumerator PoseStaying(string whatpose)
    {
        switch (whatpose)
        {
            case ("Chair"):
                pose = "Head";
                for (int i = 0; i < 45; i+=1)
                {
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, 1);
                    yield return new WaitForSeconds(0.1f);
                }
                pose = "Body";
                for(int i = 0; i < 20; i++)
                {
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, 1);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, 2);
                    yield return new WaitForSeconds(0.1f);
                }
                state = null;
                mode = null;
                pose = null;
                StopCoroutine(PoseStaying("Chair"));
                break;
            case ("ModifiedChair"):
                pose = "Head";
                for (int i = 0; i < 45; i += 1)
                {
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, 1);
                    yield return new WaitForSeconds(0.1f);
                }
                pose = "Body";
                for (int i = 0; i < 20; i++)
                {
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, 1);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, 2);
                    yield return new WaitForSeconds(0.1f);
                }
                pose = "Spine";
                for (int i = 0; i < 10; i++)
                {
                    spine.transform.Rotate(Vector3.forward, 1);
                    yield return new WaitForSeconds(0.1f);
                }
                state = null;
                mode = null;
                pose = null;
                StopCoroutine(PoseStaying("ModifiedChair"));
                break;
            case ("Dinner"):
                pose = "Head";
                for (int i = 0; i < 45; i += 1)
                {
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, 1);
                    yield return new WaitForSeconds(0.1f);
                }
                
                state = null;
                mode = null;
                pose = null;
                StopCoroutine(PoseStaying("Dinner"));
                break;
            case ("Verticalize"):
                pose = "Spine";
                for (int i = 0; i < 15; i++)
                {
                    spine.transform.Rotate(Vector3.back, 1);
                    yield return new WaitForSeconds(0.1f);
                }
                state = null;
                mode = null;
                pose = null;
                StopCoroutine(PoseStaying("Verticalize"));
                break;
        }
    }
    public void calibrateall()
    {
        CountZero();
        InvokeRepeating("GoingZero", 0.1f, 0.1f);
    }

    public void WaveProg()
    {
        if (wavecount > 0)
        {
            if(wavecount<=200 && wavecount > 170)
            {
                head.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed / 20);
                wavecount -= rotationspeed / 20;
            }
            else if(wavecount<=170 && wavecount > 160)
            {
                wavecount -= rotationspeed / 20;
            }
            else if(wavecount<=160 && wavecount > 130)
            {
                body.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed / 20);
                legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, rotationspeed / 10);
                wavecount -= rotationspeed / 20;
            }
            else if (wavecount <= 130 && wavecount > 110)
            {
                legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotationspeed / 20);
                wavecount -= rotationspeed / 50;
            }
            else if(wavecount <=110 && wavecount > 100)
            {
                wavecount -= rotationspeed / 20;
            }
            else if(wavecount<=100 && wavecount > 90)
            {
                spine.transform.Rotate(Vector3.back, rotationspeed / 20);
                wavecount -= rotationspeed / 20;
            }
            else if(wavecount<=90 && wavecount > 60)
            {
                head.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed / 20);
                wavecount -= rotationspeed / 20;
            }
            else if (wavecount <= 60 && wavecount > 40)
            {
                legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, rotationspeed / 20);
                wavecount -= rotationspeed / 50;
            }
            else if (wavecount <= 40 && wavecount > 10)
            {
                body.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed / 20);
                legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotationspeed / 10);
                wavecount -= rotationspeed / 20;
            }
            else if (wavecount <= 10 && wavecount > 0)
            {
                spine.transform.Rotate(Vector3.forward, rotationspeed / 20);
                wavecount -= rotationspeed / 20;
            }
        }
        else
        {
            wavecount = 200;
        }
    }
    public void StartProg()
    {
        state = "Calibrating";
        pose = null;
        calibrateall();

    }
    public void StopProg()
    {
        state = null;
        pose = null;
        CancelInvoke("GoingZero");
        CancelInvoke("WaveProg");
        wavecount = 0;
    }
    public void poseorquit(string whatweneed)
    {
        state = whatweneed;
        calibrateall();
    }

    public IEnumerator Sendinfo(string neededinformation) 
    {
        if (connectedtoserver && Application.internetReachability != NetworkReachability.NotReachable)
        {
            string URL = "http://" + ip + ":" + port;
            WWWForm form = new WWWForm();
            form.AddField("ID", id);
            form.AddField("Time", Time.time.ToString());
            form.AddField("Information", neededinformation);
            WWW www = new WWW(URL, form);
            yield return www;
            Debug.Log("Server answer:" + www.text);
        }
    }
}
