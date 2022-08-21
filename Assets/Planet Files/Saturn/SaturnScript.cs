using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SaturnScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable HideButton;
    public KMSelectable CurrentPosButton;
    public KMSelectable CurrentEndButton;
    public KMSelectable[] PlanetButtons;
    public GameObject Planet;
    public GameObject[] PositionPlanets;
    public GameObject[] PositionPlanetsRotators;
    public MeshRenderer[] InnerSpheres;
    public Material Brown;
    public TextMesh[] CoordsTexts;

    float[] ZValues = new float[] { 0.67f, 0.70722222222f, 0.74444444444f, 0.78166666666f, 0.81888888888f, 0.85611111111f, 0.89333333333f, 0.93055555555f, 0.96777777777f, 1.005f };
    string[] OuterMazeWalls = new string[] { "UD", "UD", "UD", "U", "UD", "UD", "UR", "LU", "UDR", "LU", "UD", "UD", "UD", "UD", "U", "UD", "UR", "LUD", "UD", "UD", "UD", "UD", "UD", "UD", "UR", "ULD", "U", "UR", "LUR", "LU", "UD", "UD", "UR", "LU", "UD", "UD", "UR", "ULD", "U", "UD", "UD", "UR", "LU", "U", "UD", "UR", "LU", "UD", "UD", "UR", "LU", "UD", "UD", "UD", "UD", "UD", "UD", "UR", "LUD", "UD", "U", "UD", "UR", "LU",
                                             "LU", "U", "UD", "DR", "LUR", "LU", "DR", "L", "UD", "RD", "LU", "UR", "LU", "UD", "R", "LU", "DR", "LU", "UD", "UD", "UD", "UR", "LUR", "LU", "RD", "LU", "RD", "LR", "LD", "RD", "LU", "UD", "DR", "LR", "LU", "UR", "LD", "UD", "RD", "LUR", "LU", "RD", "LR", "LD", "UR", "LR", "LD", "UD", "UD", "LR", "LD", "UD", "UR", "LU", "UR", "LUD", "UD", "D", "UR", "LU", "R", "LUD", "RD", "LR",
                                             "LDR", "L", "UD", "UR", "LR", "LD", "U", "RD", "LU", "UD", "RD", "LR", "LDR", "LU", "RD", "LD", "RU", "LR", "LU", "UD", "UR", "L", "D", "DR", "LU", "R", "LUR", "L", "UR", "LU", "RD", "LU", "UD", "R", "LR", "LD", "UD", "UR", "ULD", "R", "LD", "UR", "LDR", "LU", "R", "LR", "LU", "UD", "U", "R", "LUR", "LU", "RD", "LR", "LD", "UD", "UD", "U", "RD", "LR", "LDR", "LU", "UD", "RD",
                                             "UR", "LDR", "LU", "RD", "L", "UR", "LDR", "LU", "RD", "DLU", "UR", "LD", "U", "RD", "DLU", "UR", "LR", "LR", "LD", "UR", "LR", "LD", "UD", "UR", "LR", "LR", "LD", "DR", "LD", "DR", "LUR", "LD", "UR", "LDR", "LR", "LU", "UR", "LD", "UR", "L", "URD", "LD", "UD", "RD", "LR", "LD", "RD", "DLU", "DR", "LD", "DR", "LR", "LUR", "LR", "LUR", "LU", "UD", "RD", "DLU", "D", "UR", "LR", "DLU", "UD",
                                             "LD", "UD", "RD", "DLU", "DR", "LDR", "LDU", "RD", "LDU", "URD", "LDR", "DLU", "D", "UD", "URD", "LDR", "LD", "RD", "DLU", "DR", "LD", "UD", "URD", "LD", "DR", "LD", "URD", "DLU", "UD", "UD", "RD", "ULDR", "LD", "UD", "RD", "LDR", "LD", "UD", "RD", "LD", "UD", "UD", "UD", "UD", "RD", "DLU", "UD", "URD", "DLU", "UD", "UD", "RD", "LDR", "LD", "RD", "LD", "UD", "UD", "UD", "UD", "RD", "LD", "URD", "URDL"
                                            };
    string[] InnerMazeWalls = new string[] { "URD", "LUR", "UL", "URD", "LUR", "LUR", "LUD", "UR", "ULRD", "LUR", "LUD", "UR", "LU", "UD", "UDR", "LUD", "UD", "UD", "RUD", "LU", "UDR", "LUR", "UDL", "UD", "UR", "UDL", "UDR", "UDL", "UR", "UDL", "UD", "UD", "UD", "UD", "UR", "UDL", "UR", "UDL", "UDR", "UDL", "UD", "UD", "UD", "UR", "LUD", "UD", "UDR", "UDL", "UR", "LDU", "UD", "UR", "LUD", "UDR", "UL", "UDR", "LU", "UDR", "LDU", "UR", "LDU", "U", "UDR", "LU",
                                             "LU", "R", "L", "UR", "LD", "RD", "LU", "R", "LU", "R", "LUD", "D", "D", "UD", "UD", "UD", "U", "U", "UD", "DR", "LU", "", "UD", "UD", "D", "UDR", "LU", "U", "D", "UDR", "LU", "U", "U", "U", "D", "UDR", "LD", "UD", "U", "UR", "LU", "UR", "LUD", "D", "UD", "UD", "UD", "UD", "D", "URD", "LU", "R", "LU", "U", "", "UR", "LD", "UD", "UD", "D", "UD", "RD", "LU", "R",
                                             "LD", "D", "D", "DR", "UL", "U", "D", "DR", "L", "", "U", "UR", "LU", "U", "UD", "UD", "D", "DR", "LUD", "UD", "D", "D", "UD", "UD", "U", "UR", "L", "R", "LU", "U", "D", "RD", "L", "R", "LU", "U", "UD", "UD", "D", "DR", "L", "", "UD", "UD", "UD", "UD", "UD", "UD", "UD", "UDR", "L", "", "D", "DR", "L", "", "UD", "UDR", "UL", "U", "UD", "UD", "D", "DR",
                                             "UL", "U", "UD", "UD", "", "R", "LU", "U", "D", "RD", "LD", "RD", "LD", "D", "UD", "UD", "UD", "UD", "U", "UR", "LU", "U", "UD", "UD", "D", "DR", "LD", "DR", "L", "R", "LU", "U", "D", "RD", "L", "R", "LU", "U", "UD", "UD", "D", "D", "U", "UR", "LU", "U", "U", "UR", "LU", "U", "", "R", "LU", "UR", "L", "", "UD", "UD", "", "R", "LU", "U", "U", "UR",
                                             "D", "DR", "UDL", "UD", "D", "RD", "LD", "D", "UD", "UD", "UD", "UD", "UD", "UD", "UD", "UD", "UD", "UD", "D", "RD", "LD", "D", "UD", "UD", "UD", "UD", "UD", "UD", "D", "RD", "LD", "RD", "LUD", "UD", "D", "RD", "LD", "D", "UD", "UD", "UD", "UDR", "LD", "D", "D", "RD", "LD", "D", "D", "RD", "LD", "RD", "LD", "D", "D", "D", "UD", "UDR", "LD", "D", "D", "RD", "LD", "D"
                                            };

    bool visible = true, CurrentOuter = true, EndOuter = true, isAnimating = false;
    int UpIndex, CurrentIndex, EndIndex;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable PlanetButton in PlanetButtons) {
            PlanetButton.OnInteract += delegate () { PlanetButtonPress(PlanetButton); return false; };
        }

        HideButton.OnInteract += delegate () { if (!isAnimating) StartCoroutine(HidePlanet()); return false; };
        CurrentPosButton.OnHighlight += delegate () { if (CurrentOuter) CoordsTexts[0].text = (9 - (CurrentIndex / 64)).ToString(); else CoordsTexts[0].text = (4 - (CurrentIndex / 64)).ToString(); CoordsTexts[1].text = (CurrentIndex % 64).ToString(); };
        CurrentPosButton.OnHighlightEnded += delegate () { CoordsTexts[0].text = string.Empty; CoordsTexts[1].text = string.Empty; };
        CurrentEndButton.OnHighlight += delegate () { if (EndOuter) CoordsTexts[0].text = (9 - (EndIndex / 64)).ToString(); else CoordsTexts[0].text = (4 - (EndIndex / 64)).ToString(); CoordsTexts[1].text = (EndIndex % 64).ToString(); };
        CurrentEndButton.OnHighlightEnded += delegate () { CoordsTexts[0].text = string.Empty; CoordsTexts[1].text = string.Empty; };
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(PlanetRotation());
        UpIndex = UnityEngine.Random.Range(0, 4);
        InnerSpheres[UpIndex].material = Brown;
        CurrentIndex = UnityEngine.Random.Range(0, OuterMazeWalls.Length);
        EndIndex = UnityEngine.Random.Range(0, OuterMazeWalls.Length);
        if (UnityEngine.Random.Range(0, 2) == 0) CurrentOuter = false;
        if (UnityEngine.Random.Range(0, 2) == 0) EndOuter = false;
        while (EndIndex == CurrentIndex && CurrentOuter == EndOuter)
            CurrentIndex = UnityEngine.Random.Range(0, OuterMazeWalls.Length);
        int keyCur = 9;
        int keyEnd = 9;
        if (!CurrentOuter)
            keyCur = 4;
        if (!EndOuter)
            keyEnd = 4;
        PositionPlanets[0].transform.localPosition = new Vector3(0, 0, ZValues[keyCur - (CurrentIndex / 64)]);
        PositionPlanetsRotators[0].transform.localEulerAngles = new Vector3(0, (float)(UpIndex * 90 + (CurrentIndex % 64 * 5.625)), 0);
        PositionPlanets[1].transform.localPosition = new Vector3(0, 0, ZValues[keyEnd - (EndIndex / 64)]);
        PositionPlanetsRotators[1].transform.localEulerAngles = new Vector3(0, (float)(UpIndex * 90 + (EndIndex % 64 * 5.625)), 0);
        Debug.LogFormat("[Saturn #{0}] Your starting position is {1}.", moduleId, "(" + (keyCur - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
        Debug.LogFormat("[Saturn #{0}] The end destination is {1}.", moduleId, "(" + (keyEnd - (EndIndex / 64)) + " up, " + (EndIndex % 64) + " right)");
    }

    private IEnumerator PlanetRotation() {
        var elapsed = 0f;
        while (true) {
            Planet.transform.localEulerAngles = new Vector3(0f, elapsed / 706 * 360, 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator HidePlanet() {
        isAnimating = true;
        if (visible)
            yield return AnimationCoroutine.Animation(1, d => Planet.transform.localScale = Mathf.Lerp(0.14f, 0, d) * Vector3.one);
        else
            yield return AnimationCoroutine.Animation(1, d => Planet.transform.localScale = Mathf.Lerp(0, 0.14f, d) * Vector3.one);
        visible = !visible;
        Debug.LogFormat("<Saturn #{0}> Visibility toggled to {1}.", moduleId, visible);
        isAnimating = false;
    }

    void PlanetButtonPress(KMSelectable PlanetButton) {
        if (!visible || moduleSolved) return;
        if (Array.IndexOf(PlanetButtons, PlanetButton) == 4)
        {
            if (CurrentOuter)
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Saturn #{0}] Attempted to move out a ring but that would bring you out of the maze, strike.", moduleId);
                return;
            }
            CurrentOuter = true;
            PositionPlanets[0].transform.localPosition = new Vector3(0, 0, ZValues[(CurrentOuter ? 9 : 4) - (CurrentIndex / 64)]);
        }
        else if (Array.IndexOf(PlanetButtons, PlanetButton) == 5)
        {
            if (!CurrentOuter)
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Saturn #{0}] Attempted to move in a ring but that would bring you out of the maze, strike.", moduleId);
                return;
            }
            CurrentOuter = false;
            PositionPlanets[0].transform.localPosition = new Vector3(0, 0, ZValues[(CurrentOuter ? 9 : 4) - (CurrentIndex / 64)]);
        }
        else
        {
            int temp = UpIndex;
            int ct = 0;
            while (temp != Array.IndexOf(PlanetButtons, PlanetButton))
            {
                temp = (temp + 1) % 4;
                ct++;
            }
            switch (ct)
            {
                case 0:
                    if (CurrentOuter && OuterMazeWalls[CurrentIndex].Contains("U"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going up at {1}, strike.", moduleId, "(" + (9 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else if (!CurrentOuter && InnerMazeWalls[CurrentIndex].Contains("U"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going up at {1}, strike.", moduleId, "(" + (4 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else
                        CurrentIndex -= 64;
                    PositionPlanets[0].transform.localPosition = new Vector3(0, 0, ZValues[(CurrentOuter ? 9 : 4) - (CurrentIndex / 64)]);
                    Audio.PlaySoundAtTransform("light", transform);
                    break;
                case 1:
                    if (CurrentOuter && OuterMazeWalls[CurrentIndex].Contains("R"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going right at {1}, strike.", moduleId, "(" + (9 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else if (!CurrentOuter && InnerMazeWalls[CurrentIndex].Contains("R"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going right at {1}, strike.", moduleId, "(" + (4 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else if (CurrentIndex % 64 == 63) {
                        CurrentIndex -= 63;
                        Audio.PlaySoundAtTransform("light", transform);
                    }
                    else
                        CurrentIndex += 1;
                    PositionPlanetsRotators[0].transform.localEulerAngles = new Vector3(0, (float)(UpIndex * 90 + (CurrentIndex % 64 * 5.625)), 0);
                    Audio.PlaySoundAtTransform("light", transform);
                    break;
                case 2:
                    if (CurrentOuter && OuterMazeWalls[CurrentIndex].Contains("D"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going down at {1}, strike.", moduleId, "(" + (9 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else if (!CurrentOuter && InnerMazeWalls[CurrentIndex].Contains("D"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going down at {1}, strike.", moduleId, "(" + (4 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else {
                        CurrentIndex += 64;
                    PositionPlanets[0].transform.localPosition = new Vector3(0, 0, ZValues[(CurrentOuter ? 9 : 4) - (CurrentIndex / 64)]);
                    Audio.PlaySoundAtTransform("light", transform);
                    break;
                    }
                default:
                    if (CurrentOuter && OuterMazeWalls[CurrentIndex].Contains("L"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going left at {1}, strike.", moduleId, "(" + (9 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else if (!CurrentOuter && InnerMazeWalls[CurrentIndex].Contains("L"))
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Saturn #{0}] Hit a wall by going left at {1}, strike.", moduleId, "(" + (4 - (CurrentIndex / 64)) + " up, " + (CurrentIndex % 64) + " right)");
                        return;
                    }
                    else if (CurrentIndex % 64 == 0) {
                        CurrentIndex += 63;
                        Audio.PlaySoundAtTransform("light", transform);
                    }
                    else {
                        CurrentIndex -= 1;
                    PositionPlanetsRotators[0].transform.localEulerAngles = new Vector3(0, (float)(UpIndex * 90 + (CurrentIndex % 64 * 5.625)), 0);
                    Audio.PlaySoundAtTransform("light", transform);
                    }
                    break;
            }
        }
        if (CurrentIndex == EndIndex && CurrentOuter == EndOuter)
        {
            moduleSolved = true;
            PositionPlanets[0].SetActive(false);
            GetComponent<KMBombModule>().HandlePass();
            Debug.LogFormat("[Saturn #{0}] End destination reached, module solved.", moduleId);
            Audio.PlaySoundAtTransform("saber", transform);
        }
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} hover <white/green> [Hovers over the sphere with the specified color] | !{0} press <U/D/L/R/F/B> [Presses the specified sphere (U=Up, D=Down, L=Left, R=Right, F=Front, B=Back)] | !{0} toggle [Toggles planet visibility] | Presses can be chained, for ex: !{0} press UURBL";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*toggle\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (isAnimating)
                yield break;
            HideButton.OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*hover\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 2)
                yield return "sendtochaterror Too many parameters!";
            else if (parameters.Length == 2)
            {
                if (parameters[1].EqualsIgnoreCase("green"))
                {
                    yield return null;
                    if (!visible) yield break;
                    CurrentEndButton.OnHighlight();
                    yield return new WaitForSeconds(2f);
                    CurrentEndButton.OnHighlightEnded();
                }
                else if (parameters[1].EqualsIgnoreCase("white"))
                {
                    yield return null;
                    if (!visible) yield break;
                    CurrentPosButton.OnHighlight();
                    yield return new WaitForSeconds(2f);
                    CurrentPosButton.OnHighlightEnded();
                }
                else
                    yield return "sendtochaterror!f The specified color '" + parameters[1] + "' is invalid!";
            }
            else if (parameters.Length == 1)
                yield return "sendtochaterror Please specify a sphere to press!";
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 2)
                yield return "sendtochaterror Too many parameters!";
            else if (parameters.Length == 2)
            {
                string uppedParam = parameters[1].ToUpper();
                char[] valids = { 'D', 'R', 'U', 'L', 'F', 'B' };
                for (int i = 0; i < uppedParam.Length; i++)
                {
                    if (!valids.Contains(uppedParam[i]))
                    {
                        yield return "sendtochaterror!f The specified sphere '" + parameters[1] + "' is invalid!";
                        yield break;
                    }
                }
                yield return null;
                if (!visible) yield break;
                for (int i = 0; i < uppedParam.Length; i++)
                {
                    PlanetButtons[Array.IndexOf(valids, uppedParam[i])].OnInteract();
                    yield return new WaitForSeconds(.1f);
                }
            }
            else if (parameters.Length == 1)
                yield return "sendtochaterror Please specify a sphere to press!";
        }
    }
}