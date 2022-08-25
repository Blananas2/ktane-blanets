using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;
using System.Text;

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

    bool visible = true;
    bool isAnimating;

    Stack<int>[] currentBalls = new[] { new Stack<int>(5), new Stack<int>(5), new Stack<int>(5), new Stack<int>(5), new Stack<int>(5) };
    public Stack<int>[] solutionBalls = new Stack<int>[5];

    int heldBall = -1;
    string colorNames = "cmykw";

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
            int c = Rnd.Range(0, 5);
            int p;
            do p = Rnd.Range(0, 5);
            while (currentBalls[p].Count == 5);

            currentBalls[p].Push(c);
        }

        for (int tubeIx = 0; tubeIx < 5; tubeIx++)
        {
            solutionBalls[tubeIx] = new Stack<int>(currentBalls[tubeIx]);
            List<int> orderedBalls = currentBalls[tubeIx].ToList();
//            orderedBalls.Reverse();
            for (int slotIx = 0; slotIx < 5; slotIx++)
                PaperSprites[5 * tubeIx + slotIx].sprite = slotIx >= orderedBalls.Count ? null : PaperBalls[orderedBalls[slotIx]];
        }

        do
        {
            for (int i = 0; i < 100; i++) { //shuffle the solution to get the starting position
                int from, to;
                do
                {
                    from = Rnd.Range(0, 5);
                    to = Rnd.Range(0, 5);
                } while (from == to || currentBalls[to].Count == 5 || currentBalls[from].Count == 0); 
                currentBalls[to].Push(currentBalls[from].Pop());
              }

        } while (StacksEqual(currentBalls, solutionBalls));
        

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
        for (int tubeIx = 0; tubeIx < 5; tubeIx++)
        {
            List<int> orderedBalls = currentBalls[tubeIx].ToList();
            orderedBalls.Reverse();
            for (int slotIx = 0; slotIx < 5; slotIx++)
            {
                GameObject ball = Balls[5 * tubeIx + slotIx];
                if (slotIx >= orderedBalls.Count)
                    ball.SetActive(false);
                else
                {
                    ball.SetActive(true);
                    ball.GetComponent<MeshRenderer>().material = BallColors[orderedBalls[slotIx]];
                }
            }
        }
    }

    string Stringify (Stack<int>[] s) {
        StringBuilder output = new StringBuilder();

        int[][] stacks = s.Select(st => st.ToArray()).ToArray();
        
        for (int tubeIx = 0; tubeIx < 5; tubeIx++)
        {
            int tubeLength = stacks[tubeIx].Length;
            StringBuilder tubeStr = new StringBuilder(5);

            for (int slotIx = 0; slotIx < 5; slotIx++)
                tubeStr.Append(slotIx >= tubeLength ? '.' : colorNames[stacks[tubeIx][slotIx]]);
            output.Append(tubeStr);
            if (tubeIx != 4)
                output.Append('|');
        }
        if (heldBall != -1)
            output.Append(string.Format(" ({0})", colorNames[heldBall]));
        return output.ToString();
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
        yield return Ut.Animation(0.5f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(1, 12, d), 1));
        visible = !visible;
        Planet.SetActive(visible);
        Pivot.SetActive(visible);
        yield return Ut.Animation(0.5f, d => Background.transform.localScale = new Vector3(1, Mathf.Lerp(12, 1, d), 1));
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
                    if (currentBalls[u].Count == 0) {
                        return;
                    }
                    heldBall = currentBalls[u].Pop();
                    Audio.PlaySoundAtTransform("ping", transform);
                } else {
                    if (currentBalls[u].Count == 5) {
                        Debug.LogFormat("[Mercury #{0}] You attempted to put a ball into a full tube ({1}). Strike!", moduleId, u+1);
                        GetComponent<KMBombModule>().HandleStrike();
                    } else {
                        currentBalls[u].Push(heldBall);
                        heldBall = -1;
                        Audio.PlaySoundAtTransform("pong", transform);
                    }
                }
                UpdateVisuals();
                Debug.LogFormat("[Mercury #{0}] -> {1}", moduleId, Stringify(currentBalls));
                if (StacksEqual(currentBalls, solutionBalls)) {
                    Debug.LogFormat("[Mercury #{0}] Current state matches desired state. Module solved.", moduleId);
                    GetComponent<KMBombModule>().HandlePass();
                    moduleSolved = true;
                    Audio.PlaySoundAtTransform("game", transform);
                }
            }
        }
    }

    bool StacksEqual(Stack<int>[] a, Stack<int>[] b) {
        for (int tubeIx = 0; tubeIx < 5; tubeIx++)
            if (!a[tubeIx].SequenceEqual(b[tubeIx]))
                return false;
        return true;
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} move 1 2 3 4 5> to press those test tubes, ordered with 1 at the top and proceeding clockwise. Use !{0} hide to press the hide button.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpperInvariant();
        Match m = Regex.Match(command, @"^(?:MOVE\s+)?((?:[1-5]\s*)+)$");
        if (command == "HIDE")
        {
            yield return null;
            HideButton.OnInteract();
        }
        else if (m.Success)
        {
            yield return null;
            foreach (char ch in m.Groups[1].Value.Where(c => !char.IsWhiteSpace(c)))
                yield return Ut.Press(Tubes[ch - '1'], 0.33f);
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        List<Movement> path = FindPath(currentBalls);
        foreach (Movement movement in path)
        {
            yield return Ut.Press(Tubes[movement.ballFrom], 0.33f);
            yield return Ut.Press(Tubes[movement.ballTo], 0.33f);
        }
    }
    List<Movement> FindPath(Stack<int>[] start)
    {
        if (StacksEqual(start, solutionBalls))
            return null;
        Dictionary<int, Queue<State>> qs = new Dictionary<int, Queue<State>>(5);
        //Makes a queue for each score from [0-4]. Higher scores will be prioritized.
        for (int i = 0; i < 5; i++)
            qs.Add(i, new Queue<State>());

        List<Movement> allMoves = new List<Movement>();
        State startState = new State(start, solutionBalls, -1);
        HashSet<long> visitedStates = new HashSet<long>() { startState.GetNumericValue() };
        Movement lastMove = null;

        startState.AddToQueues(qs);
        while (qs.Any(q => q.Value.Count > 0))
        {
            //Tubes with the highest scores are more worthy to spend calc time on. 
            //Figure out what the highest score we have is and look at that. 
            int max = -1;
            for (int i = 4; i >= 0; i--)
                if (qs[i].Count > 0)
                {
                    max = i;
                    break;
                }
            //Takes the first element in the queue with our max score.
            var cur = qs[max].Dequeue();

            //Iterate through every possible ball movement.
            for (int from = 0; from < 5; from++)
            {
                //Don't pick up a ball that we just put down.
                //Also if the tube has no balls, we can't take from it.
                if (from == cur.prevTo || cur.balls[from].Count == 0)
                    continue;
                //Go through each possible destination.
                for (int to = 0; to < 5; to++)
                {
                    //Don't make redundant moves, and don't add to a tube that is already full.
                    if (from == to || cur.balls[to].Count == 5)
                        continue;

                    //Makes a copy of the Stacks.
                    Stack<int>[] newStacks = new Stack<int>[5];
                    for (int i = 0; i < 5; i++)
                        newStacks[i] = new Stack<int>(cur.balls[i]);

                    //Takes the top ball of [from] and places it on [to].
                    newStacks[to].Push(newStacks[from].Pop());
                    State newState = new State(newStacks, solutionBalls, to);

                    //If we haven't already visited this exact state, add it to our queue!
                    if (visitedStates.Add(newState.GetNumericValue()))
                    {
                        //If all 5 tubes are correctly set, we win!
                        if (newState.score == 5)
                        {
                            lastMove = new Movement { start = cur, end = newState, ballFrom = from, ballTo = to };
                            Debug.LogFormat("<Mercury #{0}> Found path!", moduleId);
                            goto End_Of_BFS; 
                        }

                        //Otherwise, it adds our movement and then adds the new state to our queues.
                        allMoves.Add(new Movement { start = cur, end = newState, ballFrom = from, ballTo = to });
                        newState.AddToQueues(qs);
                    
                    }
                }
            }
        }
        End_Of_BFS:
        Debug.Log(lastMove.end.score);
        //Log info about how long it took for funsies.
        List<int> scores = new List<int>(5);
        for (int i = 0; i < 5; i++)
            scores.Add(qs[i].Count);
        Debug.LogFormat("<Mercury #{0}> Found path to goal with score counts of {1} in queue.", moduleId, scores.Join());

        //Stores the presses in __reverse__ order. We'll reverse this at the end.
        List<Movement> path = new List<Movement>() { lastMove };
        //Recurse backwards through the movements until we hit the start state.
        //The check before the && is there to spare us a costly StacksEqual function when the states have entirely different scores.
        while (lastMove.start.score != startState.score || !StacksEqual( lastMove.start.balls, start))
        {
            lastMove = allMoves.First(x => x.end.score == lastMove.start.score && StacksEqual(x.end.balls, lastMove.start.balls));
            path.Add(lastMove);
        }
        path.Reverse();
        return path;
    }
    public struct State
    {
        public Stack<int>[] balls;
        public int prevTo;
        public int score;
        public State(Stack<int>[] balls, Stack<int>[] solution, int prevTo)
        {
            this.balls = balls;
            this.prevTo = prevTo;
            score = 0;
            //Go through each tube and for each one we have correct, increment [score].
            for (int tube = 0; tube < 5; tube++)
                if (TubesEqual(balls[tube], solution[tube]))
                    score++;
        }
        private bool TubesEqual(Stack<int> a, Stack<int> b)
        {
            if (a.Count != b.Count)
                return false;
            Stack<int> a1 = new Stack<int>(a);
            Stack<int> b1 = new Stack<int>(b);
            while (a1.Count > 0)
                if (a1.Pop() != b1.Pop())
                    return false;
            return true;
        }

        //Specialty method to shorthand add to the [qs] object (see the FindPath method)
        public void AddToQueues(Dictionary<int, Queue<State>> qs)
        {
            qs[score].Enqueue(this);
        }
        //Converts each possible state into one numeric code.
        //Basically creates a 5-digit base-1296 (6^4) number, where each digit THEN represents a 5-digit base-6 number.
        public long GetNumericValue()
        {
            long rtn = 0;
            for (int tube = 0; tube < 5; tube++)
            {
                int tubePtr = 0;
                foreach (int val in balls[tube])
                {
                    tubePtr++;
                    rtn += val + 1;
                    rtn *= 6;
                }
                for (int i = tubePtr; i < 5; i++)
                    rtn *= 6;
                rtn *= 7776;
            }
            return rtn;
        }
    }
    public class Movement
    {
        public State start;
        public State end;
        public int ballFrom;
        public int ballTo;
    }
}