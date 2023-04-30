using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;
using KModkit;

/*******************************************************************************************
The original version of the code was created by Blananas2 in its entirety on March 3rd 2022.
See Jupiter.txt if you're curious what the first version of the source code looked like.
*******************************************************************************************/

public class jupiterScript : MonoBehaviour { 

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable HideButton;
    public GameObject Planet;
	public KMSelectable Jupiter;
	public KMStatusLightParent sl;
    public GameObject Background;
	public GameObject StatusLightPosition;

    bool visible = true;
	bool isAnimating;
	int MoonCount, SunCount;
	bool Unicorn;
	int CurrentNumber;
	bool BottomClockwise, FrontClockwise;
	int RotationAmount;

	string[] NodeDesc = { "(Yellow orbital top-left)", "(Gray orbital top-left)", "(Red orbital top-right)", "(Cyan orbital top-right)", "(Red standard)", "(Green connector)", "(Blue standard)", "(Green orbital top-left)", "(Red orbital top-left)", "(Blue orbital top-right)", "(Green orbital top-right)", "(Yellow connector)", "(Goal)", "(Cyan connector)", "(Gray orbital bottom-left)", "(Blue orbital bottom-left)", "(Cyan orbital bottom-right)", "(Yellow orbital bottom-right)", "(Orange standard bottom-left)", "(Magenta connector)", "(Orange standard bottom-right)", "(White orbital bottom-left)", "(Magenta orbital bottom-left)", "(Magenta orbital bottom-right)", "(White orbital bottom-right)" };
	int VisitedNodes = 0;
	int CurrentNode = -1;
	int PreviousNode = -1;
	int PreviousStandard = -1;
	int PreviousOrbital = -1;
	int PreviousConnector = -1;
	bool[] VisitedConnectors = { false, false, false, false }; //5, 11, 13, 19

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        Jupiter.OnInteract += delegate () { JupiterPress(); return false; };
        HideButton.OnInteract += delegate () { StartCoroutine(HidePlanet()); return false; };
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(PlanetRotation());

		for (int m = 0; m < Bomb.GetSolvableModuleNames().Count(); m++) {
            if (Bomb.GetSolvableModuleNames().ToArray()[m] == "The Moon") {
               MoonCount++;
            }
            else if (Bomb.GetSolvableModuleNames().ToArray()[m] == "The Sun") {
               SunCount++;
            }
        }
        if (MoonCount == 79 && SunCount == 1) {
            Unicorn = true;
        }

