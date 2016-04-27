using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class fade_in_event_handler:MonoBehaviour
{

    public void on_fade_in_finished()
    {
        combo_builder.inst.start_flash();
    }
}