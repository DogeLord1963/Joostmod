using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoostMod.Projectiles
{
    public class IceBeamCharged : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Beam");
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
            //projectile.light = 1.5f;
            aiType = ProjectileID.Bullet;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.coldDamage = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects effects = SpriteEffects.None;
            Color color = Color.White;
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(new Rectangle(0, 0, tex.Width, tex.Height)), color, projectile.rotation, new Vector2(tex.Width / 2, tex.Height / 2), projectile.scale, effects, 0f);
            return false;
        }
        public override void OnHitNPC(NPC n, int damage, float knockback, bool crit)
        {
            Player owner = Main.player[projectile.owner];
            n.AddBuff(44, 600);

        }
        public override void AI()
        {
            if (projectile.ai[0] != 0)
            {
                if (projectile.localAI[0] == 0)
                {
                    projectile.localAI[0] = projectile.position.X;
                }
                if (projectile.localAI[1] == 0)
                {
                    projectile.localAI[1] = projectile.position.Y;
                }
                float freq = 0.15f;
                float mag = 40f;
                int time = 600 - projectile.timeLeft;
                Vector2 pos = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                Vector2 dir = projectile.velocity;
                dir.Normalize();
                Vector2 axis = dir.RotatedBy(90 * projectile.ai[0] * 0.0174f);
                Vector2 wave = axis * (float)Math.Sin(time * freq) * mag;
                projectile.position = pos + wave;
                projectile.localAI[0] = projectile.position.X - wave.X + projectile.velocity.X;
                projectile.localAI[1] = projectile.position.Y - wave.Y + projectile.velocity.Y;
            }
        }
    }
}

