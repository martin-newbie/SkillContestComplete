using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    GunDelayLevel,
    GunDamageLevel,
    GunSpreadLevel,
    HealPoint,//hp
    PainPoint,
    MissileAmmo,
    HomingAmmo,
    Invincible
}

public class Item : MonoBehaviour
{

    public ItemType thisType;


    void ItemEffect(ItemType item)
    {
        switch (item)
        {
            case ItemType.GunDelayLevel:
                GameManager.Instance.player.GunDelayLevel++;
                SoundManager.Instance.PlaySound("LevelUp");
                break;
            case ItemType.GunDamageLevel:
                GameManager.Instance.player.GunDamageLevel++;
                SoundManager.Instance.PlaySound("LevelUp");
                break;
            case ItemType.GunSpreadLevel:
                GameManager.Instance.player.GunSpreadLevel++;
                SoundManager.Instance.PlaySound("LevelUp");
                break;
            case ItemType.HealPoint:
                GameManager.Instance.Hp += 10f;
                break;
            case ItemType.PainPoint:
                GameManager.Instance.Pp -= 10f;
                break;
            case ItemType.MissileAmmo:
                GameManager.Instance.player.M_inventory += 15;
                break;
            case ItemType.HomingAmmo:
                GameManager.Instance.player.H_inventory += 10;
                break;
            case ItemType.Invincible:
                GameManager.Instance.player.ItemBulletProof();
                break;
        }
    }

    public void SetItemEffect(ItemType item)
    {
        ItemEffect(item);
    }

    public void SetItemEffect()
    {
        ItemType temp;
        temp = (ItemType)Random.Range(0, (int)ItemType.Invincible + 1);
        ItemEffect(temp);
    }
}
