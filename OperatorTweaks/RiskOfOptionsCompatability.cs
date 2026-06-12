using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OperatorTweaks
{
    internal class RiskOfOptionsCompatability
    {
        private static bool? _enabled;
        public static bool Enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                }
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void InitConfig()
        {
            ModSettingsManager.AddOption(new IntSliderOption(OperatorTweaksSettings.ShieldTransparency, new IntSliderConfig
            {
                min = 0,
                max = 200,
                restartRequired = true
            }));
            ModSettingsManager.AddOption(new StepSliderOption(OperatorTweaksSettings.ShieldExitTime, new StepSliderConfig
            {
                min = 0f,
                max = 1f,
                increment = 0.05f,
                restartRequired = true
            }));
            ModSettingsManager.AddOption(new StepSliderOption(OperatorTweaksSettings.ShieldDurationForMaxCharge, new StepSliderConfig
            {
                min = 0f,
                max = 4f,
                increment = 0.1f,
                restartRequired = true
            }));
            ModSettingsManager.AddOption(new CheckBoxOption(OperatorTweaksSettings.LeapUseMovementDirection, new CheckBoxConfig
            {
                restartRequired = true
            }));
        }
    }
}
