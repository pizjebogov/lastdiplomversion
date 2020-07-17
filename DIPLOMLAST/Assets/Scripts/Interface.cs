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
    public GameObject MedicalBed, InfoOptionsPanel, AllOptions,InfoModeText,DateTimePanel,QuitOptPanel ;
    public Sprite suntheme, moontheme;
    public GameObject block;
    public Sprite blocked, unblocked;
    public bool TimeSettedManually = false;
    public Slider Batteryslide;
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
    }

    // Update is called once per frame
    void Update()
    {
        checktheme();
        checkmaterials();
        checkblocking();
        CheckTime();
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
            AllOptions.transform.Find("Theme").GetComponent<Image>().color = Color.green;
        }
        else if (theme == "NightTheme")
        {
            Camera.main.GetComponent<Skybox>().material = nighttheme;
            glowing = glowingnight;
            AllOptions.transform.Find("Theme").GetComponent<Image>().sprite = moontheme;
            AllOptions.transform.Find("Theme").GetComponent<Image>().color = Color.green;
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
        neededpanel.SetActive(!neededpanel.activeSelf);
    }
    public void CheckTime()
    {
        if (!TimeSettedManually)
        {
            DateTimePanel.transform.Find("TimeText").GetComponent<Text>().text = DateTime.Now.ToString("HH:mm");
            DateTimePanel.transform.Find("DateText").GetComponent<Text>().text = DateTime.Now.ToString("dd:MM:yyyy");
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
}
