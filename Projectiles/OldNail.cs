using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoostMod.Projectiles
{
    public class OldNail : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Old Nail");
            Main.projFrames[projectile.type] = 9;
		}
        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.ownerHitCheck = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -1;
        }

        public override bool PreAI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            bool channeling = player.itemAnimation > 0 && !player.noItems && !player.CCed;
            /*if(channeling)
            {
                float scaleFactor6 = 1f;
                if (player.inventory[player.selectedItem].shoot == projectile.type)
                {
                    scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                }
                Vector2 vector13 = Main.MouseWorld - vector;
                vector13.Normalize();
                if (vector13.HasNaNs())
                {
                    vector13 = Vector2.UnitX * (float)player.direction;
                }
                vector13 *= scaleFactor6;
                if (vector13.X != projectile.velocity.X || vector13.Y != projectile.velocity.Y)
                {
                    projectile.netUpdate = true;
                }
                projectile.velocity = vector13;
            }
            else
            {
                projectile.Kill();
            }*/
            if (!channeling)
            {
                projectile.Kill();
            } 
			if (player.itemAnimation > (int)(8*(float)player.itemAnimationMax/9))
			{
				projectile.frame = 0;
			}
            else if (player.itemAnimation > (int)(7*(float)player.itemAnimationMax/9))
			{
				projectile.frame = 1;
			}
            else if (player.itemAnimation > (int)(6*(float)player.itemAnimationMax/9))
			{
				projectile.frame = 2;
			}
            else if (player.itemAnimation > (int)(5*(float)player.itemAnimationMax/9))
			{
				projectile.frame = 3;
			}
            else if (player.itemAnimation > (int)(4*(float)player.itemAnimationMax/9))
			{
				projectile.frame = 4;
			}
            else if (player.itemAnimation > (int)(3*(float)player.itemAnimationMax/9))
			{
				projectile.frame = 5;
			}
            else if (player.itemAnimation > (int)(2*(float)player.itemAnimationMax/9))
			{
				projectile.frame = 6;
			}
            else if (player.itemAnimation > (int)((float)player.itemAnimationMax/9))
			{
				projectile.frame = 7;
			}
            else
			{
				projectile.frame = 8;
			}
            projectile.position = (projectile.velocity + vector) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.direction == -1 ? 3.14f : 0);
            projectile.spriteDirection = projectile.direction;
            //player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            //player.itemTime = 10;
            //player.itemAnimation = 10;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
            return false;
        }
  		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
		{
			Player player = Main.player[projectile.owner];
            if (projectile.velocity.Y * player.gravDir > 0 && player.velocity.Y * player.gravDir > 0 && Math.Abs(projectile.velocity.X) < 2)
            {
		        player.velocity.Y = Math.Abs(player.velocity.Y) < 5 ? -5 * player.gravDir : -player.velocity.Y;
            }
		}
    }
}