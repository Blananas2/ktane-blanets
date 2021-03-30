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
    public GameObject Background;

    bool Visible = true;

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

    private IEnumerator PlanetRotation() {
        var elapsed = 0f;
        while (true) {
            Planet.transform.localEulerAngles = new Vector3(elapsed / 14 * 360, 90f, 90f); //depends on time it takes to go around in 1 day
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator HidePlanet() {
        for (int i = 0; i < 25; i++) {
            yield return new WaitForSeconds(0.05f);
            Background.transform.localScale += new Vector3(0f, 0.01f, 0f); //depends on size of the planet
        }
        Visible = !Visible;
        Planet.SetActive(Visible);
        for (int i = 0; i < 25; i++) {
            yield return new WaitForSeconds(0.05f);
            Background.transform.localScale -= new Vector3(0f, 0.01f, 0f); //see above
        }
        Debug.LogFormat("<Venus #{0}> Visible toggled to {1}.", moduleId, Visible);
        yield return null;
    }

    void PlanetButtonPress(KMSelectable PlanetButton) {
        for (int i = 0; i < 6; i++) {
            if (PlanetButtons[i] == PlanetButton) {
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
                Debug.LogFormat("[Venus #{0}] Pressed {1}, module solved.", moduleId, i);
            }
        }
    }

}
