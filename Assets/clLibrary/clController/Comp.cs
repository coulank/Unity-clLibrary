using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UclController
{
    // 補正クラス
    public class Comp
    {
    }

    public class CompValue
    {
        public float plus = 0f, minus = 0f;
        public void Set(float value)
        {
            if (value > 0f)
            {
                plus = value;
                if (plus < 0.0001) { plus = 0f; }
            }
            else if (value < 0f)
            {
                minus = -value;
                if (minus < 0.0001) { minus = 0f; }
            }
        }
        public float DoComp(float value)
        {
            if (value > 0f)
            {
                if (plus != 0f) value /= plus;
                if (value > 1f) { value = 1; }
            }
            else if (value < 0f)
            {
                if (minus != 0f) value /= minus;
                if (value < -1f) { value = -1; }
            }
            return value;
        }
    }
    public class CompPNT : Dictionary<PointType, CompValue>
    {
        public void Set(float value, PointType pointType)
        {
            CompValue cmp;
            if (ContainsKey(pointType))
            {
                cmp = this[pointType];
            }
            else
            {
                cmp = new CompValue();
                Add(pointType, cmp);
            }
            cmp.Set(value);
        }
        public float DoComp(float value, PointType pointType)
        {
            if (ContainsKey(pointType))
            {
                value = this[pointType].DoComp(value);
            }
            return value;
        }
    }
    public class CompPOS : Dictionary<PosType, CompPNT>
    {
        public void Set(float value, PosType posType, PointType pointType)
        {
            CompPNT pnt;
            if (ContainsKey(posType))
            {
                pnt = this[posType];
            }
            else
            {
                pnt = new CompPNT();
                Add(posType, pnt);
            }
            pnt.Set(value, pointType);
        }
        public float DoComp(float value, PosType posType, PointType pointType)
        {
            if (ContainsKey(posType))
            {
                value = this[posType].DoComp(value, pointType);
            }
            return value;
        }
    }
    public class CompPoint : Dictionary<ConType, CompPOS>
    {
        public void Set(float value, ConType conType, PosType posType, PointType pointType)
        {
            CompPOS pos;
            if (ContainsKey(conType))
            {
                pos = this[conType];
            }
            else
            {
                pos = new CompPOS();
                Add(conType, pos);
            }
            pos.Set(value, posType, pointType);
        }
        public CompPoint()
        {
            Set(0.75f, ConType.Switch, PosType.Left, PointType.X);
            Set(-0.55f, ConType.Switch, PosType.Left, PointType.X);
            Set(0.55f, ConType.Switch, PosType.Left, PointType.Y);
            Set(-0.70f, ConType.Switch, PosType.Left, PointType.Y);
            Set(0.62f, ConType.Switch, PosType.Right, PointType.X);
            Set(-0.70f, ConType.Switch, PosType.Right, PointType.X);
            Set(0.60f, ConType.Switch, PosType.Right, PointType.Y);
            Set(-0.70f, ConType.Switch, PosType.Right, PointType.Y);
        }
        public float DoComp(float value, ConType conType, PosType posType, PointType pointType)
        {
            if (ContainsKey(conType))
            {
                value = this[conType].DoComp(value, posType, pointType);
            }
            return value;
        }
    }
}