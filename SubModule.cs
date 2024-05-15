using HarmonyLib;
using BastardsPlus.AddonHelpers;
using BastardsPlus.Models;
using BastardsPlus.Patches;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;


namespace BastardsPlus
{
    public class SubModule : MBSubModuleBase
    {
        private readonly Harmony _harmony = new("BastardsPlus");
        public static Random Random = new();
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new Harmony("BastardsPlus");
            harmony.PatchAll();
            SetupBastardEvents();

        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(new InformationMessage("BastardsPlus Activated"));

        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;

                campaignStarter.AddBehavior(new BastardCampaignBehavior(campaignStarter));
                campaignStarter.AddBehavior(new AIBastardConceptionCampaignBehavior());
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick); // New Aging Code
            }
        }

        private void OnDailyTick() // New Aging Code - Needs tweaking
        {
            // Fast growing rate
            ApplyGrowthRatesToAllChildren();
            float newGrowthRate = 1;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.IsKnownToPlayer && hero.Clan == Hero.MainHero.Clan && hero.Age < (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(newGrowthRate * 50));
                }
            }
        }

        private void ApplyGrowthRatesToAllChildren() // New Aging Code - Needs tweaking
        {
            float newGrowthRate = 1;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.Age < (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(newGrowthRate - 1f));
                }
            }
        }

        private void SetupBastardEvents()
        {
            BastardCampaignEvents.AddAction_OnPlayerBastardConceptionAttempt((hero) => ChangeRelationOnConceptionAttempt(hero));
            BastardCampaignEvents.AddAction_OnAIBastardConceptionAttempt((hero1, hero2) => ChangeRelationOnConceptionAttempt(hero1, hero2));
        }

        private void ChangeRelationOnConceptionAttempt(Hero hero1, Hero? hero2 = null)
        {
            if (hero2 == null)
                ChangeRelationAction.ApplyPlayerRelation(hero1, 2, false);
            else
                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, 2);
        }

    }
}