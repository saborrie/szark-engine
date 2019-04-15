using System;
using Szark;

namespace Example
{
    class PongExample : SzarkEngine
    {
        private const float playerSpeed = 500;
        private const float enemyDifficulty = 4;

        private float playerY, enemyY;
        private Vector ballPos, ballVel;

        private int playerScore, enemyScore;
        private bool ballShot;

        private Sprite paddle, line, ball;
        private Text text;

        PongExample() : base("Pong Example",
            1280, 720) { }

        protected override void Start()
        {
            paddle = new Sprite(16, 128);
            paddle.ClearToColor(Color.White);

            line = new Sprite(8, Height);
            line.ClearToColor(Color.White);

            ball = new Sprite(16, 16);
            ball.ClearToColor(Color.White);

            text = new Text("Arial", 32);
        }

        protected override void Update(float deltaTime)
        {
            /* Player Code */

            playerY += deltaTime * playerSpeed * 
                (Input.GetKey("W") ? 1 : Input.GetKey("S") ? -1 : 0);
            playerY = Mathf.Clamp(playerY, -Height + 144, Height - 144);

            if (!ballShot && Input.GetKeyDown("Space"))
            {
                ballShot = true;
                ballVel.x = 800;
                ballVel.y = new Random().Next(-800, 800);
            }

            /* Enemy Code */

            enemyY = Mathf.Lerp(enemyY, ballPos.y, enemyDifficulty * deltaTime);
            enemyY = Mathf.Clamp(enemyY, -Height + 144, Height - 144);

            /* Ball Code */

            ballPos += ballVel * deltaTime;
            ballPos.x = ballShot ? ballPos.x : -Width + 128;
            ballPos.y = ballShot ? ballPos.y : playerY;

            if (ballPos.y > Height - 16 || ballPos.y < -Height + 16)
                ballVel.y *= -1;

            bool hitEnemyPaddle = ballPos.y < enemyY + 128 &&
                ballPos.y > enemyY - 128 && ballPos.x > Width - 88;
            bool hitPlayerPaddle = ballPos.y < playerY + 128 &&
                ballPos.y > playerY - 128 && ballPos.x < -Width + 88;

            if (hitEnemyPaddle || hitPlayerPaddle)
            {
                ballVel.x *= -1;
                ballVel.y += new Random().Next(-250, 250);
            }

            if (ballPos.x > Width)
            {
                ballShot = false;
                playerScore++;
            }

            if (ballPos.x < -Width)
            {
                ballShot = false;
                enemyScore++;
            }
        }

        protected override void Draw(float deltaTime)
        {
            paddle.Render(-Width + 64, playerY);
            paddle.Render(Width - 64, enemyY);
            ball.Render(ballPos.x, ballPos.y);
            line.Render(0, 0);
            text.DrawString(playerScore.ToString(), -64, Height - 128, 1);
            text.DrawString(enemyScore.ToString(), 64, Height - 128, 1);
        }

        protected override void Destroyed() { }

        static void Main() => new PongExample();
    }
}