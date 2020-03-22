using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class CreateComponent : MonoBehaviour, IBeginDragHandler , IDragHandler ,IEndDragHandler  , IDropHandler
{
    public bool designermode=false;

    public string PrefabComponentPath = "";
    public float scale = 0.8f;

    GameObject NewComponent;

    public void Start()
    {
        if (transform.childCount == 2)
        {
            //transform.GetChild(1).GetComponent<BaseComponent>().Awake();
            transform.GetChild(1).GetComponent<BaseComponent>().enabled = false;
            transform.GetChild(1).GetComponent<BaseComponent>().ChangeParent(transform,false);
            if (transform.parent.name == "Deck")
            {
                transform.GetChild(1).localPosition = Vector3.zero;
                transform.GetChild(1).localScale = scale * Vector3.one;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.childCount == 1)  // Slot empty
        {
            eventData.pointerDrag = null;
            return;
        }
        

        if (designermode)
        { // To allow reodering or removing
            NewComponent = transform.GetChild(1).gameObject;
        }
        else
        {
            NewComponent = Instantiate(transform.GetChild(1).gameObject);
        }

        NewComponent.GetComponent<BaseComponent>().enabled = true;
 
        NewComponent.GetComponent<BaseComponent>().ChangeParent(GameObject.Find("CanvasDragged").transform);

        NewComponent.transform.localPosition = Vector3.zero;
        NewComponent.transform.localScale = Vector3.one * 1.2f;
        NewComponent.GetComponent<BaseComponent>().destroyable = true;
        NewComponent.transform.localRotation = Quaternion.Euler(0, 0, 0);

        BaseComponent.itemBeingDragged = NewComponent;
        if (designermode)
            BaseComponent.startParent = transform;
        else
            BaseComponent.startParent = null;

        BaseComponent.endParent = null;

    }

    public void OnDrag(PointerEventData eventData)
    {
       NewComponent.GetComponent<BaseComponent>().OnDrag(eventData);
       /* Vector3 vec = Input.mousePosition;
        vec.z = 1.0f;
        NewComponent.transform.position = Camera.main.ScreenToWorldPoint(vec);*/
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        NewComponent.GetComponent<BaseComponent>().OnEndDrag(eventData); //Drop();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!designermode || !BaseComponent.itemBeingDragged) return;
        if (BaseComponent.itemBeingDragged.GetComponent<BaseComponent>().isFrontiers) return;
        if (BaseComponent.itemBeingDragged.name.Contains("Empty")) return;
        if (BaseComponent.itemBeingDragged.name.Contains("Rock")) return;

        BaseComponent.endParent = transform;

    }

    public void PlaceComponent(GameObject component)
    {
        PrefabComponentPath = component.GetComponent<BaseComponent>().PrefabPath;
        GameObject c = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;

        string data = JsonUtility.ToJson(component.GetComponent<BaseComponent>());
        JsonUtility.FromJsonOverwrite(data, c.GetComponent<BaseComponent>());

        c.transform.SetParent(transform);

        c.transform.localPosition = Vector3.zero;
        c.transform.localRotation = Quaternion.identity;
        c.transform.localScale = 0.8f * Vector3.one;
        c.GetComponent<BaseComponent>().locked = false;
        c.GetComponent<BaseComponent>().enabled = false;
        c.GetComponent<BaseComponent>().StartCoroutine("Awake"); //usefull for elements that display value
        c.GetComponent<BaseComponent>().ChangeParent(transform, false);

        Destroy(component);
    }

    public void OnEnable()
    {
        if (transform.childCount == 2)
        {
            transform.GetChild(1).GetComponent<BaseComponent>().Awake();
            //transform.GetChild(1).localScale = scale * Vector3.one;
        }
    }

    public void InvokeConfig()
    {
        if (transform.childCount == 1)
            return;

        transform.GetChild(1).GetComponent<BaseComponent>().OnLongClick();
    }
}
