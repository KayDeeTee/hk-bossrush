using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModInventory;
using UnityEngine.UI;

public class Rewards : PaneHandler
{

    public override string PaneTitle
    {
        get { return "Rewards"; }
        set { }
    }

    public bool item1;
    public bool item2;
    public bool item3;

    public static Sprite placeholder;

    public override void PreInit()
    {
        placeholder = CanvasUtils.CanvasUtil.createSprite(BossRush.ResourceLoader.placeholder(), 0, 0, 78, 79);
        //load sprites here
    }


    public override void Init()
    {
        invItems.Add(new InvItem("item1", new Vector2(-600, -350), new Vector2(500, 150), dirPress, (System.Func<string, string, string>)getText, onSelect, onSubmit, "", true));
        invItems.Add(new InvItem("item2", new Vector2(0, -350), new Vector2(500, 150), dirPress, (System.Func<string, string, string>)getText, onSelect, onSubmit, "", true));
        invItems.Add(new InvItem("item3", new Vector2(600, -350), new Vector2(500, 150), dirPress, (System.Func<string, string, string>)getText, onSelect, onSubmit, "", true));

        invItems.Add(new InvItem("img1", new Vector2(-600, 0), new Vector2(300, 300), dirPress, (System.Func<string, string, Sprite>)getSprite, onSelect, onSubmit, "", true));
        invItems.Add(new InvItem("img2", new Vector2(0, 0), new Vector2(300, 300), dirPress, (System.Func<string, string, Sprite>)getSprite, onSelect, onSubmit, "", true));
        invItems.Add(new InvItem("img3", new Vector2(600, 0), new Vector2(300, 300), dirPress, (System.Func<string, string, Sprite>)getSprite, onSelect, onSubmit, "", true));
    }

    public override void PostInit()
    {
        findItemByName("item1").gameObject.GetComponent<Text>().fontSize = 48;
        findItemByName("item2").gameObject.GetComponent<Text>().fontSize = 48;
        findItemByName("item3").gameObject.GetComponent<Text>().fontSize = 48;

        onSelect(selected.n, selected.v);
    }

    public InvItem dirPress(MoveDirection md, string itemName)
    {
        switch (itemName)
        {
            case "item1":
                {
                    if (md == MoveDirection.UP) return selected;
                    if (md == MoveDirection.RIGHT) return findItemByName("item2");
                    if (md == MoveDirection.DOWN) return selected;
                    if (md == MoveDirection.LEFT) return findItemByName("item3");
                    break;
                }
            case "item2":
                {
                    if (md == MoveDirection.UP) return selected;
                    if (md == MoveDirection.RIGHT) return findItemByName("item3");
                    if (md == MoveDirection.DOWN) return selected;
                    if (md == MoveDirection.LEFT) return findItemByName("item1");
                    break;
                }
            case "item3":
                {
                    if (md == MoveDirection.UP) return selected;
                    if (md == MoveDirection.RIGHT) return findItemByName("item1");
                    if (md == MoveDirection.DOWN) return selected;
                    if (md == MoveDirection.LEFT) return findItemByName("item2");
                    break;
                }
        }
        return invItems[0];
    }

    public Sprite getSprite(string itemName, string varName)
    {
        return placeholder;
    }

    public string getText(string itemName, string varName)
    {
        return itemName;
    }

    public override void onMenuOpen()
    {
        findItemByName("item1").gameObject.GetComponent<Text>().text = BossRush.BossInfo.itemName(0);
        findItemByName("item2").gameObject.GetComponent<Text>().text = BossRush.BossInfo.itemName(1);
        findItemByName("item3").gameObject.GetComponent<Text>().text = BossRush.BossInfo.itemName(2);

        findItemByName("item1").gameObject.GetComponent<Text>().color = new Color(1, 1, 1);
        findItemByName("item2").gameObject.GetComponent<Text>().color = new Color(1, 1, 1);
        findItemByName("item3").gameObject.GetComponent<Text>().color = new Color(1, 1, 1);

        item1 = false;
        item2 = false;
        item3 = false;
    }


    public override void onSelect(string n, string v)
    {
    }

    public void callback(string n, string v)
    {
    }

    public override void onSubmit(string n, string v)
    {
        if (BossRush.BossRushUpdate.spawnedItems == true)
        {
            if (n == "item1" && !item1)
            {
                item1 = true;
                BossRush.BossInfo.pickupItem(0);
                findItemByName("item1").gameObject.GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f);
            }
            if (n == "item2" && !item2)
            {
                item2 = true;
                BossRush.BossInfo.pickupItem(1);
                findItemByName("item2").gameObject.GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f);
            }
            if (n == "item3" && !item3)
            {
                item3 = true;
                BossRush.BossInfo.pickupItem(2);
                findItemByName("item3").gameObject.GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
