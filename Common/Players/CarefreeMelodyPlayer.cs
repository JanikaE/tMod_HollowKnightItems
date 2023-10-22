using HollowKnightItems.Assets;
using HollowKnightItems.Content.Projectiles;

namespace HollowKnightItems.Common.Players
{
    internal class CarefreeMelodyPlayer : ModPlayer
    {
        public bool HasCarefreeMelody;
        public int CarefreeOdds;  // 无忧旋律生效概率        

        public override void ResetEffects()
        {
            HasCarefreeMelody = false;
        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            // 判定无忧旋律
            if (HasCarefreeMelody && Player == Main.LocalPlayer)
            {
                int CarefreeHit = random.Next(0, 100);
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
                                                0,
                                                Player.whoAmI);
                    }
                    SoundEngine.PlaySound(SoundLoader.CarefreeMelody);
                    Player.immune = true;
                    Player.immuneTime += 40;
                    for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                    {
                        Player.hurtCooldowns[i] += 60;
                    }
                    CarefreeOdds = 0;
                    return true;
                }
                else
                {
                    CarefreeOdds += 10;
                    return false;
                }
            }
            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["CarefreeOdds"] = CarefreeOdds;
        }

        public override void LoadData(TagCompound tag)
        {
            CarefreeOdds = (int)tag["CarefreeOdds"];
        }
    }
}
