using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
// using static Obeliskial_Essentials.Essentials;
using System;
using static ExpNormalization.Plugin;
using static ExpNormalization.CustomFunctions;
using static ExpNormalization.ExpNormalizationFunctions;
using System.Collections.Generic;
using static Functions;
using UnityEngine;
// using Photon.Pun;
using TMPro;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using UnityEngine.Rendering;
// using Unity.TextMeshPro;

// Make sure your namespace is the same everywhere
namespace ExpNormalization
{

    [HarmonyPatch] // DO NOT REMOVE/CHANGE - This tells your plugin that this is part of the mod

    public class ExpNormalizationPatches
    {
        public static bool devMode = false; //DevMode.Value;
        public static bool bSelectingPerk = false;
        public static bool IsHost()
        {
            return GameManager.Instance.IsMultiplayer() && NetworkManager.Instance.IsMaster();
        }




        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), "GetExperienceFromCombat")]
        public static void GetExperienceFromCombat(ref int __result)
        {
            if (!AtOManager.Instance)
            {
                return;
            }
            if (!IsRandomCombat())
            {
                return;
            }
            CombatData combat = AtOManager.Instance.GetCurrentCombatData();
            if (combat == null)
            {
                LogDebug($"Exp Normalization: No combat data found for {AtOManager.Instance.currentMapNode}.");
                return;
            }
            int highestDifficulty = GetHighestEnemyDifficulty(combat.CombatTier);
            int championDifficulty = GetChampionDifficulty(combat.CombatTier);
            bool isAct3 = AtOManager.Instance.GetActNumberForText() == 3;
            int[] highestExp = GetHighestExp(highestDifficulty, isAct3: isAct3);
            int championExp = GetChampionExp(championDifficulty, isAct3: isAct3);
            __result = highestExp.Sum() + championExp;
            LogDebug($"Exp Normalization Applied: Combat {combat.CombatId}, Highest Difficulty {highestDifficulty}, Champion Difficulty {championDifficulty}, Total Exp {__result}");
            return;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Globals), "CreateGameContent")]
        public static void CreateGameContentPostfix()
        {

            SetDifficultyExpDictionary(Globals.Instance.NPCs);
            SetAct3DifficultyExpDictionary(Globals.Instance.NPCs);
        }



    }
}