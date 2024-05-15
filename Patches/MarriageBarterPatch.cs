using HarmonyLib;
using SandBox.BoardGames.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem;
using BastardsPlus.StaticUtils;
using BastardsPlus.Models;

namespace BastardsPlus.Patches
{
    [HarmonyPatch(typeof(MarriageBarterable), nameof(MarriageBarterable.GetUnitValueForFaction))]
    internal class MarriageBarterPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ref int __result, MarriageBarterable __instance)
        {
            Hero? heroBeingProposedTo = Traverse.Create(__instance).Field("HeroBeingProposedTo").GetValue() as Hero;
            Hero? proposingHero = Traverse.Create(__instance).Field("ProposingHero").GetValue() as Hero;

            if (heroBeingProposedTo == null || proposingHero == null)
                return;

            int modifierValue = (int)Math.Round((double)Math.Abs(__result) * 1);

            Bastard? bastardBeingProposedTo = Utils.GetBastardFromHero(heroBeingProposedTo);
            if (bastardBeingProposedTo != null)
                __result -= modifierValue;

            Bastard? proposingBastard = Utils.GetBastardFromHero(proposingHero);
            if (proposingBastard != null)
                __result += modifierValue;
        }
    }
}
