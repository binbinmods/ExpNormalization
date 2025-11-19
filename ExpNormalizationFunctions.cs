using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
// using Obeliskial_Content;
// using Obeliskial_Essentials;
using System.IO;
using static UnityEngine.Mathf;
using UnityEngine.TextCore.LowLevel;
using static ExpNormalization.Plugin;
using System.Collections.ObjectModel;
using UnityEngine;

namespace ExpNormalization
{
    public class ExpNormalizationFunctions
    {
        public static Dictionary<int, SortedList<int, int>> DifficultyExpDictionary = [];

        public static int GetHighestEnemyDifficulty(Enums.CombatTier combatTier)
        {
            int highestDifficulty = 0;
            // List<EnemyData> enemiesInCombat = AtOManager.Instance.GetCurrentCombatData().GetAllEnemiesInCombat();
            // foreach (EnemyData enemy in enemiesInCombat)
            // {
            //     int enemyDifficulty = (int)enemy.GetDifficultyForTier(combatTier);
            //     if (enemyDifficulty > highestDifficulty)
            //     {
            //         highestDifficulty = enemyDifficulty;
            //     }
            // }
            return highestDifficulty;
        }

        public static int GetChampionDifficulty(Enums.CombatTier combatTier)
        {
            int highestDifficulty = 0;
            // List<EnemyData> enemiesInCombat = AtOManager.Instance.GetCurrentCombatData().GetAllEnemiesInCombat();
            // foreach (EnemyData enemy in enemiesInCombat)
            // {
            //     int enemyDifficulty = (int)enemy.GetDifficultyForTier(combatTier);
            //     if (enemyDifficulty > highestDifficulty)
            //     {
            //         highestDifficulty = enemyDifficulty;
            //     }
            // }
            return highestDifficulty;
        }

        public static int[] GetHighestExp(int difficulty, bool isAct3 = false)
        {
            List<int> highestExp = [0, 0, 0, 0];

            SortedList<int, int> expList = [];
            // expList.Last().Key;

            // List<EnemyData> enemiesInCombat = AtOManager.Instance.GetCurrentCombatData().GetAllEnemiesInCombat();
            // foreach (EnemyData enemy in enemiesInCombat)
            // {
            //     int enemyDifficulty = (int)enemy.GetDifficultyForTier(combatTier);
            //     if (enemyDifficulty > highestDifficulty)
            //     {
            //         highestDifficulty = enemyDifficulty;
            //     }
            // }
            return highestExp.ToArray();
        }

        public static int GetChampionExp(int difficulty, bool isAct3 = false)
        {
            int championExp = 0;
            // List<EnemyData> enemiesInCombat = AtOManager.Instance.GetCurrentCombatData().GetAllEnemiesInCombat();
            // foreach (EnemyData enemy in enemiesInCombat)
            // {
            //     int enemyDifficulty = (int)enemy.GetDifficultyForTier(combatTier);
            //     if (enemyDifficulty > highestDifficulty)
            //     {
            //         highestDifficulty = enemyDifficulty;
            //     }
            // }
            return championExp;
        }

        public static void SetDifficultyExpDictionary(Dictionary<string, NPCData> npcDictionary)
        {

        }



    }
}

