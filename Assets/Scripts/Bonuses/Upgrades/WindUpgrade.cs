using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindUpgrade : Upgrade
{
    public override void DoUpgrade() {
        GameManager.Instance.motherShip.Movable.canUseWind = true;
    }
}