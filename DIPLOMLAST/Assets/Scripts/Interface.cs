using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public GameManager gm;
    public Material normal, glowing, glowingday, glowingnight, daytheme, nighttheme;
    public string theme;
    public GameObject MedicalBed, InfoOptionsPanel, AllOptions,InfoModeText,DateTimePanel,QuitOptPanel,PoseWavePanel,MotionPanel,StartStopPanel ;
    public Sprite suntheme, moontheme;
    public GameObject block;
    public Sprite blocked, unblocked;
    public bool TimeSettedManually = false;
    public Slider Batteryslide;
    public GameObject modepanel;
    public Button[] MotionButtons = new Button[3];
    public Button[] ModeButtons = new Button[6];
    public Button[] StartStopButtons = new Button[2];
    public Vector3 beginpoint, endpoint, touchdirection;
    public GameObject anglepanel, anglevector;
    public GameObject[] anglebuttons = new GameObject[5];
    public DateTime dt;
    public GameObject InputTimeDate;
    public Text[] timedateinputs = new Text[6];
    public Button[] OptionButtons = new Button[3];
    void Start()
    {
        Calibrate(InfoOptionsPanel, Screen.width, Screen.height / 8, 0, -Screen.height / 16);
        Calibrate(AllOptions, Screen.width, Screen.height - Param(InfoOptionsPanel).y,0, -Screen.width / 16);
        
        //AllOptions.transform.position = new Vector3(0, 0, MedicalBed.transform.position.z - 1);
        AllOptions.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(AllOptions).x / 3, Param(AllOptions).y / 2);
        block.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(AllOptions).x / 9, Param(AllOptions).y / 4);
        Calibrate(InfoModeText, Param(InfoOptionsPanel).x * 0.6f, Param(InfoOptionsPanel).y, 0, 0);
        Calibrate(DateTimePanel, Param(InfoOptionsPanel).x * 0.2f, Param(InfoOptionsPanel).y, Param(InfoOptionsPanel).x * 0.1f, 0);
        Calibrate(QuitOptPanel, Param(InfoOptionsPanel).x * 0.2f, Param(InfoOptionsPanel).y, -Param(InfoOptionsPanel).x * 0.1f, 0);
        DateTimePanel.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(DateTimePanel).x / 2, Param(DateTimePanel).y);
        QuitOptPanel.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(QuitOptPanel).x / 4, Param(QuitOptPanel).y/2);
        Calibrate(modepanel, Screen.width, Screen.height / 6.5f, 0, Screen.height / 13);
        modepanel.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(modepanel).x / 6, Param(modepanel).y);
        Calibrate(PoseWavePanel, Screen.width * 2, Screen.height / 2, 0, 0);
        PoseWavePanel.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(PoseWavePanel).x / 6, Param(PoseWavePanel).y);
        Calibrate(MotionPanel, Screen.width / 6, Screen.height / 2, -Screen.width / 12, 0);
        MotionPanel.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(MotionPanel).x / 1.5f, Param(PoseWavePanel).y/3);
        Calibrate(StartStopPanel, Screen.width / 4, Screen.height / 2, Screen.width / 8, 0);
        StartStopPanel.transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(StartStopPanel).x / 2, Param(StartStopPanel).y / 2);
        Calibrate(anglepanel, Screen.width / 4, Screen.height / 3, Screen.width / 8, Screen.height / 6);
        for(int i = 0; i < 5; i += 1)
        {
            Calibrate(anglebuttons[i], Param(anglepanel).x / 5, Param(anglepanel).x / 5, -Param(anglepanel).x * 0.75f, i*15);
            anglebuttons[i].transform.RotateAround(anglevector.transform.position, Vector3.back,i*15);
            anglebuttons[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            anglebuttons[i].GetComponent<LineRenderer>().SetPosition(0, anglebuttons[i].transform.position);
            anglebuttons[i].GetComponent<LineRenderer>().SetPosition(1, anglevector.transform.position);
        }
        AllOptions.transform.Find("MaxAngle").GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(AllOptions).x / 3, Param(AllOptions).y / 6);
        StartCoroutine(Timeticks());
        AllOptions.transform.Find("CheckClock").GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(AllOptions).x / 3, Param(AllOptions).y / 6);
        InputTimeDate.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Param(AllOptions).x / 9, Param(AllOptions).y / 12);
    }

    // Update is called once per frame
    void Update()
    {
        checktheme();
        checkmaterials();
        checkblocking();
        CheckTime();
        CheckBattery();
        checkupdown();
        checktextinfo();
        checkstate();
        anglemaximum();
    }
    public void Calibrate(GameObject objectforcalib, float width,float height,float posx,float posy)
    {
        RectTransform rt = objectforcalib.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = new Vector2(posx, posy);
    }
    public Vector2 Param(GameObject itsparametres)
    {
        float width = itsparametres.GetComponent<RectTransform>().rect.width;
        float height = itsparametres.GetComponent<RectTransform>().rect.height;
        return new Vector2(width, height);
    }

    public void checkmaterials()
    {
        if (gm.mode == "HeadMode")
        {
            gm.head.GetComponent<Renderer>().material = glowing;
            gm.body.GetComponent<Renderer>().material = normal;
            gm.legs.GetComponent<Renderer>().material = normal;
            gm.spine.GetComponent<Renderer>().material = normal;
        }
        else if(gm.mode == "BodyMode")
        {
            gm.head.GetComponent<Renderer>().material = normal;
            gm.body.GetComponent<Renderer>().material = glowing;
            gm.legs.GetComponent<Renderer>().material = normal;
            gm.spine.GetComponent<Renderer>().material = normal;
        }
        else if (gm.mode == "LegMode")
        {
            gm.head.GetComponent<Renderer>().material = normal;
            gm.body.GetComponent<Renderer>().material = normal;
            gm.legs.GetComponent<Renderer>().material = glowing;
            gm.spine.GetComponent<Renderer>().material = normal;
        }
        else if (gm.mode == "SpineMode")
        {
            gm.head.GetComponent<Renderer>().material = normal;
            gm.body.GetComponent<Renderer>().material = normal;
            gm.legs.GetComponent<Renderer>().material = normal;
            gm.spine.GetComponent<Renderer>().material = glowing;
        }
        else
        {
            if (gm.pose == "Head")
            {
                gm.head.GetComponent<Renderer>().material = glowing;
                gm.body.GetComponent<Renderer>().material = normal;
                gm.legs.GetComponent<Renderer>().material = normal;
                gm.spine.GetComponent<Renderer>().material = normal;
            }
            else if (gm.pose == "Body")
            {
                gm.head.GetComponent<Renderer>().material = normal;
                gm.body.GetComponent<Renderer>().material = glowing;
                gm.legs.GetComponent<Renderer>().material = normal;
                gm.spine.GetComponent<Renderer>().material = normal;
            }
            else if (gm.pose == "Leg")
            {
                gm.head.GetComponent<Renderer>().material = normal;
                gm.body.GetComponent<Renderer>().material = normal;
                gm.legs.GetComponent<Renderer>().material = glowing;
                gm.spine.GetComponent<Renderer>().material = normal;
            }
            else if (gm.pose == "Spine")
            {
                gm.head.GetComponent<Renderer>().material = normal;
                gm.body.GetComponent<Renderer>().material = normal;
                gm.legs.GetComponent<Renderer>().material = normal;
                gm.spine.GetComponent<Renderer>().material = glowing;
            }
            else
            {
                gm.head.GetComponent<Renderer>().material = normal;
                gm.body.GetComponent<Renderer>().material = normal;
                gm.legs.GetComponent<Renderer>().material = normal;
                gm.spine.GetComponent<Renderer>().material = normal;
            }
        }
    }
    public void checktheme()
    {
        if (theme == "DayTheme")
        {
            Camera.main.GetComponent<Skybox>().material = daytheme;
            glowing = glowingday;
            AllOptions.transform.Find("Theme").GetComponent<Image>().sprite = suntheme;
            AllOptions.transform.Find("Theme").GetComponent<Image>().color = new Color(1, 0.81f, 0.28f);
        }
        else if (theme == "NightTheme")
        {
            Camera.main.GetComponent<Skybox>().material = nighttheme;
            glowing = glowingnight;
            AllOptions.transform.Find("Theme").GetComponent<Image>().sprite = moontheme;
            AllOptions.transform.Find("Theme").GetComponent<Image>().color = new Color(0.74f, 0.81f, 0.89f);
        }
    }
    public void switchtheme()
    {
        if (theme == "DayTheme")
        {
            theme = "NightTheme";
        }
        else if (theme == "NightTheme")
        {
            theme = "DayTheme";
        }
    }
    public void checkblocking()
    {
        if (gm.blockedhead)
        {
            block.transform.Find("BlockHead").GetComponent<Image>().sprite = blocked;
        }
        else if (!gm.blockedhead)
        {
            block.transform.Find("BlockHead").GetComponent<Image>().sprite = unblocked;
        }
        if (gm.blockedbody)
        {
            block.transform.Find("BlockBody").GetComponent<Image>().sprite = blocked;
        }
        else if (!gm.blockedbody)
        {
            block.transform.Find("BlockBody").GetComponent<Image>().sprite = unblocked;
        }
        if (gm.blockedspine) { 
            block.transform.Find("BlockSpine").GetComponent<Image>().sprite = blocked;
        }
        else if (!gm.blockedspine)
        {
            block.transform.Find("BlockSpine").GetComponent<Image>().sprite = unblocked;
        }
    }
    public void OpenClose(GameObject neededpanel)
    {
        if(neededpanel==AllOptions && PoseWavePanel.active == true)
        {
          //  gm.mode = null;
            PoseWavePanel.SetActive(false);
        }
        else if (neededpanel == PoseWavePanel && AllOptions.active == true)
        {
            AllOptions.SetActive(false);
        }
        neededpanel.SetActive(!neededpanel.activeSelf);
    }
    public void CheckTime()
    {
        
        if(Application.internetReachability != NetworkReachability.NotReachable)
        {
            TimeSettedManually = false;
            DateTimePanel.transform.Find("TimeText").GetComponent<Text>().text = DateTime.Now.ToString("HH:mm");
            DateTimePanel.transform.Find("DateText").GetComponent<Text>().text = DateTime.Now.ToString("dd:MM:yyyy");
        }
        else
        {
            if (!TimeSettedManually)
            {
                DateTimePanel.transform.Find("TimeText").GetComponent<Text>().text = ("Set Date Manually");
                DateTimePanel.transform.Find("DateText").GetComponent<Text>().text = ("Set Time Manually");
            }
            else
            {
                DateTimePanel.transform.Find("TimeText").GetComponent<Text>().text = dt.ToString("HH:mm");
                DateTimePanel.transform.Find("DateText").GetComponent<Text>().text = dt.ToString("dd:MM:yyyy");
            }
        }
    }
    public void CheckBattery()
    {
        if (SystemInfo.batteryLevel != -1)
        {
            float BatteryValue = Batteryslide.GetComponent<Slider>().value = SystemInfo.batteryLevel;
            if(BatteryValue<=1 && BatteryValue > 0.75f)
            {
                Batteryslide.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.green;
            }
            else if (BatteryValue <= 0.75f && BatteryValue > 0.5f)
            {
                Batteryslide.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.yellow;
            }
            else if (BatteryValue <= 0.5f && BatteryValue > 0.25f)
            {
                Batteryslide.transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(1,0.55f,0);
            }
            else if (BatteryValue <= 0.25f && BatteryValue > 0.0f)
            {
                Batteryslide.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.red;
            }
        }
        else
        {
            Batteryslide.GetComponent<Slider>().value = 1;
            Batteryslide.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.green;
        }
    }

    public void checkupdown()
    {
        if (gm.up)
        {
            MotionButtons[0].GetComponent<Image>().color = Color.green;
        }
        else if (gm.down)
        {
            MotionButtons[2].GetComponent<Image>().color = Color.red;
        }
        else
        {
            MotionButtons[0].GetComponent<Image>().color = Color.white;
            MotionButtons[2].GetComponent<Image>().color = Color.white;
        }
    }
    public void checktextinfo()
    {
        Text text = InfoModeText.GetComponent<Text>();
        text.color = Color.white;
        if(gm.mode=="HeadMode"|| gm.mode == "BodyMode" || gm.mode == "LegMode" || gm.mode == "SpineMode") 
        {
            anglepanel.SetActive(true);
            MotionPanel.SetActive(true);
            StartStopPanel.SetActive(false);
            if(gm.state!="Quit"|| gm.state != "SwitchingPose" || gm.state != "Options")
            {
                if (gm.mode == "HeadMode") 
                {
                    text.text = "Position:Head\tAngle:" + gm.localheadangle;
                    foreach (Button mode in ModeButtons)
                    {
                        mode.GetComponent<Image>().color = new Color(1,1,1,0.5f);
                        ModeButtons[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    foreach(GameObject angle in anglebuttons)
                    {
                        if(Convert.ToSingle(angle.gameObject.name) > gm.AngularHeadLimit)
                        {
                            angle.SetActive(false);
                        }
                        else
                        {
                            angle.SetActive(true);
                        }
                    }
                }
                else if (gm.mode == "BodyMode")
                {
                    text.text = "Position:Body\tAngle:" + gm.localbodyangle;
                    foreach (Button mode in ModeButtons)
                    {
                        mode.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                        ModeButtons[1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    foreach (GameObject angle in anglebuttons)
                    {
                        if (Convert.ToSingle(angle.gameObject.name) > gm.AngularBodyLimit)
                        {
                            angle.SetActive(false);
                        }
                        else
                        {
                            angle.SetActive(true);
                        }
                    }
                }
                else if (gm.mode == "LegMode")
                {
                    text.text = "Position:Legs\tAngle:" + gm.locallegangle;
                    foreach (Button mode in ModeButtons)
                    {
                        mode.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                        ModeButtons[2].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    anglepanel.SetActive(false);
                }
                else if (gm.mode == "SpineMode")
                {
                    text.text = "Position:Spine\tAngle:" + gm.spineangle;
                    foreach (Button mode in ModeButtons)
                    {
                        mode.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                        ModeButtons[3].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    foreach (GameObject angle in anglebuttons)
                    {
                        if (Convert.ToSingle(angle.gameObject.name) > gm.AngularSpineLimit)
                        {
                            angle.SetActive(false);
                        }
                        else
                        {
                            angle.SetActive(true);
                        }
                    }
                }


            }
            if (gm.state == "Calibrating")
            {
                MotionButtons[0].GetComponent<Button>().interactable = false;
                MotionButtons[2].GetComponent<Button>().interactable = false;
                MotionButtons[1].GetComponent<Button>().interactable = true;
                foreach (Button mode in ModeButtons)
                {
                    mode.GetComponent<Button>().interactable = false;
                }
                foreach(Button option in OptionButtons)
                {
                    option.GetComponent<Button>().interactable = false;
                }
            }
            else if(gm.state != "Calibrating")
            {
                MotionButtons[0].GetComponent<Button>().interactable = true;
                MotionButtons[2].GetComponent<Button>().interactable = true;
                MotionButtons[1].GetComponent<Button>().interactable = false;
                foreach(Button mode in ModeButtons)
                {
                    mode.GetComponent<Button>().interactable = true;
                }
                foreach (Button option in OptionButtons)
                {
                    option.GetComponent<Button>().interactable = true;
                }
            }
        }
        else if (gm.mode == "GoingZeroMode" || gm.mode == "ProgramWave")
        {
            anglepanel.SetActive(false);
            MotionPanel.SetActive(false);
            StartStopPanel.SetActive(true);
            if (gm.state != "Quit" || gm.state != "SwitchingPose" || gm.state != "Options")
            {
                if (gm.mode == "GoingZeroMode") 
                {
                    text.text = "Calibrating to Zero\n" + ((gm.pose == null || gm.pose == "") ? null : "Position:" + gm.pose);
                    foreach (Button mode in ModeButtons)
                    {
                        mode.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                        ModeButtons[4].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                }
                else if(gm.mode == "ProgramWave")
                {
                    text.text = "Wave Cycle\n" + ((gm.pose==null||gm.pose=="")?null:"Position:" + gm.pose);
                    foreach (Button mode in ModeButtons)
                    {
                        mode.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                        ModeButtons[5].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                }
                
            }
            if (gm.state == "Calibrating")
            {
                StartStopButtons[0].GetComponent<Button>().interactable = false;
                StartStopButtons[1].GetComponent<Button>().interactable = true;
                foreach (Button mode in ModeButtons)
                {
                    mode.GetComponent<Button>().interactable = false;
                }
            }
            else if (gm.state != "Calibrating")
            {
                StartStopButtons[0].GetComponent<Button>().interactable = true;
                StartStopButtons[1].GetComponent<Button>().interactable = false;
                foreach (Button mode in ModeButtons)
                {
                    mode.GetComponent<Button>().interactable = true;
                }
            }
        }
        else {
            anglepanel.SetActive(false);
            MotionPanel.SetActive(false);
            StartStopPanel.SetActive(false);
            if (gm.state != "Quit" || gm.state != "SwitchingPose" || gm.state != "Options")
            {
                foreach (Button mode in ModeButtons)
                {
                    mode.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    mode.GetComponent<Button>().interactable = true;
                }
                text.text = "All Information Will Be Displayed Here";
            }
        }
    }
    public void checkstate()
    {
        
        Text text = InfoModeText.GetComponent<Text>();
        if (gm.state == "Options")
        {
            text.text = "Options";
            AllOptions.SetActive(true);
            PoseWavePanel.SetActive(false);
            modepanel.SetActive(false);
            
        }
        else if (gm.state == "SwitchingPose")
        {
            text.text = "Here you can choose needed mode or pose";
            AllOptions.SetActive(false);
            PoseWavePanel.SetActive(true);
            foreach (Button mode in ModeButtons)
            {
                mode.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                mode.GetComponent<Button>().interactable = false;
                ModeButtons[5].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                ModeButtons[5].GetComponent<Button>().interactable = true;
            }
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    beginpoint = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Moved)
                {
                    endpoint = touch.position;
                    touchdirection = (endpoint - beginpoint).normalized;
                    PoseWavePanel.transform.Translate(new Vector3(touchdirection.x, 0, 0) * 10 * Time.deltaTime);

                }
            }

        }
        else if (gm.state == "Quit")
        {
            text.text = "Quitting from app.\n Please wait untill calibration";
            text.color = Color.red;
            AllOptions.SetActive(false);
            PoseWavePanel.SetActive(false);
        }
        else
        {
            modepanel.SetActive(true);
            AllOptions.SetActive(false);
            PoseWavePanel.SetActive(false);
        }
    }
    public void anglemaximum()
    {
        GameObject maxangle = AllOptions.transform.Find("MaxAngle").gameObject;
        GameObject head = maxangle.transform.Find("Head").gameObject;
        GameObject body= maxangle.transform.Find("Body").gameObject;
        GameObject spine = maxangle.transform.Find("Spine").gameObject;
        Slider headslider = head.transform.Find("Slider").GetComponent<Slider>();
        Text headtext = head.transform.Find("Panel").Find("Text").GetComponent<Text>();
        Slider bodyslider = body.transform.Find("Slider").GetComponent<Slider>();
        Text bodytext = body.transform.Find("Panel").Find("Text").GetComponent<Text>();
        Slider spineslider = spine.transform.Find("Slider").GetComponent<Slider>();
        Text spinetext = spine.transform.Find("Panel").Find("Text").GetComponent<Text>();
        

        if (gm.localheadangle<headslider.value) 
        {
            gm.AngularHeadLimit = headslider.value;
        }
        else
        {
            headslider.value = gm.AngularHeadLimit;
        }
        if (gm.localbodyangle < bodyslider.value)
        {
            gm.AngularBodyLimit = bodyslider.value;
        }
        else
        {
            bodyslider.value = gm.AngularBodyLimit;
        }
        if (gm.localspineangle < spineslider.value)
        {
            gm.AngularSpineLimit = spineslider.value;
        }
        else 
        {
            spineslider.value = gm.AngularSpineLimit;
        }
        headtext.text = "Head:" + headslider.value;
        bodytext.text = "Body:" + bodyslider.value;
        spinetext.text = "Spine:" + spineslider.value;

    }

    public IEnumerator Timeticks()
    {
        while (true)
        {
            dt += new TimeSpan(0, 0, 1);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public void settime()
    {
        dt = new DateTime(Convert.ToInt32(timedateinputs[2].text), Convert.ToInt32(timedateinputs[1].text), Convert.ToInt32(timedateinputs[0].text), Convert.ToInt32(timedateinputs[3].text), Convert.ToInt32(timedateinputs[4].text), Convert.ToInt32(timedateinputs[5].text));
        TimeSettedManually = true;
    }
}
