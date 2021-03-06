using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Enums;

namespace JoostMod.Projectiles
{
	public class SunLaser : ModProjectile
	{
		private const int MAX_CHARGE = 80;
		private const float MOVE_DISTANCE = 0f;       //The distance charge particle from the player center
		private int sound = 0;
		public float Distance
		{
			get { return projectile.ai[0]; }
			set { projectile.ai[0] = value; }
		}

		public float Charge
		{
			get { return projectile.localAI[0]; }
			set { projectile.localAI[0] = value; }
		}

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sunbeam");
		}
		public override void SetDefaults()
		{
			projectile.width = 138;
			projectile.height = 138;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 9;
			projectile.magic = true;
			projectile.hide = true;
            drawHeldProjInFrontOfHeldItemAndArms = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (Charge == MAX_CHARGE)
			{
				Vector2 unit = projectile.velocity;
				DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], 
					projectile.Center, unit, 10, projectile.damage, 
					-1.57f, 1f, 1000f, Color.White, (int)MOVE_DISTANCE);
			}
			return false;

		}

		/// <summary>
		/// The core function of drawing a laser
		/// </summary>
		public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
		{
			Vector2 origin = start;
			float r = unit.ToRotation() + rotation;

			#region Draw laser body
			for (float i = transDist + 4; i <= Distance; i += step)
			{
				Color c = Color.White;
				origin = start + i * unit;
				spriteBatch.Draw(texture, origin - Main.screenPosition,
					new Rectangle(0, 154, 138, 16), i < transDist ? Color.Transparent : c, r,
					new Vector2(138 / 2, 16 / 2), scale, 0, 0);
			}
            #endregion

            Color c2 = Color.White;
            #region Draw laser tail
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
				new Rectangle(0, 0, 138, 154), c2, r, new Vector2(138 / 2, 154 / 2), scale, 0, 0);
            #endregion
            
            #region Draw laser head
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
				new Rectangle(0, 170, 138, 26), c2, r, new Vector2(138 / 2, 26 / 2), scale, 0, 0);
			#endregion
		}

		/// <summary>
		/// Change the way of collision check of the projectile
		/// </summary>
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (Charge == MAX_CHARGE)
			{
				Player p = Main.player[projectile.owner];
				Vector2 unit = projectile.velocity;
				float point = 0f;
				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + unit * Distance, 138, ref point))
				{
					return true;
				}
			}
			return false;
		}
        

		/// <summary>
		/// The AI of the projectile
		/// </summary>
		public override void AI()
		{

			Vector2 mousePos = Main.MouseWorld;
			Player player = Main.player[projectile.owner];

			#region Set projectile position
			if (projectile.owner == Main.myPlayer) // Multiplayer support
			{
				projectile.velocity = new Vector2(0, 1);
				projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
				projectile.netUpdate = true;
			}
            //projectile.position = (mousePos + projectile.velocity * MOVE_DISTANCE) - new Vector2(projectile.width/2, projectile.height/2);
            if (projectile.Distance(mousePos) > 5)
            {
                projectile.position = projectile.position + projectile.DirectionTo(mousePos) * 5;
            }
			projectile.timeLeft = 2;
			int dir = projectile.direction;
			player.ChangeDir(dir);
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
            #endregion

            #region Charging process
            // Kill the projectile if the player stops channeling
            if (!player.channel || player.dead || !player.active || player.noItems || player.CCed)
            {
				projectile.Kill();
			}
			else
			{
				if (Main.time % 16 < 1 && !player.CheckMana(player.inventory[player.selectedItem].mana, true))
				{
					projectile.Kill();
				}
				Vector2 offset = projectile.velocity;
				offset *= MOVE_DISTANCE - 20;
				Vector2 pos = projectile.Center + offset - new Vector2(40, 60);
				if (Charge == 0)
				{
					Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 13);
				}
				if (Charge < MAX_CHARGE)
				{
					Charge++;
				}
                if (Charge == MAX_CHARGE - 1)
                {
                    Main.PlaySound(42, (int)projectile.position.X, (int)projectile.position.Y, 56);
                }
				if (Charge >= MAX_CHARGE)
				{
					sound++;
				}
                if (sound >= 16)
                {
                    Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 15);
                    sound = 0;
                }
				int chargeFact = (int)(Charge / 20f);
				Vector2 dustVelocity = Vector2.UnitX * 18f;
				dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f, default(Vector2));
				Vector2 spawnPos = projectile.Center + dustVelocity;
				for (int k = 0; k < chargeFact + 1; k++)
				{
					Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - (chargeFact * 2));
					Dust dust = Main.dust[Dust.NewDust(pos, 80, 80, 178, projectile.velocity.X / 2f,
						projectile.velocity.Y / 2f, 0, default(Color), 1f)];
					dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
					dust.noGravity = true;
					dust.scale = Main.rand.Next(10, 20) * 0.05f;
				}
			}
			#endregion


			#region Set laser tail position and dusts
			if (Charge < MAX_CHARGE) return;
			Vector2 start = projectile.Center;
			Vector2 unit = projectile.velocity;
			unit *= -1;
			for (Distance = MOVE_DISTANCE; Distance <= 2200f; Distance += 5f)
			{
				start = projectile.Center + projectile.velocity * Distance;
				if (!Collision.CanHit(projectile.Center, 1, 1, start, 1, 1))
				{
					Distance -= 5f;
					break;
				}
			}

			Vector2 dustPos = projectile.Center + projectile.velocity * Distance;
			//Imported dust code from source because I'm lazy
			for (int i = 0; i < 2; ++i)
			{
				float num1 = projectile.velocity.ToRotation() + (Main.rand.Next(2) == 1 ? -1.0f : 1.0f) * 1.57f;
				float num2 = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
				Vector2 dustVel = new Vector2((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
				Dust dust = Main.dust[Dust.NewDust(dustPos + new Vector2(-65, 0), 130, 0, 178, dustVel.X, dustVel.Y, 0, new Color(), 1f)];
				dust.noGravity = true;
				dust.scale = 1.2f;
				// At this part, I was messing with the dusts going across the laser beam very fast, but only really works properly horizontally now
				dust = Main.dust[Dust.NewDust(player.Center + (new Vector2(40 * player.direction, -30)), 0, 0, 178, unit.X, unit.Y, 0, new Color(), 1f)];
				dust.fadeIn = 0f;
				dust.noGravity = true;
				dust.scale = 0.88f;
            }
            if (Main.rand.Next(5) == 0)
            {
                Vector2 offset = projectile.velocity.RotatedBy(1.57f, new Vector2()) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
                Dust dust = Main.dust[Dust.NewDust(dustPos + offset - Vector2.One * 4f, 8, 8, 31, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity = dust.velocity * 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);

                unit = dustPos - Main.player[projectile.owner].Center;
                unit.Normalize();
                dust = Main.dust[Dust.NewDust(player.Center + new Vector2(20 * player.direction, -20), 8, 8, 31, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity = dust.velocity * 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            }
            #endregion

            //Add lights
            DelegateMethods.v3_1 = new Vector3(1f, 1f, 0.4f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - MOVE_DISTANCE), 26, new Utils.PerLinePoint(DelegateMethods.CastLight));
		}

		public override bool ShouldUpdatePosition()
		{
			return false;
		}

		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 unit = projectile.velocity;
			Utils.PlotTileLine(projectile.Center, projectile.Center + unit * Distance, (projectile.width + 16) * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
		}
	}
}
