using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoostMod.Projectiles
{
	public class Flamethrown : ModProjectile
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame");
		}
		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.aiStyle = 2;
			projectile.friendly = true;
			projectile.thrown = true;
			projectile.ignoreWater = false;
			projectile.penetrate = 1;
			projectile.timeLeft = 250;
			projectile.alpha = 35;
			projectile.light = 0.15f;
			projectile.extraUpdates = 1;
			aiType = ProjectileID.Shuriken;
		}
		public override void AI()
		{
			if(Main.tile[(int)projectile.Center.ToTileCoordinates().X, (int)projectile.Center.ToTileCoordinates().Y].liquid > 80 && (Main.tile[(int)projectile.Center.ToTileCoordinates().X, (int)projectile.Center.ToTileCoordinates().Y].liquidType() == 0 || Main.tile[(int)projectile.Center.ToTileCoordinates().X, (int)projectile.Center.ToTileCoordinates().Y].liquidType() == 2))
			{
				projectile.Kill();
			}
		}
		public override bool? CanHitNPC(NPC target)
		{
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("Flame2thrown"), projectile.damage, 0, projectile.owner);	
		}

	}
}


