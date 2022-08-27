using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class NeptuneScript : MonoBehaviour { //depends on name

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable HideButton;
    public GameObject Planet;
    public GameObject StatStuff;
    public KMSelectable[] PlanetButtons;
    public GameObject Background;
    public SpriteRenderer[] Sprites;
    public Sprite[] Characters;
    public Sprite Empty;
    public GameObject[] Coins;
    public GameObject Star;

    bool visible = true;
    bool isAnimating;
    bool oddSN = false;
    int starSpeed = 0;
    int RNG = 0;
    int chosenStar = 0;
    int usedStar = 0;
    int coinCount = 0;
    List<Value> bannedRNGvalues = new List<Value>();
    string b36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    Value v = new Value(0, 0, 0, 0, 0);
    int stage = 0;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool greenLight;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable PlanetButton in PlanetButtons) {
            PlanetButton.OnInteract += delegate () { PlanetButtonPress(PlanetButton); return false; };
        }

        HideButton.OnInteract += delegate () { StartCoroutine(HidePlanet()); return false; };
    }

    // Use this for initialization
    void Start () {
        oddSN = Bomb.GetSerialNumberNumbers().Last() % 2 == 1;
        starSpeed = UnityEngine.Random.Range(100, 201);
        Debug.LogFormat("<Neptune #{0}> Star Speed: {1}", moduleId, starSpeed);
        StartCoroutine(Rotation());
        GeneratePuzzle();
        ChangeRNGSprites();
    }

    void GeneratePuzzle() {
        RNG = UnityEngine.Random.Range(0, 100000);
        Debug.LogFormat("[Neptune #{0}] RNG starts at {1}.", moduleId, RNG);
        RNGToLetters();
        bannedRNGvalues.Add(v);

        chosenStar = UnityEngine.Random.Range(0, 120);
        Debug.LogFormat("[Neptune #{0}] Star is {1} / {2}.", moduleId, NeptuneData.LoggedAbbrs[chosenStar], NeptuneData.LoggedStars[chosenStar]);

        if (!NeptuneData.HasEnemies[chosenStar]) {
            Debug.LogFormat("[Neptune #{0}] That star in the table has Ø or N/A.", moduleId);
            int starIx = Array.IndexOf(NeptuneData.NoEnemies, chosenStar);
            Debug.LogFormat("[Neptune #{0}] The serial number is {1}.", moduleId, (oddSN ? "odd" : "even"));
            if (oddSN) {
                usedStar = NeptuneData.OddArrow[starIx];
            } else {
                usedStar = NeptuneData.EvenArrow[starIx];
            }
            Debug.LogFormat("[Neptune #{0}] New star is {1} / {2}.", moduleId, NeptuneData.LoggedAbbrs[usedStar], NeptuneData.LoggedStars[usedStar]);
        } else {
            usedStar = chosenStar;
        }

        coinCount = NeptuneData.Enemies[usedStar].Count();
        Debug.LogFormat("[Neptune #{0}] The number of coins shown is {1}.", moduleId, coinCount);

        for (int n = 0; n < 6; n++) {
            if (n >= coinCount) {
                Coins[n].SetActive(false);
            }
        }

        for (int s = 0; s < 9; s++) {
            if (NeptuneData.Stars[chosenStar][s].ToString() == " ") {
                Sprites[s+5].sprite = Empty;
            } else {
                Sprites[s+5].sprite = Characters[b36.IndexOf(NeptuneData.Stars[chosenStar][s])];
            }
        }

        Debug.LogFormat("[Neptune #{0}] Sequence of enemies: {1}", moduleId, NeptuneData.Enemies[usedStar].Join(", "));
    }

    void ChangeRNGSprites () {
        string RNGstring = RNG.ToString("00000");
        for (int r = 0; r < 5; r++) {
            Sprites[r].sprite = Characters[b36.IndexOf(RNGstring[r])];
        }
    }

    void RNGToLetters () {
        int R = RNG;
        v.E = R % 10;
        R /= 10; v.D = R % 10;
        R /= 10; v.C = R % 10;
        R /= 10; v.B = R % 10;
        R /= 10; v.A = R % 10;
    }

    int LettersToRNG (Value v) {
        return v.A*10000 + v.B*1000 + v.C*100 + v.C*10 + v.E;
    }

    private IEnumerator Rotation() {
        var elapsed = 0f;
        while (true) {
            Planet.transform.localEulerAngles = new Vector3(elapsed / 3954 * 360, 90f, 90f); //depends on time it takes to go around in 1 day
            Star.transform.localEulerAngles = new Vector3(elapsed * starSpeed, 90f, 90f);
            for (int c = 0; c < 6; c++) {
                Coins[c].transform.localEulerAngles = new Vector3(elapsed * 144, 90f, 90f);
            }
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator HidePlanet() {
        if (isAnimating) yield break;
        isAnimating = true;
        yield return Ut.Animation(0.75f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(1, 16, d), 1));
        visible = !visible;
        if (!moduleSolved) {
            Planet.SetActive(visible);
            StatStuff.SetActive(visible);
        } else {
            Star.SetActive(visible);
        }
        yield return Ut.Animation(0.75f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(16, 1, d), 1));
        Debug.LogFormat("<Neptune #{0}> Visible toggled to {1}.", moduleId, visible);
        isAnimating = false;
    }

    void PlanetButtonPress(KMSelectable PlanetButton) {
        if (moduleSolved) {
            return;
        }
        for (int i = 0; i < 5; i++) {
            if (PlanetButtons[i] == PlanetButton) {
                if (i == 0) {
                    if (CheckEnemy(NeptuneData.Enemies[usedStar][stage], v, RNG)) {
                        Debug.LogFormat("[Neptune #{0}] {1} manipulated with {2}.", moduleId, NeptuneData.Enemies[usedStar][stage], RNG.ToString("00000"));
                        stage += 1;
                        Coins[coinCount - stage].SetActive(false);
                        bool f = false;
                        bannedRNGvalues.Add(v);
                        if (coinCount == stage) {
                            moduleSolved = true;
                            Audio.PlaySoundAtTransform("star_collect", transform);
                            Planet.SetActive(false);
                            StatStuff.SetActive(false);
                            StopAllCoroutines();
                            StartCoroutine(AnimateStar());
                        } else {
                            PlanetButton.AddInteractionPunch();
                            Audio.PlaySoundAtTransform("coin_collect", transform);
                        }
                    } else {
                        Debug.LogFormat("[Neptune #{0}] {1} cannot be manipulated with {2}, Strike!", moduleId, NeptuneData.Enemies[usedStar][stage], RNG.ToString("00000"));
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                } else {
                    Audio.PlaySoundAtTransform("so_" + ".BGRO"[i], transform);
                    switch (i) {
                        case 1:
                            v.A = 9-v.A; v.B = 9-v.B; v.C = 9-v.C; v.D = 9-v.D; v.E = 9-v.E;
                        break;
                        case 2:
                            v.B = (v.B+v.A)%10; v.C = (v.C+v.A)%10; v.D = (v.D+v.A)%10; v.E = (v.E+v.A)%10;
                        break;
                        case 3:
                            v.B = (v.B*v.A)%10; v.C = (v.C*v.A)%10; v.D = (v.D*v.A)%10; v.E = (v.E*v.A)%10;
                        break;
                        case 4:
                            int t = v.A; v.A = v.B; v.B = v.C; v.C = v.D; v.D = v.E; v.E = t;
                        break;
                    }
                    RNG = LettersToRNG(v);
                    ChangeRNGSprites();
                }
            }
        }
    }

    bool CheckEnemy(string enemy, Value v, int RNG = -1) {
        if (bannedRNGvalues.Contains(v))
            return false;
        if (RNG == -1)
            RNG = LettersToRNG(v);
        switch (enemy) {
            case "B": return ((32768 < RNG) && (RNG < 65536));
            case "F": return (v.A+v.B+v.C+v.D+v.E <= 20);
            case "G": return (v.A+v.B+v.C+v.D+v.E >= 24);
            case "S": return (v.A < v.B && v.B < v.C && v.C < v.D && v.D < v.E);
            case "Sd": return (v.A > v.B && v.B > v.C && v.C > v.D && v.D > v.E);
            case "Cy": return (v.A+v.B == v.D+v.E);
            case "Lk": return (v.A+v.B+v.C+v.D == v.E);
            case "PP": 
                if (v.D == 0) {return false;} 
                return ((v.A*100+v.B*10+v.C) % v.D == v.E);
            case "Bo": return ((v.A*10+v.D) == (v.B*10+v.E));
            case "Bo+": return ((v.A*100+v.B*10+v.C) == (v.C*100+v.D*10+v.E));
            case "HH": return (v.A*v.B*v.C == (v.D*10+v.E));
            case "Bk": return ((v.A*10+v.B) - (v.C*10+v.D) == v.E);
            case "G-": return (v.A+v.B+v.C+v.D+v.E >= 12);
            case "Cl": 
                if ((v.C*10+v.D) == 0) {return false;} 
                return ((double)(v.A*10+v.B) / (v.C*10+v.D) == (double)v.E);
            case "FS": return ((v.B*100+v.E*10+v.D) == 222);
            case "Wh": return (v.A*v.B > v.D*v.E);
            case "MI": return ((v.C == 0) && (v.A*v.B*v.D*v.E != 0));
            case "Sf": 
                if (v.C == 0) {return false;} 
                return (RNG % v.C == 6);
            case "Sw": return ((v.A*10+v.B) == (v.C*10+v.D)*v.E);
            case "Bw": return (v.A*v.B*v.C*v.D*v.E == 64);
            case "MP": return (v.A+v.B-v.C+v.D-v.E < 0);
            case "Pk": return ((v.B == v.A+1) && (v.C == v.A+2));
            case "Bl": return ((v.B*100+v.C*10+v.D) < 256);
            case "Bl+": return ((v.B*100+v.C*10+v.D) > 728);
            case "Dr": return (v.A == (v.B*10+v.C) - (v.D*10+v.E));
            case "Kp+": return (16384 > RNG);
            case "MB": return ((v.A*10+v.B) + (v.C*10+v.D) == 100);
            case "Pn": return (v.A*v.B*v.C*v.D*v.E != 0);
            case "Wh+": return (v.A*v.B < v.D*v.E);
            case "B+": return (32768 > RNG);
            case "Bl*": return ((256 < (v.B*100+v.C*10+v.D)) && ((v.B*100+v.C*10+v.D) < 728));
            case "CC": return ((v.E*100+v.C*10+v.A) % 111 == 0);
            case "Ee": 
                if ((v.C*10+v.D) == 0) {return false;} 
                return ((v.A*10+v.B) % (v.C*10+v.D) == v.E);
            case "Ey": return (v.A < v.B && v.B < v.C && v.C > v.D && v.D > v.E);
            case "Fw": return (v.A > v.B && v.B > v.C && v.C < v.D && v.D < v.E);
            case "G+": return (v.A+v.B+v.C+v.D+v.E >= 36);
            case "Gr": return (RNG % 7 == 3);
            case "Kl": return (v.A+v.B+v.C == (v.D*10+v.E));
            case "Kp": return ((v.B*10+v.C) == 0);
            case "MM": return (v.A-v.B+v.C-v.D+v.E < 0);
            case "Mn": return (v.A*v.B*v.C*v.D*v.E > 255);
            case "MR": return (v.A*v.E == v.B*v.D);
            case "Pg-": return (1996 > RNG);
            case "Pg+": return (98004 < RNG);
            case "Sk": 
                if ((v.C*10+v.B) == 0) {return false;} 
                return ((double)(v.E*10+v.D) / (v.C*10+v.B) == (double)v.A);
            case "Sn": 
                if ((v.D*10+v.B) == 0) {return false;} 
                return ((v.E*100+v.C*10+v.A) % (v.D*10+v.B) == 0);
            case "TB": return (65536 < RNG);
            case "Th": return ((v.C == v.E+2) && (v.D == v.E+1));
            case "Uk": return (v.B == v.D && v.C == v.E && v.D != v.E);
            case "Wg": return ((v.A*10+v.B) == (v.E*10+v.D));
        }
        return false;
    }

    IEnumerator AnimateStar () {
        Star.transform.localPosition = new Vector3(0f, 0.05f, -0.04f);
        Star.transform.localScale = new Vector3(0.02f, 0.02f, -0.02f);
        yield return Ut.Animation(4.128f, d => Star.transform.localEulerAngles = new Vector3(d * 4.128f * 394, 90, 90));
        Star.transform.localEulerAngles = new Vector3(180f, 90f, 90f);
        GetComponent<KMBombModule>().HandlePass();
        greenLight = true;
        Debug.LogFormat("[Neptune #{0}] All enemies manipulated, module solved.", moduleId);
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} URDLS> to press the up, right, down, left, then submit (center) buttons. Use <!{0} move ULRD> to move up, left, right, then down. Use !{0} hide to press the hide button.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpperInvariant();
        Match m = Regex.Match(command, @"^(?:PRESS\s+)?((?:[SULRD]\s*)+)$");
        const string dirs = "SULRD";
        if (command == "HIDE")
        {
            yield return null;
            HideButton.OnInteract();
        }
        else if (m.Success && visible)
        {
            yield return null;
            foreach (char ch in m.Groups[1].Value.Where(ch => dirs.Contains(ch)))
                yield return Ut.Press(PlanetButtons[dirs.IndexOf(ch)], 0.25f);
            if (moduleSolved)
                yield return "solve";
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (!visible)
        {
            HideButton.OnInteract();
            while (isAnimating)
                yield return true;
        }
        while (!moduleSolved)
        {
          string enemy = NeptuneData.Enemies[usedStar][stage];
          foreach (Modification mod in GetPath(v, enemy))
              yield return Ut.Press(PlanetButtons[(int)mod + 1], 0.25f);
          yield return Ut.Press(PlanetButtons[0], 0.25f);
        }
        while (!greenLight)
            yield return true;
    }
    Modification[] GetPath(Value start, string enemy)
    {
        if (CheckEnemy(enemy, start) && !bannedRNGvalues.Contains(start)) return new Modification[0];
        Queue<Value> q = new Queue<Value>();
        List<Movement> allMoves = new List<Movement>();
        HashSet<Value> visitedValues = new HashSet<Value>();
        q.Enqueue(start);
        while (q.Count > 0)
        {
            Value cur = q.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                Value modified = cur.Modify((Modification)i);
                if (visitedValues.Add(modified))
                {
                    q.Enqueue(modified);
                    allMoves.Add(new Movement { start = cur, end = modified, mod = (Modification)i});
                }
            }

            if (CheckEnemy(enemy, cur) && !bannedRNGvalues.Contains(cur))
            {
                Debug.Log("Found end!");
                break;
            }
        }
        Movement lastMove = allMoves.First(x => CheckEnemy(enemy, x.end));
        List<Movement> path = new List<Movement>() { lastMove };
        while (!lastMove.start.Equals(start))
        {
            lastMove = allMoves.First(x => x.end.Equals(lastMove.start));
            path.Add(lastMove);
        }
        path.Reverse();
        Debug.Log(path.Select(x => x.mod).Join());
        Debug.LogFormat("{0} items in queue.", q.Count);
        return path.Select(x => x.mod).ToArray();
    }

    enum Modification
    {
        Invert,
        AddFirst,
        MultFirst,
        Shift
    }
    struct Value
    {
        public int A, B, C, D, E;
        public Value(int a, int b, int c, int d, int e)
        {
            A = a % 10;
            B = b % 10;
            C = c % 10;
            D = d % 10;
            E = e % 10;
        }
        public Value Modify(Modification modifier)
        {
            switch (modifier)
            {
                case Modification.Invert: return new Value(9 - A, 9 - B, 9 - C, 9 - D, 9 - E);
                case Modification.AddFirst: return new Value(A, A+B, A+C, A+D, A+E);
                case Modification.MultFirst: return new Value(A, A * B, A * C, A * D, A * E);
                case Modification.Shift: return new Value(B, C, D, E, A);
            }
            throw new ArgumentOutOfRangeException("modifier");
        }
    }
    struct Movement
    {
        public Value start, end;
        public Modification mod;
    }
}
