using Terraria.ID;
using Terraria.ModLoader;

namespace JoostMod.Projectiles
{
	public class Rain : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rain");
		}
		public override void SetDefaults()
		{
			projectile.width = 2;
			projectile.height = 40;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.thrown = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
			aiType = ProjectileID.Bullet;
		}
		public override void AI()
		{
			projectile.rotation = 0;
		}
	}
}

