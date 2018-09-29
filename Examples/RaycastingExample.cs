using olc;
using OpenTK.Input;
using System;
using System.IO;

namespace Example
{
    class RaycastingExample : PixelGameEngine
    {
        private float playerX = 2;
        private float playerY = 2;

        private float dirX = -1;
        private float dirY = 0;

        private float planeX = 0;
        private float planeY = 0.66f;

        private int mapSize = 16;

        private string map;

        private Sprite brickTexture;

        public RaycastingExample()
        {
            appName = "Example";
        }

        protected override void OnUserCreate()
        {
            map += "################";
            map += "#--------------#";
            map += "#--------------#";
            map += "#----------#####";
            map += "#---##-----#---#";
            map += "#---##---------#";
            map += "#--------------#";
            map += "#----------#---#";
            map += "#----------#####";
            map += "######---------#";
            map += "#----#---------#";
            map += "#------------###";
            map += "#----#---------#";
            map += "#----#---------#";
            map += "#----#---#-----#";
            map += "################";

            string path = Directory.GetCurrentDirectory();
            brickTexture = new Sprite(path + "\\Examples\\Assets\\Brick.png");
        }

        protected override void OnUserUpdate(float time)
        {
            // Input
            var vert = 5f * time * (GetKey(Key.W) ? 1 : GetKey(Key.S) ? -1 : 0);
            var rot = 3 * time * (GetKey(Key.D) ? -1 : GetKey(Key.A) ? 1 : 0);

            // Movement
            playerX += dirX * vert;
            playerY += dirY * vert;

            if (map[(int)playerY * mapSize + (int)playerX] == '#' && vert != 0)
            {
                playerX -= dirX * vert;
                playerY -= dirY * vert;
            }

            // Rotation
            double oldDirX = dirX;
            double oldPlaneX = planeX;

            dirX = (float)(dirX * Math.Cos(rot) - dirY * Math.Sin(rot));
            dirY = (float)(oldDirX * Math.Sin(rot) + dirY * Math.Cos(rot));

            planeX = (float)(planeX * Math.Cos(rot) - planeY * Math.Sin(rot));
            planeY = (float)(oldPlaneX * Math.Sin(rot) + planeY * Math.Cos(rot));

            for (int x = 0; x < ScreenWidth(); x++)
            {
                double cameraX = 2 * x / (double)ScreenWidth() - 1;
                double rayDirX = dirX + planeX * cameraX;
                double rayDirY = dirY + planeY * cameraX;

                int mapX = (int)playerX;
                int mapY = (int)playerY;

                double sideDistX;
                double sideDistY;

                double deltaDistX = Math.Abs(1 / rayDirX);
                double deltaDistY = Math.Abs(1 / rayDirY);
                double perpWallDist;

                int stepX;
                int stepY;

                bool hit = false;
                int side = 0;

                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = ((double)playerX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - playerX) * deltaDistX;
                }

                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = ((double)playerY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - playerY) * deltaDistY;
                }

                while (!hit)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    if (map[mapY * mapSize + mapX] == '#')
                        hit = true;
                }

                if (side == 0)
                    perpWallDist = (mapX - (double)playerX + (1 - stepX) / 2) / rayDirX;
                else
                    perpWallDist = (mapY - (double)playerY + (1 - stepY) / 2) / rayDirY;

                int lineHeight = (int)(ScreenHeight() / perpWallDist);

                int drawStart = -lineHeight / 2 + ScreenHeight() / 2;
                if (drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + ScreenHeight() / 2;
                if (drawEnd >= ScreenHeight()) drawEnd = ScreenHeight() - 1;

                int texNum = map[mapY * mapSize + mapX] - 1;
                double wallX;
                if (side == 0) wallX = playerY + perpWallDist * rayDirY;
                else wallX = playerX + perpWallDist * rayDirX;
                wallX -= Math.Floor(wallX);

                int texX = (int)(wallX * 64.0);
                if (side == 0 && rayDirX > 0) texX = 64 - texX - 1;
                if (side == 1 && rayDirY < 0) texX = 64 - texX - 1;

                for (int y = 0; y < ScreenHeight(); y++)
                {
                    if (y > drawStart && y < drawEnd)
                    {
                        int d = y * 256 - ScreenHeight() * 128 + lineHeight * 128;
                        int texY = ((d * 64) / lineHeight) / 256;
                        Pixel col = brickTexture.GetPixel(texX, texY);
                        Draw(x, y, col);
                    }
                    else if (y < drawEnd)
                    {
                        Draw(x, y, new Pixel(135, 206, 235));
                    }
                    else
                    {
                        Draw(x, y, new Pixel(96, 128, 56));
                    }
                }
            }
        }
    }
}
