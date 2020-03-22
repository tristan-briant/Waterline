using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategorySelection : MonoBehaviour {

    public List<GameObject> buttons;
    public GameObject configPanel;

    public GameObject deck;
    public GameObject components;

    /*private void Start()
    {
        deck = GameObject.Find("DeckHolder");
        components= GameObject.Find("ContentComponent");
    }*/

    public void Toggle()
    {
        Transform parent = buttons[0].transform.parent;
        foreach (Transform child in parent.transform)  // disable All
            child.gameObject.SetActive(false);

        foreach (GameObject but in buttons)  // Enable selected
            but.SetActive(true);
    } 

    public void LaunchConfigPanel()
    {
        GameObject CP = Instantiate(configPanel);

       
        CP.transform.SetParent(GameObject.Find("CanvasConfig").transform);
        CP.transform.localScale = Vector3.one;
        CP.transform.localPosition = Vector3.zero;

        RectTransform rect = CP.transform.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 300);
        rect.anchoredPosition = new Vector2(0, 0);
    }

    public void ShowDeck()
    {
        deck.SetActive(true);
        components.SetActive(false);
    }

    public void ShowComponent()
    {
        deck.SetActive(false);
        components.SetActive(true);
    }

}
