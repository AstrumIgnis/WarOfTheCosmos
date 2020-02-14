using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WarOfTheCosmos.Enums;

namespace WarOfTheCosmos.NPCs.Boss
{
    [AutoloadBossHead]
    public class Eymis : ModNPC
    {
        public const int NPC_STATE = 0;
        public const int PAUSE_TIMER = 1;

        public enum States
        {
            FloatTowards,
            DashingFirstTime,
            DashingSecondTime,
            DashingThirdTime,
            MovingToPlayer,
            CirclingAroundPlayerFirstTime,
            CirclingAroundPlayerSecondTime,
            CirclingAroundPlayerThirdTime,
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eymis");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = (int)AIStyles.CustomAI;
            npc.lifeMax = 15000;
            npc.damage = 50;
            npc.defense = 30;
            npc.knockBackResist = 0f;
            npc.width = 100;
            npc.height = 100;
            animationType = NPCID.DemonEye;
            Main.npcFrameCount[npc.type] = 2;
            npc.value = Item.buyPrice(0, 40, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.Item9;
            npc.DeathSound = SoundID.NPCDeath59;
            npc.buffImmune[24] = true;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Fall");

            npc.ai[NPC_STATE] = (int)States.FloatTowards;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.579f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.6f);
        }
        public override void AI()
        {
            switch ((States)npc.ai[NPC_STATE])
            {
                case States.FloatTowards:
                    npc.TargetClosest();
                    FloatTowardsPlayer();
                    break;
                case States.DashingFirstTime:
                    DashingFirstTime();
                    break;
                case States.DashingSecondTime:
                    break;
                case States.DashingThirdTime:
                    break;
                case States.MovingToPlayer:
                    break;
                case States.CirclingAroundPlayerFirstTime:
                    break;
                case States.CirclingAroundPlayerSecondTime:
                    break;
                case States.CirclingAroundPlayerThirdTime:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //FloatsTowardsPlayer
            //Dash at the player
            //Dash at the player
            //Dash at the player
            //Move above the player
            //Move in a circle around the player
            //While circling the player fire a project
            //While circling the player summon a minion
            //Move in a circle around the player
            //While circling the player fire a project
            //While circling the player summon a minion
            //Move in a circle around the player
            //While circling the player fire a project
            //While circling the player summon a minion
            //Start back at the top
        }

        private void FloatTowardsPlayer()
        {
            //var player = Main.player[npc.target];
            //var moveTo = player.Center; //This player is the same that was retrieved in the targeting section.

            //var speed = 1f; //make this whatever you want
            //var move = moveTo - npc.Center; //this is how much your boss wants to move
            //var magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y); //fun with the Pythagorean Theorem
            //npc.velocity = move * (speed / (float) magnitude);
            var player = Main.player[npc.target];
            var moveTo = player.Center; //This player is the same that was retrieved in the targeting section.
            float turnResistance = 100f; //the larger this is, the slower the npc will turn

            var speed = 20f;
            var move = moveTo - npc.Center;
            var magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y);

            if (magnitude > speed)
            {
                move *= (speed / (float)magnitude);
            }
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
            {
                move *= (speed / (float)magnitude);
            }
            npc.velocity = move;

            if (magnitude < 300)
            {
                npc.ai[NPC_STATE] = (int)States.DashingFirstTime;
                npc.ai[PAUSE_TIMER] = 0;
            }
        }

        private void DashingFirstTime()
        {
            npc.ai[PAUSE_TIMER]++;

            if (npc.ai[PAUSE_TIMER] <= 300)
            {
                return; //Wait for 30 ticks
            }

            var player = Main.player[npc.target];
            var moveTo = player.Center; //This player is the same that was retrieved in the targeting section.

            var speed = 30f; //Charging is fast.
            var move = moveTo - npc.Center;
            var magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y);
            move *= (speed / (float) magnitude);
            npc.velocity = move;
            //There are 60 ticks in one second, so this will make the NPC charge for 1 second before changing directions.
        }
    }
}