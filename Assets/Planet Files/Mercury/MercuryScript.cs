using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class MercuryScript : MonoBehaviour { //depends on name

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable HideButton;
    public GameObject Planet;
    public GameObject Pivot;
    public KMSelectable[] Tubes;
    public GameObject Background;
    public Material[] BallColors;
    public GameObject[] Balls;
    public SpriteRenderer[] PaperSprites;
    public Sprite[] PaperBalls;
    public Sprite Empty;

    bool visible = true;
    bool isAnimating;

    int[] ballAmounts = { 0, 0, 0, 0, 0 };
    int[][] currentBalls = {
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 }
    };
    int[][] solutionBalls = {
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1, -1 }
    };
    int heldBall = -1;
    string colorNames = "-cmykw";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Tube in Tubes) {
            Tube.OnInteract += delegate () { TubePress(Tube); return false; };
        }

        HideButton.OnInteract += delegate () { StartCoroutine(HidePlanet()); return false; };

    }

    // Use this for initialization
    void Start () {
        StartCoroutine(PlanetRotation());
        GeneratePuzzle();
    }

    void GeneratePuzzle() {
        for (int b = 0; b < 10; b++) { //generate random ball structure
            redo:
            int c = UnityEngine.Random.Range(0,5);
            int p = UnityEngine.Random.Range(0,5);
            if (ballAmounts[p] == 5) {
                goto redo;
            } else {
                currentBalls[p][ballAmounts[p]] = c;
                ballAmounts[p] += 1;
            }
        }

        for (int e = 0; e < 25; e++) { //make structure the solution
            solutionBalls[e/5][e%5] = currentBalls[e/5][e%5];
            if (currentBalls[e/5][e%5] == -1) {
                PaperSprites[e].sprite = Empty;
            } else {
                PaperSprites[e].sprite = PaperBalls[currentBalls[e/5][e%5]];
            }
        }

        reshuffle:
        for (int i = 0; i < 100; i++) { //shuffle the solution to get the starting position
            int f = UnityEngine.Random.Range(0,5);
            int t = UnityEngine.Random.Range(0,5);
            while (f == t || ballAmounts[t] == 5 || ballAmounts[f] == 0) {
                f = UnityEngine.Random.Range(0,5);
                t = UnityEngine.Random.Range(0,5);
            }
            int m = currentBalls[f][ballAmounts[f]-1];
            currentBalls[f][ballAmounts[f]-1] = -1;
            ballAmounts[f] -= 1;
            currentBalls[t][ballAmounts[t]] = m;
            ballAmounts[t] += 1;
        }
        if (ChecksOut()) {
            goto reshuffle;
        }

        Debug.LogFormat("[Mercury #{0}] Starting state: {1}", moduleId, Stringify(currentBalls));
        Debug.LogFormat("[Mercury #{0}] Desired state: {1}", moduleId, Stringify(solutionBalls));
        UpdateVisuals();
    }

    void UpdateVisuals () {
        if (heldBall == -1) {
            Balls[25].SetActive(false);
        } else {
            Balls[25].SetActive(true);
            Balls[25].GetComponent<MeshRenderer>().material = BallColors[heldBall];
        }
        for (int v = 0; v < 25; v++) {
            if (currentBalls[v/5][v%5] == -1) {
                Balls[v].SetActive(false);   
            } else {
                Balls[v].SetActive(true);
                Balls[v].GetComponent<MeshRenderer>().material = BallColors[currentBalls[v/5][v%5]];
            }
        }
    }

    string Stringify (int[][] s) {
        string g = "";
        for (int h = 0; h < 25; h++) {
            g += colorNames[s[h/5][h%5] + 1];
            if (h % 5 == 4 && h != 24) {
                g += "|";
            }
        }
        g += " (" + colorNames[heldBall + 1] + ")";

        return g;
    }

    private IEnumerator PlanetRotation() {
        var elapsed = 0f;
        while (true) {
            Planet.transform.localEulerAngles = new Vector3(elapsed / 5 * 360, 90f, 90f); //depends on time it takes to go around in 1 day
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator HidePlanet() {
        if (isAnimating) yield break;
        isAnimating = true;
        yield return AnimationCoroutine.Animation(0.5f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(1, 12, d), 1));
        visible = !visible;
        Planet.SetActive(visible);
        Pivot.SetActive(visible);
        yield return AnimationCoroutine.Animation(0.5f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(12, 1, d), 1));
        Debug.LogFormat("<Mercury #{0}> Visible toggled to {1}.", moduleId, visible);
        isAnimating = false;
    }

    void TubePress(KMSelectable Tube) {
        if (moduleSolved) {
            return;
        }
        for (int u = 0; u < 5; u++) {
            if (Tubes[u] == Tube) {
                if (heldBall == -1) {
                    if (ballAmounts[u] == 0) {
                        return;
                    }
                    heldBall = currentBalls[u][ballAmounts[u]-1];
                    currentBalls[u][ballAmounts[u]-1] = -1;
                    ballAmounts[u] -= 1;
                    Audio.PlaySoundAtTransform("ping", transform);
                } else {
                    if (ballAmounts[u] == 5) {
                        Debug.LogFormat("[Mercury #{0}] You attempted to put a ball into a full tube ({1}). Strike!", moduleId, u+1);
                        GetComponent<KMBombModule>().HandleStrike();
                    } else {
                        currentBalls[u][ballAmounts[u]] = heldBall;
                        heldBall = -1;
                        ballAmounts[u] += 1;
                        Audio.PlaySoundAtTransform("pong", transform);
                    }
                }
                UpdateVisuals();
                Debug.LogFormat("[Mercury #{0}] -> {1}", moduleId, Stringify(currentBalls));
                if (ChecksOut()) {
                    Debug.LogFormat("[Mercury #{0}] Current state matches desired state. Module solved.", moduleId);
                    GetComponent<KMBombModule>().HandlePass();
                    moduleSolved = true;
                    Audio.PlaySoundAtTransform("game", transform);
                }
            }
        }
    }

    bool ChecksOut () {
        for (int x = 0; x < 25; x++) {
            if (currentBalls[x/5][x%5] != solutionBalls[x/5][x%5]) {
                return false;
            }
        }
        return true;
    }
}
