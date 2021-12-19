using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalEnums;
using UnityEngine;

namespace EnemyHitInfo
{
    public class EnemyData
    {
        public string EnemyName;

        public Dictionary<AttackTypes, int> HitData;

        public EnemyData(GameObject go)
        {
            EnemyName = go.name;
            HitData = new();
            foreach (AttackTypes type in Enum.GetValues(typeof(AttackTypes)))
            {
                HitData[type] = 0;
            }
        }
    }
}
