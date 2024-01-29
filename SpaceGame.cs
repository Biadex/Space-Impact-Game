using System;
using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Messy
{
    public class SpaceImpactGame
    {
        private Player _player;
        private Window _gameWindow;

        private Bullet _bullet = new Bullet();
        private List<Enemy> _enemyList = new List<Enemy>();
        private List<Enemy> _bossList = new List<Enemy>();

        private Enemy _enemy;
        private Bitmap background;
        private Bitmap background2;
        public bool enemyFlag = false;
        public bool bossFlag = false;
        int positionX = 0;
        int positionY = 0;
        int position = -740;
        static int hitCounter;
        static int hitCounter2;
        static int hitCounterBoss;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SpaceImpactGame()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _gameWindow = new Window("SpaceImpact", 800, 600);
            LoadAssets();
            _player = new Player { X = _gameWindow.Width - 80, Y = _gameWindow.Height - 80 };
            for (int i = 0; i < 1; i++)
            {
                _enemyList.Add(new LocalEnemy { X = 0 + 100 * i, Y = 50 });
                _bossList.Add(new Boss { X = 0 + 100 * i, Y = 50 });
            }
        }

        private void LoadAssets()
        {
            SplashKit.LoadBitmap("Gliese", "Gliese.png");
            SplashKit.LoadBitmap("Pegasi", "Pegasi.png");
            SplashKit.LoadBitmap("Bullet", "Fire.png");
            SplashKit.LoadBitmap("Aquarii", "Aquarii.png");
            SplashKit.LoadBitmap("Enemy", "Enemy1.png");
            background = SplashKit.LoadBitmap("Space1", "Space1.jpg");
            background2 = SplashKit.LoadBitmap("Space2", "Space2.jpg");
        }

        public void StartGame()
        {
            while (!_gameWindow.CloseRequested)
            {
                MoveEnemies(_gameWindow);
                SplashKit.ProcessEvents();
                KeepPlayerOnWindow(_gameWindow);

                if (SplashKit.KeyDown(KeyCode.UpKey))
                {
                    _player.Move(0, 4);
                }
                if (SplashKit.KeyDown(KeyCode.DownKey))
                {
                    _player.Move(0, -4);
                }
                if (SplashKit.KeyTyped(KeyCode.SpaceKey))
                {
                    _player.Shoot();
                }

                if (enemyFlag)
                {
                    foreach (var enemy in _enemyList)
                    {
                        enemy.Move(0, 10 * SplashKit.Rnd());
                    }
                }
                else
                {
                    foreach (var enemy in _enemyList)
                    {
                        enemy.Move(0, -10 * SplashKit.Rnd());
                    }
                }

                _player.Update();
                if (_enemyList.Count < 1)
                {
                    _enemyList.Add(new LocalEnemy { X = 0 + 50, Y = 50 });
                }
                foreach (var enemy in _enemyList)
                {
                    enemy.Update();
                }

                DrawGameScreen();

                CheckPlayerHit(_player);
                CheckEnemyHit(_enemyList);
            }
            _gameWindow.Close();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _gameWindow = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        private void DrawGameScreen()
        {
            positionY = position;
            SplashKitSDK.Timer timer = new SplashKitSDK.Timer("Background Timer");
            timer.Start();
            SplashKit.Delay(100);

            _gameWindow.Clear(Color.Black);
            if (timer.Ticks / 1000 < 1)
            {
                position += 2;
                if (position == 10)
                {
                    position = -740;
                }
            }
            _gameWindow.DrawBitmap(background, positionX, positionY);
            _player.Draw();
            foreach (var enemy in _enemyList)
            {
                enemy.Draw();
            }
            if (_player.score % 5 == 0)
            {
                _bossList.ElementAt(0).Draw();
            }
            _gameWindow.Refresh(60);
        }

        public void KeepPlayerOnWindow(Window window)
        {
            const double gap = 10;
            if (_player.X < gap)
            {
                _player.X = gap;
            }
            else if (_player.X > window.Width - 90)
            {
                _player.X = window.Width - 90;
            }
        }

        public void MoveEnemies(Window window)
        {
            const double gap = 10;
            foreach (var enemy in _enemyList)
            {
                if (enemy.X < gap)
                {
                    enemy.X = gap;
                    enemyFlag = false;
                    for (int i = 0; i < _enemyList.Count; i++)
                        _enemyList.ElementAt(i).Shoot();
                }
                else if (enemy.X > window.Width - 90)
                {
                    enemy.X = window.Width - 90;
                    enemyFlag = true;
                    for (int i = 0; i < _enemyList.Count; i++)
                        _enemyList.ElementAt(i).Shoot();
                }
            }
        }

        void CheckPlayerHit(Player player)
        {
            bool playerHit = false;
            int lifeLost;
            for (int i = 0; i < _enemyList.Count; i++)
            {
                foreach (var bullet in _enemyList[i]._bulletList)
                {
                    if (bullet.CollidedWith(player, bullet))
                    {
                        playerHit = true;
                        break;
                    }
                }
            }

            if (playerHit)
            {
                playerHit = false;
                hitCounter++;
                if (hitCounter > 4)
                {
                    lifeLost = player._life--;
                    hitCounter = 0;
                    if (lifeLost <= 0)
                    {
                        SplashKit.CloseCurrentWindow();
                    }
                    else
                    {
                        player.RemoveLife(lifeLost);
                    }
                }
            }
        }

        void CheckEnemyHit(List<Enemy> enemyList)
        {
            List<Enemy> removeEnemies = new List<Enemy>();
            bool enemyHit = false;

            foreach (var currentEnemy in _enemyList)
            {
                foreach (var bullet in _player._bulletList)
                {
                    if (bullet.CollidedWith(currentEnemy, bullet))
                    {
                        enemyHit = true;
                        _player.score++;
                        removeEnemies.Add(currentEnemy);
                        break;
                    }
                }
            }

            if (enemyHit)
            {
                enemyHit = false;
                hitCounter2++;
                if (hitCounter2 > 3)
                {
                    _player.score++;
                    hitCounter2 = 0;
                }
            }

            foreach (var enemyToRemove in removeEnemies)
            {
                enemyList.Remove(enemyToRemove);
            }
        }
    }
}
