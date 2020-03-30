using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseComponent : MonoBehaviour, IBeginDragHandler, IDragHandler,
    IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public string PrefabPath = "";
    protected float q = 0, f = 0;
    protected GameObject configPanel;
    protected float p0, p1, p2, p3;


    public int dir = 0;
    public bool destroyable = true;
    public bool isSuccess = false;
    virtual public bool IsSuccess { get => isSuccess; set => isSuccess = value; }

    public bool locked = false;
    public bool Locked { get => locked; set { locked = value; SetLocked(); } }

    public bool mirror = false;
    public bool isFrontiers = false;

    protected float R = 1f, L = 1f, C = 0.0f, Rground = 50;
    protected float fluxMinSound = 0.01f;
    protected float[] pin = new float[4];
    protected float[] iin = new float[4];
    protected bool[] tubeEnd = { false, false, false, false }; // Has tube ends in directions 0,1,2,3
    [NonSerialized] public float success = 1;
    protected float fail = 0;
    protected const float fMinBubble = 0.05f;

    private float[] pressure = new float[4];
    public void SetPressure(float value, int index = 0)
    {
        if (float.IsNaN(value))
        {
            pressure[index] = 0;
        }
        else pressure[index] = value;
    }
    public float GetPressure(int index = 0) { return pressure[index]; }

    protected GameController gc; // le moteur du jeu à invoquer parfois
    protected AudioSource[] audios;
    PlaygroundParameters parameters;

    public virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        success = 0;
        gc = (GameController)GameObject.Find("GameController").GetComponent(typeof(GameController)); //find the game engine

        GameObject PG = GameObject.Find("Playground");
        parameters = PG.transform.GetComponent<PlaygroundParameters>();
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();

        if (parameters != null)
        {
            R = parameters.R;
            C = parameters.C;
            L = parameters.L;
        }
        else
        {
            Debug.Log("Parameters not found");
        }

        Rotate();
        SetLocked();
    }

    public bool HasTubeEnd(int direction)
    { // Tell if the component has an end in that direction
        int a = (direction - dir) % 4;
        while (a < 0) a += 4;
        return tubeEnd[a];
    }

    public void RemoveAllStoppers()
    {
        foreach (Transform child in transform)
            if (child.name.Contains("Stopper")) Destroy(child.gameObject);
    }

    virtual public void PutStopper(int direction) // Put a stopper
    {
        GameObject stopper = Instantiate(Resources.Load("Components/Stopper"), transform) as GameObject;
        stopper.transform.localPosition = Vector3.zero;
        stopper.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 * (direction - dir)));

    }

    virtual public void PutStoppers()
    {
        RemoveAllStoppers();
        for (int i = 0; i < 4; i++)
            if (HasTubeEnd(i)) PutStopper(i);
    }

    protected Color PressureColor(float p)
    { // donne la convention de pression
        float PMAX = 1.0f;

        Color max = new Color(0.3f, .80f, 0.80f);  // p=2
        Color zero = new Color(0, 100.0f / 255, 140.0f / 255);  // p=0
        Color min = new Color(150.0f / 255, 75.0f / 255, 120.0f / 255);  // p=-2

        p = Mathf.Clamp(p, -PMAX, PMAX);

        if (p >= 0)
            return Color.Lerp(zero, max, p / PMAX);
        else
            return Color.Lerp(min, zero, 1 + p / PMAX);

    }

    public void Set_i_p(float[] p, float[] i)
    {

        if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }

        for (int k = 0; k < 4; k++)
        {
            pin[k] = p[k];
            iin[k] = i[k];
        }
    }

    public void Clamp_i_p(float[] p, float[] i)
    {
        for (int k = 0; k < 4; k++)
        {
            p[k] = Mathf.Clamp(p[k], -10, 10);
            i[k] = Mathf.Clamp(i[k], -10, 10);
        }
    }

    public virtual void Constraint(float[] p, float[] i, float dt)
    {  // Put constraint here as i blocked or p imposed
        i[0] = i[1] = i[2] = i[3] = 0;
    }

    public virtual void Reset_i_p()
    {
        for (int k = 0; k < 4; k++)
        {
            pin[k] = 0;
            iin[k] = 0;
        }
        q = f = 0;
    }

    public virtual void Calcule_i_p(float[] p, float[] i, float dt)
    {
    }

    public virtual void Calcule_i_p_blocked(float[] p, float[] i, float dt, int index)
    {
        i[index] = 0;
    }

    void ToggleLocked()
    {
        Locked = !Locked;
        SetLocked();
    }

    void SetLocked()
    {
        Transform loc = transform.Find("Locked");
        if (loc)
            loc.gameObject.SetActive(Locked);
    }

    bool dragged;

    bool IsClickable()
    { // Determine if the component is clickable
        if (dragged) return false;

        if (name.Contains("Empty")) return false;

        bool designerMode = LevelManager.designerMode;
        if (locked && !designerMode) return false;

        return true;
    }

    bool IsLongClickable()
    {
        if (dragged) return false;

        bool designerMode = LevelManager.designerMode;
        return designerMode;
    }

    bool IsDraggable()
    {
        if (dragged) return false;
        if (itemBeingDragged != null) return false;

        bool designerMode = LevelManager.designerMode;

        if (locked && !designerMode) return false;

        if (name.Contains("Empty"))
        {
            if (locked && designerMode)
                return true;
            else
                return false;
        }

        if (isFrontiers && !designerMode)
            return false;

        return true;
    }

    bool IsDestroyable()
    {
        if (LevelManager.designerMode) return true;

        if (locked) return false;

        if (name.Contains("Empty")) return true;

        return destroyable;
    }

    bool IsEmpty()
    {
        if (name.Contains("Empty") && locked == false)
            return true;

        return false;
    }

    bool IsMovable()
    {
        bool designerMode = LevelManager.designerMode;

        if (designerMode)
            return true;
        else
            return !locked;

    }

    public virtual void Rotate()
    {
        transform.localRotation = Quaternion.Euler(0, 0, dir * 90);
        gc.StopperChanged = true;
    }

    float longPressDuration = 0.3f;
    bool pressing = false;
    float startPressTime;

    public void OnPointerDown(PointerEventData eventData)
    {
        startPressTime = Time.time;
        pressing = true;
    }

    protected virtual void LateUpdate()
    {
        if (pressing && (Time.time > startPressTime + longPressDuration)) //continuous press
        {
            if (IsLongClickable())
                OnLongClick();

            pressing = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.dragging) return;

        /*if ((Time.time - clickStart) > 0.25f)  // Long click
        {
            if(IsLongClickable())
                OnLongClick();
            else
                audios[1].Play();
        }
        else*/
        if ((Time.time < startPressTime + longPressDuration))
        {
            if (IsClickable())
                OnClick();
            else
                audios[1].Play();
        }
        pressing = false;
    }

    public virtual void OnClick()
    {
        if (/*!dir_locked &&*/ !isFrontiers)
        {
            dir = (dir + 1) % 4;

            Rotate();

            foreach (BaseComponent bc in FindObjectsOfType<BaseComponent>())
                if (bc.enabled)
                    bc.ResetSuccess();


            audios[0].Play();
        }
        else
        {
            audios[1].Play();
        }
    }

    public virtual void OnLongClick()
    {
        if (!IsLongClickable()) return;


        //Launch Config Panel
        foreach (ConfigPanel cp in FindObjectsOfType<ConfigPanel>())
            cp.Close();

        if (configPanel == null) return;

        GameObject CP = Instantiate(configPanel, GameObject.Find("CanvasConfig").transform);
        CP.GetComponent<ConfigPanel>().component = this;

        CP.transform.localScale = Vector3.one;
        CP.transform.localPosition = Vector3.zero;

        RectTransform rect = CP.transform.GetComponent<RectTransform>();
        //rect.sizeDelta = new Vector2(0, 300);
        rect.anchoredPosition = new Vector2(0, 0);

    }

    virtual public void ResetSuccess()
    {
        success = 0;
    }

    public static Transform startParent;
    public static Transform endParent;
    public static GameObject itemBeingDragged;

    public virtual void BlockCurrant()
    {
        f = 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDraggable())
        {
            eventData.pointerDrag = null;
            return;
        }

        itemBeingDragged = gameObject;
        startParent = transform.parent;
        endParent = null;

        //GetComponent<CanvasGroup >().blocksRaycasts = false;

        dragged = true;

        ChangeParent(GameObject.Find("CanvasDragged").transform);

        gc.PopulateComposant();

        transform.localScale = transform.localScale * 1.2f;

        BlockCurrant();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 vec = Input.mousePosition;
        vec.z = 1.0f;
        transform.position = Camera.main.ScreenToWorldPoint(vec);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Happens after OnDrop
        Drop();
    }

    public void Drop()
    {
        itemBeingDragged = null;
        transform.localScale = Vector3.one;

        if (endParent == null)
        {
            if (IsDestroyable() || startParent == null)
                StartCoroutine(DestroyComponent());
            else
                StartCoroutine(FlightToFinalPosition(startParent));

        }

        if (endParent && startParent)
        { // on échange
            if (endParent == startParent)
            {
                StartCoroutine(FlightToFinalPosition(endParent, 0.05f));
            }
            else
            {
                Transform c = null;
                if (endParent.childCount > 1)
                    c = endParent.GetChild(1);

                if (!c || c.GetComponent<BaseComponent>().IsEmpty())
                {
                    StartCoroutine(FlightToFinalPosition(endParent, 0.05f));
                }
                else
                {
                    if (c.GetComponent<BaseComponent>().IsMovable())
                    {

                        c.GetComponent<BaseComponent>().ChangeParent(GameObject.Find("CanvasDragged").transform);
                        gc.PopulateComposant();

                        c.GetComponent<BaseComponent>().StartCoroutine(c.GetComponent<BaseComponent>().FlightToFinalPosition(startParent));

                        StartCoroutine(FlightToFinalPosition(endParent, 0.05f));

                    }
                    else // retour à la case départ
                    {
                        StartCoroutine(FlightToFinalPosition(startParent));
                    }
                }

            }
        }

        if (endParent && startParent == null)
        { //provient du designer
            if (name.Contains("Rock"))
            {
                endParent.GetChild(1).GetComponent<BaseComponent>().ToggleLocked();
                Destroy(gameObject);
            }
            else
            {
                if (endParent.childCount == 1 || endParent.GetChild(1).GetComponent<BaseComponent>().IsDestroyable()) //erase if possible
                {
                    StartCoroutine(FlightToFinalPosition(endParent, 0.05f));
                }
                else
                {
                    StartCoroutine(DestroyComponent());
                }
            }
        }

        dragged = false;
        startParent = endParent = null;

    }

    protected float SpeedAnim()
    {
        if (!float.IsNaN(f))
            return Mathf.Atan(f) / fMinBubble;
        else
            return 0;
    }

    protected float SpeedAnim(float flux)
    {
        if (!float.IsNaN(flux))
            return Mathf.Atan(flux) / fMinBubble;
        else
            return 0;
    }

    protected float SpeedAnim(float f1, float f2)
    {
        if (float.IsNaN(f1) || float.IsNaN(f2))
            return 0;

        float flux;
        if ((f1 > 0 && f2 > 0) || (f1 < 0 && f2 < 0))
            flux = 0;
        else
        {
            if (Mathf.Abs(f1) < Mathf.Abs(f2))
                flux = f1;
            else
                flux = -f2;
        }
        if (!float.IsNaN(flux))
            return Mathf.Atan(flux) / fMinBubble;
        else
            return 0;
    }

    public void ChangeParent(Transform newParent, bool overrideSorting = true) // set new parent and change sorting layer
    {
        transform.SetParent(newParent);
        Canvas canvasParent = newParent.GetComponentInParent<Canvas>();
        if (canvasParent)
        {
            foreach (Canvas c in GetComponentsInChildren<Canvas>())
            {
                c.sortingLayerName = canvasParent.sortingLayerName;
                c.overrideSorting = overrideSorting;
            }
        }
    }

    public IEnumerator FlightToFinalPosition(Transform newParent, float flightTime = 0.2f, bool cleanNewParent = true)
    {
        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = newParent.position;

        float t = 0;

        while (t < flightTime)
        {
            transform.position = (initialPosition * (flightTime - t) + finalPosition * t) / flightTime;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ChangeParent(newParent);

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;


        if (cleanNewParent)
        {
            if (newParent.parent.name.Contains("Deck") && newParent.childCount == 2)
                Destroy(newParent.GetChild(1).gameObject);
            else
                DestroyImmediate(newParent.GetChild(1).gameObject);
        }

        gc = (GameController)GameObject.Find("GameController").GetComponent(typeof(GameController)); //find the game engine
        gc.PopulateComposant();

        if (newParent.GetComponent<SlotManager>() && newParent.GetComponent<SlotManager>().isSlotFrontier)
            Rotate();

        if (newParent.GetComponent<CreateComponent>())
        {  // Slot of Deck !
            newParent.GetComponent<CreateComponent>().PlaceComponent(gameObject);
        }

        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();
        audios[1].Play();
    }

    public IEnumerator DestroyComponent(float flightTime = 0.3f)
    {

        Vector3 initialScale = transform.localScale;
        float initialRotation = transform.rotation.eulerAngles.z;


        float t = 0;

        while (t < flightTime)
        {
            transform.localScale = (initialScale * (flightTime - t)) / flightTime;
            transform.rotation = Quaternion.Euler(0, 0, initialRotation + 360 * t / flightTime);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    public float Troncate(float x)
    {
        return Mathf.Round(x * 1000) / 1000;
    }

    public float Sature(float x)
    {
        if (x > 0)
            return x / (1 + x);
        else
            return x / (1 - x);
    }


    virtual public String ToString(char name = 'e')
    {
        if (name == 'e')
            return "e"; //empty component

        // sinon '[N]ame' + '[L]ocked' + [D]ir/Mirror
        String str = "";
        str += name;
        if (locked) str += "L";
        str += (dir + (mirror ? 4 : 0)).ToString();
        return str;
    }

}
