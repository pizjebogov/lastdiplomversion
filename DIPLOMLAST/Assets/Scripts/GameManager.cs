﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

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
    void Start()
    {
        rotationspeed = 10;
        AngularHeadLimit = 20;
        AngularSpineLimit = 45;
        AngularBodyLimit = 50;
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
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed * Time.deltaTime);
                    break;
                case ("BodyMode"):
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed * Time.deltaTime);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, 2* rotationspeed * Time.deltaTime);
                    break;
                case ("LegMode"):
                    
                    newangle = Mathf.Clamp(locallegangle + rotatedegrees, -10, 10);
                    rotatedegrees = newangle - locallegangle;
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, rotatedegrees);
                    break;
                case ("SpineMode"):
                    newangle = Mathf.Clamp(spineangle + rotatedegrees, -AngularSpineLimit, AngularSpineLimit);
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
                    head.transform.RotateAround(anchorheadbody.transform.position, Vector3.forward, rotationspeed * Time.deltaTime);
                    break;
                case ("BodyMode"):
                    body.transform.RotateAround(anchorheadbody.transform.position, Vector3.back, rotationspeed * Time.deltaTime);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.forward, 2*rotationspeed * Time.deltaTime);
                    break;
                case ("LegMode"):
                    newangle = Mathf.Clamp((locallegangle - rotatedegrees), -10, 10);
                    rotatedegrees = Mathf.Abs(newangle -locallegangle);
                    legs.transform.RotateAround(anchorbodylegs.transform.position, Vector3.back, rotatedegrees);
                    break;
                case ("SpineMode"):
                    newangle = Mathf.Clamp(spineangle + rotatedegrees, -AngularSpineLimit, AngularSpineLimit);
                    rotatedegrees = newangle - spineangle;
                    spine.transform.Rotate(Vector3.forward, rotatedegrees);
                    break;
            }
        }
    }
}
