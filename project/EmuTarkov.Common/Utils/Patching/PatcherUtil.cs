using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace EmuTarkov.Common.Utils.Patching
{
	public static class PatcherUtil
	{
		private static Harmony harmony;

		static PatcherUtil()
		{
			harmony = new Harmony("com.emutarkov.common");
		}

		public static MethodInfo GetOriginalMethod<T>(string methodName)
		{
			return AccessTools.Method(typeof(T), methodName);
		}
		
		public static void PatchPrefix<T>() where T : AbstractPatch, new()
        {
			harmony.Patch(new T().TargetMethod(), prefix: new HarmonyMethod(typeof(T).GetMethod("Prefix")));
			Debug.LogError("EmuTarkov.Common: Applied prefix patch " + typeof(T).Name);
		}

		public static void PatchPostfix<T>() where T : AbstractPatch, new()
        {
			harmony.Patch(new T().TargetMethod(), postfix: new HarmonyMethod(typeof(T).GetMethod("Postfix")));
			Debug.LogError("EmuTarkov.Common: Applied postfix patch " + typeof(T).Name);
		}

		public static void Patch<T>() where T : GenericPatch<T>, new()
		{
			var patch = new T();
			harmony.Patch(patch.TargetMethod,
						  prefix: patch.Prefix.ToHarmonyMethod(),
						  postfix: patch.Postfix.ToHarmonyMethod(),
						  transpiler: patch.Transpiler.ToHarmonyMethod(),
						  finalizer: patch.Finalizer.ToHarmonyMethod());
			Debug.LogError("EmuTarkov.Common: Applied patch " + typeof(T).Name);
		}
	}

	static class Extensions
	{
		public static HarmonyMethod ToHarmonyMethod(this MethodInfo methodInfo)
		{
			return methodInfo != null 
				? new HarmonyMethod(methodInfo)
				: null;
		}
	}
}
