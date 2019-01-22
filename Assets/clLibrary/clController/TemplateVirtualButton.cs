using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UclController
{
    public class TemplateVirtualButton : VirtualButton
    {
        new void Start()
        {
            Start((object)this);
        }
        new void Update()
        {
            base.Update();
        }
    }
}