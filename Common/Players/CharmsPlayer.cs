using HollowKnightItems.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Common.Players
{
    internal class CharmsPlayer : ModPlayer
    {
        public bool HasCarefreeMelody;
        public bool HasGrimmchild;
        public bool GrimmchildType;  // 格林之子类型，true为饰品栏，false为时装栏

        public int CarefreeOdds = 0;  // 无忧旋律生效概率        

        public override void ResetEffects()
        {
            HasCarefreeMelody = false;
            HasGrimmchild = false;
            GrimmchildType = false;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            // 判定无忧旋律
            if (HasCarefreeMelody)
            {
                int CarefreeHit = random.Next(0, 99);
                if (CarefreeHit < CarefreeOdds)
                {
                    for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 3f)
                    {
                        Vector2 position = new(Player.Center.X + (float)Math.Cos(r) * 30 - 40,
                                            Player.Center.Y + (float)Math.Sin(r) * 30 - 60);
                        Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(),
                            position,
                            new Vector2(0, 0),
                            ModContent.ProjectileType<CarefreeMelodyFire>(),
                            0,
                            0);
                    }
                    SoundEngine.PlaySound(CarefreeMelodySound);
                    Player.immune = true;
                    Player.immuneTime += 40;
                    for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                    {
                        Player.hurtCooldowns[i] += 60;
                    }
                    CarefreeOdds = 0;
                    return false;
                }
                else
                {
                    CarefreeOdds += 10;
                    return true;
                }
            }

            return true;
        }

        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor.type == NPCID.Guide)
            {
                shopInventory.Initialize();
            }
        }
    }
}
