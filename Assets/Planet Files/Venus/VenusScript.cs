using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using System.Text;

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
    const string dirs = "ULRD";
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
        do end = UnityEngine.Random.Range(0, 24);
        while (FindPath(start, end).Length < 2);

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
        yield return Ut.Animation(0.75f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(1, 14, d), 1));
        visible = !visible;
        Planet.SetActive(visible);
        for (int i = 0; i < 4; i++) {
            PBObjects[i].SetActive(visible);
        }
        yield return Ut.Animation(0.75f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(14, 1, d), 1));
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
                    current = MoveFrom(current, dirs[i]);
                    Debug.Log("current = " + current);
                    Debug.LogFormat("[Venus #{0}] You went {1} from {2}, now you're at {3}.", moduleId, directionNames[i], b, colors[current]);
                    Audio.PlaySoundAtTransform("match", transform);
                    //StartCoroutine(GeXish(xVals[i], zVals[i]));
                    StartCoroutine(GeXishReborn(dirs[i]));
                } else if (!ButtonIsValid(i)) {
                    GetComponent<KMBombModule>().HandleStrike();
                    Debug.LogFormat("[Venus #{0}] You tried to go {1} from {2} but you cannot go that way, strike!", moduleId, directionNames[i], colors[current]);
                }
            }
        }
    }

    bool ButtonIsValid(int i) {
        return directions[current].Contains(dirs[i]);
    }
    private IEnumerator GeXishReborn(char dir)
    {
        Animating = true;
        Quaternion initRot = Planet.transform.localRotation;
        Quaternion endRot = Quaternion.identity;
        switch (dir)
        {
            case 'U': endRot = Quaternion.AngleAxis( 90, Vector3.right  ) * initRot; break;
            case 'L': endRot = Quaternion.AngleAxis( 90, Vector3.forward) * initRot; break;
            case 'R': endRot = Quaternion.AngleAxis(-90, Vector3.forward) * initRot; break;
            case 'D': endRot = Quaternion.AngleAxis(-90, Vector3.right  ) * initRot; break;
        };
        yield return Ut.Animation(0.25f, d => Planet.transform.localRotation = Quaternion.Slerp(initRot, endRot, d));

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
    string FindPath(int start, int end)
    {
        if (start == end) return "";
        Queue<int> q = new Queue<int>();
        List<Movement> allMoves = new List<Movement>();
        q.Enqueue(start);
        while (q.Count > 0)
        {
            int cur = q.Dequeue();
            for (int d = 0; d < 4; d++)
            {
                if (directions[cur].Contains(dirs[d]))
                {
                    int newPos = MoveFrom(cur, dirs[d]);
                    if (newPos != -1 && !allMoves.Any(x => x.start == newPos))
                    {
                        q.Enqueue(newPos);
                        allMoves.Add(new Movement { start = cur, end = newPos, direction = dirs[d] });
                    }
                }
            }
            if (cur == end)
            {
                Debug.Log("Found end!");
                break;
            }
        }
        Debug.LogFormat("{0} -> {1}", start, end);
        Movement lastMove = allMoves.First(x => x.end == end);
        List<Movement> path = new List<Movement>() { lastMove };
        while (lastMove.start != start)
        {
            lastMove = allMoves.First(x => x.end == lastMove.start);
            path.Add(lastMove);
        }
        path.Reverse();
        Debug.Log(path.Select(x => x.direction).Join(""));
        return path.Select(x => x.direction).Join("");
    }
    int MoveFrom(int start, char d)
    {
        string cell = "";
        string[] seq = (colors[start] + backColor[start]).Select(x => x.ToString()).ToArray();
        switch (d)
        {
            case 'U': cell += seq[2] + seq[1] + seq[4] + seq[3] + seq[5]; break;
            case 'L': cell += seq[0] + seq[2] + seq[3] + seq[5] + seq[4]; break;
            case 'R': cell += seq[0] + seq[5] + seq[1] + seq[2] + seq[4]; break;
            case 'D': cell += seq[5] + seq[1] + seq[0] + seq[3] + seq[2]; break;
            default: throw new Exception("bitch");
        }
        return Array.IndexOf(colors, cell);
    }
    struct Movement
    {
        public int start;
        public int end;
        public char direction;
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} start> to enter DARK VENUS! Use <!{0} move ULRD> to move up, left, right, then down. Use !{0} hide to press the hide button.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpperInvariant();
        Match m = Regex.Match(command, @"^(?:MOVE\s+)?((?:[ULRD]\s*)+)$");
        if (command == "HIDE")
        {
            yield return null;
            HideButton.OnInteract();
        }
        else if (command == "START" && visible)
        {
            yield return null;
            if (Started)
                yield return "sendtochaterror DARK VENUS!!!!!!! is already entered :(";
            else PlanetButtons[0].OnInteract();
        }
        else if (m.Success && Started && visible)
        {
            yield return null;
            foreach (char dir in m.Groups[1].Value.Where(ch => "ULRD".Contains(ch)))
            {
                yield return new WaitUntil(() => !isAnimating);
                yield return new WaitForSeconds(0.2f);
                PlanetButtons[dirs.IndexOf(dir)].OnInteract();
            }
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (!Started)
            yield return Ut.Press(PlanetButtons[0], 0.2f);
        foreach (char ch in FindPath(current, end))
        {
            yield return new WaitUntil(() => !isAnimating);
            yield return new WaitForSeconds(0.2f);
            Debug.Log(ch);
            PlanetButtons[dirs.IndexOf(ch)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
