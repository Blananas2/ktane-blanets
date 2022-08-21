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
    int[] bannedRNGvalues = {-1, -1, -1, -1, -1, -1, -1};
    string b36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    int a = 0; int b = 0; int c = 0; int d = 0; int e = 0;
    int stage = 0;

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
        bannedRNGvalues[0] = RNG;
        RNGToLetters();

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
        e = R % 10;
        R = R / 10; d = R % 10;
        R = R / 10; c = R % 10;
        R = R / 10; b = R % 10;
        R = R / 10; a = R % 10;
    }

    void LettersToRNG () {
        RNG = a*10000 + b*1000 + c*100 + d*10 + e;
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
        yield return AnimationCoroutine.Animation(0.75f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(1, 16, d), 1));
        visible = !visible;
        if (!moduleSolved) {
            Planet.SetActive(visible);
            StatStuff.SetActive(visible);
        } else {
            Star.SetActive(visible);
        }
        yield return AnimationCoroutine.Animation(0.75f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(16, 1, d), 1));
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
                    if (CheckEnemy(stage)) {
                        Debug.LogFormat("[Neptune #{0}] {1} manipulated with {2}.", moduleId, NeptuneData.Enemies[usedStar][stage], RNG.ToString("00000"));
                        stage += 1;
                        Coins[coinCount - stage].SetActive(false);
                        bool f = false;
                        for (int k = 0; k < 7; k++) {
                            if (bannedRNGvalues[k] == -1 && !f) {
                                bannedRNGvalues[k] = RNG;
                                f = true;
                            }
                        }
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
                            a = 9-a; b = 9-b; c = 9-c; d = 9-d; e = 9-e;
                        break;
                        case 2:
                            b = (b+a)%10; c = (c+a)%10; d = (d+a)%10; e = (e+a)%10;
                        break;
                        case 3:
                            b = (b*a)%10; c = (c*a)%10; d = (d*a)%10; e = (e*a)%10;
                        break;
                        case 4:
                            int t = a; a = b; b = c; c = d; d = e; e = t;
                        break;
                    }
                    LettersToRNG();
                    ChangeRNGSprites();
                }
            }
        }
    }

    bool CheckEnemy(int s) {
        for (int m = 0; m < 7; m++) {
            if (bannedRNGvalues[m] == RNG) {
                return false;
            }
        }

        string enemy = NeptuneData.Enemies[usedStar][s];
        switch (enemy) {
            case "B": return ((32768 < RNG) && (RNG < 65536));
            case "F": return (a+b+c+d+e <= 20);
            case "G": return (a+b+c+d+e >= 24);
            case "S": return (a < b && b < c && c < d && d < e);
            case "Sd": return (a > b && b > c && c > d && d > e);
            case "Cy": return (a+b == d+e);
            case "Lk": return (a+b+c+d == e);
            case "PP": 
                if (d == 0) {return false;} 
                return ((a*100+b*10+c) % d == e);
            case "Bo": return ((a*10+d) == (b*10+e));
            case "Bo+": return ((a*100+b*10+c) == (c*100+d*10+e));
            case "HH": return (a*b*c == (d*10+e));
            case "Bk": return ((a*10+b) - (c*10+d) == e);
            case "G-": return (a+b+c+d+e >= 12);
            case "Cl": 
                if ((c*10+d) == 0) {return false;} 
                return ((float)(a*10+b) / (c*10+d) == (float)e);
            case "FS": return ((b*100+e*10+d) == 222);
            case "Wh": return (a*b > d*e);
            case "MI": return ((c == 0) && (a*b*d*e != 0));
            case "Sf": 
                if (c == 0) {return false;} 
                return (RNG % c == 6);
            case "Sw": return ((a*10+b) == (c*10+d)*e);
            case "Bw": return (a*b*c*d*e == 64);
            case "MP": return (a+b-c+d-e < 0);
            case "Pk": return ((b == a+1) && (c == a+2));
            case "Bl": return ((b*100+c*10+d) < 256);
            case "Bl+": return ((b*100+c*10+d) > 728);
            case "Dr": return (a == (b*10+c) - (d*10+e));
            case "Kp+": return (16384 > RNG);
            case "MB": return ((a*10+b) + (c*10+d) == 100);
            case "Pn": return (a*b*c*d*e != 0);
            case "Wh+": return (a*b < d*e);
            case "B+": return (32768 > RNG);
            case "Bl*": return ((256 < (b*100+c*10+d)) && ((b*100+c*10+d) < 728));
            case "CC": return ((e*100+c*10+a) % 111 == 0);
            case "Ee": 
                if ((c*10+d) == 0) {return false;} 
                return ((a*10+b) % (c*10+d) == e);
            case "Ey": return (a < b && b < c && c > d && d > e);
            case "Fw": return (a > b && b > c && c < d && d < e);
            case "G+": return (a+b+c+d+e >= 36);
            case "Gr": return (RNG % 7 == 3);
            case "Kl": return (a+b+c == (d*10+e));
            case "Kp": return ((b*10+c) == 0);
            case "MM": return (a-b+c-d+e < 0);
            case "Mn": return (a*b*c*d*e > 255);
            case "MR": return (a*e == b*d);
            case "Pg-": return (1996 > RNG);
            case "Pg+": return (98004 < RNG);
            case "Sk": 
                if ((c*10+b) == 0) {return false;} 
                return ((float)(e*10+d) / (c*10+b) == (float)a);
            case "Sn": 
                if ((d*10+b) == 0) {return false;} 
                return ((e*100+c*10+a) % (d*10+b) == 0);
            case "TB": return (65536 < RNG);
            case "Th": return ((c == e+2) && (d == e+1));
            case "Uk": return (b == d && c == e && d != e);
            case "Wg": return ((a*10+b) == (e*10+d));
        }
        return false;
    }

    IEnumerator AnimateStar () {
        Star.transform.localPosition = new Vector3(0f, 0.05f, -0.04f);
        Star.transform.localScale = new Vector3(0.02f, 0.02f, -0.02f);
        yield return AnimationCoroutine.Animation(4.128f, d => Star.transform.localEulerAngles = new Vector3(d * 4.128f * 394, 90, 90));
        Star.transform.localEulerAngles = new Vector3(180f, 90f, 90f);
        GetComponent<KMBombModule>().HandlePass();
        Debug.LogFormat("[Neptune #{0}] All enemies manipulated, module solved.", moduleId);
    }
}
