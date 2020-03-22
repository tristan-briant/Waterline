using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseFrontier : BaseComponent {

    public int type = 0; //0 = corner beach, 1 = beach,  2 = beach corner>ground, 3 = beach>corner ground, 4 = ground, 5= corner ground...

    protected override void Start() {
        gc = (GameController)GameObject.Find("GameController").GetComponent(typeof(GameController)); //find the game engine
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();
    }

    public override void Rotate()
    {
        dir = GetComponentInParent<SlotManager>().dir;
        transform.localRotation = Quaternion.Euler(0, 0, dir * 90);
    }

    public void GetValueFromSlot()
    {
        type = GetComponentInParent<SlotManager>().type;
        dir = GetComponentInParent<SlotManager>().dir;
    }

    public void ChangeSlotImage()
    {
        SlotManager SM = GetComponentInParent<SlotManager>();
        SM.ChangeSlotImage(type);

    }

    public void InitializeSlot() { //set the Frontier slot

        SlotManager SM = GetComponentInParent<SlotManager>();
        SM.InitializeSlot(dir);
        SM.ChangeSlotImage(type);
    }


    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[0] = 0;
    }

    override public String ToString()
    {
        return "W"; //empty Wall
    }

}
