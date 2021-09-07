using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCont : MonoBehaviour
{
    public static DataCont instanse;

    public FixedJoystick fixedJoystickMove;
    public FixedJoystick fixedJoystickGun;
  

  
    void Awake()
    {
        instanse = this;
    }

   
}
