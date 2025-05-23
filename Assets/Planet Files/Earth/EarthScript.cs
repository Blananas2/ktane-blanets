using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class EarthScript : MonoBehaviour { //depends on name

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable HideButton;
    public GameObject Planet;
    public KMSelectable[] PlanetButtons;
    public GameObject[] ActualButtons;
    public GameObject Background;
    public Material[] Colors; //black, orange, blue

    bool visible = true;
    bool isAnimating;
    private Coroutine buttonHold;
	private bool holding = false;
    bool wantRotation = true;
    float elapsed = 0f;

    int[][] tris = new int[][] {
        new int[] {2, 8, 47},
        new int[] {2, 25, 47},
        new int[] {25, 45, 47},
        new int[] {38, 45, 47},
        new int[] {38, 45, 46},
        new int[] {6, 45, 46},
        new int[] {6, 46, 47},
        new int[] {6, 8, 47},
        new int[] {6, 8, 20},
        new int[] {8, 20, 44},
        new int[] {20, 36, 44},
        new int[] {36, 44, 47},
        new int[] {35, 36, 47},
        new int[] {18, 35, 47},
        new int[] {18, 46, 47},
        new int[] {18, 38, 46},
        new int[] {18, 22, 38},
        new int[] {22, 29, 38},
        new int[] {29, 38, 44},
        new int[] {38, 44, 47},
        new int[] {7, 29, 44},
        new int[] {2, 7, 8},
        new int[] {2, 7, 43},
        new int[] {2, 25, 43},
        new int[] {16, 25, 43},
        new int[] {16, 25, 45},
        new int[] {15, 16, 45},
        new int[] {15, 24, 45},
        new int[] {6, 24, 45},
        new int[] {6, 24, 39},
        new int[] {6, 20, 39},
        new int[] {0, 20, 39},
        new int[] {0, 11, 20},
        new int[] {11, 20, 36},
        new int[] {11, 31, 36},
        new int[] {3, 31, 36},
        new int[] {3, 35, 36},
        new int[] {3, 35, 37},
        new int[] {32, 35, 37},
        new int[] {18, 32, 35},
        new int[] {4, 18, 32},
        new int[] {4, 18, 22},
        new int[] {4, 22, 42},
        new int[] {22, 29, 42},
        new int[] {21, 29, 42},
        new int[] {21, 29, 40},
        new int[] {7, 29, 40},
        new int[] {7, 40, 43},
        new int[] {33, 40, 43},
        new int[] {26, 33, 43},
        new int[] {23, 26, 43},
        new int[] {16, 23, 43},
        new int[] {16, 19, 23},
        new int[] {15, 16, 19},
        new int[] {15, 19, 24},
        new int[] {19, 24, 27},
        new int[] {24, 27, 39},
        new int[] {0, 27, 39},
        new int[] {0, 27, 30},
        new int[] {0, 1, 30},
        new int[] {0, 1, 11},
        new int[] {1, 11, 17},
        new int[] {11, 17, 31},
        new int[] {3, 17, 31},
        new int[] {3, 17, 37},
        new int[] {1, 17, 37},
        new int[] {1, 9, 37},
        new int[] {1, 9, 28},
        new int[] {1, 28, 30},
        new int[] {13, 28, 30},
        new int[] {13, 27, 30},
        new int[] {13, 14, 27},
        new int[] {14, 19, 27},
        new int[] {10, 14, 19},
        new int[] {10, 14, 28},
        new int[] {13, 14, 28},
        new int[] {9, 10, 28},
        new int[] {9, 32, 37},
        new int[] {9, 12, 32},
        new int[] {4, 12, 32},
        new int[] {4, 12, 34},
        new int[] {5, 12, 34},
        new int[] {5, 12, 41},
        new int[] {9, 12, 41},
        new int[] {9, 10, 41},
        new int[] {10, 19, 41},
        new int[] {19, 23, 41},
        new int[] {5, 23, 41},
        new int[] {5, 23, 26},
        new int[] {5, 26, 34},
        new int[] {4, 26, 34},
        new int[] {4, 26, 33},
        new int[] {4, 33, 42},
        new int[] {21, 33, 42},
        new int[] {21, 33, 40},
        new int[] {7, 8, 44},
    };
    private List<string> placeNames = new List<string> {"China", "India", "United States", "Indonesia", "Brazil", "Nigeria", "Russia", "Mexico", "Japan", "Uganda", "Egypt", "Vietnam", "DR Congo", "Iran", "Turkey", "Germany", "United Kingdom", "Thailand", "South Africa", "Italy", "South Korea", "Colombia", "Argentina", "Algeria", "Ukraine", "Canada", "Morocco", "Uzbekistan", "Saudi Arabia", "Peru", "Afghanistan", "Malaysia", "Mozambique", "Venezuela", "Ivory Coast", "Madagascar", "Australia", "Sri Lanka", "Chile", "Kazakstan", "Guatemala", "Chad", "Bolivia", "Cuba", "Papua New Guinea", "Greenland", "Antarctica", "New Zealand"};
    private string[] abbrs = new[] { "CN", "IN", "US", "ID", "BR", "NG", "RU", "MX", "JP", "UG", "EG", "VN", "CD", "IR", "TR", "DE", "GB", "TH", "ZA", "IT", "KR", "CO", "AR", "DZ", "UA", "CA", "MA", "UZ", "SA", "PE", "AF", "MY", "MZ", "VE", "CI", "MG", "AU", "LK", "CL", "KZ", "GT", "TD", "BO", "CU", "PG", "GL", "AQ", "NZ" };
    private List<int> primes = new List<int> {2,3,5,7,11,13,17,19,23,29,31,37,41,43,47,53,59,61,67,71,73,79,83,89,97,101,103,107,109,113,127,131,137,139,149,151,157,163,167,173,179,181,191,193,197,199,211,223,227,229,233,239,241,251,257,263,269,271};

    int startingTri = -1;
    int endingTri = -1;
    int currentTri = -1;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable PlanetButton in PlanetButtons) {
            PlanetButton.OnInteract += delegate () { PlanetButtonPress(PlanetButton); return false; };
        }

        HideButton.OnInteract += delegate () { HidePress(); return false; }; //StartCoroutine(HidePlanet());
        HideButton.OnInteractEnded += delegate { HideRelease(); };

    }

    // Use this for initialization
    void Start () {
        StartCoroutine(PlanetRotation());
        startingTri = UnityEngine.Random.Range(0, tris.Length);
        currentTri = startingTri;


        do endingTri = UnityEngine.Random.Range(0, tris.Length);
        while (FindPath(startingTri, endingTri).Count < 3);

        Debug.LogFormat("[Earth #{0}] Starting tri is {1}, {2}, {3}", moduleId, placeNames[tris[startingTri][0]], placeNames[tris[startingTri][1]], placeNames[tris[startingTri][2]]);
        Debug.LogFormat("[Earth #{0}] Ending tri is {1}, {2}, {3}", moduleId, placeNames[tris[endingTri][0]], placeNames[tris[endingTri][1]], placeNames[tris[endingTri][2]]);

        ColorSpheres();
    }

    private IEnumerator PlanetRotation() {
        while (wantRotation) {
            Planet.transform.localEulerAngles = new Vector3(elapsed / 24 * 360, 90f, 90f); //depends on time it takes to go around in 1 day
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator HidePress() {
        if (buttonHold != null)
		{
			holding = false;
			StopCoroutine(buttonHold);
			buttonHold = null;
		}

		buttonHold = StartCoroutine(HoldChecker());
        return null;
    }

    IEnumerator HoldChecker()
	{
		yield return new WaitForSeconds(.4f);
        holding = true;
    }

    private IEnumerator HideRelease() {
        if (holding) {
            wantRotation = !wantRotation;
            StartCoroutine(PlanetRotation());
            Debug.LogFormat("<Earth #{0}> Rotation toggled to {1}.", moduleId, wantRotation);
        } else {
            StartCoroutine(HidePlanet());
        }
        return null;
    }

    private IEnumerator HidePlanet() {
        if (isAnimating) yield break;
        isAnimating = true;
        yield return Ut.Animation(0.7f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(1, 15, d), 1));
        visible = !visible;
        Planet.SetActive(visible);
        yield return Ut.Animation(0.7f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(15, 1, d), 1));
        Debug.LogFormat("<Earth #{0}> Visible toggled to {1}.", moduleId, visible);
        isAnimating = false;
    }

    void PlanetButtonPress(KMSelectable PlanetButton) {
        for (int i = 0; i < 48; i++) {
            if (PlanetButtons[i] == PlanetButton) {
                if (ChecksOut(i) != -1) {
                    currentTri = ChecksOut(i);
                    Debug.LogFormat("[Earth #{0}] Pressed {1}: Location is {2}, {3}, {4}", moduleId, placeNames[i], placeNames[tris[currentTri][0]], placeNames[tris[currentTri][1]], placeNames[tris[currentTri][2]]);
                    Audio.PlaySoundAtTransform("single", transform);
                    ColorSpheres();
                    if (currentTri == endingTri) {
                        GetComponent<KMBombModule>().HandlePass();
                        Debug.LogFormat("[Earth #{0}] Made it to the ending tri. Module solved.", moduleId);
                        Audio.PlaySoundAtTransform("multiple", transform);
                    }
                } else {
                    Debug.LogFormat("[Earth #{0}] No tri has two of the previous tri's points and {1}. Strike!", moduleId, placeNames[i]);
                    GetComponent<KMBombModule>().HandleStrike();
                }
                Debug.LogFormat("<Earth #{0}> {1}, {2}, {3} // {4}", moduleId, tris[currentTri][0], tris[currentTri][1], tris[currentTri][2], i);
            }
        }
    }

    void ColorSpheres()
    {
        for (int j = 0; j < 48; j ++) {
            ActualButtons[j].GetComponent<MeshRenderer>().material = Colors[0];
        }
        for (int k = 0; k < 6; k++) {
            if (k < 3) {
                ActualButtons[tris[endingTri][k]].GetComponent<MeshRenderer>().material = Colors[2];
                ActualButtons[tris[currentTri][k]].GetComponent<MeshRenderer>().material = Colors[1];
            } else {
                if (tris[endingTri].Contains(tris[currentTri][k-3])) {
                    ActualButtons[tris[currentTri][k-3]].GetComponent<MeshRenderer>().material = Colors[3];
                }
            }
        }
    }

    int ChecksOut (int n) {
        int match = -1;
        int A = primes[tris[currentTri][0]]*primes[tris[currentTri][1]]*primes[n];
        int B = primes[tris[currentTri][0]]*primes[tris[currentTri][2]]*primes[n];
        int C = primes[tris[currentTri][1]]*primes[tris[currentTri][2]]*primes[n];

        for (int q = 0; q < tris.Length; q++) {
            int X = primes[tris[q][0]]*primes[tris[q][1]]*primes[tris[q][2]];
            if ((X == A) || (X == B) || (X == C)) {
                match = q;
            }
        }

        return match;
    }

    List<TriTuple> GetAdjacents(int triIx)
    {
        List<TriTuple> rtn = new List<TriTuple>(3);
        int[] thisTri = tris[triIx];
        for (int i = 0; i < tris.Length; i++)
        {
            int[] thatTri = tris[i];
            if (i != triIx && (
                (thatTri.Contains(thisTri[0]) && thatTri.Contains(thisTri[1])) ||
                (thatTri.Contains(thisTri[1]) && thatTri.Contains(thisTri[2])) ||
                (thatTri.Contains(thisTri[0]) && thatTri.Contains(thisTri[2]))))
            {
                rtn.Add(new TriTuple { triIx = i, move = thatTri.Single(x => !thisTri.Contains(x))});
            }
        }
        return rtn;
    }
    List<int> FindPath(int start, int end)
    {
        if (start == end)
            return new List<int>();
        Queue<int> q = new Queue<int>();
        List<Movement> allMoves = new List<Movement>();
        HashSet<int> visitedTris = new HashSet<int>();

        q.Enqueue(start);
        while (q.Count > 0)
        {
            int cur = q.Dequeue();
            foreach (TriTuple adj in GetAdjacents(cur))
            {
                if (visitedTris.Add(adj.triIx))
                {
                    q.Enqueue(adj.triIx);
                    allMoves.Add(new Movement { start = cur, end = adj.triIx, pressed = adj.move });
                }
            }
            if (cur == end)
            {
                Debug.Log("<Earth Autosolver> Found end!");
                break;
            }
        }
        Debug.LogFormat("<Earth Autosolver> {0} -> {1}", start, end);
        Movement lastMove = allMoves.First(x => x.end == end);
        List<Movement> path = new List<Movement>() { lastMove };
        while (lastMove.start != start)
        {
            lastMove = allMoves.First(x => x.end == lastMove.start);
            path.Add(lastMove);
        }
        path.Reverse();
        Debug.Log("<Earth Autosolver> " + path.Select(x => x.pressed).Join());
        return path.Select(x => x.pressed).ToList();
    }
    class TriTuple
    {
        public int triIx;
        public int move;
    }
    class Movement
    {
        public int start;
        public int end;
        public int pressed;
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} US MX CA> to press the spheres corresponding to the US, Mexico, and Canada. Press 'Toggle Codes' in the manual to see their country codes. Use <!{0} hide> to hide the planet.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpperInvariant();
        Match m = Regex.Match(command, @"^(?:(?:MOVE|PRESS)\s+)?((?:[A-Z][A-Z](?:\s+|$))+)$");
        if (command == "HIDE")
        {
            yield return null;
            HideButton.OnInteract();
             HideButton.OnInteractEnded();
        }
        else if (m.Success && visible)
        {
            string[] parameters = m.Groups[1].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pm in parameters)
            {
                if (!abbrs.Contains(pm))
                {
                    yield return "sendtochaterror Unrecognized country code: " + pm;
                    yield break;
                }
            }
            yield return null;
            foreach (string abbr in parameters)
            {
                yield return Ut.Press(PlanetButtons[Array.IndexOf(abbrs, abbr)], 0.3f);
            }
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (!visible)
        {
            HideButton.OnInteract();
            HideButton.OnInteractEnded();
            while (isAnimating)
                yield return true;
        }
        foreach (int sphere in FindPath(currentTri, endingTri))
            yield return Ut.Press(PlanetButtons[sphere], 0.3f);
    }

}
