using UnityEngine;
using System.Collections.Generic;
using RoboClean.Player;

namespace RoboClean.Data
{
    public static class RuntimeDataLoader
    {
        public static RuntimePlayData RuntimePlayData
        {
            get; private set;
        }

        public static RuntimeStageData RuntimeStageData
        {
            get; private set;
        }

        public static RuntimeUIData RuntimeUIData
        {
            get; private set;
        }

        public static PlayerConfig PlayerConfig
        {
            get; private set;
        }

        public static RuntimePlayerInfo PlayerRuntimeInfo
        {
            get; private set;
        }

        public static SpawnConfig SpawnConfig
        {
            get; private set;
        }

        public static CameraConfig CameraConfig
        {
            get; private set;
        }

        public static Dictionary<string, SkillConfig[]> ComboSkillConfigs
        {
            get; private set;
        }

        public static Dictionary<string, MonsterConfig> MonsterConfigDic
        {
            get; private set;
        }

        public static Dictionary<eDialogueTag, DialogueConfig> DialogueConfigDic
        {
            get; private set;
        }

        private static bool mIsLoaded = false;

        static RuntimeDataLoader()
        {
            LoadData();
        }

        public static void LoadData()
        {
            if (mIsLoaded)
            {
                return;
            }

            RuntimePlayData = Resources.Load<RuntimePlayData>("RuntimePlayData");
            RuntimeStageData = Resources.Load<RuntimeStageData>("RuntimeStageData");
            RuntimeUIData = Resources.Load<RuntimeUIData>("RuntimeUIData");
            PlayerConfig = Resources.Load<PlayerConfig>("Player/PlayerConfig");
            PlayerRuntimeInfo = Resources.Load<RuntimePlayerInfo>("Player/_Runtime/PlayerRuntimeInfo");
            Debug.Assert(PlayerConfig != null, "Cannot Found Player Config");
            SpawnConfig = Resources.Load<SpawnConfig>("Spawners/SpawnConfig");
            Debug.Assert(SpawnConfig != null, "Cannot Found Spawn Config");
            CameraConfig = Resources.Load<CameraConfig>("Camera/CameraConfig");
            Debug.Assert(CameraConfig != null, "Cannot Found Camera Config");


            DialogueConfig[] configs = Resources.LoadAll<DialogueConfig>("Dialogue");
            DialogueConfigDic = new Dictionary<eDialogueTag, DialogueConfig>();
            for (int i = 0; i < configs.Length; i++)
            {
                Debug.Assert(!DialogueConfigDic.ContainsKey(configs[i].Tag),
                    $"Duplciated DialogueConfig tag detected [{configs[i].Tag}]");
                DialogueConfigDic.Add(configs[i].Tag, configs[i]);
            }
            LoadAndBindingComoboSkillConfigs();
            MonsterConfigDic = LoadDatas<MonsterConfig>("Monsters");
            mIsLoaded = true;
        }

        public static Dictionary<string, T> LoadDatas<T>(string dataPath) where T : DataConfigBase
        {
            var dataDics = new Dictionary<string, T>();

            DataConfigBase[] datas = Resources.LoadAll<T>(dataPath);

            for (int i = 0; i < datas.Length; i++)
            {
                dataDics.Add(datas[i].name, datas[i] as T);
            }

            return dataDics;
        }

        public static void LoadAndBindingComoboSkillConfigs()
        {
            SkillConfig[] datas = Resources.LoadAll<SkillConfig>("");

            ComboSkillConfigs = new Dictionary<string, SkillConfig[]>();
            for (int i = 0; i < datas.Length; i++)
            {
                SkillConfig skillConfig = datas[i];
                skillConfig.InitializeInRuntime();
                if (skillConfig.IsComboAttack)
                {
                    if (!ComboSkillConfigs.ContainsKey(skillConfig.ComboSkillName))
                    {
                        ComboSkillConfigs.Add(skillConfig.ComboSkillName, new SkillConfig[skillConfig.TotalComboCount]);
                    }

                    ComboSkillConfigs[skillConfig.ComboSkillName][skillConfig.ComboIndex] = skillConfig;
                }
            }

#if UNITY_EDITOR
            foreach (var item in ComboSkillConfigs)
            {
                for (int i = 0; i < item.Value.Length; i++)
                {
                    if (item.Value[i] == null)
                    {
                        Debug.LogError
                            ($"Validate Combo skill config fail null ComboSkill dected" +
                            $"Config Name [{item.Key}] , Index [{i}]");
                    }
                }
            }
#endif
        }
    }
}
