using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class VenusScript : MonoBehaviour { //depends on name

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable HideButton;
    public GameObject Planet;
    public KMSelectable[] PlanetButtons;
    public GameObject[] PBObjects;
    public GameObject Background;
    public Material Dark;
    public Material Light;
    public GameObject Rotator;
    public GameObject LightBase;
    public Material[] ColorMats;

    bool visible = true;
    bool isAnimating;
    bool Started = false;
    bool AllowedToRotate = true;
    bool Animating = false;

    int start, end, current;
    int[] Xvalues = {1, 2, 2, 2, 2, 1, 0, 1, 2, 2, 3, 0, 3, 2, 2, 2, 3, 2, 2, 2, 1, 2, 3, 2}; //Multiply 90 to get true values
    int[] Yvalues = {2, 1, 0, 0, 0, 0, 2, 2, 2, 1, 0, 1, 2, 3, 2, 1, 0, 3, 2, 3, 2, 1, 0, 2};
    int[] Zvalues = {3, 0, 0, 3, 2, 0, 3, 1, 1, 1, 2, 1, 3, 2, 0, 2, 3, 0, 3, 1, 0, 3, 0, 2};
    string[] directions = {"U ", "L ", "L ", "LR", "UR", "L ", 
                           "UD", "LDR", "UR", "DR", "LDR", "ULR", 
                           "UDR", "DLR", "UL", "URD", "LR", "LD", 
                           "D ", "UR", "LDR", "LUR", "LU", "D "};
    string[] colors = {"RGYMC", "CBMYR", "YCMRB", "YMRGB", "YRGCB", "GCYRM", 
                       "YGCMB", "CMYGR", "BMCGY", "GBCYM", "GRBCM", "GYRBM", 
                       "CGBMR", "CYGBR", "BRMCY", "RBGYC", "RMBGC", "RYMBC", 
                       "BGRMY", "MYCBG", "MRYCG", "MBRYG", "MCBRG", "BCGRY"};
    string backColor = "BGGCMBRBRRYCYMGMYGCRBCYM";
    string[] directionNames = {"up", "left", "right", "down"};

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable PlanetButton in PlanetButtons) {
            PlanetButton.OnInteract += delegate () { PlanetButtonPress(PlanetButton); return false; };
        }

        HideButton.OnInteract += delegate () { StartCoroutine(HidePlanet()); return false; };

    }

    // Use this for initialization
    void Start () {
        StartCoroutine(PlanetRotation());
    }

    void GeneratePuzzle () {

        int[] dummy = { 0, 1, -1, 2, 3 };

        start = UnityEngine.Random.Range(0, 24);
        end = UnityEngine.Random.Range(0, 24);
        while (start == end) {
            end = UnityEngine.Random.Range(0, 24);
        }
        Debug.Log("end = " + end);
        current = start;

        Debug.LogFormat("[Venus #{0}] Starting at {1}, Ending at {2}.", moduleId, colors[start], colors[end]);

        Planet.transform.localEulerAngles = new Vector3(90f * Xvalues[start], 90f * Yvalues[start], 90f * Zvalues[start]);

        for (int i = 0; i < 5; i++) {
            int n = dummy[i];
            if (i != 2) {
                switch (colors[end][i]) {
                    case 'R': PBObjects[n].GetComponent<MeshRenderer>().material = ColorMats[0]; break;
                    case 'G': PBObjects[n].GetComponent<MeshRenderer>().material = ColorMats[1]; break;
                    case 'B': PBObjects[n].GetComponent<MeshRenderer>().material = ColorMats[2]; break;
                    case 'C': PBObjects[n].GetComponent<MeshRenderer>().material = ColorMats[3]; break;
                    case 'M': PBObjects[n].GetComponent<MeshRenderer>().material = ColorMats[4]; break;
                    case 'Y': PBObjects[n].GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                }
            } else {
                switch (colors[end][2]) {
                    case 'R': LightBase.GetComponent<MeshRenderer>().material = ColorMats[0]; break;
                    case 'G': LightBase.GetComponent<MeshRenderer>().material = ColorMats[1]; break;
                    case 'B': LightBase.GetComponent<MeshRenderer>().material = ColorMats[2]; break;
                    case 'C': LightBase.GetComponent<MeshRenderer>().material = ColorMats[3]; break;
                    case 'M': LightBase.GetComponent<MeshRenderer>().material = ColorMats[4]; break;
                    case 'Y': LightBase.GetComponent<MeshRenderer>().material = ColorMats[5]; break;
                }
            }
        }
    }

    private IEnumerator PlanetRotation() {
        var elapsed = 0f;
        while (AllowedToRotate) {
            Planet.transform.localEulerAngles = new Vector3(elapsed / -14 * 360, 90f, 90f); //depends on time it takes to go around in 1 day
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator HidePlanet() {
        if (isAnimating) yield break;
        isAnimating = true;
        yield return Ut.Animation(0.7f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(1, 14, d), 1));
        visible = !visible;
        Planet.SetActive(visible);
        for (int i = 0; i < 4; i++) {
            PBObjects[i].SetActive(visible);
        }
        yield return Ut.Animation(0.7f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(14, 1, d), 1));
        Debug.LogFormat("<Venus #{0}> Visible toggled to {1}.", moduleId, visible);
        isAnimating = false;
    }

    void PlanetButtonPress(KMSelectable PlanetButton) {
        if (moduleSolved) {
            return;
        }
        for (int i = 0; i < 4; i++) {
            if (PlanetButton == PlanetButtons[i]) {
                if (!Started) {
                    AllowedToRotate = false;
                    Planet.GetComponent<MeshRenderer>().material = Dark;
                    Started = true;
                    GeneratePuzzle();
                } else if (!Animating && ButtonIsValid(i)) {
                    string b = colors[current];
                    int[] xVals = new int[] { 1, 0, 0, -1 };
                    int[] zVals = new int[] { 0, 1, -1, 0 };
                    string a = "";
                    switch (i) {
                        case 0: a = colors[current][2] + " " + colors[current][4]; break;
                        case 1: a = colors[current][0] + " " + colors[current][3]; break;
                        case 2: a = colors[current][0] + " " + colors[current][1]; break;
                        case 3: a = backColor[current] + " " + colors[current][0]; break;
                    }
                    for (int j = 0; j < 24; j++) {
                        if (a[0] == colors[j][0] && a[2] == colors[j][2]) {
                            current = j;
                            Debug.Log("current = " + current);
                        }
                    }
                    Debug.LogFormat("[Venus #{0}] You went {1} from {2}, now you're at {3}.", moduleId, directionNames[i], b, colors[current]);
                    Audio.PlaySoundAtTransform("match", transform);
                    StartCoroutine(GeXish(xVals[i], zVals[i]));
                } else if (!ButtonIsValid(i)) {
                    GetComponent<KMBombModule>().HandleStrike();
                    Debug.LogFormat("[Venus #{0}] You tried to go {1} from {2} but you cannot go that way, strike!", moduleId, directionNames[i], colors[current]);
                }
            }
        }
    }

    bool ButtonIsValid(int i) {
        return directions[current].Contains("ULRD"[i]);
    }

    private IEnumerator GeXish(int xVal, int zVal)
    {
        Animating = true;
        for (int i = 0; i < 30; i++)
        {
            Planet.transform.RotateAround(Rotator.transform.position, Rotator.transform.right, 3f * xVal);
            Planet.transform.RotateAround(Rotator.transform.position, Rotator.transform.forward, 3f * zVal);
            yield return new WaitForSeconds(0.005f);
        }
        Animating = false;
        CheckIfItsSolved();
    }

    void CheckIfItsSolved() {
        if (current == end) {
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
            Debug.LogFormat("[Venus #{0}] Made it to the destination! Module solved.", moduleId);
            Audio.PlaySoundAtTransform("burn", transform);
            Planet.GetComponent<MeshRenderer>().material = Light;
            AllowedToRotate = true;
            StartCoroutine(PlanetRotation());
        }
    }

}
