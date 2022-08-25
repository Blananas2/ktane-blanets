using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class PlutoScript : MonoBehaviour { //depends on name

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable HideButton;
    public GameObject Planet;
    public GameObject Background;

    public KMSelectable PVPbutton;
    public GameObject PVPobj;
    public KMSelectable[] PVPsliders;
    public GameObject[] PVPsliderObjs;
    public TextMesh[] PVPwords;
    public Material[] PVPbackColors;
    public GameObject PVPbackObj;
    
    public GameObject AsteroidsObj;
    public KMSelectable[] Asteroids;
    public Mesh[] AsteroidMeshes;
    public Material[] AsteroidColors;
    public GameObject[] IndivAsteroidObjs;
    public TextMesh[] AsteroidTexts;

    public KMSelectable Pluto;

    bool visible = true;
    bool PVPvisible = false;
    bool theGoblinsWereAwakened;

    string[] ityWords = { "???ity", "Ability", "Abnormality", "Acidity", "Activity", "Adjustability", "Adorability", "Affectivity", "Alcoholicity", "Alienability", "Ambidexterity", "Ambiguity", "Anonymity", "Antigravity", "Antirationality", "Antistability", "Anxiety", "Atrocity", "Audibility", "Authenticity", "Authority", "Availability", "Brutality", "Bumpscocity", "Calculability", "Capacity", "Captivity", "Causality", "Celebrity", "City", "Clarity", "Community", "Complexity", "Conformity", "Convexity", "Countability", "Creativity", "Cubicity", "Curiosity", "Cylindricality", "Decipherability", "Defusivity", "Density", "Detonability", "Dexterity", "Diffusivity", "Digisibility", "Directionality", "Disability", "Discoverability", "Diversity", "Divinity", "Divisibility", "Duality", "Effectivity", "Elasticity", "Electricity", "Ellipticity", "Emotivity", "Enjoyability", "Entity", "Equality", "Ethnicity", "Exclusivity", "Exemplarity", "Exhaustivity", "Expandability", "Expansivity", "Expendability", "Extendability", "Facility", "Familiarity", "Feasibility", "Festivity", "Fishability", "Flammability", "Flexibility", "Fragility", "Fungibility", "Generosity", "Gradability", "Gravity", "Habitability", "Hatchability", "Hexeractivity", "Hideosity", "Horizontality", "Hostility", "Humanity", "Humidity", "Hydrophilicity", "Hydrophobicity", "Hypnotizability", "Identity", "Immaturity", "Impartiality", "Impenetrability", "Impossibility", "Inaccessability", "Inferiority", "Infinity", "Insanity", "Integrity", "Intensity", "Invincibilty", "Irrationality", "Irrefutability", "Irresistibilty", "Irritability", "Jusitifiability", "Linearity", "Liquidity", "Logicality", "Luminosity", "Majority", "Maturity", "Minority", "Mobility", "Monstrosity", "Mortality", "Multiplicity", "Nationality", "Nebulosity", "Negative Infinity", "Negativity", "Neuralplasticity", "Nonexclusivity", "Normality", "Nudity", "Obesity", "Objectivity", "Obscenity", "Obscurity", "Oddity", "Orbity", "Originality", "Oviparity", "Packability", "Paradoxicality", "Parity", "Peculiarity", "Perpetuality", "Personality", "Plasticity", "Plausibility", "Popularity", "Possibility", "Pregnability", "Probability", "Profanity", "Programability", "Proximity", "Pseudonymity", "Punctuality", "Purity", "Quadruplicity", "Quality", "Quantity", "Quotability", "Rarity", "Rationality", "Rhythmicity", "Sanity", "Scarcity", "Security", "Seismicity", "Sensitivity", "Serendipity", "Sexuality", "Shrinkability", "Similarity", "Simplicity", "Solutbility", "Sorority", "Sphericity", "Stability", "Stickability", "Stupidity", "Subjectivity", "Superiority", "Tensegrity", "Tesseractivity", "Tonality", "Totality", "Toxicity", "Tranquility", "Translatibility", "Uncountability", "Unilaterality", "University", "Usability", "Utility", "Validity", "Veracity", "Verticality", "Viscocity", "Visibility", "Vulgarity", "Vulnerability", "Washability" };
    int[] sliderStates = { 0, 0, 0, 0 };
    int[] sliderSFX = { -1, -1, -1, -2, -2, -2, -3, -3, -3, -4, -4, -4 };
    int chosenSound = -1;
    int[] ASScolors = { 0,2,9,4,6,1,8,5,3,7,2,8,4,1,7,5,9,0,6,3,9,3,8,4,1,7,6,2,0,5,8,4,7,2,0,5,1,3,6,9,9,3,1,5,0,4,8,2,7,6,0,8,3,9,6,1,5,7,2,4,5,6,0,4,8,9,7,3,2,1,7,8,5,3,0,4,1,6,9,2,6,1,7,8,3,9,2,5,4,0,9,3,6,1,0,4,2,8,5,7 };
    int[] ASSshapes = { 1,4,9,7,2,6,5,3,8,0,3,6,0,8,5,1,2,7,9,4,0,3,7,6,1,8,4,9,5,2,4,8,2,5,9,7,3,1,0,6,7,9,5,0,4,2,8,6,3,1,2,1,6,3,8,0,9,4,7,5,5,7,8,1,3,4,6,0,2,9,9,0,4,2,6,3,7,5,1,8,6,2,1,9,7,5,0,8,4,3,8,5,3,4,0,9,1,2,6,7 };
    //COLORS: 0=blue, 1=red, 2=green, 3=yellow, 4=magenta, 5=orange, 6=cyan, 7=white, 8=black, 9=gray
    //SHAPES: 0=goose, 1=ampersand, 2=creeper, 3=gex, 4=turd, 5=waluigi, 6=automobile, 7=gatorade_bottle, 8=bucket, 9=binary_tree
    int validTime = -1;
    int asteroidsDestroyed = 0;
    int[] fourAsteroids = {-1, -1, -1, -1};
    string[] colorNames = { "blue", "red", "green", "yellow", "magenta", "orange", "cyan", "white", "black", "gray" };
    string[] fourLetterWords = {"ROOT","WARM","THEM","MAKE","TIES","LOTS","RUGS","SUCH","STEM","DIET","RENT","AQUA","LOWS","PATH","HULL","DRAW","SLOT","BRAD","EGGS","MEMO","WIFE","SHED","FIRM","SKIP","DARK","REAR","EDGE","LOCK","BOWL","DEAD","ZONE","GOOD","FLAT","GREW","CRAP","PUBS","PEST","ORAL","FORD","MILE","TRIM","HIGH","OKAY","FLOW","HITS","JACK","RUBY","NODE","REEF","TANK","PUTS","TRIP","POST","PAIN","READ","GEAR","THEN","FATE","ARMY","MINS","DREW","WIND","FAME","FUNK","SOLD","CREW","WIFI","CITE","DAMN","FARM","NOSE","HOOD","CORN","FAIR","ADAM","TIDE","DISH","GOAL","DUMP","GUAM","GETS","SOUP","SYNC","LAWN","ROPE","CLAN","MESA","BALL","LORD","EACH","REEL","TIER","BYTE","LIFT","TUNE","HEAR","MALL","HELL","CITY","MENS","HOSE","PIKE","GOES","PALM","MILD","JURY","JUAN","SUCK","NAVY","POOR","LETS","BOOK","ACRE","INFO","MESS","JUMP","LAWS","BEST","BOFA","PURE","BUCK","TAIL","LEGS","BIND","HOST","EYES","MILF","HANG","VERY","OVAL","WELL","LENS","OOPS","DIRT","RAIN","ONTO","VICE","HOME","DUTY","VOLT","GAIN","BEAR","DAVE","GEEK","SINK","BASS","DESK","POLE","LACK","NINE","KNEW","DUCK","BLOW","ZOOM","SLOW","SPOT","MEGA","RAID","BEND","DIVE","MOMS","GROW","VOID","HOLE","TIME","ENDS","THEY","SNAP","LOGS","FROG","CAST","YEAH","PAYS","SITE","EVIL","SWAP","OILS","GENE","BOAT","ICON","FELT","RUNS","PAGE","FOLD","PAIR","PINS","ATOM","WATT","PETS","RIDE","IRON","GRIP","STUD","HORN","SOUL","FLAG","FREE","WILL","CLIP","SPAN","NAIL","RARE","NUTS","JOKE","BARS","PADS","MOON","THAT","THIS","LOUD","BIOS","BAGS","JAZZ","WITH","RATS","FEAR","WEAR","DUAL","MINE","SEAT","BIAS","FOAM","POEM","CAPE","BIDS","ASKS","TOLL","BEAN","DELL","WHOM","SWIM","BIRD","DOGS","RISE","INNS","LADY","MATH","FANS","CUPS","PROS","ZERO","NULL","YOUR","KNEE","ROLL","PREP","FROM","HAVE","CENT","POND","LIPS","WIRE","MORE","TAPE","SPAM","HIDE","ACID","BATH","BARE","HUNG","RIPE","FAIL","KEPT","JEEP","PRAY","WARS","POPE","RELY","BETA","DRAG","FONT","ARMS","CHAD","TIRE","APPS","JOSH","TILE","BEAT","FLIP","BOOB","BOOB","WAIT","JUST","DOCS","KEYS","IRAN","EXIT","LIST","NAME","ROSE","WAVE","MESH","YEAR","SOFT","LYNN","ACTS","HOLY","WALK","FUEL","ROWS","OVER","MATT","CATS","DATE","FIND","KITS","HARM","THAN","BOMB","BACK","ARCH","UGLY","COIN","MENT","PICK","FAKE","SOIL","CURE","LACE","INTO","COLD","MOST","WORN","WORK","SEES","DOUG","LAST","FOLK","TRIO","CUBE","DENY","SHOT","LUNG","QUIT","DATA","USED","NEXT","EARS","MISS","FIST","TASK","DEAN","GODS","TALE","MART","WILD","HEAT","BOND","TAXI","MASS","FOOT","MERE","POLL","BULL","TONY","BENT","SNOW","LUKE","SAGE","TRAY","PETE","CAMP","ONLY","SLIM","ISLE","WHEN","MATE","DIED","KILL","INCH","CHEF","JEFF","LANE","FILL","NUKE","BOOM","CALM","PEAK","BUZZ","GONE","WHAT","FORT","EURO","GREG","NEWS","BELL","PENN","SALT","DICK","FORK","TROY","TIED","HERE","CAVE","BIKE","DIES","DROP","RICH","WERE","TRAP","FOOL","OVEN","SOME","LOSE","SEEK","LOAD","DEER","TAGS","LIKE","JAIL","GUYS","BEEN","ONES","GAVE","CAMS","ALSO","TITS","WOOL","RANK","JOEL","HELP","EYED","SEEM","VIEW","TONS","NOON","COOK","PLUG","GRAB","HIRE","AGES","PINK","HAWK","SHOW","EVEN","CORP","MIND","LOST","TOUR","SIZE","CODE","MENU","KENT","HELD","TEND","GULF","ADDS","PINE","LIME","AIMS","PULL","HOPE","EASY","DAME","DECK","BLUE","FINE","MOLD","BLAN","AXIS","HERB","ALOT","ROLE","SOAP","WISH","IDLE","LONG","CAME","TUBE","RAIL","STAR","DRUG","COPY","YANG","KING","PICS","MINT","OPEN","CASH","REID","BUSY","NICK","TONE","PLOT","HEEL","FLEX","MEAN","HUNT","TURN","LINK","SEAL","MUCH","SIGN","SOON","HAND","TINY","RICK","COVE","WING","WASH","FILE","NECK","PORN","SEEN","SPIN","STOP","PORT","KEEP","HOUR","BUSH","CLUB","EASE","RUSH","TEXT","CHAR","DOLL","RATE","MALE","FUND","DOCK","TABS","MODS","DUMB","MODE","DICE","ROAD","GIFT","WEED","TOOK","SELL","OWEN","BITE","CHIP","DOOM","WIKI","TOOL","IDEA","BODY","PERU","EAST","MYTH","SONG","FALL","LATE","HARD","BARN","FARE","BUTT","PUNK","WANT","PACE","AREA","DUKE","MOVE","GUNS","BELT","BALD","HUGE","KIND","HERO","LOGO","WAGE","BOLT","TAKE","WENT","DAWN","FOUR","CARL","COAT","SPAS","HINT","LEAD","SHOP","PORK","NICE","GRID","BUYS","BAND","ECHO","SENT","STAY","TECH","GRAD","LEAF","PEER","MEET","OURS","BOTH","LEVY","PAST","UNIT","FACT","EDIT","AUTO","PLUS","UPON","FIVE","BRAS","FAST","SONS","GURU","GOLD","KISS","PIPE","RISK","CUBA","TOWN","MEAT","SILK","BOLD","ARTS","BANK","SCAN","ZINC","URGE","LIES","IDOL","FEEL","STAN","TIPS","CUTE","GAPS","WISE","WORD","ODDS","BILL","WARD","SEED","LAID","DOWN","NEST","CLAY","WEAK","TEAR","EVER","CARE","TALK","PALE","MARK","ELSE","SOLE","BULK","ROCK","ROOF","AGED","SAYS","LAND","NEON","DONE","GALE","GAME","TRUE","KIDS","NUDE","OWNS","CHAT","FIRE","CORK","MILL","CUTS","PENS","MARS","SAME","HEAD","GATE","ONCE","MATS","SLIP","DEAF","AWAY","CELL","SELF","WORM","LOSS","HASH","LAZY","PILL","CASE","FRED","WOOD","BABY","META","NEAR","BASE","DOME","COPE","FACE","FILM","CARB","PUMP","TERM","CAPS","ANNE","SONY","WIDE","TELL","LOAN","LABS","KEEN","CARS","BANG","ALEX","RYAN","PEAS","GIRL","ABLE","GREY","TOYS","GOLF","WOLF","URLS","BEDS","SORT","BONE","MIME","NONE","MEAL","PAUL","BLAH","MOSS","HURT","LAKE","KICK","FITS","EXAM","SURE","HALF","STEP","BUGS","BURN","SUIT","QUAD","LAMP","IRAQ","CAGE","SHOE","DASH","BOYS","OAKS","KNIT","REST","HUGH","FORM","MAIL","DEEP","PLAN","WAYS","LION","ALAN","LOOP","CROP","SICK","CAKE","TOLD","RICE","TALL","DIAL","FOOD","CULT","RULE","DEAR","GORE","VOTE","SKIN","CARD","HATE","LAMB","JANE","FELL","MUST","WALL","THOU","TREE","YORK","THUS","RAGE","MINI","BEER","FLUX","COAL","REAL","PING","WINE","SEAS","ITEM","YARD","COST","LUCK","MASK","POOL","FULL","SANS","SURF","YARN","MILK","HOOK","JAVA","LEAN","LIFE","ASIA","KNOW","POET","ROSS","USES","WEST","RING","MARY","PASS","ROOM","GARY","FEES","SAKE","BOSS","EPIC","FOUL","TEEN","JOIN","ARAB","CORD","ROME","JADE","VARY","HATS","DUDE","NOTE","LIVE","JUNK","DOOR","GOTO","WEEK","SOLO","JUNE","FEET","ALTO","DOSE","SEXY","NORM","PART","NEIL","LEFT","BEEF","TEAM","DAYS","LOOK","BOOT","SALE","WARE","PUSH","LUCY","COOL","GANG","PHIL","JOBS","MANY","WRAP","FEED","MOOD","BLOG","QUIZ","LESS","GLAD","SOFA","NEED","VAST","SAFE","JOHN","JILL","WAKE","WINS","TWIN","GOAT","GLOW","HALO","GAYS","FEAT","DONT","USER","EXEC","GRAY","POUR","SAID","MAUI","SAIL","TYPE","TILL","TION","DRUM","CALL","REED","JETS","SING","DUST","YALE","YOGA","VIDS","SETS","MICE","GIVE","TEMP","BETH","CHAN","SIDE","BITS","PARK","SEND","HOLD","DEAL","CORE","MAIN","DEBT","MAPS","HILL","DANA","SHIP","SEGA","THIN","CART","DOES","ANDY","SAND","DARE","DEMO","NOVA","DEPT","RAYS","HACK","PAID","COME","RACK","SEMI","PRIX","ANAL","JULY","MADE","FISH","SHUT","CONF","HAIR","EARN","LOVE","TEST","PLAY","PACK","BORN","LITE","ACNE","TOPS","RACE","LIBS","HALL","BEAM","LINE","CAFE","WALT","UNDO","SAVE","POSE","TENT"};
    string chosenWord = "";
    string bet = "-ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    int finalAnswer = -1;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool isAnimating;

    void Awake () {
        moduleId = moduleIdCounter++;

        Bomb.OnBombExploded += delegate { StopAllCoroutines(); }; //This should stop the sound effect when hitting reset, in theory.

        foreach (KMSelectable Asteroid in Asteroids) {
            Asteroid.OnInteract += delegate () { AsteroidPress(Asteroid); return false; };
            Asteroid.OnHighlight += delegate { Hover(Asteroid); return; };
        }

        HideButton.OnInteract += delegate () { StartCoroutine(HidePlanet()); return false; };

        PVPbutton.OnInteract += delegate () { PVPtoggle(); return false; };

        foreach (KMSelectable PVPslider in PVPsliders) {
            PVPslider.OnInteract += delegate () { SliderSlide(PVPslider); return false; };
        }

        Pluto.OnInteract += delegate () { PlutoPress(); return false; };
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(PlanetRotation());
        PVPobj.SetActive(false);
        AsteroidsObj.SetActive(false);

        chosenSound = UnityEngine.Random.Range(0,48);
        for (int s = 0; s < 4; s++) {
            PVPwords[s].text = ityWords.PickRandom();
            int x = UnityEngine.Random.Range(0,3);
            for (int w = 0; w < 3; w++) {
                if (w == x) {
                    sliderSFX[s*3+w] = chosenSound;
                } else {
                    int c = UnityEngine.Random.Range(0, 48);
                    if (c == chosenSound) {
                        c = (c + 24) % 48;
                    }
                    sliderSFX[s*3+w] = c;
                }
            }
        }

        GridShit();
        chosenWord = fourLetterWords.PickRandom();
        Debug.LogFormat("[Pluto #{0}] Chosen word is {1}.", moduleId, chosenWord);
        finalAnswer = (bet.IndexOf(chosenWord[0]) + bet.IndexOf(chosenWord[1]) + bet.IndexOf(chosenWord[2]) + bet.IndexOf(chosenWord[3])) % 60;
        Debug.LogFormat("[Pluto #{0}] Pluto must be submitted on XX:{1}.", moduleId, finalAnswer.ToString("00"));
    }

    void PVPtoggle() {
        PVPvisible = !PVPvisible;
        PVPobj.SetActive(PVPvisible);
        theGoblinsWereAwakened = true;
        if (visible)
            AsteroidsObj.SetActive(true);
        StartCoroutine(AsteroidRotation());
        StartCoroutine(MergeSort());
    }

    void SliderSlide(KMSelectable slider) {
        if (moduleSolved) {
            return;
        }
        for (int i = 0; i < 4; i++) {
            if (PVPsliders[i] == slider) {
                sliderStates[i] = (sliderStates[i] + 1) % 3;
                if (sliderStates[i] == 0) {
                    PVPsliderObjs[i].transform.localPosition += new Vector3(-6.96f, 0f, 0f);
                } else {
                    PVPsliderObjs[i].transform.localPosition += new Vector3(3.48f, 0f, 0f);
                }
                Audio.PlaySoundAtTransform("fx-"+(sliderSFX[i*3+sliderStates[i]]).ToString("00"), transform);
                if (SoundsMatch()) {
                    PVPbackObj.GetComponent<MeshRenderer>().material = PVPbackColors[1];
                } else {
                    PVPbackObj.GetComponent<MeshRenderer>().material = PVPbackColors[0];
                }
            }
        }
    }

    bool SoundsMatch () {
        if ((sliderSFX[sliderStates[0]] == sliderSFX[3+sliderStates[1]] && sliderSFX[6+sliderStates[2]] == sliderSFX[9+sliderStates[3]]) && (sliderSFX[3+sliderStates[1]] == sliderSFX[6+sliderStates[2]])) {
            return true;
        }
        return false;
    }

    void GridShit () {
        int row = UnityEngine.Random.Range(1,9);
        int col = UnityEngine.Random.Range(1,9);

        int pos = row * 10 + col;

        validTime = (Bomb.GetSerialNumber().Last() % 2 == 0) ? validTime = ASScolors[pos] : ASSshapes[pos];

        int height = UnityEngine.Random.Range(1,4);
        int width = UnityEngine.Random.Range(1,4);
        while (pos + height*10 > 99 || pos - height*10 < 0) {
            height = UnityEngine.Random.Range(1,4);
        }
        while ((pos%10 + width) > 9 || (pos%10 - width) < 0) {
            width = UnityEngine.Random.Range(1,4);
        }

        fourAsteroids[0] = pos + height*10;
        fourAsteroids[2] = pos - height*10;
        fourAsteroids[1] = pos + width;
        fourAsteroids[3] = pos - width;

        Debug.LogFormat("[Pluto #{0}] Chosen asteroids are at indexes {1}, {2}, {3}, {4}.", moduleId, fourAsteroids[0], fourAsteroids[1], fourAsteroids[2], fourAsteroids[3]);
        Debug.LogFormat("[Pluto #{0}] Center asteroid is at index {1}.", moduleId, pos);
        Debug.LogFormat("[Pluto #{0}] First valid time is {1}, all other valid times are {2}.", moduleId, validTime, (validTime%2==1) ? "0, 2, 4, 6, 8" : "1, 3, 5, 7, 9");

        for (int a = 0; a < 4; a++) {
            IndivAsteroidObjs[a].GetComponent<MeshFilter>().mesh = AsteroidMeshes[ASSshapes[fourAsteroids[a]]];
            IndivAsteroidObjs[a].GetComponent<MeshRenderer>().material = AsteroidColors[ASScolors[fourAsteroids[a]]];
        }
    }

    void AsteroidPress(KMSelectable Asteroid) {
        if (moduleSolved) {
            return;
        }
        if (!SoundsMatch()) {
            GetComponent<KMBombModule>().HandleStrike();
            Audio.PlaySoundAtTransform("fuck", transform);
            Debug.LogFormat("[Pluto #{0}] Sounds did not match, strike!", moduleId);
            return;
        }
        for (int i = 0; i < 4; i++) {
            if (Asteroids[i] == Asteroid) {
                if (asteroidsDestroyed == 0 && (int)Bomb.GetTime()%10 == validTime) {
                    Debug.LogFormat("[Pluto #{0}] First asteroid destroyed successfully.", moduleId);
                    IndivAsteroidObjs[i].SetActive(false);
                    StartCoroutine(ShowLetter(i, asteroidsDestroyed));
                    asteroidsDestroyed += 1;
                } else if (asteroidsDestroyed != 0 && ((int)Bomb.GetTime()%2 != validTime%2)) {
                    switch (asteroidsDestroyed) {
                        case 1: Debug.LogFormat("[Pluto #{0}] Second asteroid destroyed successfully.", moduleId); break;
                        case 2: Debug.LogFormat("[Pluto #{0}] Third asteroid destroyed successfully.", moduleId); break;
                        case 3: Debug.LogFormat("[Pluto #{0}] Fourth asteroid destroyed successfully.", moduleId); break;
                    }
                    IndivAsteroidObjs[i].SetActive(false);
                    StartCoroutine(ShowLetter(i, asteroidsDestroyed));
                    asteroidsDestroyed += 1;
                } else {
                    GetComponent<KMBombModule>().HandleStrike();
                    Audio.PlaySoundAtTransform("fuck", transform);
                    switch (asteroidsDestroyed) {
                        case 0: Debug.LogFormat("[Pluto #{0}] First asteroid was not destroyed successfully. Strike!", moduleId); break;
                        case 1: Debug.LogFormat("[Pluto #{0}] Second asteroid was not destroyed successfully. Strike!", moduleId); break;
                        case 2: Debug.LogFormat("[Pluto #{0}] Third asteroid was not destroyed successfully. Strike!", moduleId); break;
                        case 3: Debug.LogFormat("[Pluto #{0}] Fourth asteroid was not destroyed successfully. Strike!", moduleId); break;
                    }
                }
            }
        }
    }

    void PlutoPress () {
        if (moduleSolved) {
            return;
        }
        if (asteroidsDestroyed != 4) {
            GetComponent<KMBombModule>().HandleStrike();
            Audio.PlaySoundAtTransform("fuck", transform);
            Debug.LogFormat("[Pluto #{0}] Not all asteroids were destroyed. Strike!", moduleId);
        }
        if ((int)Bomb.GetTime()%60 == finalAnswer) {
            GetComponent<KMBombModule>().HandlePass();
            Debug.LogFormat("[Pluto #{0}] Alvin Flang will be proud. Module solved.", moduleId);
            Audio.PlaySoundAtTransform("BEST BUY", transform);
            moduleSolved = true;
        } else {
            GetComponent<KMBombModule>().HandleStrike();
            Audio.PlaySoundAtTransform("fuck", transform);
            Debug.LogFormat("[Pluto #{0}] Pluto pressed at an incorrect time. Strike!", moduleId);
            for (int n = 0; n < 4; n++) {
                StartCoroutine(ShowLetter(n, n));
            }
        }
    }

    void Hover (KMSelectable Asteroid) {
        for (int a = 0; a < 4; a++) {
            if (Asteroids[a] == Asteroid) {
                Audio.PlaySoundAtTransform(colorNames[ASScolors[fourAsteroids[a]]], transform);
            }
        }
    }

    private IEnumerator AsteroidRotation() {
        var elapsed = 0f;
        while (true) {
            AsteroidsObj.transform.localEulerAngles = new Vector3(0f, elapsed * 25, 0f); 
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator ShowLetter(int a, int x) {
        AsteroidTexts[a].text = chosenWord[x].ToString();
        yield return new WaitForSeconds(1);
        AsteroidTexts[a].text = "   ";
    }

    private IEnumerator MergeSort() {
        while (asteroidsDestroyed != 4) {
            Audio.PlaySoundAtTransform("MERGE SORT", transform);
            yield return new WaitForSeconds(14.695f);
            yield return null;
        }
    }

    private IEnumerator PlanetRotation() {
        bool swag = true;
        float elapsed = 0f;
        while (swag) {
            if (!moduleSolved) {
                Planet.transform.localEulerAngles = new Vector3(elapsed / 2142000, 90f, 90f); //depends on time it takes to go around in 1 day, REALLY FUCKING SLOW
            } else {
                elapsed = 0f;
                Planet.transform.localPosition += new Vector3(-0.05f, 0f, 0.05f);
                if (elapsed > 10f) {
                    swag = !swag;
                }
            }
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator HidePlanet() {
        if (!moduleSolved && !isAnimating) {
            isAnimating = true;
            yield return Ut.Animation(0.5f, d => Background.transform.transform.transform.transform.transform.transform.transform.localScale = new Vector3(1, Mathf.Lerp(1, 10, d), 1));
            visible = !visible;
            if (theGoblinsWereAwakened)
                AsteroidsObj.SetActive(visible);
            Planet.SetActive(visible);
            yield return Ut.Animation(0.5f, d => Background.transform.transform.transform.transform.transform.transform.transform.transform.localScale = new Vector3(1, Mathf.Lerp(10, 1, d), 1));
            Debug.LogFormat("<Pluto #{0}> Visible toggled to {1}.", moduleId, visible);
            isAnimating = false;
        }
        yield return null;
    }

}
