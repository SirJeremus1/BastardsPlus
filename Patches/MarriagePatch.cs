using HarmonyLib;
using SandBox.BoardGames.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem;
using BastardsPlus.StaticUtils;
using BastardsPlus.Models;

namespace BastardsPlus.Patches
{
    [HarmonyPatch(typeof(MarriageAction), nameof(MarriageAction.Apply))]
    internal class MarriagePatch
    {
        [HarmonyPrefix]
        private static void Prefix(Hero firstHero, Hero secondHero)
        {
            // FEMALE TAKES BASTARDS
            Hero? femaleHero = Utils.GetFemaleHero(firstHero, secondHero);
            if (femaleHero != null)
            {
                foreach (Hero child in femaleHero.Children)
                {
                    Bastard? bastard = Utils.GetBastardFromHero(child);
                    if (bastard != null && child.Clan == femaleHero.Clan)
                    {
                        Clan newClan = Campaign.Current.Models.MarriageModel.GetClanAfterMarriage(firstHero, secondHero);
                        child.Clan = newClan;
                    }
                }
            }

            // LEGITIZE BY MARRIAGE

            Bastard? firstBastard = Utils.GetBastardFromHero(firstHero);
            if (firstBastard != null)
                firstBastard.Legitimize();

            Bastard? secondBastard = Utils.GetBastardFromHero(secondHero);
            if (secondBastard != null)
                secondBastard.Legitimize();
        }
    }
}
