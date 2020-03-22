using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public static int N, M; // size of the playground

    float[][] deltai;
    float[][] deltap;

    public static BaseComponent[][] composants;
    public BaseComponent videFrontier;
    public BaseFrontier[] borders;
    public int currentLevel;
    public Text levelText;
    public Text nextLevelText;
    public Button nextButton;
    public Button prevButton;

    public bool StopperChanged;

    public bool composantChanged;
    protected bool paused = false;

    public LevelManager LVM;
    public GameObject Deck; // The Deck
    static GameObject Pg; //The "playground" from which are determined N and M 

    static bool firstPopulate = true;

    bool gameOver = false;
    bool fail = false;
    public GameObject winText;
    public GameObject loseText;

    private void Start()
    {
        LVM = FindObjectOfType<LevelManager>();
        currentLevel = LVM.currentLevel;
        Pg = GameObject.Find("Playground");

        if (currentLevel > 0 && !LVM.designerScene) // (0 mean level designer)
        {
            GameObject.Find("MainCanvas").transform.Find("HeaderDesigner").gameObject.SetActive(false);
            GameObject.Find("MainCanvas").transform.Find("HeaderLevel").gameObject.SetActive(true);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("DesignerUI"))
                go.SetActive(false);
            LevelManager.designerMode = false;

            LoadLevel();
        }
        else //level designer
        {
            GameObject.Find("MainCanvas").transform.Find("HeaderDesigner").gameObject.SetActive(true);
            GameObject.Find("MainCanvas").transform.Find("HeaderLevel").gameObject.SetActive(false);
            GameObject.Find("MainCanvas").transform.Find("Selectors/CategorySelector").gameObject.SetActive(true);
            //Pg = GameObject.Find("Playground");
            LevelManager.designerMode = true;
            nextLevelText.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);

            if (currentLevel == 0)
            {
                Designer.LoadFromPrefs();
                InitializePlayground();
            }
            else
                LoadLevel();
        }

        InvokeRepeating("Evolution", 0.0f, Engine.repetitionTime);

    }


    public void Pause(bool pause)
    {
        if (pause)
        {
            CancelInvoke();
            paused = true;
        }
        else
        {
            //CancelInvoke(); // Not to have 2 evolution running in parallel
            if (paused) InvokeRepeating("Evolution", 0.0f, 0.01f);
            paused = false;
        }
    }

    public void InitializePlayground()
    {
        ///////////// Array Initialization /////////////
        Pg = GameObject.Find("Playground");
        N = Pg.GetComponent<GridLayoutGroup>().constraintCount; // -2 for the frontier
        M = Pg.transform.childCount / N; //itou

        Engine.initialize_p_i(N, M); // create the array of currant and pressure

        composants = new BaseComponent[N][];

        for (int k = 0; k < N; k++)
        {
            composants[k] = new BaseComponent[M];
        }

        firstPopulate = true;
        PopulateComposant();

        ResizePlayGround();
        gameOver = false;
        winText.SetActive(false);
    }

    [ContextMenu("Resize Playground")]
    public void ResizePlayGround()
    {
        Transform go = Pg.transform.parent;
        RectTransform objectRectTransform = go.GetComponent<RectTransform>();
        float width = objectRectTransform.rect.width;
        float height = objectRectTransform.rect.height;
        //Debug.Log(" W :" + width + " H  " + height);
        //Debug.Log(" N :" + N + " M  " + M);

        float wx = width / (100f * N);

        bool trivialEdge = true;
        if (!LevelManager.designerMode)
        {
            for (int j = 1; j < M - 1; j++)
            {
                int i = 0;
                GameObject slot = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc = slot.GetComponentInChildren<BaseComponent>();
                if (!bc.name.Contains("Wall"))
                {
                    trivialEdge = false;
                    break;
                }
            }
            for (int j = 1; j < M - 1; j++)
            {
                int i = N - 1;
                GameObject slot = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc = slot.GetComponentInChildren<BaseComponent>();
                if (!bc.name.Contains("Wall"))
                {
                    trivialEdge = false;
                    break;
                }
            }

            if (trivialEdge)
                wx = width / (100f * (N - 1.5f));
        }

        float wy = height / (100f * M);

        float wc = Mathf.Min(wx, wy);

        Pg.transform.localScale = new Vector3(wc, wc, 1);
        Pg.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        GameObject edge = GameObject.Find("LeftEdge");
        edge.GetComponent<RectTransform>().sizeDelta = new Vector2((width / wc - (100 * N)) / 2, 100 * M);
        edge.transform.localScale = wc * Vector3.one;

        edge = GameObject.Find("RightEdge");
        edge.GetComponent<RectTransform>().sizeDelta = new Vector2((width / wc - (100 * N)) / 2, 100 * M);
        edge.transform.localScale = wc * Vector3.one;
    }


    bool HasSuccessComponent;

    public void PopulateComposant()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Field/GrassAtlas");


        for (int i = 1; i < N - 1; i++)
            for (int j = 1; j < M - 1; j++)
            {
                GameObject slot = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot


                if (slot.transform.childCount > 1)
                {
                    if (slot.transform.childCount != 2) Debug.Log("Populate error" + slot.transform.childCount);
                    BaseComponent bc = slot.transform.GetChild(1).GetComponent<BaseComponent>();
                    bc.enabled = true; //sometime usefull if exchange from the deck
                    Vector3 v = bc.transform.localPosition;
                    v.z = 0;
                    bc.transform.localPosition = v;
                    bc.transform.localScale = new Vector3(1, 1, 1);
                    composants[i][j] = bc;



                    if (firstPopulate)
                    {
                        slot.GetComponentInChildren<Image>().sprite = sprites[((i + j) % 2) * 2 + +(int)Random.Range(0, 1.999f)];
                    }
                }
                else
                {
                    if (firstPopulate)
                    {
                        slot.GetComponentInChildren<Image>().sprite = sprites[((i + j) % 2) * 2 + (int)Random.Range(0, 1.999f)];
                        slot.GetComponentInChildren<Image>().color = Color.white;
                    }
                    GameObject bc = Instantiate(Resources.Load("Components/Empty", typeof(GameObject))) as GameObject;
                    bc.transform.SetParent(slot.transform);
                    bc.transform.localScale = new Vector3(1, 1, 1);
                    bc.transform.localPosition = Vector3.zero;
                    composants[i][j] = bc.GetComponent<BaseComponent>();
                }
            }


        //if (firstPopulate)
        {
            for (int i = 0; i < N; i++)
            {
                int j = 0;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 1)
                    bc = go.transform.GetChild(1).GetComponent<BaseComponent>();
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                //bc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                bc.Rotate();
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }

            for (int i = 0; i < N; i++)
            {
                int j = M - 1;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 1)
                    bc = go.transform.GetChild(1).GetComponent<BaseComponent>();
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                bc.Rotate(); //bc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }

            for (int j = 1; j < M - 1; j++)
            {
                int i = 0;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 1)
                    bc = go.transform.GetChild(1).GetComponent<BaseComponent>();
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                bc.Rotate(); //bc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }

            for (int j = 1; j < M - 1; j++)
            {
                int i = N - 1;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 1)
                    bc = go.transform.GetChild(1).GetComponent<BaseComponent>();
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                bc.Rotate(); //bc.transform.localRotation = Quaternion.Euler(0, 0, -0);
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }
        }

        StopperChanged = true;

        firstPopulate = false;
    }


    public void PutAllStopper()
    {
        for (int j = 0; j < M; j++)
            for (int i = 0; i < N; i++)
                composants[i][j].PutStoppers();

        if (BaseComponent.itemBeingDragged != null)
        {
            BaseComponent dg = BaseComponent.itemBeingDragged.GetComponent<BaseComponent>();
            dg.PutStoppers();
        }
    }

    void Evolution()
    {

        float success = 0;

        for (int n = 0; n < Engine.repetitionStep; n++)
            Engine.OneStep1(composants);

        //if (!HasSuccessComponent) success = 0; // Avoid to trigger win animation if no success component in the scene

        success = Engine.SuccessValue(composants);

        if (!LevelManager.designerMode)
        {

            if (success >= 1 && BaseComponent.itemBeingDragged == null && !gameOver)
            {
                gameOver = true;
                StartCoroutine("WinAnimation");
            }

            if (success < 0 && !gameOver)
            {
                gameOver = true;
                fail = true;
                //CancelInvoke(); // Stop calculate the evolution
                StartCoroutine("LoseAnimation");
            }
        }
    }

    IEnumerator WinAnimation()
    {
        LVM.LevelCompleted(currentLevel);

        winText.SetActive(true);                            //lance l'animation de victoire
        yield return new WaitForSeconds(2f);
        if (currentLevel < LVM.LevelMax())
        {

            levelText.gameObject.SetActive(false);
            nextLevelText.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
        }
    }

    IEnumerator LoseAnimation()
    {
        loseText.SetActive(true);                            //lance l'animation de victoire
        yield return new WaitForSeconds(2f);
        if (LVM.language == "french")
            levelText.text = "Réessayer";
        else
            levelText.text = "Retry";
        levelText.GetComponent<Button>().enabled = true;
        levelText.GetComponent<Animator>().enabled = true;
    }

    public void LoadNextOrRetryLevel()
    {
        if (!fail)
            LVM.currentLevel = currentLevel + 1;
        SceneManager.LoadScene("level");
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        LoadLevel();
    }

    public void LoadPrevLevel()
    {
        currentLevel--;
        LoadLevel();
    }

    public void LoadLevel()
    {
        StopAllCoroutines();
        string filename = LVM.GetPlaygroundName(currentLevel);
        GetComponent<Designer>().LoadFromRessources(filename);

        Deck.GetComponent<DeckManager>().DrawDeck();

        if (currentLevel == 1)
            prevButton.gameObject.SetActive(false);
        else
            prevButton.gameObject.SetActive(true);

        if (currentLevel < LVM.LevelMax() && (LVM.LevelIsCompleted(currentLevel) || LVM.hacked))
            nextButton.gameObject.SetActive(true);
        else
            nextButton.gameObject.SetActive(false);

        levelText.gameObject.SetActive(true);
        levelText.text = levelText.GetComponent<languageManager>().GetText() + currentLevel;
        nextLevelText.gameObject.SetActive(false);
    }


    void LateUpdate()
    {

        Engine.update_composant_p_i(composants);

        if (LVM.WithStopper && StopperChanged)
        {
            PutAllStopper();
            StopperChanged = false;
        }

        if (Input.GetKey(KeyCode.Home))
        {

        }
        if (Input.GetKey(KeyCode.Escape))
        {
            // SceneManager.LoadScene(0);
            //Application.Quit();
            BackToMenu();
        }
        if (Input.GetKey(KeyCode.Menu))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Send mail");
            Application.OpenURL("mailto: ?subject=Email&body=from Unity");
        }
    }

    public void BackToMenu()
    {
        Designer.SaveToPrefs();
        SceneManager.LoadScene(0);
    }

    public void ResetComponent()
    {
        Engine.Reset_p_i(composants);
    }



}

