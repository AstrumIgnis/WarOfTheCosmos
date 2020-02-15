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
        public const int COOLDOWN = 2;
        public const int LASER_TIMER = 3;

        public enum States
        {
            FloatTowards,
            DashingFirstTime,
            DashingSecondTime,
            DashingThirdTime,
            //MovingToPlayer,
            //CirclingAroundPlayerFirstTime,
            //CirclingAroundPlayerSecondTime,
            //CirclingAroundPlayerThirdTime,
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eymis");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = (int)AIStyles.CustomAI;
            npc.lifeMax = 15000;
            npc.damage = 80;
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
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
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
                    DashingSecondTime();
                    break;
                case States.DashingThirdTime:
                    DashingThirdTime();
                    break;
                //case States.MovingToPlayer:
                   //break;
               //case States.CirclingAroundPlayerFirstTime:
                    //break;
                //case States.CirclingAroundPlayerSecondTime:
                    //break;
                //case States.CirclingAroundPlayerThirdTime:
                    //break;
                default:
                    throw new ArgumentOutOfRangeException(); 
            }
            //FloatsTowardsPlayer
            //Dash at the player
            //Dash at the player
            //Dash at the player
            //Start back at the top
            //Shoot lasers the entire time
        }

        private void FloatTowardsPlayer()
        {
            //var player = Main.player[npc.target];
            //var moveTo = player.Center; //This player is the same that was retrieved in the targeting section.

            //var speed = 1f; //make this whatever you want
            //var move = moveTo - npc.Center; //this is how much your boss wants to move
            //var magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y); //fun with the Pythagorean Theorem
            //npc.velocity = move * (speed / (float) magnitude);
            npc.ai[COOLDOWN]++;
            var player = Main.player[npc.target];
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
            }
            npc.netUpdate = true;
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

            //lasers
            npc.ai[LASER_TIMER]++;
            if (npc.ai[LASER_TIMER] >= 30)
            {
                float Speed = 30f;
                Vector2 vector8 = new Vector2(npc.position.X + (npc.width / 2), npc.position.Y + (npc.height / 2));
                int damage = 30;
                int type = mod.ProjectileType("Elementdisc");
                Main.PlaySound(SoundID.Item60, (int)npc.position.X, (int)npc.position.Y);
                float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                int num54 = Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                npc.ai[LASER_TIMER] = 0;
            }

            if ((magnitude < 50) && (npc.ai[COOLDOWN] > 180)) //180 wait time
            {
                npc.ai[NPC_STATE] = (int)States.DashingFirstTime;
                npc.ai[PAUSE_TIMER] = 30;
            }
        }

        private void DashingFirstTime()  
        {
            npc.ai[PAUSE_TIMER]++;

            if (npc.ai[PAUSE_TIMER] <= 45)
            {
                return; //Wait for 45 ticks
            }

            var player = Main.player[npc.target];
            var moveTo = player.Center; //This player is the same that was retrieved in the targeting section.

            var speed = 30f; //Charging is fast.
            var move = moveTo - npc.Center;
            var magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y);
            move *= (speed / (float) magnitude);
            npc.velocity = move;
            npc.ai[PAUSE_TIMER] = 0;
            npc.ai[NPC_STATE] = (int)States.DashingSecondTime;
            //There are 60 ticks in one second, so this will make the NPC charge for 1 second before changing directions.
        }

        private void DashingSecondTime()
        {
            npc.ai[PAUSE_TIMER]++;

            if (npc.ai[PAUSE_TIMER] <= 45)
            {
                return; //Wait for 45 ticks
            }

            var player = Main.player[npc.target];
            var moveTo = player.Center; //This player is the same that was retrieved in the targeting section.

            var speed = 30f; //Charging is fast.
            var move = moveTo - npc.Center;
            var magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y);
            move *= (speed / (float)magnitude);
            npc.velocity = move;
            npc.ai[PAUSE_TIMER] = 0;
            npc.ai[NPC_STATE] = (int)States.DashingThirdTime;
            //There are 60 ticks in one second, so this will make the NPC charge for 1 second before changing directions.
        }

        private void DashingThirdTime()
        {
            npc.ai[PAUSE_TIMER]++;

            if (npc.ai[PAUSE_TIMER] <= 45)
            {
                return; //Wait for 45 ticks
            }

            var player = Main.player[npc.target];
            var moveTo = player.Center; //This player is the same that was retrieved in the targeting section.

            var speed = 30f; //Charging is fast.
            var move = moveTo - npc.Center;
            var magnitude = Math.Sqrt(move.X * move.X + move.Y * move.Y);
            move *= (speed / (float)magnitude);
            npc.velocity = move;
            npc.ai[PAUSE_TIMER] = 0;
            npc.ai[COOLDOWN] = 0;
            npc.ai[NPC_STATE] = (int)States.FloatTowards;
        }
    }
}