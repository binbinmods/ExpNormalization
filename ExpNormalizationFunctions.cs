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

        public static Dictionary<int, List<int>> DifficultyExpDictionary = [];
        public static Dictionary<int, List<int>> Act3DifficultyExpDictionary = [];

        public static Dictionary<int, List<int>> ChampionDifficultyExpDictionary = [];
        public static Dictionary<int, List<int>> Act3ChampionDifficultyExpDictionary = [];
        public static Dictionary<Enums.CombatTier, int[]> champTierDict = new Dictionary<Enums.CombatTier, int[]>
        {
            { Enums.CombatTier.T0, [2] },
            { Enums.CombatTier.T1, [2] },
            { Enums.CombatTier.T2, [4] },
            { Enums.CombatTier.T3, [6] },
            { Enums.CombatTier.T4, [8] },
            { Enums.CombatTier.T5, [10] },
            { Enums.CombatTier.T6, [6, 8] },
            { Enums.CombatTier.T7, [8, 10] },
            { Enums.CombatTier.T8, [15] },
            { Enums.CombatTier.T9, [16] },
            { Enums.CombatTier.T10, [10, 12] },
            { Enums.CombatTier.T11, [12] },
            { Enums.CombatTier.T12, [12] }
        };

        public static Dictionary<Enums.CombatTier, int[]> npcTierDict = new Dictionary<Enums.CombatTier, int[]>
        {
            { Enums.CombatTier.T0, [1] },
            { Enums.CombatTier.T1, [1] },
            { Enums.CombatTier.T2, [3] },
            { Enums.CombatTier.T3, [5, 7] },
            { Enums.CombatTier.T4, [5, 7, 9] },
            { Enums.CombatTier.T5, [7, 9] },
            { Enums.CombatTier.T6, [5, 7] },
            { Enums.CombatTier.T7, [7, 9] },
            { Enums.CombatTier.T8, [3, 5] },
            { Enums.CombatTier.T9, [7, 9] },
            { Enums.CombatTier.T10, [9, 11] },
            { Enums.CombatTier.T11, [11, 13] },
            { Enums.CombatTier.T12, [11, 13] }
        };

        public static bool IsRandomCombat()
        {
            bool isRC = false;
            try
            {
                NodeData node = Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode);
                isRC = (node.CombatPercent > 0 || node.NodeCombat.Length > 0) && !node.DisableRandom;
                if (node.NodeId == "dream_2")
                {

                    LogDebug($"Exp Normalization: NodeCombat for node {AtOManager.Instance.currentMapNode} is dream_2. Not a random combat.");
                    return false;
                }
                if (!isRC)
                {
                    LogDebug($"Exp Normalization: NodeCombat for node {AtOManager.Instance.currentMapNode} is {node.NodeCombat.Length}. Not a random combat.");
                }
            }
            catch (Exception ex)
            {
                LogError($" Error getting random combat: {ex.ToString()}");
            }
            return isRC;
        }

        public static int[] GetEnemyDifficulties(Enums.CombatTier combatTier)
        {

            return npcTierDict[combatTier];
        }

        public static int[] GetChampionDifficulty(Enums.CombatTier combatTier)
        {
            return champTierDict[combatTier];
        }

        public static int[] GetHighestExp(int[] difficulty, bool isAct3 = false)
        {
            List<int> highestExp = [];

            Dictionary<int, List<int>> expDictionary = isAct3 ? Act3DifficultyExpDictionary : DifficultyExpDictionary;
            List<int> totalList = [];
            for (int i = 0; i < difficulty.Length; i++)
            {
                try
                {
                    int diff = difficulty[i];
                    if (expDictionary.TryGetValue(diff, out List<int> expList))
                    {
                        if (expList.Count < 1)
                        {
                            LogDebug($"Not enough EXP data for npc difficulty {diff} to determine highest EXP value.");
                            continue;
                        }
                        totalList.AddRange(expList);
                    }
                    else
                    {
                        LogError($"No EXP data found for npc difficulty {diff}.");
                    }
                }
                catch (Exception ex)
                {
                    LogError($" Error getting highest EXP: {ex.ToString()}");
                }
            }
            totalList.Sort();
            if (totalList.Count < 3)
            {
                LogError($"Insufficient EXP data found for npc difficulties {string.Join(", ", difficulty)}.");
                return [.. highestExp];
            }
            highestExp.Add(totalList[^1]);
            highestExp.Add(totalList[^2]);
            highestExp.Add(totalList[^3]);
            LogDebug($"Acquired Highest Experience for Difficulty {difficulty}: {string.Join(", ", highestExp)}");
            return [.. highestExp];
        }

        public static int GetChampionExp(int[] difficulty, bool isAct3 = false)
        {
            int championExp = 0;
            Dictionary<int, List<int>> expDictionary = isAct3 ? Act3ChampionDifficultyExpDictionary : ChampionDifficultyExpDictionary;
            List<int> totalList = [];
            for (int i = 0; i < difficulty.Length; i++)
            {
                try
                {
                    int diff = difficulty[i];
                    if (expDictionary.TryGetValue(diff, out List<int> expList))
                    {
                        if (expList.Count < 1)
                        {
                            LogDebug($"Not enough EXP data for npc difficulty {diff} to determine highest EXP value.");
                            continue;
                        }
                        totalList.AddRange(expList);
                    }
                    else
                    {
                        LogError($"No EXP data found for npc difficulty {diff}.");
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error getting champion EXP: {ex.ToString()}");
                }
            }
            totalList.Sort();
            if (totalList.Count < 3)
            {
                LogDebug($"No EXP data found for npc difficulties {string.Join(", ", difficulty)}.");
                return championExp;
            }
            return totalList.Last();
        }

        public static void SetDifficultyExpDictionary(Dictionary<string, NPCData> npcDictionary, bool isChampion = false)
        {
            LogDebug("Exp Normalization: Populating DifficultyExpDictionary...");
            if (npcDictionary == null || DifficultyExpDictionary == null)
            {
                LogError("NPC dictionary or DifficultyExpDictionary is null. Not populating DifficultyExpDictionary.");
                return;
            }
            if (isChampion)
            {
                ChampionDifficultyExpDictionary.Clear();
            }
            else
            {
                DifficultyExpDictionary.Clear();
            }

            foreach (KeyValuePair<string, NPCData> npcEntry in npcDictionary)
            {
                NPCData npcData = npcEntry.Value;
                if (npcData == null || npcData.Difficulty == -1)
                {
                    continue;
                }
                int difficulty = npcData.Difficulty;
                if (npcData.NgPlusMob != null)
                {
                    npcData = npcData.NgPlusMob;
                }
                if (npcData.HellModeMob != null)
                {
                    npcData = npcData.HellModeMob;
                }

                int exp = npcData.ExperienceReward;
                if ((isChampion ? ChampionDifficultyExpDictionary : DifficultyExpDictionary).TryGetValue(difficulty, out List<int> existingList))
                {
                    existingList.Add(exp);
                    existingList.Sort();
                }
                else
                {
                    List<int> difficultyExpList = [exp];
                    (isChampion ? ChampionDifficultyExpDictionary : DifficultyExpDictionary)[difficulty] = difficultyExpList;
                }
            }
            LogDebug($"Exp Normalization: DifficultyExpDictionary populated with {(isChampion ? ChampionDifficultyExpDictionary : DifficultyExpDictionary).Count} difficulty levels.");
        }

        public static void SetAct3DifficultyExpDictionary(Dictionary<string, NPCData> npcDictionary, bool isChampion = false)
        {
            LogDebug("Exp Normalization: Populating Act3DifficultyExpDictionary...");
            if (npcDictionary == null || Act3DifficultyExpDictionary == null)
            {
                LogError("NPC dictionary or Act3DifficultyExpDictionary is null. Not populating Act3DifficultyExpDictionary.");
                return;
            }
            if (isChampion)
            {
                Act3ChampionDifficultyExpDictionary.Clear();
            }
            else
            {
                Act3DifficultyExpDictionary.Clear();
            }

            foreach (KeyValuePair<string, NPCData> npcEntry in npcDictionary)
            {
                NPCData npcData = npcEntry.Value;
                if (npcData == null || npcData.Difficulty == -1)
                {
                    continue;
                }
                int difficulty = npcData.Difficulty;
                if (npcData.UpgradedMob != null)
                {
                    npcData = npcData.UpgradedMob;
                }
                if (npcData.NgPlusMob != null)
                {
                    npcData = npcData.NgPlusMob;
                }
                if (npcData.HellModeMob != null)
                {
                    npcData = npcData.HellModeMob;
                }
                int exp = npcData.ExperienceReward;
                if ((isChampion ? Act3ChampionDifficultyExpDictionary : Act3DifficultyExpDictionary).TryGetValue(difficulty, out List<int> existingList))
                {
                    existingList.Add(exp);
                    existingList.Sort();
                }
                else
                {
                    List<int> difficultyExpList = [exp];
                    (isChampion ? Act3ChampionDifficultyExpDictionary : Act3DifficultyExpDictionary)[difficulty] = difficultyExpList;
                }
            }
            LogDebug($"Exp Normalization: Act3DifficultyExpDictionary populated with {(isChampion ? Act3ChampionDifficultyExpDictionary : Act3DifficultyExpDictionary).Count} difficulty levels.");
        }



    }
}

