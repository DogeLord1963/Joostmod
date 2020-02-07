using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoostMod.Projectiles
{
    public class DragonBlaster : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dragon Blaster");
		}
        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 44;
            projectile.scale = 0.8f;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }
        public override bool? CanHitNPC(NPC target)
		{
			return false;
		}
        public override bool CanHitPvp(Player target)
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool PreAI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            float speed = 11;
            float shootSpeed = 13f;
            if (projectile.ai[0] == 1)
            {
                projectile.scale = 0.75f;
            }
            projectile.width = (int)(44f * projectile.scale);
            projectile.height = (int)(44f * projectile.scale);

            if (player.inventory[player.selectedItem].shoot == projectile.type)
            {
                shootSpeed = player.inventory[player.selectedItem].shootSpeed;
                speed = player.inventory[player.selectedItem].useTime;
                projectile.netUpdate = true;
            }
            bool channelling = (player.controlUseItem && projectile.ai[0] == 0) || (player.controlUseTile && projectile.ai[0] == 1);
            if (player.ownedProjectileCounts[projectile.type] < 2 && ((projectile.ai[0] == 0 && player.controlUseTile) || (projectile.ai[0] == 1 && player.controlUseItem && projectile.ai[1] > 1)))
            {
                player.ownedProjectileCounts[projectile.type] = 2;
                Projectile.NewProjectileDirect(vector, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 1 - projectile.ai[0]);
            }
            if (projectile.ai[1] <= 1)
            {
                projectile.ai[1]++;
            }
            if (projectile.localAI[1] <= 0 && Main.myPlayer == projectile.owner)
            {
                Vector2 vector13 = Main.MouseWorld - vector;
                vector13.Normalize();
                if (vector13.HasNaNs())
                {
                    vector13 = Vector2.UnitX * (float)player.direction;
                }
                if (vector13.X != projectile.velocity.X || vector13.Y != projectile.velocity.Y)
                {
                    projectile.netUpdate = true;
                }
                projectile.velocity = vector13;
            }
            if (player.noItems || player.CCed || player.dead || !player.active || (!channelling && projectile.localAI[1] <= 0 && projectile.localAI[0] <= 0 && projectile.ai[1] <= 2))
            {
                projectile.Kill();
            }
            else if (channelling && projectile.localAI[0] > 0)
            {
                projectile.ai[1]++;
                if (projectile.ai[1] >= 15 && projectile.ai[1] < 90)
                {
                    if (projectile.ai[1] % 15 == 0)
                    {
                        Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
                    }
                    if (Main.rand.NextBool(15 - (int)(projectile.ai[1] / 6)))
                    {
                        Dust.NewDustDirect(projectile.Center + projectile.velocity * 30 * projectile.scale - new Vector2(12, 12), 12, 12, 6).noGravity = true;
                    }
                }
                if (projectile.ai[1] == 90)
                {
                    Main.PlaySound(42, (int)projectile.position.X, (int)projectile.position.Y, 218);
                }
                if (projectile.ai[1] >= 90)
                {
                    Dust.NewDustDirect(projectile.Center + projectile.velocity * 30 * projectile.scale - new Vector2(12, 12), 12, 12, 6).noGravity = true;
                }
            }
            else if (projectile.localAI[0] <= 0)
            {
                if (projectile.ai[1] < 90)
                {
                    int type = 0;
                    Item item = new Item();
                    bool canShoot = false;
                    bool flag = false;
                    for (int i = 54; i < 58; i++)
                    {
                        if (player.inventory[i].ammo == AmmoID.Bullet && player.inventory[i].stack > 0)
                        {
                            item = player.inventory[i];
                            canShoot = true;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        for (int j = 0; j < 54; j++)
                        {
                            if (player.inventory[j].ammo == AmmoID.Bullet && player.inventory[j].stack > 0)
                            {
                                item = player.inventory[j];
                                canShoot = true;
                                break;
                            }
                        }
                    }
                    if (canShoot)
                    {
                        shootSpeed += item.shootSpeed;
                        type = item.shoot;
                        if (item.consumable)
                        {
                            player.ConsumeItem(item.type);
                        }
                        Projectile.NewProjectile(projectile.Center, projectile.velocity * shootSpeed, type, projectile.damage + item.damage, projectile.knockBack + item.knockBack, projectile.owner);
                        Main.PlaySound(2, projectile.Center, 41);
                    }
                    else
                    {
                        Main.PlaySound(SoundLoader.customSoundType, projectile.Center, mod.GetSoundSlot(SoundType.Custom, "Sounds/Custom/MissileClick"));
                    }
                }
                else
                {
                    //Projectile.NewProjectile(projectile.Center, projectile.velocity * shootSpeed, mod.ProjectileType("DragonBlast"), projectile.damage * 8, projectile.knockBack * 8, projectile.owner);
                    Main.PlaySound(42, projectile.Center, 217);
                }
                projectile.localAI[0] = 1;
                projectile.localAI[1] = speed;
                projectile.ai[1] = 2;
            }
            if (projectile.localAI[1] > 0 && projectile.localAI[0] > 0)
            {
                projectile.localAI[1]--;
                float kick = (projectile.localAI[1] < speed * 0.66f) ? projectile.localAI[1] : speed - projectile.localAI[1];
                float rot = projectile.velocity.ToRotation() - (5f * 0.0174f * projectile.direction * kick);
                projectile.rotation = rot + (projectile.direction == -1 ? 3.14f : 0);
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + (projectile.direction == -1 ? 3.14f : 0);
            }
            if (projectile.localAI[1] <= 0 && !channelling)
            {
                projectile.localAI[0] = 0;
            }
            projectile.position = (vector + projectile.velocity * 11 * (projectile.ai[0] * 0.8f + 0.5f)) - projectile.Size / 2f;
            projectile.spriteDirection = projectile.direction;
            player.ChangeDir(projectile.direction);
            if (projectile.ai[0] == 0)
            {
                player.heldProj = projectile.whoAmI;
            }
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects effects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);
            Color color = lightColor;
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, tex.Width, tex.Height)), color, projectile.rotation, drawOrigin, projectile.scale, effects, 0f);
            return false;
        }
    }
}