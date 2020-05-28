using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            weapon.FireBullet();

        }
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(0.5f);
        weapon.FireBullet();
    }
}
