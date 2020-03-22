using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;
using System.Net.Http;
using System.Web;

public class Designer : MonoBehaviour
{

    const int MaxSize = 8;
    const int MinSize = 3;
    static public int N, M;

    public GameObject Pg;

    static public string PGdata;

    private void Awake()
    {
        Pg = GameObject.Find("Playground");
    }

    private void Start()
    {
        N = Pg.GetComponent<PlaygroundParameters>().N;
        M = Pg.GetComponent<PlaygroundParameters>().M;
    }

    public void MakeThumb(string filename)
    {
        GameObject canvas = GameObject.Find("CanvasThumbnail");
        Pg.transform.SetParent(canvas.transform);
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.ResizePlayGround();

        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);
        Camera cam = GameObject.Find("CameraThumbnail").GetComponent<Camera>();
        cam.Render();
        RenderTexture.active = cam.targetTexture;
        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        // save the image

        File.WriteAllBytes(filename, bytes);

        canvas = GameObject.Find("PlaygroundHolder");
        Pg.transform.SetParent(canvas.transform);
        gc.ResizePlayGround();

    }

    IEnumerator RenderThumbnail()
    {

        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);

        Camera cam = GameObject.Find("CameraThumbnail").GetComponent<Camera>();
        // Initialize and render

        cam.Render();
        RenderTexture.active = cam.targetTexture;

        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);

        yield return 0;

        texture.Apply();

        yield return 0;

        byte[] bytes = texture.EncodeToPNG();

        // save the image
        string imagePath = "amazingPath.png";
        File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + imagePath, bytes);

    }

    //[ContextMenu("SaveToFile")]
    public void SaveToFile()
    {
        string filename = "PlayGround" + System.DateTime.Now.ToShortDateString().Replace("/", "-") + "-"
            + System.DateTime.Now.ToLongTimeString().Replace(":", "-");

        string file = Path.Combine(Application.persistentDataPath, filename + ".txt");

        SaveToString();

        File.WriteAllText(file, PGdata);

        MakeThumb(Path.Combine(Application.persistentDataPath, filename + ".png"));
    }


    static public void SaveToPrefs()
    {

        LevelManager lvm = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        if (lvm.currentLevel == 0 || lvm.designerScene)
        {
            if (LevelManager.designerMode)
                SaveToString();

            PlayerPrefs.SetString("SandBox", PGdata);
        }
        Debug.Log(PGdata);

    }

    static public void LoadFromPrefs()
    {
        if (PlayerPrefs.HasKey("SandBox"))
        {
            PGdata = PlayerPrefs.GetString("SandBox");
            LoadFromString();
        }
        else
        {
            PGdata = Resources.Load<TextAsset>("Levels/Default-Designer").ToString();
            LoadFromString();
        }
    }

    public void SaveToResources(string path)
    {
        path.Replace(".txt", "");

        SaveToString();
        Debug.Log("Saving " + path);
        StreamWriter sr = File.CreateText("Assets/Resources/Levels/" + path + ".txt");
        sr.Write(PGdata);
        sr.Close();
        sr.Dispose();

        MakeThumb("Assets/Resources/Levels/" + path + ".png");
    }

    static public void SaveToString(bool reset = false)
    {
        GameObject PG = GameObject.Find("Playground");

        PGdata = "";
        PlaygroundParameters parameters = FindObjectOfType<PlaygroundParameters>();
        PGdata = JsonUtility.ToJson(parameters) + "\n";

        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().Deck.transform;

        foreach (Transform slot in deck)
        {
            BaseComponent component = slot.GetComponentInChildren<BaseComponent>();
            if (component != null && component.name != "" && !reset)
            {
                string path = component.PrefabPath;
                PGdata += path + "\n";
                PGdata += JsonUtility.ToJson(component) + "\n";
            }
            else
            {
                PGdata += "empty\n";
            }
        }

        foreach (Transform slot in PG.transform)
        {
            BaseComponent component = slot.GetComponentInChildren<BaseComponent>();
            if (component != null && component.name != "")
            {
                string path = component.PrefabPath;
                PGdata += path + "\n";
                PGdata += JsonUtility.ToJson(component) + "\n";
            }
            else
            {
                PGdata += "empty\n";
                PGdata += "\n";

            }

        }

    }

    void ClearPlayground()
    {
        Pg = GameObject.Find("Playground");
        int count = Pg.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = Pg.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        GameObject edge = GameObject.Find("LeftEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        edge = GameObject.Find("RightEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

    }

    //[ContextMenu("LoadFromString")]
    static public void LoadFromString()
    {
        if (PGdata == null) return;

        GameObject PG = GameObject.Find("Playground");

        PGdata = PGdata.Replace("\r", ""); //clean up string

        PG = GameObject.Find("Playground");
        int count = PG.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = PG.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        GameObject edge = GameObject.Find("LeftEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        edge = GameObject.Find("RightEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }


        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().Deck.transform;

        foreach (Transform slotDeck in deck)
        {
            if (slotDeck.transform.childCount > 1)
            {
                Transform child = slotDeck.transform.GetChild(1);
                DestroyImmediate(child.gameObject);
            }
        }


        string[] tokens = PGdata.Split('\n');

        int k = 0;
        JsonUtility.FromJsonOverwrite(tokens[k++], PG.GetComponent<PlaygroundParameters>()); //k=0 puis 1  
        //k++; // Ignore Playground Parameters

        foreach (Transform slotDeck in deck)
        {
            string prefab = tokens[k++];

            if (prefab != "empty")
            {
                GameObject component = Instantiate(Resources.Load(prefab)) as GameObject; //Instantiate(Resources.Load(prefab, typeof(GameObject))) as GameObject;
                //component.transform.SetParent(slotDeck.transform);
                component.GetComponent<BaseComponent>().ChangeParent(slotDeck.transform);

                JsonUtility.FromJsonOverwrite(tokens[k++], slotDeck.transform.GetComponentInChildren<BaseComponent>());
                slotDeck.GetComponent<CreateComponent>().Start();

            }
        }


        N = PG.GetComponent<PlaygroundParameters>().N;
        M = PG.GetComponent<PlaygroundParameters>().M;

        PG.GetComponent<GridLayoutGroup>().constraintCount = N;

        GameObject slot = CreateSlot(PG, "Field/SlotCorner", tokens[k++], 1, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type + 1, true);      //type + 1

        for (int i = 1; i < N - 1; i++)
        {
            CreateSlot(PG, "Field/SlotWall", tokens[k++], 0, tokens[k++]);
        }

        slot = CreateSlot(PG, "Field/SlotCorner", tokens[k++], 0, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type + 1, false);      //type + 1

        for (int j = 1; j < M - 1; j++)
        {
            slot = CreateSlot(PG, "Field/SlotWall", tokens[k++], 1, tokens[k++]);
            CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, true);

            for (int i = 1; i < N - 1; i++) CreateSlot(PG, "Field/SlotComponent", tokens[k++], 0, tokens[k++]); //empty component

            slot = CreateSlot(PG, "Field/SlotWall", tokens[k++], 3, tokens[k++]);
            CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, false);

        }

        slot = CreateSlot(PG, "Field/SlotCorner", tokens[k++], 2, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, true);

        for (int i = 1; i < N - 1; i++) CreateSlot(PG, "Field/SlotWall", tokens[k++], 2, tokens[k++]);

        slot = CreateSlot(PG, "Field/SlotCorner", tokens[k++], 3, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, false);


        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

    }

    public void LoadFromFile(string filename)
    {
        PGdata = File.ReadAllText(filename);
        LoadFromString();
    }

    public void LoadFromRessources(string filename)
    {
        PGdata = Resources.Load<TextAsset>("Levels/" + filename.Replace(".txt", "")).ToString();
        LoadFromString();
    }

    [ContextMenu("LoadFromLastFile")]
    public void LoadFromLastFile()
    {
        string filename;
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath, "*.txt");
        if (fileNames.Length > 0)
        {
            filename = fileNames[fileNames.Length - 1];
            PGdata = File.ReadAllText(filename);
            LoadFromString();
        }
    }

    static GameObject CreateSlotFrontier(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir, int type)
    {
        GameObject slot = Instantiate(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;

        GameObject component = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
        component.transform.SetParent(slot.transform);
        component.transform.localPosition = Vector3.zero;
        component.transform.localScale = Vector3.one;
        component.GetComponent<BaseComponent>().dir = dir;
        component.GetComponent<BaseFrontier>().type = type;

        component.GetComponent<BaseFrontier>().InitializeSlot();

        return slot;
    }

    static GameObject CreateSlotComponent(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir)
    {
        GameObject slot = Instantiate(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        slot.transform.localRotation = Quaternion.identity;

        if (PrefabComponentPath != "")
        {
            GameObject component = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
            component.transform.SetParent(slot.transform);
            component.transform.localPosition = Vector3.zero;
            component.transform.localScale = Vector3.one;
            component.GetComponent<BaseComponent>().dir = dir;
        }

        return slot;
    }

    static GameObject CreateSlot(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir, string data)
    {
        GameObject slot = Instantiate(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        slot.transform.localRotation = Quaternion.identity;

        if (PrefabComponentPath != "empty")
        {
            GameObject component = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
            //component.transform.SetParent(slot.transform);
            component.GetComponent<BaseComponent>().ChangeParent(slot.transform);
            component.transform.localPosition = Vector3.zero;
            component.transform.localScale = Vector3.one;


            JsonUtility.FromJsonOverwrite(data, slot.transform.GetComponentInChildren<BaseComponent>());
            slot.transform.GetComponentInChildren<BaseComponent>().Awake();

            if (slot.GetComponentInChildren<BaseFrontier>() != null)
            { //if corner or frontier
                component.GetComponent<BaseFrontier>().InitializeSlot();
                component.GetComponent<BaseComponent>().dir = dir;  //override frontier direction in case it is in the wrong way
                component.GetComponent<BaseComponent>().Rotate();
            }


            component.GetComponent<BaseComponent>().destroyable = LevelManager.designerMode;
        }
        return slot;

    }

    static void CreateSlotEdge(int type, bool onTheLeft)
    {
        Transform edge;
        GameObject slot = Instantiate(Resources.Load("Field/SlotEdge", typeof(GameObject))) as GameObject;
        if (onTheLeft)
            edge = GameObject.Find("LeftEdge").transform;
        else
            edge = GameObject.Find("RightEdge").transform;

        slot.transform.SetParent(edge);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;

        ChangeSlotEdgeImage(slot, type);
    }

    static public void ChangeSlotEdgeImage(GameObject slot, int type)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Field/AtlasEdge");

        Image im = slot.GetComponentInChildren<Image>();

        switch (type)
        {
            case 0:
            case 1: im.color = new Color(0, 0, 0, 0); break;
            case 2: im.color = Color.white; im.sprite = sprites[(int)UnityEngine.Random.Range(0, 1.999f)]; break;
            case 3:
            case 4:
            case 5: im.color = Color.white; im.sprite = sprites[2]; break;
        }
    }

    void DrawEdges()
    {
        foreach (Transform child in GameObject.Find("RightEdge").transform)
            Destroy(child.gameObject);

        foreach (Transform child in GameObject.Find("LeftEdge").transform)
            Destroy(child.gameObject);

        // corner is special
        if (Pg.transform.GetChild(0).GetComponentInChildren<BaseFrontier>().type == 1)
            CreateSlotEdge(2, true);
        else
            CreateSlotEdge(0, true);

        if (Pg.transform.GetChild(N - 1).GetComponentInChildren<BaseFrontier>().type == 1)
            CreateSlotEdge(2, false);
        else
            CreateSlotEdge(0, false);


        for (int j = 1; j < M; j++)
        {
            int type = Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type;
            CreateSlotEdge(type, true);

            type = Pg.transform.GetChild(j * N + N - 1).GetComponentInChildren<BaseFrontier>().type;
            CreateSlotEdge(type, false);
        }

    }

    public void ResetField()
    {
        Pg = GameObject.Find("Playground");
        PlaygroundParameters param = Pg.GetComponent<PlaygroundParameters>();
        param.N = N;
        param.M = M;

        // On retire tout
        ClearPlayground();

        Pg.GetComponent<GridLayoutGroup>().constraintCount = N;

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 6, 0);
        CreateSlotEdge(0, true);

        for (int i = 1; i < N - 1; i++) CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 0, 1);
        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 0, 0);
        CreateSlotEdge(0, false);

        for (int j = 1; j < M - 1; j++)
        {
            int type = Mathf.Min(j + 1, 4);
            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 1, type);
            CreateSlotEdge(type, true);
            for (int i = 1; i < N - 1; i++) CreateSlotComponent(Pg, "Field/SlotComponent", "", 0); //empty component
            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 3, type);
            CreateSlotEdge(type, false);

        }

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 5, 5);
        CreateSlotEdge(5, true);

        for (int i = 1; i < N - 1; i++) CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 2, 4);

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 2, 5);
        CreateSlotEdge(5, false);

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

    }

    public void ReduceWidth()
    {
        if (N > MinSize)
        {
            int i = N - 2;
            for (int j = M - 1; j >= 0; j--)
            {
                DestroyImmediate(Pg.transform.GetChild(i + j * N).gameObject);
            }
            Pg.GetComponent<GridLayoutGroup>().constraintCount = N - 1;
            Pg.GetComponent<PlaygroundParameters>().N = N - 1;
            N = N - 1;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible de réduire");
        }

    }

    public void IncreaseWidth()
    {
        if (N < MaxSize)
        {
            int i = N - 1;

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 0, 1);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i); // remet le nouveau child au bon endroit

            for (int j = 1; j < M - 1; j++)
            {
                CreateSlotComponent(Pg, "Field/SlotComponent", "", 0);
                Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i + j * (N + 1));
            }

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 2, 4);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i + (M - 1) * (N + 1));

            Pg.GetComponent<GridLayoutGroup>().constraintCount = N + 1;
            Pg.GetComponent<PlaygroundParameters>().N = N + 1;
            N = N + 1;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible d'agrandir");
        }
    }

    public void ReduceHeight()
    {
        if (M > MinSize)
        {
            int j = M - 2;
            for (int i = N - 1; i >= 0; i--)
            {
                DestroyImmediate(Pg.transform.GetChild(i + j * N).gameObject);
            }
            GameObject edge = GameObject.Find("LeftEdge");
            DestroyImmediate(edge.transform.GetChild(M - 2).gameObject);
            edge = GameObject.Find("RightEdge");
            DestroyImmediate(edge.transform.GetChild(M - 2).gameObject);

            Pg.GetComponent<PlaygroundParameters>().M = M - 1;
            M = M - 1;


            int type = Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;
                // Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 4;
            }
            //else
            Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 5;

            type = Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;
                // Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 4;
            }
            //else
            Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 5;


            Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible de réduire");
        }
    }

    public void IncreaseHeight()
    {
        if (M < MaxSize)
        {
            int j = M - 1;

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 1, 0);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(j * N); // remet le nouveau child au bon endroit

            for (int i = 1; i < N - 1; i++)
            {
                CreateSlotComponent(Pg, "Field/SlotComponent", "", 0);
                Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i + j * N);
            }

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 3, 0);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(N - 1 + j * N);

            Pg.GetComponent<PlaygroundParameters>().M = M + 1;
            M = M + 1;


            j = M - 1;
            int type = Pg.transform.GetChild((j - 2) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 3;
                Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 5;
            }
            else
                Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 4;

            type = Pg.transform.GetChild(N - 1 + (j - 2) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 3;
                Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 5;
            }
            else
                Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 4;


            Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible d'agrandir");
        }
    }

    public void IncreaseBeach(bool left)
    {
        int j = 0;
        int i = 0;
        if (!left) i = N - 1;

        for (j = 1; j < M - 1; j++)
        {
            int type = Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type;
            if (type >= 3) break; //ground detected
        }

        if (j == M - 1) return; //already full beach

        if (j == 1)
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 0;
        else
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 1;

        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type = 2;

        Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();

        DrawEdges();
    }

    public void DecreaseBeach(bool left)
    {
        int j = 0;
        int i = 0;
        if (!left) i = N - 1;

        for (j = M - 1; j > 1; j--)
        {
            int type = Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type;
            if (type <= 2) break; //beach detected
        }

        if (j == 0) return; //already full ground

        if (j == 1)
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 1;
        else
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;


        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type = 3;

        if (j == M - 2)
            Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().type = 5;
        else
            Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().type = 4;

        Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();

        DrawEdges();

        //GameObject edge = GameObject.Find(left ? "LeftEdge" : "RightEdge");
        //ChangeSlotEdgeImage(edge.transform.GetChild(j-1).gameObject, 2);
        //ChangeSlotEdgeImage(edge.transform.GetChild(j).gameObject, 4);
        //ChangeSlotEdgeImage(edge.transform.GetChild(j+1).gameObject, 4);

    }

    static bool playMode = false;
    GameObject[] designerElements;
    public void TogglePlayMode()
    {
        playMode = !playMode;

        LevelManager.designerMode = !playMode;

        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Transform deck = gc.Deck.transform;


        if (playMode)
        { //Go in playMode
            SaveToString();
            GetComponentInChildren<Text>().text = "Designer Mode";
            GameObject.Find("MainCanvas").transform.Find("Selectors/CategorySelector").gameObject.SetActive(false);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("DesignerUI"))
                go.SetActive(false);

            //deck.parent.gameObject.SetActive(true);
            LoadFromString();
        }
        else
        { //Go in designer mode
            LoadFromString();
            GetComponentInChildren<Text>().text = "Play Mode";
            GameObject.Find("MainCanvas").transform.Find("Selectors/CategorySelector").gameObject.SetActive(true);
            //deck.parent.gameObject.SetActive(false);
        }

        deck.GetComponent<DeckManager>().DrawDeck();


    }


    public void Paste()
    {
        string clipBoard = GUIUtility.systemCopyBuffer;
        Debug.Log(clipBoard);
        Debug.Log(CompressData(clipBoard, false));
        PGdata = CompressData(clipBoard, false);

        LoadFromString();
    }

    public void SendMail()
    {
        SaveToString();
        string cpstr = CompressData(PGdata);
        /*Debug.Log(PGdata);
        Debug.Log(CompressData(PGdata));
        Debug.Log(CompressData(cpstr,false));
        Debug.Log(PGdata.Equals(CompressData(CompressData(PGdata), false)));
        Debug.Log(" " + PGdata.Length + "   " + cpstr.Length);*/


        cpstr = "Copy all and paste in Waterline Sand Box:%0A#" + cpstr + "#";

        string url = "";

#if UNITY_ANDROID
        url = "mailto: ?subject=Waterline circuit&body=" + cpstr;
#endif

#if UNITY_STANDALONE

        //url =  Application.persistentDataPath;
        url = "mailto: ?subject=Waterline circuit&body=" + cpstr;
#endif

        //Application.OpenURL(url);
        //printString();
 Application.OpenURL("file://");

    }

    int DiffersAtIndex(string s1, string s2)
    {
        int index = 0;
        int min = Mathf.Min(s1.Length, s2.Length);
        while (index < min && s1[index] == s2[index])
            index++;

        return (index == min && s1.Length == s2.Length) ? -1 : index;
    }


    string[] strCorrespondance = {
        //"\"PrefabPath\"" , "PFP",
        "empty\n", "*",
        "Components/Pipeline\n{\"PrefabPath\":\"Components/Pipeline\"" , "?PL",
        "Components/Elbow\n{\"PrefabPath\":\"Components/Elbow\"" , "?EL",
        "Components/Te\n{\"PrefabPath\":\"Components/Te\"" , "?T",
        "Components/Cross\n{\"PrefabPath\":\"Components/Cross\"" , "?CR",
        "Components/Capacitor\n{\"PrefabPath\":\"Components/Capacitor\"","?CA",
        "Components/Empty\n{\"PrefabPath\":\"Components/Empty\"", "?e",
        "Components/PipelineDouble\n{\"PrefabPath\":\"Components/PipelineDouble\"","?PP",
        "Components/ElbowDouble\n{\"PrefabPath\":\"Components/ElbowDouble\"","?EE",
        "Components/Resistor\n{\"PrefabPath\":\"Components/Resistor\"","?R",
        "Components/Inductor\n{\"PrefabPath\":\"Components/Inductor\"","?I",
        "Components/Diode\n{\"PrefabPath\":\"Components/Diode\"","?D",
        "Components/transistor-NPN\n{\"PrefabPath\":\"Components/transistor-NPN\"","?NPN",
        "Components/Valve\n{\"PrefabPath\":\"Components/Valve\"","?V",
        "Sensors/Generator\n{\"PrefabPath\":\"Sensors/Generator\"","$G",
        "Sensors/PressostatTrigger\n{\"PrefabPath\":\"Sensors/PressostatTrigger\"", "$PT",
        "Sensors/PressostatCircle\n{\"PrefabPath\":\"Sensors/PressostatCircle\"","$PC",
        "Sensors/Pressostat\n{\"PrefabPath\":\"Sensors/Pressostat\"","$PS",
        "Sensors/PressostatDigital\n{\"PrefabPath\":\"Sensors/PressostatDigital\"","$PD",
        "Sensors/Debimeter\n{\"PrefabPath\":\"Sensors/Debimeter\"","$DM",
        "Sensors/Flush\n{\"PrefabPath\":\"Sensors/Flush\"","$F",
        "Sensors/WatchClock\n{\"PrefabPath\":\"Sensors/WatchClock\"","$W",
        "Frontiers/Corner\n{\"PrefabPath\":\"Frontiers/Corner\"" , "!CN",
        "Frontiers/Wall\n{\"PrefabPath\":\"Frontiers/Wall\"" , "!W",
        "Frontiers/OutLetForest\n{\"PrefabPath\":\"Frontiers/OutLetForest\"" , "!OF",
        "Frontiers/Ground\n{\"PrefabPath\":\"Frontiers/Ground\"", "!G",
        "Frontiers/DigitalInlet\n{\"PrefabPath\":\"Frontiers/DigitalInlet\"","!D",
        "Frontiers/InletDebimetreTrigger\n{\"PrefabPath\":\"Frontiers/InletDebimetreTrigger\"", "!IDT",
        "Frontiers/Inlet\n{\"PrefabPath\":\"Frontiers/Inlet\"","!IN",
        ",\"dir\":0", "D0", ",\"dir\":1", "D1",",\"dir\":2", "D2", ",\"dir\":3", "D3", ",\"dir\":4", "D4", ",\"dir\":5", "D5", ",\"dir\":6", "D6",
        "\"type\":0", "T0", "\"type\":1", "T1","\"type\":2", "T2", "\"type\":3", "T3", "\"type\":4", "T4", "\"type\":5", "T5", "\"type\":6", "T6",
        ",\"destroyable\":true" , "dT",",\"destroyable\":false" , "dF",
        ",\"isSuccess\":false", "iF",",\"isSuccess\":true", "iT",
        ",\"locked\":false", "lF",",\"locked\":true", "lT",
        ",\"mirror\":false", "mF",",\"mirror\":true", "mT",
        "\"periodic\":false","PRF","\"periodic\":true","PRT","\"periode\":","PE",
        "\"setPoint\":","SP",
        ",\"isFrontiers\":false" , "fF",",\"isFrontiers\":true" , "fT",
        "\"imaxcadran\":","IMXC","\"imax\":","IMX","\"pset\":","PS",
        "\"iMax\":","iMX","\"pMax\":","pMX",
        "\"chargeSuccess\":","CS",
        "\"symmetric\":true","ST","\"symmetric\":false","SF","\"setPointLow\":","SpL","\"setPointHigh\":","SpH",
        "\"openTime\":","YY","\"raiseTime\":","XX","\"timeOut\":","ZZ",
        "}\n", " ",
        "\"","~"
        //"\n", "%0A",
    };

    string CompressData(string data, bool compress = true)
    {
        string buf = data;


        if (compress)
        {
            buf = "" + data + "";
            for (int i = 0; i < strCorrespondance.Length / 2; i++)
            {
                buf = buf.Replace(strCorrespondance[2 * i], strCorrespondance[2 * i + 1]);
            }
        }
        else
        {
            // first remove header and tail

            int indexStart = buf.IndexOf("#") + 1;
            int indexEnd = buf.IndexOf("#", indexStart + 1);

            if (indexStart >= 0 && indexEnd >= 0)
            {
                buf = buf.Substring(indexStart, indexEnd - indexStart);
                //Debug.Log(buf);
            }



            for (int i = 0; i < strCorrespondance.Length / 2; i++)
            {
                buf = buf.Replace(strCorrespondance[2 * i + 1], strCorrespondance[2 * i]);
            }
        }


        return buf;
    }

    [ContextMenu("Playground To String")]
    public void printString()
    {
        GameObject PG = GameObject.Find("Playground");

        String str = "";
        foreach (Transform slot in PG.transform)
        {
            BaseComponent component = slot.GetComponentInChildren<BaseComponent>();
            str += component.ToString();
        }
        Debug.Log(str);

    }
 // Environment.GetCommandLineArgs Method to open 
}