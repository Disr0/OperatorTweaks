using BepInEx;
using BepInEx.Configuration;
using HG.GeneralSerializer;
using RoR2;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OperatorTweaks
{
    public static class OperatorTweaksSettings
    {
        public static ConfigEntry<int> ShieldTransparency { get; set; }
        public static ConfigEntry<float> ShieldExitTime { get; set; }
        public static ConfigEntry<float> ShieldDurationForMaxCharge { get; set; }
    }

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    public class OperatorTweaks : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "disro";
        public const string PluginName = "OperatorTweaks";
        public const string PluginVersion = "1.0.0";

        private EntityStateConfiguration _shieldAsset;
        public void Awake()
        {
            Log.Init(Logger);

            OperatorTweaksSettings.ShieldTransparency = Config.Bind("Shield", "Shield Transparency", 100, "The percent of the original shield transparency");
            OperatorTweaksSettings.ShieldDurationForMaxCharge = Config.Bind("Shield", "Shield Duration For Max Charge", 2f, "Seconds until the shield is fully charged up");
            OperatorTweaksSettings.ShieldExitTime = Config.Bind("Shield", "Shield exit time", 0.1f, "Delay before the shield is sent flying after key release. Set to 0 for no delay");
            if (RiskOfOptionsCompatability.Enabled)
            {
                RiskOfOptionsCompatability.InitConfig();
            }

            _shieldAsset =
                Addressables.LoadAssetAsync<EntityStateConfiguration>(
                    "RoR2/DLC3/Drone Tech/EntityStates.DroneTech.Weapon.ShieldFormation.asset"
                ).WaitForCompletion();
            ApplyShieldModifications(_shieldAsset);
        }

        private void ApplyShieldModifications(EntityStateConfiguration shieldAsset)
        {
            if (!shieldAsset)
            {
                Log.Error("Shield asset missing");
                return;
            }

            float transparencyMultiplier = OperatorTweaksSettings.ShieldTransparency.Value / 100f;
            float exitTime = OperatorTweaksSettings.ShieldExitTime.Value;
            float durationForMaxCharge = OperatorTweaksSettings.ShieldDurationForMaxCharge.Value;

            var fields = shieldAsset.serializedFieldsCollection.serializedFields;
            for (int i = 0; i < fields.Length; i++)
            {
                switch (fields[i].fieldName)
                {
                    case "color25":
                    case "color50":
                    case "color75":
                    case "color100":
                    case "defaultColor":
                        Color color = (Color)StringSerializer.Deserialize(typeof(Color), fields[i].fieldValue.stringValue);
                        color.a = color.a * transparencyMultiplier;
                        fields[i].fieldValue.stringValue = StringSerializer.Serialize(typeof(Color), color);
                        break;
                    case "exitTime":
                        Log.Debug($"exitTime before: {fields[i].fieldValue.stringValue}");
                        fields[i].fieldValue.stringValue = StringSerializer.Serialize(typeof(float), exitTime);
                        Log.Debug($"exitTime now: {fields[i].fieldValue.stringValue}");
                        break;
                    case "durationForMaxCharge":
                        Log.Debug($"durationForMaxCharge before: {fields[i].fieldValue.stringValue}");
                        fields[i].fieldValue.stringValue = StringSerializer.Serialize(typeof(float), durationForMaxCharge);
                        Log.Debug($"durationForMaxCharge now: {fields[i].fieldValue.stringValue}");
                        break;
                }
            }
        }
    }
}
