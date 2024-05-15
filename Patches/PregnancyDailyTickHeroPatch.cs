using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using BastardsPlus.Models;

namespace BastardsPlus.Patches
{
    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "DailyTickHero")]
    internal class PregnancyDailyTickHeroPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Hero hero)
        {
            if (!hero.IsPregnant)
                return true;

            Bastard bastard;

            try
            {
                bastard = BastardCampaignBehavior.Instance.Bastards.First(x => x.hero == null && x.mother == hero);
            }
            catch (Exception)
            {
                return true;
            }

            bastard.Tick();
            return false;
        }
    }
}
