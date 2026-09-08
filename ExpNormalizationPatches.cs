using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
// using static Obeliskial_Essentials.Essentials;
using System;
using static ExpNormalization.Plugin;
// using static ExpNormalization.CustomFunctions;
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
using System.Collections;
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
                LogDebug("Exp Normalization: Null AtOManager. Skipping EXP normalization.");
                return;
            }
            if (!IsRandomCombat())
            {
                LogDebug("Exp Normalization: Not a random combat encounter. Skipping EXP normalization.");
                return;
            }
            CombatData combat = AtOManager.Instance.GetCurrentCombatData();
            if (combat == null)
            {
                LogDebug($"Exp Normalization: No combat data found for {AtOManager.Instance.currentMapNode}.");
                return;
            }
            NodeData node = Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode);
            if (node == null)
            {
                LogDebug($"Exp Normalization: No node data found for {AtOManager.Instance.currentMapNode}.");
                return;
            }
            int[] highestDifficulty = GetEnemyDifficulties(node.NodeCombatTier);
            int[] championDifficulty = GetChampionDifficulty(node.NodeCombatTier);
            bool isAct3 = AtOManager.Instance.GetActNumberForText() == 3;
            int[] highestExp = GetHighestExp(highestDifficulty, isAct3: isAct3);
            int championExp = GetChampionExp(championDifficulty, isAct3: isAct3);
            __result = highestExp.Sum() + championExp;
            LogDebug($"Exp Normalization Applied: Combat {node.NodeId}, Highest Difficulty {highestDifficulty}, Champion Difficulty {championDifficulty}, Total Exp {string.Join(", ", highestExp)} + {championExp} = {__result}");
            return;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Globals), "CreateGameContentRoutine")]

        public static void CreateGameContentPostfix(ref IEnumerator __result)
        {
            __result = RunAfter(__result, CreateGameContentPostfix);
        }

        public static void CreateGameContentPostfix()
        {
            LogDebug("CreateGameContentPostfix - Setting up EXP dictionaries.");
            try
            {
                Dictionary<string, NPCData> npcDictionary = Traverse.Create(Globals.Instance).Field("_NPCsSource").GetValue<Dictionary<string, NPCData>>();
                if (npcDictionary == null)
                {
                    LogError("Special test - NPC dictionary is null. Not setting difficulty EXP dictionary.");
                    return;
                }
                SetDifficultyExpDictionary(npcDictionary);
            }
            catch (Exception ex)
            {
                LogError($"Exp Normalization: Error setting difficulty EXP dictionary: {ex.ToString()}");
            }
            try
            {
                Dictionary<string, NPCData> npcDictionary = Traverse.Create(Globals.Instance).Field("_NPCsSource").GetValue<Dictionary<string, NPCData>>();
                SetAct3DifficultyExpDictionary(npcDictionary);
            }
            catch (Exception ex)
            {
                LogError($"Exp Normalization: Error setting Act3 difficulty EXP dictionary: {ex.ToString()}");
            }
            LogDebug("CreateGameContentPostfix - Done setting up EXP dictionaries.");
        }



    }
}