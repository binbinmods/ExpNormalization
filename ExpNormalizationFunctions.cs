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
        public static Dictionary<Enums.CombatTier, int[]> champTierDict = new Dictionary<Enums.CombatTier, int[]>
        {
            { Enums.CombatTier.T0, [2] },
            { Enums.CombatTier.T1, [2] },
            { Enums.CombatTier.T2, [4] },
            { Enums.CombatTier.T3, [6] },
            { Enums.CombatTier.T4, [8] },
            { Enums.CombatTier.T5, [10] },
            { Enums.CombatTier.T6, [6] },
            { Enums.CombatTier.T7, [8] },
            { Enums.CombatTier.T8, [15] },
            { Enums.CombatTier.T9, [16] },
            { Enums.CombatTier.T10, [10] },
            { Enums.CombatTier.T11, [12] },
            { Enums.CombatTier.T12, [12] }
        };

        public static Dictionary<Enums.CombatTier, int[]> npcTierDict = new Dictionary<Enums.CombatTier, int[]>
        {
            { Enums.CombatTier.T0, [1] },
            { Enums.CombatTier.T1, [1] },
            { Enums.CombatTier.T2, [3] },
            { Enums.CombatTier.T3, [5] },
            { Enums.CombatTier.T4, [9] },
            { Enums.CombatTier.T5, [9] },
            { Enums.CombatTier.T6, [5] },
            { Enums.CombatTier.T7, [9] },
            { Enums.CombatTier.T8, [5] },
            { Enums.CombatTier.T9, [9] },
            { Enums.CombatTier.T10, [11] },
            { Enums.CombatTier.T11, [13] },
            { Enums.CombatTier.T12, [13] }
        };

        public static bool IsRandomCombat()
        {
            NodeData node = Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode);
            bool isRC = (node.CombatPercent > 0 || node.NodeCombat.Length > 0) && !node.DisableRandom;
            if (node.NodeId == "dream_2")
            {

                LogDebug($"Exp Normalization: NodeCombat for node {AtOManager.Instance.currentMapNode} is dream_2. Not a random combat.");
                return false;
            }
            if (!isRC)
            {
                LogDebug($"Exp Normalization: NodeCombat for node {AtOManager.Instance.currentMapNode} is {node.NodeCombat.Length}. Not a random combat.");
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
                int diff = difficulty[i];
                if (expDictionary.TryGetValue(diff, out List<int> expList))
                {
                    if (expList.Count < 1)
                    {
                        LogDebug($"Exp Normalization: Not enough EXP data for npc difficulty {diff} to determine highest EXP value.");
                        continue;
                    }
                    totalList.AddRange(expList);
                }
                else
                {
                    LogError($"Exp Normalization: No EXP data found for npc difficulty {diff}.");
                }
            }
            totalList.Sort();
            if (totalList.Count < 3)
            {
                LogDebug($"Exp Normalization: No EXP data found for npc difficulties {string.Join(", ", difficulty)}.");
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
            Dictionary<int, List<int>> expDictionary = isAct3 ? Act3DifficultyExpDictionary : DifficultyExpDictionary;
            List<int> totalList = [];
            for (int i = 0; i < difficulty.Length; i++)
            {
                int diff = difficulty[i];
                if (expDictionary.TryGetValue(diff, out List<int> expList))
                {
                    if (expList.Count < 1)
                    {
                        LogDebug($"Exp Normalization: Not enough EXP data for npc difficulty {diff} to determine highest EXP value.");
                        continue;
                    }
                    totalList.AddRange(expList);
                }
                else
                {
                    LogError($"Exp Normalization: No EXP data found for npc difficulty {diff}.");
                }
            }
            totalList.Sort();
            if (totalList.Count < 3)
            {
                LogDebug($"Exp Normalization: No EXP data found for npc difficulties {string.Join(", ", difficulty)}.");
                return championExp;
            }
            return totalList.Last();
        }

        public static void SetDifficultyExpDictionary(Dictionary<string, NPCData> npcDictionary)
        {
            LogDebug("Exp Normalization: Populating DifficultyExpDictionary...");
            DifficultyExpDictionary.Clear();
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
                if (DifficultyExpDictionary.TryGetValue(difficulty, out List<int> existingList))
                {
                    existingList.Add(exp);
                    existingList.Sort();
                }
                else
                {
                    List<int> difficultyExpList = [exp];
                    DifficultyExpDictionary[difficulty] = difficultyExpList;
                }
            }
            LogDebug($"Exp Normalization: DifficultyExpDictionary populated with {DifficultyExpDictionary.Count} difficulty levels.");
        }

        public static void SetAct3DifficultyExpDictionary(Dictionary<string, NPCData> npcDictionary)
        {
            LogDebug("Exp Normalization: Populating Act3DifficultyExpDictionary...");
            Act3DifficultyExpDictionary.Clear();
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
                if (Act3DifficultyExpDictionary.TryGetValue(difficulty, out List<int> existingList))
                {
                    existingList.Add(exp);
                    existingList.Sort();
                }
                else
                {
                    List<int> difficultyExpList = [exp];
                    Act3DifficultyExpDictionary[difficulty] = difficultyExpList;
                }
            }
            LogDebug($"Exp Normalization: Act3DifficultyExpDictionary populated with {DifficultyExpDictionary.Count} difficulty levels.");
        }



    }
}

