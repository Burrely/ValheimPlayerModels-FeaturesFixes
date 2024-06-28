﻿#if PLUGIN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ValheimPlayerModels
{
	[HarmonyPatch(typeof(Player), "Awake")]
	static class Patch_Player_Awake
	{
		[HarmonyPostfix]
		static void Postfix(Player __instance)
        {
            if(PluginConfig.enablePlayerModels.Value)
                __instance.gameObject.AddComponent<PlayerModel>();
        }
	}

    [HarmonyPatch(typeof(VisEquipment), "UpdateLodgroup")]
    static class Patch_VisEquipment_UpdateLodgroup
    {
        [HarmonyPostfix]
        static void Postfix(VisEquipment __instance)
        {
            if (PluginConfig.enablePlayerModels.Value)
                __instance.GetComponent<PlayerModel>()?.ToggleEquipments();
        }
    }

    [HarmonyPatch(typeof(Ragdoll), "Start")]
    static class Patch_Ragdoll_Start
    {
        [HarmonyPostfix]
        static void Postfix(Ragdoll __instance)
        {
            if (PluginConfig.enableCustomRagdoll.Value)
            {
                if (__instance.gameObject.name.StartsWith("Player"))
                {
                    if (ZNet.instance)
                    {
                        PlayerModel[] playerModels = Object.FindObjectsOfType<PlayerModel>();
                        PlayerModel player = playerModels.FirstOrDefault(p =>
                            p.player.GetZDOID().UserID == __instance.m_nview.m_zdo.m_uid.UserID);

                        if (player) player.SetupRagdoll(__instance);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Terminal), "TryRunCommand")]
    static class Patch_Terminal_TryRunCommand
    {
        [HarmonyPostfix]
        static void Postfix(Terminal __instance, string text, bool silentFail = false, bool skipAllowedCheck = false)
        {
            string command = text.ToLower();
            string[] param = command.Split(' ');
            if (command.StartsWith("anim") && param.Length == 3)
            {
                if (PlayerModel.localModel)
                {
                    if (bool.TryParse(param[2], out bool valueBool))
                        if (PlayerModel.localModel.avatar.SetBool(param[1], valueBool)) return;

                    if (int.TryParse(param[2], out int valueInt))
                        if (PlayerModel.localModel.avatar.SetInt(param[1], valueInt)) return;

                    if (float.TryParse(param[2], out float valuefloat))
                        PlayerModel.localModel.avatar.SetFloat(param[1], valuefloat);
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameCamera), "UpdateMouseCapture")]
    static class Patch_GameCamera_UpdateMouseCapture
    {
        [HarmonyPrefix]
        static bool Prefix(GameCamera __instance)
        {
            if (PluginConfig.enablePlayerModels.Value && (Plugin.showActionMenu || Plugin.showAvatarMenu))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(Player), "SetMouseLook")]
    static class Patch_Player_SetMouseLook
    {
        [HarmonyPrefix]
        static bool Prefix(Player __instance)
        {
            if (PluginConfig.enablePlayerModels.Value && (Plugin.showActionMenu || Plugin.showAvatarMenu))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(Character), "SetVisible")]
    static class Patch_Character_SetVisible
    {
        [HarmonyPostfix]
        static void Postfix(Character __instance, bool visible)
        {
            if (!__instance.IsPlayer()) return;

            PlayerModel playerModel;

            if (!Plugin.playerModelCharacters.ContainsKey(__instance))
            {
                playerModel = __instance.GetVisual().transform.parent.GetComponent<PlayerModel>();
                Plugin.playerModelCharacters.Add(__instance, playerModel);
            }
            else
            {
                playerModel = Plugin.playerModelCharacters[__instance];
            }

            if (!playerModel || playerModel.avatar == null) return;

            var lodGroup = playerModel.avatar.lodGroup;
            if (!lodGroup) return;

            if (visible)
            {
                lodGroup.localReferencePoint = __instance.m_originalLocalRef;
            }
            else
            {
                lodGroup.localReferencePoint = new Vector3(999999f, 999999f, 999999f);
            }
        }
    }

    [HarmonyPatch(typeof(EntryPointSceneLoader), nameof(EntryPointSceneLoader.Start))]
    static class Patch_All_SoftReferenceableAssets {
        [HarmonyPrefix]
        static void Prefix() {
            SoftReferenceableAssets.Runtime.MakeAllAssetsLoadable();
        }
    }
}
#endif