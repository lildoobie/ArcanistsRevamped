using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Testbed.Tests;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ArcanistsRevamped
{
    public class GroundTerrain : Terrain
    {
        public float circleRadius = 2.5f;
        private Terrain groundTerrain;
        private AABB groundTerrainArea;
        public Texture2D groundTexture;

        public GroundTerrain(World world, AABB aabb) : base(world, aabb)
        {
            groundTerrainArea = aabb;
            groundTerrain = new Terrain(world, groundTerrainArea)
            {
                PointsPerUnit = 10,
                CellSize = 50,
                SubCellSize = 5,
                Decomposer = TriangulationAlgorithm.Earclip,
                Iterations = 2,
            };

            groundTerrain.Initialize();
        }

        private void Initialize(Texture2D texture)
        {
            groundTexture = texture;
            Color[] colorData = new Color[groundTexture.Width * groundTexture.Height];
            
            groundTexture.GetData(colorData);

            sbyte[,] data = new sbyte[groundTexture.Width, groundTexture.Height];

            for (int y = 0; y < groundTexture.Height; y++)
            {
                for (int x = 0; x < groundTexture.Width; x++)
                {
                    //If the color on the coordinate is black, we include it in the terrain.
                    bool inside = colorData[(y * groundTexture.Width) + x] == Color.Black;

                    if (!inside)
                        data[x, y] = 1;
                    else
                        data[x, y] = -1;
                }
            }

            groundTerrain.ApplyData(data, new Vector2(250, 250));

            //Initialize(texture);
        }

        public void DrawCircleOnMap(Vector2 center, sbyte value)
        {
            for (float by = -circleRadius; by < circleRadius; by += 0.1f)
            {
                for (float bx = -circleRadius; bx < circleRadius; bx += 0.1f)
                {
                    if ((bx * bx) + (by * by) < circleRadius * circleRadius)
                    {
                        float ax = bx + center.X;
                        float ay = by + center.Y;
                        groundTerrain.ModifyTerrain(new Vector2(ax, ay), value);
                    }
                }
            }
        }
    }
}