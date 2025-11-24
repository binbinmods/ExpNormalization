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
        public static Dictionary<int, SortedList<int, int>> Act3DifficultyExpDictionary = [];
        public static Dictionary<Enums.CombatTier, int> champTierDict = new Dictionary<Enums.CombatTier, int>
        {
            { Enums.CombatTier.T1, 2 },
            { Enums.CombatTier.T2, 4 },
            { Enums.CombatTier.T3, 6 },
            { Enums.CombatTier.T4, 8 },
            { Enums.CombatTier.T5, 10 },
            { Enums.CombatTier.T6, 6 },
            { Enums.CombatTier.T7, 8 },
            { Enums.CombatTier.T8, 15 },
            { Enums.CombatTier.T9, 16 },
            { Enums.CombatTier.T10, 10 },
            { Enums.CombatTier.T11, 12 },
            { Enums.CombatTier.T12, 12 }
        };

        public static Dictionary<Enums.CombatTier, int> npcTierDict = new Dictionary<Enums.CombatTier, int>
        {
            { Enums.CombatTier.T1, 1 },
            { Enums.CombatTier.T2, 3 },
            { Enums.CombatTier.T3, 7 },
            { Enums.CombatTier.T4, 9 },
            { Enums.CombatTier.T5, 9 },
            { Enums.CombatTier.T6, 7 },
            { Enums.CombatTier.T7, 9 },
            { Enums.CombatTier.T8, 5 },
            { Enums.CombatTier.T9, 9 },
            { Enums.CombatTier.T10, 11 },
            { Enums.CombatTier.T11, 13 },
            { Enums.CombatTier.T12, 13 }
        };

        public static bool IsRandomCombat()
        {
            return Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode).CombatPercent > 0;
        }

        public static int GetHighestEnemyDifficulty(Enums.CombatTier combatTier)
        {

            return npcTierDict[combatTier];
        }

        public static int GetChampionDifficulty(Enums.CombatTier combatTier)
        {
            return champTierDict[combatTier];
        }

        public static int[] GetHighestExp(int difficulty, bool isAct3 = false)
        {
            List<int> highestExp = [];

            Dictionary<int, SortedList<int, int>> expDictionary = isAct3 ? Act3DifficultyExpDictionary : DifficultyExpDictionary;
            if (expDictionary.TryGetValue(difficulty, out SortedList<int, int> expList))
            {
                if (expList.Count < 3)
                {
                    LogDebug($"Exp Normalization: Not enough EXP data for npc difficulty {difficulty} to determine top 3 highest EXP values.");
                    return expList.Values.ToArray();
                }
                highestExp.Add(expList[expList.Count - 1]);
                highestExp.Add(expList[expList.Count - 2]);
                highestExp.Add(expList[expList.Count - 3]);
            }
            else
            {
                LogError($"Exp Normalization: No EXP data found for npc difficulty {difficulty}.");
            }
            return highestExp.ToArray();
        }

        public static int GetChampionExp(int difficulty, bool isAct3 = false)
        {
            int championExp = 0;
            Dictionary<int, SortedList<int, int>> expDictionary = isAct3 ? Act3DifficultyExpDictionary : DifficultyExpDictionary;
            if (expDictionary.TryGetValue(difficulty, out SortedList<int, int> expList))
            {
                if (expList.Count < 1)
                {
                    LogError($"Exp Normalization: No EXP data found for champion difficulty {difficulty}.");
                    return 0;
                }
                championExp = expList.Last().Key;
            }
            else
            {
                LogError($"Exp Normalization: No EXP data found for champion difficulty {difficulty}.");
            }
            return championExp;
        }

        public static void SetDifficultyExpDictionary(Dictionary<string, NPCData> npcDictionary)
        {
            DifficultyExpDictionary.Clear();
            foreach (KeyValuePair<string, NPCData> npcEntry in npcDictionary)
            {
                NPCData npcData = npcEntry.Value;
                if (npcData == null || npcData.Difficulty == -1)
                {
                    continue;
                }
                int difficulty = npcData.Difficulty;
                if (npcData.NgPlusMob)
                {
                    npcData = npcData.NgPlusMob;
                }
                if (npcData.HellModeMob)
                {
                    npcData = npcData.HellModeMob;
                }

                int exp = npcData.ExperienceReward;
                if (DifficultyExpDictionary.TryGetValue(difficulty, out SortedList<int, int> existingList))
                {
                    existingList.Add(exp, exp);
                }
                else
                {
                    SortedList<int, int> difficultyExpList = new SortedList<int, int>
                    {
                        { exp, exp }
                    };
                    DifficultyExpDictionary[difficulty] = difficultyExpList;
                }
            }
            LogDebug($"Exp Normalization: DifficultyExpDictionary populated with {DifficultyExpDictionary.Count} difficulty levels.");
        }

        public static void SetAct3DifficultyExpDictionary(Dictionary<string, NPCData> npcDictionary)
        {
            Act3DifficultyExpDictionary.Clear();
            foreach (KeyValuePair<string, NPCData> npcEntry in npcDictionary)
            {
                NPCData npcData = npcEntry.Value;
                if (npcData == null || npcData.Difficulty == -1)
                {
                    continue;
                }
                int difficulty = npcData.Difficulty;
                if (npcData.UpgradedMob)
                {
                    npcData = npcData.UpgradedMob;
                }
                else
                {
                    continue;
                }


                if (npcData.NgPlusMob)
                {
                    npcData = npcData.NgPlusMob;
                }
                if (npcData.HellModeMob)
                {
                    npcData = npcData.HellModeMob;
                }

                int exp = npcData.ExperienceReward;
                if (Act3DifficultyExpDictionary.TryGetValue(difficulty, out SortedList<int, int> existingList))
                {
                    existingList.Add(exp, exp);
                }
                else
                {
                    SortedList<int, int> difficultyExpList = new SortedList<int, int>
                    {
                        { exp, exp }
                    };
                    Act3DifficultyExpDictionary[difficulty] = difficultyExpList;
                }
            }
            LogDebug($"Exp Normalization: Act3DifficultyExpDictionary populated with {DifficultyExpDictionary.Count} difficulty levels.");
        }



    }
}