		BeginCalculation();
		sl.transform.rotation = Quaternion.Euler(0, 0, sl.transform.rotation.eulerAngles.z);
    }

	void BeginCalculation () {
		if (Unicorn) {
			Debug.LogFormat("[Jupiter #{0}] Why would you load a bomb with 79 Moons, a Sun, and Jupiter?", moduleId);
			return;
		}

		CurrentNumber = Bomb.GetSerialNumberNumbers().ToArray().Sum();

		BottomClockwise = Random.Range(0, 2) == 0;
		Debug.LogFormat("[Jupiter #{0}] From the bottom, Jupiter is rotating {1}.", moduleId, BottomClockwise ? "clockwise" : "counter-clockwise");
		FrontClockwise = Random.Range(0, 2) == 0;
		Debug.LogFormat("[Jupiter #{0}] From the front, Jupiter is rotating {1}.", moduleId, FrontClockwise ? "clockwise" : "counter-clockwise");

		RotationAmount = Random.Range(0, 4);
		StatusLightPosition.transform.localEulerAngles += new Vector3(0, (float) (90 * RotationAmount), 0);
		string[] RotNames = { "top-right", "bottom-right", "bottom-left", "top-left" };
		Debug.LogFormat("[Jupiter #{0}] The status light is {1}.", moduleId, RotNames[RotationAmount]);

		Debug.LogFormat("[Jupiter #{0}] Starting number is {1}.", moduleId, CurrentNumber);
		switch (RotationAmount) {
			case 0: StandardNode(6); break;
			case 1: StandardNode(20); break;
			case 2: StandardNode(18); break;
			case 3: StandardNode(4); break;
		}
	}

	void StandardNode (int n) {
		CurrentNode = n;
		VisitedNodes += 1;
		if (VisitedNodes == 12) {
        	Debug.LogFormat("[Jupiter #{0}] Current visit total is twelve, ending movement.", moduleId);
			GoalNode(false);
        	return;
      	}

		bool[] Ruleset = { false, false, false };
		switch (n) {
			case 4: //Red
			CurrentNumber *= 3;
			Ruleset[0] = Rule('B'); Ruleset[1] = Rule('F'); Ruleset[2] = Rule('H');
			break; 
			case 6: //Blue
			float floatyNum = CurrentNumber * 1.5f;
			CurrentNumber = (int)floatyNum;
			Ruleset[0] = Rule('H'); Ruleset[1] = Rule('B'); Ruleset[2] = Rule('C');
			break; 
			default: //Orange
			if (CurrentNumber > -1) { CurrentNumber |= 1; }
			Ruleset[0] = Rule('C'); Ruleset[1] = Rule('E'); Ruleset[2] = Rule('H');
			break; 
		}
		Debug.LogFormat("[Jupiter #{0}] After going to node #{1} {2}, the number is now {3}.", moduleId, n + 1, NodeDesc[n], CurrentNumber);
		Debug.LogFormat("<Jupiter #{0}> Ruleset for #{1} {2}: {3}", moduleId, n + 1, NodeDesc[n], Ruleset.Join(", "));

		int po = PreviousOrbital;
		int pc = PreviousConnector;

		PreviousNode = n;
		PreviousStandard = n;

		if (Ruleset[0]) {
			if (po == -1) {
				switch (RotationAmount) {
					case 0: po = 21; break;
					case 1: po = 0; break;
					case 2: po = 3; break;
					case 3: po = 24; break;
				}
			}
			switch (po) {
				case 0: OrbitalNode(1); break;
				case 1: OrbitalNode(8); break;
				case 8: OrbitalNode(7); break;
				case 7: OrbitalNode(0); break;
				
				case 2: OrbitalNode(3); break;
				case 3: OrbitalNode(10); break;
				case 10: OrbitalNode(9); break;
				case 9: OrbitalNode(2); break;
				
				case 16: OrbitalNode(17); break;
				case 17: OrbitalNode(24); break;
				case 24: OrbitalNode(23); break;
				case 23: OrbitalNode(16); break;
				
				case 14: OrbitalNode(15); break;
				case 15: OrbitalNode(22); break;
				case 22: OrbitalNode(21); break;
				case 21: OrbitalNode(14); break;
			}
		} else if (Ruleset[1]) {
			switch (n) {
				case 4: if (pc == 5) { ConnectorNode(19); } else { ConnectorNode(5); } break;
				case 6: if (pc == 13) { ConnectorNode(5); } else { ConnectorNode(13); } break;
				case 18: if (pc == 19) { ConnectorNode(13); } else { ConnectorNode(11); } break;
				case 20: if (pc == 11) { ConnectorNode(11); } else { ConnectorNode(19); } break;
			}
		} else if (Ruleset[2]) {
			switch (n) {
				case 4: StandardNode(18); break;
				case 6: StandardNode(4); break;
				case 20: StandardNode(6); break;
				case 18: StandardNode(20); break;
			}
		} else {
			if (pc == -1) {
				switch (n) {
					case 4: ConnectorNode(11); break;
					case 6: ConnectorNode(5); break;
					case 20: ConnectorNode(13); break;
					case 18: ConnectorNode(19); break;
				}
			} else {
				ConnectorNode(pc);
			}
		}
	}

	void OrbitalNode (int n) {
		CurrentNode = n;
		VisitedNodes += 1;
		if (VisitedNodes == 12) {
        	Debug.LogFormat("[Jupiter #{0}] Current visit total is twelve, ending movement.", moduleId);
			GoalNode(false);
        	return;
      	}

		bool[] Ruleset = { false, false, false };
		switch (n) {
			case 2: case 8: //Red
			CurrentNumber -= 2;
			Ruleset[0] = Rule('B'); Ruleset[1] = Rule('G'); Ruleset[2] = Rule('J');
			break;
			case 0: case 17: //Yellow
			if (CurrentNumber > -1) { CurrentNumber &= 9; }
			Ruleset[0] = Rule('G'); Ruleset[1] = Rule('A'); Ruleset[2] = Rule('H');
			break;
			case 7: case 10: //Green
			CurrentNumber += 7;
			Ruleset[0] = Rule('F'); Ruleset[1] = Rule('J'); Ruleset[2] = Rule('K');
			break;
			case 9: case 15: //Blue
			CurrentNumber /= 4;
			Ruleset[0] = Rule('C'); Ruleset[1] = Rule('D'); Ruleset[2] = Rule('K');
			break;
			case 3: case 16: //Cyan
			CurrentNumber %= 5;
			Ruleset[0] = Rule('J'); Ruleset[1] = Rule('D'); Ruleset[2] = Rule('A');
			break;
			case 22: case 23: //Magenta
			if (CurrentNumber > -1) { CurrentNumber |= 11; }
			Ruleset[0] = Rule('C'); Ruleset[1] = Rule('D'); Ruleset[2] = Rule('K');
			break;
			case 21: case 24: //White
			CurrentNumber += 522;
			Ruleset[0] = Rule('A'); Ruleset[1] = Rule('D'); Ruleset[2] = Rule('H');
			break;
			case 1: case 14: //Gray
			CurrentNumber *= 7;
			Ruleset[0] = Rule('D'); Ruleset[1] = Rule('E'); Ruleset[2] = Rule('F');
			break;
		}
		Debug.LogFormat("[Jupiter #{0}] After going to node #{1} {2}, the number is now {3}.", moduleId, n + 1, NodeDesc[n], CurrentNumber);
		Debug.LogFormat("<Jupiter #{0}> Ruleset for #{1} {2}: {3}", moduleId, n + 1, NodeDesc[n], Ruleset.Join(", "));

		int pc = PreviousConnector;

		PreviousNode = n;
		PreviousOrbital = n;

		if (Ruleset[0]) {
			switch (n) {
				case 2: case 10: case 14: case 17: StandardNode(4); break;
				case 7: case 8: case 15: case 16: StandardNode(6); break;
				case 0: case 3: case 21: case 22: StandardNode(20); break;
				case 1: case 9: case 23: case 24: StandardNode(18); break;
			}
		} else if (Ruleset[1]) {
			switch (n) {
				case 0: case 1: case 7: case 8:  
					if (VisitedConnectors[0] && !VisitedConnectors[1]) {
						ConnectorNode(11);
					} else {
						ConnectorNode(5);
					}
				break;
				case 2: case 3: case 9: case 10: 
					if (VisitedConnectors[2] && !VisitedConnectors[0]) {
						ConnectorNode(5);
					} else {
						ConnectorNode(13);
					}
				break;
				case 16: case 17: case 23: case 24:  
					if (VisitedConnectors[3] && !VisitedConnectors[2]) {
						ConnectorNode(13);
					} else {
						ConnectorNode(19);
					}
				break;
				case 14: case 15: case 21: case 22:  
					if (VisitedConnectors[1] && !VisitedConnectors[3]) {
						ConnectorNode(19);
					} else {
						ConnectorNode(11);
					}
				break;
			}
		} else if (Ruleset[2]) {
			switch (n) {
				case 0: case 1: case 7: case 8: 
					if (!(pc == 13 || pc == 19) && (VisitedConnectors[0] && !VisitedConnectors[1])) {
						ConnectorNode(5);
					} else {
						ConnectorNode(11);
					}
				break;
				case 2: case 3: case 9: case 10:  
					if (!(pc == 11 || pc == 19) && (VisitedConnectors[2] && !VisitedConnectors[0])) {
						ConnectorNode(13);
					} else {
						ConnectorNode(5);
					}
				break;
				case 16: case 17: case 23: case 24:  
					if (!(pc == 5 || pc == 11) && (VisitedConnectors[3] && !VisitedConnectors[2])) {
						ConnectorNode(19);
					} else {
						ConnectorNode(13);
					}
				break;
				case 14: case 15: case 21: case 22:  
					if (!(pc == 5 || pc == 13) && (VisitedConnectors[1] && !VisitedConnectors[3])) {
						ConnectorNode(11);
					} else {
						ConnectorNode(19);
					}
				break;
			}
		} else {
			switch (n) {
				case 0: OrbitalNode(7); break;
				case 1: OrbitalNode(0); break;
				case 7: OrbitalNode(8); break;
				case 8: OrbitalNode(1); break;

				case 2: OrbitalNode(9); break;
				case 3: OrbitalNode(2); break;
				case 9: OrbitalNode(10); break;
				case 10: OrbitalNode(3); break;

				case 16: OrbitalNode(23); break;
				case 17: OrbitalNode(16); break;
				case 23: OrbitalNode(24); break;
				case 24: OrbitalNode(17); break;

				case 14: OrbitalNode(21); break;
				case 15: OrbitalNode(14); break;
				case 21: OrbitalNode(22); break;
				case 22: OrbitalNode(15); break;
			}
		}
	}

	void ConnectorNode (int n) {
		CurrentNode = n;
		VisitedNodes += 1;
		if (VisitedNodes == 12) {
        	Debug.LogFormat("[Jupiter #{0}] Current visit total is twelve, ending movement.", moduleId);
        	GoalNode(false);
			return;
      	}

		bool[] Ruleset = { false, false, false, false, false };
		switch (n) {
			case 11: //Yellow
			CurrentNumber *= CurrentNumber;
			Ruleset[0] = Rule('E'); Ruleset[1] = Rule('B'); Ruleset[2] = Rule('K'); Ruleset[3] = Rule('C'); Ruleset[4] = Rule('I');
			break;
			case 5: //Green
			Ruleset[0] = Rule('F'); Ruleset[1] = Rule('D'); Ruleset[2] = Rule('K'); Ruleset[3] = Rule('B'); Ruleset[4] = Rule('E');
			break;
			case 13: //Cyan 
			CurrentNumber %= 9;
			Ruleset[0] = Rule('I'); Ruleset[1] = Rule('D'); Ruleset[2] = Rule('E'); Ruleset[3] = Rule('A'); Ruleset[4] = Rule('F');
			break;
			case 19: //Magenta 
			CurrentNumber += 11;
			Ruleset[0] = Rule('K'); Ruleset[1] = Rule('G'); Ruleset[2] = Rule('J'); Ruleset[3] = Rule('E'); Ruleset[4] = Rule('A');
			break;
		}
		Debug.LogFormat("[Jupiter #{0}] After going to node #{1} {2}, the number is now {3}.", moduleId, n + 1, NodeDesc[n], CurrentNumber);
		Debug.LogFormat("<Jupiter #{0}> Ruleset for #{1} {2}: {3}", moduleId, n + 1, NodeDesc[n], Ruleset.Join(", "));

		int ps = PreviousStandard;

		PreviousNode = n;
		PreviousConnector = n;
		switch (n) {
			case 5: VisitedConnectors[0] = true; break;
			case 11: VisitedConnectors[1] = true; break;
			case 13: VisitedConnectors[2] = true; break;
			case 19: VisitedConnectors[3] = true; break;
		}

		if (Ruleset[0]) {
			GoalNode(true);
		} else if (Ruleset[1]) {
			switch (n) {
				case 5: ConnectorNode(13); break;
				case 11: ConnectorNode(5); break;
				case 13: ConnectorNode(19); break;
				case 19: ConnectorNode(11); break;
			}
		} else if (Ruleset[2]) {
			GoalNode(true);
		} else if (Ruleset[3]) {
			switch (n) {
				case 5: ConnectorNode(11); break;
				case 11: ConnectorNode(19); break;
				case 13: ConnectorNode(5); break;
				case 19: ConnectorNode(13); break;
			}
		} else if (Ruleset[4]) {
			GoalNode(true);
		} else {
			switch (ps) {
				case 4: OrbitalNode(0); break;
				case 6: OrbitalNode(2); break;
				case 18: OrbitalNode(14); break;
				case 20: OrbitalNode(16); break;
			}
		}
	}

	void GoalNode (bool b) {
		if (b) {
			VisitedNodes += 1;
		}
		while (CurrentNumber < 0) {
			CurrentNumber += 10;
		}
		CurrentNumber %= 10;
		Debug.LogFormat("[Jupiter #{0}] {1}he obtained number is {2}.", moduleId, ( b ? "Goal node reached, t" : "T" ), CurrentNumber);

		CurrentNumber = (CurrentNumber + VisitedNodes) % 10;
		Debug.LogFormat("[Jupiter #{0}] Jupiter must be pressed when the last digit is a {1}.", moduleId, CurrentNumber);
	}

	bool Rule (char c) {
		int[] India = { 2, 4, 7, 10, 11 };
      	int[] Juliett = { 1, 3, 5, 6, 8 };
		int[] Kilo = { 2, 4, 5, 6, 7, 8, 9, 10, 15 };

		switch (c) {
			case 'A': return VisitedNodes % 2 == 0; //Amount of nodes visited at this point is even.
			case 'B': return FrontClockwise; //Jupiter is rotating clockwise according to the defuser's perspective when they click on the module.
			case 'C': return BottomClockwise; //Jupiter is rotating clockwise according to the bottom of the planet.
			case 'D': return VisitedNodes % 2 == 1; //Amount of nodes visited at this point is odd.
			case 'E': return RotationAmount > 1; //The status light is on the left half of the module.
			case 'F': return RotationAmount < 2; //The status light is on the right half of the module.
			case 'G': return RotationAmount % 3 == 0; //The status light is on the top half of the module.
			case 'H': return RotationAmount % 3 != 0; //The status light is on the bottom half of the module.
			case 'I': return India.Contains(VisitedNodes); //If this is the 2nd, 4th, 7th, 10th, or 11th node visited.
			case 'J': return Juliett.Contains(VisitedNodes); //If this is the 1st, 3rd, 5th, 6th, or 8th node visited.
			case 'K': return ((VisitedNodes == 9) || (Kilo.Contains(PreviousNode))); //If this is either the 9th node visited, or the previous node visited was either red, green, or blue.
		}
		return false;
	}

	void JupiterPress () {
		if (moduleSolved) { return; }
		if (Unicorn) {
			Debug.LogFormat("[Jupiter #{0}] Holy shit you actually did it! I'm so impressed!", moduleId);
			GetComponent<KMBombModule>().HandlePass();
			moduleSolved = true;
			Audio.PlaySoundAtTransform("jupter", transform);
			return;
      	}
		if ((int) Bomb.GetTime() % 10 == CurrentNumber % 10) {
			Debug.LogFormat("[Jupiter #{0}] You pressed on the correct time, module disarmed.", moduleId);
			GetComponent<KMBombModule>().HandlePass();
			moduleSolved = true;
			Audio.PlaySoundAtTransform("jupter", transform);
		}
		else {
			Debug.LogFormat("[Jupiter #{0}] You pressed on a {1}, that is incorrect.", moduleId, (int) Bomb.GetTime() % 10);
			GetComponent<KMBombModule>().HandleStrike();
		}
	}

    private IEnumerator PlanetRotation () {
      var elapsed = 90f;
      var YElapsed = 90f;
      while (!moduleSolved) {
         Planet.transform.localEulerAngles = new Vector3(BottomClockwise ? elapsed / 285 * 360 : -elapsed / 285 * 360, FrontClockwise ? YElapsed / 285 * 360 : -YElapsed / 285 * 360, 90f);
         yield return null;
         elapsed += Time.deltaTime;
         YElapsed += Time.deltaTime;
      }
   }

    private IEnumerator HidePlanet() {
		if (isAnimating) yield break;
		isAnimating = true;
		if (visible)
			yield return Ut.Animation(1, d => Planet.transform.localScale = Mathf.Lerp(0.15f, 0, d) * Vector3.one);
		else
			yield return Ut.Animation(1, d => Planet.transform.localScale = Mathf.Lerp(0, 0.15f, d) * Vector3.one);
		visible = !visible;
		Debug.LogFormat("<Jupiter #{0}> Visibility toggled to {1}.", moduleId, visible);
		isAnimating = false;
	}
#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"Use <!{0} press/submit #> to press the planet when the last timer digit is #. Use <!{0} hide> to hide the planet.";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
	{
		command = command.Trim().ToUpperInvariant();
		Match m = Regex.Match(command, @"^(?:(?:MOVE|PRESS)\s+)?[0-9]$");
		if (command == "HIDE")
		{
			yield return null;
			HideButton.OnInteract();
		}
		if (m.Success)
        {
			yield return null;
			while ((int)Bomb.GetTime() % 10 != (command.Last() - '0'))
				yield return "trycancel The planet was not pressed because the command was canceled. Cancel culture strikes again...";
			Jupiter.OnInteract();
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
		if (Unicorn)
			Jupiter.OnInteract();
        else
        {
			while ((int)Bomb.GetTime() % 10 != CurrentNumber % 10)
				yield return true;
			Jupiter.OnInteract();
        }

	}
}
