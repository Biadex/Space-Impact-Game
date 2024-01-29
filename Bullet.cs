using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Messy
{
    public class Bullet
    {
        private Bitmap _bulletBitmap;
        private double _x, _y, _angle;
        private bool _active = false;

        public List<Enemy> RemoveEnemy = new List<Enemy>();

        public Bullet(double x, double y, double angle)
        {
            _bulletBitmap = SplashKit.BitmapNamed("Bullet");
            _x = x - _bulletBitmap.Width / 2;
            _y = y - _bulletBitmap.Height / 2;
            _angle = angle;
            _active = true;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Bullet()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _active = false;
        }

        public void Update()
        {
            const int TOAST = 8;
            Vector2D movement = new Vector2D();
            Matrix2D rotation = SplashKit.RotationMatrix(_angle);
            movement.X += TOAST;
            movement = SplashKit.MatrixMultiply(rotation, movement);
            _x += movement.X;
            _y += movement.Y;

            if ((_x > SplashKit.ScreenWidth() || _x < 0) || _y > SplashKit.ScreenHeight() || _y < 0)
            {
                _active = false;
            }
        }

        public void Draw()
        {
            if (_active)
            {
                DrawingOptions options = SplashKit.OptionRotateBmp(_angle);
                _bulletBitmap.Draw(_x, _y, options);
            }
        }

        public Circle CollisionCircle
        {
            get
            {
                return SplashKit.CircleAt(_x - 26, _y, 200);
            }
        }

        public bool CollidedWith(Player player, Bullet bullet)
        {
            bullet.Update();
            return _bulletBitmap.BitmapCollision(_x, _y, player._shipBitmap, player.X, player.Y);
        }

        public bool CollidedWith(Enemy enemy, Bullet bullet)
        {
            bullet.Update();
            return _bulletBitmap.BitmapCollision(_x, _y, enemy._shipBitmap, enemy.X, enemy.Y);
        }
    }
}
