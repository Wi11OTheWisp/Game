using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        bool goLeft, goRight;

        int enemySpriteCounter = 0;
        int enemyCounter = 100;
        int playerSpeed = 10;
        int score = 0;
        int hp = 100;

        int limit = 50;
        int enemySpeed = 10;

        Rect playerHitBox;

        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();


        Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus();

            gameTimer.Tick += GameTimerEvent;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            //ImageBrush playerImage = new ImageBrush();
            //playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            //player.Fill = playerImage;
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            if (goLeft == true && Canvas.GetLeft(player) > 5)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }
            if (goRight == true && Canvas.GetLeft(player) + (player.Width + 20) < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }
            //if (goUp == true && Canvas.GetTop(player) > 35)
            //{
            //   Canvas.SetTop(player, Canvas.GetTop(player) - playerSpeed);
            //}
            //if (goDown == true && Canvas.GetTop(player) + (player.Height + 43) < Application.Current.MainWindow.Height)
            //{
            //   Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);
            //}

            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            enemyCounter -= 1;

            Kills.Content = "Score: " + score;
            HP.Content = "HP: " + hp;
            
            if (enemyCounter < 0)
            {
                MakeEnemise();
                enemyCounter = limit;
            }

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);
                    //Canvas.SetTop(x, Canvas.GetTop(x) + 20);
                    //Canvas.SetLeft(x, Canvas.GetLeft(x) - 20);
                    //Canvas.SetLeft(x, Canvas.GetLeft(x) + 20);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemRemover.Add(x);
                    }

                    foreach(var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                                itemRemover.Add(x);
                                itemRemover.Add(y);
                                score++;

                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);
                    
                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        hp -= 10;
                    }
                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        itemRemover.Add(x);
                        hp -= 5;
                    }
                }
            }

            foreach (Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);
            }

            if (score > 5)
            {
                limit = 20;
                enemySpeed = 15;
            }
            if(hp <= 0)
            {
                gameTimer.Stop();
                HP.Content = "HP: 100";
                HP.Foreground = Brushes.Red;
                MessageBox.Show("ОК для повторной игры");

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }
            //if (e.Key == Key.Up)
            //{
            //    goUp = true;
            //}
            //if (e.Key == Key.Down)
            //{
            //   goDown = true;
            //}
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }
            //if (e.Key == Key.Up)
            //{
            //    goUp = false;
            //}
            //if (e.Key == Key.Down)
            //{
            //    goDown = false;
            //}

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 10,
                    Width = 5,
                    Fill = Brushes.Black,
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + 8);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) +8);

                MyCanvas.Children.Add(newBullet);
            }
        }




        private void MakeEnemise()
        {
            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 20,
                Width = 30,
                Fill = Brushes.Red
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rand.Next(75, 425));

            MyCanvas.Children.Add(newEnemy);
        }

    }
}
