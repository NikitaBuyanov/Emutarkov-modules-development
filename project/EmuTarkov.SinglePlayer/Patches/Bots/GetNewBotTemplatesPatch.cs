using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Comfort.Common;
using EFT;
using EmuTarkov.Common.Utils.Patching;
using WaveInfo = GClass865;
using BotsPresets = GClass294;
using BotData = GInterface13;
using PoolManager = GClass1081;
using JobPriority = GClass584;

namespace EmuTarkov.SinglePlayer.Patches.Bots
{
    public class GetNewBotTemplatesPatch : AbstractPatch
    {
        public static FieldInfo __field;
        private static readonly Func<BotsPresets, BotData, Profile> _getNewProfileFunc;

        static GetNewBotTemplatesPatch()
        {
            // compile-time checks
            _ = nameof(WaveInfo.Difficulty);
            _ = nameof(BotsPresets.GetNewProfile);
            _ = nameof(BotData.PrepareToLoadBackend);
            _ = nameof(PoolManager.LoadBundlesAndCreatePools);
            _ = nameof(JobPriority.General);

            // creating delegate
            _getNewProfileFunc = typeof(BotsPresets)
                .GetMethod("GetNewProfile", BindingFlags.NonPublic | BindingFlags.Instance)
                .CreateDelegate(typeof(Func<BotsPresets, BotData, Profile>)) as Func<BotsPresets, BotData, Profile>;
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(BotsPresets).GetMethod("method_2", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static bool Prefix(ref Task<Profile> __result, BotsPresets __instance, BotData data)
        {
            /*
                in short when client wants new bot and GetNewProfile() return null (if not more available templates or they don't satisfied by Role and Difficulty condition)
                then client gets new piece of WaveInfo collection (with Limit = 30 by default) and make request to server
                but use only first value in response (this creates a lot of garbage and cause freezes)
                after patch we request only 1 template from server

                along with other patches this one causes to call data.PrepareToLoadBackend(1) gets the result with required role and difficulty:
                new[] { new WaveInfo() { Limit = 1, Role = role, Difficulty = difficulty } }
                then perform request to server and get only first value of resulting single element collection
            */

            var session = Utils.Config.BackEndSession;
            Task<Profile> taskAwaiter = null;
            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // try get profile from cache
            var profile = _getNewProfileFunc(__instance, data);

            if (profile == null)
            {
                // load from server
                Debug.LogError("EmuTarkov.SinglePlayer: Loading bot profile from server");
                List<WaveInfo> source = data.PrepareToLoadBackend(1).ToList();
                taskAwaiter = session.LoadBots(source).ContinueWith(GetFirstResult, taskScheduler);
            }
            else
            {
                // return cached profile
                Debug.LogError("EmuTarkov.SinglePlayer: Loading bot profile from cache");
                taskAwaiter = Task.FromResult(profile);
            }

            // load bundles for bot profile
            var continuation = new Continuation(taskScheduler);

            __result = taskAwaiter.ContinueWith(continuation.LoadBundles, taskScheduler).Unwrap();

            return false;
        }

        static Profile GetFirstResult(Task<Profile[]> task)
        {
            return task.Result[0];
        }

        struct Continuation
        {
            Profile Profile;
            TaskScheduler TaskScheduler { get; }

            public Continuation(TaskScheduler taskScheduler)
            {
                Profile = null;
                TaskScheduler = taskScheduler;
            }

            public Task<Profile> LoadBundles(Task<Profile> task)
            {
                Profile = task.Result;

                Task loadTask = Singleton<PoolManager>.Instance
                    .LoadBundlesAndCreatePools(PoolManager.PoolsCategory.Raid, 
                                               PoolManager.AssemblyType.Local, 
                                               Profile.GetAllPrefabPaths(false).ToArray(), 
                                               JobPriority.General, 
                                               null, 
                                               default(CancellationToken));

                return loadTask.ContinueWith(GetProfile, TaskScheduler);
            }

            private Profile GetProfile(Task task)
            {
                return Profile;
            }
        }
    }
}
