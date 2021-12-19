using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnemyHitInfo
{
    public class EnemyHitInfo : Mod
    {
        internal static EnemyHitInfo instance;
        
        public EnemyHitInfo() : base(null)
        {
            instance = this;
        }
        
		public override string GetVersion()
		{
            return "0.1";
		}
        
        public override void Initialize()
        {
            Log("Initializing Mod...");

            ModHooks.OnEnableEnemyHook += ModHooks_OnEnableEnemyHook;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            On.HealthManager.TakeDamage += HealthManager_TakeDamage;

            DebugMod.AddActionToKeyBindList(CycleForward, "Next Enemy", nameof(EnemyHitInfo));
            DebugMod.AddActionToKeyBindList(CycleBackward, "Previous Enemy", nameof(EnemyHitInfo));
            DebugMod.CreateSimpleInfoPanel(nameof(EnemyHitInfo), 200);
            DebugMod.AddInfoToSimplePanel(nameof(EnemyHitInfo), null, null);
            DebugMod.AddInfoToSimplePanel(nameof(EnemyHitInfo), "Enemy Name", () => { try { return Data[CurrentEnemy].EnemyName; } catch { return ""; } });
            DebugMod.AddInfoToSimplePanel(nameof(EnemyHitInfo), null, null);
            foreach (AttackTypes type in Enum.GetValues(typeof(AttackTypes)))
            {
                DebugMod.AddInfoToSimplePanel(nameof(EnemyHitInfo), type.ToString(), () => { try { return Data[CurrentEnemy].HitData[type].ToString(); } catch { return ""; } });
            }
        }

        private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (Data.TryGetValue(self.gameObject, out EnemyData data))
            {
                data.HitData[hitInstance.AttackType] += 1;
            }
            orig(self, hitInstance);
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            Enemies.Clear();
            Data.Clear();
            CurrentEnemy = null;
        }

        private bool ModHooks_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
        {
            if (!isAlreadyDead)
            {
                if (Data.Count == 0) CurrentEnemy = enemy;
                Enemies.Add(enemy);
                Data.Add(enemy, new(enemy));
            }
            return isAlreadyDead;
        }

        public void CycleForward()
        {
            if (Enemies.Count == 0) return;
            int i;
            if (Enemies.Contains(CurrentEnemy))
            {
                i = Enemies.IndexOf(CurrentEnemy);
            }
            else
            {
                i = -1;
            }
            i = (i + 1) % (Enemies.Count);
            CurrentEnemy = Enemies[i];
        }

        public void CycleBackward()
        {
            if (Enemies.Count == 0) return;
            int i;
            if (Enemies.Contains(CurrentEnemy))
            {
                i = Enemies.IndexOf(CurrentEnemy);
            }
            else
            {
                i = 1;
            }
            i = (i - 1) % (Enemies.Count);
            CurrentEnemy = Enemies[i];
        }

        public GameObject CurrentEnemy = null;

        public List<GameObject> Enemies = new();
        public Dictionary<GameObject, EnemyData> Data = new();
    }
}