using UnityEngine;
using EmuTarkov.Core.Patches;
using EmuTarkov.Common.Utils.Patching;

namespace EmuTarkov.Core
{
	public class Instance : MonoBehaviour
	{
		private void Start()
		{
            Debug.LogError("EmuTarkov.Core: Loaded");

            PatcherUtil.Patch<BattleEyePatch>();
            PatcherUtil.Patch<SslCertificatePatch>();
            PatcherUtil.Patch<UnityWebRequestPatch>();
        }
	}
}
